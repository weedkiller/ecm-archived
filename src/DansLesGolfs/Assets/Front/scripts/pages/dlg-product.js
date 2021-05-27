var regex = new RegExp(/[^0-9]+/);
jQuery(document).ready(function ($) {

    $('#item-qty').change(updateItemPrice).keyup(updateItemPrice);

    $('#buy-button').click(onAddToCartButtonIsClicked);

    // Rating Callback.
    $('.starrr').on('starrr:change', onUserGiveRating);

    $('#open-rating-popup-button, #post-comment-link, #send-offer-popup-link').fancybox({
        autoScale: true,
        minHeight: 20,
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        }
    });

    $('#product-popup-link').fancybox({
        modal: true,
        padding: 0
    });

    $('#close-rating-popup-button, #close-message-popup-button, #continue-to-shopping').click(function () {
        $.fancybox.close();
    });
    $('#submit-review-button').click(onSubmitRating);

    $('#send-email-button').click(onSendEmailButtonIsClicked);

    $('#print-button').click(function () {
        $('#print_document').remove();
        var $frame = $('<iframe id="print_document" name="print_document" style="width:100%;height:0;display:none;"></iframe>').appendTo('body');
        $frame.attr('src', window.printUrl);
        window.frames['print_document'].focus();
    });

    $('#item-specifications, #reviews-list').mCustomScrollbar({
        mouseWheelPixels: 100
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
        alert(ex.message);
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
                //window.location = getUrl('cart');
                showAddedItemPopup(qty);
            } else {
                alert(result.message);
            }
        }
    });
}

function showAddedItemPopup(qty) {
    jQuery('#product-popup .popup-content .item-qty').text(qty);
    jQuery('#product-popup-link').click();
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
            }
        }
    });
}

function onSendEmailButtonIsClicked(e) {
    var name = jQuery('#txtRecipientName').val();
    var email = jQuery('#txtRecipientEmail').val();
    var message = jQuery('#txtMessageToFriend').val();
    
    jQuery.ajax({
        url: getUrl('ItemAjax/SendEmailItemInfo'),
        data: { itemId: jQuery('#item-info').data('id'), name: name, email: email },
        dataType: 'json',
        type: 'post',
        beforeSend: function(xhr, opts) {
            showLoader(true, 'body');
        },
        success: function (result) {
            showLoader(false, 'body');
            if (result.isSuccess) {
                $.fancybox.close();
            }
        },
        error: function (xhr, msg) {
            showLoader(false, 'body');
        }
    });
}