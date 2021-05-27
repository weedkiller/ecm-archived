using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class ItemTypeController : BaseAdminCRUDController
    {
        #region Constructor
        public ItemTypeController()
        {
            ObjectName = "ItemType";
            TitleName = "Item Type";
            PrimaryKey = "ItemTypeId";

            // Define Column Names.
            ColumnNames.Add("ItemTypeName", "Item Type Name");
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<ItemType> models = DataAccess.GetAllItemType(param);
            return models;
        }

        protected override object DoPrepareNew()
        {
            return new ItemType();
        }

        protected override object DoPrepareEdit(long id)
        {
            ItemType model = DataAccess.GetItemType(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            ItemType model = null;
            int id = DataManager.ToInt(Request.Form["id"]);
            if (id > 0)
            {
                model = DataAccess.GetItemType(id);
                if (model == null)
                {
                    model = new ItemType();
                }
            }
            else
            {
                model = new ItemType();
            }
            model.ItemTypeId = id;
            model.ItemTypeName = DataManager.ToString(Request.Form["ItemTypeName"]).Trim();

            if (id > -1)
            {
                result = DataAccess.UpdateItemType(model);
            }
            else
            {
                result = DataAccess.AddItemType(model);
                model.ItemTypeId = result;
            }
            ViewBag.id = result > -1 ? model.ItemTypeId : -1;
            return result > 0;
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteItemType(id) > 0;
        }
        #endregion
    }
}
