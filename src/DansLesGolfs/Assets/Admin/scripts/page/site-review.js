jQuery(function ($) {
    initDataTable();
});

function initDataTable() {

    window.dataTable = $('.dataTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": getUrl("Admin/SiteReview/AjaxLoadData"),
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

function deleteReview(ids) {
    $.ajax({
        url: getUrl('Admin/SiteReview/AjaxDelete'),
        data: { ids: ids },
        dataType: 'json',
        type: 'post',
        traditional: true,
        success: function (result) {
            if (result.isSuccess) {
                reloadTable();
            }
        }
    });
}

function setApprove(id, isApproved) {
    $.ajax({
        url: getUrl('Admin/SiteReview/AjaxSetApproval'),
        data: { id: id, approved: isApproved },
        dataType: 'json',
        type: 'post',
        traditional: true,
        success: function (result) {
            if (result.isSuccess) {
                reloadTable();
            }
        }
    });
}

function reloadTable() {
    window.dataTable.fnDraw();
}