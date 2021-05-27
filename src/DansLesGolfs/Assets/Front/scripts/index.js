$(document).ready(function () {
    // Init Homepage Slider.
    initHomepageSlider();
});

function initHomepageSlider() {
    $.ajax({
        url: getUrl('Common/AjaxGetHomepageSlider'),
        type: 'get',
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                $('#homepage-slider').html(result.html).royalSlider({
                    keyboardNavEnabled: true,
                    imageScaleMode: 'fill',
                    imageScalePadding: 0,
                    controlNavigation: 'bullets',
                    loop: true,
                    autoPlay: {
                        enabled: true,
                        pauseOnHover: true
                    }
                });
            }
        }
    });
}