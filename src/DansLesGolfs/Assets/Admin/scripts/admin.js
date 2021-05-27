window.tinymceOptions = {
    selector: "textarea.editor",
    skin: "lightgray",

    content_css: ["/Assets/Libraries/bootstrap/css/bootstrap.min.css", "/Assets/Front/css/style.css"],

    plugins: [
            "advlist autolink autosave link image lists charmap print preview hr anchor pagebreak spellchecker",
            "searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking",
            "table contextmenu directionality emoticons template textcolor paste fullpage textcolor moxiemanager"
    ],

    toolbar1: "styleselect formatselect fontselect fontsizeselect | cut copy paste undo redo",
    toolbar2: "bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | bullist numlist | outdent indent blockquote | forecolor backcolor | searchreplace",
    toolbar3: "table | hr removeformat | subscript superscript | charmap emoticons | print fullscreen | ltr rtl | visualchars visualblocks nonbreaking template pagebreak restoredraft",
    toolbar4: "spellchecker | link unlink anchor image media code",

    menubar: false,
    toolbar_items_size: 'small',

    style_formats: [
            { title: 'Bold text', inline: 'strong' },
            { title: 'Italic text', inline: 'em' },
            { title: 'Red text', inline: 'span', styles: { color: '#ff0000' } },
            { title: 'Red header', block: 'h1', styles: { color: '#ff0000' } },
            { title: 'Lead Cap', block: 'p', classes: 'lead' },
            { title: 'Well', block: 'p', classes: 'well' },
            { title: 'Well Pink', block: 'p', classes: 'well pink' }
    ],

    forced_root_block: "",
    force_p_newlines: true,
    force_br_newlines: false,
    relative_urls : false,
    remove_script_host: false
};

jQuery(document).ready(function ($) {

    if (window.menuClassName) {

        var $li = $('.page-sidebar-menu li.' + window.menuClassName).addClass('active');
        var $parentLi = $li.parents('li');
        $parentLi.addClass('active');
    }
    else {
        $('.page-sidebar-menu > li:first-child').addClass('active');
    }


    var startdate = $('.startdate').datetimepicker({
        language: 'fr',
        defaultDate: "11/1/2013",
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        minView: 2,
        forceParse: 0,
        format: "yyyy-mm-dd"
    });

    var enddate = $('.enddate').datetimepicker({
        language: 'fr',
        defaultDate: "11/1/2013",
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        minView: 2,
        forceParse: 0,
        format: "yyyy-mm-dd"
    });


    $('.startdate').datetimepicker('setDate', new Date());
    $('.enddate').datetimepicker('setDate', new Date());
    //$('.startdate').setDate("10/23/2013");

    var startdatetime = null;
    var enddatetime = null;


    //startdate.change(function (ev) {

    //    startdatetime = $('.startdate').datetimepicker("getDate").toJSON();
    //    enddatetime = $('.enddate').datetimepicker("getDate").toJSON();

    //    $.ajax({
    //        type: "POST",
    //        url: getUrl('admin/reports/GetSalesReportPartial'),
    //        contentType: 'application/json',
    //        dataType: "html",
    //        data: JSON.stringify({ "StartDate": startdatetime, "EndDate": enddatetime }),
    //        success: function (data) {
    //            $("#SalesData").html("");
    //            $("#SalesData").html(data);
    //            $("#SalesDatapopup").html("");
    //            $("#SalesDatapopup").html(data);
    //        }
    //    });
    //});

    //enddate.change(function (ev) {
    //    startdatetime = $('.startdate').datetimepicker("getDate").toJSON();
    //    enddatetime = $('.enddate').datetimepicker("getDate").toJSON();

    //    $.ajax({
    //        type: "POST",
    //        url: getUrl('admin/reports/GetSalesReportPartial'),
    //        contentType: 'application/json',
    //        dataType: "html",
    //        data: JSON.stringify({ "StartDate": startdatetime, "EndDate": enddatetime }),
    //        success: function (data) {
    //            $("#SalesData").html("");
    //            $("#SalesData").html(data); 
    //            $("#SalesDatapopup").html("");
    //            $("#SalesDatapopup").html(data); 
    //        }
    //    });
    //});

    $("#showreportbutton").click(function () {
        startdatetime = $('.startdate').datetimepicker("getDate").toJSON();
        enddatetime = $('.enddate').datetimepicker("getDate").toJSON();

        $.ajax({
            type: "POST",
            url: getUrl('admin/reports/GetSalesReportPartial'),
            contentType: 'application/json',
            dataType: "html",
            data: JSON.stringify({ "StartDate": startdatetime, "EndDate": enddatetime }),
            success: function (data) {
                $("#SalesData").html("");
                $("#SalesData").html(data);
                $("#SalesDatapopup").html("");
                $("#SalesDatapopup").html(data);
            }
        });
    });


    $("#showResellerreportbutton").click(function () {
        startdatetime = $('.startdate').datetimepicker("getDate").toJSON();
        enddatetime = $('.enddate').datetimepicker("getDate").toJSON();

        $.ajax({
            type: "POST",
            url: getUrl('Reseller/report/GetResellerSalesReportPartial'),
            contentType: 'application/json',
            dataType: "html",
            data: JSON.stringify({ "StartDate": startdatetime, "EndDate": enddatetime }),
            success: function (data) {
                $("#SalesData").html("");
                $("#SalesData").html(data);
                $("#SalesDatapopup").html("");
                $("#SalesDatapopup").html(data);
            }
        });
    });



});

function getUrl(url) {
    var full_url = window.location.protocol + "//" + window.location.host + "/";
    return full_url + url;
}

function initGeolocation(callback) {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(callback);
    }
}


function showLoader(isShow, selector) {
    if (selector === undefined || selector === null) {
        selector = 'body';
    }

    var $selector = jQuery(selector);
    $selector.each(function () {
        var $this = jQuery(this);
        if (isShow) {
            var $overlay = $this.data('loaderOverlay');
            if (!$this.data('loaderOverlay')) {
                $overlay = jQuery('<div class="loaderOverlay"><div class="loaderWrapper"><div class="loaderAnimation"></div></div></div>').appendTo('body');
                $this.data('loaderOverlay', $overlay);
            }
            var $animation = $overlay.find('.loaderAnimation');

            // Width & Height
            var width = $this.outerWidth();
            var height = $this.outerHeight();
            var offset = $this.offset();
            $overlay.css({
                'width': width + 'px',
                'height': height + 'px',
                'top': offset.top + 'px',
                'left': offset.left + 'px'
            });

            var length = width < height ? width : height;
            var animateLength = length * 0.3;

            $animation.css({
                'width': animateLength + 'px',
                'height': animateLength + 'px',
                'top': ((height - animateLength) / 2) + 'px',
                'left': ((width - animateLength) / 2) + 'px'
            });

            $overlay.fadeIn();
        } else {
            var $overlay = $this.data('loaderOverlay');
            if ($overlay) {
                $overlay.fadeOut().animate({ 'dummy': '1' }, 100).remove();
                $this.removeData('loaderOverlay');
            }
        }
    });
}

function showMessageModal(title, body) {
    $('#message-modal .modal-title').text(title);
    $('#message-modal .modal-body').text(body);
    $('#message-modal').modal('show');
}