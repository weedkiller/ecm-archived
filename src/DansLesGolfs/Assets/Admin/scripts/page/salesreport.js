


var oOrderTable = null;
var saleOrder = new SaleReport();

$(function () {

    // Setup DatePicker
    $('.datepicker').datepicker({
        dateFormat: 'dd/mm/yy',
        autoSize: true
    });

    $('.view-report').click(function () {
        //startdatetime = $('.startdate').datetimepicker("getDate").toJSON();
        //enddatetime = $('.enddate').datetimepicker("getDate").toJSON();
        //console.log(startdatetime);
        //console.log(enddatetime);
        var startdatetime = $('#dtp_input1').val();
        var enddatetime = $('#dtp_input2').val();
        var status = $('#Status').val();
        var url = GetUrl('Admin/Reports/ViewReport?startdate=' + startdatetime + '&&enddate=' + enddatetime + '&&status=' + status);

        var win = window.open(url, '_blank'); 
    });
    $('#myModal').bind('hidden.bs.modal', function () {
        $("html").css("margin-right", "0px");
    });
    $('#myModal').bind('show.bs.modal', function () {
        $("html").css("margin-right", "-15px");
    });

    oOrderTable = $('#saleorder').dataTable({
        "pagingType": "bootstrap",
        "order": [[1, "desc"]],
        "scrollY": "450px",
        "bProcessing": true,
        "bServerSide": true,
        "bJQueryUI": false, 
        "sAjaxSource": GetUrl('Admin/Reports/DoLoadDataJSON'),
        
        "fnServerData": function (sSource, aoData, fnCallback) {
            //startdatetime = $('.startdate').datetimepicker("getDate").toJSON();
            //enddatetime = $('.enddate').datetimepicker("getDate").toJSON();

            var startdatetime = $('#dtp_input1').val();
            var enddatetime = $('#dtp_input2').val();
            aoData.push({ "name": "startDate", "value": startdatetime });
            aoData.push({ "name": "endDate", "value": enddatetime });
            aoData.push({ "name": "Status", "value": $("#Status").val() });

            $.getJSON(sSource, aoData, function (json) {
                fnCallback(json)
            });
        },
        "columnDefs": [{
            "targets": -1,
            "searchable": false, 
            "render": function (data, type, row) {
                console.log('render');
                console.log(data);
                console.log(type);
                console.log(row); 
                var html = '';
                if (row[5] == 'success') {
                    html = '<i data-id="' + row[5] + '" title="paid" class="bg-active payment-status"></i>';
                  //  html = '<p>paid</p>';
                } else {
                    html = '<i data-id="' + row[5] + '" title="unpaid" class="bg-nonactive  payment-status"></i>';
                  //  html = '<p>unpaid</p>';
                }

                return html;
            },
            //"defaultContent": "<button>Click!</button>"
        }]

    });  
    //$.datepicker.regional[""].dateFormat = 'dd/mm/yy';
    //$.datepicker.setDefaults($.datepicker.regional['']);

    $("#refresh").click(function (e) {
        oOrderTable.fnDraw();
    });

    $("#Status").change(function (e) {
        oOrderTable.fnDraw(); 
    });
     
});


$('#saleorder tbody').on('click', '.payment-status', function () {
    var id = $(this).attr('data-id'); 
    var IsPaySuccess = true;

    if ($(this).hasClass('bg-active')) {
        IsPaySuccess = false;
    }
     
    saleOrder.changeStatus(id, IsPaySuccess, function (res) {
        if (res.isSuccess) {
            if (IsPaySuccess) {
                $('.payment-status[data-id="' + res.OrderId + '"]').removeClass('bg-nonactive');
                $('.payment-status[data-id="' + res.OrderId + '"]').addClass('bg-active'); 
            } else {
                $('.payment-status[data-id="' + res.OrderId + '"]').removeClass('bg-active');
                $('.payment-status[data-id="' + res.OrderId + '"]').addClass('bg-nonactive');
            }
        }
    });
   // console.log("status is: " + data['PaymentStatus']); 
});


function showDetails() {
    //so something funky with the data
}


function SaleReport() {
    t = this;
    t.Data = new Array();
    t.changeStatus = function (id, isStatus,callback) {
        t = this;
        var data = {
            OrderId : id,
            IsSuccess: isStatus
        }
        $.post('/admin/reports/DoUpdateOrderPaymentStatus', data, function (res) {
            console.log(res);
            if (callback != null && callback != undefined) {
                callback(res);
            }
        });
    }
    t.renderStatus = function () {
        t = this;

    }
}