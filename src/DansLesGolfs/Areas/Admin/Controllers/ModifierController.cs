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
    public class ModifierController : BaseAdminCRUDController
    {
        #region Constructor
        public ModifierController()
        {
            ObjectName = "Modifier";
            TitleName = "Modifier";
            PrimaryKey = "ModifierId";

            // Define Column Names.
            ColumnNames.Add("ModifierName", "Modifier Name");
        }
        #endregion

        #region Overriden Methods
        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<Modifier> models = DataAccess.GetAllModifiers(param, this.CultureId);
            return models;
        }

        protected override void DoPrepareForm(int? id = null)
        {
            List<SelectListItem> controlTypes = new List<SelectListItem>();
            controlTypes.Add(new SelectListItem()
            {
                Text = "Options",
                Value = "0"
            });
            controlTypes.Add(new SelectListItem()
            {
                Text = "Text Field",
                Value = "1"
            });
            ViewBag.ControlTypes = controlTypes;
        }

        protected override object DoPrepareNew()
        {
            return new Modifier();
        }

        protected override object DoPrepareEdit(long id)
        {
            Modifier model = DataAccess.GetModifier(id, this.CultureId);
            return model;
        }

        protected override bool DoSave()
        {
            int result = -1;
            Modifier model = null;
            int id = DataManager.ToInt(Request.Form["id"]);
            if (id > 0)
            {
                model = DataAccess.GetModifier(id);
                if (model == null)
                {
                    model = new Modifier();
                }
            }
            else
            {
                model = new Modifier();
            }
            model.ControlType = DataManager.ToShort(Request.Form["ControlType"]);
            model.UpdateDate = DateTime.Now;
            if (Auth.User != null)
                model.UserId = Auth.User.UserId;

            if (model.ModifierId > 0)
            {
                result = DataAccess.UpdateModifier(model);
            }
            else
            {
                model.Active = true;
                result = DataAccess.AddModifier(model);
                model.ModifierId = result;
            }
            ViewBag.id = result > -1 ? model.ModifierId : -1;
            return result > 0;
        }

        protected override void DoSaveSuccess(int id)
        {
            string[] modifierNames = Request.Form.GetValues("ModifierName");
            string[] modifierDescs = Request.Form.GetValues("ModifierDesc");
            string[] choiceNames = Request.Form.GetValues("ChoiceName");

            ModifierLang modifierLang = null;

            for (int i = 0, j = modifierNames.Length; i < j; i++)
            {
                modifierLang = new ModifierLang();
                modifierLang.ModifierId = id;
                modifierLang.LangId = i + 1;
                modifierLang.ModifierName = modifierNames[i].Trim();
                modifierLang.ModifierDesc = modifierDescs[i].Trim();

                DataAccess.SaveModifierLang(modifierLang);
            } 

            //ModifierChoice choice = null;
            //DateTime now = DateTime.Now;
            //int userId = 0;
            //if (Auth.User != null)
            //    userId = Auth.User.UserId;

            //for (int i = 0, j = choiceNames.Length; i < j; i++)
            //{
            //    choice = new ModifierChoice();
            //    choice.ModifierId = id;
            //    choice.Active = true;
            //    choice.IsDefault = i == 0;
            //    choice.Price = 0;
            //    choice.UpdateDate = now;
            //    choice.UserId = userId;

            //    DataAccess.SaveModifierChoice(choice);
            //}
        }

        protected override bool DoDelete(int id)
        {
            return DataAccess.DeleteModifier(id) > 0;
        }
        #endregion
    }
}
