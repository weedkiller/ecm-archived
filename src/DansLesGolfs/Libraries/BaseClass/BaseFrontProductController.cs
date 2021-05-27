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
    public class BaseFrontProductController : BaseController
    {
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            return base.BeginExecuteCore(callback, state);
        }
    }

}
