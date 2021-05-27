jQuery(document).ready(function ($) {
    $('input[type=radio]').uniform();

    $('#agreement-confirm-link, #cart-status-link, #error-link').fancybox({
        autoScale: true,
        modal: true,
        minHeight: 20,
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        }
    });

    $('#agreement-confirm-popup .ok-button, #cart-status-popup .ok-button, #error-popup .ok-button').click(function () { $.fancybox.close(); });

    $('#IsAcceptTermsAndAgreements').change(function (e) {
        if (this.checked) {
            $('#checkout-button').removeClass('disabled-link').removeAttr('disabled');
        } else {
            $('#checkout-button').addClass('disabled-link').attr('disabled', 'disabled');
        }
    });

    $('#checkout-button').click(function () {
        showLoader(true, '#page-order-payment');

        if ($('#IsAcceptTermsAndAgreements:checked').length <= 0 || $(this).hasClass('disabled-link')) {
            $('#agreement-confirm-link').trigger('click');
            showLoader(false, '#page-order-payment');
            return;
        }

        var cartStatus = checkCartStatus();

        if (!cartStatus || !cartStatus.isPassed) {
            $('#cart-status-popup').find('.popup-content').html(cartStatus.message);
            $('#cart-status-link').trigger('click');
            showLoader(false, '#page-order-payment');
            return;
        }

        redirectPayment();

        /*jQuery.ajax({
            url: getUrl('Order/AjaxPaymentCheckCartStatus'),
            type: 'post',
            dataType: 'json',
            async: false,
            success: function (result) {
                if (result.isSuccess) {
                    redirectPayment();
                } else {
                    showLoader(false, '#page-order-payment');
                    jQuery('#error-popup .popup-content').html(result.message);
                }
            },
            error: function (xhr, msg) {
                showLoader(false, '#page-order-payment');
                jQuery('#error-popup .popup-content').html(msg);
            }
        });*/
    });
});

function checkCartStatus() {
    var cartStatus = undefined;
    jQuery.ajax({
        url: getUrl('Order/AjaxPaymentCheckCartStatus'),
        type: 'post',
        dataType: 'json',
        async: false,
        success: function (result) {
            if (result.isSuccess) {
                cartStatus = result.cartStatus;
            }
        },
        error: function (xhr, msg) {
            cartStatus = {
                isPassed: false,
                message: msg
            };
        }
    });
    return cartStatus;
}

function redirectPayment() {
    var paymentGateway = jQuery('#payment-gateways input[type=radio]:checked').val();
    switch (paymentGateway) {
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
        case 'lydia-card':
            window.location = getUrl('Order/PaymentLydia?type=card');
            break;
        case 'lydia-mobile':
            window.location = getUrl('Order/PaymentLydia?type=mobile');
            break;
        case 'check':
            window.location = getUrl('Order/PaymentCheck');
            break;
        case 'free':
            window.location = getUrl('Order/PaymentFree');
            break;
    }
}

function callConversionTracking(id) {
    jQuery.post(getUrl('ItemAjax/AjaxTriggerConversionTracking'), { id: id, type: 'checkout'});
}