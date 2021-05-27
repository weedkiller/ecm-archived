jQuery(document).ready(function ($) {
    window.grid = $('.k-grid').data('kendoGrid');
    $('#search-button').click(function () {
        filteringGridView();
    });

    $('#add-button').click(addRow);

    $('#delete-button').click(removeRow);

    $('#filter-panel input[type=text]').keyup(function (e) {
        if (e.which === 13) {
            $('#search-button').click();
        }
    });
});

function clear_filter() {
    window.grid.filterBy(0, "");
    window.grid._f_rowsBuffer = null;
}
function addRow() {
    clear_filter();
    window.grid.addRow(grid.uid(), 'new customer');
}
function removeRow() {
    clear_filter();
    window.grid.deleteSelectedRows();
}

function filteringGridView() {
    var data = $('#search-button').closest('form').serialize();
    data += "&page=" + $('.pager li.selected').data('page');
    data += "&pageSize=" + 5;
    jQuery.ajax({
        url: getUrl('Customer/AjaxSearchBulkCustomer?uid=' + window.grid.uid()),
        type: 'post',
        dataType: 'json',
        data: data,
        beforeSend: function () {
            showLoader(true, '#page-customer-bulk-edit');
        },
        success: function (result) {
            showLoader(false, '#page-customer-bulk-edit');
            if (result.isSuccess) {
                grid.clearAll();
                grid.parse(result.data, "json");
                updatePager(result.page, result.totalPages);
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#page-customer-bulk-edit');
            console.error(xhr);
        }
    });
}

function updatePager(page, totalPages) {
    var html = '<ul class="pagination">';
    for (var i = 1; i <= totalPages; i++) {
        html += '<li';
        if (i === page) {
            html += ' class="active"'
        }
        html += '><a href="javascript:void(0);">' + i + '</a></li>';
    }
    html += '</ul>';
    jQuery('.pager').html(html);
    jQuery('.pager li').click(function () {
        jQuery('.pager li').removeClass('active');
        jQuery(this).addClass('active');
    });
}

function bulkEditAdditionalData() {
    return {
        siteId: $('#SiteId').val()
    };
}