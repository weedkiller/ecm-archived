window.isAddressFormValid = true;

jQuery(document).ready(function ($) {
    prepareShippingData();

    $('#address-creation-link, #address-modified-link, #address-required-link, #address-delete-link').fancybox({
        autoScale: true,
        modal: true,
        minHeight: 20,
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        }
    });
    $('#address-creation-popup button, #address-modified-popup button, #address-required-popup button, #address-delete-popup button').click(function () { $.fancybox.close(); });

    // Register editor control events.
    $('#address-table tr.editor-row input, #address-table tr.editor-row select').change(function () {
        window.isFormModified = true;
    });

    $('#address-table .editor-row select').uniform({ selectAutoWidth: false });

    $('#add-address-button').click(function (e) {
        var $this = $(this);
        window.isAddressFormValid = false;
        window.isFormModified = true;

        var $table = $this.closest('table');
        var $tfoot = $table.find('tfoot');
        var $editorRow = $table.find('tbody tr.editor-row');
        var $rows = $table.find('tbody tr:not(.editor-row)');
        if ($editorRow.css('display') == 'none') {
            prepareEditValue($editorRow, $table);
            showEditorRow(true);

        } else {
            saveAddress($editorRow, $table);
            if (window.isAddressFormValid === true) {
                $this.text("+ " + window.localize.addAddress);
                window.isFormModified = false;
            }
        }
    });

    $('#address-table form').each(function () {
        this.onsubmit = function (e) {
            $('#add-address-button').trigger('click');
            return false;
        }
    }).validate();

    $('#checkout-button').click(onCheckOutButtonIsClicked);

    addAddressRowEvents();

    $('#address-table tr:not(.editor-row) input[type=radio]').uniform();

    $('#address-modified-popup .yes-button').click(function () { jQuery('#add-address-button').trigger('click'); });

    checkNoAddress();
    $('#address-creation-popup .yes-button').click(onAddressCreationSuggestionIsClicked);

    $('#address-delete-popup .yes-button').click(onDeleteConfirmButtonIsClicked);

    $('.editor-row #PostalCode').change(onPostalCodeChanged);

    // Prevent these field to input non-numeric characters.
    $('.editor-row #MobilePhone').numeric();
    $('.editor-row #MobilePhoneCountryCode').numeric();

    $('#back-to-shopping-button').click(onGoBackButtonIsClicked);
});

function prepareEditValue($editRow, $table) {
    if (window.addressId > 0) {
        var $row = $('tbody tr[data-id=' + window.addressId + ']');
        jQuery('#AddressId', $editRow).val(window.addressId);
        if ($row.data('title-id') && $row.data('title-id') > 0) {
            jQuery('#TitleId').val($row.data('title-id'));
        } else {
            jQuery('#TitleId').val(jQuery('#TitleId option:eq(0)').val());
        }
        jQuery('#Firstname').val($row.data('firstname'));
        jQuery('#Lastname').val($row.data('lastname'));
        jQuery('#Address').val($row.data('address'));
        jQuery('#Complement').val($row.data('complement'));
        jQuery('#PostalCode').val($row.data('postal-code'));
        if ($row.data('city-id') && $row.data('city-id') > 0) {
            jQuery('#CityId').val($row.data('city-id'));
        } else {
            jQuery('#CityId').val(jQuery('#CityId option:eq(0)').val());
        }
        jQuery('#Country').val($row.data('country'));
        jQuery('#MobilePhoneCountryCode').val($row.data('country-code'));
        jQuery('#MobilePhone').val($row.data('mobile'));
        jQuery('#Digicode').val($row.data('digicode'));
        jQuery('#Floor').val($row.data('floor'));
        jQuery('#AddressName').val($row.data('address-name'));
        loadCitiesDropDownList(function () {
            if ($row.data('city-id') && $row.data('city-id') > 0) {
                jQuery('#CityId').val($row.data('city-id'));
            } else {
                jQuery('#CityId').val(jQuery('#CityId option:eq(0)').val());
            }
            jQuery.uniform.update();
        });
    } else {
        jQuery('#AddressId', $editRow).val(0);
        jQuery('#TitleId').val(jQuery('#TitleId option:eq(0)').val());
        jQuery('#Firstname').val("");
        jQuery('#Lastname').val("");
        jQuery('#Address').val("");
        jQuery('#Complement').val("");
        jQuery('#PostalCode').val("");
        jQuery('#CityId').val(jQuery('#CityId option:eq(0)').val());
        jQuery('#Country').val("");
        jQuery('#MobilePhoneCountryCode').val("");
        jQuery('#MobilePhone').val("");
        jQuery('#Digicode').val("");
        jQuery('#Floor').val("");
        jQuery('#AddressName').val("");
    }
    jQuery.uniform.update();
}

