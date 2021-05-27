window.locale = 'fr';

/**
* Define fbAsyncInit event.
* To request user permission.
*/
window.fbAsyncInit = function () {
    FB.init({ appId: window.FBAppId, status: true, cookie: true, xfbml: true, oauth: true, channelUrl: window.FBChannelUrl });
    jQuery('.fblogin-button').each(function (i) {
        this.onclick = function () {
            FB.login(function (response) {
                if (response.authResponse) {
                    FB.api('/me', function (info) {
                        fbLogin(response, info);
                    });
                }
            }, { scope: 'email,user_birthday,user_about_me' });
        };
    });

    /**
    * Login with facebook data through AJAX.
    */
    function fbLogin(response, info) {
        if (response.authResponse) {
            info.action = "fbs-signin";
            jQuery.post(getUrl('Logon/LoginViaFacebook'), info, function (result) {
                if (result.isSuccess) {
                    window.location.reload();
                } else {

                }
            }, 'json');
        }
    }

    function fbLoginConect(response, info) {
        if (response.authResponse) {
            info.action = "fbs-signin";
            jQuery.post(getUrl('Logon/LoginViaFacebook'), info, function (result) {
                if (result.isSuccess) {
                    if (result.isRequireUsername) {
                        jQuery('#fblogin-popup').data(info);
                        jQuery('#fblogin-popup input[name=email]').val(info.email);
                        jQuery('#fblogin-popup input[name=password]').val("");
                        jQuery('#fblogin-popup input[name=id]').val(info.id);
                        jQuery('#fblogin-popup input[name=username]').val(info.username);
                        jQuery('#fblogin-popup input[name=first_name]').val(info.first_name);
                        jQuery('#fblogin-popup input[name=last_name]').val(info.last_name);
                        jQuery('#fblogin-popup input[name=birthday]').val(info.birthday);
                        jQuery('#fblogin-popup input[name=gender]').val(info.gender);
                        onFBRegisterButtonIsClicked();
                    } else {
                        window.location.reload();
                    }
                } else {

                }
            }, 'json');
        }
    }
};

jQuery(document).ready(function ($) {
    // Enable Lazy Loading.
    $("img").unveil();

    $('#fblogin-popup-link').fancybox();
    $('#user-logon .forgot-password-link').fancybox({
        width: 600,
        afterLoad: function () {
            $(".fancybox-wrap").addClass("forgot-password-popup");
        }
    });
    $('#fblogin-popup .close').click(function () {
        $.fancybox.close();
    });

    $('#fblogin-popup .register-button').click(function () { $('.fbregister-form').trigger('submit'); });
    $('.fbregister-form').on('submit', function (e) {
        e.preventDefault();
        onFBRegisterButtonIsClicked();
    });

    LoadFooter();
    LoadFooterTabInfomations();
    LoadFooterTabCategory();
    LoadFooterTabImprint();
    LoadFooterTabContract();
    // Footer tabs
    //$('#footerTabContent a:last').tab('show'); // Temporary Disabled, waiting for investigate bug.

    $("#product-filter").tabs().addClass("ui-tabs-vertical ui-helper-clearfix");
    $("#product-filter li").removeClass("ui-corner-top").addClass("ui-corner-left");
    openProductSearchTab();

    // Init jQuery Uniform.
    $('select:not(.normal), input[type=checkbox], input[type=radio]').uniform({
        selectAutoWidth: false
    });

    // Init DLG Cart Popup.
    if ($.fn.dlgCartPopup) {
        $('#topnav #panier').dlgCartPopup({
            cartItemsUrl: getUrl('Common/AjaxGetCartItems'),
            updateItemUrl: getUrl('Cart/SetItemQuantity'),
            customY: true,
            speed: 'fast',
            align: 'right',
            marginRight: 20,
            checkOutUrl: getUrl('cart'),
            shoppingUrl: getUrl('')
        });
    }

    $('#scroll-up-button').click(function () {
        $('html, body').animate({ scrollTop: '0px' });
    });

    // Header User Logon Widget.
    $('.formholder #us').keypress(function (e) {
        if (e.which == 13) {
            $('.formholder #pass').focus();
        }
    });
    $('.formholder #pass').keypress(function (e) {
        if (e.which == 13) {
            $('.formholder .login-ok').click();
        }
    });
    $('#topnav .formholder form').submit(onUserLogonWidgetLogin);

    $('.datepicker').datepicker({
        dateFormat: 'dd/mm/yy',
        autoSize: true,
        showOtherMonths: true,
        selectOtherMonths: true,
        showOn: 'both',
        buttonImageOnly: true,
        buttonImage: getUrl('Assets/Front/img/icon-datepicker-calendar-pink.png')
    });

    // Adjust Scroll To Top Button
    adjustScrollToTopButton();
    $(window).scroll(onWindowScrolled);

    // Scrolling Content.

    $('#footerTabContent #mention #text-body-mention').mCustomScrollbar({
        mouseWheelPixels: 1000
    });
});

