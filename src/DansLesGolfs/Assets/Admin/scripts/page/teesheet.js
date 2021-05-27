var months = [window.teeSheetLocale.january,
            window.teeSheetLocale.february,
            window.teeSheetLocale.march,
            window.teeSheetLocale.april,
            window.teeSheetLocale.may,
            window.teeSheetLocale.june,
            window.teeSheetLocale.july,
            window.teeSheetLocale.august,
            window.teeSheetLocale.september,
            window.teeSheetLocale.october,
            window.teeSheetLocale.november,
            window.teeSheetLocale.december
];

var today = new Date();
var monday = new Date();
var sunday = new Date();

var teeSheetOptions = {
    startHour: 7,
    endHour: 20,
    showDateNav: true,
    defaultPrice: window.defaultPrice,
    currency: window.currency,
    breakHours: [12],
    data: window.teeSheetData,

    onPriceChanged: function ($teesheet) {
        var price = Math.round(eval($teesheet.data('price')));
        var percent = Math.round(eval($teesheet.data('percent')));
        if (price <= 0 && percent <= 0) {
            $teesheet.removeClass('discount-1')
                .removeClass('discount-2')
                .removeClass('discount-3')
        }
        else if (price > 0 && percent <= 20) {
            $teesheet.removeClass('discount-2')
                .removeClass('discount-3')
                .addClass('discount-1');
        } else if (percent < 40) {
            $teesheet.removeClass('discount-1')
                .removeClass('discount-3')
                .addClass('discount-2');
        } else {
            $teesheet.removeClass('discount-1')
                .removeClass('discount-2')
                .addClass('discount-3');
        }
    },
    onClearTeeSheet: function ($teesheet) {
        $teesheet.removeClass('discount-0 discount-1 discount-2 discount-3 expired');
    },
    onTeeSheetGenerated: onTeeSheetGenerated,
    onTeeSheetClicking: function ($teesheet) {
        if ($teesheet.hasClass('expired'))
            return false;
        else
            return true;
    },
    onPreBookingDaysValuesChanged: onPreBookingDaysValuesChanged
};

jQuery(document).ready(function ($) {
    $('#daterange-popup .ok-button').click(onDateRangeIsSelected);

    $('#duplicate-selected-button').click(onDuplicateSelectedItemsIsClicked);
    $('#select-all-button').click(onSelectAllButtonIsClicked);
    $('#deselect-all-button').click(onDeselectAllButtonIsClicked);
    $('#delete-all-button').click(onDeleteAllButtonIsClicked);

    //$('#CategoryId').change(onGreenFeeCategoryChanged);

    $('#CourseId').change(function () {
        var courseId = jQuery(this).val();
        if (courseId > 0) {
            jQuery.ajax({
                url: getUrl('Admin/GreenFee/AjaxGetCourseDefaultPrice'),
                type: 'post',
                dataType: 'json',
                data: { courseId: courseId },
                success: function (result) {
                    if (result.isSuccess) {
                        window.defaultPrice = result.defaultPrice;
                        reloadTeeSheet();
                    } else {
                        alert(result.message);
                    }
                }
            });
        }
    });
    $('#CourseId').trigger('change');

    $('#date-from').change(onDateChanged);

    reloadTeeSheet();
    //onGreenFeeCategoryChanged();

    today = jQuery('#date-from').datepicker('getDate');
    monday = findMonday(today);
    sunday = new Date(monday);
    sunday.setDate(sunday.getDate() + 6);
});

function onGreenFeeCategoryChanged() {
    var found = false;
    var val = $('#CategoryId').val();
    for (var i in window.allowTeeSheetCategories) {
        if (window.allowTeeSheetCategories[i] == val) {
            found = true;
            break;
        }
    }
    if (found) {
        $('a[href=#greenfee-teesheet]').parents('li').show();
    } else {
        $('a[href=#greenfee-teesheet]').parents('li').hide();
    }
}
function reloadTeeSheet() {

    $('#teesheet-editor').teeSheet(teeSheetOptions);

    $('#teesheet-editor .teesheet-date-navigation .go-back-link').unbind('click');
    $('#teesheet-editor .teesheet-date-navigation .go-next-link').unbind('click');

    $('#teesheet-editor .teesheet-date-navigation .go-back-link').click(function () {
        var $this = jQuery(this).parents('#teesheet-editor');
        var $tableBody = $this.find('tbody');
        monday.setDate(monday.getDate() - 7);
        sunday = new Date(monday);
        sunday.setDate(sunday.getDate() + 6);
        generateDateText($this, monday, sunday);
        clearTeeSheets($tableBody);
        setTeeSheetData();

        $('#date-from').datepicker('setDate', monday);
        $('#date-to').datepicker('setDate', sunday);
    });

    $('#teesheet-editor .teesheet-date-navigation .go-next-link').click(function () {
        var $this = jQuery(this).parents('#teesheet-editor');
        var $tableBody = $this.find('tbody');
        monday.setDate(monday.getDate() + 7);
        sunday = new Date(monday);
        sunday.setDate(sunday.getDate() + 6);
        generateDateText($this, monday, sunday);
        clearTeeSheets($tableBody);
        setTeeSheetData();

        $('#date-from').datepicker('setDate', monday);
        $('#date-to').datepicker('setDate', sunday);
    });
}

