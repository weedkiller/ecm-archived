window.gridSelectAcrossPagesText = 'Click here to select all records across the pages.';
window.gridDeselectAcrossPagesText = "Clear all selection.";

window.tinymceOptions = {
    selector: "textarea.editor",
    skin: "lightgray",

    content_css: ["/Assets/Libraries/bootstrap/css/bootstrap.min.css", "/Assets/Front/css/style.css"],

    plugins: [
        "advlist autolink autosave link image lists charmap print preview hr anchor pagebreak spellchecker",
        "searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking",
        "table contextmenu directionality emoticons template textcolor paste fullpage textcolor moxiemanager"
    ],

    toolbar1: "styleselect formatselect fontselect fontsizeselect | cut copy paste undo redo",
    toolbar2: "bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | bullist numlist | outdent indent blockquote | forecolor backcolor | searchreplace",
    toolbar3: "table | hr removeformat | subscript superscript | charmap emoticons | print fullscreen | ltr rtl | visualchars visualblocks nonbreaking template pagebreak restoredraft",
    toolbar4: "spellchecker | link unlink anchor image media code",

    menubar: false,
    toolbar_items_size: 'small',

    style_formats: [
        { title: 'Bold text', inline: 'strong' },
        { title: 'Italic text', inline: 'em' },
        { title: 'Red text', inline: 'span', styles: { color: '#ff0000' } },
        { title: 'Red header', block: 'h1', styles: { color: '#ff0000' } },
        { title: 'Lead Cap', block: 'p', classes: 'lead' },
        { title: 'Well', block: 'p', classes: 'well' },
        { title: 'Well Pink', block: 'p', classes: 'well pink' }
    ],

    forced_root_block: "",
    force_p_newlines: true,
    force_br_newlines: false,
    convert_urls: false,
    //relative_urls: true,
    remove_script_host: false
};



jQuery(document).ready(function ($) {

    if (window.menuClassName) {

        var $li = $('.page-sidebar-menu li.' + window.menuClassName).addClass('active');
        var $parentLi = $li.parents('li');
        $parentLi.addClass('active');
    }
    else {
        $('.page-sidebar-menu > li:first-child').addClass('active');
    }

    initKendoGridEvents();
});

function initKendoGridEvents() {
    var $grids = $('.k-grid');
    $grids.each(function (i, g) {
        var $grid = $(g);
        var grid = $(g).data('kendoGrid');
        grid.bind('dataBound', function (e) {
            $grid.find('.k-grid-content input.checkbox').off('change').on('change', function (e) {
                if ($(this).closest('tr').length > 0) {
                    if (this.checked) {
                        $(this).closest('tr').addClass('k-state-selected');
                    } else {
                        $(this).closest('tr').removeClass('k-state-selected');
                    }
                }
            });
            $grid.find('.k-header input.checkbox').off('change').on('change', function (e) {
                var isChecked = this.checked;
                $('.k-grid-content input.checkbox').each(function () {
                    this.checked = isChecked;
                    $(this).change();
                });

                if ($grid.find('.k-grid-preheader').length <= 0) {
                    $grid.prepend('<div class="k-grid-preheader hide"><a href="javascript:void(0);" class="select-all-link hide">' + window.gridSelectAcrossPagesText + '</a><a href="javascript:void(0);" class="deselect-all-link hide">' + window.gridDeselectAcrossPagesText + '</a></div>');
                }

                var preheader = $grid.find('.k-grid-preheader');

                if (isChecked) {
                    var totalRows = grid.dataSource.total();
                    var totalPages = grid.dataSource.totalPages();
                    if (totalPages > 1) {
                        preheader.removeClass('hide');
                        preheader.find('.select-all-link')
                            .text(gridSelectAcrossPagesText + " (total " + totalRows + " records)")
                            .removeClass('hide')
                            .unbind('click').bind('click', function (e) {
                                $(this).addClass('hide');
                                preheader.find('.deselect-all-link').removeClass('hide');
                                $grid.data('select-all', true);
                                console.log($grid);
                            });
                        preheader.find('.deselect-all-link').addClass('hide').unbind('click').bind('click', function (e) {
                            preheader.addClass('hide');
                            $(this).addClass('hide');
                            preheader.find('.select-all-link').removeClass('hide');
                            $grid.find('.k-header input.checkbox').prop('checked', false).trigger('change').uniform.update();
                            $grid.data('select-all', false);
                        });
                    }
                } else {
                    preheader.addClass('hide');
                    preheader.find('.select-all-link').removeClass('hide');
                    $('.k-grid-content input.checkbox').prop('checked', false);
                }
            });
            grid.unbind('change').bind('change', function (e) {
                $('.k-grid-content input.checkbox', g).prop('checked', false);
                var rows = grid.select();
                rows.find('input.checkbox').prop('checked', true);
            });
        });
    });
}

