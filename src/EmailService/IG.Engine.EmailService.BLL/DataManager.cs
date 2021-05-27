using System;
using System.Collections.Generic;
using System.Text;

namespace IG.Engine.EmailService.BLL
{
    public class DataManager
    {
        public static double ConvertToDouble(object obj)
        {
            return ConvertToDouble(obj, 0);
        }

        public static double ConvertToDouble(object obj, double defaultValue)
        {
            double fResult;
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    fResult = defaultValue;
                }
                else
                {
                    fResult = Convert.ToDouble(obj);
                }
            }
            catch
            {
                return 0F;
            }
            return fResult;
        }

        public static float ConvertToFloat(object obj, float defaultValue)
        {
            float fResult;
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    fResult = defaultValue;
                }
                else
                {
                    fResult = ((float)(Convert.ToDouble(obj)));
                }
            }
            catch
            {
                return 0F;
            }
            return fResult;
        }

        public static int ConvertToInteger(object obj, int defaultValue)
        {
            int dResult;
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    dResult = defaultValue;
                }
                else
                {

                    dResult = Convert.ToInt32(obj);
                }
            }
            catch
            {
                return defaultValue;
            }
            return dResult;
        }

        public static int ConvertToInteger(object obj)
        {
            int dResult;
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    dResult = 0;
                }
                else
                {
                    dResult = Convert.ToInt32(obj);
                }
            }
            catch
            {
                return 0;
            }
            return dResult;
        }

        public static long ConvertToLong(object obj, int defaultValue)
        {
            long dResult;
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    dResult = defaultValue;
                }
                else
                {

                    dResult = Convert.ToInt64(obj);
                }
            }
            catch
            {
                return defaultValue;
            }
            return dResult;
        }

        public static long ConvertToLong(object obj)
        {
            long dResult;
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    dResult = 0;
                }
                else
                {
                    dResult = Convert.ToInt64(obj);
                }
            }
            catch
            {
                return 0;
            }
            return dResult;
        }


        public static Guid ConvertToGuid(object obj, Guid defaultValue)
        {
            Guid result = defaultValue;
            try
            {
                if (!System.Convert.IsDBNull(obj) && obj != null)
                {
                    result = (Guid)obj;
                }
            }
            catch
            {

            }
            return result;
        }


        public static decimal ConvertToDecimal(object obj, decimal defaultValue)
        {
            decimal dResult;
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    dResult = defaultValue;
                }
                else
                {
                    dResult = Convert.ToDecimal(obj);
                }
            }
            catch
            {
                dResult = defaultValue;
            }
            return dResult;
        }

        public static decimal ConvertStringToDecimal(string sInput, decimal defaultValue)
        {
            decimal dResult;
            try
            {
                dResult = Convert.ToDecimal(sInput);
            }
            catch
            {
                dResult = defaultValue;
            }
            return dResult;
        }

        public static DateTime ConvertStringToDateTime(string sInput, DateTime defaultValue)
        {
            DateTime dResult;
            try
            {
                dResult = Convert.ToDateTime(sInput);
            }
            catch
            {
                dResult = defaultValue;
            }
            return dResult;
        }

        public static string ConvertToString(object obj, string defaultValue)
        {
            string sResult;
            if (System.Convert.IsDBNull(obj) || obj == null)
            {
                sResult = defaultValue;
            }
            else
            {
                sResult = obj.ToString();
            }
            return sResult;
        }

        public static string ConvertToString(object obj)
        {
            return ConvertToString(obj, "");
        }

        public static DateTime ConvertToDateTime(object obj, DateTime defaultValue)
        {
            DateTime dResult;
            if (System.Convert.IsDBNull(obj) || obj == null)
            {
                dResult = defaultValue;
            }
            else
            {
                dResult = Convert.ToDateTime(obj);
            }
            return dResult;
        }

        public static DateTime ConvertToDateTime(object obj)
        {
            DateTime dResult;
            if (System.Convert.IsDBNull(obj) || obj == null)
            {
                //System.Data.SqlDbType.DateTime.
                dResult = DateTime.MinValue;
            }
            else
            {
                dResult = Convert.ToDateTime(obj);
            }
            return dResult;
        }

        public static bool ConvertToBoolean(object obj)
        {
            return ConvertToBoolean(obj, false);
        }

        public static bool ConvertToBoolean(object obj, bool defaultValue)
        {
            bool bResult;
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    bResult = defaultValue;
                }
                else
                {
                    bResult = Convert.ToBoolean(obj);
                }
            }
            catch 
            {
                bResult = defaultValue;
            }

            return bResult;
        }

    }
}
