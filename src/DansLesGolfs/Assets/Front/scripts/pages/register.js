jQuery(function ($) {
    loadCitiesDropDownList();

    $('input[name=postalcode]').change(function () {
        loadCitiesDropDownList();
    });

    $('input[name=Phone], input[name=MobilePhone]').mask('9999999999');
});

/**
* Load cities drop down list by postal code.
*/
function loadCitiesDropDownList(callback) {
    jQuery.ajax({
        url: getUrl('Account/GetDataByPostalCode'),
        type: 'post',
        dataType: 'json',
        data: { postalCode: jQuery('input[name=postalcode]').val() },
        beforeSend: function () {
            showLoader(true, '#MyDetailsForm');
        },
        success: function (result) {
            showLoader(false, '#register-form');
            if (result.isSuccess && result.cityId > 0) {
                jQuery('#CountryId').val(result.countryId).prop('value', result.countryId).data('country-id', result.countryId);
                html = '';
                for (var i in result.cities) {
                    html += '<option value="' + result.cities[i].CityId + '">' + result.cities[i].CityName + '</option>';
                }
                jQuery('#MyDetailsForm #ddlCity').html(html);
                if (jQuery('#MyDetailsForm #ddlCity').data('city-id') > 0) {
                    jQuery('#MyDetailsForm #ddlCity').val(jQuery('#MyDetailsForm #ddlCity').data('city-id'));
                } else {
                    jQuery('#MyDetailsForm #ddlCity').val(result.cityId).data('city-id', result.cityId);
                }

                jQuery.uniform.update();
                if (callback && typeof (callback) == 'function') {
                    callback();
                }
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#register-form');
        }
    });
}