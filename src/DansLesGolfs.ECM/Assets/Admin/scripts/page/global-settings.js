/**
* On document ready.
*/
jQuery(document).ready(function ($) {
    $('.info-form').validate();

    $('#SMTPUseVERP').change(function () {
        $('#BouncedReturnEmail').prop('disabled', this.checked);
    }).trigger('change');
});