$('#loginform').click(function () {
    $('.login').fadeToggle('slow');
    $('#content').css("opacity", "0.3")
    $('#header').css("opacity", "0.3")
    $('#footer1').css("opacity", "0.3")
    $('#footer').css("opacity", "0.3")
    $('#btn-ft').css("opacity", "0.3")
    $('#footer-link').css("opacity", "0.3")
    $('.formholder #us').focus();
});

$('#use_menu_form').click(function () {
    $('.user-menu').fadeToggle('slow');
});

$('#user-logon .forgot-password-link').click(function (e) {
    e.preventDefault();
});

/*
$(".user-menu ul li").mouseover(function () {
    $(this).addClass("focus");
});
*/

$(document).mouseup(function (e) {
    var container = $(".login");

    if (!container.is(e.target) // if the target of the click isn't the container...
        && container.has(e.target).length === 0) // ... nor a descendant of the container
    {
        container.hide();
        $('#content').css("opacity", "1")
        $('#header').css("opacity", "1")
        $('#footer1').css("opacity", "1")
        $('#footer').css("opacity", "1")
        $('#btn-ft').css("opacity", "1")
        $('#footer-link').css("opacity", "1")
    }

    var container_us = $(".user-menu");

    if (!container_us.is(e.target) // if the target of the click isn't the container...
        && container_us.has(e.target).length === 0) // ... nor a descendant of the container
    {
        container_us.hide();
    }




    $(".user-menu ul li").hover(function () {
        var get_id = $(this).attr("id");

        if (get_id == "first") {
            $(".arrow-up-2-user").css({
                'width': '0',
                'height': '0',
                'border-left': '12px solid transparent',
                'border-right': '12px solid transparent',
                'border-bottom': '12px solid #5d5d5d',
                'left': '52%',
                'position': 'absolute',
                'top': '-9px'
            });
        }
        else {
            $(".arrow-up-2-user").css({
                'width': '0',
                'height': '0',
                'border-left': '12px solid transparent',
                'border-right': '12px solid transparent',
                'border-bottom': '12px solid #f5f5f5',
                'left': '52%',
                'position': 'absolute',
                'top': '-9px'
            });
        }

        $(this).css("background", "#5d5d5d");

    },
   function () {
       $(this).css("background", "#f5f5f5");
       $(".arrow-up-2-user").css({
           'width': '0',
           'height': '0',
           'border-left': '12px solid transparent',
           'border-right': '12px solid transparent',
           'border-bottom': '12px solid #f5f5f5',
           'left': '52%',
           'position': 'absolute',
           'top': '-9px'
       });
   });



});

function openProductSearchTab() {
    if (!window.itemType)
        return;

    switch (window.itemType) {
        case "product green-fee":
            $("#product-filter").tabs("option", "active", 0);
            break;
        case "product stay-package":
            $("#product-filter").tabs("option", "active", 1);
            break;
        case "product golf-lesson":
            $("#product-filter").tabs("option", "active", 2);
            break;
        case "product driving-range":
            $("#product-filter").tabs("option", "active", 3);
            break;
        case "product reseller-product":
            $("#product-filter").tabs("option", "active", 4);
            break;
        case "where-to-play":
            $("#product-filter").tabs("option", "active", 5);
            break;
    }
}



