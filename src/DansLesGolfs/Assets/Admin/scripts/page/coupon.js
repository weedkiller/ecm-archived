window.couponPageSize = 500;

jQuery(document).ready(function ($) {
    $('#Prefix').change(function () {
        this.value = this.value.toUpperCase();
    });
    $('#select-item-link').fancybox({
        afterLoad: function (e) {
            $('.fancybox-iframe').get(0).contentWindow.onSelectData = function (data) {
                addItemRows(data);
            };
        }
    });
    $('#import-coupons-popup .ok-button').click(importCoupons);
    $('#confirm-import-coupons-popup .ok-button').click(sendImportCouponsRequest);
    $('#generate-coupons-button').click(onGenerateButtonIsClicked);
    $('#import-coupons-button').click(onImportButtonIsClicked);
    $('#coupons-table tbody .delete-link').click(onCouponDeleteLinkIsClicked);
    $('#items-table tbody .delete-link').click(onItemDeleteLinkIsClicked);
    $('#CouponUsageType').change(function () {
        if (this.value == 1) {
            $('#select-item-link, #item-picker').show();
            $('#categories-list').hide();
            $('#itemtypes-list').hide();
        } else if (this.value == 2) {
            $('#select-item-link, #item-picker').hide();
            $('#categories-list').show();
            $('#itemtypes-list').hide();
        } else if (this.value == 3) {
            $('#select-item-link, #item-picker').hide();
            $('#categories-list').hide();
            $('#itemtypes-list').show();
        } else {
            $('#select-item-link, #item-picker').hide();
            $('#categories-list').hide();
            $('#itemtypes-list').hide();
        }
    }).trigger('change');

    $('#UsagePeriodType').change(function () {
        if (this.value == 1 || this.value == 2) {
            $('.timestouse-controls').show();
        } else {
            $('.timestouse-controls').hide();
        }
    }).trigger('change');

    $('#ItemTypeId').change(function () {
        var itemTypeId = this.value;
        $('#categories-list #checkboxes-list li').hide();
        $('#categories-list #checkboxes-list li.item-type-' + itemTypeId).show();
    }).trigger('change');

    $('#item-picker .delete-all-link').click(onDeleteAllItemsButtonIsClicked);
    $('#item-picker .delete-selected-link').click(onDeleteSelectedItemsButtonIsClicked);
    $('#coupon-picker .delete-all-link').click(onDeleteAllCouponsButtonIsClicked);
    $('#coupon-picker .delete-selected-link').click(onDeleteSelectedCouponsButtonIsClicked);

    $('#coupons-table thead .column-checkbox input[type=checkbox]').click(function (e) {
        var $checkboxes = null;
        if (this.checked) {
            $checkboxes = $('#coupons-table tbody .column-checkbox input[type=checkbox]').prop('checked', true);
        } else {
            $checkboxes = $('#coupons-table tbody .column-checkbox input[type=checkbox]').prop('checked', false);
        }
        $.uniform.update($checkboxes);
    });

    // Add Pagination Event.
    addPaginationEvent();

    loadCoupons();
});

function loadCoupons(page, pageSize) {
    if (page == null || page == undefined)
        page = 1;

    if (pageSize == null || pageSize == undefined)
        pageSize = window.couponPageSize;

    jQuery.ajax({
        url: getUrl('Admin/Coupon/AjaxLoadCoupon'),
        data: {
            couponGroupId: window.couponGroupId,
            page: page,
            pageSize: pageSize
        },
        dataType: 'json',
        type: 'post',
        beforeSend: function() {
            showLoader(true, '#coupons.tab-pane');
        },
        success: function (results) {
            showLoader(false, '#coupons.tab-pane');
            if (results.isSuccess) {
                $('#coupons-table tbody').html('');
                for (var i in results.coupons) {
                    var $el = jQuery(getCouponHTML(results.coupons[i]));
                    $el.appendTo('#coupons-table tbody');
                    $el.find('.delete-link').click(onCouponDeleteLinkIsClicked);
                }
            }
            updateCouponPager(results);
        },
        error: function (xhr, msg) {
            showLoader(false, '#coupons.tab-pane');
        }
    });
}