function saveAddress($editorRow, $table) {
    if (!window.isFormModified) {
        showEditorRow(false);
        window.addressId = 0;
        return;
    }

    if (!jQuery('#address-table form').valid()) {
        jQuery('#address-required-link').trigger('click');
        window.isAddressFormValid = false;
        return;
    } else {
        window.isAddressFormValid = true;
    }

    $.ajax({
        url: getUrl('Order/SaveAddress'),
        data: $editorRow.find('form').serialize(),
        dataType: 'json',
        type: 'post',
        async: false,
        beforeSend: function () {
            showLoader(true, '#address-table');
        },
        success: function (result) {
            showLoader(false, '#address-table');

            if (result.isSuccess) {
                isAddressFormValid = true;
                var $tbody = $table.find('tbody');
                var $tfoot = $table.find('tfoot');
                $tbody.find('tr:not(.editor-row)').remove();
                $tbody.append(result.html);
                addAddressRowEvents();
                showEditorRow(false);
                window.addressId = 0;
                $('#address-table tr:not(.editor-row) input[type=radio]').uniform();
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#address-table');
        }
    });
}

function addSampleAddress() {
    var $editRow = jQuery('#address-table .editor-row');
    jQuery('#AddressId', $editRow).val(0);
    jQuery('#TitleId').val("1");
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
    e.preventDefault();

    if (window.isFormModified) {
        jQuery('#add-address-button').trigger('click');

        if (jQuery('input[type=radio][name=AddressId]:checked').length < 1) { // If after save address, there are more than 1 address, do nothing let user choose prefer address.
            jQuery('input[type=radio][name=AddressId]').eq(0).prop('checked', true);
        }
        jQuery.uniform.update();


        return;
    }

    if (jQuery('input[type=radio][name=AddressId]:checked').length <= 0) {
        return;
    }

    if (window.isAddressFormValid === true) {
            jQuery.post(getUrl('Order/SaveShippingData'), { addressId: jQuery('input[type=radio][name=AddressId]:checked').val() }, function (result) {
                window.location = getUrl('Order/Payment');
            }, 'json');
    }
}

function addAddressRowEvents() {
    jQuery('#address-table tbody tr:not(.editor-row) .edit-link').click(function () {
        var $row = $(this).closest('tr');
        window.addressId = $row.data('id');
        $('#add-address-button').trigger('click');
    });

    jQuery('#address-table tbody tr:not(.editor-row) .delete-link').click(function () {
        $('#address-delete-link').trigger('click');
        window.focusedRow = $(this).closest('tr');
    });
}

function onGoBackButtonIsClicked(e) {
    e.preventDefault();
    e.stopPropagation();

    if (jQuery('#address-table tbody tr.editor-row').css('display') == 'none') {
        window.location = getUrl('Cart');
    } else {
        window.isFormModified = false;
        jQuery('#add-address-button').click();
    }
}

function onDeleteConfirmButtonIsClicked(e) {
    if (!window.focusedRow)
        return;

    var $row = window.focusedRow;
    jQuery.post(getUrl('Order/DeleteAddress'), { addressId: $row.data('id') }, function (result) {
        if (result.isSuccess) {
            var $tbody = $('#address-table tbody');
            var $tfoot = $('#address-table tfoot');
            $tbody.find('tr:not(.editor-row)').remove();
            $tfoot.show();
            jQuery('#required-fields-warning').hide();
            $tbody.append(result.html);
            addAddressRowEvents();
            window.addressId = 0;
        }
    }, 'json');
}

function checkNoAddress() {
    if (window.hasNoAddress) {
        $('#address-creation-link').trigger('click');
    }
}

function onAddressCreationSuggestionIsClicked() {
    jQuery.ajax({
        url: getUrl('Order/CreateShippingAddressByProfileAddress'),
        type: 'post',
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                window.location.reload();
            }
        }
    });
}

function onPostalCodeChanged() {
    loadCitiesDropDownList();
}

function loadCitiesDropDownList(callback) {
    jQuery.ajax({
        url: getUrl('Order/GetDataByPostalCode'),
        type: 'post',
        dataType: 'json',
        data: { postalCode: jQuery('.editor-row #PostalCode').val() },
        beforeSend: function () {
            showLoader(true, '#address-table');
        },
        success: function (result) {
            showLoader(false, '#address-table');
            if (result.isSuccess && result.cityId > 0) {
                jQuery('.editor-row #Country').val(result.country).prop('value', result.country);
                html = '';
                for (var i in result.cities) {
                    html += '<option value="' + result.cities[i].CityId + '">' + result.cities[i].CityName + '</option>';
                }
                jQuery('.editor-row #CityId').html(html);
                jQuery.uniform.update();
                jQuery('.editor-row #CityId').val(result.cityId);
                if (callback && typeof (callback) == 'function') {
                    callback();
                }
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#address-table');
        }
    });
}

function prepareShippingData() {
    jQuery.post(getUrl('Order/PrepareShippingCacheData'), function (result) {
        if (!result.isSuccess) {
        }
    })
}

function showEditorRow(isShow) {
    if (isShow) {
        jQuery('#add-address-button').hide();
        jQuery('.address-table-title').text(window.localize.shippingEditorRowTitle);
        jQuery('.editor-row').show();
        $('#address-table tbody tr:not(.editor-row)').hide();
        $('#address-table tfoot').hide();
        jQuery('#required-fields-warning').show();
    } else {
        jQuery('#add-address-button').show();
        jQuery('.address-table-title').text(window.localize.shipping);
        jQuery('.editor-row').hide();
        $('#address-table tbody tr:not(.editor-row)').show();
        $('#address-table tfoot').show();
        jQuery('#required-fields-warning').hide();
    }
    if (window.addressId > 0) {
        jQuery('#add-address-button').text("+ " + window.localize.editShippingAddress);
    } else {
        jQuery('#add-address-button').text("+ " + window.localize.addShippingAddress);
    }
}