/********************** Functions ***********************/
function getUrl(url) {
    var full_url = window.location.protocol + "//" + window.location.host + "/";
    return full_url + url;
}
function weekStart(Month, Year, StartOfWeekDay) {
    _Date = new Date(Year, Month, 1)
    _LastDate = new Date()
    _LastDate = new Date(_Date)
    _LastDate.setMonth(_Date.getMonth() + 1)
    _LastDate.setHours(_LastDate.getHours() - 24)
    _Returns = new Array()
    if (StartOfWeekDay == null) {
        StartOfWeekDay = 0
    }
    var I = 1
    while (I <= _LastDate.getDate()) {
        _Date.setDate(I)
        if (_Date.getDay() == StartOfWeekDay) {
            _Returns[_Returns.length] = new Date(_Date)
            I += 6
        }
        I++
    }
    return (_Returns)
}
function getSunday(d) {
    d = new Date(d);
    var day = d.getDay(),
        diff = d.getDate() - day; // adjust when day is sunday
    return new Date(d.setDate(diff));
}
function showLoader(isShow, selector, customCss) {
    if (selector === undefined || selector === null) {
        selector = 'body';
    }

    var $selector = jQuery(selector);
    $selector.each(function () {
        var $this = jQuery(this);
        if (isShow) {
            var $overlay = $this.data('loaderOverlay');
            if (!$this.data('loaderOverlay')) {
                $overlay = jQuery('<div class="loaderOverlay"><div class="loaderWrapper"><div class="loaderAnimation"></div></div></div>').appendTo('body');
                $this.data('loaderOverlay', $overlay);
            }
            var $animation = $overlay.find('.loaderAnimation');

            // Width & Height
            if (selector == 'body') {
                var width = $(window).outerWidth();
                var height = $(window).outerHeight();
                var offset = $this.offset();
                $overlay.css({
                    'width': width + 'px',
                    'height': height + 'px',
                    'top': offset.top + 'px',
                    'left': offset.left + 'px',
                    'position': 'fixed'
                });

                var length = width < height ? width : height;
                var animateLength = length * 0.2;

                $animation.css({
                    'width': animateLength + 'px',
                    'height': animateLength + 'px',
                    'top': ((height - animateLength) / 2) + 'px',
                    'left': ((width - animateLength) / 2) + 'px'
                });
                if (customCss) {
                    $animation.css(customCss);
                }

                $overlay.fadeIn();
            } else {
                var width = $this.outerWidth();
                var height = $this.outerHeight();
                var offset = $this.offset();
                $overlay.css({
                    'width': width + 'px',
                    'height': height + 'px',
                    'top': offset.top + 'px',
                    'left': offset.left + 'px'
                });

                var length = width < height ? width : height;
                var animateLength = length * 0.3;

                $animation.css({
                    'width': animateLength + 'px',
                    'height': animateLength + 'px',
                    'top': ((height - animateLength) / 2) + 'px',
                    'left': ((width - animateLength) / 2) + 'px'
                });
                if (customCss) {
                    $animation.css(customCss);
                }

                $overlay.fadeIn();
            }
        } else {
            var $overlay = $this.data('loaderOverlay');
            if ($overlay) {
                $overlay.fadeOut().animate({ 'dummy': '1' }, 100).remove();
                $this.removeData('loaderOverlay');
            }
        }
    });
}

// Read a page's GET URL variables and return them as an associative array.
function getUrlVars() {
    var vars = [], hash;
    var q = document.URL.split('?')[1];
    if (q != undefined) {
        q = q.split('&');
        for (var i = 0; i < q.length; i++) {
            hash = q[i].split('=');
            vars.push(hash[1]);
            vars[hash[0]] = hash[1];
        }
    }
    return vars;
}


/********************** Functions Loads Footer ***********************/
function LoadFooter() {

    var $this = jQuery(this);
    var $footerwrap = $("#footer-body");
    jQuery.ajax({
        url: getUrl('Common/AjaxGetSiteFooter'),
        type: 'get',
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                var nCount = 0;
                var content = "<table id='footer-site-slug'>"
                for (var i in result.list) {
                    if (nCount == 0)
                        content += '<tr>';
                    content += '<td><a class="last-ft" href="' + getUrl('Site/' + result.list[i].SiteSlug) + '">' + result.list[i].SiteName + '</a></td>';
                    nCount = nCount + 1;
                    if (nCount == 3) {
                        content += '</tr>';
                        nCount = 0;
                    }
                }
                content += "</table>"
                $footerwrap.append(content);
            }
        }
    });
};

