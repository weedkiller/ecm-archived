var regex = new RegExp(/[^0-9]+/);
jQuery(document).ready(function ($) {
    $(".royalSlider").royalSlider({
        keyboardNavEnabled: true,
        imageScaleMode: 'fill',
        imageScalePadding: 0,
        controlNavigation: 'thumbnails',
        loop: true,
        autoPlay: {
            enabled: true,
            pauseOnHover: true
        },
        thumbs: {
            arrows: true
        }
    });

    $('#reviews-list').mCustomScrollbar({
        mouseWheelPixels: 100
    });

    $('#item-qty').change(updateItemPrice).keyup(updateItemPrice);

    $('#add-to-cart').click(onAddToCartButtonIsClicked);

    // Rating Callback.
    $('.starrr').on('starrr:change', onUserGiveRating);

    $('#open-rating-popup-button, #send-email-popup').fancybox({
        modal: true
    });
    $('#close-rating-popup-button').click(function () {
        $.fancybox.close();
    });
    jQuery('#submit-review-button').click(onSubmitRating);

    // Fixing RoyalSlider Thumbnail Arror.
    $('.rsThumbsArrowLeft').click(function () { $('.rsArrowLeft').trigger('click'); });
    $('.rsThumbsArrowRight').click(function () { $('.rsArrowRight').trigger('click'); });

    $('#reserve-date').change(onReserveDateChanged);

    var calendarColor = getCalendarColor();
    $('#reserve-date').datepicker('option', 'onSelect', onReserveDateSelect);
    $('#reserve-date').datepicker('option', 'buttonImage', getUrl('Assets/Front/img/icon-datepicker-calendar-' + calendarColor + '.png'));
});

/************************ Functions ******************************/
function updateItemPrice(e) {
    var $this = $(this);
    $this.val($this.val().replace(regex, ''));

    if ($this.val().trim() == '')
        $this.val(1);

    var $itemInfo = $('#item-info');
    var price = $itemInfo.data('price');
    //var specialPrice = $itemInfo.data('special-price');
    //var periodPrice = $itemInfo.data('period-price');
    var cheapestPrice = $itemInfo.data('cheapest-price');
    //var hasPeriodPrice = $itemInfo.data('has-period-price');
    //var hasSpecialPrice = $itemInfo.data('has-special-price');
    try {
        //if (hasSpecialPrice) {
        //    $('.regular-price-number').text(eval($this.val() * price));
        //    $('.item-price-number').text(eval($this.val() * specialPrice));
        //} else if (hasPeriodPrice) {
        //    $('.regular-price-number').text(eval($this.val() * price));
        //    $('.item-price-number').text(eval($this.val() * periodPrice));
        //} else{
        //    $('.item-price-number').text(eval($this.val() * price));
        //}
        if (cheapestPrice == price) {
            $('#item-pricing .item-price-number').text(eval($this.val() * price));
        } else {
            $('#item-pricing .regular-price-number').text(eval($this.val() * price));
            $('#item-pricing .item-price-number').text(eval($this.val() * cheapestPrice));
        }
    } catch (ex) {
        console.error(ex.message);
    }
}

/************************ Event Functions ******************************/
/**
* trigger on user clicks on "Add To Cart" button.
* @param e      Event Argument.
*/
function onAddToCartButtonIsClicked(e) {
    var qty = $('#item-qty').val();
    if (!qty || qty < 0) {
        return false;
    }

    if (window.itemType == 'product green-fee') {
        window.location = getUrl('teesheet/table/' + window.itemSlug + '?date=' + jQuery('#reserve-date').val());
    } else {
        jQuery.ajax({
            url: getUrl('Cart/AddItem'),
            type: 'post',
            dataType: 'json',
            data: { itemId: jQuery('#item-info').data('id'), quantity: qty, reserveDate: jQuery('#reserve-date').val() },
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
}

function onReserveDateChanged(e) {
    jQuery.ajax({
        url: getUrl('ItemAjax/GetPriceByDate'),
        type: 'post',
        dataType: 'json',
        data: { itemId: jQuery('#item-info').data('id'), date: jQuery(this).val() },
        success: function (result) {
            if (result.isSuccess) {
                changePrice(result.price, result.periodPrice, result.specialPrice, result.cheapestPrice);
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
        }
    });
}

function onReserveDateSelect(text) {
    jQuery.ajax({
        url: getUrl('ItemAjax/GetPriceByDate'),
        type: 'post',
        dataType: 'json',
        data: { itemId: jQuery('#item-info').data('id'), date: text },
        beforeSend: function (xhr, opts) {
            showLoader(true, '#page-item-detail');
        },
        success: function (result) {
            showLoader(false, '#page-item-detail');
            if (result.isSuccess) {
                changePrice(result.price, result.periodPrice, result.specialPrice, result.cheapestPrice);
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#page-item-detail');
            console.error(xhr);
        }
    });
}

function changePrice(price, periodPrice, specialPrice, cheapestPrice) {
    jQuery('#item-info')
        .data('price', price)
        .data('period-price', periodPrice)
        .data('special-price', specialPrice)
        .data('cheapest-price', cheapestPrice)
        .data('has-period-price', periodPrice > 0)
        .data('has-special-price', specialPrice > 0);

    var qty = jQuery('#item-qty').val();

    var p = price * qty;
    var pp = periodPrice * qty;
    var sp = specialPrice * qty;
    var cp = cheapestPrice * qty;

    if (cheapestPrice == price) {
        // Checkout price
        jQuery('#item-price-number').html('<span class="item-pricing"><span class="item-price-number">' + p + '</span> &euro;</span>');

        // Price Tag
        jQuery('#item-info .product-price .product-price-wrapper').html('<span class="price-number">' + price + ' &euro;</span>');
    } else {
        // Checkout price
        jQuery('#item-price-number').html('<span class="regular-pricing"><span class="regular-price-number">' + p + '</span> &euro;</span>');
        jQuery('#item-price-number').append('<span class="item-pricing"><span class="item-price-number">' + cp + '</span> &euro;</span>');

        // Price Tag
        var html = "";
        if (cheapestPrice == price) {
            html += '<span class="price-number">' + price + '</span>';
        } else {
            if (specialPrice > 0) {
                html = '<span class="special-pricing">';
                if (periodPrice > 0) {
                    html += '<span class="starting-at">' + window.priceBeginText + '</span>';
                } console.log(price);
                html += '<span class="regular-price-number">' + price + '</span>';
                html += '<span class="price-number">' + cheapestPrice + ' &euro;</span>';
                html += '</span>';
            } else if (periodPrice > 0) {
                html += '<span class="starting-at">' + window.priceBeginText + '</span>';
                html += '<span class="price-number">' + price + '</span>';
            }
        }
        jQuery('#item-info .product-price .product-price-wrapper').html(html);
    }
}

/**************************** Item Rating *****************************/
function onUserGiveRating(e, value) {
    window.rating = value;
    jQuery('#open-rating-popup-button').click();
    jQuery('#review-message-input').select();
}

function onSubmitRating() {
    var message = jQuery('#review-message-input').val();
    jQuery.ajax({
        url: getUrl('ItemAjax/UserGiveRating'),
        data: { itemId: window.itemId, rating: window.rating, message: message },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            $.fancybox.close();
            if (result.isSuccess) {
                jQuery('#item-reviews #reviews-list .mCSB_container').html(result.html);
                jQuery('#item-reviews #reviews-list').mCustomScrollbar('update');
                jQuery("#item-reviews #reviews-list").mCustomScrollbar("scrollTo", "top", { scrollInertia: 200 });
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
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