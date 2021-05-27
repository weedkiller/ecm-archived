window.deletedTopNavLinks = new Array();

jQuery(document).ready(function ($) {
    loadTopNavLinks();

    $('#add-link').click(onCreateAdsButtonIsClicked);

    $('#save-links-button').click(saveTopNavLinks);

    // Sortable
    $('#top-nav-links-list').sortable();
});

function loadTopNavLinks() {
    jQuery.ajax({
        url: getUrl('Admin/TopNavLink/GetAllTopNavLinks'),
        type: 'post',
        dataType: 'json',
        beforeSend: function () {
            showLoader(true, '#top-nav-links-list');
        },
        success: function (result) {
            showLoader(false, '#top-nav-links-list');
            if (result.isSuccess) {
                jQuery('#top-nav-links-list').html('');
                var elements = jQuery(result.content).appendTo('#top-nav-links-list');

                $('#add-link', elements).click(onCreateAdsButtonIsClicked);

                $('.delete-button', elements).click(onLinkDeleteButtonIsClicked);
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#top-nav-links-list');
        }
    });
}

function onChooseTopNavLinkImages(args) {
    var imageUrls = new Array();
    for (var i in args.files) {
        if (args.files[i].isFile) {
            imageUrls.push(args.files[i].url);
        }
    }

    if (imageUrls.length > 0) {
        jQuery.ajax({
            url: getUrl('Admin/TopNavLink/GetPreviewLinks'),
            type: 'post',
            dataType: 'json',
            traditional: true,
            data: { imageUrls: imageUrls },
            beforeSend: function () {
                showLoader(true, '#top-nav-links-list');
            },
            success: function (result) {
                showLoader(false, '#top-nav-links-list');
                if (result.isSuccess) {
                    var elements = jQuery(result.content).appendTo('#top-nav-links-list');
                    $('.delete-button', elements).click(onLinkDeleteButtonIsClicked);
                }
            },
            error: function (xhr, msg) {
                showLoader(false, '#top-nav-links-list');
            }
        });
    }
}

function onLinkDeleteButtonIsClicked(e) {
    var $parent = jQuery(this).parents('.topnavlink');
    var id = $parent.data('id');
    if (id > 0) {
        window.deletedTopNavLinks.push(id);
    }
    $parent.remove();
}

function saveTopNavLinks(e) {
    var ids = new Array();
    var linkUrls = new Array();
    var imageUrls = new Array();

    // Prepare data.
    jQuery('#top-nav-links-list .topnavlink').each(function () {
        var $this = jQuery(this);
        ids.push($this.data('id'));
        linkUrls.push($this.find('input[name=link_url]').val().trim());
        imageUrls.push($this.find('img.ads_image').attr('src').trim());
    });

    jQuery.ajax({
        url: getUrl('Admin/TopNavLink/SaveTopNavLinks'),
        type: 'post',
        dataType: 'json',
        traditional: true,
        data: { ids: ids, linkUrls: linkUrls, imageUrls: imageUrls, deletedTopNavLinks: window.deletedTopNavLinks },
        beforeSend: function () {
            showLoader(true, '#top-nav-links-list');
        },
        success: function (result) {
            showLoader(false, '#top-nav-links-list');
            if (result.isSuccess) {
                loadTopNavLinks();
                jQuery('#ads-save-success-dialog').modal('show');
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#top-nav-links-list');
        }
    });
}

function onAdsetIsChanged(e) {
    loadTopNavLinks();
}

function onCreateAdsButtonIsClicked(e) {
    moxman.browse({
        extensions: 'gif,jpg,jpeg,png,swf,svg',
        view: 'thumbs',
        oninsert: onChooseTopNavLinkImages
    });
}