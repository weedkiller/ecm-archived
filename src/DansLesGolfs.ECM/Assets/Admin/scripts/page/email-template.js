window.newAttachmentId = 0;
window.editorContainer = '#gjs';
var editorOptions = {
    assetUrl: window.assetUrl,
    uploadUrl: window.uploadUrl,
    deleteUrl: window.deleteUrl
};

/**
* On document ready.
*/
jQuery(document).ready(function ($) {
    $('.info-form').validate();

    window.editor = initEditor(window.editorContainer, editorOptions);

    $('#attachments').uploadifive({
        multi: true,
        auto: true,
        buttonText: "Add files...",
        buttonClass: "btn btn-default",
        fileTypeExts: '*.html; *.htm; *.doc; *.docx; *.snp; *.rtf; *.odt; *.txt; *.pdf; *.ppt; *.swf; *.pps; *.pub; *.ics; *.vcs; *.bmp; *.jpg; *.jpeg; *.tif; *.tiff; *.eps; *.png; *.ico; *.gif; *.zip; *.rar; *.xls; *.xlsx; *.csv; *.dbf; *.wav; *.mp3; *.m4a; *.raw',
        fileSizeLimit: '10MB',
        uploadScript: getUrl('EmailTemplate/UploadAttachments'),
        swf: getUrl('Assets/Libraries/uploadify/uploadify.swf'),
        onUploadComplete: function (file, data) {
            var formData = data.split('|');
            addAttachmentList(formData[0], formData[1], formData[2], formData[3]);
        }
    });
    $('.add-variable-button').click(onAddVariableButtonIsClicked);
    $('.info-form').submit(onFormSubmited);
    $('#preview-button').click(onPreviewButtonIsClicked);
    $('#attachments-list li .delete-link').click(onAttachmentDeleteIsClicked);
});

function onAddVariableButtonIsClicked(e) {
    var $parent = $(this).parents('.variable-panel');
    var $select = $parent.find('select');
    insertValueToEditor($select.val());
}

function updateEmailTemplateForm() {
    var html = removeSelfClosingTags(window.editor.getHtml());
    return html;
}

function setHtmlDetailString(html) {
    var input = $('input#HtmlDetailString');
    input.val(html);
}

function onFormSubmited(e) {
    showLoader(true);
    var html = updateEmailTemplateForm();
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
    setHtmlDetailString(html);
    showLoader(false);
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

function addAttachmentList(fileName, baseName, fileExtension, filePath) {
    var newId = window.newAttachmentId--;
    var html = '<li id="attachment-' + newId + '" class="attachment-file" data-id="' + newId + '">' +
        '<span><i class="fa fa-file-text"></i> ' + fileName + ' <a class="delete-link" href="javascript:void(0)"><span class="fa fa-trash-o"></span></a></span>' +
        '<input type="hidden" name="AttachmentIds" value="' + newId + '" />' +
        '<input type="hidden" name="FileNames" value="' + fileName + '" />' +
        '<input type="hidden" name="BaseNames" value="' + baseName + '" />' +
        '<input type="hidden" name="FileExtensions" value="' + fileExtension + '" />' +
        '<input type="hidden" name="FilePaths" value="' + filePath + '" />' +
        '</li>';
    $('#attachments-list').append(html);
    $('#attachments-list #attachment-' + newId + ' .delete-link').click(onAttachmentDeleteIsClicked);
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