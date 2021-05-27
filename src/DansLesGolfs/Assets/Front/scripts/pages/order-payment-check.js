jQuery(document).ready(function ($) {

    $('#error-link').fancybox({
        autoScale: true,
        modal: true,
        minHeight: 20,
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        }
    });

    $('#error-popup .ok-button').click(function () { $.fancybox.close(); });

    $('#confirm-button').click(onConfirmButtonIsClicked);
});

function onConfirmButtonIsClicked() {
    jQuery.ajax({
        url: getUrl('Order/AjaxConfirmPaymentByCheck'),
        dataType: 'json',
        type: 'post',
        beforeSend: function () {
            showLoader(true, '#page-order-payment-check');
        },
        success: function (result) {
            showLoader(false, '#page-order-payment-check');
            if (result.isSuccess) {
                window.location = getUrl('Order/Confirmation/' + result.orderId);
            } else {
                $('#error-popup .popup-content').html(result.message);
                $('#error-link').trigger('click');
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#page-order-payment-check');
        }
    });
}