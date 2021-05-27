jQuery(document).ready(function ($) {
    $('#register-form, #login-form, #subscription-form').each(function (i) {
        this.onsubmit = function (e) {
            return false;
        };
    });

    // Login Form
    $('#login-form input[type=submit]').click(function (e) {
        $.post(getUrl('Order/LoginForm'), $(this).closest('form').serialize(), function (result) {
            if (result.isSuccess) {
                $('#login-form .error-messages').hide();
                window.location = getUrl('Order/Shipping');
            } else {
                $('#login-form .error-messages').html(result.message).show();
                console.error(result.message);
            }
        }, 'json');
    });

    // Register Form
    $('#register-form input[type=submit]').click(function (e) {
        $.post(getUrl('Order/registerForm'), $(this).closest('form').serialize(), function (result) {
            if (result.isSuccess) {
                $('#register-form .error-messages').hide();
                window.location = getUrl('Order/Shipping');
            } else {
                $('#register-form .error-messages').html(result.message).show();
                console.error(result.message);
            }
        }, 'json');
    });

    // Subscription Form
    $('#subscription-form input[type=submit]').click(function (e) {
        $.post(getUrl('Order/subscriptionForm'), $(this).closest('form').serialize(), function (result) {
            if (result.isSuccess) {
                $('#subscription-form .error-messages').hide();
                window.location = getUrl('Order/Shipping');
            } else {
                $('#subscription-form .error-messages').html(result.message).show();
                console.error(result.message);
            }
        }, 'json');
    });

    // Facebook Login
    $('#facebook-login-popup-link').fancybox();
    $('#facebook-login-popup .register-button').click(function () {
        window.fbInfo.password = $('#facebook-login-popup .input-password').val();
        jQuery.post('Logon/RegisterFacebookUser', window.fbInfo.password, function (result) {
            if (result.isSuccess) {
                window.location = getUrl("~/Order/Shipping");
            } else {
                $('#facebook-login-popup .error-message').html(result.message);
                console.error(result.message);
            }
        }, 'json');
    });
    $('#facebook-login-popup .cancel-button').click(function () {
        $.fancybox.close();
    });
});