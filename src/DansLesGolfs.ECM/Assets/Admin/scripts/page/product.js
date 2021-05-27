/**
* On document ready.
*/
jQuery(document).ready(function ($) {
    $('.info-form').validate();

    tinymce.init(window.tinymceOptions);

    // On Add Modifier Button is clicked.
    $('#btn-add-modifier').click(onBtnAddModifierClicked);

    // On Save Modifiers Button is clicked.
    $('#btn-save-modifiers').click(onBtnSaveModifiersClicked);

    $('input[name=ItemName]').blur(function (e) {
        if ($('#ItemSlug').val() == '' || window.itemId <= 0)
            generateItemSlugByItemName();
    });

    $('#ItemSlug').change(function (e) {
        if ($('#ItemSlug').val() == '') {
            generateItemSlugByItemName();
        } else {
            regenerateItemSlug();
        }
    });

    // Make sortable area.
    $('#product-images-area').sortable({
        update: function (e, ui) {
            saveAllItemImages();
        }
    });
    $('#item-modifiers-list').sortable({
        update: function (e, ui) {
            reorderModifiers();
        }
    });

    // Init uploadifive elements.
    $('#product-image-file').uploadifive({
        formData: { id: window.itemId },
        multi: true,
        auto: true,
        buttonText: "Add Images...",
        buttonClass: "btn btn-default",
        fileTypeExts: '*.gif; *.jpg; *.png',
        fileSizeLimit: '4MB',
        uploadScript: getUrl('Admin/Item/AddItemImages'),
        swf: getUrl('Assets/Libraries/uploadify/uploadify.swf'),
        onUploadComplete: function (file, data) {
            var formData = data.split(',');
            addItemImage(formData[0], formData[1], formData[2], formData[3], formData[4], formData[5], formData[6]);
        }
    });

    if ($('#CourseId').length > 0) {
        $('#SiteId').change(onSiteIdChanged);
    }

    //initPriceDataTable();
    $('.add-price-button').click(onAddPriceButtonIsClicked);
    addDeletePriceEvent($('.price-datatable tbody tr .delete-link'));

    // Setup DatePicker
    $('.datepicker').datepicker({
        dateFormat: 'dd/mm/yy',
        autoSize: true
    });
});

function initPriceDataTable() {
    window.dataTable = $('.price-datatable').dataTable({
        "aaSorting": [[0, "asc"]],
        "bProcessing": false,
        "aoColumnDefs": [
            {
                'iWidth': 20,
                'bSortable': false,
                'aTargets': [0, window.columnLength - 2, window.columnLength - 1]
            }
        ],
        "fnDrawCallback": onDataTableDrawn
    });
}

function onDataTableDrawn() {
    $('.price-datatable tbody .delete-link').on('click', function (e) {
        if (confirm(window.confirmDeleteText)) {
            var id = $(this).data('id');
            deleteItems([id]);
        }
    });
}

function onAddPriceButtonIsClicked(e) {
    var $this = $(this);
    var $parent = $this.parents('.pricing-box');
    var $table = $parent.find('.price-datatable');
    var priceType = $parent.data('price-type');
    var date = new Date();
    var day = date.getDate();
    day = day < 10 ? "0" + day : day;
    var month = date.getMonth() + 1;
    month = month < 10 ? "0" + month : month;
    var dateText = day + "/" + month + "/" + date.getFullYear();
    var html = '<tr data-id="@price.ItemPriceId">' +
                '<td class="checkboxes column-checkbox"><input type="checkbox" /></td>' +
                '<td class="column-price"><input type="text" class="text-right number" min="0" name="SpecialPrices" value="0" /><input type="hidden" name="PriceTypes" value="' + priceType + '" /></td>' +
                '<td class="column-start"><input type="text" name="PriceStartDates" value="' + dateText + '" class="datepicker input-small" placeholder="dd/mm/yyyy" /></td>' +
                '<td class="column-end"><input type="text" name="PriceEndDates" value="' + dateText + '" class="datepicker input-small" placeholder="dd/mm/yyyy" /></td>' +
                '<td class="column-delete"><a href="javascript:void(0)" class="delete-link">' + window.deleteText + '</a></td>' +
                '</tr>';
    $table.find('tbody .column-empty').remove();
    $table.find('tbody').append(html);
    $table.find('tbody tr:last-child .datepicker').datepicker({
        dateFormat: 'dd/mm/yy',
        autoSize: true
    });
    addDeletePriceEvent($table.find('tbody tr:last-child .delete-link'));
}

