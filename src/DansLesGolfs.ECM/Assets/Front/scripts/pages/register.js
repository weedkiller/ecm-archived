/**
* Define fbAsyncInit event.
* To request user permission.
*/
//window.fbAsyncInit = function () {
//    FB.init({ appId: window.FBAppId, status: true, cookie: true, xfbml: true, oauth: true });
//    var button = document.getElementById('btn-facebook-login');
//    button.onclick = function () {
//        FB.login(function (response) {
//            if (response.authResponse) {
//                FB.api('/me', function (info) {
//                    login(response, info);
//                });
//            }
//        }, { scope: 'email,user_birthday,status_update,publish_stream,user_about_me' });
//    };

//    /**
//    * Login with facebook data through AJAX.
//    */
//    function login(response, info) {
//        if (response.authResponse) {
//            info.action = "fbs-signin";
//            window.fbInfo = info;
//            jQuery.post('Logon/LoginViaFacebook', info, function (result) {
//                if (result.isSuccess) {
//                    if (result.isRequirePassword) {
//                        jQuery('#facebook-login-popup-link').click();
//                    } else {
//                        window.location = getUrl("~/");
//                    }
//                } else {
//                    console.error(result.message);
//                }
//            }, 'json');
//        }
//    }
//};

jQuery(document).ready(function ($) {
    //$('#facebook-login-popup .login-button').click(function () {
    //    window.fbInfo.password = $('#facebook-login-popup .input-password').val();
    //    jQuery.post('Logon/RegisterFacebookUser', window.fbInfo.password, function (result) {
    //        if (result.isSuccess) {
    //            window.location = getUrl("~/");
    //        } else {
    //            console.error(result.message);
    //        }
    //    }, 'json');
    //});
});