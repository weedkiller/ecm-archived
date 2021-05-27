var regex = new RegExp(/[^0-9]+/);
jQuery(document).ready(function ($) {

    $('#item-qty').change(updateItemPrice).keyup(updateItemPrice);

    $('#buy-button').click(onAddToCartButtonIsClicked);

    // Rating Callback.
    $('.starrr').on('starrr:change', onUserGiveRating);

    $('#open-rating-popup-button, #post-comment-link, #send-email-popup-link').fancybox({
        modal: true
    });
    $('#close-rating-popup-button, #close-message-popup-button').click(function () {
        $.fancybox.close();
    });
    $('#submit-review-button').click(onSubmitRating);

    $('#send-email-button').click(onSendEmailButtonIsClicked);

    $('#print-button').click(function () {
        window.print();
    });
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
    var hasSpecialPrice = $itemInfo.data('has-special-price');
    try {
        if (hasSpecialPrice) {
            $('.regular-price-number').text(eval($this.val() * price));
            $('.item-price-number').text(eval($this.val() * specialPrice));
        } else {
            $('.item-price-number').text(eval($this.val() * price));
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
    var qty = $('#quantity').val();
    if (!qty || qty < 0) {
        return false;
    }

    jQuery.ajax({
        url: getUrl('Cart/AddItem'),
        type: 'post',
        dataType: 'json',
        data: { itemId: jQuery('#item-info').data('id'), quantity: qty },
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
                jQuery('#item-reviews #reviews-list').html('<div class="content">' + result.html + '</div>').mCustomScrollbar('update');
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
        }
    });
}

function onSendEmailButtonIsClicked(e) {
    var name = jQuery('#txtRecipientName').val();
    var email = jQuery('#txtRecipientEmail').val();
    var message = jQuery('#txtMessageToFriend').val();
    console.log(name, email, message);
    jQuery.ajax({
        url: getUrl('ItemAjax/SendEmailItemInfo'),
        data: { itemId: jQuery('#item-info').data('id'), name: name, email: email, message: message },
        dataType: 'json',
        type: 'post',
        beforeSend: function(xhr, opts) {
            showLoader(true, 'body');
        },
        success: function (result) {
            showLoader(false, 'body');
            if (result.isSuccess) {
                $.fancybox.close();
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            showLoader(false, 'body');
            console.error(xhr);
        }
    });
}

function afterOWLinit() {

    // adding A to div.owl-page
    $('.owl-controls .owl-page').append('<a class="item-link" href="#"/>');

    var pafinatorsLink = $('.owl-controls .item-link');

    /**
     * this.owl.userItems - it's your HTML <div class="item"><img src="http://www.ow...t of us"></div>
     */
    $.each(this.owl.userItems, function (i) {

        $(pafinatorsLink[i])
            // i - counter
            // Give some styles and set background image for pagination item
            .css({
                'background': 'url(' + $(this).find('img').attr('src') + ') center center no-repeat',
                '-webkit-background-size': 'cover',
                '-moz-background-size': 'cover',
                '-o-background-size': 'cover',
                'background-size': 'cover'
            })
            // set Custom Event for pagination item
            .click(function () {
                owl.trigger('owl.goTo', i);
            });

    });



    // add Custom PREV NEXT controls
    $('.owl-pagination').prepend('<a href="#prev" class="prev-owl"/>');
    $('.owl-pagination').append('<a href="#next" class="next-owl"/>');


    // set Custom event for NEXT custom control
    $(".next-owl").click(function () {
        owl.trigger('owl.next');
    });

    // set Custom event for PREV custom control
    $(".prev-owl").click(function () {
        owl.trigger('owl.prev');
    });

}