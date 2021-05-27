/**
* Define fbAsyncInit event.
* To request user permission.
*/
window.fbAsyncInit = function () {
    FB.init({ appId: window.FBAppId, status: true, cookie: true, xfbml: true, oauth: true });
    var button = document.getElementById('btn-facebook-login');
    button.onclick = function () {
        FB.login(function (response) {
            if (response.authResponse) {
                FB.api('/me', function (info) {
                    fbLogin(response, info);
                });
            }
        }, { scope: 'email,user_birthday,status_update,publish_stream,user_about_me' });
    };

    /**
    * Login with facebook data through AJAX.
    */
    function fbLogin(response, info) {
        if (response.authResponse) {
            info.action = "fbs-signin";
            jQuery.post('Logon/LoginViaFacebook', info, function (result) {
                if (result.isSuccess) {
                    if (result.isRequireUsername) {
                        jQuery('#facebook-login-modal').modal('show');
                    } else {
                        window.location = getUrl("");
                    }
                } else {

                }
            }, 'json');
        }
    }
};