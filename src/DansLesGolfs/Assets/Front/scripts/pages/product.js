var regex = new RegExp(/[^0-9]+/);

// Custom Validation Rules
jQuery.validator.addMethod("less_than", function (value, element, params) {
    return this.optional(element) || value <= $(param).val();
}, "This value must less than {1}");

jQuery(document).ready(function ($) {
    $('#reviews-list').mCustomScrollbar({
        mouseWheelPixels: 1000
    });

    $('#item-qty').change(updateItemPrice).keyup(updateItemPrice);

    $('#add-to-cart').click(onAddToCartButtonIsClicked);

    // Rating Callback.
    $('.starrr').off('starrr:change').on('starrr:change', onUserGiveRating);

    $('#open-rating-popup-button, #open-rating-finish-button').fancybox({
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        }
    });

    $('#product-popup-link, #error-link').fancybox({
        padding: 0,
        modal: true,
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        }
    });
    $('#help-popup, #error-popup .ok-button').click(function () {
        $.fancybox.close();
    });
    jQuery('#submit-review-button').unbind('click').click(onSubmitRating);

    // Fixing RoyalSlider Thumbnail Arror.
    $('.rsThumbsArrowLeft').click(function () { $('.rsArrowLeft').trigger('click'); });
    $('.rsThumbsArrowRight').click(function () { $('.rsArrowRight').trigger('click'); });

    //$('#reserve-date').change(onReserveDateChanged);

    var calendarColor = getCalendarColor();
    $('#reserve-date').datepicker({
        dateFormat: 'dd/mm/yy',
        minDate: window.itemMinDate,
        maxDate: window.itemMaxDate,
        firstDay: 1,
        autoSize: true,
        showOtherMonths: true,
        selectOtherMonths: true,
        showOn: 'both',
        buttonImageOnly: true,
        buttonImage: getUrl('Assets/Front/img/icon-datepicker-calendar-' + calendarColor + '.png'),
        onSelect: onReserveDateSelect,
        beforeShow: function (input, inst) {
            var cal = inst.dpDiv;
            var top = $(this).offset().top + $(this).outerHeight();
            var left = $(this).offset().left;
            setTimeout(function () {
                cal.css({
                    'top': top,
                    'left': left
                });
            }, 10);
        }
    }).attr('readonly', 'readonly').val(window.reserveDateDefaultText);


    initItemImageSlider();
});

/************************ Functions ******************************/
function updateItemPrice(e) {
    var $this = $(this);
    $this.val($this.val().replace(regex, ''));

    if ($this.val().trim() == '')
        $this.val(1);

    var $itemInfo = $('#item-info');
    var price = $itemInfo.data('price');
    var specialPrice = $itemInfo.data('special-price');
    var periodPrice = $itemInfo.data('period-price');
    var cheapestPeriodPrice = $itemInfo.data('cheapest-period-price');
    var hasPeriodPrice = $itemInfo.data('has-period-price');
    var hasSpecialPrice = $itemInfo.data('has-special-price');
    var hasCheapestPeriodPrice = $itemInfo.data('has-cheapest-period-price');
    var cheapestPrice = $itemInfo.data('cheapest-price');
    try {
        if (hasSpecialPrice) {
            if (hasPeriodPrice) {
                $('#item-pricing .regular-price-number').text(eval($this.val() * periodPrice).toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }));
            } else {
                $('#item-pricing .regular-price-number').text(eval($this.val() * price).toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }));
            }
            $('#item-pricing .item-price-number').text(eval($this.val() * specialPrice).toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }));
        } else if (hasPeriodPrice) {
            $('#item-pricing .item-price-number').text(eval($this.val() * periodPrice).toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }));
            if (price > 0) {
                $('#item-pricing .regular-price-number').text(eval($this.val() * price).toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }));
            }
        } else {
            if (price > 0) {
                $('#item-pricing .item-price-number').text(eval($this.val() * price).toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }));
            } else {
                $('#item-pricing .item-price-number').text(window.notAvailableText);
            }
        }
    } catch (ex) {
        //alert(ex.message);
    }
}