function onDuplicateSelectedItemsIsClicked(e) {
    $('#daterange-popup').modal('show');
}

function onSelectAllButtonIsClicked(e) {
    jQuery('.teesheet-table tbody td:not(.expired)').addClass('selected');
}

function onDeselectAllButtonIsClicked(e) {
    jQuery('.teesheet-table tbody td').removeClass('selected');
}

function onDeleteAllButtonIsClicked(e) {
    if (!confirm(window.confirmDeleteText))
        return;

    jQuery.ajax({
        url: getUrl('Admin/GreenFee/AjaxDeleteAllTeeSheet'),
        type: 'post',
        dataType: 'json',
        data: { id: itemId },
        success: function (result) {
            if (!result.isSuccess) {
                showMessageModal("Error", result.message);
            } else {
                //$('#teesheet-editor input[name=TeeSheetData]').val(JSON.stringify({
                //    teeSheet: new Array()
                //}));
                //window.tsData = null;
                //onDateChanged(e);
                window.location.hash = '#greenfee-teesheet';
                window.location.reload(true);
            }
        }
    });
}

function onDateChanged(e) {
    e.preventDefault();
    e.stopPropagation();

    var from = jQuery('#date-from').datepicker('getDate');
    var to = jQuery('#date-to').datepicker('getDate');
    var monday = findMonday(from);
    var $t = jQuery('#teesheet-editor'); 
    $t.teeSheet('refreshDataSource');
    $t.teeSheet('loadWeekByDate', from.getFullYear(), from.getMonth(), from.getDate());
}

function findMonday(from) {
    var d = new Date(from);
    var day = d.getDay(), diff = d.getDate() - day + (day == 0 ? -6 : 1);
    return new Date(d.setDate(diff));
}

function generateDateText($this, monday, sunday) {
    $this.find('.teesheet-date-navigation .date-text').html(window.teeSheetLocale.from + ' ' + monday.getDate() + ' ' + months[monday.getMonth()] + " " + monday.getFullYear() + ' ' + window.teeSheetLocale.to + ' ' + sunday.getDate() + ' ' + months[sunday.getMonth()] + " " + sunday.getFullYear());
    var d = undefined;
    for (var i = 0 ; i < 7; i++) {
        d = new Date(monday);
        d.setDate(d.getDate() + i);
        $this.find('.teesheet-table thead .day-' + (i + 1) + ' .date-number').html(d.getDate())
            .parents('th').attr('data-date', d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate());
    }
    $this.find('tbody tr').each(function () {
        for (var i = 0; i <= 7; i++) {
            d = new Date(monday);
            d.setDate(d.getDate() + i);
            $(this).find('td:eq(' + i + ')').attr('data-date', d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate());
        }
    });
};

function clearTeeSheets($tableBody) {
    $tableBody.find('tr td').html('')
        .data('price', 0)
        .data('discount', 0)
        .data('percent', 0).removeClass('discount-0 discount-1 discount-2 discount-3 expired');
}

function setTeeSheetData() {
    var jsonStr = $('#teesheet-editor input[name=TeeSheetData]').val();
    var data = JSON.parse(jsonStr);
    window.teeSheetData = data.teeSheets;
    var $teesheet;
    var $tr = $('.teesheet-table tbody tr');
    if (teeSheetOptions.emptyText && data != null && data != undefined && data.length > 0 && !isDataInCurrentWeek(data)) {
        // Show Empty Text.
        jQuery('.teesheet-table tfoot').show();
        jQuery('.teesheet-table tbody').hide();
    } else {
        // Generate teesheet from existing data.
        jQuery('.teesheet-table tfoot').hide();
        jQuery('.teesheet-table tbody').show();

        jQuery('.tee-time', $tr).each(function () {
            $teesheet = jQuery(this);
            var day = $teesheet.data('day') - 1;
            var date = new Date(monday);
            date.setDate(date.getDate() + day);
            $teesheet.data('date', date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate());
        });

        for (var i in window.teeSheetData) {
            $teesheet = $tr.find('td[data-date=' + window.teeSheetData[i].date + '][data-start=' + window.teeSheetData[i].start + '][data-end=' + window.teeSheetData[i].end + ']');

            if ($teesheet.length > 0) {
                $teesheet.data('id', window.teeSheetData[i].id);
                $teesheet.data('date', window.teeSheetData[i].date);
                setPrice($teesheet, window.teeSheetData[i].price, window.teeSheetData[i].discount, window.teeSheetData[i].prebooking);
            }
        }

        onTeeSheetGenerated();
    }
}

