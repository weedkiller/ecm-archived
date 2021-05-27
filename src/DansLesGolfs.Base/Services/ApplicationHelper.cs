using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.Base.Services
{
    public class ApplicationHelper
    {
        #region Fields
        private static int? _currentLangId; 
        #endregion

        #region Properties
        #region CurrentLangId
        /// <summary>
        /// Current Language Id
        /// </summary>
        public static int? CurrentLangId
        {
            get { return _currentLangId; }
            set { _currentLangId = value; }
        }
        #endregion 
        #endregion
    }
}
