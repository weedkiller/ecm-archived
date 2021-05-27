var months = ["January", "Febuary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

jQuery(document).ready(function ($) {
    $('#teesheet-confirm-popup-link').fancybox({
        modal: true
    });

    $('#confirm-teesheet-button').click(onTeeSheetConfirmPopupLinkClicked);

    $('#teesheet-confirm-popup .close-button').click(function () {
        $.fancybox.close();
    });

    $('#teesheet-editor').teeSheet({
        startHour: window.startHour,
        endHour: window.endHour,
        duration: window.duration,
        defaultPrice: window.defaultPrice,
        currency: '&euro;',
        breakHours: [12],
        data: window.teeSheetData,
        readOnly: true,
        emptyText: window.teeSheetEmptyText,

        onPriceChanged: function ($teesheet) {
            var percent = Math.round(eval($teesheet.data('percent')));
            var isExpired = false;
            var date = $teesheet.data('date');
            var prebooking = $teesheet.data('prebooking');

            if (date) {
                var datePart = date.split('-');
                if (datePart.length == 3) {
                    var start = $teesheet.data('start');
                    var now = new Date();
                    var today = new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0);
                    var teeSheetDateTime = new Date(eval(datePart[0]), eval(datePart[1]) - 1, eval(datePart[2]), start, 0, 0);

                    if (now >= teeSheetDateTime) {
                        isExpired = true;
                    }

                    if (!isExpired && prebooking > 0) {
                        var prebookingDate = new Date(teeSheetDateTime);
                        var diff = teeSheetDateTime.getDate() - prebooking;
                        prebookingDate.setDate(diff);
                        if (today > prebookingDate) {
                            isExpired = true;
                            var prebookingErrorText = window.bookingAdvanceDescription.replace('{0}', prebooking);
                            $teesheet.attr('title', prebookingErrorText);
                        } else {
                            $teesheet.removeAttr('title');
                        }
                    }
                }
            }

            if (isExpired) {
                $teesheet.removeClass('discount-0')
                    .removeClass('discount-1')
                    .removeClass('discount-2')
                    .removeClass('discount-3')
                    .addClass('expired');
            }
            else if (percent <= 0) {
                $teesheet.removeClass('discount-1')
                    .removeClass('discount-2')
                    .removeClass('discount-3')
                    .removeClass('expired')
                    .addClass('discount-0');
            } else if (percent <= 20) {
                $teesheet.removeClass('discount-0')
                    .removeClass('discount-2')
                    .removeClass('discount-3')
                    .removeClass('expired')
                    .addClass('discount-1');
            } else if (percent < 40) {
                $teesheet.removeClass('discount-0')
                    .removeClass('discount-1')
                    .removeClass('discount-3')
                    .removeClass('expired')
                    .addClass('discount-2');
            } else {
                $teesheet.removeClass('discount-0')
                    .removeClass('discount-1')
                    .removeClass('discount-2')
                    .removeClass('expired')
                    .addClass('discount-3');
            }

        },
        onTeeSheetClicked: function ($teesheet) {

            var day = $teesheet.data('day');

            var percent = Math.round(eval($teesheet.data('percent')));
            //if (percent == 0)
            //    return;

            if ($teesheet.hasClass('expired'))
                return;

            if ($teesheet.data('price') == 0)
                return;

            window.teeSheetId = $teesheet.data('id');
            window.teeSheetDate = $teesheet.parents('table').find('thead .day-' + day).data('date');
            window.teeSheetStart = $teesheet.data('start');
            window.teeSheetEnd = $teesheet.data('end');
            window.teeSheetPrice = $teesheet.data('price');
            window.teeSheetDiscount = $teesheet.data('discount');

            var teeSheetDate = window.teeSheetDate.split('-');
            var d = new Date(teeSheetDate[0], teeSheetDate[1], teeSheetDate[2]);
            $('#teesheet-confirm-popup').data('id', window.teeSheetId).data('date', window.teeSheetDate);
            $('#teesheet-confirm-popup .teesheet-date').text(d.getDate() + '/' + (d.getMonth() + 1) + '/' + d.getFullYear());
            $('#teesheet-confirm-popup .teesheet-start').text(window.teeSheetStart + ':00');
            $('#teesheet-confirm-popup .teesheet-end').text(window.teeSheetEnd + ':00');
            if (window.teeSheetDiscount > 0) {
                $('#teesheet-confirm-popup .teesheet-price').text(window.teeSheetDiscount.toLocaleString('fr', { style: 'currency', currency: 'EUR', minimumFractionDigits: 2 }));
            } else {
                $('#teesheet-confirm-popup .teesheet-price').text(window.teeSheetPrice.toLocaleString('fr', { style: 'currency', currency: 'EUR', minimumFractionDigits: 2 }));
            }
            $('#teesheet-confirm-popup-link').click();
        },
        onClearTeeSheet: function ($teesheet) {
            $teesheet.removeClass('discount-0 discount-1 discount-2 discount-3 expired');
        },
        onTeeSheetGenerating: function () {
            showLoader(true, '.teesheet-table');
        },
        onTeeSheetGenerated: function () {
            var blankRows = 0;
            var $rows = jQuery('.teesheet-table tbody tr');
            $rows.each(function () {
                var $row = jQuery(this);
                var found = 0;
                $row.find('td').each(function () {
                    if (jQuery(this).data('price') == 0) {
                        found++;
                    }
                    if (found >= 7) {
                        blankRows++;
                        $row.hide();
                    } else {
                        $row.show();
                    }
                });
            });
            if (blankRows == $rows.length) {
                jQuery('.teesheet-table tfoot').show();
            } else {
                jQuery('.teesheet-table tfoot').hide();
            }

            showLoader(false, '.teesheet-table');
        }
    });
});

function onTeeSheetConfirmPopupLinkClicked() {
    jQuery.ajax({
        url: getUrl('Cart/AddItemTeeSheet'),
        type: 'post',
        dataType: 'json',
        data: {
            itemId: jQuery('#item-info').data('id'),
            teeSheetDate: window.teeSheetDate,
            numberOfPlayers: jQuery('#input-number-players').val(),
            teeSheetStart: window.teeSheetStart,
            teeSheetEnd: window.teeSheetEnd,
            price: window.teeSheetPrice,
            discount: window.teeSheetDiscount
        },
        success: function (result) {
            if (result.isSuccess) {
                window.location = getUrl('cart');
            }
        }
    });
}