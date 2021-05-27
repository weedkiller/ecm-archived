jQuery(document).ready(function ($) {
    $('#cart-summary .column-quantity input').change(onInputQuantityChanged).keyup(onInputQuantityChanged).numberEditor({
        min: 1
    });
    $('#cart-summary tbody .column-delete .delete-button').click(onCartItemDeleteButtonIsClicked);
    $('#apply-coupon-button').click(onApplyCouponButtonIsClicked);

    $('#error-popup-link').fancybox({
        autoScale: true,
        modal: true,
        minHeight: 20,
        width: 320,
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        }
    });

    $('#error-popup .ok-button').click(function () { $.fancybox.close(); });
});

function onInputQuantityChanged(e) {
    if (this.value <= 0) {
        this.value = 1;
    }

    var regex = new RegExp(/[^0-9]+/);
    var $this = $(this);
    $this.val($this.val().replace(regex, ''));

    if ($this.val().trim() == '' || $this.val().trim() < 0)
        $this.val(0);

    setItemQuantity($this.parents('tr').data('id'), $this.val());
    updateCartPrices();
}
function updateCartPrices() {
    var sum = 0;
    var $row = null;
    var unitPrice = 0;
    var shippingCost = 0;
    var discount = 0;
    var quantity = 0;
    var totalPrice = 0;
    var totalShippingCost = 0;
    var totalDiscount = 0;
    var $items = jQuery('#cart-summary .column-quantity input');
    var discountMinimumAmount = jQuery('#DiscountMinimumAmount').val();
    if ($items.length > 0) {
        $items.each(function (i) {
            $row = jQuery(this).parents('tr');
            quantity = eval(jQuery(this).val());
            unitPrice = eval($row.data('unit-price'));
            totalPrice = unitPrice * quantity;
            shippingCost = eval($row.data('shipping-cost'));
            if(discountMinimumAmount > 0)
            {
                console.log(totalPrice);
                discount = totalPrice >= discountMinimumAmount ? eval($row.data('discount')) : 0;
            } else {
                discount = eval($row.data('discount'));
            }
            sum += totalPrice;
            totalDiscount += discount * quantity;
            totalShippingCost += shippingCost * quantity;
            $row.find('.column-total-price .price-number').text(totalPrice.toLocaleString('fr', { minimumFractionDigits: 2 }));
        });
    } else {
        addCartEmptyRow();
    }
    sum = sum - totalDiscount + totalShippingCost;
    jQuery('#cart-summary tfoot .row-shipping-cost').data('total-discount', totalShippingCost).find('.price-number').text('+ ' + totalShippingCost.toLocaleString('fr', { minimumFractionDigits: 2 }));
    jQuery('#cart-summary tfoot .row-promotion-code').data('total-discount', totalDiscount).find('.price-number').text('- ' + totalDiscount.toLocaleString('fr', { minimumFractionDigits: 2 }));
    if (totalDiscount > 0) {
        jQuery('#cart-summary tfoot .row-promotion-code').show();
    } else {
        jQuery('#cart-summary tfoot .row-promotion-code').hide();
    }
    jQuery('#cart-summary tfoot .row-total-price').data('total-price', sum.toFixed(2)).find('.price-number').text(sum.toLocaleString('fr', { minimumFractionDigits: 2 }));
    jQuery('#panier').dlgCartPopup('loadCartItems');
}
function setItemQuantity(itemId, quantity, callback) {
    jQuery.ajax({
        url: getUrl('Cart/SetItemQuantity'),
        data: { itemId: itemId, quantity: quantity },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            if (result.isSuccess) {
                if (callback && typeof (callback) == "function") {
                    callback(result);
                }
            }
        }
    });
}
function onCartItemDeleteButtonIsClicked() {
    //if (confirm(window.msg.confirmDeleteMsg) == false)
    //    return;

    var $tr = jQuery(this).parents('tr');
    var id = $tr.data('id');

    jQuery.ajax({
        url: getUrl('Cart/RemoveItem'),
        data: { itemId: id },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            if (result.isSuccess) {
                window.location.reload();
            }
        }
    });
}

function onApplyCouponButtonIsClicked(e) {
    var $couponCode = jQuery('#coupon-code');
    if ($couponCode.val().trim() == "")
        return;

    jQuery.ajax({
        url: getUrl('Cart/ApplyCouponCode'),
        data: { code: $couponCode.val() },
        dataType: 'json',
        type: 'post',
        beforeSend: function () {
            showLoader(true, '#cart-summary');
        },
        success: function (result) {
            showLoader(false, '#cart-summary');
            if (result.isSuccess) {
                window.location.reload();
            } else {
                jQuery('#error-popup .popup-content').html(result.message);
                jQuery('#error-popup-link').trigger('click');
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#cart-summary');
        }
    });
}

function addCartEmptyRow() {
    jQuery('#cart-summary tbody').html('<tr><td colspan="6" class="text-center">' + window.msg.cartEmptyText + '</td></tr>');
}