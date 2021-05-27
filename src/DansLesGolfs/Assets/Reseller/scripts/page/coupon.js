jQuery(document).ready(function ($) {
    $('#CouponCode').change(function () {
        this.value = this.value.toUpperCase();
    });
});