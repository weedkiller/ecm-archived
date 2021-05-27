jQuery(document).ready(function ($) {

    //initializeMapInTab();

    /*jQuery('#product-filter form:not(.where-to-play-form) select[name=CountryId]').change(function () {
        var $this = jQuery(this);
        var $states = $this.parents('fieldset').find('select[name=StateId]');
        var countryId = $this.val();
        jQuery.ajax({
            url: getUrl('Common/AjaxGetStatesByCountryId'),
            data: { countryId: countryId },
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (result.isResult) {
                    $states.html('');
                    for (var i in result.list) {
                        $states.append('<option value="' + result.list[i].StateId + '">' + result.list[i].StateName + '</option>');
                    }
                } else {
                    console.error(result.message);
                }
            },
            error: function (xhr, msg) {
                console.error(xhr);
            }
        });
    });*/

    jQuery('#product-filter form select[name=CountryId]').change(function () {
        var $this = jQuery(this);
        var $region = $this.parents('fieldset').find('select[name=RegionId]');
        var countryId = $this.val();
        jQuery.ajax({
            url: getUrl('Common/AjaxGetRegionsByCountryId'),
            data: { countryId: countryId },
            dataType: 'json',
            type: 'post',
            beforeSend: function(xhr, opts) {
                showLoader(true, '#product-filter');
            },
            success: function (result) {
                showLoader(false, '#product-filter');
                if (result.isResult) {
                    $region.html('');
                    for (var i in result.list) {
                        $region.append('<option value="' + result.list[i].RegionId + '">' + result.list[i].RegionName + '</option>');
                    }
                    if ($region.find('option').length > 0) {
                        $region.find('option:eq(0)').attr('selected', 'selected');
                        $region.trigger('change');
                    }
                    $.uniform.update();
                } else {
                    console.error(result.message);
                }
            },
            error: function (xhr, msg) {
                console.error(xhr);
            }
        });
    });

    jQuery('#product-filter select[name=StateId]').change(function () {
        var $this = jQuery(this);
        var $sites = $this.parents('fieldset').find('select[name=SiteId]');
        var stateId = $this.val();
        jQuery.ajax({
            url: getUrl('Common/AjaxGetSitesByStateId'),
            data: { stateId: stateId },
            dataType: 'json',
            type: 'post',
            beforeSend: function (xhr, opts) {
                showLoader(true, '#product-filter');
            },
            success: function (result) {
                showLoader(false, '#product-filter');
                if (result.isResult) {
                    $sites.html('');
                    for (var i in result.list) {
                        $sites.append('<option value="' + result.list[i].SiteId + '">' + result.list[i].SiteName + '</option>');
                    }
                    $.uniform.update();
                } else {
                    console.error(result.message);
                }
            },
            error: function (xhr, msg) {
                console.error(xhr);
            }
        });
    });

    jQuery('#product-filter select[name=RegionId]').change(function () {
        var $this = jQuery(this);
        var $states = $this.parents('fieldset').find('select[name=StateId]');
        var countryId = $this.val();
        jQuery.ajax({
            url: getUrl('Common/AjaxGetStatesByRegionId'),
            data: { countryId: countryId },
            dataType: 'json',
            type: 'post',
            beforeSend: function (xhr, opts) {
                showLoader(true, '#product-filter');
            },
            success: function (result) {
                showLoader(false, '#product-filter');
                if (result.isResult) {
                    $states.html('');
                    for (var i in result.list) {
                        $states.append('<option value="' + result.list[i].StateId + '">' + result.list[i].StateName + '</option>');
                    }
                    $.uniform.update();
                } else {
                    console.error(result.message);
                }
            },
            error: function (xhr, msg) {
                console.error(xhr);
            }
        });
    });

    jQuery('select[name=ItemCategoryId]').change(function () {
        var $this = jQuery(this);
        var $subCategory = $this.parents('fieldset').find('select[name=ItemSubCategoryId]');
        var categoryId = $this.val();
        jQuery.ajax({
            url: getUrl('Common/AjaxGetItemSubCategoriesByItemCategoryId'),
            data: { categoryId: categoryId },
            dataType: 'json',
            type: 'post',
            beforeSend: function (xhr, opts) {
                showLoader(true, '#product-filter');
            },
            success: function (result) {
                showLoader(false, '#product-filter');
                if (result.isResult) {
                    $subCategory.html('');
                    for (var i in result.list) {
                        $subCategory.append('<option value="' + result.list[i].CategoryId + '">' + result.list[i].CategoryName + '</option>');
                    }
                    $.uniform.update();
                } else {
                    console.error(result.message);
                }
            },
            error: function (xhr, msg) {
                console.error(xhr);
            }
        });
    });

    // Regis product popup link.
    //$('#product-popup-link').fancybox();

    //$('#products-list .product a:not(.view-offer)').click(function (e) {
    //    var $product = $(this).closest('.product');
    //    window.currentItemId = $product.data('id');
    //    var itemType = $product.data('itemType');

    //    if (itemType != 'product green-fee') {
    //        e.preventDefault();
    //        jQuery.ajax({
    //            url: getUrl('Cart/AddItem'),
    //            type: 'post',
    //            dataType: 'json',
    //            data: { itemId: window.currentItemId, quantity: 1 },
    //            success: function (result) {
    //                    showLoader(false, '#product-filter');
    //                $('#topnav #panier').trigger('loadCartItems');

    //                var $popup = $('#product-popup');
    //                var $popupItemName = $popup.find('.popup-content .item-name');
    //                var $popupItemImage = $popup.find('.popup-content .item-image');
    //                var $popupItemPrice = $popup.find('.popup-content .item-price');
    //                var itemUrl = $product.data('url');
    //                $popupItemName.html('<a href="' + itemUrl + '" title="' + $product.data('name') + '">' + $product.data('name') + '</a>');
    //                $popupItemImage.css('background-image', 'url(' + $product.data('image') + ')');
    //                $popupItemPrice.text($product.data('price'));
    //                //$('#product-popup .checkout-button').attr('href', itemUrl);
    //                $('#product-popup-link').click();
    //            },
    //            error: function (xhr, msg) {
    //                console.error(xhr);
    //            }
    //        });
    //    }
    //});
    $('#product-popup .continue-to-shopping').click(function () {
        $.fancybox.close();
    });

    // Open calendar by default.
    //$('#product-filter #green-fees-tab input[name=FromDate]').datepicker('show');

    initializeMapInTab();
});

