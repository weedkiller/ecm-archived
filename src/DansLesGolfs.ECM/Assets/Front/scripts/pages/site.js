jQuery(document).ready(function ($) {

    // Rating Callback.
    $('.starrr').on('starrr:change', onUserGiveRating);

    $('#open-rating-popup-button').fancybox({
        modal: true
    });
    $('#close-rating-popup-button').click(function () {
        $.fancybox.close();
    });
    jQuery('#submit-review-button').click(onSubmitRating);
});

function onUserGiveRating(e, value) {
    window.rating = value;
    jQuery('#open-rating-popup-button').click();
    jQuery('#review-message-input').select();
}

function onSubmitRating() {
    var message = jQuery('#review-message-input').val();
    jQuery.ajax({
        url: getUrl('SiteAjax/UserGiveRating'),
        data: { siteId: window.SiteId, rating: window.rating, message: message },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            $.fancybox.close();
            if (result.isSuccess) {
                jQuery('#item-reviews #reviews-list .mCSB_container').html(result.html);
                jQuery('#item-reviews #reviews-list').mCustomScrollbar('update');
                jQuery("#item-reviews #reviews-list").mCustomScrollbar("scrollTo", "top", { scrollInertia: 200 });
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
        }
    });
}