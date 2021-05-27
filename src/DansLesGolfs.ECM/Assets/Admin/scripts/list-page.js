window.confirmDeleteText = "Are you sure for deleting this item?";
window.confirmDeleteSelectedText = "Are you sure for deleting these item?";
window.confirmDeleteInBackgroundText = "There are a lot of data to be deleted, so all of them will be deleted in background process.\nAre you sure for deleting these item?";

jQuery(document).ready(function ($) {
    $('#datatable thead .checkboxes input[type=checkbox]').click(function (e) {
        $('#datatable tbody tr td:first-child input[type=checkbox]').prop('checked', this.checked);
    });

    $('.action-button').click(function () {
        var selectedOption = $('.actions-dropdown').val();
        var ids = new Array();
        $('#gridCRUD tbody tr td input[type=checkbox]:checked').each(function (i) {
            ids.push(eval(this.value));
        });
        console.log(ids);
        if (ids.length <= 0)
            return;

        if (selectedOption === "delete-selected") {
            if ($('#gridCRUD').data('select-all') === true) {
                if (confirm(window.confirmDeleteInBackgroundText)) {
                    var grid = $('#gridCRUD').data('kendoGrid');
                    kendo.ui.progress($('#gridCRUD'), true);
                    grid.dataSource.read();
                    grid.refresh();
                    $('#gridCRUD .k-grid-preheader .deselect-all-link').trigger('click');
                }
            } else {
                if (confirm(window.confirmDeleteSelectedText)) {
                    deleteItems(ids);
                }
            }
        }
    });
});

var kWindow = $("#notify-window").kendoWindow({
    title: window.confirmDeleteText,
    visible: false, //the window will not appear before its .open method is called
    width: "400px",
    height: "200px",
}).data("kendoWindow");

function deleteItems(ids) {
    $.ajax({
        url: getUrl(window.controllerName + '/Delete'),
        data: { 'ids': ids },
        traditional: true,
        dataType: 'json',
        type: 'post',
        success: function (result) {
            if (result.success) {
                var grid = $('#gridCRUD').data('kendoGrid');
                grid.dataSource.read();
                grid.refresh();
            }
        }
    });
}

function onGridRequestStart(e) {
    //kendo.ui.progress($('#gridCRUD'), true);
}
function onGridRequestEnd(e) {
    kendo.ui.progress($('#gridCRUD'), false);
}

function onGridDataBound() {
    $.uniform.update();
    $('#gridCRUD .k-grid-content table tbody .delete-link').on('click', function (e) {
        if (confirm(window.confirmDeleteText)) {
            var id = $(this).data('id');
            deleteItems([id]);
        }
    });

    updateEvents();
}

function onGridRequestData() {
    return {
        isDeleteMode: ($('#gridCRUD').data('select-all') === true)
    }
}

function updateEvents() {

    $('.sync-customer-group-button').click(function () {
        var id = jQuery(this).data('id');
        var siteId = jQuery(this).data('site-id');
        if (!id || !siteId)
            return;

        if (!confirm("Synchronization will be processed in background.\nDo you want to continue?\n\nNOTE: You will need to set Chronogolf Club ID in your Golf manager page."))
            return;

        jQuery.ajax({
            url: getUrl(`CustomerGroup/AjaxSyncCustomerGroups/${id}?siteId=${siteId}`),
            type: 'get',
            dataType: 'json',
            beforeSend: function () {
                showLoader(true);
            },
            success: function (result) {
                if (result.success) {
                    showLoader(false);
                } else {
                    console.error(result.message);
                }
            }
        });
    });
}