/**
* plugin name: jQuery TeeSheet
* description: Using for create Tee Sheet Table.
* author name: Weerayut Teja
* author email: kenessar@gmail.com
*/
if (!window.teeSheetLocale) {
    window.teeSheetLocale = {
        decimalSign: ".",
        price: "Price",
        discount: "Discount",
        cancel: "Cancel",
        setPrice: "Set Price",
        setPriceContext: "Set Price",
        clearPrice: "Clear Price",
        next7Days: "Next 7 Days",
        previous7Days: "Previous 7 Days",
        sunday: "Sunday",
        monday: "Monday",
        tuesday: "Tuesday",
        wednesday: "Wednesday",
        thursday: "Thursday",
        friday: "Friday",
        saturday: "Saturday",
        january: "January",
        february: "February",
        march: "March",
        april: "April",
        may: "May",
        june: "June",
        july: "July",
        august: "August",
        september: "September",
        october: "October",
        november: "November",
        december: "December",
        from: "From",
        to: "To",
        preBookingDays: "Pre-Booking Days",
        setPreBookingDaysContext: "Set Pre-Booking Days"
    };
}
(function ($) {
    TeeSheet = function (o, a, b, c, d, e) {

        var options = undefined;
        if (typeof (o) == "string") {
            options = window.teeSheetOptions;
        } else {
            options = o;
            window.teeSheetOptions = options;
        }

        var getSunday = function (d) {
            d = new Date(d);
            var day = d.getDay(),
                diff = d.getDate() - day + (day == 0 ? -6 : 1); // adjust when day is sunday

            return new Date(d.setDate(diff));
        }

        var data = new Array();
        var today = new Date();
        var sunday = getSunday(today);
        var saturday = new Date(sunday);

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

        saturday.setDate(sunday.getDate() + 6);

        var addTeeSheet = function (teeSheetData) {
            for (var i in data) {
                if (data[i].day === teeSheetData.day && data[i].start == teeSheetData.start && data[i].end == teeSheetData.end) {
                    data[i] = teeSheetData
                    refresh();
                    return;
                }
            }
            data.push(teeSheetData);
            refresh();
        };

        var setBulkPreBooking = function (teeSheetData) {
            for (var i in data) {
                if (data[i].day === teeSheetData.day && data[i].start == teeSheetData.start && data[i].end == teeSheetData.end) {
                    data[i].prebooking = teeSheetData.prebooking;
                    refresh();
                    return;
                }
            }
            data.push(teeSheetData);
            refresh();
        };

        var removeTeeSheet = function (day, start, end) {
            for (var i in data) {
                if (data[i].day === day && data[i].start == start && data[i].end == end) {
                    data[i] = undefined;
                    refresh();
                    return;
                }
            }
        };

        var clearTeeSheet = function () {
            data = new Array();
        };

        var refresh = function () {
            $('input[name=TeeSheetData]').val(JSON.stringify({ teeSheets: data }));
        };

        var getMondayFromDate = function (date) {
            var d = new Date(date);
            var day = d.getDay(), diff = d.getDate() - day + (day == 0 ? -6 : 1);
            return new Date(d.setDate(diff));
        };

        var loadWeekByDate = function (year, month, date) {
            var $this = jQuery(this);
            var $tableBody = $this.find('tbody');
            var d = new Date(year, month, date);
            sunday = getMondayFromDate(d);
            saturday.setDate(sunday.getDate() + 6);
            generateDateText($this);
            clearTeeSheets($tableBody);
            setTeeSheetData.call(this);
        };

        var getTimePeriods = function (beginTime, endTime, duration, breakHour) {
            var times = new Array();
            for (var i = beginTime, j = 0; i <= endTime; i++, j++) {
                /*if (i == breakHour) {
                    times.push({
                        start: i,
                        end: i + 1,
                        type: 'break'
                    });
                    j = -1;
                } else*/ if (j == 0) {
                    times.push({
                        start: i,
                        end: (i + duration > endTime ? endTime : i + duration),
                        type: 'normal'
                    });
                } else if (j == duration) {
                    times.push({
                        start: i,
                        end: (i + duration > endTime ? endTime : i + duration),
                        type: 'normal'
                    });
                    j = 0;
                } else {
                    continue;
                }
            }
            return times;
        };

        var isDataInCurrentWeek = function (data) {
            var tempDate = null;
            var dateParts = new Array();
            for (var i in data) {
                dateParts = data[i].date.split('-');
                tempDate = new Date(dateParts[0], dateParts[1], dateParts[2]);
                if (tempDate >= sunday && tempDate <= saturday) {
                    return true;
                }
            }
            return false;
        };

        var doOpenPricePopup = function ($teesheet, callback) {
            var isOpen = true;
            if (isOpen) {
                openPricePopup($teesheet, callback);
            }
        };

        var doOpenPreBookingPopup = function ($teesheet, callback) {
            var isOpen = true;
            if (isOpen) {
                openPreBookingPopup($teesheet, callback);
            }
        };

        var onTeeSheetIsClicked = function (e) {
            var $teesheet = $(this);
            if (options.onTeeSheetClicking && typeof (options.onTeeSheetClicking) == 'function') {
                isOpen = options.onTeeSheetClicking($teesheet);
            }
            if (!options.readOnly && isOpen) {
                if (window.isTeeSheetPressCtrl === true) {
                    if ($teesheet.hasClass('selected')) {
                        $teesheet.removeClass('selected');
                    } else {
                        $teesheet.addClass('selected');
                    }
                } else {
                    if ($teesheet.hasClass('selected')) {
                        $('.teesheet-contextmenu').data('teesheet', $teesheet);
                        $('.teesheet-setprice-button').trigger('click');
                    } else {
                        $('.teesheet-table .tee-time.selected').removeClass('selected');

                        doOpenPricePopup($teesheet);
                    }
                }
            }
            if (options.onTeeSheetClicked && typeof (options.onTeeSheetClicked) == 'function') {
                options.onTeeSheetClicked($teesheet);
            }

        };

        var onTeeSheetKeyDown = function (e) {
            if (e.ctrlKey) {
                window.isTeeSheetPressCtrl = true;
            }
        };

        var onTeeSheetKeyUp = function (e) {
            if (window.isTeeSheetPressCtrl === true) {
                window.isTeeSheetPressCtrl = false;
            }
        };

        var onTeeSheetContextMenu = function (e) {
            e.preventDefault();
            if (!options.readOnly) {
                $('.teesheet-contextmenu').css({
                    top: e.pageY + 'px',
                    left: e.pageX + 'px'
                }).show().data('teesheet', $(this));
            }
        };

        var onContextMenuSetPriceIsClicked = function (e) {
            var $parent = $(this).parents('.teesheet-contextmenu');
            if ($parent.data('teesheet')) {
                var $teesheet = $parent.data('teesheet');
                var $teesheetArray = null;
                if ($('.teesheet-table .tee-time.selected').length > 0) {
                    $teesheetArray = $('.teesheet-table .tee-time.selected');
                } else {
                    $teesheetArray = $teesheet;
                }
                doOpenPricePopup($teesheet, function ($t) {
                    var $this = null;
                    $teesheetArray.each(function () {
                        $this = $(this);
                        setPrice($this, $t.data('price'), $t.data('discount'), $t.data('prebooking'));
                        addTeeSheet({
                            id: 0,
                            date: $this.attr('data-date'),
                            day: $this.data('day'),
                            start: $this.data('start'),
                            end: $this.data('end'),
                            price: $this.data('price'),
                            discount: $this.data('discount'),
                            percent: $this.data('percent'),
                            prebooking: $this.data('prebooking')
                        });
                    });
                });
            }
        };

        var onContextMenuClearPriceIsClicked = function (e) {
            var $parent = $(this).parents('.teesheet-contextmenu');
            if ($parent.data('teesheet')) {
                var $teesheet = $parent.data('teesheet');
                var $teesheetArray = null;
                if ($('.teesheet-table .tee-time.selected').length > 0) {
                    $teesheetArray = $('.teesheet-table .tee-time.selected');
                } else {
                    $teesheetArray = $teesheet;
                }
                var $this = null;
                $teesheetArray.each(function () {
                    $this = $(this);
                    setPrice($this, 0, 0, 0);
                    addTeeSheet({
                        id: 0,
                        date: $this.attr('data-date'),
                        day: $this.data('day'),
                        start: $this.data('start'),
                        end: $this.data('end'),
                        price: 0,
                        discount: 0,
                        percent: 0,
                        prebooking: 0
                    });
                });
            }
        };

        var onContextMenuSetPreBookingDayIsClicked = function (e) {
            var $parent = $(this).parents('.teesheet-contextmenu');
            if ($parent.data('teesheet')) {
                var $teesheet = $parent.data('teesheet');
                var $teesheetArray = null;
                if ($('.teesheet-table .tee-time.selected').length > 0) {
                    $teesheetArray = $('.teesheet-table .tee-time.selected');
                } else {
                    $teesheetArray = $teesheet;
                }
                $teesheetArray.each(function () {
                    $this = $(this);
                });

                doOpenPreBookingPopup($teesheet, function ($t) {
                    var $this = null;
                    $teesheetArray.each(function () {
                        $this = $(this);
                        $this.data('prebooking', $t.data('prebooking'));
                        setBulkPreBooking({
                            id: 0,
                            date: $this.attr('data-date'),
                            day: $this.data('day'),
                            start: $this.data('start'),
                            end: $this.data('end'),
                            price: $this.data('price'),
                            discount: $this.data('discount'),
                            percent: $this.data('percent'),
                            prebooking: $this.data('prebooking')
                        });
                    });

                    if (options.onPreBookingDaysValuesChanged && typeof (options.onPreBookingDaysValuesChanged) == 'function') {
                        isOpen = options.onPreBookingDaysValuesChanged($teesheetArray);
                    }
                });
            }
        };

        var onContainerDocumentIsClicked = function (e) {
            //e.stopPropagation();
            $('.teesheet-contextmenu').hide();
            //if (!$(this).hasClass('tee-time')) {
            //    if ($(this).closest('.teesheet-contextmenu').length <= 0) {
            //        $('.teesheet-table .tee-time').removeClass('selected');
            //    }
            //}
        };

        var setPrice = function ($teesheet, price, discount, prebooking) {
            if (price == undefined)
                price = 0;
            
            if (discount == undefined)
                discount = 0;

            if (prebooking == undefined)
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

                    html += '<span class="teesheet-oldprice">' + priceDisplay + ' ' + options.currency + '</span> <span class="discount-percent">' + Math.round(percent) + '%</span><br />';
                    html += '<span class="teesheet-price">' + discountDisplay + ' ' + options.currency + '</span>';
                } else {
                    var priceDisplay = price.toFixed(2);
                    if (window.teeSheetLocale.decimalSign != '.') {
                        priceDisplay = priceDisplay.substring(0, priceDisplay.indexOf('.')) + window.teeSheetLocale.decimalSign + '<span class="floating-number">' + priceDisplay.substring(priceDisplay.indexOf('.') + 1) + '</span>';
                    } else {
                        priceDisplay = priceDisplay.substring(0, priceDisplay.indexOf('.')) + '.' + '<span class="floating-number">' + priceDisplay.substring(priceDisplay.indexOf('.') + 1) + '</span>';
                    }
                    html += '<span class="teesheet-oldprice"></span><br />';
                    html += '<span class="teesheet-price">' + priceDisplay + ' ' + options.currency + '</span>';
                }
            } else {
                html += '<span class="teesheet-price"></span>';
            }
            html += '<input type="hidden" name="teesheet_days[]" value="' + day + '" />';
            html += '<input type="hidden" name="teesheet_start_times[]" value="' + start + '" />';
            html += '<input type="hidden" name="teesheet_end_times[]" value="' + end + '" />';
            html += '<input type="hidden" name="teesheet_prices[]" value="' + price + '" />';
            html += '<input type="hidden" name="teesheet_discounts[]" value="' + discount + '" />';
            html += '<input type="hidden" name="teesheet_prebookings[]" value="' + prebooking + '" />';
            html += '<input type="hidden" name="teesheet_percents[]" value="' + percent + '" />';
            $teesheet.html(html)
                .data('price', price)
                .data('discount', discount)
                .data('percent', percent)
                .data('prebooking', prebooking);

            if (options.onPriceChanged && typeof (options.onPriceChanged) == 'function') {
                options.onPriceChanged($teesheet);
            }
        };

        /**
        * Close pricing popup.
        */
        var closePricePopup = function (e) {
            $('.teesheet-overlay').fadeOut();
            $('.teesheet-price-popup').fadeOut();
            $('.teesheet-price-popup input').val(0);
        };

        /**
        * On set price button is clicked.
        */
        var onSetPriceButtonIsClicked = function (e) {
            var $teesheet = $('.teesheet-price-popup').data('teesheet');
            var callback = $('.teesheet-price-popup').data('callback');
            var price = eval($('.teesheet-price-popup .teesheet-input-price').val());
            var discount = eval($('.teesheet-price-popup .teesheet-input-discount').val());
            var prebooking = eval($('.teesheet-price-popup .teesheet-input-prebooking').val());
            setPrice($teesheet, price, discount, prebooking);
            addTeeSheet({
                id: 0,
                date: $teesheet.attr('data-date'),
                day: $teesheet.data('day'),
                start: $teesheet.data('start'),
                end: $teesheet.data('end'),
                price: $teesheet.data('price'),
                discount: $teesheet.data('discount'),
                percent: $teesheet.data('percent'),
                prebooking: $teesheet.data('prebooking')
            });
            if (callback && typeof (callback) == 'function') {
                callback($teesheet);
            }
            closePricePopup();
        };

        /**
        * On clear price button is clicked.
        */
        var onClearPriceButtonIsClicked = function (e) {
            var $teesheet = $('.teesheet-price-popup').data('teesheet');
            var callback = $('.teesheet-price-popup').data('callback');
            var price = eval($('.teesheet-price-popup .teesheet-input-price').val());
            var discount = eval($('.teesheet-price-popup .teesheet-input-discount').val());
            var prebooking = eval($('.teesheet-price-popup .teesheet-input-prebooking').val());
            setPrice($teesheet, 0, 0, 0);
            addTeeSheet({
                id: 0,
                date: $teesheet.attr('data-date'),
                day: $teesheet.data('day'),
                start: $teesheet.data('start'),
                end: $teesheet.data('end'),
                price: 0,
                discount: 0,
                percent: 0,
                prebooking: 0
            });
            if (callback && typeof (callback) == 'function') {
                callback($teesheet);
            }
            closePricePopup();
        };

        /**
        * Close prebooking popup.
        */
        var closePreBookingPopup = function (e) {
            $('.teesheet-overlay').fadeOut(); console.log($('.teesheet-prebooking-popup').length);
            $('.teesheet-prebooking-popup').fadeOut();
            $('.teesheet-prebooking-popup input').val(0);
        };

        /**
        * On set prebooking button is clicked.
        */
        var onSetPreBookingButtonIsClicked = function (e) {
            var $teesheet = $('.teesheet-prebooking-popup').data('teesheet');
            var callback = $('.teesheet-prebooking-popup').data('callback');
            var prebooking = eval($('.teesheet-prebooking-popup .teesheet-input-prebooking').val());
            $teesheet.data('prebooking', prebooking);
            setBulkPreBooking({
                id: 0,
                date: $teesheet.attr('data-date'),
                day: $teesheet.data('day'),
                start: $teesheet.data('start'),
                end: $teesheet.data('end'),
                price: $teesheet.data('price'),
                discount: $teesheet.data('discount'),
                percent: $teesheet.data('percent'),
                prebooking: $teesheet.data('prebooking')
            });
            if (callback && typeof (callback) == 'function') {
                callback($teesheet);
            }
            closePreBookingPopup();
        };

        /**
        * On clear prebooking button is clicked.
        */
        var onClearPreBookingButtonIsClicked = function (e) {
            var $teesheet = $('.teesheet-prebooking-popup').data('teesheet');
            var callback = $('.teesheet-prebooking-popup').data('callback');
            var prebooking = eval($('.teesheet-prebooking-popup .teesheet-input-prebooking').val());
            $teesheet.data('prebooking', 0);
            addTeeSheet({
                id: 0,
                date: $teesheet.attr('data-date'),
                day: $teesheet.data('day'),
                start: $teesheet.data('start'),
                end: $teesheet.data('end'),
                price: 0,
                discount: 0,
                percent: 0,
                prebooking: 0
            });
            if (callback && typeof (callback) == 'function') {
                callback($teesheet);
            }
            closePricePopup();
        };

        /**
        * On input price textbox key up event is trigged.
        */
        var onInputPriceKeyUp = function (e) {
            if (e.which == 13) {
                $('.teesheet-price-popup .ok-button').trigger('click');
                e.preventDefault();
            }
        };

        var openPricePopup = function ($teesheet, callback) {
            var windowWidth = $(window).width();
            var windowHeight = $(window).height();
            var docWidth = $(document).width();
            var docHeight = $(document).height();
            if ($('.teesheet-price-popup').length > 0) {
                $('.teesheet-overlay').fadeIn();
                var $popup = $('.teesheet-price-popup');
                $popup.find('.teesheet-input-price').val(eval($teesheet.data('price')).toFixed(2));
                $popup.find('.teesheet-input-discount').val(eval($teesheet.data('discount')).toFixed(2));
                $popup.find('.teesheet-input-prebooking').val(eval($teesheet.data('prebooking')));
                $('.teesheet-price-popup').fadeIn();

            } else {
                $('<div class="teesheet-overlay"></div>').appendTo('body')
                    .width(docWidth)
                    .height(docHeight)
                    .fadeIn()
                    .click(closePricePopup);
                $('<div class="teesheet-price-popup"></div>').appendTo('body');
                $('<div><label>' + window.teeSheetLocale.price + '</label><input type="number" min="0" class="teesheet-input-price form-control" /></div>').appendTo('.teesheet-price-popup');
                $('<div><label>' + window.teeSheetLocale.discount + '</label><input type="number" min="0" class="teesheet-input-discount form-control" /></div>').appendTo('.teesheet-price-popup');
                $('<div><label>' + window.teeSheetLocale.preBookingDays + '</label><input type="number" min="0" class="teesheet-input-prebooking form-control" /></div>').appendTo('.teesheet-price-popup');
                $('<div class="action-buttons"><button class="ok-button btn btn-primary pull-left">' + window.teeSheetLocale.setPrice + '</button> <button class="clear-button btn btn-default pull-left">' + window.teeSheetLocale.clearPrice + '</button><button class="cancel-button btn btn-default pull-right">' + window.teeSheetLocale.cancel + '</button></div>').appendTo('.teesheet-price-popup');
                var $popup = $('.teesheet-price-popup');
                var popupWidth = $popup.outerWidth();
                var popupHeight = $popup.outerHeight();

                $('.teesheet-price-popup .cancel-button').click(closePricePopup);
                $('.teesheet-price-popup .ok-button').click(onSetPriceButtonIsClicked);
                $('.teesheet-price-popup .clear-button').click(onClearPriceButtonIsClicked);
                
                $('.teesheet-price-popup .teesheet-input-price').val($teesheet.data('price').toFixed(2)).keyup(onInputPriceKeyUp);
                $('.teesheet-price-popup .teesheet-input-discount').val($teesheet.data('discount').toFixed(2)).keyup(onInputPriceKeyUp);
                $('.teesheet-price-popup .teesheet-input-prebooking').val($teesheet.data('prebooking')).keyup(onInputPriceKeyUp);

                $popup.css({
                    position: 'fixed',
                    left: ((windowWidth - popupWidth) / 2) + 'px',
                    top: ((windowHeight - popupHeight) / 2) + 'px'
                }).fadeIn();
            }

            $popup.data('teesheet', $teesheet);
            $popup.data('callback', callback);

            var numOnly = new RegExp("^(\\d+|\\d+[.]\\d+)$");

            //setPrice($teesheet, price, discount, prebooking);
        };

        var openPreBookingPopup = function ($teesheet, callback) {
            var windowWidth = $(window).width();
            var windowHeight = $(window).height();
            var docWidth = $(document).width();
            var docHeight = $(document).height();
            if ($('.teesheet-prebooking-popup').length > 0) {
                $('.teesheet-overlay').fadeIn();
                var $popup = $('.teesheet-prebooking-popup');
                $popup.find('.teesheet-input-prebooking').val(eval($teesheet.data('prebooking')));
                $('.teesheet-prebooking-popup').fadeIn();

            } else {
                $('<div class="teesheet-overlay"></div>').appendTo('body')
                    .width(docWidth)
                    .height(docHeight)
                    .fadeIn()
                    .click(closePricePopup);
                $('<div class="teesheet-prebooking-popup"></div>').appendTo('body');
                $('<div><label>' + window.teeSheetLocale.preBookingDays + '</label><input type="number" min="0" class="teesheet-input-prebooking form-control" /></div>').appendTo('.teesheet-prebooking-popup');
                $('<div class="action-buttons"><button class="ok-button btn btn-primary pull-left">' + window.teeSheetLocale.setPreBookingDaysContext + '</button> <button class="clear-button btn btn-default pull-left">' + window.teeSheetLocale.clearPrice + '</button><button class="cancel-button btn btn-default pull-right">' + window.teeSheetLocale.cancel + '</button></div>').appendTo('.teesheet-prebooking-popup');
                var $popup = $('.teesheet-prebooking-popup');
                var popupWidth = $popup.outerWidth();
                var popupHeight = $popup.outerHeight();

                $('.teesheet-prebooking-popup .cancel-button').click(closePreBookingPopup);
                $('.teesheet-prebooking-popup .ok-button').click(onSetPreBookingButtonIsClicked);
                $('.teesheet-prebooking-popup .clear-button').click(onClearPreBookingButtonIsClicked);

                $('.teesheet-prebooking-popup .teesheet-input-prebooking').val($teesheet.data('prebooking')).keyup(onInputPriceKeyUp);
                
                $popup.css({
                    position: 'fixed',
                    left: ((windowWidth - popupWidth) / 2) + 'px',
                    top: ((windowHeight - popupHeight) / 2) + 'px'
                }).fadeIn();
            }

            $popup.data('teesheet', $teesheet);
            $popup.data('callback', callback);

            var numOnly = new RegExp("^(\\d+|\\d+[.]\\d+)$");
        };

        var setTeeSheetData = function () {
            if (options.onTeeSheetGenerating && typeof (options.onTeeSheetGenerating) == 'function') {
                options.onTeeSheetGenerating();
            }
            var $teesheet;
            var $tr = $('.teesheet-table tbody tr');
            data = window.tsData;
            if (options.emptyText && (data == null || data == undefined || data.length == 0)) {
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
                    var date = new Date(sunday);
                    date.setDate(date.getDate() + day);
                    $teesheet.data('date', date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate());
                });

                for (var i in data) {
                    if (options.showDateNav) {
                        $teesheet = $tr.find('td[data-date=' + data[i].date + '][data-start=' + data[i].start + '][data-end=' + data[i].end + ']');
                    } else {
                        $teesheet = $tr.find('td[data-day=' + data[i].day + '][data-start=' + data[i].start + '][data-end=' + data[i].end + ']');
                    }

                    $teesheet.data('id', data[i].id);
                    $teesheet.data('date', data[i].date);
                    setPrice($teesheet, data[i].price, data[i].discount, data[i].prebooking);
                }
            }
            if (options.onTeeSheetGenerated && typeof (options.onTeeSheetGenerated) == 'function') {
                options.onTeeSheetGenerated();
            }
        };

        var generateDateText = function ($this) {
            $this.find('.teesheet-date-navigation .date-text').html(window.teeSheetLocale.from + ' ' + sunday.getDate() + ', ' + months[sunday.getMonth()] + " " + sunday.getFullYear() + ' ' + window.teeSheetLocale.to + ' ' + saturday.getDate() + ', ' + months[saturday.getMonth()] + " " + saturday.getFullYear());
            var d = undefined;
            for (var i = 0 ; i < 7; i++) {
                d = new Date(sunday);
                d.setDate(d.getDate() + i);
                $this.find('.teesheet-table thead .day-' + (i + 1) + ' .date-number').html(d.getDate())
                    .parents('th').attr('data-date', d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate());
            }
            $this.find('tbody tr').each(function () {
                for (var i = 0; i <= 7; i++) {
                    d = new Date(sunday);
                    d.setDate(d.getDate() + i);
                    $(this).find('td:eq(' + i + ')').attr('data-date', d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate());
                }
            });
        };

        var getTBodyHTML = function () {
            var html = '';
            for (var i = options.startHour; i < options.endHour; i++) {
                hourStart = i;
                hourEnd = i + 1;
                html += '<tr class="start-' + hourStart + ' end-' + hourEnd + '">';
                html += '<th class="start-' + hourStart + ' end-' + hourEnd + '">' + hourStart + 'h - ' + hourEnd + 'h</th>';
                for (var j = 1; j <= 7; j++) {
                    html += '<td class="tee-time start-' + hourStart + ' end-' + hourEnd + ' day-' + j + '" data-day="' + j + '" data-start="' + hourStart + '" data-end="' + hourEnd + '" data-price="0" data-discount="0" data-percent="0" data-prebooking="0">';
                    html += '</td>';
                }
                html += '</tr>';
            }
            return html;
        };

        var getTFootHTML = function () {
            return '<tr class="empty-row"><td class="empty-cell" colspan="8">' + options.emptyText.trim() + '</td></tr>';
        };

        var clearTeeSheets = function ($tableBody) {
            if (options.onClearTeeSheet) {
                $tableBody.find('tr td').each(function (i) {
                    $(this).html('')
                    .data('price', 0)
                    .data('discount', 0)
                    .data('percent', 0)
                    .data('prebooking', 0);
                    options.onClearTeeSheet($(this));
                });
            } else {
                $tableBody.find('tr td').html('')
                    .data('price', 0)
                    .data('discount', 0)
                    .data('percent', 0)
                    .data('prebooking', 0);
            }
        }

        var refreshDataSource = function () {
            var dataStr = jQuery('input[name=TeeSheetData]').val();
            var newData = JSON.parse(dataStr);
            window.tsData = data = newData.teeSheets;
        }

        // Document Load
        $(document).ready(function () {
            $(window).keydown(onTeeSheetKeyDown)
            .keyup(onTeeSheetKeyUp);
            $('*').click(onContainerDocumentIsClicked);
        });

        if (typeof (o) == "string") {
            data = window.tsData;
            eval(o + '.call(this, ' + a + ',' + b + ',' + c + ',' + d + ',' + e + ')');
             //eval(options + '.apply(' + a + ',' + b + ',' + c + ',' + d + ',' + e + ');');
        } else {

            var defaultOptions = {
                showDateNav: true,
                data: new Array(),
                onGenerateDayOfWeek: function (daysOfWeek, i) {
                    var d = new Date(sunday);
                    d.setDate(d.getDate() + i);
                    return daysOfWeek[i] + (options.showDateNav ? ' <span class="date-number">' + (d.getDate()) + '</span>' : '');
                },
                onTeeSheetClicking: function ($teesheet) {
                    return true;
                },
                onClearTeeSheet: function ($teesheet) {

                },
                onTeeSheetGenerating: function () {

                },
                onTeeSheetGenerated: function () {

                },
                emptyText: ''
            };
            options = $.extend({}, defaultOptions, options);
            data = options.data && typeof (options.data) == "object" ? options.data : new Array();
            window.tsData = data;
            return this.each(function () {
                //$.extends(defaultOptions, {}, options);

                var html = '';
                
                var dayOfWeeks = [
                    window.teeSheetLocale.monday,
                    window.teeSheetLocale.tuesday,
                    window.teeSheetLocale.wednesday,
                    window.teeSheetLocale.thursday,
                    window.teeSheetLocale.friday,
                    window.teeSheetLocale.saturday,
                    window.teeSheetLocale.sunday
                ];

                var $this = $(this);

                $this.addClass('teesheet-container');

                $this.html('<input type="hidden" name="TeeSheetData" value="{\"teeSheets\": []}" />');
                if (options.showDateNav) {
                    $this.append('<div class="teesheet-date-navigation"><div class="row"><div class="col-md-3 text-left"><a href="javascript:void(0)" class="go-back-link">' + window.teeSheetLocale.previous7Days + '</a></div><div class="col-md-6 text-center"><strong><span class="date-text"></span></strong></div><div class="col-md-3 text-right"><a href="javascript:void(0)" class="go-next-link">' + window.teeSheetLocale.next7Days + '</a></div></div></div>');
                }
                $this.append('<table class="teesheet-table"><thead><tr></tr></thead><tbody></tbody><tfoot></tfoot></table>');
                var $table = $(this).find('.teesheet-table');
                var $tableHeader = $table.find('thead');
                var $tableBody = $table.find('tbody');
                var $tableFooter = $table.find('tfoot');

                // Create table header.
                var $tableHeaderRow = $tableHeader.find('tr');
                $tableHeaderRow.append("<th>&nbsp;</th>");
                for (var i = 0; i < 7; i++) {
                    $tableHeaderRow.append('<th class="day-' + (i + 1) + '">' + options.onGenerateDayOfWeek(dayOfWeeks, i) + '</th>');
                }

                // Generate Rows.
                /*var times = getTimePeriods(options.startHour, options.endHour, options.duration, 12);
                var t = undefined;
                html = '';
                for (var i in times) {
                    t = times[i];
                    if (t.type == 'break') {
                        tempDuration = options.duration;
                        html += '<tr class="start-' + t.start + ' end-' + t.end + '">';
                        html += '<th class="start-' + t.start + ' end-' + t.end + '">' + t.start + 'h - ' + t.end + 'h</th>';
                        html += '<td colspan="7" class="break-time">&nbsp;</td>';
                        html += '</tr>';
                        continue;
                    } else {
                        html += '<tr class="start-' + t.start + ' end-' + t.end + '">';
                        html += '<th class="start-' + t.start + ' end-' + t.end + '">' + t.start + 'h - ' + t.end + 'h</th>';
                        for (var j = 1; j <= 7; j++) {
                            html += '<td class="tee-time start-' + t.start + ' end-' + t.end + ' day-' + j + '" data-day="' + j + '" data-start="' + t.start + '" data-end="' + t.end + '" data-price="0" data-discount="0" data-percent="0" data-prebooking="0">';
                            html += '</td>';
                        }
                        html += '</tr>';
                        tempDuration = 0;
                    }
                }*/
                var hourStart = 0;
                var hourEnd = 0;
                html = getTBodyHTML();
                $tableBody.html(html);
                $tableFooter.html(getTFootHTML());

                // Add Pricing.
                $tableBody.find('td').each(function (i) {
                    if (!$(this).hasClass('break-time')) {
                        $(this).click(onTeeSheetIsClicked).bind('contextmenu', onTeeSheetContextMenu);
                        //setPrice($(this), eval(options.defaultPrice), 0, 0);
                    }
                });

                // Add Events for Date Navigation.
                $this.find('.teesheet-date-navigation .go-back-link').click(function () {
                    sunday = getMondayFromDate(sunday);
                    sunday.setDate(sunday.getDate() - 7);
                    saturday.setDate(saturday.getDate() - 7);
                    generateDateText($this);
                    clearTeeSheets($tableBody);
                    setTeeSheetData();
                });

                $this.find('.teesheet-date-navigation .go-next-link').click(function () {
                    sunday = getMondayFromDate(sunday);
                    sunday.setDate(sunday.getDate() + 7);
                    saturday.setDate(saturday.getDate() + 7);
                    generateDateText($this);
                    clearTeeSheets($tableBody);
                    setTeeSheetData();
                });
                generateDateText($this);
                setTeeSheetData();
                refresh();

                // Create context menu.
                if ($('.teesheet-contextmenu').length <= 0) {
                    $('<ul class="teesheet-contextmenu"></ul>').appendTo('body').css({
                        'position': 'absolute',
                        'z-index': 9999,
                        'display': 'none'
                    });
                    $('<li class="teesheet-setprice-button"><a href="javascript:void(0)">' + window.teeSheetLocale.setPriceContext + '</a></li>').appendTo('ul.teesheet-contextmenu').find('a').click(onContextMenuSetPriceIsClicked);
                    $('<li class="teesheet-setprebooking-button"><a href="javascript:void(0)">' + window.teeSheetLocale.setPreBookingDaysContext + '</a></li>').appendTo('ul.teesheet-contextmenu').find('a').click(onContextMenuSetPreBookingDayIsClicked);
                    $('<li class="teesheet-clearprice-button"><a href="javascript:void(0)">' + window.teeSheetLocale.clearPrice + '</a></li>').appendTo('ul.teesheet-contextmenu').find('a').click(onContextMenuClearPriceIsClicked);
                }
            });
        }
    };

    $.fn.teeSheet = TeeSheet;
})(jQuery);