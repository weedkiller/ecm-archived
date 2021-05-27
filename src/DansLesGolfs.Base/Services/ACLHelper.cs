using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.Base.Services
{
    /// <summary>
    /// Access Control List Helper
    /// </summary>
    public class ACLHelper
    {
        #region Fields
        private List<int> allowTypes = new List<int>();
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        public void Add(int userType)
        {
            if(allowTypes.Contains(userType))
            {
                allowTypes.Add(userType);
            }
        }

        public bool Check(int userType)
        {
            return allowTypes.Contains(userType);
        }
        #endregion
    }
}
