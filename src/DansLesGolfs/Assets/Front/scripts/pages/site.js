/**
* Google Map
*/
function mapInitialize() {
    var mapOptions = {
        zoom: 8,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        center: new google.maps.LatLng(window.latitude, window.longitude)
    };

    window.map = new google.maps.Map(document.getElementById('site-map-canvas'),
        mapOptions);

    window.marker = new google.maps.Marker({
        map: window.map,
        position: window.map.getCenter()
    });
}

/**
* On Document Ready
*/
jQuery(document).ready(function ($) {
    mapInitialize();

    $(".royalSlider").css('display', 'block').royalSlider({
        keyboardNavEnabled: true,
        imageScaleMode: 'fill',
        imageScalePadding: 0,
        controlNavigation: 'thumbnails',
        autoScaleSlider: false,
        autoHeight: false,
        loop: true,
        autoPlay: {
            enabled: true,
            pauseOnHover: true
        },
        thumbs: {
            arrows: true
        }
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        mapInitialize();
    });

    $('#reviews-list').mCustomScrollbar({
        mouseWheelPixels: 1000
    });

    // Rating Callback.
    $('.starrr').off('starrr:change').on('starrr:change', onUserGiveRating);

    $('#open-rating-popup-button, #open-rating-finish-button').fancybox({
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        }
    });
    $('#close-rating-popup-button').unbind('click').click(function () {
        $.fancybox.close();
    });
    jQuery('#submit-review-button').unbind('click').click(onSubmitRating);
});


/***************************** Site Review ******************************/
function onUserGiveRating(e, value) {
    e.stopPropagation();
    window.rating = value;
    jQuery('#rating-popup .review-stars').data('rating', value);
    jQuery('#rating-popup .review-stars .star').removeClass('active');
    for (var i = 0; i < value; i++) {
        jQuery('#rating-popup .review-stars .star').eq(i).addClass('active');
    }
    jQuery.ajax({
        url: getUrl('Common/CheckLogin'),
        type: 'post',
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess && result.isLoggedIn) {
                jQuery('#open-rating-popup-button').click();
                jQuery('#review-message-input').select();
            } else {
                window.location = getUrl('Login?returnUrl=' + encodeURI(window.location.href));
            }
        }
    });
}

function onSubmitRating(e) {
    e.stopPropagation();

    var subject = jQuery('#review-subject-input').val();
    var message = jQuery('#review-message-input').val();

    jQuery.ajax({
        url: getUrl('SiteAjax/UserGiveRating'),
        data: { siteId: window.siteId, rating: jQuery('#rating-popup .review-stars').data('rating'), subject: subject, message: message },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            $.fancybox.close();
            if (result.isSuccess) {
                jQuery('#item-reviews #reviews-list .mCSB_container').html(result.html);
                jQuery('#item-reviews #reviews-list').mCustomScrollbar('update');
                jQuery("#item-reviews #reviews-list").mCustomScrollbar("scrollTo", "top", { scrollInertia: 200 });
                jQuery('.review-panel .starrr').data('rating', result.averageRating);
                jQuery('.review-panel .starrr .star').removeClass('active');
                jQuery('.review-panel .starrr').each(function () {
                    for (var i = 0; i < result.averageRating; i++) {
                        jQuery(this).find('.star').eq(i).addClass('active');
                    }
                });
                jQuery('.review-number').text(result.reviewNumber);
                $('#review-message-input').val('');
                $("#reviews-list .starrr").starrr();

                $('#open-rating-finish-button').trigger('click');
            }
        }
    });
}