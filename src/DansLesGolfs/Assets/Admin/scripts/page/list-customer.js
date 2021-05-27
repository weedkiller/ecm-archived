jQuery(function ($) {
    $('.action-button').click(function () {
        var action = $('.actions-dropdown').val();
        if (action == 'export') {
            doExportCustomerData();
        }
    });
});

function doExportCustomerData() {
    window.location = getUrl('Admin/Customer/ExportData');
}