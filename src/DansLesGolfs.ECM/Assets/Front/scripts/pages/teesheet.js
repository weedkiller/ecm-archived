
jQuery(document).ready(function ($) {
    $('#teesheet-confirm-popup-link').fancybox({
        modal: true
    });
});

function onTeeSheetConfirmPopupLinkClicked() {
    jQuery.ajax({
        url: getUrl('Cart/AddItemTeeSheet'),
        type: 'post',
        dataType: 'json',
        data: { itemId: jQuery('#item-info').data('id'), quantity: qty, teeSheetId: window.teeSheetId, teeSheetDate: window.teeSheetDate, numberOfPlayers: jQuery('#input-number-players').val() },
        success: function (result) {
            if (result.isSuccess) {
                window.location = getUrl('cart');
            } else {
                alert(result.message);
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
        }
    });
}