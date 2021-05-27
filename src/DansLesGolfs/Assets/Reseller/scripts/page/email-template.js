/**
* On document ready.
*/
jQuery(document).ready(function ($) {
    $('.info-form').validate();

    tinymce.init(window.tinymceOptions);

    // Init uploadifive elements.
    $('#email-attachment-file').uploadifive({
        formData: { id: window.itemId },
        multi: true,
        auto: true,
        buttonText: "Add Attachment File...",
        buttonClass: "btn btn-default",
        fileSizeLimit: '10MB',
        uploadScript: getUrl('DLGEmail/EmailTemplate/AddAttachmentFile'),
        swf: getUrl('Assets/Libraries/uploadify/uploadify.swf'),
        onUploadComplete: function (file, data) {
            var formData = data.split(',');
            addAttachmentFile(formData[0], formData[1]);
        }
    });
});

/************************* Attachment Files *************************/
/**
* Add Item Image
* @param int imageId        ID of Image.
* @param string url         URL
* @param string thumbUrl    URL of thumbnail.
* @param string filename    Name of file.
* @param string thumbname   Name of thumbnail file.
* @param string basename    Basename of file.
* @param string extension   File extension.
*/
function addAttachmentFile(imageId, url, thumbUrl, filename, thumbname, basename, extension) {
    var obj = {
        AttachmentFileId: imageId,
        ItemId: window.itemId,
        ImageName: filename,
        ListNo: $('#attachment-files-area .attachment-file').length,
        BaseName: basename,
        FileExtension: extension,
        ImageUrl: url,
        ThumbnailUrl: thumbUrl
    };
    var html = getAttachmentFileHTML(obj);
    $("#attachment-files-area").append(html);
    $('#attachment-files-area .attachment-file:last-child .delete').click(onAttachmentFileDelete);
}

/**
* Get Item Image HTML from Item Image Object.
* @param object obj         Item Image object.
* @return string            Item Image HTML elements.
*/
function getAttachmentFileHTML(obj) {
    var html = '<li class="product-image image-item" data-imageid="' + obj.AttachmentFileId + '" data-url="' + obj.ImageUrl + '" data-thumb-url="' + obj.ThumbnailUrl + '" data-filename="' + obj.ImageName + '" data-basename="' + obj.BaseName + '" data-extension="' + obj.FileExtension + '">';
    html += '<div class="tools-panel">';
    html += '<a href="javascript:void(0);" class="delete">Delete</a>';
    html += '</div>';
    html += '<div class="thumbnail" style="background-image:url(' + obj.ThumbnailUrl + ')">';
    html += '</div>';
    html += '</li>';
    return html;
}

function saveAllAttachmentFiles() {
    var imageIds = new Array();
    var fileNames = new Array();
    var baseNames = new Array();
    var fileExtensions = new Array();
    $('#attachment-files-area .attachment-file').each(function (i) {
        imageIds.push($(this).data('imageid'));
        fileNames.push($(this).data('filename'));
        baseNames.push($(this).data('basename'));
        fileExtensions.push($(this).data('extension'));
    });

    $.ajax({
        url: getUrl('DLGEmail/EmailTemplate/SaveAttachmentFiles'),
        type: 'post',
        dataType: 'json',
        traditional: true,
        data: { id: window.itemId, imageIds: imageIds, fileNames: fileNames, baseNames: baseNames, fileExtensions: fileExtensions },
        success: function (result) {
            if (result.isSuccess) {
                // Do nothing.
            } else {
                console.error(result.message);
            }
        }
    });
}

function loadAttachmentFiles() {
    $.ajax({
        url: getUrl('DLGEmail/EmailTemplate/LoadAttachmentFiles'),
        type: 'post',
        dataType: 'json',
        traditional: true,
        data: { id: window.itemId },
        success: function (result) {
            if (result.isSuccess) {
                var html = "";
                for (var i in result.AttachmentFiles) {
                    html += getAttachmentFileHTML(result.AttachmentFiles[i]);
                }
                $('#attachment-files-area').html(html);
                $('#attachment-files-area .attachment-file .delete').on('click', onAttachmentFileDelete);
            } else {
                console.error(result.message);
            }
        }
    });
}

function onAttachmentFileDelete(e) {
    if (confirm("Your file will be deleted permanently. Are you sure for deleting it?")) {
        var $parent = $(this).parents('.attachment-file');
        var AttachmentFileId = $parent.data('imageid');
        $.ajax({
            url: getUrl('DLGEmail/EmailTemplate/DeleteAttachmentFile'),
            type: 'post',
            dataType: 'json',
            data: { id: window.itemId, AttachmentFileId: AttachmentFileId },
            success: function (result) {
                if (result.isSuccess) {
                    $parent.remove();
                    saveAllAttachmentFiles();
                } else {
                    console.error(result.message);
                }
            }
        });
    }
}