/********************** Functions Loads Footer Webcontents ***********************/
function LoadFooterTabInfomations() {

    var $this = jQuery(this);
    var $footerwrap = $("#ft_info_tab");
    jQuery.ajax({
        url: getUrl('Common/AjaxGetFooterTabInfomations'),
        type: 'get',
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                var nCount = 0;
                var content = "<table id='info_tab'>";
                for (var i in result.list) {
                    if (result.list[i].ContentCategory == "FOOTER") {
                        if (nCount == 0)
                            content += '<tr>';

                        content += '<td><a class="last-ft" href="' + getUrl('page/' + result.list[i].ContentKey) + '">' + result.list[i].TopicName + '</a></td>';
                        nCount = nCount + 1;
                        if (nCount == 3) {
                            content += '</tr>';
                            nCount = 0;
                        }
                    }
                }
                content += "</table>";
                $footerwrap.append(content);
            }
        }
    });
};

//function LoadFooterTabCategory() {

//    var $this = jQuery(this);
//    var $footerwrap = $("#ft_category_tab div#cat_right");
//    jQuery.ajax({
//        url: getUrl('Common/AjaxGetFooterTabCategory'),
//        type: 'post',
//        dataType: 'json',
//        success: function (result) {
//            if (result.isSuccess) {

//                var nCount = 0;
//                var content = "<table id='category_tab'>"


//                var countviewmore = 0;

//                for (var i in result.list) {

//                    if (nCount == 0)
//                        content += '<tr>';

//                    //content += '<td><a class="last-ft" href="' + getUrl('Site/' + result.list[i].ContentKey) + '">' + result.list[i].ContentKey + '</a></td>';
//                    content += '<td><h4>' + i + '</h4>';
//                    content += '<ul>';
//                    for (var x in result.list[i]) {

//                        content += '<li><a href="' + getUrl('Item/' + result.list[i][x].ItemSlug) + '" >' + result.list[i][x].ItemName + '</a></li>';
//                    }
//                    content += '</ul>';

//                    if (countviewmore == 0) {
//                        content += '<a href="' + getUrl('GreenFees') + '" class="read-more">'+'@Resources.Seemore_greenfees_footer'+'</a></td>';
//                    } else if (countviewmore == 1) {
//                        content += '<a href="' +  getUrl('StayPackages') + '" class="read-more">voir tous stages de golf</a></td>';
//                    } else if (countviewmore == 2) {
//                        content += '<a href="' + getUrl('GolfLessons') + '" class="read-more">voir tous nos séjours de gol</a></td>';
//                    } else if (countviewmore == 3) {
//                        content += '<a href="' + getUrl('DrivingRanges') + '" class="read-more">voir tous nos practices</a></td>';
//                    }


//                    countviewmore++;
//                    nCount = nCount + 1;

//                    if (nCount == 2) {
//                        content += '</tr>';
//                        nCount = 0;
//                    }
//                }

//                content += "</table>";

//                $footerwrap.append(content);
//            }
//        }
//    });
//};

/********************** Newsletter Subscribe ***********************/
$('#ft-subscribe').click(function () {

    var emailTo = $('#email-subscribe').val();
    if (email == '' || emailTo.length <= 0) {
    }
    else {
        jQuery.ajax({
            url: getUrl('Common/AjaxNewsletterSubscribe'),
            data: { email: emailTo },
            type: 'post',
            dataType: 'json',
            success: function (result) {

                if (result.isSuccess && result.data != -2) {
                    $("#validate-newletter").html("<div style='color:green'>" + result.message + "</div>");
                    $("#email-subscribe").val("@");
                    $.post(getUrl('Common/AjaxNewsletterSubscribePostProcess'), { email: emailTo });
                } else if (result.data == -2) {
                    $("#validate-newletter").html("<div style='color:red'>" + result.message + "</div>");
                } else {
                    $("#validate-newletter").html("<div style='color:red'>" + result.message + "</div>");

                }
            }
        });
    }

});

/********************** Footer To Sponsor ***********************/
$('#btn-ft-sponsor').click(function () {
    var email = $('#email-ft-sponsor').val();
    window.location = getUrl("Account/Sponsorship?Email=" + email);
});

/*****************mention******************************/
function LoadFooterTabImprint() {
    jQuery.ajax({
        url: getUrl('Common/AjaxLoadFooterTabImprint'),
        type: 'get',
        dataType: 'json',
        success: function (result) {
            jQuery('#footerTabContent #mention #text-body-mention .mCSB_container').html(result.list);
            jQuery('#footerTabContent #mention #text-body-mention').mCustomScrollbar('update');
            jQuery("#footerTabContent #mention #text-body-mention").mCustomScrollbar("scrollTo", "top", { scrollInertia: 200 });
        }
    });

}

/*****************end mention******************************/

