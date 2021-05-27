var regex = new RegExp(/[^0-9]+/);
jQuery(document).ready(function ($) {
    loadPrimaTeeSheet();

    jQuery('#DepatureTime, #prima-gametype').change(function (e) {
        window.teeSheetDate = this.value;
        loadPrimaTeeSheet();
    });
});

/************************ Functions ******************************/
function loadPrimaTeeSheet() {
    var date = jQuery('#DepatureTime').val();
    jQuery.ajax({
        url: getUrl('ItemAjax/GetPrimaTeeSheet'),
        type: 'post',
        dataType: 'json',
        data: { siteId: jQuery('#prima-teesheet-table').data('site-id'), date: date , gameType: jQuery('#prima-gametype').val() },
        beforeSend: function () {
            var width = jQuery('.teesheet-wrapper').outerWidth();
            var aWidth = 48;
            var left = (width - aWidth) / 2;
            showLoader(true, '#page-teesheet', { top: 40, left: left, width: 48, height: 48 });
        },
        success: function (result) {
            if (result.isSuccess) {
                jQuery('#prima-teesheet-table tbody').html(result.html);
                jQuery('#prima-teesheet-table tbody .reserve-button').click(onReserveButtonIsClicked);
                jQuery('#prima-teesheet-table tbody input[name=number_of_players]').numberEditor();
                showLoader(false, '#page-teesheet');
            }
        }
    });
}

function onReserveButtonIsClicked(e) {
    e.preventDefault();

    if (this.disabled)
        return;

    var $row = jQuery(this).parents('tr');
    var itemId = jQuery('#item-info').data('id');
    var courseId = $row.data('id');
    var teeDate = $row.data('date');
    var teeTime = $row.data('time');
    var teeTime9In = $row.data('time9in');
    var teeTime9Out = $row.data('time9out');
    var price = $row.data('price');
    var numberOfPlayers = $row.find('input[name=number_of_players]').val();
    var gameType = $row.data('gametype');
    jQuery.ajax({
        url: getUrl('Cart/AddItemPrimaTeeSheet'),
        type: 'post',
        dataType: 'json',
        data: { itemId: itemId, courseId: courseId, teeDate: teeDate, teeTime: teeTime, teeTime9In: teeTime9In, teeTime9Out: teeTime9Out, numberOfPlayers: numberOfPlayers, price: price, gameType: gameType },
        beforeSend: function () {
            showLoader(true, '#prima-teesheet-table');
        },
        success: function (result) {
            showLoader(false, '#prima-teesheet-table');
            if (result.isSuccess) {
                onAddToCartSuccess(itemId, function () {
                    window.location = getUrl('cart');
                });
            } else {
                jQuery('#error-popup .popup-content').html(result.message);
                jQuery('#error-link').trigger('click');
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#prima-teesheet-table');
        }
    });
}