jQuery(function ($) {

    // Load cities drop down list.
    loadCitiesDropDownList();

    $('input[name=PostalCode]').change(function () {
        loadCitiesDropDownList();
    });

    $('input[name=PhoneCountryCode], input[name=MobilePhoneCountryCode]').mask('99');
    $('input[name=Phone], input[name=MobilePhone]').mask('9999999999');
});

/**
* Load cities drop down list by postal code.
*/
function loadCitiesDropDownList(callback) {
    jQuery.ajax({
        url: getUrl('Common/GetDataByPostalCode'),
        type: 'post',
        dataType: 'json',
        data: { postalCode: jQuery('input[name=PostalCode]').val() },
        beforeSend: function () {
            showLoader(true, '#form-aa');
        },
        success: function (result) {
            showLoader(false, '#form-aa');
            if (result.isSuccess && result.cityId > 0) {
                jQuery('#form-aa #CountryId').val(result.countryId).prop('value', result.countryId).data('country-id', result.countryId);
                html = '';
                for (var i in result.cities) {
                    html += '<option value="' + result.cities[i].CityId + '">' + result.cities[i].CityName + '</option>';
                }
                jQuery('#form-aa #CityId').html(html);
                if (jQuery('#form-aa #CityId').data('city-id') > 0) {
                    jQuery('#form-aa #CityId').val(jQuery('#form-aa #CityId').data('city-id'));
                } else {
                    jQuery('#form-aa #CityId').val(result.cityId).data('city-id', result.cityId);
                }

                jQuery.uniform.update();
                if (callback && typeof (callback) == 'function') {
                    callback();
                }
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#form-aa');
        }
    });
}