jQuery(function ($) {
    $('#help-popup-link').fancybox({
        modal: true,
        width: 500,
        height: 300,
        autoSize: false,
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        }
    }); $('#help-thankyou-popup-link').fancybox({
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        }
    });
    $('#order-help-form').on('submit', onHelpFormIsSubmited);

    $('.order-box-content .order-help-link').click(onOrderHelpLinkIsClicked);

    $('#help-popup .close-button, #help-thankyou-popup button').click(function () {
        $.fancybox.close();
    });
});

function onOrderHelpLinkIsClicked(e) {
    e.stopPropagation();
    jQuery('#help-popup').data('id', jQuery(this).data('id'));
    jQuery('#help-popup-link').click();
}

function onHelpFormIsSubmited(e) {
    e.stopPropagation();
    e.preventDefault();

    var id = jQuery('#help-popup').data('id');
    jQuery.ajax({
        url: getUrl('Account/SendOrderHelpEmail'),
        data: {
            id: id,
            subject: jQuery('#help-popup #OrderHelpSubject').val(),
            message: jQuery('#help-popup #OrderHelpMessage').val()
        },
        dataType: 'json',
        type: 'post',
        beforeSend: function () {
            showLoader(true, '.fancybox-wrap');
        },
        success: function (result) {
            showLoader(false, '.fancybox-wrap');
            if (result.isSuccess) {
                jQuery.fancybox.close();
                jQuery('#help-popup').data('id', 0);
                jQuery('#help-popup #OrderHelpSubject').val(''),
                jQuery('#help-popup #OrderHelpMessage').val('')
                jQuery('#help-thankyou-popup-link').click();
            }
        }
    });
}