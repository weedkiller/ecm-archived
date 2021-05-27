var regex = new RegExp(/[^0-9]+/);
jQuery(document).ready(function ($) {
    loadAlbatrosTeeSheet();

    jQuery('#DepatureTime').change(function (e) {
        window.teeSheetDate = this.value;
        loadAlbatrosTeeSheet();
    });
});

/************************ Functions ******************************/
function loadAlbatrosTeeSheet() {
    var date = jQuery('#DepatureTime').val();
    jQuery.ajax({
        url: getUrl('ItemAjax/GetAlbatrosTeeSheet'),
        type: 'post',
        dataType: 'json',
        data: { courseId: jQuery('#albatros-teesheet-table').data('course-id'), siteId: jQuery('#albatros-teesheet-table').data('site-id'), date: date },
        beforeSend: function () {
            var width = jQuery('.teesheet-wrapper').outerWidth();
            var aWidth = 48;
            var left = (width - aWidth) / 2;
            showLoader(true, '#page-teesheet', { top: 40, left: left, width: 48, height: 48 });
        },
        success: function (result) {
            if (result.isSuccess) {
                jQuery('#albatros-teesheet-table tbody').html(result.html);
                jQuery('#albatros-teesheet-table tbody .reserve-button').click(onReserveButtonIsClicked);
                jQuery('#albatros-teesheet-table tbody input[name=number_of_players]').numberEditor();
                showLoader(false, '#page-teesheet');
            }
        }
    });
}

function onReserveButtonIsClicked(e) {
    e.preventDefault();

    if (this.disabled)
        return;


    if (!checkLogin())
        return;

    var $row = jQuery(this).parents('tr');
    var itemId = jQuery('#item-info').data('id');
    var teeTimeId = $row.data('id');
    var teeDate = $row.data('date');
    var teeTime = $row.data('time');
    var price = $row.data('price');
    var available = $row.data('available');
    var numberOfPlayers = $row.find('input[name=number_of_players]').val();
    var gameType = $row.data('gametype');
    jQuery.ajax({
        url: getUrl('Cart/AddItemAlbatrosTeeSheet'),
        type: 'post',
        dataType: 'json',
        data: { itemId: itemId, teeTimeId: teeTimeId, teeDate: teeDate, teeTime: teeTime, numberOfPlayers: numberOfPlayers, price: price, gameType: gameType, available: available },
        beforeSend: function () {
            showLoader(true, '#albatros-teesheet-table');
        },
        success: function (result) {
            showLoader(false, '#albatros-teesheet-table');
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
            showLoader(false, '#albatros-teesheet-table');
        }
    });
}

function checkLogin() {
    var isLoggedIn = false;
    jQuery.ajax({
        url: getUrl('Common/CheckLogin'),
        type: 'post',
        dataType: 'json',
        async: false,
        beforeSend: function() {
            showLoader(true, '#albatros-teesheet-table');
        },
        success: function (result) {
            showLoader(false, '#albatros-teesheet-table');
            if (result.isSuccess) {
                isLoggedIn = result.isLoggedIn;
                if (isLoggedIn == false) {
                    window.location = getUrl('Login?returnUrl=' + encodeURI(window.location.href));
                }
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#albatros-teesheet-table');
        }
    });
    return isLoggedIn;
}