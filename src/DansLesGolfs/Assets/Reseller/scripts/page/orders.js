jQuery(function ($) {
    $('#order-items-table .column-item-name .item-name').keyup(function (e) {
        var $this = $(this);
        $this.parents('td').find('input[name=ItemName]').val($this.text());
    });
    $('#order-items-table .column-unit-price input').keyup(function (e) {
        var $this = $(this);
        var value = $this.val();
        if (value.indexOf(',') > 0)
        {
            value = eval(value.replace(',', '.').replace(' ', ''));
        }
        var $row = $this.parents('tr');
        $row.data('unit-price', value);
        calculatePrice();
    });
    $('#order-items-table .column-quantity input').change(function (e) {
        var $this = $(this);
        calculatePrice();
    });
});

function calculatePrice() {
    var unitPrice = 0;
    var qty = 0;
    var reduction = 0;
    var reductionType = 0;
    var totalReduction = 0;
    var shippingCost = 0;
    var totalShippingCost = 0;
    var totalBasePrice = 0;
    var rowTotalPrice = 0;
    var baseTotal = 0;
    var totalPrice = 0;
    var $this = null;
    var $row = null;
    jQuery('#order-items-table tbody tr').each(function () {
        $this = jQuery(this);
        unitPrice = eval($this.data('unit-price'));
        qty = $this.find('input[name=Qty]').val();
        shippingCost = eval($this.data('shipping-cost')) * qty;
        totalShippingCost += shippingCost;
        rowTotalPrice = unitPrice * qty;
        reduction = getReduction(unitPrice, qty, $this.data('reduction'), $this.data('reduction-type'));
        totalReduction += reduction;
        baseTotal += rowTotalPrice;
        $this.find('.column-total-price .pricing-number').text(rowTotalPrice.toLocaleString(window.locale, { minimumFractionDigits: 2 }));
    });
    totalPrice = baseTotal - totalReduction + totalShippingCost;
    jQuery('#order-items-table tfoot tr .column-base-total .pricing-number').text(baseTotal.toLocaleString(window.locale, { minimumFractionDigits: 2 }));
    jQuery('#order-items-table tfoot tr .column-discount .pricing-number').text(totalReduction.toLocaleString(window.locale, { minimumFractionDigits: 2 }));
    jQuery('#order-items-table tfoot tr .column-shipping-cost .pricing-number').text(totalShippingCost.toLocaleString(window.locale, { minimumFractionDigits: 2 }));
    jQuery('#order-items-table tfoot tr .column-total-price .pricing-number').text(totalPrice.toLocaleString(window.locale, { minimumFractionDigits: 2 }));
}

function getReduction(price, qty, reduction, type) {
    if (type == 0) {
        if (reduction >= price)
            return price * qty;
        else
            return (price - reduction) * qty;
    } else {
        if (reduction >= 100)
            return price * qty;
        else
            return (price * reduction / 100) * qty;
    }
}