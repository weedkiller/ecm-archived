﻿/**
* On document ready.
*/
jQuery(document).ready(function ($) {
    $('.info-form').validate();

    tinymce.init(window.tinymceOptions);

    jQuery('#GolfBrandId').change(onGolfBrandIdChanged);

    $('input[name=SiteName]').blur(function (e) {
        if ($('#SiteSlug').val() == '' || window.siteId <= 0)
            generateSiteSlugBySiteName();
    });

    $('#SiteSlug').change(function (e) {
        if ($('#SiteSlug').val() == '') {
            generateSiteSlugBySiteName();
        } else {
            regenerateSiteSlug();
        }
    });

    jQuery('select[name=CountryId]').change(function () {
        var $this = jQuery(this);
        var $region = $('select[name=RegionId]');
        var countryId = $this.val();
        jQuery.ajax({
            url: getUrl('Common/AjaxGetRegionsByCountryId'),
            data: { countryId: countryId },
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (result.isResult) {
                    $region.html('');
                    for (var i in result.list) {
                        $region.append('<option value="' + (result.list[i].RegionId > 0 ? result.list[i].RegionId : "") + '">' + result.list[i].RegionName + '</option>');
                    }
                    if ($region.find('option').length > 0) {
                        $region.find('option:eq(0)').attr('selected', 'selected');
                        $region.trigger('change');
                    }
                    $.uniform.update();
                } else {
                    console.error(result.message);
                }
            },
            error: function (xhr, msg) {
                console.error(xhr);
            }
        });
    });

    jQuery('select[name=RegionId]').change(function () {
        var $this = jQuery(this);
        var $states = $('select[name=StateId]');
        var regionId = $this.val();
        var countryId = jQuery('select[name=CountryId]').val();
        jQuery.ajax({
            url: getUrl('Common/AjaxGetStatesByRegionId'),
            data: { countryId: countryId, regionId: regionId },
            dataType: 'json',
            type: 'post',
            success: function (result) {
                if (result.isResult) {
                    $states.html('');
                    for (var i in result.list) {
                        $states.append('<option value="' + (result.list[i].StateId > 0 ? result.list[i].StateId : "") + '">' + result.list[i].StateName + '</option>');
                    }
                    $.uniform.update();
                } else {
                    console.error(result.message);
                }
            },
            error: function (xhr, msg) {
                console.error(xhr);
            }
        });
    });

    // Make sortable area.
    $('#site-images-area').sortable();

    // Init uploadifive elements.
    $('#site-image-file').uploadifive({
        formData: { id: window.siteId },
        multi: true,
        auto: true,
        buttonText: "Add Images...",
        buttonClass: "btn btn-default",
        fileTypeExts: '*.gif; *.jpg; *.png',
        fileSizeLimit: '4MB',
        uploadScript: getUrl('Site/AddSiteImages'),
        swf: getUrl('Assets/Libraries/uploadify/uploadify.swf'),
        onUploadComplete: function (file, data) {
            var formData = data.split(',');
            addSiteImage(formData[0], formData[1], formData[2], formData[3], formData[4], formData[5], formData[6]);
        }
    });

    $('#IsUseGlobalSMTPSettings').change(onIsUseGlobalSMTPSettingsChanged).trigger('change'); 
    $('#IsUseGlobalNetmessageSettings').change(onIsUseGlobalNetmessageSettingsChanged).trigger('change');

    $('#SMTPUseVERP').change(function () {
        if (!$('#IsUseGlobalSMTPSettings').prop('checked'))
            $('#BouncedReturnEmail').prop('readonly', this.checked);
    }).trigger('change');

    loadSiteImages();
});

/************************* Site Info *************************/
function generateSiteSlug(text) {
    jQuery.ajax({
        url: getUrl('Site/GenerateSiteSlug'),
        data: { text: text, skipId: window.siteId },
        dataType: 'json',
        type: 'post',
        success: function (result) {
            if (result.isSuccess) {
                jQuery('#SiteSlug').val(result.slug);
            } else {
                console.error(result.message);
            }
        }
    });
}
function generateSiteSlugBySiteName() {
    generateSiteSlug(jQuery('input[name=SiteName]').val());
}
function regenerateSiteSlug() {
    generateSiteSlug(jQuery('#SiteSlug').val());
}
function onGolfBrandIdChanged(e) {
    console.log('yes');
    var $this = jQuery(this);
    var text = $this.find('option:selected').text().toLowerCase();
    var checkboxText = '';
    jQuery('input[name=SiteEmailTemplateIds]').prop('checked', false).each(function () {
        checkboxText = jQuery(this).data('name').toLowerCase();
        if (checkboxText.indexOf(text) > -1) {
            jQuery(this).prop('checked', true);
        }
        jQuery.uniform.update('input[name=SiteEmailTemplateIds]');
    });
}