function initItemImageSlider() {
    $.ajax({
        url: getUrl('ItemAjax/AjaxGetImageSlider'),
        type: 'get',
        data: { id: window.itemId },
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                $('#item-images-slider').html(result.html).css('display', 'block')
                $('#item-images-slider').royalSlider({
                    keyboardNavEnabled: true,
                    imageScaleMode: 'fill',
                    imageScalePadding: 0,
                    controlNavigation: 'thumbnails',
                    autoScaleSlider: false,
                    autoHeight: false,
                    loop: true,
                    autoPlay: {
                        enabled: true,
                        pauseOnHover: true
                    },
                    thumbs: {
                        arrows: true
                    }
                });
            }
        }
    });
}

/************************ Event Functions ******************************/
/**
* trigger on user clicks on "Add To Cart" button.
* @param e      Event Argument.
*/
function onAddToCartButtonIsClicked(e) {
    var qty = $('#item-qty').val();
    var cheapestPrice = jQuery('#item-info').data('cheapest-price');
    if (!qty || qty <= 0) {
        return false;
    }
    if (!cheapestPrice || cheapestPrice <= 0) {
        return false;
    }

    var itemId = jQuery('#item-info').data('id');
    var reserveDate = jQuery('#reserve-date').val();
    jQuery.ajax({
        url: getUrl('Cart/AddItem'),
        type: 'post',
        dataType: 'json',
        data: { itemId: itemId, quantity: qty, reserveDate: reserveDate },
        beforeSend: function () {
            showLoader(true);
        },
        success: function (result) {
            if (result.isSuccess) {
                //window.location = getUrl('cart');
                onAddToCartSuccess(itemId, function () {
                    showAddedItemPopup(qty);
                    showLoader(false);
                });
            }
        }
    });
    //if (window.itemType == 'product green-fee' && window.siteId > 0) {
    //    var hasDate = jQuery('#reserve-date').val() != window.reserveDateDefaultText;
    //    window.location = getUrl('teesheet/table/' + window.itemSlug + ((hasDate) ? '?date=' + jQuery('#reserve-date').val() : ""));
    //} else {
    //    jQuery.ajax({
    //        url: getUrl('Cart/AddItem'),
    //        type: 'post',
    //        dataType: 'json',
    //        data: { itemId: jQuery('#item-info').data('id'), quantity: qty, reserveDate: jQuery('#reserve-date').val() },
    //        beforeSend: function() {
    //            showLoader(true);
    //        },
    //        success: function (result) {
    //            if (result.isSuccess) {
    //                //window.location = getUrl('cart');
    //                showAddedItemPopup(qty);
    //                showLoader(false);
    //            }
    //        }
    //    });
    //}
}

function showAddedItemPopup(qty) {
    jQuery('#product-popup .popup-content .item-qty').text(qty);
    jQuery('#product-popup-link').click();
}

//function onReserveDateChanged(e) {
//    jQuery.ajax({
//        url: getUrl('ItemAjax/GetPriceByDate'),
//        type: 'post',
//        dataType: 'json',
//        data: { itemId: jQuery('#item-info').data('id'), date: jQuery(this).val() },
//        success: function (result) {
//            if (result.isSuccess) {
//                changePrice(result.price, result.periodPrice, result.specialPrice, result.cheapestPrice, result.cheapestPeriodPrice);
//            }
//        }
//    });
//}

function onReserveDateSelect(text) {
    var qty = jQuery('#item-qty').val();
    jQuery.ajax({
        url: getUrl('ItemAjax/GetPriceByDate'),
        type: 'post',
        dataType: 'json',
        data: { itemId: jQuery('#item-info').data('id'), date: text, qty: qty },
        beforeSend: function (xhr, opts) {
            showLoader(true, '#page-item-detail');
        },
        success: function (result) {
            showLoader(false, '#page-item-detail');
            if (result.isSuccess) {
                //jQuery('#item-info .product-price').replaceWith(result.priceTagHTML);
                jQuery('#item-pricing').show();
                jQuery('#item-price-number').html(result.checkOutPriceHTML);
                changePrice(result.price, result.periodPrice, result.specialPrice, result.cheapestPrice, result.cheapestPeriodPrice);
            }
        }
    });
}