function setPrice($teesheet, price, discount, prebooking) {
    if (price == undefined)
        price = 0;

    if (discount == undefined)
        discount = 0;

    var day = $teesheet.data('day');
    var start = $teesheet.data('start');
    var end = $teesheet.data('end');

    var html = '';
    var percent = 0;
    var discountPrice = 0;
    if (price > 0) {
        if (price > 0 && discount > 0) {
            discountPrice = price - discount;
            percent = discountPrice * 100 / price;

            var priceDisplay = price.toFixed(2);
            var discountDisplay = discount.toFixed(2);
            if (window.teeSheetLocale.decimalSign != '.') {
                priceDisplay = priceDisplay.substring(0, priceDisplay.indexOf('.')) + window.teeSheetLocale.decimalSign + '<span class="floating-number">' + priceDisplay.substring(priceDisplay.indexOf('.') + 1) + '</span>';
                discountDisplay = discountDisplay.substring(0, discountDisplay.indexOf('.')) + window.teeSheetLocale.decimalSign + '<span class="floating-number">' + discountDisplay.substring(discountDisplay.indexOf('.') + 1) + '</span>';
            } else {
                priceDisplay = priceDisplay.substring(0, priceDisplay.indexOf('.')) + '.' + '<span class="floating-number">' + priceDisplay.substring(priceDisplay.indexOf('.') + 1) + '</span>';
                discountDisplay = discountDisplay.substring(0, discountDisplay.indexOf('.')) + '.' + '<span class="floating-number">' + discountDisplay.substring(discountDisplay.indexOf('.') + 1) + '</span>';
            }

            html += '<span class="teesheet-oldprice">' + priceDisplay + ' ' + window.teeSheetOptions.currency + '</span> <span class="discount-percent">' + Math.round(percent) + '%</span><br />';
            html += '<span class="teesheet-price">' + discountDisplay + ' ' + window.teeSheetOptions.currency + '</span>';
        } else {
            var priceDisplay = price.toFixed(2);
            if (window.teeSheetLocale.decimalSign != '.') {
                priceDisplay = priceDisplay.substring(0, priceDisplay.indexOf('.')) + window.teeSheetLocale.decimalSign + '<span class="floating-number">' + priceDisplay.substring(priceDisplay.indexOf('.') + 1) + '</span>';
            } else {
                priceDisplay = priceDisplay.substring(0, priceDisplay.indexOf('.')) + '.' + '<span class="floating-number">' + priceDisplay.substring(priceDisplay.indexOf('.') + 1) + '</span>';
            }
            html += '<span class="teesheet-oldprice"></span><br />';
            html += '<span class="teesheet-price">' + priceDisplay + ' ' + window.teeSheetOptions.currency + '</span>';
        }
    } else {
        html += '<span class="teesheet-price"></span>';
    }
    html += '<input type="hidden" name="teesheet_days[]" value="' + day + '" />';
    html += '<input type="hidden" name="teesheet_start_times[]" value="' + start + '" />';
    html += '<input type="hidden" name="teesheet_end_times[]" value="' + end + '" />';
    html += '<input type="hidden" name="teesheet_prices[]" value="' + price + '" />';
    html += '<input type="hidden" name="teesheet_discounts[]" value="' + discount + '" />';
    html += '<input type="hidden" name="teesheet_percents[]" value="' + percent + '" />';
    html += '<input type="hidden" name="teesheet_prebookings[]" value="' + prebooking + '" />';
    $teesheet.data('price', price)
        .data('discount', discount)
        .data('percent', percent)
        .data('prebooking', prebooking)
        .html(html);

    var price = Math.round(eval($teesheet.data('price')));
    var percent = Math.round(eval($teesheet.data('percent')));
    if (price <= 0 && percent <= 0) {
        $teesheet.removeClass('discount-1')
            .removeClass('discount-2')
            .removeClass('discount-3')
    }
    else if (price > 0 && percent <= 20) {
        $teesheet.removeClass('discount-2')
            .removeClass('discount-3')
            .addClass('discount-1');
    } else if (percent < 40) {
        $teesheet.removeClass('discount-1')
            .removeClass('discount-3')
            .addClass('discount-2');
    } else {
        $teesheet.removeClass('discount-1')
            .removeClass('discount-2')
            .addClass('discount-3');
    }
};

