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

namespace DansLesGolfs.Areas.Admin.Controllers
{
    public class UserMessageAdminController : BaseAdminCRUDController
    {
        #region Constructor
        public UserMessageAdminController()
        {
            //ViewBag.ObjectName = Resources.Resources.Reports;
            //ViewBag.ClassName = "Promotion";

            ObjectName = "UserMessage";
            TitleName = "UserMessage";
            PrimaryKey = "MessageId";

            // Define Column Names.
            ColumnNames.Add("MessageId", "MessageId");
            ColumnNames.Add("Subject", "Subject");
            ColumnNames.Add("FromName", "From User");
            ColumnNames.Add("ToName", "To User");
            
        }
        #endregion


        public ActionResult UserMessageList()
        {
            ViewBag.ColumnNames = ColumnNames;

            return View("Index");
        }

        public ActionResult AddUserMessageAdmin()
        {
            UserMessageAdminViewModel vm = new UserMessageAdminViewModel();

            vm.UserList = DataAccess.GetInterestList();
            vm.UserList.Insert(0,new BLL.User
            {
                UserId = -1,
                FirstName = "------All-----" 
            });

            return View("AddUserMessageAdmin", vm);
        }

        [HttpPost]
        public ActionResult SaveUserMessageAdmin(UserMessageAdminViewModel vm,HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string fileExtension = string.Empty;
                string baseFileName = string.Empty;
                string fileName = string.Empty;
                var RandomfileName = string.Empty;

                if (file != null)
                {
                     fileExtension = Path.GetExtension(file.FileName);
                     baseFileName = Path.GetFileNameWithoutExtension(file.FileName);
                     fileName = Path.GetFileName(file.FileName);
                     RandomfileName = Guid.NewGuid().ToString();

                    try
                    {

                        string uploadDir = System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];

                        Directory.CreateDirectory(Server.MapPath("~/" + uploadDir + "/UserMessage/"));
                        var path = Path.Combine(Server.MapPath("~/" + uploadDir + "/UserMessage/"), RandomfileName + fileExtension);
                        file.SaveAs(path);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                DataAccess.AddUserMessageAdmin(vm.Subject, Auth.User.UserId, vm.ToUserId, vm.Body, baseFileName, fileExtension, RandomfileName);
                return RedirectToAction("AddUserMessageAdmin", "UserMessageAdmin");
            }

            vm.UserList = DataAccess.GetInterestList();
            vm.UserList.Insert(0, new BLL.User
            {
                UserId = -1,
                FirstName = "------All-----"
            });

            return View("AddUserMessageAdmin", vm);
        }

        protected override IEnumerable<object> DoLoadDataJSON(jQueryDataTableParamModel param)
        {
            List<UserMessageModel> promotion = DataAccess.GetUserMessageList(param.start, param.length, null, null, param.search,null,null);
            return promotion;
        }

        [HttpGet]
        public JsonResult LoadDataPromotionJSON(jQueryDataTableParamModel param)
        {
            try
            {

                IEnumerable<object> result = DoLoadDataJSON(param);
                var list = result.Cast<UserMessageModel>().ToArray();
                if (result != null)
                {
                    object[] resultArray = ConvertIEnumerableToArray(result);
                    return Json(new
                    {
                        sEcho = param.draw,
                        iTotalRecords = list[0].ToltalItemCount,
                        iTotalDisplayRecords = list[0].ToltalItemCount,
                        aaData = resultArray
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        sEcho = param.draw,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = new object[0]
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    sEcho = param.draw,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new object[0]
                }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override bool DoDelete(int messageId)
        {
            return DataAccess.DeleteUserMessageAdmin(messageId) > 0;

        }
       

    }

}
