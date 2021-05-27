using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class MenuController : BaseAdminController
    {
        public ActionResult Index()
        {
            ViewBag.Title = Resources.Resources.Menu;
            ViewBag.TitleName = Resources.Resources.Menu;
            ViewBag.ClassName = "menu-manager";
            Breadcrumbs.Add(Resources.Resources.Menu, "~/Admin/Menu");
            InitBreadcrumbs();

            ViewBag.ItemCategories = DataAccess.GetAllItemCategories();
            ViewBag.ItemType = DataAccess.GetAllItemType();
            List<Menu> menu = DataAccess.GetMenuDropDownList();
            ViewBag.MenuDropDownList = ListToDropDownList(menu, "MenuId", "MenuName", menu.Any() ? menu.First().MenuId : 0);

            return View();
        }
        private void RefreshCache()
        {
            System.Runtime.Caching.MemoryCache.Default.Remove("DLGMainMenu");
            System.Runtime.Caching.MemoryCache.Default.Remove("DLGFooterMenu");
        }

        [HttpPost]
        public JsonResult AjaxSaveMenu(int? menuId, int[] ids, int[] parentIds, string[] types, string[] titles, string[] values, string[] deletedMenuItems)
        {
            try
            {
                if (deletedMenuItems != null && deletedMenuItems.Any())
                {
                    string deletedIds = String.Join(",", deletedMenuItems);
                    DataAccess.DeleteMenuItemsByIds(deletedIds);
                }

                Menu menu = DataAccess.GetMenuByMenuId(menuId.Value);
                List<MenuItem> menuItems = new List<MenuItem>();
                for (int i = 0, n = ids.Length; i < n; i++)
                {
                    if (deletedMenuItems != null && deletedMenuItems.Contains(ids[i].ToString()))
                        continue;

                    menuItems.Add(new MenuItem()
                    {
                        MenuId = menuId.Value,
                        MenuItemId = ids[i],
                        MenuType = types[i],
                        MenuTitle = titles[i],
                        MenuValue = values[i],
                        ParentId = parentIds[i]
                    });
                }

                var parentItems = menuItems.Where(it => it.ParentId == 0);
                menu.MenuItems = ArrangeMenuItems(parentItems, menuItems);
                DataAccess.SaveMenu(menu);

                RefreshCache();
                return Json(new
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult AjaxLoadMenuItems(int? menuId)
        {
            try
            {
                if (!menuId.HasValue)
                    throw new Exception("Please specific Menu ID.");

                Menu menu = DataAccess.GetMenuByMenuId(menuId.Value);
                string html = GetMenuItemHtml(menu.MenuItems);

                return Json(new
                {
                    isSuccess = true,
                    html = html
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult AjaxSaveMenuItem(int? menuId, int? id, string type, string title, string value)
        {
            try
            {
                if (!menuId.HasValue)
                    throw new Exception("Please specific Menu ID.");

                if (!id.HasValue)
                    throw new Exception("Please specific Menu Item ID.");

                MenuItem item = null;
                if (id.Value <= 0)
                {
                    item = new MenuItem()
                    {
                        MenuId = menuId.Value,
                        MenuItemId = 0,
                        ParentId = 0,
                        MenuType = type,
                        MenuTitle = title,
                        MenuValue = value
                    };
                }
                else
                {
                    item = DataAccess.GetMenuItem(id.Value);
                    item.MenuType = type;
                    item.MenuTitle = title;
                    item.MenuValue = value;
                }

                item.MenuItemId = DataAccess.SaveMenuItem(item);

                if (item.MenuItemId <= 0)
                    throw new Exception("Can't save menu item, please try again.");

                RefreshCache();

                return Json(new
                {
                    isSuccess = true,
                    id = item.MenuItemId
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult AjaxGetPreviewMenuItem(int? menuId, string type, string title, string value)
        {
            try
            {
                List<MenuItem> list = new List<MenuItem>();
                list.Add(new MenuItem()
                {
                    MenuId = menuId.HasValue ? menuId.Value : 0,
                    MenuItemId = 0,
                    MenuType = type,
                    MenuTitle = title,
                    MenuValue = value
                });
                string html = GetMenuItemHtml(list, false);
                return Json(new
                {
                    isSuccess = true,
                    html = html
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = ex.Message
                });
            }
        }

        private string GetMenuItemHtml(List<MenuItem> items, bool hasWrapper = true)
        {
            string html = hasWrapper ? "<ol class=\"dd-list\">" : "";
            foreach (MenuItem i in items)
            {
                html += "<li class=\"menu-item dd-item\" data-id=\"" + i.MenuItemId + "\" data-menu-id=\"" + i.MenuId + "\" data-type=\"" + i.MenuType + "\" data-title=\"" + i.MenuTitle + "\" data-value=\"" + i.MenuValue + "\"><span class=\"menu-text dd-handle\">" + i.MenuTitle + "</span>";

                if (i.Children != null && i.Children.Any())
                {
                    html += GetMenuItemHtml(i.Children, true);
                }

                html += "<a href=\"javascript:void(0);\" class=\"edit-button\">" + Resources.Resources.Edit + "</a><a href=\"javascript:void(0);\" class=\"delete-button\">" + Resources.Resources.Delete + "</a></li>";
            }
            html += hasWrapper ? "</ol>" : "";
            return html;
        }

        private List<MenuItem> ArrangeMenuItems(IEnumerable<MenuItem> list, IEnumerable<MenuItem> wholeList)
        {
            List<MenuItem> newList = new List<MenuItem>();
            foreach (MenuItem item in list)
            {
                var children = wholeList.Where(it => it.ParentId == item.MenuItemId);
                if(children != null && children.Any())
                {
                    item.Children = ArrangeMenuItems(children, wholeList);
                }
                newList.Add(item);
            }
            return newList;
        }

    }
}