function addDeletePriceEvent($selector) {
    $selector.click(function () {
        if (!confirm(window.confirmDeleteText))
            return;

        jQuery(this).closest('tr').remove();
    });
}

/************************* Item Info *************************/
function generateItemSlug(text) {
    jQuery.ajax({
        url: getUrl('Admin/Item/GenerateItemSlug'),
        data: { text: text, skipId: window.itemId },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            if (result.isSuccess) {
                jQuery('#ItemSlug').val(result.slug);
            } else {
                console.error(result.message);
            }
        }
    });
}
function generateItemSlugByItemName() {
    generateItemSlug(jQuery('input[name=ItemName]').val());
}
function regenerateItemSlug() {
    generateItemSlug(jQuery('#ItemSlug').val());
}

function onSiteIdChanged(e) {
    var siteId = jQuery('#SiteId').val();
    jQuery.ajax({
        url: getUrl('Common/AjaxGetCoursesBySiteId'),
        data: { siteId: siteId },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            if (result.isResult) {
                jQuery('#CourseId').html('');
                for (var i in result.list) {
                    jQuery('#CourseId').append('<option value="' + result.list[i].CourseId + '">' + result.list[i].CourseName + '</option>');
                }
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
        }
    });
}

/************************* Item Images *************************/
/**
* Add Item Image
* @param int imageId        ID of Image.
* @param string url         URL
* @param string thumbUrl    URL of thumbnail.
* @param string filename    Name of file.
* @param string thumbname   Name of thumbnail file.
* @param string basename    Basename of file.
* @param string extension   File extension.
*/
function addItemImage(imageId, url, thumbUrl, filename, thumbname, basename, extension) {
    var obj = {
        ItemImageId: imageId,
        ItemId: window.itemId,
        ImageName: filename,
        ListNo: $('#product-images-area .product-image').length,
        BaseName: basename,
        FileExtension: extension,
        ImageUrl: url,
        ThumbnailUrl: thumbUrl
    };
    var html = getItemImageHTML(obj);
    $("#product-images-area").append(html);
    $('#product-images-area .product-image:last-child .delete').click(onItemImageDelete);
}

/**
* Get Item Image HTML from Item Image Object.
* @param object obj         Item Image object.
* @return string            Item Image HTML elements.
*/
function getItemImageHTML(obj) {
    var html = '<li class="product-image image-item" data-imageid="' + obj.ItemImageId + '" data-url="' + obj.ImageUrl + '" data-thumb-url="' + obj.ThumbnailUrl + '" data-filename="' + obj.ImageName + '" data-basename="' + obj.BaseName + '" data-extension="' + obj.FileExtension + '">';
    html += '<div class="tools-panel">';
    html += '<a href="javascript:void(0);" class="delete">Delete</a>';
    html += '</div>';
    html += '<div class="thumbnail" style="background-image:url(' + obj.ThumbnailUrl + ')">';
    html += '</div>';
    html += '</li>';
    return html;
}

function saveAllItemImages() {
    var imageIds = new Array();
    var fileNames = new Array();
    var baseNames = new Array();
    var fileExtensions = new Array();
    $('#product-images-area .product-image').each(function (i) {
        imageIds.push($(this).data('imageid'));
        fileNames.push($(this).data('filename'));
        baseNames.push($(this).data('basename'));
        fileExtensions.push($(this).data('extension'));
    });

    $.ajax({
        url: getUrl('Admin/Item/SaveItemImages'),
        type: 'post',
        dataType: 'json',
        traditional: true,
        data: { id: window.itemId, imageIds: imageIds, fileNames: fileNames, baseNames: baseNames, fileExtensions: fileExtensions },
        success: function (result) {
            if (result.isSuccess) {
                // Do nothing.
            } else {
                console.error(result.message);
            }
        }
    });
}

function loadItemImages() {
    $.ajax({
        url: getUrl('Admin/Item/LoadItemImages'),
        type: 'post',
        dataType: 'json',
        traditional: true,
        data: { id: window.itemId },
        success: function (result) {
            if (result.isSuccess) {
                var html = "";
                for (var i in result.itemImages) {
                    html += getItemImageHTML(result.itemImages[i]);
                }
                $('#product-images-area').html(html);
                $('#product-images-area .product-image .delete').on('click', onItemImageDelete);
            } else {
                console.error(result.message);
            }
        }
    });
}

