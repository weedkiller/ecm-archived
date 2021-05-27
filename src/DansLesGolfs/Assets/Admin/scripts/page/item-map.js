window.map = null;
window.marker = null;

jQuery(document).ready(function ($) {
    if (window.latitude == '' || window.latitude == null || window.latitude == undefined) {
        window.latitude = window.defaultLatitude;
    }
    if (window.longitude == '' || window.longitude == null || window.longitude == undefined) {
        window.longitude = window.defaultLongitude;
    }

    initialize(function () {
        addMarker(window.latitude, window.longitude);
    });
    /*if (window.latitude && window.longitude) {
        initialize(function () {
            addMarker(window.latitude, window.longitude);
        });
    } else {
        initGeolocation(function (position) {
            window.latitude = position.coords.latitude;
            window.longitude = position.coords.longitude;
            initialize();
        });
    }*/
});

function initialize(callback) {
    var mapOptions = {
        zoom: 6,
        center: new google.maps.LatLng(window.latitude, window.longitude)
    };
    window.map = new google.maps.Map(document.getElementById('item-map-canvas'), mapOptions);

    google.maps.event.addDomListener(window.map, 'click', onMapIsClicked);
    google.maps.event.addDomListener(window.map, 'center_changed', onMapResized);
    google.maps.event.addDomListener(window.map, 'resize', onMapResized);

    if (callback != null && typeof (callback) == "function") {
        callback();
    }

    addMarker(window.latitude, window.longitude);
}

google.maps.event.addDomListener(window, 'load', initialize);

$('a[href="#item-map"]').on('shown.bs.tab', function (e) {
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
        icon: getUrl('Assets/Front/img/pin' + window.itemTypeId + '.png'),
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