function onGenerateButtonIsClicked(e) {
    jQuery.ajax({
        url: getUrl('Admin/Coupon/AjaxGenerateCoupons'),
        data: {
            couponGroupId: window.couponGroupId,
            prefix: $('#Prefix').val(),
            qty: $('#Qty').val()
        },
        dataType: 'json',
        type: 'post',
        beforeSend: function() {
            showLoader(true, '#coupons.tab-pane');
        },
        success: function (results) {
            showLoader(false, '#coupons.tab-pane');
            if (results.isSuccess) {
                window.location.reload(true);
            }
        }
});
}

/**
* On import coupons button is clicked.
*/
function onImportButtonIsClicked(e) {
    e.preventDefault();

    $('#import-coupons-popup').modal('show');
    $('#import-coupons-popup textarea').focus();
}

function onCouponDeleteLinkIsClicked(e) {
    e.preventDefault();

    if (!confirm("This coupon will be deleted permaently, are you sure to deleting it?"))
        return;

    var $row = jQuery(this).parents('tr');
    var ids = new Array();
    ids.push($row.data('id'));
    deleteCouponsByIds(ids);
}

function onDeleteAllCouponsButtonIsClicked(e) {
    e.preventDefault();

    if (!confirm("These coupons will be deleted permaently, are you sure to deleting them?"))
        return;

    var ids = new Array();
    jQuery('#coupons-table tbody tr').each(function () {
        ids.push(jQuery(this).data('id'));
    });

    deleteCouponsByIds(ids);
}

function onDeleteSelectedCouponsButtonIsClicked(e) {
    e.preventDefault();

    if (!confirm("These coupons will be deleted permaently, are you sure to deleting them?"))
        return;

    var ids = new Array();
    jQuery('#coupons-table tbody tr input[type=checkbox]:checked').each(function () {
        var $parent = jQuery(this).parents('tr');
        ids.push($parent.data('id'));
    });

    deleteCouponsByIds(ids);
}

function onDeleteAllItemsButtonIsClicked(e) {
    e.preventDefault();

    if (!confirm("These items will be deleted permaently, are you sure to deleting them?"))
        return;

    jQuery('#items-table tbody tr').remove();
}

function onDeleteSelectedItemsButtonIsClicked(e) {
    e.preventDefault();

    if (!confirm("These items will be deleted permaently, are you sure to deleting them?"))
        return;

    var ids = new Array();
    jQuery('#items-table tbody tr input[type=checkbox]:checked').each(function () {
        jQuery(this).parents('tr').remove();
    });
}

function deleteCouponsByIds(ids) {
    jQuery.ajax({
        url: getUrl('Admin/Coupon/AjaxDeleteCoupons'),
        data: { couponIds: ids },
        dataType: 'json',
        type: 'post',
        traditional: true,
        success: function (results) {
            if (results.isSuccess) {
                for (var i in ids) {
                    jQuery('#coupons-table tbody tr.coupon-' + ids[i]).remove();
                }
            }
        }
    });
}

function addItemRows(rows) {
    var html = '';
    var $element;
    for (var i in rows) {
        html = '<tr data-id="' + rows[i]['ItemId'] + '">';
        for (var j in rows[i]) {
            if (j == 'ItemId') {
                html += '<td class="column-checkbox"><input type="checkbox" value="' + rows[i]['ItemId'] + '" /><input type="hidden" name="ItemIds" value="' + rows[i]['ItemId'] + '" /></td>';
            } else {
                html += '<td>' + rows[i][j] + '</td>';
            }
        }
        html += '<td><a href="javascript:void(0);" class="delete-link">Delete</a></td>';
        html += '</tr>';
        $element = jQuery(html).appendTo('#items-table');
        $element.find('.delete-link').click(onItemDeleteLinkIsClicked);
        $element.find('input[type=checkbox]').uniform.update();
    }
}

