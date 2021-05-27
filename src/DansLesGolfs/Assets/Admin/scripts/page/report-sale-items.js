jQuery(function ($) {
    initDataTable();

    $('#refresh-button').click(onRefreshButtonIsClicked);
    $('#view-report-button').click(onViewReportButtonIsClicked);

    // Setup DatePicker
    $('.datepicker').datepicker({
        dateFormat: 'dd/mm/yy',
        autoSize: true,
        firstDay: 1
    });
});

function initDataTable() {

    window.dataTable = $('.dataTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": getUrl("Admin/SaleItemsReport/AjaxLoadData"),
            "type": "post",
            "data": function (d) {
                d.fromDate = jQuery('#FromDate').val();
                d.toDate = jQuery('#ToDate').val();
            }
        },
        "columnDefs": [
            {
                'width': 20,
                'orderable': false,
                'targets': getUnorderableColumns()
            }
        ],
        "order": [[1, "asc"]],
        "drawCallback": onDataTableDrawn,
        "paginationType": "bootstrap"
    });
}

function getUnorderableColumns() {
    var columns = new Array();
    jQuery('#datatable thead th').each(function (i) {
        if (jQuery(this).hasClass('unorderable'))
            columns.push(i);
    });
    return columns;
}

function onDataTableDrawn() {
    $('#datatable tbody tr').click(function () {
        $('#datatable tbody tr').removeClass('selected');
        $(this).addClass('selected');
    });
    $('#datatable tbody .delete-link').on('click', function (e) {
        if (confirm(window.confirmDeleteText)) {
            var id = $(this).data('id');
            var ids = [id];
            deleteReview(ids);
        }
    });
    $('#datatable tbody .approve-link').on('click', function (e) {
        var id = $(this).data('id');
        var isApproved = $(this).data('approved');
        setApprove(id, isApproved);
    });
}

function reloadTable() {
    window.dataTable.fnDraw();
}

function onRefreshButtonIsClicked(e) {
    reloadTable();
}

function onViewReportButtonIsClicked(e) {
    var fromDate = jQuery('#FromDate').val();
    var toDate = jQuery('#ToDate').val();
    var popup = window.open(getUrl('Admin/SaleItemsReport/ViewReport?fromDate=' + fromDate + '&toDate=' + toDate), 'saleitemsreport');
}