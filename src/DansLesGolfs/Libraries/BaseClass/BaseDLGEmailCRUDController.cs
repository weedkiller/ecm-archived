using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs.Controllers
{
    public class BaseDLGEmailCRUDController : BaseAdminCRUDController
    {
        #region Properties
        public override string AreaName
        {
            get { return "DLGEmail"; }
        }
        #endregion
    }
}