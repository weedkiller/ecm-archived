jQuery(document).ready(function ($) {
    $('#checkout-button').click(function () {
        if ($('#IsAcceptTermsAndAgreements:checked').length <= 0 || $(this).hasClass('disabled-link')) {
            $('#alert-modal').modal('show', true);
            return;
        }

        var paymentGateway = jQuery('#payment-gateways input[type=radio]:checked').val();
        switch(paymentGateway)
        {
            case 'cb':
                window.location = getUrl('Order/PaymentCreditCard?card=cb');
                break;
            case 'mastercard':
                window.location = getUrl('Order/PaymentCreditCard?card=mastercard');
                break;
            case 'visa':
                window.location = getUrl('Order/PaymentCreditCard?card=visa');
                break;
            case 'paypal':
                window.location = getUrl('Order/PaymentPaypal');
                break;
            case 'check':
                window.location = getUrl('Order/PaymentCheck');
                break;
        }
    });

    $('#IsAcceptTermsAndAgreements').change(function (e) {
        if (this.checked) {
            $('#checkout-button').removeClass('disabled-link').removeAttr('disabled');
        } else {
            $('#checkout-button').addClass('disabled-link').attr('disabled', 'disabled');;
        }
    });
});