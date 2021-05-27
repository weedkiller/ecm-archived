jQuery(document).ready(function ($) {
    $('#CourseId').change(function () {
        var courseId = jQuery(this).val();
        if (courseId > 0) {
            jQuery.ajax({
                url: getUrl('Admin/GreenFee/AjaxGetCourseDefaultPrice'),
                type: 'post',
                dataType: 'json',
                data: { courseId: courseId },
                success: function (result) {
                    if (result.isSuccess) {
                        window.defaultPrice = result.defaultPrice;
                        reloadTeeSheet();
                    } else {
                        alert(result.message);
                    }
                },
                error: function (xhr, msg) {
                    console.error(xhr);
                }
            });
        }
    });
    $('#CourseId').trigger('change');
    reloadTeeSheet();
});
function reloadTeeSheet() {

    $('#teesheet-editor').teeSheet({
        startHour: 8,
        endHour: 18,
        showDateNav: false,
        defaultPrice: window.defaultPrice,
        currency: window.currency,
        breakHours: [12],
        data: window.teeSheetData,

        onPriceChanged: function ($teesheet) {
            var price = Math.round(eval($teesheet.data('price')));
            var percent = Math.round(eval($teesheet.data('percent')));
            if (price <= 0 && percent <= 0) {
                $teesheet.removeClass('discount-1')
                    .removeClass('discount-2')
                    .removeClass('discount-3')
            }
            else if (price > 0 && percent <= 10) {
                $teesheet.removeClass('discount-2')
                    .removeClass('discount-3')
                    .addClass('discount-1');
            } else if (percent < 40) {
                $teesheet.removeClass('discount-1')
                    .removeClass('discount-3')
                    .addClass('discount-2');
            } else {
                $teesheet.removeClass('discount-1')
                    .removeClass('discount-2')
                    .addClass('discount-3');
            }
        },
        onClearTeeSheet: function ($teesheet) {
            $teesheet.removeClass('discount-0 discount-1 discount-2 discount-3');
        }
    });
}