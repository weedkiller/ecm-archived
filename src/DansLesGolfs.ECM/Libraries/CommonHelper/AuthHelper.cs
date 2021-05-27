using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using DansLesGolfs.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DansLesGolfs
{
    public class AuthHelper
    {
        #region Fields
        public User _loggedInUser = null;
        private HttpContext Context = System.Web.HttpContext.Current;
        #endregion

        #region Properties
        public User User
        {
            get
            {
                return _loggedInUser;
            }
            private set
            {
                _loggedInUser = value;
            }
        }
        #endregion

        #region Constructor
        public AuthHelper()
        {
            Init();
        }
        #endregion

        #region Public Methods
        #region Attempt
        public bool Attempt(string username, string password, bool remember = false, int[] userTypes = null)
        {
            password = DataProtection.Encrypt(password);
            SqlDataAccess DataAccess = DataFactory.GetInstance();
            DansLesGolfs.BLL.User user = DataAccess.UserAuthentication(username, password, userTypes);
            bool result = false;
            if (user != null)
            {
                result = true;
                this.User = user;
                SetSessionByUserObject(user);
                if (remember)
                {
                    CookieHelper cookie = new CookieHelper();
                    cookie.AddCookie("LogonUserId", user.UserId.ToString(), DateTime.Now.AddYears(1));
                }
            }
            return result;
        }
        #endregion

        #region Check
        public bool Check()
        {
            int userId = 0;
            if (System.Web.HttpContext.Current.Session["LogonUserId"] == null)
            {
                CookieHelper cookie = new CookieHelper();
                HttpCookie logonCookie = cookie.GetCookie("LogonUserId");
                if (logonCookie != null)
                {
                    userId = DataManager.ToInt(logonCookie.Value);
                }
            }
            else
            {
                userId = DataManager.ToInt(System.Web.HttpContext.Current.Session["LogonUserId"], 0);
            }

            if (userId > 0)
            {
                SqlDataAccess DataAccess = DataFactory.GetInstance();
                User user = DataAccess.GetUser(userId);
                this.User = user;
                if (user != null)
                {
                    SetSessionByUserObject(user);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Logout
        public void Logout()
        {
            Context.Session.Clear();
            CookieHelper cookie = new CookieHelper();
            cookie.DeleteCookie("LogonUserId");
            cookie.DeleteCookie("_culture");
            cookie.DeleteCookie("_cultureId");
        }
        #endregion

        /// <summary>
        /// Refresh User Object to Latest User Information
        /// </summary>
        public void RefreshUserInfo()
        {
            if (this.User != null)
            {
                SqlDataAccess DataAccess = DataFactory.GetInstance();
                this.User = DataAccess.GetUser(this.User.UserId);
            }
        }
        #endregion

        #region Private Methods
        public void SetSessionByUserObject(DansLesGolfs.BLL.User user)
        {
            Context.Session["LogonUserId"] = user.UserId;
            Context.Session["LogonUserEmail"] = user.Email;
            Context.Session["LogonUserType"] = user.UserTypeId;
            Context.Session["LogonUserFirstname"] = user.FirstName;
            Context.Session["LogonUserMiddlename"] = user.MiddleName;
            Context.Session["LogonUserLastname"] = user.LastName;
            Context.Session["LogonSiteId"] = user.SiteId;
            Context.Session["IsLoggedIn"] = true;
            Context.Session["moxiemanager.filesystem.rootpath"] = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Contents");
            Context.Session["general.language"] = CultureHelper.GetCurrentNeutralCulture();
        }

        private void Init()
        {
            if (User == null && Context.Session["LogonUserid"] != null)
            {
                User = new User();
                User.UserId = DataManager.ToInt(Context.Session["LogonUserId"]);
                User.Email = DataManager.ToString(Context.Session["LogonUserEmail"]);
                User.UserTypeId = DataManager.ToInt(Context.Session["LogonUserType"]);
                User.FirstName = DataManager.ToString(Context.Session["LogonUserFirstname"]);
                User.MiddleName = DataManager.ToString(Context.Session["LogonUserMiddlename"]);
                User.LastName = DataManager.ToString(Context.Session["LogonUserLastname"]);
                User.SiteId = DataManager.ToInt(Context.Session["LogonSiteId"]);
            }
        }
        #endregion
    }
}