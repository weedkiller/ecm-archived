$(document).ready(function () {
    // Init Homepage Slider.
    $("#homepage-slider").royalSlider({
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
});