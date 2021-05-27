jQuery(document).ready(function ($) {
    $('#confirm-button').click(onConfirmButtonIsClicked);
});

function onConfirmButtonIsClicked() {
    jQuery.post(getUrl('Order/AjaxConfirmPaymentByCheck'), function (result) {
        if (result.isSuccess) {
            window.location = getUrl('Order/Confirmation/' + result.orderId);
        } else {
            console.error(result.message);
        }
    }, 'json');
}