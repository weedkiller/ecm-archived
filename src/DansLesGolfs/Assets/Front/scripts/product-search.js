window.isLoadProducts = false;

jQuery(document).ready(function ($) {

    $('#open-rating-popup-button, #open-rating-finish-button').fancybox({
        afterLoad: function () {
            $(".fancybox-wrap").addClass("dlg-fancybox");
        }
    });

    // Rating Callback.
    $('.starrr').off('starrr:change').on('starrr:change', onUserGiveRating);
    jQuery('#submit-review-button').unbind('click').click(onSubmitRating);

    jQuery('#product-filter form select[name=CountryId]').change(function () {
        var $this = jQuery(this);
        var $region = $this.parents('fieldset').find('select[name=RegionId]');
        var countryId = $this.val();
        jQuery.ajax({
            url: getUrl('Common/AjaxGetRegionsByCountryId'),
            data: { countryId: countryId },
            dataType: 'json',
            type: 'post',
            beforeSend: function (xhr, opts) {
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
                }
            }
        });
    });

    jQuery('#product-filter select[name=StateId]').change(function () {
        var $this = jQuery(this);
        var $sites = $this.parents('fieldset').find('select[name=SiteId]');
        var $region = $this.parents('fieldset').find('select[name=RegionId]');
        var $country = $this.parents('fieldset').find('select[name=CountryId]');
        var stateId = $this.val();
        var regionId = $region.length > 0 ? $region.val() : 0;
        var countryId = $country.length > 0 ? $country.val() : 0;

        jQuery.ajax({
            url: getUrl('Common/AjaxGetSitesByStateId'),
            data: { stateId: stateId, regionId: regionId, countryId: countryId },
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
                }
            }
        });
    });

    jQuery('#product-filter select[name=RegionId]').change(function () {
        var $this = jQuery(this);
        var $states = $this.parents('fieldset').find('select[name=StateId]');
        var $country = $this.parents('fieldset').find('select[name=CountryId]');
        var regionId = $this.val();
        var countryId = $country.length > 0 ? $country.val() : null;

        jQuery.ajax({
            url: getUrl('Common/AjaxGetStatesByRegionId'),
            data: { regionId: regionId, countryId: countryId },
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
                    if ($states.find('option').length > 0) {
                        $states.find('option:eq(0)').attr('selected', 'selected');
                        $states.trigger('change');
                    }
                    $.uniform.update();
                }
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
                }
            }
        });
    });

    $('#product-popup .continue-to-shopping').click(function () {
        $.fancybox.close();
    });

    $('#load-more-button').click(onLoadMoreButtonIsClicked);

    if ($('body').hasClass('home')) {
        loadMoreProducts(8);
    } else {
        loadMoreProducts(10);
    }
});

function initializeMapInTab(callback) {
    if (window.google == undefined || window.google == null)
        return;

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

function onLoadMoreButtonIsClicked(e) {
    loadMoreProducts(15);
}

function loadMoreProducts(pageSize) {
    if (window.isLoadProducts === true)
        return;

    window.isLoadProducts = true;
    jQuery('#load-more-button').addClass('hide');
    if (!window.excludedItemIds) {
        window.excludedItemIds = new Array();
        jQuery('.dynamic-products-list .product').each(function () {
            excludedItemIds.push(jQuery(this).data('id'));
        });
    }
    jQuery.ajax({
        url: getUrl('ItemAjax/GetAnotherItems'),
        data: {
            exclude: window.excludedItemIds,
            pageSize: pageSize,
            searchTerm: window.searchTerm,
            itemTypeId: window.itemTypeId,
            countryId: window.countryId,
            regionId: window.regionId,
            stateId: window.stateId,
            siteId: window.siteId,
            itemCategoryId: window.itemCategoryId,
            itemSubCategoryId: window.itemSubCategoryId,
            golfLessonCategoryId: window.golfLessonCategoryId,
            timeSlot: window.timeSlot,
            includePractice: window.includePractice,
            includeAccommodation: window.includeAccommodation,
            departureMonth: window.departureMonth,
            fromDate: window.fromDate,
            toDate: window.toDate
        },
        traditional: true,
        dataType: 'json',
        type: 'post',
        beforeSend: function () {

        },
        success: function (result) {
            if (result.isSuccess) {
                var $elements = jQuery(result.html).appendTo('.dynamic-products-list');
                jQuery('.starrr', $elements).starrr();
                $('.starrr', $elements).off('starrr:change').on('starrr:change', onUserGiveRating);
                for (var i in result.itemIds) {
                    window.excludedItemIds.push(result.itemIds[i]);
                }

                if (result.isFinal) {
                    jQuery('#load-more-button').addClass('hide');
                    isLoadProducts = true;
                } else {
                    setTimeout(function () {
                        isLoadProducts = false;
                        jQuery('#load-more-button').removeClass('hide');
                    }, 1000);
                }
            }
        }
    });
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

/**************************** Item Rating *****************************/
function onUserGiveRating(e, value) {
    window.productElement = jQuery(this).closest('.product');
    window.itemId = window.productElement.data('id');
    window.rating = value;
    jQuery('#rating-popup .review-stars').data('rating', value).prop('rating', value);
    jQuery('#rating-popup .review-stars .star').removeClass('active');
    for (var i = 0; i < value; i++) {
        jQuery('#rating-popup .review-stars .star:eq(' + i + ')').addClass('active');
    }
    jQuery.ajax({
        url: getUrl('Common/CheckLogin'),
        type: 'post',
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess && result.isLoggedIn) {
                jQuery('#open-rating-popup-button').click();
                jQuery('#review-message-input').select();
            } else {
                window.location = getUrl('Login?returnUrl=' + encodeURI(window.location.href));
            }
        }
    });
}

function onSubmitRating(e) {
    e.stopPropagation();

    var subject = jQuery('#review-subject-input').val();
    var message = jQuery('#review-message-input').val();

    jQuery.ajax({
        url: getUrl('ItemAjax/UserGiveRating'),
        data: { itemId: window.itemId, rating: jQuery('#rating-popup .review-stars').data('rating'), subject: subject, message: message },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            $.fancybox.close();
            if (result.isSuccess) {
                if (window.productElement) {
                    jQuery('.starrr', window.productElement).removeClass('active');
                    jQuery('.starrr', window.productElement).each(function () {
                        for (var i = 0; i < result.averageRating; i++) {
                            jQuery(this).find('.star').eq(i).addClass('active');
                        }
                    });

                    $('#open-rating-finish-button').trigger('click');
                }
            }
        }
    });
}