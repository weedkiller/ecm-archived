var editorOptions = {
    assetUrl: window.assetUrl,
    uploadUrl: window.uploadUrl,
    deleteUrl: window.deleteUrl
};
window.editorContainer = "#gjs";

jQuery(document).ready(function ($) {

    window.editor = initEditor(window.editorContainer, editorOptions);

    $('.add-variable-button').click(onAddVariableButtonIsClicked);
    $('.add-mirrorpage-button').click(onAddMirrorPageButtonIsClicked);
    $('.add-unsubscribe-button').click(onAddUnsubscribeButtonIsClicked);
    $('#form-send-email').submit(onFormSubmited);
    $('#load-template-button').click(onLoadTemplateButtonIsClicked);
    $('#preview-template-button').click(onPreviewTemplateButtonIsClicked);
    $('#preview-button').click(onPreviewButtonIsClicked);

    $('#attachments').uploadifive({
        multi: true,
        auto: true,
        buttonText: "Add files...",
        buttonClass: "btn btn-default",
        fileTypeExts: '*.html; *.htm; *.doc; *.docx; *.snp; *.rtf; *.odt; *.txt; *.pdf; *.ppt; *.swf; *.pps; *.pub; *.ics; *.vcs; *.bmp; *.jpg; *.jpeg; *.tif; *.tiff; *.eps; *.png; *.ico; *.gif; *.zip; *.rar; *.xls; *.xlsx; *.csv; *.dbf; *.wav; *.mp3; *.m4a; *.raw',
        fileSizeLimit: '10MB',
        uploadScript: getUrl('Emailing/UploadAttachments'),
        swf: getUrl('Assets/Libraries/uploadify/uploadify.swf'),
        onUploadComplete: function (file, data) {
            var formData = data.split('|');
            addAttachmentList(formData[0], formData[1], formData[2], formData[3], false);
        }
    });

    $('#attachments-list li .delete-link').click(onAttachmentDeleteIsClicked);
});

function onAddVariableButtonIsClicked(e) {
    var $parent = $(this).parents('.variable-panel');
    var $select = $parent.find('select');
    insertValueToEditor($select.val());
}

function onAddMirrorPageButtonIsClicked(e) {
    var text = prompt(window.askForMirrorLinkText, window.sampleMirrorLinkText);
    var link = '<span id="view"><a href="{!web}">' + text + '</a></span>';
    insertValueToEditor(link);
}

function onAddUnsubscribeButtonIsClicked(e) {
    var text = prompt(window.askForUnsubscribeLinkText, window.sampleUnsubscribeLinkText);
    var link = '<span id="unsub"><a href="{!unsubscribe_url}">' + text + '</a></span>';
    insertValueToEditor(link);
}

function onFormSubmited(e) {
    showLoader(true);
    var code = updateEmailTemplateForm();

    jQuery('#TemplateId').val(jQuery('#SelectTemplateId').val());

    //if ((html.indexOf('$$http://clicview.html$$') < 0 && html.indexOf('{!web}') < 0) || html.indexOf('id="view"') < 0) {
    //    alert(window.warningMirrorLinkText);
    //    e.preventDefault();
    //    showLoader(false);
    //    return;
    //} else if (html.indexOf('{!unsubscribe}') < 0 && html.indexOf('{!unsubscribe_url}') < 0 && html.indexOf('id="unsub"') < 0) {
    //    alert(window.warningUnsubscribeLink);
    //    e.preventDefault();
    //    showLoader(false);
    //    return;
    //}
    setHtmlDetailString(code);
    showLoader(false);
}

function updateEmailTemplateForm() {
    var mjml = removeSelfClosingTags(window.editor.getHtml());
    var code = window.editor.runCommand('mjml-get-code');
    return {
        mjml: mjml,
        html: code.html
    };
}

function setHtmlDetailString(code) {
    var mjml = $('input#MjmlDetailString');
    var html = $('input#HtmlDetailString');
    mjml.val(code.mjml);
    html.val(code.html);
}

