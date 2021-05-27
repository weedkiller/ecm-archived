using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DansLesGolfs.ECM.Controllers
{
    public class BaseFrontSecurityController : BaseFrontController
    {
        public BaseFrontSecurityController()
            : base()
        {
            CheckLogin();
        }

        private void CheckLogin()
        {
            if(!Auth.Check())
            {
                System.Web.HttpContext.Current.Response.Redirect("~/Login");
            }
            else if(System.Web.HttpContext.Current.Session["LogonUserType"] != null)
            {
                int userType = DataManager.ToInt(System.Web.HttpContext.Current.Session["LogonUserType"]);
                if (userType > -1) // Allow all user type.
                {
                    System.Web.HttpContext.Current.Response.Redirect("~/Login");
                }
            }
        }
    }
}
