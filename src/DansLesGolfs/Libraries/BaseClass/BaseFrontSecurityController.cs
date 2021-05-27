using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DansLesGolfs.Controllers
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
            if (!Auth.Check())
            {
                System.Web.HttpContext.Current.Response.Redirect("~/Error/NotLogIn");
            }
            else if (System.Web.HttpContext.Current.Session["LogonUserType"] != null)
            {
                int userType = DataManager.ToInt(System.Web.HttpContext.Current.Session["LogonUserType"]);
                //switch (userType)
                //{
                //    case UserTypes.Type.SuperAdmin:
                //    case UserTypes.Type.Admin:
                //        System.Web.HttpContext.Current.Response.Redirect("~/Admin");
                //        break;
                //    case UserTypes.Type.Customer:
                //    case UserTypes.Type.Reseller:
                //        System.Web.HttpContext.Current.Response.Redirect("~/Account");
                //        break;
                //    default:
                //        System.Web.HttpContext.Current.Response.Redirect("~/Login");
                //        break;
                //}
                if(userType < 0)
                {
                    System.Web.HttpContext.Current.Response.Redirect("~/Error/NotLogIn");
                }
            }
        }
    }
}
