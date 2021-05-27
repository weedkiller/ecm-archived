using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Areas.Reseller.Controllers
{
    public class InterestController : BaseResellerCRUDController
    {
        #region Constructor
        public InterestController()
        {
            ObjectName = "Interest";
            TitleName = Resources.Resources.UserInteresting;
            PrimaryKey = "InterestId";

            // Define Column Names.
            ColumnNames.Add("InterestName", Resources.Resources.InterestName);
            ColumnNames.Add("CategoryName", Resources.Resources.Category);
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<Interest> models = DataAccess.GetAllInterests(param);
            return models;
        }

        protected override void DoPrepareForm(int? id = null)
        {
            List<ItemCategory> itemCategories = DataAccess.GetAllItemCategories();
            ViewBag.ItemCategories = ListToDropDownList<ItemCategory>(itemCategories, "CategoryId", "CategoryName");
        }

        protected override object DoPrepareNew()
        {
            return new Interest();
        }

        protected override object DoPrepareEdit(long id)
        {
            Interest model = DataAccess.GetInterest(id);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            Interest model = null;
            int id = DataManager.ToInt(Request.Form["id"]);
            if (id > 0)
            {
                model = DataAccess.GetInterest(id);
                if (model == null)
                {
                    model = new Interest();
                }
            }
            else
            {
                model = new Interest();
            }
            model.InterestId = id;
            model.CategoryId = DataManager.ToInt(Request.Form["CategoryId"]);

            if (id > 0)
            {
                result = DataAccess.UpdateInterest(model);
            }
            else
            {
                model.Active = true;

                result = DataAccess.AddInterest(model);
                model.InterestId = result;
            }
            ViewBag.id = result > -1 ? model.InterestId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            string[] interestNames = Request.Form.GetValues("InterestName");
            string[] interestDescs = Request.Form.GetValues("InterestDesc");
            InterestLang InterestLang = null;

            for (int i = 0, j = interestNames.Length; i < j; i++)
            {
                InterestLang = new InterestLang();
                InterestLang.InterestId = id;
                InterestLang.LangId = i + 1;
                InterestLang.InterestName = interestNames[i].Trim();
                InterestLang.InterestDesc = interestDescs[i].Trim();

                DataAccess.SaveInterestLang(InterestLang);
            }
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteInterest(id) > 0;
        }
        #endregion
    }
}
