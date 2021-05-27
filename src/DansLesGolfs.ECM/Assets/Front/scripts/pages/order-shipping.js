jQuery(document).ready(function ($) {
    $('#add-address-button').click(function (e) {
        var $this = $(this);

        var $table = $this.closest('table');
        var $editorRow = $table.find('tbody tr.editor-row');
        var $rows = $table.find('tbody tr:not(.editor-row)');
        if ($editorRow.css('display') == 'none') {
            if (window.addressId > 0) {
                $this.text("+ " + window.localize.editShippingAddress);
            } else {
                $this.text("+ " + window.localize.addShippingAddress);
            }
            prepareEditValue($editorRow, $table);

            $rows.hide();
            $editorRow.show();
        } else {
            $this.text("+ " + window.localize.addAddress);
            saveAddress($editorRow, $table);
        }
    });

    $('#address-table form').each(function () {
        this.onsubmit = function (e) {
            $('#add-address-button').trigger('click');
            return false;
        }
    });

    $('#checkout-button').click(onCheckOutButtonIsClicked);

    addAddressRowEvents();
});

function prepareEditValue($editRow, $table) {
    if (window.addressId > 0) {
        var $row = $('tbody tr[data-id=' + window.addressId + ']');
        jQuery('#AddressId', $editRow).val(window.addressId);
        jQuery('#Civilite').val($row.data('civilite'));
        jQuery('#Firstname').val($row.data('firstname'));
        jQuery('#Lastname').val($row.data('lastname'));
        jQuery('#Address').val($row.data('address'));
        jQuery('#Complement').val($row.data('complement'));
        jQuery('#PostalCode').val($row.data('postal-code'));
        jQuery('#City').val($row.data('city'));
        jQuery('#Country').val($row.data('country'));
        jQuery('#MobilePhone').val($row.data('mobile'));
        jQuery('#Digicode').val($row.data('digicode'));
        jQuery('#Floor').val($row.data('floor'));
        jQuery('#AddressName').val($row.data('address-name'));
    } else {
        jQuery('#AddressId', $editRow).val(0);
        jQuery('#Civilite').val("");
        jQuery('#Firstname').val("");
        jQuery('#Lastname').val("");
        jQuery('#Address').val("");
        jQuery('#Complement').val("");
        jQuery('#PostalCode').val("");
        jQuery('#City').val("");
        jQuery('#Country').val("");
        jQuery('#MobilePhone').val("");
        jQuery('#Digicode').val("");
        jQuery('#Floor').val("");
        jQuery('#AddressName').val("");
    }
}

function saveAddress($editorRow, $table) {
    $.post(getUrl('Order/SaveAddress'), $editorRow.find('form').serialize(), function (result) {
        if (result.isSuccess) {
            var $tbody = $table.find('tbody');
            $tbody.find('tr:not(.editor-row)').remove();
            $tbody.append(result.html);
            addAddressRowEvents();
            $editorRow.hide();
            window.addressId = 0;
        } else {
            console.error(result.message);
        }
    }, 'json');
}

function addSampleAddress() {
    var $editRow = jQuery('#address-table .editor-row');
    jQuery('#AddressId', $editRow).val(0);
    jQuery('#Civilite').val("1");
    jQuery('#Firstname').val("Weerayut");
    jQuery('#Lastname').val("Teja");
    jQuery('#Address').val("87 Moo.8 T.Nongpheun A.Saraphi");
    jQuery('#Complement').val("1");
    jQuery('#PostalCode').val("50140");
    jQuery('#City').val("Chiang Mai");
    jQuery('#Country').val("Thailand");
    jQuery('#MobilePhone').val("+66882613416");
    jQuery('#Digicode').val("N/A");
    jQuery('#Floor').val("3");
    jQuery('#AddressName').val("Apartment");
}

function onCheckOutButtonIsClicked(e) {
    if (jQuery('input[type=radio][name=AddressId]:checked').length <= 0) {
        e.preventDefault();
        return;
    }

    jQuery.post(getUrl('Order/SaveShippingData'), { addressId: jQuery('input[type=radio][name=AddressId]:checked').val() }, function (result) {
        window.location = getUrl('Order/Payment');
    }, 'json');
}

function addAddressRowEvents() {
    jQuery('#address-table tbody tr:not(.editor-row) .edit-link').click(function () {
        var $row = $(this).closest('tr');
        window.addressId = $row.data('id');
        $('#add-address-button').trigger('click');
    });
    
    jQuery('#address-table tbody tr:not(.editor-row) .delete-link').click(function () {
        if (!confirm(window.localize.confirmDeleteAddress))
            return;

        var $row = $(this).closest('tr');
        jQuery.post(getUrl('Order/DeleteAddress'), { addressId: $row.data('id') }, function (result) {
            if (result.isSuccess) {
                var $tbody = $('#address-table tbody');
                $tbody.find('tr:not(.editor-row)').remove();
                $tbody.append(result.html);
                addAddressRowEvents();
                window.addressId = 0;
            } else {
                console.error(result.message);
            }
        }, 'json');
    });
}