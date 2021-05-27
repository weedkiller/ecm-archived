using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.Base
{
    #region ExceptionHelper
    public class ExceptionHelper
    {
        #region Fields
        private static List<AdvancedException> exceptions = new List<AdvancedException>();
        #endregion

        #region Properties
        public static List<AdvancedException> Exceptions
        {
            get { return exceptions; }
            set { exceptions = value; }
        }

        public static int Count
        {
            get { return exceptions.Count; }
        }
        #endregion

        #region Public Methods
        #region ClearExceptions
        public static void ClearExceptions()
        {
            exceptions.Clear();
        }
        #endregion

        #region AddException
        public static AdvancedException AddException(AdvancedException ex)
        {
            exceptions.Add(ex);
            return ex;
        }

        public static AdvancedException AddException(string message, string errorCode = "")
        {
            AdvancedException ex = new AdvancedException(message);
            if (!String.IsNullOrEmpty(errorCode))
            {
                ex.ErrorCode = errorCode;
            }
            return AddException(ex);
        }
        #endregion

        #region RemoveException
        public static void RemoveException(AdvancedException ex)
        {
            exceptions.Remove(ex);
        }
        #endregion
        #endregion
    }
    #endregion

    #region AdvancedException
    public class AdvancedException : Exception
    {
        #region Properties
        public string ErrorCode { get; set; }
        #endregion

        #region Constructors
        public AdvancedException()
            : base()
        {

        }

        public AdvancedException(string message)
            : base(message)
        {

        }
        #endregion
    }
    #endregion
}
