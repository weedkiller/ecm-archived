window.deletedMenuItems = new Array();

jQuery(document).ready(function ($) {
    $('#menu-list').nestable();

    $('.panel .add-button').click(onAddButtonIsClicked);

    $('#save-menu-button').click(onSaveMenuButtonIsClicked);

    $('#MenuId').change(function () { loadMenuItems(); });

    $('#menu-editing-dialog #save-change-menu-button').click(onSaveChangeMenuButtonIsClicked);

    loadMenuItems();
});

/**
* Add new menu item.
* @param {String} type  Type of menu item.
* @param {String} title Title of menu item.
* @param {String} value Value of menu item.
*/
function addMenu(menuId, type, title, value) {
    if (title == '')
        title = 'Untitled';

    $.ajax({
        url: getUrl('Admin/Menu/AjaxGetPreviewMenuItem'),
        data: { menuId: $('#MenuId').val(), type: type, title: title, value: value },
        dataType: 'json',
        type: 'post',
        beforeSend: function () {
            showLoader(true, '#page-menu-manager');
        },
        success: function (result) {
            showLoader(false, '#page-menu-manager');
            if (result.isSuccess) {
                $(result.html).appendTo('#menu-list > ol');
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#page-menu-manager');
        }
    });
}

function getMenuData() {
    var menu = {
        ids: new Array(),
        types: new Array(),
        titles: new Array(),
        values: new Array(),
        parents: new Array()
    };
    menu = getAllMenuItem(menu, 0, $('#menu-list > ol'));
    return menu;
}

function getAllMenuItem(menu, parent, $ele) {
    $ele.children('li').each(function () {
        var $this = $(this);
        var menuItemId = $this.data('id')
        if (menuItemId == 0) {
            menuItemId = getNewId();
            $this.data('id', menuItemId);
        }
        menu.ids.push(menuItemId);
        menu.types.push($this.data('type'));
        menu.titles.push($this.data('title'));
        menu.values.push($this.data('value'));
        menu.parents.push(parent);

        if ($this.find('ol').length > 0) {
            menu = getAllMenuItem(menu, $this.data('id'), $this.find('ol'));
        }
    })
    return menu;
}

function getNewId() {
    var newId = 0;
    $('#menu-list .menu-item').each(function () {
        if ($(this).data('id') < newId)
            newId = $(this).data('id');
    });
    return newId - 1;
}

function onAddButtonIsClicked(e) {
    e.preventDefault();
    e.stopPropagation();

    var $parent = $(this).parents('.panel');
    var menuId = $parent.data('menu-id');
    if ($parent.attr('id') == 'link-menu-creator') {
        var title = $parent.find('input[name=title]');
        var url = $parent.find('input[name=url]');
        addMenu(menuId, 'link', title.val().trim(), url.val().trim());

        // Clear textboxes.
        title.val('');
        url.val('');
    } else if ($parent.attr('id') == 'itemtype-menu-creator') {
        var checked = $parent.find('input:checked');
        if (checked.length == 0)
            return;

        checked.each(function () {
            var c = $(this);
            addMenu(menuId, 'itemtype', c.data('title'), c.val());
        });

        // Clear checkboxes.
        checked.prop('checked', false);
        $.uniform.update();
    } else if ($parent.attr('id') == 'category-menu-creator') {
        var checked = $parent.find('input:checked');
        if (checked.length == 0)
            return;

        checked.each(function () {
            var c = $(this);
            addMenu(menuId, 'itemtype', c.data('title'), c.val());
        });

        // Clear checkboxes.
        checked.prop('checked', false);
        $.uniform.update();
    }
}

function onSaveMenuButtonIsClicked(e) {
    e.preventDefault();
    e.stopPropagation();

    var menu = getMenuData();
    $.ajax({
        url: getUrl('Admin/Menu/AjaxSaveMenu'),
        data: { menuId: $('#MenuId').val(), ids: menu.ids, types: menu.types, titles: menu.titles, values: menu.values, parentIds: menu.parents, deletedMenuItems: window.deletedMenuItems },
        dataType: 'json',
        type: 'post',
        traditional: true,
        beforeSend: function () {
            showLoader(true, '#page-menu-manager');
        },
        success: function (result) {
            showLoader(false, '#page-menu-manager');
            if (result.isSuccess) {
                window.location.reload();
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#page-menu-manager');
        }
    });
}

function loadMenuItems() {
    $.ajax({
        url: getUrl('Admin/Menu/AjaxLoadMenuItems'),
        data: { menuId: $('#MenuId').val() },
        dataType: 'json',
        type: 'post',
        beforeSend: function () {
            showLoader(true, '#page-menu-manager');
        },
        success: function (result) {
            showLoader(false, '#page-menu-manager');
            if (result.isSuccess) {
                $('#menu-list').html('');
                var $elements = $(result.html).appendTo('#menu-list');
                $('.edit-button', $elements).click(onEditButtonIsClicked);
                $('.delete-button', $elements).click(onDeleteButtonIsClicked);
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#page-menu-manager');
        }
    });
}

function onEditButtonIsClicked(e) {
    e.preventDefault();
    e.stopPropagation();

    var $parent = $(this).parents('.menu-item');
    window.currentMenuItem = $parent;
    $('#menu-editing-dialog input[name=menu_title]').val($parent.data('title'));
    if ($parent.data('type') == 'link') {
        $('#menu-editing-dialog #url-control').show();
        $('#menu-editing-dialog input[name=menu_url]').val($parent.data('value'));
    } else {
        $('#menu-editing-dialog #url-control').hide();
    }

    $('#menu-editing-dialog').modal('show');
}

function onDeleteButtonIsClicked(e) {
    e.preventDefault();
    e.stopPropagation();

    if (!confirm("Are you sure for delete this item?"))
        return;

    var $parent = $(this).parents('.menu-item');
    var id = $parent.data('id');
    if(id && id > 0) {
        window.deletedMenuItems.push(id);
    }
    $parent.hide();
}

function onSaveChangeMenuButtonIsClicked(e) {
    if (!window.currentMenuItem)
        return;

    var $menuItem = window.currentMenuItem;
    var title = $('#menu-editing-dialog input[name=menu_title]').val();
    var url = $('#menu-editing-dialog input[name=menu_url]').val();
    $menuItem.data('title', title);
    if ($menuItem.data('type') == 'link') {
        $menuItem.data('value', url);
        saveMenuItem($menuItem.data('menu-id'), $menuItem.data('id'), $menuItem.data('type'), $menuItem.data('title'), $menuItem.data('value'))
    }
    $menuItem.data('title', title).find('.menu-text').text(title);
    $('#menu-editing-dialog input[name=menu_title]').val('');
    $('#menu-editing-dialog input[name=menu_url]').val('');
}

function saveMenuItem(menuId, id, type, title, value) {
    $.ajax({
        url: getUrl('Admin/Menu/AjaxSaveMenuItem'),
        data: { menuId: menuId, id: id, type: type, title: title, value: value },
        dataType: 'json',
        type: 'post',
        beforeSend: function () {
            showLoader(true, '#page-menu-manager');
        },
        success: function (result) {
            showLoader(false, '#page-menu-manager');
            if (result.isSuccess) {
            }
        },
        error: function (xhr, msg) {
            showLoader(false, '#page-menu-manager');
        }
    });
}