function onItemDeleteLinkIsClicked() {
    jQuery(this).parents('tr').remove();
}

function importCoupons() {
    $.fancybox.close();

    var coupons = $('#import-coupons-popup textarea').val().split('\n');
    window.availableCoupons = new Array();

    $.ajax({
        url: getUrl('Admin/Coupon/AjaxCheckCouponCodeAvailable'),
        data: {
            id: window.couponGroupId,
            coupons: coupons
        },
        traditional: true,
        type: 'post',
        dataType: 'json',
        beforeSend: function () {
            showLoader(true, '#import-coupons-popup');
        },
        success: function (result) {
            showLoader(false, '#import-coupons-popup');
            if (result.isSuccess) {
                $('#import-coupons-popup textarea').val('');
                $('#import-coupons-popup').modal('hide');
                $('#confirm-import-coupons-popup .popup-content').html(result.html);
                $('#confirm-import-coupons-popup').modal('show');
                window.availableCoupons = result.availableCoupons;
            } else {
                $('#error-popup .popup-content').html(result.message);
                $('#error-popup').modal('show');
                $('#import-coupons-popup').modal('hide');
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#import-coupons-popup');
            $('#error-popup .popup-content').html(msg);
            $('#error-popup').modal('show');
            $('#import-coupons-popup').modal('hide');
        }
    });
}

function sendImportCouponsRequest() {
    $.ajax({
        url: getUrl('Admin/Coupon/AjaxImportCoupons'),
        data: {
            id: window.couponGroupId,
            coupons: window.availableCoupons
        },
        traditional: true,
        type: 'post',
        dataType: 'json',
        beforeSend: function () {
            showLoader(true, '#confirm-import-coupons-popup');
        },
        success: function (result) {
            showLoader(false, '#confirm-import-coupons-popup');
            if (result.isSuccess) {
                $('#import-coupons-popup textarea').val('');
                $('#import-coupons-popup').modal('hide');
                window.location.has = '#coupons';
                window.location.reload(true);
            } else {
                $('#error-popup .popup-content').html(result.message);
                $('#error-popup').modal('show');
                $('#import-coupons-popup').modal('hide');
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#confirm-import-coupons-popup');
            $('#error-popup .popup-content').html(msg);
            $('#error-popup').modal('show');
            $('#import-coupons-popup').modal('hide');
        }
    });
}

function getCouponHTML(coupon) {
    var html = '<tr class="coupon-' + coupon.couponId + '" data-id="' + coupon.couponId + '">';
    html += '<td class="column-checkbox">';
    html += '<input class="coupon-' + coupon.couponId + '" type="checkbox" value="' + coupon.couponId + '" /></td>';
    html += '<td>' + coupon.couponCode + '</td>';
    html += '<td><span class="label label-' + (coupon.isAvailable == true ? 'success' : 'danger') + '">' + coupon.availableText + '</span></td>';
    html += '<td><a href="javascript:void(0)" class="delete-link">' + window.deleteText + '</a></td>';
    html += '</tr>';
    return html;
}

function updateCouponPager(results) {
    var html = '<li data-page="1"><a href="#" aria-label="Previous"><span aria-hidden="true">&laquo;</span></a></li>';
    for(var i = 1; i <= results.totalPages; i++) {
        html += ' <li data-page="' + i + '"><a href="#">' + i + '</a></li>';
    }
    html += '<li data-page="' + results.totalPages + '"><a href="#" aria-label="Next"><span aria-hidden="true">&raquo;</span></a></li>';
    $('#coupons .pagination').html(html);
    addPaginationEvent();
}

function addPaginationEvent() {
    $('#coupons .pagination li').unbind('click')
    .bind('click', function (e) {
        loadCoupons($(this).data('page'));
    });
}