function insertValueToEditor(value) {
    var $textarea = $('.editor');
    if ($textarea.length === 0)
        return;

    if ($textarea.hasClass('editor')) {
        tinymce.activeEditor.execCommand('mceInsertContent', false, value);
    } else {
        $textarea.html($textarea.html() + value);
    }
}

function onLoadTemplateButtonIsClicked() {
    if (!confirm("This template content and attachment files will overwrite current content.\nDo you want to continue?"))
        return;

    jQuery.ajax({
        url: getUrl('Emailing/GetTemplateContent'),
        dataType: 'json',
        type: 'post',
        data: { templateId: jQuery('#SelectTemplateId').val() },
        traditional: true,
        beforeSend: function () {
            showLoader(true, '.info-form');
        },
        complete: function () {
            showLoader(false, '.info-form');
        },
        success: function (result) {
            if (result.isSuccess) {
                jQuery('#gjs').html(result.html);
                window.editor = initEditor('#gjs', editorOptions);
                $('#attachments-list .attachment-file .delete-link').trigger('click');
                if (result.attachments && result.attachments.length > 0) {
                    for (var i in result.attachments) {
                        addAttachmentList(result.attachments[i].FileName, result.attachments[i].BaseName, result.attachments[i].FileExtension, result.attachments[i].FilePath, true);
                    }
                }
            } else {
                alert(result.message);
            }
        },
        error: function (xhr, msg) {
            alert(msg);
        }
    });
}

function onPreviewTemplateButtonIsClicked(e) {
    jQuery.ajax({
        url: getUrl('EmailTemplate/PreviewRaw/' + jQuery('#SelectTemplateId').val()),
        type: 'GET',
        success: function (html) {
            let previewWindow = window.open('', 'preview_window');
            previewWindow.document.write(html);
        }
    })
}

function onPreviewButtonIsClicked(e) {
    try {
        let code = window.editor.runCommand('mjml-get-code');
        let html = code.html;
        if (window.previewWindow) {
            window.previewWindow.close();
        }
        window.previewWindow = window.open('', 'preview_window');
        window.previewWindow.document.write(html);
        window.previewWindow.title = "Preview Window";
    } catch (ex) {
        alert(ex.message);
    }
}

function addAttachmentList(fileName, baseName, fileExtension, filePath, isCopy) {
    var newId = window.newAttachmentId--;
    var html = '<li id="attachment-' + newId + '" class="attachment-file" data-id="' + newId + '">' +
                    '<span><i class="fa fa-file-text"></i> ' + fileName + ' <a class="delete-link" href="javascript:void(0)"><span class="fa fa-trash-o"></span></a></span>' +
                    '<input type="hidden" name="AttachmentIds" value="' + newId + '" />' +
                    '<input type="hidden" name="FileNames" value="' + fileName + '" />' +
                    '<input type="hidden" name="BaseNames" value="' + baseName + '" />' +
                    '<input type="hidden" name="FileExtensions" value="' + fileExtension + '" />' +
                    '<input type="hidden" name="FilePaths" value="' + filePath + '" />' +
                    '<input type="hidden" name="IsCopy" value="' + (isCopy === true ? "true" : "false") + '" />' +
                '</li>';
    $('#attachments-list').append(html);
    $('#attachments-list #attachment-' + newId + ' .delete-link').click(onAttachmentDeleteIsClicked);
}

function IsExistsAttachmentFileName(fileName) {
    $('#attachments-list input[name=FileNames]').each(function () {
        if (this.value === fileName) {
            return true;
        }
    });
    return false;
}

function onAttachmentDeleteIsClicked() {
    var $parent = $(this).parents('li');
    var id = $parent.data('id');

    if (id <= 0)
        return;

    if ($('#deletedAttachmentIds').val().trim() !== '') {
        id = "," + id;
    }
    $('#deletedAttachmentIds').val($('#deletedAttachmentIds').val() + id);
    $parent.remove();
}