function getUrl(url) {
    var full_url = window.location.protocol + "//" + window.location.host + "/";
    return full_url + url;
}

function initGeolocation(callback) {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(callback);
    }
}

function showLoader(isShow, selector, customCss) {
    if (selector === undefined || selector === null) {
        selector = 'body';
    }

    let $selector = jQuery(selector);
    $selector.each(function () {
        let $this = jQuery(this);
        if (isShow) {
            let $overlay = $this.data('loaderOverlay');
            if (!$this.data('loaderOverlay')) {
                $overlay = jQuery('<div class="loaderOverlay"><div class="loaderWrapper"><div class="loaderAnimation"></div></div></div>').appendTo('body');
                $this.data('loaderOverlay', $overlay);
            }
            var $animation = $overlay.find('.loaderAnimation');

            // Width & Height
            if (selector === 'body') {
                let width = $(window).outerWidth();
                let height = $(window).outerHeight();
                let offset = $this.offset();
                $overlay.css({
                    'width': width + 'px',
                    'height': height + 'px',
                    'top': offset.top + 'px',
                    'left': offset.left + 'px',
                    'position': 'fixed'
                });

                let length = width < height ? width : height;
                let animateLength = length * 0.2;

                $animation.css({
                    'width': animateLength + 'px',
                    'height': animateLength + 'px',
                    'top': ((height - animateLength) / 2) + 'px',
                    'left': ((width - animateLength) / 2) + 'px'
                });
                if (customCss) {
                    $animation.css(customCss);
                }

                $overlay.fadeIn();
            } else {
                let width = $this.outerWidth();
                let height = $this.outerHeight();
                let offset = $this.offset();
                $overlay.css({
                    'width': width + 'px',
                    'height': height + 'px',
                    'top': offset.top + 'px',
                    'left': offset.left + 'px'
                });

                let length = width < height ? width : height;
                let animateLength = length * 0.3;

                $animation.css({
                    'width': animateLength + 'px',
                    'height': animateLength + 'px',
                    'top': ((height - animateLength) / 2) + 'px',
                    'left': ((width - animateLength) / 2) + 'px'
                });
                if (customCss) {
                    $animation.css(customCss);
                }

                $overlay.fadeIn();
            }
        } else {
            let $overlay = $this.data('loaderOverlay');
            if ($overlay) {
                $overlay.fadeOut().animate({ 'dummy': '1' }, 100).remove();
                $this.removeData('loaderOverlay');
            }
        }
    });
}

function showMessageModal(title, body) {
    $('#message-modal .modal-title').text(title);
    $('#message-modal .modal-body').text(body);
    $('#message-modal').modal('show');
}

/**
 * Initialize and get GrapesJS Editor.
 * @param {any} container Container's Selector
 * @param {any} options Initializing options
 * @returns {any} GrapesJS Editor Instance.
 */