function initializeMapInTab(callback) {
    if (jQuery('#search-map').length > 0) {
        var mapOptions = {
            zoom: 4,
            center: new google.maps.LatLng(window.defaultLatitude, window.defaultLongitude)
        };
        window.mapSearch = new google.maps.Map(document.getElementById('search-map'), mapOptions);

        google.maps.event.addDomListener(window.mapSearch, 'click', onMapSearchIsClicked);
        google.maps.event.addDomListener(window.mapSearch, 'center_changed', onMapSearchCenterChanged);
        google.maps.event.addDomListener(window.mapSearch, 'resize', onMapSearchResized);

        //jQuery('#product-filter .where-to-play-form select[name=CountryId], #product-filter .where-to-play-form select[name=RegionId], #product-filter .where-to-play-form select[name=StateId]').change(onWhereToPlayControlValueChanged);
        jQuery('#product-filter .where-to-play-form select[name=StateId]').change(onWhereToPlayControlValueChanged);

        if (callback && typeof (callback) == "function") {
            callback();
        }
    }
}

function onMapSearchIsClicked() {

}

function onMapSearchCenterChanged() {
    if (window.map) {
        google.maps.event.clearListeners(window.map, 'center_changed');
        window.map.setCenter(window.mapSearch.getCenter());
        google.maps.event.addDomListener(window.map, 'center_changed', onFindCourseMapCenterChanged);
    }
}

function onMapSearchResized() {

}

function onWhereToPlayControlValueChanged(e) {
    var countryName = "";
    var regionName = "";
    var stateName = "";
    if (jQuery('#product-filter .where-to-play-form select[name=CountryId]').val() > 0) {
        countryName = jQuery('#product-filter .where-to-play-form select[name=CountryId] option:selected').text().trim();
    }
    if (jQuery('#product-filter .where-to-play-form select[name=RegionId]').val() > 0) {
        regionName = jQuery('#product-filter .where-to-play-form select[name=RegionId] option:selected').text().trim();
    }
    if (jQuery('#product-filter .where-to-play-form select[name=StateId]').val() > 0) {
        stateName = jQuery('#product-filter .where-to-play-form select[name=StateId] option:selected').text().trim();
    }

    var address = "";
    if (stateName.trim() != '') {
        address += stateName + ", ";
    }
    if (regionName.trim() != '') {
        address += regionName + ", ";
    }
    if (countryName.trim() != '') {
        address += countryName + " ";
    }
    address = address.trim();
    if (address.substring(address.lengh - 1, 1) == ',') {
        address = address.substring(0, address.length - 1);
    }

    address = address.replace(/\s/g, '+');

    if (address.length == 0)
        return;

    var requestUrl = 'https://maps.googleapis.com/maps/api/geocode/json?address=' + address;
    requestUrl += '&key=' + window.googleAPIKey;

    var tempLatLng = window.mapSearch.getCenter();

    jQuery.get(requestUrl, function (output) {
        if (output.status == "OK" && output.results && output.results.length > 0) {
            var lat = output.results[0].geometry.location.lat;
            var lng = output.results[0].geometry.location.lng;
            var latLng = new google.maps.LatLng(lat, lng);
            window.mapSearch.setCenter(latLng);
        }
    }, 'json');
}