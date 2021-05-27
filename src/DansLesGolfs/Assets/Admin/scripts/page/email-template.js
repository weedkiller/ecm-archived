/**
* On document ready.
*/
jQuery(document).ready(function ($) {
    $('.info-form').validate();

    tinymce.init(window.tinymceOptions);

    $('.add-variable-button').click(onAddVariableButtonIsClicked);
    $('#preview-button').click(onPreviewButtonIsClicked);
});

function onAddVariableButtonIsClicked(e) {
    var $parent = $(this).parents('.variable-panel');
    var $select = $parent.find('select');
    var $formGroup = $parent.parents('.form-group');
    var $textarea = $formGroup.find('textarea');
    if ($textarea.length == 0)
        return;

    if ($textarea.hasClass('editor')) {
        tinymce.activeEditor.execCommand('mceInsertContent', false, $select.val());
    } else {
        $textarea.html($textarea.html() + $select.val());
    }
}

function onPreviewButtonIsClicked(e) {
    //$('input[name=IsPreview]').val('1');
    //$('#submit').trigger('click');
    var editor_id = $('textarea[name=HtmlDetail]').attr('id');
    $('#' + editor_id).val(tinyMCE.get(editor_id).getContent());
    var data = $('.info-form').serialize();
    $.ajax({
        url: getUrl('Admin/EmailTemplate/PerformPreview'),
        data: data,
        dataType: 'json',
        type: 'post',
        success: function (result) {
            if (result.isSuccess) {
                window.open(getUrl('Admin/EmailTemplate/Preview/?previewId=' + result.previewId), 'email_template_preview');
            }
        }
    });
}