/*****************Contract******************************/
function LoadFooterTabContract() {
    jQuery.ajax({
        url: getUrl('Common/AjaxLoadFooterTabContract'),
        type: 'get',
        dataType: 'json',
        success: function (result) {
            $("#ct-left").html(result.list);
        }
    });
}

$('#contact-form-footer').submit(function (e) {
    e.preventDefault();

    $("#contract-validate").html("");
    var name = $("#contract-name").val();
    var email = $("#contract-email").val();
    var subject = $("#contract-subject").val();
    var message = $("#contract-message").val();

    if (name.length <= 0) {
        $("#contract-name").addClass("input-validation-error");
    }
    else if (name.length > 0) {
        $("#contract-name").removeClass("input-validation-error");
    }

    if (email.length <= 0) {
        $("#contract-email").addClass("input-validation-error");
    }
    else if (email.length > 0) {
        $("#contract-email").removeClass("input-validation-error");
    }

    if (subject.length <= 0) {
        $("#contract-subject").addClass("input-validation-error");
    }
    else if (subject.length > 0) {
        $("#contract-subject").removeClass("input-validation-error");
    }

    if (message.length <= 0) {
        $("#contract-message").addClass("input-validation-error");
    }
    else if (message.length > 0) {
        $("#contract-message").removeClass("input-validation-error");
    }


    if (name.length > 0 && email.length > 0 && subject.length > 0 && message.length > 0) {
        $("#loading-contract").show();
        $("#ct-right-content").hide();
        var formData = jQuery('#contact-form-footer').serialize();
        jQuery.ajax({
            url: getUrl('Common/SentEmailContract'),
            data: formData,
            type: 'post',
            dataType: 'json',
            success: function (result) {

                if (result.isSuccess) {
                    $("#contract-name").val("");
                    $("#contract-email").val("");
                    $("#contract-subject").val("");
                    $("#contract-message").val("");
                    $("#contract-validate").html("<div style='color:green'>" + result.message + "</div>");
                    $("#loading-contract").hide();
                    $("#ct-right-content").show();
                } else {
                    $("#loading-contract").hide();
                    $("#ct-right-content").show();
                    $("#contract-validate").html("<div style='color:red'>" + result.message + "</div>");
                }
            }
        });

    }
});


/*****************end Contract******************************/



/********************** User Logon Widget ***********************/
function onUserLogonWidgetLogin(e) {
    e.preventDefault();
    e.stopPropagation();
    var $form = jQuery(this);
    var $errorMessage = $form.find('.error-message');
    $errorMessage.text('').hide();
    jQuery.post(getUrl('Common/AjaxLogin'), $form.serialize(), function (result) {
        if (result.isSuccess) {
            window.location = getUrl('');
        } else {
            $errorMessage.text(result.message).show();
        }
    }, 'json');
}

function onFBRegisterButtonIsClicked() {
    var $form = jQuery('.fbregister-form');
    jQuery.post(getUrl('Common/AjaxRegisterFacebookUser'), $form.serialize(), function (result) {
        if (result.isSuccess) {
            window.location.reload();
        } else {
            showPopupMessage(result.message);
        }
    }, 'json');
}

function showPopupMessage(message) {
    alert(message);
}


/************************ Adjust Scroll To Top Button ********************************/
function adjustScrollToTopButton() {
    var $content = jQuery('#content');
    var left = $content.position().left + $content.outerWidth() + 30;
    var top = ($(window).height() - jQuery('#scroll-up-button').height()) / 2;
    jQuery('#scroll-up-button').css({
        left: left + 'px',
        top: top + 'px',
        right: 'auto'
    });
    onWindowScrolled();
}

function onWindowScrolled(e) {
    // Show scroll button after 55% of window height.
    if (jQuery(window).scrollTop() > (jQuery(window).height() * 0.55)) {
        jQuery('#scroll-up-button').fadeIn();
    } else {
        jQuery('#scroll-up-button').fadeOut();
    }
}

function isValidEmailAddressSponsor(emailAddress) {
    var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
    var arr = emailAddress.split(",");
    var IsFalse = false;
    var tempEmail = '';
    for (var i = 0; i < arr.length; i++) {
        tempEmail = arr[i].trim();
        if (!pattern.test(tempEmail)) {
            IsFalse = true;
        } else {
            if (tempEmail.length > 100) {
                IsFalse = true;
            }
        }
    }
    if (IsFalse) {
        return false;
    } else {
        return true;
    }
};