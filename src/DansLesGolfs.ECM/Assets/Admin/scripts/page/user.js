jQuery(document).ready(function ($) {
    $('input[name=PostalCode], input[name=ShippingPostalCode]').on('change', function () {
        loadCitiesDropDownList(this);
    }).trigger('change');
});

/**
* Load cities drop down list by postal code.
*/
function loadCitiesDropDownList(sender, callback) {
    var $this = jQuery(sender);
    var $panel = $this.parents('.panel');
    var $city = $panel.find('#CityId, #ShippingCityId');
    var $country = $panel.find('#CountryId');
    jQuery.ajax({
        url: getUrl('Common/GetDataByPostalCode'),
        type: 'post',
        dataType: 'json',
        data: { postalCode: jQuery('input[name=PostalCode]').val() },
        beforeSend: function () {
            showLoader(true, '.info-form');
        },
        success: function (result) {
            showLoader(false, '.info-form');
            if (result.isSuccess) {
                $country.val(result.countryId).prop('value', result.countryId).data('country-id', result.countryId);
                html = '';
                for (var i in result.cities) {
                    html += '<option value="' + result.cities[i].CityId + '">' + result.cities[i].CityName + '</option>';
                }
                $city.html(html);
                if ($city.data('city-id') > 0) {
                    $city.val($city.data('city-id')).data('city-id', 0);
                } else {
                    $city.val(result.cityId).data('city-id', 0);
                }

                $.uniform.update();
                if (callback && typeof (callback) == 'function') {
                    callback();
                }
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '.info-form');
        }
    });
}