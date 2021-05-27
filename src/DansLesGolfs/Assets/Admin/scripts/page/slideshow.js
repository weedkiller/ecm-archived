jQuery(document).ready(function ($) {
    // Init uploadifive elements.
    $('#slideshow-file').uploadifive({
        formData: { id: window.itemId },
        multi: true,
        auto: true,
        buttonText: "Add Images...",
        buttonClass: "btn btn-default",
        fileTypeExts: '*.gif; *.jpg; *.png',
        fileSizeLimit: '4MB',
        uploadScript: getUrl('Admin/Slideshow/AddImages'),
        swf: getUrl('Assets/Libraries/uploadify/uploadify.swf'),
        onUploadComplete: function (file, data) {
            var formData = data.split(',');
            jQuery('#images-list').append(getSlideImageHTML(formData[0], formData[1], ''));
            addLastSlideImageEvent();
        }
    });

    // Make sortable.
    $('#images-list').sortable({
        update: function (e, ui) {
            saveAllItemImages();
        }
    });

    loadSlideImages();

    // Register events.
    $('#save-slideshow-button').click(onSaveSlideButtonIsClicked);
});

function loadSlideImages() {
    jQuery.ajax({
        url: getUrl('Admin/Slideshow/LoadSlideImages'),
        dataType: 'json',
        type: 'POST',
        success: function (result) {
            if (result.isSuccess) {
                addSlideImages(result.images);
            }
        }
    });
}

function addSlideImages(images) {
    var $imagesList = jQuery('#images-list');
    $imagesList.html('');
    for (var i in images) {
        $imagesList.append(getSlideImageHTML(images[i].ImageName, images[i].ImageUrl, images[i].Description, images[i].LinkUrl));
        addLastSlideImageEvent();
    }
}

function getSlideImageHTML(imageName, imageUrl, description, linkUrl) {
    var html = '<li data-image-name="' + imageName + '" class="slide-image">';
    html += '<div class="row">';
    html += '<div class="col-md-7"><img src="' + imageUrl + '" /></div>';
    html += '<div class="col-md-4">';
    html += '<input class="image-link form-control" placeholder="Link URL" value="' + linkUrl + '" />';
    html += '<textarea class="image-description form-control" placeholder="Description">' + description + '</textarea>';
    html += '</div>';
    html += '<div class="col-md-1">';
    html += '<a href="javascript:void(0);" class="delete">Delete</a>';
    html += '</div>';
    html += '</div>';
    html += '</li>';
    return html;
}

function addLastSlideImageEvent() {
    jQuery('.slide-image:last-child .delete').click(function (e) {
        jQuery(this).parents('.slide-image').remove();
    });
}

function saveAllSlideImages(callback) {
    var imageNames = new Array();
    var descriptions = new Array();
    var linkUrls = new Array();
    jQuery('.slide-image').each(function (i) {
        imageNames.push(jQuery(this).data('image-name'));
        descriptions.push(jQuery(this).find('.image-description').val());
        linkUrls.push(jQuery(this).find('.image-link').val());
    });
    jQuery.ajax({
        url: getUrl('Admin/Slideshow/AjaxSaveAllSlideImages'),
        data: { imageNames: imageNames, descriptions: descriptions, linkUrls: linkUrls },
        dataType: 'json',
        type: 'post',
        traditional: true,
        success: function (result) {
            if (result.isSuccess) {
                // Do nothing.
            }
            callback(result);
        }
    });
}

function onSaveSlideButtonIsClicked(e) {
    saveAllSlideImages(function (result) {
        jQuery('#slide-save-success-dialog').modal('show');
    });
}