/************************* Site Images *************************/
/**
* Add Site Image
* @param int imageId        ID of Image.
* @param string url         URL
* @param string thumbUrl    URL of thumbnail.
* @param string filename    Name of file.
* @param string thumbname   Name of thumbnail file.
* @param string basename    Basename of file.
* @param string extension   File extension.
*/
function addSiteImage(imageId, url, thumbUrl, filename, thumbname, basename, extension) {
    var obj = {
        SiteImageId: imageId,
        SiteId: window.siteId,
        ImageName: filename,
        ListNo: $('#site-images-area .site-image').length,
        BaseName: basename,
        FileExtension: extension,
        ImageUrl: url,
        ThumbnailUrl: thumbUrl
    };
    var html = getSiteImageHTML(obj);
    $("#site-images-area").append(html);
    $('#site-images-area .site-image:last-child .delete').click(onSiteImageDelete);
}

/**
* Get Site Image HTML from Site Image Object.
* @param object obj         Site Image object.
* @return string            Site Image HTML elements.
*/
function getSiteImageHTML(obj) {
    var html = '<li class="site-image image-item" data-imageid="' + obj.SiteImageId + '" data-url="' + obj.ImageUrl + '" data-thumb-url="' + obj.ThumbnailUrl + '" data-filename="' + obj.ImageName + '" data-basename="' + obj.BaseName + '" data-extension="' + obj.FileExtension + '">';
    html += '<div class="tools-panel">';
    html += '<a href="javascript:void(0);" class="delete">Delete</a>';
    html += '</div>';
    html += '<div class="thumbnail" style="background-image:url(' + obj.ThumbnailUrl + ')">';
    html += '<input type="hidden" name="SiteImageIds" value="' + obj.SiteImageId + '" />';
    html += '<input type="hidden" name="ImageNames" value="' + obj.ImageName + '" />';
    html += '<input type="hidden" name="BaseNames" value="' + obj.BaseName + '" />';
    html += '<input type="hidden" name="FileExtensions" value="' + obj.FileExtension + '" />';
    html += '<input type="hidden" name="IsDelete" value="0" />';
    html += '</div>';
    html += '</li>';
    return html;
}

function saveAllSiteImages() {
    var imageIds = new Array();
    var fileNames = new Array();
    var baseNames = new Array();
    var fileExtensions = new Array();
    $('#site-images-area .site-image').each(function (i) {
        imageIds.push($(this).data('imageid'));
        fileNames.push($(this).data('filename'));
        baseNames.push($(this).data('basename'));
        fileExtensions.push($(this).data('extension'));
    });

    $.ajax({
        url: getUrl('Site/SaveSiteImages'),
        type: 'post',
        dataType: 'json',
        traditional: true,
        data: { id: window.siteId, imageIds: imageIds, fileNames: fileNames, baseNames: baseNames, fileExtensions: fileExtensions },
        success: function (result) {
            if (result.isSuccess) {
                // Do nothing.
            } else {
                console.error(result.message);
            }
        }
    });
}

function loadSiteImages() {
    if (!window.siteId || window.siteId <= 0)
        return;
    console.log(window.siteId);
    $.ajax({
        url: getUrl('Site/LoadSiteImages'),
        type: 'post',
        dataType: 'json',
        traditional: true,
        data: { id: window.siteId },
        success: function (result) {
            if (result.isSuccess) {
                var html = "";
                for (var i in result.itemImages) {
                    html += getSiteImageHTML(result.itemImages[i]);
                }
                $('#site-images-area').html(html);
                $('#site-images-area .site-image .delete').on('click', onSiteImageDelete);
            } else {
                console.error(result.message);
            }
        }
    });
}

function onSiteImageDelete(e) {
    var $parent = $(this).parents('.site-image');
    $parent.find('input[name=IsDelete]').val("1");
    $parent.fadeOut();
}

function onIsUseGlobalSMTPSettingsChanged(e) {
    var readOnly = document.getElementById('IsUseGlobalSMTPSettings').checked;
    jQuery('#SMTPServer, #SMTPUsername, #SMTPPassword, #SMTPPort, #SMTPUseSSL, #DefaultSenderName, #DefaultSenderEmail, #SMTPUseVERP, #BouncedReturnEmail').prop('readonly', readOnly);
}

function onIsUseGlobalNetmessageSettingsChanged(e) {
    var readOnly = document.getElementById('IsUseGlobalNetmessageSettings').checked;
    jQuery('#NetmessageFTPUsername, #NetmessageFTPPassword, #NetmessageAccountName').prop('readonly', readOnly);
}