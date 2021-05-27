jQuery(function ($) {
    $('#cart-summary .column-quantity input').change(onInputQuantityChanged).keyup(onInputQuantityChanged);
    $('#cart-summary tbody .column-delete .delete-button').click(onCartItemDeleteButtonIsClicked);
});

function onInputQuantityChanged(e) {
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
    var quantity = 0;
    var totalPrice = 0;
    var $items = jQuery('#cart-summary .column-quantity input');
    if ($items.length > 0) {
        $items.each(function (i) {
            $row = jQuery(this).parents('tr');
            quantity = eval(jQuery(this).val());
            unitPrice = $row.data('unit-price');
            totalPrice = unitPrice * quantity;
            sum += totalPrice;
            $row.find('.column-total-price .price-number').text(totalPrice.toFixed(2));
        });
    } else {
        jQuery('#cart-summary tbody').html('<tr><td colspan="5" class="text-center">The basket is empty.</td></tr>');
    }
    jQuery('#cart-summary .column-sub-total').data('subtotal', sum.toFixed(2)).find('.price-number').text(sum.toFixed(2));
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
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
        }
    });
}
function onCartItemDeleteButtonIsClicked() {
    //if (confirm(window.msg.confirmDeleteMsg) == false)
    //    return;

    var $tr = jQuery(this).parents('tr');
    var id = $tr.data('id');
    setItemQuantity(id, 0, function () {
        $tr.remove();
    });
    jQuery.ajax({
        url: getUrl('Cart/RemoveItem'),
        data: { itemId: id },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            if (result.isSuccess) {
                $tr.remove();
                updateCartPrices();
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
        }
    });
}