function onItemImageDelete(e) {
    if (confirm("Your image will be deleted permanently. Are you sure for deleting it?")) {
        var $parent = $(this).parents('.product-image');
        var itemImageId = $parent.data('imageid');
        $.ajax({
            url: getUrl('Admin/Item/DeleteItemImage'),
            type: 'post',
            dataType: 'json',
            data: { id: window.itemId, itemImageId: itemImageId },
            success: function (result) {
                if (result.isSuccess) {
                    $parent.remove();
                    saveAllItemImages();
                } else {
                    console.error(result.message);
                }
            }
        });
    }
}

/************************* Item Modifiers *************************/
function loadItemModifiers() {
    $('#item-modifiers-list').html("");
    $.ajax({
        url: getUrl('Admin/Item/LoadItemModifiers'),
        type: 'post',
        dataType: 'json',
        traditional: true,
        data: { id: window.itemId },
        success: function (result) {
            if (result.isSuccess) {
                for (var i in result.itemModifiers) {
                    $('#item-modifiers-list').append(getModifierHTML(result.itemModifiers[i]));
                    bindEventLastModifier();
                }
            } else {
                console.error(result.message);
            }
            checkItemModifiersList();
        }
    });
}
function onBtnAddModifierClicked(e) {
    var $ddl = $('#Modifiers');
    var modifier = {
        modifierId: $('#Modifiers').val(),
        modifierName: $('#Modifiers option:selected').text()
    };
    addModifier(modifier);
}
function addModifier(modifier) {
    $.post(getUrl('Admin/Item/AddItemModifier'), { id: window.itemId, modifierId: modifier.modifierId }, function (result) {
        if (result.isSuccess) {
            var html = getModifierHTML(result.modifier);
            $('#item-modifiers-list').append(html);
            bindEventLastModifier();
            checkItemModifiersList();
        } else {
            console.error(result.message);
        }
    }, 'json');
}
function getModifierHTML(modifier) {
    var html = '<li class="item-modifier panel panel-default" data-id="' + modifier.modifierId + '" data-control="' + modifier.controlType + '" data-active="1">';
    html += '<div class="panel-heading">';
    html += '<h3 class="panel-title pull-left">' + modifier.modifierName + '</h3>';
    html += '<a href="javascript:void(0);" class="btn-delete pull-right"><i class="glyphicon glyphicon-remove"></i></a>';
    html += '<div class="clearfix"></div>';
    html += '</div>';
    html += '<div class="panel-body">';

    html += '<div class="row">';
    html += '<div class="form-group">';
    html += '<div class="control-label col-md-3">';
    html += '<label>Name:</label>';
    html += '</div>'; // End of control label.
    html += '<div class="col-md-9">Value</div>';
    html += '</div>'; // End of form group.
    html += '</div>'; // End of control row.

    html += '<div class="row">';
    html += '<div class="form-group">';
    html += '<div class="control-label col-md-3">';
    html += '<label>' + modifier.modifierName + ':</label>';
    html += '</div>'; // End of control label.
    html += '<div class="col-md-9">';
    if (modifier.controlType == "select") {
        var choiceString = "";
        if (modifier.choices && typeof (modifier.choices) == 'object') {
            choiceString = modifier.choices.join(',');
        }
        html += '<input type="text" class="modifier-choice choice-select form-control" placeholder="Input value and press ENTER." data-role="tagsinput" value="' + choiceString + '" />'
    } else {
        html += '<input type="text" class="modifier-choice choice-text form-control" placeholder="Separate each value by | sign, eg: Value1|Value2|Value3" />'
    }
    html += '</div>';
    html += '</div>'; // End of form group.
    html += '</div>'; // End of control row.

    html += '<div class="row">';
    html += '<div class="form-group">';
    html += '<div class="control-label col-md-3">';
    html += '<label><input type="checkbox" class="is-variation"';
    if (modifier.isVariation) {
        html += ' checked="checked"'
    }
    html += ' /> Use for variation</label>';
    html += '</div>'; // End of control label.
    html += '<div class="col-md-9"></div>';
    html += '</div>'; // End of form group.
    html += '</div>'; // End of control row.

    html += '</div>'; // End of panel body.
    html += '</li>';
    return html;
}
function bindEventLastModifier() {
    var $modifier = $('#item-modifiers-list .item-modifier:last-child');
    var $choice = $modifier.find('.modifier-choice');
    $modifier.find('.btn-delete').click(onBtnDeleteModifierClicked);
    if ($choice.hasClass("choice-select")) {
        $choice.tagsinput({
            typeahead: {
                source: function (query) {
                    return $.getJSON(getUrl('Admin/Item/GetChoicesJSON'), { modifierId: $modifier.data('id') });
                }
            }
        });
    }
}
function onBtnSaveModifiersClicked(e) {
    var modifierIds = new Array();
    var isVariants = new Array();
    var isActives = new Array();
    var $this = null;
    $('#item-modifiers-list .item-modifier').each(function (i) {
        $this = $(this);
        modifierIds.push($this.data('id'));
        isVariants.push($this.find('input.is-variation').get(0).checked);
        if ($this.data('active') == "1") {
            isActives.push(true);
            saveModifiersChoices($this);
        } else {
            isActives.push(false);
        }
    });
    saveModifiers(modifierIds, isVariants, isActives);
}
function saveModifiers(modifierIds, isVariants, isActives) {
    $.ajax({
        url: getUrl('Admin/Item/SaveItemModifiers'),
        data: { id: window.itemId, modifierIds: modifierIds, isVariants: isVariants, isActives: isActives },
        dataType: 'json',
        type: 'post',
        traditional: true,
        success: function (result) {
            if (result.isSuccess) {
                alert(result.message);
            } else {
                alert(result.message);
            }
        },
        error: function (xhr, msg) {
            alert(msg);
        }
    });
}
function saveModifiersChoices($modifier) {
    var modifierId = $modifier.data('id');
    var choices = new Array();
    var $choices = $modifier.find('.modifier-choice');
    if ($choices.length <= 0)
        return;

    var $choice = null;
    $choices.each(function (i) {
        $choice = jQuery(this);
        if ($choice.hasClass("choice-select")) {
            choices = $choice.tagsinput("items");
        } else {
            var choiceValues = $choice.val().split('|');
            for (var i in choiceValues) {
                if (choiceValues[i].trim() == '')
                    continue;

                choices.push(choiceValues[i].trim());
            }
        }
        if (choices.length > 0) {
            jQuery.ajax({
                url: getUrl('Admin/Item/SaveModifierChoices'),
                data: { itemId: window.itemId, modifierId: modifierId, choices: choices },
                dataType: 'json',
                type: 'post',
                traditional: true,
                success: function (result) {
                    if (!result.isSuccess) {
                        console.error(result.message);
                    }
                }
            });
        }
    });
}
function onBtnDeleteModifierClicked(e) {
    if (confirm("Are you sure for deleting this item?")) {
        var $modifier = jQuery(this).parents('.item-modifier');
        var itemId = window.itemId;
        var modifierId = $modifier.data('id');
        $modifier.data('active', 0).fadeOut();
    }
}
function checkItemModifiersList() {
    if (jQuery('#item-modifiers-list').html().trim() == '') {
        jQuery('#item-modifiers-list').html('<span class="label label-info">Tip</span> There is no any modifier right now. You can select them below.');
    }
}
function reorderModifiers() {
    var modifierIds = new Array();
    var $modifiers = jQuery('#item-modifiers-list .item-modifier');
    $modifiers.each(function (i) {
        modifierIds.push(jQuery(this).data('id'));
    });
    jQuery.ajax({
        url: getUrl('Admin/Item/ReorderItemModifier'),
        data: { id: window.itemId, modifierIds: modifierIds },
        traditional: true,
        type: 'post',
        dataType: 'json'
    });
}
/************ Variations ****************************/
function loadVariations() {
    jQuery.ajax({
        url: getUrl('Admin/Item/AjaxLoadVariations'),
        data: { id: window.itemId },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            jQuery('.variations-list').html('');
            if (result.isSuccess) {
                jQuery('.variations-list').html(result.content);
            } else {
                console.error(result.message);
            }
        }, error: function (xhr, msg) {
            console.error(xhr);
        }
    });
}