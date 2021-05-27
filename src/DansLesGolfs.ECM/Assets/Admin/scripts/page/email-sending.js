jQuery(document).ready(function ($) {
    $('#test-email-button').click(onTestEmailButtonIsClicked);
    $('#send-test-email-button').click(onSendTestEmailButtonIsClicked);
    $('#send-test-email-button').click(onSendTestEmailButtonIsClicked);
    $('#SiteId').change(onSiteChanged);
});

function onTestEmailButtonIsClicked(e) {
    if (jQuery('#test-email-panel').hasClass('hide')) {
        jQuery('#test-email-panel').hide().removeClass('hide').slideDown();
    } else if (jQuery('#test-email-panel').css('display') === 'none') {
        jQuery('#test-email-panel').slideDown();
    } else {
        jQuery('#test-email-panel').slideUp();
        jQuery('#test-email-info-msg').addClass('hide');
        jQuery('#test-email-success-msg').addClass('hide');
        jQuery('#test-email-error-msg').text('').addClass('hide');
    }
}

function onSendTestEmailButtonIsClicked(e) {
    jQuery('#test-email-info-msg').removeClass('hide');
    jQuery('#test-email-success-msg').addClass('hide');
    jQuery('#test-email-error-msg').text('').addClass('hide');

    // Mailing list IDs
    var customerGroupIds = new Array();
    jQuery('input[name=CustomerGroupIds]:checked').each(function () {
        customerGroupIds.push(this.value);
    });

    jQuery.ajax({
        url: getUrl('Emailing/SendTestEmail'),
        dataType: 'json',
        type: 'post',
        data: { emails: jQuery('#test-email-textarea').val().trim(), customerGroupIds: customerGroupIds },
        traditional: true,
        success: function (result) {
            jQuery('#test-email-info-msg').addClass('hide');
            if (result.isSuccess) {
                jQuery('#test-email-success-msg').removeClass('hide');
                jQuery('#test-email-error-msg').addClass('hide');
            } else {
                jQuery('#test-email-success-msg').addClass('hide');
                jQuery('#test-email-error-msg').text(result.message).removeClass('hide');
            }
        },
        error: function (xhr, msg) {
            jQuery('#test-email-info-msg').addClass('hide');
            jQuery('#test-email-success-msg').addClass('hide');
            jQuery('#test-email-error-msg').text(msg).removeClass('hide');
        }
    });
}

function onSiteChanged() {
    var siteId = jQuery('#SiteId').val();
    loadDefaultSender(siteId);
}

function loadImpressumBySiteId(siteId) {
    jQuery.ajax({
        url: getUrl('Emailing/GetImpressum'),
        dataType: 'json',
        type: 'post',
        data: { siteId: siteId },
        traditional: true,
        success: function (result) {
            if (result.isSuccess) {
                jQuery('#EmailFormatId').html(result.html);
            } else {
                alert(result.message);
            }
        },
        error: function (xhr, msg) {
            alert(msg);
        }
    });
}

function loadDefaultSender(siteId) {
    jQuery.ajax({
        url: getUrl('Emailing/GetDefaultSender'),
        dataType: 'json',
        type: 'post',
        data: { siteId: siteId },
        traditional: true,
        success: function (result) {
            if (result.isSuccess) {
                jQuery('#FromName').val(result.name);
                jQuery('#FromEmail').val(result.email);
            } else {
                alert(result.message);
            }
        },
        error: function (xhr, msg) {
            alert(msg);
        }
    });
}