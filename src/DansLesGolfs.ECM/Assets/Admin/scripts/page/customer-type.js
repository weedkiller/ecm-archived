jQuery(document).ready(function ($) {
    $('#SiteId').change(onSiteChange).trigger('change');
    $('#ParentId').change(onParentCustomerTypeChanged);
});

function onSiteChange(e) {
    loadCustomerTypes();
    loadParentCustomerTypes();
}

function onParentCustomerTypeChanged(e) {
    var $this = jQuery(this);
    $this.closest('form-group').data('parent-id', $this.val());
}

function loadParentCustomerTypes() {
    jQuery.ajax({
        url: getUrl('CustomerType/GetParentCustomerTypes'),
        data: { siteId: jQuery('#SiteId').val(), skipId: jQuery('#id').val() },
        type: 'post',
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                jQuery('#ParentId').html(result.html);
                var parentId = jQuery('#ParentId').closest('form-group').data('parent-id');
                if (parentId > 0) {
                    jQuery('#ParentId option').removeAttr('selected');
                    jQuery('#ParentId option[value=' + parentId + ']').attr('selected', 'selected');
                }
            } else {
                console.error(result.message);
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
        }
    });
}

function loadCustomerTypes() {
    var id = $('#id').val();
    var siteId = $('#SiteId').val();
    $.ajax({
        url: getUrl(`CustomerType/AjaxGetAffiliationTypes/${id}?siteId=${siteId}`),
        type: "get",
        dataType: "json",
        success: function (result) {
            if (result.success) {
                var data = result.data;
                var html = '<option value="0">None</option>';
                for (var i in result.data) {
                    html += `<option value="${data[i].id}" ${data[i].selected ? `selected="selected"` : ``}>${data[i].name}</option>`;
                }
                $('#AffiliationTypeId').html(html);
            } else {
                console.error(result.message);
            }
        }
    });
}