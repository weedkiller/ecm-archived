jQuery(document).ready(function ($) {
    $('#SiteId').change(onSiteChange);
    $('#CustomerTypeId').change(onCutomerTypeChange);
});

function onSiteChange(e) {
    jQuery.ajax({
        url: getUrl('Customer/GetCustomerTypes'),
        data: { siteId: jQuery('#SiteId').val() },
        type: 'post',
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                jQuery('#CustomerTypeId').html(result.html);
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
        }
    });
}

function onCutomerTypeChange(e) {
    jQuery.ajax({
        url: getUrl('Customer/GetSubCustomerTypes'),
        data: { customerTypeId: jQuery('#CustomerTypeId').val() },
        type: 'post',
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                jQuery('#SubCustomerTypeId').html(result.html);
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
        }
    });
}