function onTeeSheetGenerated() {
    jQuery('.teesheet-table tbody .tee-time').each(function () {
        var $teesheet = jQuery(this);
        var date = $teesheet.data('date');
        if (date) {
            var datePart = date.split('-');
            if (datePart.length == 3) {
                var start = $teesheet.data('start');
                var now = new Date();
                var teeSheetDateTime = new Date(eval(datePart[0]), eval(datePart[1]) - 1, eval(datePart[2]), start, 0, 0, 0);
                if ((now >= teeSheetDateTime)) {
                    $teesheet.addClass('expired');
                } else {
                    $teesheet.removeClass('expired');
                }
            }
        }
    });
}

function onDateRangeIsSelected(e) {
    var $this = $(this);
    var $parent = $this.parents('.modal');
    var start = $('#DateStart', $parent);
    var end = $('#DateEnd', $parent);

    if (!start.val()) {
        alert('Please specific date.');
        start.select();
        return;
    }

    if (!end.val()) {
        alert('Please specific date.');
        end.select();
        return;
    }

    // Get current teesheet data.
    var teeSheetData = JSON.parse($('input[name=TeeSheetData]').val());
    var teeSheets = teeSheetData.teeSheets;

    var startDate = start.datepicker('getDate');
    var endDate = end.datepicker('getDate');
    var startTime = startDate.getTime();
    var endTime = endDate.getTime();
    var oneDay = 1000 * 60 * 60 * 24;
    var tempDate = new Date();
    var tempDay = 0;
    var tempStartTeeTime = 0;
    var tempEndTeeTime = 0;
    var tempDateText = '';

    // Get only selected teetime.
    var selectedTeeTimes = $('.teesheet-table tbody td.selected:not(.expired)');
    var teeTimes = getTeeTimesBySelectedCell(teeSheets, selectedTeeTimes);
    
    for (var i = startTime; i <= endTime; i += oneDay) {
        tempDate = new Date();
        tempDate.setTime(i);
        tempDay = tempDate.getDay();
        tempDay = tempDay == 0 ? 7 : tempDay;
        tempDateText = tempDate.getFullYear() + "-" + (tempDate.getMonth() + 1) + "-" + tempDate.getDate();
        for (var j in teeTimes) {
            if (tempDay == teeTimes[j].day) {
                teeSheets = addTeeTimeToTeeSheetData(tempDateText, teeTimes[j], teeSheets);
            }
        }
    }
    $('input[name=TeeSheetData]').val(JSON.stringify({
        teeSheets: teeSheets
    }));

    // Refresh DataSource
    var $t = jQuery('#teesheet-editor');

    $('#daterange-popup').modal('hide');

    saveTeeSheetData();
}

function getTeeTimesBySelectedCell(teeSheets, selectedTeeTimes) {
    var teeTimes = new Array();
    selectedTeeTimes.each(function () {
        for (var i in teeSheets) {
            if (teeSheets[i].date == $(this).data('date')
                && teeSheets[i].start == $(this).data('start')
                && teeSheets[i].end == $(this).data('end')) {
                teeTimes.push(teeSheets[i]);
            }
        }
    });
    return teeTimes;
}

function addTeeTimeToTeeSheetData(date, teeTime, teeSheets) {
    var isFound = false;
    for (var i in teeSheets) {
        if (teeSheets[i].date == date
            && teeSheets[i].start == teeTime.start
            && teeSheets[i].end == teeTime.end) {
            isFound = true;
            teeSheets[i].price = teeTime.price;
            teeSheets[i].discount = teeTime.discount;
            teeSheets[i].prebooking = teeTime.prebooking;
            break;
        }
    }
    if (!isFound) {
        teeSheets.push({
            id: 0,
            courseId: teeTime.courseId,
            date: date,
            day: teeTime.day,
            start: teeTime.start,
            end: teeTime.end,
            price: teeTime.price,
            discount: teeTime.discount,
            prebooking: teeTime.prebooking
        });
    }

    return teeSheets;
}

function saveTeeSheetData() {
    var itemId = jQuery('input[name=id]').val();
    var teeSheetData = $('input[name=TeeSheetData]').val();
    var courseId = $('#CourseId').val();

    jQuery.ajax({
        url: getUrl('Admin/GreenFee/AjaxSaveTeeSheet'),
        type: 'post',
        dataType: 'json',
        data: { id: itemId, CourseId: courseId, TeeSheetData: teeSheetData },
        success: function (result) {
            if (!result.isSuccess) {
                showMessageModal("Error", result.message);
            }
        }
    });
}

function onPreBookingDaysValuesChanged($teesheetArray) {
    saveTeeSheetData();
}