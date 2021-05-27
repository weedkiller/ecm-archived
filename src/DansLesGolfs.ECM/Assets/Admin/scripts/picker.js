jQuery(document).ready(function () {
    initDataTable();

    $('input[type=checkbox]').uniform();
    $('#select-button').click(function (e) {
        onSelectButtonIsClicked(window.onSelectData);
    });
});

function initDataTable() {

    window.dataTable = $('.dataTable').dataTable({
        "aaSorting": [[1, "asc"]],
        "bProcessing": false,
        "aoColumnDefs": [
            {
                'iWidth': 20,
                'bSortable': true,
                'aTargets': [0, window.columnLength - 2, window.columnLength - 1]
            }
        ],
        "bLengthChange": false,
        "bPaginate": false,
        "bInfo": false
    });
}

function onSelectButtonIsClicked(callback) {
    if (callback && typeof (callback) == 'function') {
        var data = new Array();
        jQuery('#datatable tbody input[type=checkbox]:checked').each(function () {
            var $row = jQuery(this).parents('tr');
            var obj = {};
            $row.find('td').each(function () {
                obj[jQuery(this).data('column-name')] = jQuery(this).data('column-value');
            });
            data.push(obj);
        });
        callback(data);
    }
    parent.jQuery.fancybox.close();
}