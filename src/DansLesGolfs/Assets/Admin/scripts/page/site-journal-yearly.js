jQuery(document).ready(function ($) {
    // Setup DatePicker
    $('.datepicker').datepicker({
        dateFormat: 'dd/mm/yy',
        autoSize: true,
        firstDay: 1
    });

    $('#HasChallenge, #HasPingClassicTour').change(function () {
        var $datepicker = $(this).parents('.form-group').find('.datepicker');
        $datepicker.prop('disabled', !this.checked);
        if (!this.checked) {
            $datepicker.val('');
        }
    });

    $('#load-data-button').click(function (e) {
        var year = $('#FiscalYear').val();
        loadYearlyData(year);
    });

    $('#FiscalYear').keydown(function (e) {
        if (e.which == 13) {
            loadYearlyData(this.value);
            e.preventDefault();
        }
    });

    $('input[name=SiteRestaurantSuppliers]').change(function () {
        if ($('input[name=SiteRestaurantSuppliers]:checked').length > 15) {
            $(this).prop('checked', false);
            $.uniform.update();
        }
    });

    $('input[name=SiteRestaurantProductCategories]').change(function () {
        if ($('input[name=SiteRestaurantProductCategories]:checked').length > 20) {
            $(this).prop('checked', false);
            $.uniform.update();
        }
    });

    document.getElementById('form-site-journal').onsubmit = function (e) {
        var data = $(this).serialize();
        $.ajax({
            url: getUrl('Admin/SiteJournal/SaveYearlyData'),
            type: 'POST',
            data: data,
            dataType: 'json',
            traditional: true,
            success: function (result) {
                if (result.isSuccess) {
                    showMessageModal('Information', result.message);
                } else {
                    showMessageModal('Error', result.message);
                }
            },
            error: function (xhr, msg) {
                showMessageModal('Error', msg);
            }
        });
        return false;
    };
});

function loadYearlyData(year) {
    window.location = getUrl('Admin/SiteJournal/Yearly/' + $('#id').val() + '?fiscalYear=' + $('#FiscalYear').val());
}