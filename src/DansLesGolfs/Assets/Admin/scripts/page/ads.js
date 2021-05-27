window.deletedAds = new Array();

jQuery(document).ready(function ($) {
    loadAds();
    
    $('#AdsetId').change(onAdsetIsChanged);

    $('.create-ads-button').click(onCreateAdsButtonIsClicked);

    $('.save-ads-button').click(saveAds);

    // Sortable
    $('#ads-list').sortable();
});

function loadAds() {
    var adsetId = jQuery('#AdsetId').val();
    jQuery.ajax({
        url: getUrl('Admin/Advertise/GetAdsInAdset'),
        type: 'post',
        dataType: 'json',
        data: { adsetId: adsetId },
        beforeSend: function () {
            showLoader(true, '#ads-list');
        },
        success: function (result) {
            showLoader(false, '#ads-list');
            if (result.isSuccess) {
                jQuery('#ads-list').html('');
                var elements = jQuery(result.content).appendTo('#ads-list');

                $('.create-ads-button', elements).click(onCreateAdsButtonIsClicked);

                $('.delete-button', elements).click(onAdsDeleteButtonIsClicked);

                initDatePicker(elements);
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#ads-list');
        }
    });
}

function onChooseAdsImages(args) {
    var imageUrls = new Array();
    for (var i in args.files) {
        if (args.files[i].isFile) {
            imageUrls.push(args.files[i].url);
        }
    }

    if (imageUrls.length > 0) {
        jQuery.ajax({
            url: getUrl('Admin/Advertise/GetPreviewAds'),
            type: 'post',
            dataType: 'json',
            traditional: true,
            data: { adsetId: $('#AdsetId').val(), imageUrls: imageUrls },
            beforeSend: function () {
                showLoader(true, '#ads-list');
            },
            success: function (result) {
                showLoader(false, '#ads-list');
                if (result.isSuccess) {
                    var elements = jQuery(result.content).appendTo('#ads-list');
                    $('.delete-button', elements).click(onAdsDeleteButtonIsClicked);
                    initDatePicker(elements);
                    $('.no-ads').hide();
                }
            },
            error: function (xhr, msg) {
                showLoader(false, '#ads-list');
            }
        });
    }
}

function onAdsDeleteButtonIsClicked(e) {
    var $parent = jQuery(this).parents('.ads');
    var id = $parent.data('id');
    if (id > 0) {
        window.deletedAds.push(id);
    }
    $parent.remove();
}

function saveAds(e) {
    var adsIds = new Array();
    var adsNames = new Array();
    var linkUrls = new Array();
    var imageUrls = new Array();
    var fromDates = new Array();
    var toDates = new Array();

    // Prepare data.
    jQuery('#ads-list .ads').each(function () {
        var $this = jQuery(this);
        adsIds.push($this.data('id'));
        adsNames.push($this.find('input[name=ads_name]').val().trim());
        linkUrls.push($this.find('input[name=link_url]').val().trim());
        imageUrls.push($this.find('img.ads_image').attr('src').trim());
        fromDates.push($this.find('input[name=from_date]').val().trim());
        toDates.push($this.find('input[name=to_date]').val().trim());
    });

    jQuery.ajax({
        url: getUrl('Admin/Advertise/SaveAds'),
        type: 'post',
        dataType: 'json',
        traditional: true,
        data: { adsetId: $('#AdsetId').val(), adsIds: adsIds, adsNames: adsNames, linkUrls: linkUrls, imageUrls: imageUrls, deletedAds: window.deletedAds, fromDates: fromDates, toDates: toDates },
        beforeSend: function () {
            showLoader(true, '#ads-list');
        },
        success: function (result) {
            showLoader(false, '#ads-list');
            if (result.isSuccess) {
                loadAds();
                jQuery('#ads-save-success-dialog').modal('show');
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#ads-list');
        }
    });
}

function onAdsetIsChanged(e) {
    loadAds();
}

function onCreateAdsButtonIsClicked(e) {
    moxman.browse({
        extensions: 'gif,jpg,jpeg,png,swf,svg',
        view: 'thumbs',
        oninsert: onChooseAdsImages
    });
}

function initDatePicker(elements) {
    $('.datepicker', elements).datepicker({
        dateFormat: 'dd/mm/yy',
        autoSize: true,
        showOtherMonths: true,
        selectOtherMonths: true,
        todayHighlight: true,
        firstDay: 1
    });
}