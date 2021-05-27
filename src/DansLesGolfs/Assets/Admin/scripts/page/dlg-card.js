/**
* On document ready.
*/
jQuery(document).ready(function ($) {

    console.log("Yeh I'm in dlg-card.js");

    $('.info-form').validate();

    // Init uploadifive elements.
    $('#card-image-file').uploadifive({
        formData: { id: $("#ItemId").val() },
        multi: true,
        auto: true,
        buttonText: "Add Images...",
        buttonClass: "btn btn-default",
        fileTypeExts: '*.gif; *.jpg; *.png',
        fileSizeLimit: '4MB',
        uploadScript: getUrl('Admin/DLGCard/AddCardImages'),
        swf: getUrl('Assets/Libraries/uploadify/uploadify.swf'),
        onUploadComplete: function (file, data) {
            //alert(data);
            var formData = data.split(',');
          
            addCardImage(formData[0], formData[1], formData[2], formData[3], formData[4], formData[5], formData[6]);
        }
    });
   
    loadCardImages();
});

/************************* DLG Card Images *************************/
/**
* Add DLG Card Image
* @param int imageId        ID of Image.
* @param string url         URL
* @param string thumbUrl    URL of thumbnail.
* @param string filename    Name of file.
* @param string thumbname   Name of thumbnail file.
* @param string basename    Basename of file.
* @param string extension   File extension.
*/
function addCardImage(imageId, url, thumbUrl, filename,thumbUrl, thumbname, extension) {
   
    var obj = {
        CardStyleId: imageId,
        //CardId: window.itemId,
        ImageName: filename,
        ListNo: $('#card-images-area .card-image').length,
        FileExtension: extension,
        ImageUrl: url,
        ThumbnailUrl: thumbUrl,
        BaseName: thumbname
    };
    var html = getCardImageHTML(obj);
    $("#card-images-area").append(html);
    $('#card-images-area .card-image:last-child .delete').click(onCardImageDelete);
}

/**
* Get Card Image HTML from Card Image Object.
* @param object obj         Card Image object.
* @return string            Card Image HTML elements.
*/
function getCardImageHTML(obj) {
   
    var html = '<li class="card-image image-item" data-imageid="' + obj.CardStyleId + '" data-url="' + obj.ImageUrl + '" data-thumb-url="' + obj.ThumbnailUrl + '" data-filename="' + obj.ImageName + '" data-basename="' + obj.BaseName + '" data-extension="' + obj.FileExtension + '">';
    html += '<div class="tools-panel">';
    html += '<a href="javascript:void(0);" class="delete">Delete</a>';
    html += '</div>';
    html += '<div class="thumbnail" style="background-image:url(' + obj.ThumbnailUrl + ')">';
    html += '<input type="hidden" name="CardImageIds" value="' + obj.CardStyleId + '" />';
    html += '<input type="hidden" name="ImageNames" value="' + obj.ImageName + '" />';
    html += '<input type="hidden" name="BaseNames" value="' + obj.BaseName + '" />';
    html += '<input type="hidden" name="FileExtensions" value="' + obj.FileExtension + '" />';
    html += '</div>';
    html += '</li>';
    return html;
}

function saveAllCardImages() {
    var imageIds = new Array();
    var fileNames = new Array();
    var baseNames = new Array();
    var fileExtensions = new Array();
    $('#card-images-area .card-image').each(function (i) {
        imageIds.push($(this).data('imageid'));
        fileNames.push($(this).data('filename'));
        baseNames.push($(this).data('basename'));
        fileExtensions.push($(this).data('extension'));
    });

    $.ajax({
        url: getUrl('Admin/DLGCard/SaveCardImages'),
        type: 'post',
        dataType: 'json',
        traditional: true,
        data: { id: window.itemId, imageIds: imageIds, fileNames: fileNames, baseNames: baseNames, fileExtensions: fileExtensions },
        success: function (result) {
            if (result.isSuccess) {
                // Do nothing.
            }
        }
    });
}

function loadCardImages() {

    $.ajax({
        url: getUrl('Admin/DLGCard/LoadCardImages'),
        type: 'post',
        dataType: 'json',
        traditional: true,
        data: { id: $("#ItemId").val() },
        success: function (result) {
            if (result.isSuccess) {
                var html = "";
                for (var i in result.itemImages) {
                    html += getCardImageHTML(result.itemImages[i]);
                }
                $('#card-images-area').html(html);
                $('#card-images-area .card-image .delete').on('click', onCardImageDelete);
            }
        }
    });
}

function onCardImageDelete(e) {
    if (confirm("Your image will be deleted permanently. Are you sure for deleting it?")) {
        var $parent = $(this).parents('.card-image');
        var itemImageId = $parent.data('imageid');
        $parent.remove();
        //$.ajax({
        //    url: getUrl('Admin/DLGCard/DeleteCardImage'),
        //    type: 'post',
        //    dataType: 'json',
        //    data: { id: $("#ItemId").val(), itemImageId: itemImageId },
        //    success: function (result) {
        //        if (result.isSuccess) {
        //            $parent.remove();
        //            //saveAllCardImages();
        //        }
        //    }
        //});
    }
}