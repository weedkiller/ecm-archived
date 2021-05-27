jQuery(document).ready(function ($) {
    $('#SiteId').change(function (e) {
        var siteId = this.value;
        $('#customer-group-list li input[type=checkbox]').prop('checked', false);
        $.uniform.update();
        $('#customer-group-list li').hide();
        $('#customer-group-list li[data-site-id=' + siteId + ']').show();
        console.log($('#customer-group-list li:visible').length);
        if ($('#customer-group-list li:visible').length == 0) {
            $('#row-customer-group').hide();
        } else {
            $('#row-customer-group').show();
        }
    });

    jQuery('#step-1').show();
    jQuery('#step-2, #step-3').hide();

    // Init uploadifive elements.
    $('#customer-import-file').uploadifive({
        multi: false,
        auto: true,
        buttonText: "Add File...",
        buttonClass: "btn btn-primary",
        fileTypeExts: '*.csv; *.xls; *.xlsx',
        uploadScript: getUrl('Customer/AjaxImportFile'),
        swf: getUrl('Assets/Libraries/uploadify/uploadify.swf'),
        onUploadComplete: function (file, data) {
            var columns = data.split(',');
            showMatchingColumns(columns);
        }
    });

    $('#reset-button').click(resetImportingProcess);
    $('#start-import-button').click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        var sendingData = {

        };

        // Create data object.
        $('#column-matching-table tbody select').each(function () {
            if (this.id.indexOf('file-') == 0) {
                sendingData[this.id.replace('file-', '')] = this.value;
            } else {
                sendingData[this.id] = this.value;
            }
        });

        sendingData["customerGroupIds"] = new Array();
        $('#column-matching-table tbody #customer-group-list input[type=checkbox]:checked').each(function (i) {
            sendingData["customerGroupIds"].push(this.value);
        });

        resetProgress();
        // Sending request to start import.
        $.ajax({
            url: getUrl('Customer/AjaxStartImport'),
            type: 'post',
            dataType: 'json',
            data: sendingData,
            traditional: true,
            success: function (result) {
                if (result.isSuccess) {
                } else {
                    showMessageModal("Error", result.message);
                }
            },
            error: function (xhr, msg) {
                console.error(xhr);
            }
        });
        jQuery('#step-1, #step-2').hide();
        jQuery('#step-3').fadeIn();
        jQuery('#import-progress-message').html('');
        window.checkImportProgressInterval = setInterval(checkImportProgress, 2000);
    });
});

function resetImportingProcess() {
    jQuery('#step-1, #step-2, #step-3, #action-buttons').hide();
    jQuery('#step-1').fadeIn();

    jQuery('#column-matching-table tr td select').attr('disabled', 'disabled');
}

function showMatchingColumns(columns) {
    if (!columns)
        return;

    jQuery('#step-1, #step-3').hide();
    jQuery('#step-2, #action-buttons').fadeIn();
    generateFileColumns(columns);
}

function generateFileColumns(columns) {
    var html = '<option value="none">' + window.localeTexts.none + '</option>';
    for (var i in columns) {
        html += '<option value="' + columns[i] + '">' + columns[i] + '</option>';
    }
    jQuery('#column-matching-table tr td select[id*=file-]').removeAttr('disabled').html(html);
    jQuery('#column-matching-table tr td select').find('option:eq(0)').attr('selected', 'selected');
}

function checkImportProgress() {
    if(jQuery('#import-progress .progress-bar').attr('aria-valuenow') == '100')
        return;

    jQuery.ajax({
        url: getUrl('Customer/AjaxGetImportProgress'),
        type: 'post',
        dataType: 'json',
        success: function (result) {
            if (result.isSuccess) {
                if (result.percent <= 100) {
                    jQuery('#progress-percent, #progress-message span').text(result.percent);
                    jQuery('#import-progress .progress-bar').attr('aria-valuenow', result.percent).css('width', result.percent + '%');
                    if (result.isCompleted) {
                        clearImportProgressInterval();
                        showErrorMessages(result.messages, result.messageTypes, result.errorText, result.warningText, result.infoText);

                        // To make sure we will not get more than 100%
                        jQuery('#import-progress .progress-bar').attr('aria-valuenow', 100).css('width', '100%');
                    }
                }
            } else {
                console.error(result.message);
                clearImportProgressInterval();
            }
        },
        error: function (xhr, msg) {
            console.error(xhr);
            clearImportProgressInterval();
        }
    });
}

function resetProgress() {
    jQuery('#progress-percent, #progress-message span').text("0");
    jQuery('#import-progress .progress-bar').attr('aria-valuenow', "0").css('width', '0%');
    clearInterval(window.checkImportProgressInterval);
}

function clearImportProgressInterval() {
    clearInterval(window.checkImportProgressInterval);
}

function showErrorMessages(messages, messageTypes, dangerText, warningText, infoText) {
    var html = '<ul>';
    for (var i in messages) {
        html += '<li>';
        if (messageTypes[i] == 'error') {
            html += '<span class="label label-danger">' + dangerText + '</span> ';
        } else if (messageTypes[i] == 'warning') {
            html += '<span class="label label-warning">' + warningText + '</span> ';
        } else {
            html += '<span class="label label-info">' + infoText + '</span> ';
        }
        html += messages[i];
        html += '</li>';
    }
    html += '</ul>';
    jQuery('#import-progress-message').html(html);

}