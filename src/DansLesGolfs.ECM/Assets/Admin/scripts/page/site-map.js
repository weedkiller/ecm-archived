﻿window.map = null;
window.marker = null;

jQuery(document).ready(function ($) {
    if (window.latitude <= 0 || window.longitude <= 0) {
        window.latitude = window.defaultLatitude;
        window.longitude = window.defaultLongitude;
    }

    initialize(function () {
        addMarker(window.latitude, window.longitude);
    });
});

function initialize(callback) {
    var mapOptions = {
        zoom: 6,
        center: new google.maps.LatLng(window.latitude, window.longitude)
    };
    window.map = new google.maps.Map(document.getElementById('site-map-canvas'), mapOptions);

    google.maps.event.addDomListener(window.map, 'click', onMapIsClicked);
    google.maps.event.addDomListener(window.map, 'center_changed', onMapResized);
    google.maps.event.addDomListener(window.map, 'resize', onMapResized);

    if (callback != null && typeof (callback) == "function") {
        callback();
    }

    addMarker(window.latitude, window.longitude);
}

google.maps.event.addDomListener(window, 'load', initialize);

$('a[href="#site-map"]').on('shown.bs.tab', function (e) {
    google.maps.event.trigger(window.map, 'resize');
    window.map.setCenter(new google.maps.LatLng(window.latitude, window.longitude));
});

function onMapIsClicked(e) {
    addMarker(e.latLng.lat(), e.latLng.lng());
}
function addMarker(lat, lng) {
    if (window.marker != null) {
        window.marker.setMap(null);
        window.marker = null;
    }
    window.marker = new google.maps.Marker({
        position: new google.maps.LatLng(lat, lng),
        map: window.map
    });

    jQuery('input[name="Latitude"]').val(lat);
    jQuery('input[name="Longitude"]').val(lng);

    window.latitude = lat;
    window.longitude = lng;
}

function onMapResized() {
    if (window.marker != null && window.marker.getMap() != window.map) {
        window.marker.setMap(window.map);
    }
}