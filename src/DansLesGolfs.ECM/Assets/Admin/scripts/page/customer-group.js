jQuery(document).ready(function ($) {
    $('#customer-picker .delete-all-link').click(onDeleteAllCustomersButtonIsClicked);
    $('#customer-picker .delete-selected-link').click(onDeleteSelectedCustomersButtonIsClicked);
    $('#customers-table .delete-link').click(onCustomerDeleteLinkIsClicked);
    $('#select-customer-link').fancybox({
        afterLoad: function (e) {
            $('.fancybox-iframe').get(0).contentWindow.onSelectData = function (data) {
                addCustomerRows(data);
            };
        }
    });
});

function addCustomerRows(rows) {
    console.log(rows);
    var html = '';
    var $element;
    for (var i in rows) {
        html = '<tr data-id="' + rows[i]['UserId'] + '">';
        for (var j in rows[i]) {
            if (j === 'UserId') {
                html += '<td class="column-checkbox"><input type="checkbox" value="' + rows[i]['UserId'] + '" /><input type="hidden" name="CustomerIds" value="' + rows[i]['UserId'] + '" /></td>';
            } else {
                html += '<td>' + rows[i][j] + '</td>';
            }
        }
        html += '<td><a href="javascript:void(0);" class="delete-link">Delete</a></td>';
        html += '</tr>';
        $element = jQuery(html).appendTo('#customers-table');
        $element.find('.delete-link').click(onCustomerDeleteLinkIsClicked);
        $element.find('input[type=checkbox]').uniform.update();
    }
}

function onCustomerDeleteLinkIsClicked() {
    jQuery(this).parents('tr').remove();
}



function onDeleteAllCustomersButtonIsClicked(e) {
    e.preventDefault();

    if (!confirm("These items will be deleted permaently, are you sure to deleting them?"))
        return;

    jQuery('#customers-table tbody tr').remove();
}

function onDeleteSelectedCustomersButtonIsClicked(e) {
    e.preventDefault();

    if (!confirm("These items will be deleted permaently, are you sure to deleting them?"))
        return;

    var ids = new Array();
    jQuery('#customers-table tbody tr input[type=checkbox]:checked').each(function () {
        jQuery(this).parents('tr').remove();
    });
}