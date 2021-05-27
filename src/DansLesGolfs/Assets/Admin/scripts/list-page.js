window.confirmDeleteText = "Are you sure for deleting this item?";
window.confirmDeleteSelectedText = "Are you sure for deleting these item?";

jQuery(document).ready(function ($) {
    initDataTable();

    $('#datatable thead .checkboxes input[type=checkbox]').click(function (e) {
        $('#datatable tbody tr td:first-child input[type=checkbox]').prop('checked', this.checked);
    });

    $('.action-button').click(function () {
        var selectedOption = $('.actions-dropdown').val();
        var ids = new Array();
        $('#datatable tbody tr td input[type=checkbox]:checked').each(function (i) {
            ids.push(eval(this.value));
        });
        if (selectedOption == "delete-selected") {
            if (confirm(window.confirmDeleteSelectedText)) {
                deleteItems(ids);
            }
        }
    });
});

function initDataTable() {

    window.dataTable = $('.dataTable').dataTable({
        "processing": true,
        "serverSide": true,
        "ajax": getUrl(window.areaName + '/' + window.controllerName + "/LoadDataJSON"),
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

    //window.dataTable = $('.dataTable').dataTable({
    //    //"aLengthMenu": [[1, 25, 50, -1], [1, 25, 50, "All"]],
    //    "aaSorting": [[1, "asc"]],
    //    "bProcessing": false,
    //    "sAjaxSource": getUrl(window.areaName + '/' + window.controllerName + "/LoadDataJSON"),
    //    "aoColumnDefs": [
    //        {
    //            'iWidth': 20,
    //            'bSortable': true,
    //            'aTargets': [0, window.columnLength - 2, window.columnLength - 1]
    //        }
    //    ],
    //    "fnDrawCallback": onDataTableDrawn,
    //    "sPaginationType": "bootstrap"
    //});
}

function reloadDataTable() {
    window.dataTable.fnClearTable(0);
    window.dataTable.fnDraw();
    //$('#datatable tbody tr.selected').remove();
}

function getRowsHTML(rows) {
    var html = '';
    for (var i in rows) {
        html += '<tr id="row-' + rows[i][window.primaryKey] + '">';
        html += '<td class="column-checkbox"><input type="checkbox" value="' + rows[i][window.primaryKey] + '" /></td>';
        for (var key in rows[i]) {
            if (key == window.primaryKey)
                continue;

            html += getColumnHTML(key, rows[i][key]);
        }
        html += '<td class="column-edit"><a class="edit-link" href="' + getUrl('Admin/' + window.controllerName + '/Form/' + rows[i][window.primaryKey]) + '">Edit</a>';
        html += '<td class="column-edit"><a href="javascript:void(0);" class="delete-link" data-id="' + rows[i][window.primaryKey] + '">Delete</a>';
        html += '</tr>';
    }
    return html;
}

function getColumnHTML(key, value) {
    return '<td class="column-' + key + '">' + value + '</td>';
}

function getUnorderableColumns() {
    var columns = new Array();
    jQuery('#datatable thead th').each(function (i) {
        if(jQuery(this).hasClass('unorderable'))
            columns.push(i);
    });
    return columns;
}

function deleteItems(ids) {
    $.ajax({
        url: getUrl(window.areaName + '/' + window.controllerName + '/Delete'),
        data: { 'ids': ids },
        traditional: true,
        dataType: 'json',
        type: 'post',
        success: function (result) {
            if (result.success) {
                reloadDataTable();
            }
        }
    });
}

function onDataTableDrawn() {
    $('#datatable tbody tr').click(function () {
        $('#datatable tbody tr').removeClass('selected');
        $(this).addClass('selected');
    });
    $('#datatable tbody .delete-link').on('click', function (e) {
        if (confirm(window.confirmDeleteText)) {
            var id = $(this).data('id');
            deleteItems([id]);
        }
    });
}