function changePrice(price, periodPrice, specialPrice, cheapestPrice, cheapestPeriodPrice, hasTeeSheet) {
    jQuery('#item-info')
        .data('price', price)
        .data('period-price', periodPrice)
        .data('cheapest-period-price', cheapestPeriodPrice)
        .data('special-price', specialPrice)
        .data('cheapest-price', cheapestPrice)
        .data('has-period-price', periodPrice > 0)
        .data('has-special-price', specialPrice > 0)
        .data('has-cheapest-period-price', cheapestPeriodPrice > 0);

    if (price <= 0 && periodPrice <= 0 && specialPrice <= 0) {
        jQuery('#add-to-cart').addClass('disabled');
    } else {
        jQuery('#add-to-cart').removeClass('disabled');
    }

    //var qty = jQuery('#item-qty').val();

    //var p = price * qty;
    //var pp = periodPrice * qty;
    //var sp = specialPrice * qty;
    //var cp = cheapestPrice * qty;
    //var cpp = cheapestPeriodPrice * qty;

    //var html = '';
    //generatePriceTag(price, periodPrice, specialPrice, cheapestPrice, cheapestPeriodPrice, hasTeeSheet);
    //generateCheckOutPrice(p, pp, sp, cp, cpp);
}

function generatePriceTag(price, periodPrice, specialPrice, cheapestPrice, cheapestPeriodPrice, hasTeeSheet) {
    var html = '';
    if (specialPrice > 0) {
        html += '<span class="special-pricing">';
        if (cheapestPeriodPrice > 0) {
            html += '<span class="starting-at">' + window.priceBeginText + '</span>';
            html += '<span class="regular-price-number">' + cheapestPeriodPrice.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '&euro;</span>';
            html += '<span class="price-number">' + specialPrice.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '&euro;</span>';
        } else {
            if (hasTeeSheet) {
                html += '<span class="starting-at">' + window.priceBeginText + '</span>';
            }
            html += '<span class="regular-price-number">' + price.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '&euro;</span>';
            html += '<span class="price-number">' + specialPrice.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '&euro;</span>';
        }
        html += '</span>';
    }
    else if (periodPrice > 0) {
        if (price > 0) {
            html += '<span class="special-pricing">';
            html += '<span class="starting-at">' + window.priceBeginText + '</span>';
            html += '<span class="regular-price-number">' + price.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '&euro;</span>';
            html += '<span class="price-number">' + periodPrice.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '&euro;</span>';
            html += '</span>';
        } else {
            html += '<span class="starting-at">' + window.priceBeginText + '</span>';
            html += '<span class="price-number">' + cheapestPeriodPrice.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '&euro;</span>';
        }
    } else {
        if (hasTeeSheet) {
            html += '<span class="starting-at">' + window.priceBeginText + '</span>';
            html += '<span class="price-number">' + price.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '&euro;</span>';
        }
        else if (cheapestPeriodPrice > 0) {
            html += '<span class="starting-at">' + window.priceBeginText + '</span>';
            html += '<span class="price-number">' + cheapestPeriodPrice.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '&euro;</span>';
        }
        else {
            html += '<span class="price-number">' + price.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + ' &euro;</span>';
        }
    }
    jQuery('#item-info .product-price .product-price-wrapper').html(html);
}

