jQuery(document).ready(function ($) {
    initReportHub();
});

function initReportHub() {
    var reportHub = $.connection.reportHub;

    reportHub.client.getInfoMessage = function (msg) {
        jQuery('#netmessage-report-message').html('<span class="label label-info">Info</span> ' + msg);
    }

    reportHub.client.getErrorMessage = function (msg) {
        jQuery('#netmessage-report-message').html('<span class="label label-danger">Error</span> ' + msg);
    }

    $.connection.hub.start().done(function () {
        reportHub.server.getNetmessageReport(window.emailId);
    });
}