function initEditor(container, options) {
    let editor = grapesjs.init({
        container: container,
        fromElement: true,
        autorender: false,
        inlineCss: true,
        storageManager: {
            autoload: 0,
        },
        assetManager: {
            uploadText: 'Double Click/Drag images to this area to start uploading.',
            upload: options.uploadUrl,
            autoAdd: true
        },
        plugins: ['gjs-mjml', 'gjs-plugin-ckeditor'],
        pluginsOpts: {
            'gjs-mjml': {
                importPlaceholder: '<mjml><mj-body><mj-container></mj-container></mj-body></mjml>'
            },
            'gjs-plugin-ckeditor': {
                position: 'center',
                options: {
                    startupFocus: true,
                    extraAllowedContent: '*(*);*{*}', // Allows any class and any inline style
                    allowedContent: true, // Disable auto-formatting, class removing, etc.
                    enterMode: CKEDITOR.ENTER_BR,
                    extraPlugins: 'sharedspace,justify,colorbutton,panelbutton,font',
                    toolbar: [
                        { name: 'styles', items: ['Font', 'FontSize'] },
                        ['Bold', 'Italic', 'Underline', 'Strike'],
                        { name: 'paragraph', items: ['NumberedList', 'BulletedList'] },
                        { name: 'links', items: ['Link', 'Unlink'] },
                        { name: 'colors', items: ['TextColor', 'BGColor'] },
                    ],
                }
            }
        },
        style: `
            div, td {
                font-family: "MyriadPro-Regular, 'Myriad Pro Regular', MyriadPro, 'Myriad Pro', Helvetica, Arial, sans-serif";
                font-size: 1em;
                color: #000;
            }
            img {
                max-width: 100%;
                vertical-align: top;
                display: inline-block;
           }
            editable {
                min-height: 100px;
                width: 100%;
                display: block;
            }
        `
    });

    var pnm = editor.Panels;
    pnm.addButton('options', [{
        id: 'undo',
        className: 'fa fa-undo icon-undo',
        command: 'undo',
        attributes: { title: 'Undo (CTRL/CMD + Z)' }
    }, {
        id: 'redo',
        className: 'fa fa-repeat icon-redo',
        command: 'redo',
        attributes: { title: 'Redo (CTRL/CMD + SHIFT + Z)' }
    }, {
        id: 'clean-all',
        className: 'fa fa-trash icon-blank',
        command: 'clean-all',
        attributes: { title: 'Empty canvas' }
    }]);

    let comps = editor.DomComponents;
    let components = comps.getComponents();

    // Get the model and the view from the default Component type
    var defaultType = comps.getType('default');
    var defaultModel = defaultType.model;
    var defaultView = defaultType.view;

    // The `gjs-editable` will be the Component type ID
    comps.addType('editable', {
        // Define the Model
        model: defaultModel.extend({
            // Extend default properties
            defaults: Object.assign({}, defaultModel.prototype.defaults, {
            })
        },
            {
                isComponent: function (el) {
                    if (el.tagName === 'EDITABLE') {
                        return { type: 'editable' };
                    }
                },
            }),

        // Define the View
        view: defaultType.view.extend({

            // The render() should return 'this'
            render: function () {
                // Extend the original render method
                defaultType.view.prototype.render.apply(this, arguments);
                //this.el.style.minHeight = '50px';
                //this.el.display = 'block';
                return this;
            },
        }),
    });

    const am = editor.AssetManager;
    if (options.assetUrl && am.getAll().length === 0) {
        jQuery.get(options.assetUrl, function (result) {
            if (result.isSuccess) {
                for (var asset of result.data) {
                    am.add(asset);
                }
                if (!window.editorLimitAccess) {
                    const assets = am.getAll();
                    assets.on('remove', function (asset, e) {
                        jQuery.ajax({
                            url: options.deleteUrl,
                            type: 'POST',
                            dataType: 'json',
                            data: {
                                file: asset.get('src')
                            },
                            success: function (result) {
                                console.log('file removed');
                            },
                            error: function (xhr) {
                                console.error(xhr);
                            }
                        })
                    });
                }
            }
        });
    }

    let bm = editor.BlockManager;
    bm.add('mj-image', {
        label: 'Image',
        category: 'Content',
        content: '<mj-image src="http://placehold.it/350x250/78c5d6/fff"></mj-image>',
        attributes: {
            class: 'fa fa-image'
        }
    });

    if (!window.editorLimitAccess) {
        bm = editor.BlockManager;
        bm.add('editable_area', {
            label: 'Editable Area',
            content: '<editable></editable>',
            attributes: {
                class: 'fa fa-pencil'
            }
        });
    } else {
        const limitAll = model => {
            model.set({ editable: false, removable: false, draggable: false, copyable: false });
            model.set('toolbar', []);
            if (model.attributes.tagName !== 'editable') {
                model.get('components').each(model => {
                    limitAll(model);
                });
            }
        };
        components.forEach(c => limitAll(c));
    }

    editor.render();
    return editor;
}

function removeSelfClosingTags(selfClosingMJML) {
    let split = selfClosingMJML.split("/>");
    let convertedHtml = "";
    for (let i = 0; i < split.length - 1; i++) {
        let edsplit = split[i].split("<");
        convertedHtml += split[i] + "></" + edsplit[edsplit.length - 1].split(" ")[0] + ">";
    }
    return convertedHtml + split[split.length - 1];
}

function finalizeHTML(container, html) {
    let $this = jQuery(`${container}`).html(html);
    let $images = $this.find('img');
    $images.each(function () {
        let $this = jQuery(this);
        let width = $this.outerWidth();
        $this
            .attr('width', `${width}px`)
            .css('width', `${width}px`);
    });
    return jQuery(`${container}`).html();
}