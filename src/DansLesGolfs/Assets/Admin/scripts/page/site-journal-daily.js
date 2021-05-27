jQuery(document).ready(function ($) {
    // Setup DatePicker
    $('#VisitDate').datepicker({
        dateFormat: 'dd/mm/yy',
        autoSize: true,
        firstDay: 1,
        onSelect: onChangeVisitDate,
    });

    $('#load-data-button').click(function (e) {
        var year = $('#VisitDate').val();
        loadYearlyData(year);
    });

    $('#VisitDate').keydown(function (e) {
        if (e.which == 13) {
            loadYearlyData(this.value);
            e.preventDefault();
        }
    });

    document.getElementById('form-site-journal').onsubmit = function (e) {
        var data = $(this).serialize();
        $.ajax({
            url: getUrl('Admin/SiteJournal/SaveDailyData'),
            type: 'POST',
            data: data,
            dataType: 'json',
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
    window.location = getUrl('Admin/SiteJournal/Daily/' + $('#SiteId').val() + '?visitDate=' + $('#VisitDate').val());
}

function onChangeVisitDate(e) {
    $('#load-data-button').click();
}