function generateCheckOutPrice(p, pp, sp, cp, cpp, notAvailableText) {
    if (sp > 0) {
        if (pp > 0) {
            // Checkout price
            jQuery('#item-price-number').html('<span class="regular-pricing"><span class="regular-price-number">' + pp.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '</span> &euro;</span>');
            jQuery('#item-price-number').append('<span class="item-pricing"><span class="item-price-number">' + sp.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '</span> &euro;</span>');
        } else {
            // Checkout price
            jQuery('#item-price-number').html('<span class="regular-pricing"><span class="regular-price-number">' + p.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '</span> &euro;</span>');
            jQuery('#item-price-number').append('<span class="item-pricing"><span class="item-price-number">' + sp.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '</span> &euro;</span>');
        }
    }
    else if (pp > 0) {
        if (p > 0) {
            // Checkout Price
            jQuery('#item-price-number').html('<span class="regular-pricing"><span class="regular-price-number">' + p.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '</span> &euro;</span>');
            jQuery('#item-price-number').append('<span class="item-pricing"><span class="item-price-number">' + pp.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '</span> &euro;</span>');
        } else {
            // Checkout Price
            jQuery('#item-price-number').html('<span class="item-pricing"><span class="item-price-number">' + pp.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '</span> &euro;</span>');
        }
    } else {
        // Checkout Price
        if (p > 0) {
            jQuery('#item-price-number').html('<span class="item-pricing"><span class="item-price-number">' + p.toLocaleString('fr', { minimumFractionDigits: 2, useGrouping: false }) + '</span> &euro;</span>');
        } else {
            jQuery('#item-price-number').html('<span class="item-pricing"><span class="item-price-number">' + window.notAvailableText + '</span></span>');
        }
    }
}

/**************************** Item Rating *****************************/
function onUserGiveRating(e, value) {
    e.stopPropagation();
    window.rating = value;
    jQuery('#rating-popup .review-stars').data('rating', value);
    jQuery('#rating-popup .review-stars .star').removeClass('active');
    for (var i = 0; i < value; i++) {
        jQuery('#rating-popup .review-stars .star').eq(i).addClass('active');
    }
    jQuery.ajax({
        url: getUrl('Common/CheckLogin'),
        type: 'post',
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess && result.isLoggedIn) {
                jQuery('#open-rating-popup-button').click();
                jQuery('#review-message-input').select();
            } else {
                window.location = getUrl('Login?returnUrl=' + encodeURI(window.location.href));
            }
        }
    });
}

function onSubmitRating(e) {
    e.stopPropagation();

    var subject = jQuery('#review-subject-input').val();
    var message = jQuery('#review-message-input').val();

    jQuery.ajax({
        url: getUrl('ItemAjax/UserGiveRating'),
        data: { itemId: window.itemId, rating: jQuery('#rating-popup .review-stars').data('rating'), subject: subject, message: message },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            $.fancybox.close();
            if (result.isSuccess) {
                jQuery('#item-reviews #reviews-list .mCSB_container').html(result.html);
                jQuery('#item-reviews #reviews-list').mCustomScrollbar('update');
                jQuery("#item-reviews #reviews-list").mCustomScrollbar("scrollTo", "top", { scrollInertia: 200 });
                jQuery('.review-panel .starrr').data('rating', result.averageRating);
                jQuery('.review-panel .starrr .star').removeClass('active');
                jQuery('.review-panel .starrr').each(function () {
                    for (var i = 0; i < result.averageRating; i++) {
                        jQuery(this).find('.star').eq(i).addClass('active');
                    }
                });
                jQuery('.review-number').text(result.reviewNumber);
                $('#review-message-input').val('');
                $("#reviews-list .starrr").starrr();

                $('#open-rating-finish-button').trigger('click');
            }
        }
    });
}

function getCalendarColor() {
    var color = 'green';
    switch (window.itemType) {
        case 'product green-fee': color = 'green';
            break;
        case 'product stay-package': color = 'bluesky';
            break;
        case 'product golf-lesson': color = 'blue';
            break;
        case 'product driving-range': color = 'darkgreen';
            break;
        case 'product reseller-product': color = 'pink';
            break;
    }
    return color;
}


/**************************** Conversion Tracking *****************************/
function onAddToCartSuccess(id, callback) {
    jQuery.ajax({
        url: getUrl('ItemAjax/AjaxTriggerConversionTracking'),
        type: 'post',
        data: { id: id, type: 'addtocart' },
        success: function () {
            if (callback && typeof(callback) == "function") {
                callback();
            }
        },
        error: function (xhr, msg) {
            if (callback && typeof (callback) == "function") {
                callback();
            }
        }
    });
}