using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using DansLesGolfs.Data;
using DansLesGolfs.Base;

namespace DansLesGolfs.Controllers
{
    public class MessageController : BaseFrontSecurityController
    {
        #region [HttpGet] Index
        public ActionResult Index()
        {
            int userId = ViewBag.LogonUserID;
            SqlDataAccess DataAccess = DataFactory.GetInstance();
            var Messages = DataAccess.GetAllMessageByUserId(userId);
            ViewBag.Messages = Messages;
            return View();
        }
        #endregion

        #region [HttpPost] Index
        [HttpPost]
        public ActionResult Index(FormCollection Form)
        {
            return Content("");
        }
        #endregion

        #region [HttpGet] Conversation
        public ActionResult Conversation(string name)
        {
            return View();
        }
        #endregion

        #region [HttpPost] Conversation
        [HttpPost]
        public ActionResult Conversation(FormCollection Form)
        {
            return Content("");
        }
        #endregion


        public ActionResult GetUserMessageByUserId()
        {
            UserMessageModel vm = new UserMessageModel();
            //vm = DataAccess.getuser
            return View();
        }

        
    }
}
