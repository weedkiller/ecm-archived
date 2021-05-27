jQuery(document).ready(function ($) {
    var queryString = getUrlVars();

    // Load cities drop down list.
    loadCitiesDropDownList();

    // Prepare validation.
    $('#register-form, #login-form, #subscription-form').validate();

    $('#notify-message-link, #error-link').fancybox({
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        },
        height: 50,
        width: 300,
        autoSize: false
    });

    $('#notify-message-popup button, #error-popup .ok-button').click(function () {
        $.fancybox.close();
    });

    $('input[name=PostalCode]').change(function () {
        loadCitiesDropDownList();
    });

    $('input[name=PhoneCountryCode], input[name=MobilePhoneCountryCode]').numeric();
    $('input[name=Phone], input[name=MobilePhone]').numeric();

    $('#register-form, #login-form, #subscription-form').each(function (i) {
        this.onsubmit = function (e) {
            return false;
        };
    });

    // Login Form
    $('#login-form').on('submit', function (e) {
        e.preventDefault();

        if (!$(this).valid())
            return false;

        $.post(getUrl('Order/LoginForm'), $(this).closest('form').serialize(), function (result) {
            if (result.isSuccess) {
                $('#login-form .error-messages').hide();
                if (queryString["returnUrl"]) {
                    window.location = decodeURIComponent(queryString["returnUrl"]);
                } else {
                    window.location = getUrl('Order/Shipping');
                }
            } else {
                $('#login-form .error-messages').html(result.message).show();
                $('#notify-message-popup .notify-content').html(result.message);
                $('#notify-message-link').click();
            }
        }, 'json');
    });

    // Register Form
    $('#register-form').on('submit', function (e) {
        e.preventDefault();

        if (!$(this).valid())
            return false;

        $.post(getUrl('Order/registerForm'), $(this).closest('form').serialize(), function (result) {
            if (result.isSuccess) {
                $('#register-form .error-messages').hide();
                if (queryString["returnUrl"]) {
                    window.location = decodeURIComponent(queryString["returnUrl"]);
                } else {
                    window.location = getUrl('Order/Shipping');
                }
            } else {
                $('#notify-message-popup .notify-content').html(result.message);
                $('#notify-message-link').click();
            }
        }, 'json');
    });

    // Subscription Form
    $('#subscription-form').on('submit', function (e) {
        e.preventDefault();

        if (!$(this).valid())
            return false;

        $.post(getUrl('Order/subscriptionForm'), $(this).closest('form').serialize(), function (result) {
            if (result.isSuccess) {
                $('#subscription-form .error-messages').hide();
                if (queryString["returnUrl"]) {
                    window.location = decodeURIComponent(queryString["returnUrl"]);
                } else {
                    window.location = getUrl('Order/Shipping');
                }
            } else {
                $('#subscription-form .error-messages').html(result.message).show();
                alert(result.message);
            }
        }, 'json');
    });

    // Facebook Login
    $('#facebook-login-popup-link').fancybox();
    $('#facebook-login-popup .register-button').click(function () {
        window.fbInfo.password = $('#facebook-login-popup .input-password').val();
        jQuery.post('Logon/RegisterFacebookUser', window.fbInfo.password, function (result) {
            if (result.isSuccess) {
                if (queryString["returnUrl"]) {
                    window.location = decodeURIComponent(queryString["returnUrl"]);
                } else {
                    window.location = getUrl('Order/Shipping');
                }
            } else {
                $('#facebook-login-popup .error-message').html(result.message);
                alert(result.message);
            }
        }, 'json');
    });
    $('#facebook-login-popup .cancel-button').click(function () {
        $.fancybox.close();
    });
});

/**
* Load cities drop down list by postal code.
*/
function loadCitiesDropDownList(callback) {
    jQuery.ajax({
        url: getUrl('Common/GetDataByPostalCode'),
        type: 'post',
        dataType: 'json',
        data: { postalCode: jQuery('input[name=PostalCode]').val() },
        beforeSend: function () {
            showLoader(true, '#register-form');
        },
        success: function (result) {
            showLoader(false, '#register-form');
            if (result.isSuccess) {
                jQuery('#register-form #CountryId').val(result.countryId).prop('value', result.countryId).data('country-id', result.countryId);
                html = '';
                for (var i in result.cities) {
                    html += '<option value="' + result.cities[i].CityId + '">' + result.cities[i].CityName + '</option>';
                }
                jQuery('#register-form #CityId').html(html);
                if (jQuery('#register-form #CityId').data('city-id') > 0) {
                    jQuery('#register-form #CityId').val(jQuery('#register-form #CityId').data('city-id')).data('city-id', 0);
                } else {
                    jQuery('#register-form #CityId').val(result.cityId).data('city-id', 0);
                }

                jQuery.uniform.update();
                if (callback && typeof (callback) == 'function') {
                    callback();
                }
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#register-form');
            alert(xhr);
        }
    });
}