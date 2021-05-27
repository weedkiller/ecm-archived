window.map = null;
window.markers = new Array();
window.infowindow = null;
function initialize() {
    // Init info window object, because we need only one info window to show on map.
    window.infoWindow = new google.maps.InfoWindow();

    var mapOptions = {
        zoom: 6,
        center: new google.maps.LatLng(47, 2),
        zoomControl: true,
        zoomControlOptions: {
            position: google.maps.ControlPosition.LEFT_TOP
        },
        scaleControl: true,
        streetViewControl: true,
        streetViewControlOptions: {
            position: google.maps.ControlPosition.LEFT_TOP
        }
    };
    window.map = new google.maps.Map(document.getElementById('map-canvas'),
        mapOptions);

    google.maps.event.addListenerOnce(window.map, 'idle', function () {
        google.maps.event.addDomListener(window.map, 'center_changed', onFindCourseMapCenterChanged);
    });

    window.map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(document.getElementById('map-legend'));

    var latLng = null;
    var bounds = new google.maps.LatLngBounds();

    if (window.pinJson && typeof (window.pinJson) == 'object' && window.pinJson.length > 0) {
        for (var i in window.pinJson) {
            latLng = new google.maps.LatLng(window.pinJson[i].Latitude, window.pinJson[i].Longitude);
            addMarker(window.pinJson[i], latLng);
            bounds.extend(latLng);
        }

        // Don't zoom in too far on only one marker
        if (bounds.getNorthEast().equals(bounds.getSouthWest())) {
            var extendPoint1 = new google.maps.LatLng(bounds.getNorthEast().lat() + 0.001, bounds.getNorthEast().lng() + 0.001);
            var extendPoint2 = new google.maps.LatLng(bounds.getNorthEast().lat() - 0.001, bounds.getNorthEast().lng() - 0.001);
            bounds.extend(extendPoint1);
            bounds.extend(extendPoint2);
        }

        map.fitBounds(bounds);
        var currentZoom = window.map.getZoom();
        window.map.setZoom(currentZoom <= 1 ? 1 : currentZoom - 1);
    }
}

function addMarker(pin, latLng) {
    var marker = new google.maps.Marker({
        icon: getUrl('Assets/Front/img/pins/' + pin.PinIcon),
        position: latLng,
        map: window.map,
        title: pin.ItemName
    });

    //var content = '<h5 class="infowindow-title"><a href="' + pin.SiteUrl + '" target="_blank">' + pin.SiteName + '</a></h5><div class="infowindow-content">' + pin.SiteDescription + '</div>';
    var content = '<h5 class="infowindow-title"><a href="' + pin.SiteUrl + '" target="_blank">' + pin.SiteName + '</a></h5>';

    google.maps.event.addListener(marker, 'mouseover', function () {
        window.infoWindow.setContent(content);
        window.infoWindow.open(window.map, this);
    });

    window.markers.push(marker);
}

function onFindCourseMapCenterChanged() {
    if (window.mapSearch) {
        google.maps.event.clearListeners(window.mapSearch, 'center_changed');
        window.mapSearch.setCenter(window.map.getCenter());
        google.maps.event.addDomListener(window.mapSearch, 'center_changed', onMapSearchCenterChanged);
    }
}

initialize();