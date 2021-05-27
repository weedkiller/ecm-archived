using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DansLesGolfs.Base
{
    public static class DataManager
    {
        #region To
        /// <summary>
        /// Convert with generic type (without specific default value).
        /// </summary>
        /// <typeparam name="T">Type name</typeparam>
        /// <param name="value">value that you want to convert</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns></returns>
        public static T To<T>(object value)
        {
            return To<T>(value, default(T));
        }

        /// <summary>
        /// Convert with generic type.
        /// </summary>
        /// <typeparam name="T">Type name</typeparam>
        /// <param name="value">value that you want to convert</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns></returns>
        public static T To<T>(object value, T defaultValue)
        {
            if (value == null || Convert.IsDBNull(value))
            {
                return default(T);
            }
            else
            {
                try
                {
                    return (T)value;
                }
                catch (Exception ex)
                {
                    return default(T);
                }
            }
        }
        #endregion

        #region ToDouble
        /// <summary>
        /// Convert object into Double data type.
        /// </summary>
        /// <param name="obj">Object that you want to convert to Double.</param>
        /// <returns>Converted value.</returns>
        public static double ToDouble(object value)
        {
            return ToDouble(value, 0);
        }

        /// <summary>
        /// Convert object into Double data type.
        /// </summary>
        /// <param name="obj">Object that you want to convert to Double.</param>
        /// <param name="defaultValue">Defalut value, when you can't convert successful.</param>
        /// <returns>Converted value.</returns>
        public static double ToDouble(object obj, double defaultValue)
        {
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    return defaultValue;
                }
                else
                {
                    return Convert.ToDouble(obj, System.Threading.Thread.CurrentThread.CurrentCulture);
                }
            }
            catch
            {
                return 0F;
            }
        }
        #endregion

        #region ToFloat
        public static float ToFloat(object obj)
        {
            return ToFloat(obj, 0);
        }

        public static float ToFloat(object obj, float defaultValue)
        {
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    return defaultValue;
                }
                else
                {
                    return Convert.ToSingle(obj, System.Threading.Thread.CurrentThread.CurrentCulture);
                }
            }
            catch
            {
                return 0F;
            }
        }
        #endregion

        #region ToInt
        public static int ToInt(object obj, int defaultValue)
        {
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    return defaultValue;
                }
                else
                {

                    return Convert.ToInt32(obj, System.Threading.Thread.CurrentThread.CurrentCulture);
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int ToInt(object obj)
        {
            return ToInt(obj, 0);
        }
        #endregion

        #region ToShort
        public static short ToShort(object obj, short defaultValue)
        {
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    return defaultValue;
                }
                else
                {

                    return Convert.ToInt16(obj, System.Threading.Thread.CurrentThread.CurrentCulture);
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        public static short ToShort(object obj)
        {
            return ToShort(obj, 0);
        }
        #endregion

        #region ToLong
        public static long ToLong(object obj, long defaultValue)
        {
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    return defaultValue;
                }
                else
                {

                    return Convert.ToInt64(obj, System.Threading.Thread.CurrentThread.CurrentCulture);
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        public static long ToLong(object obj)
        {
            return ToLong(obj, 0);
        }
        #endregion

        #region ToGuid
        public static Guid ToGuid(object obj, Guid defaultValue)
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
        #endregion

        #region ToDecimal
        public static decimal ToDecimal(object obj, decimal defaultValue)
        {
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    return defaultValue;
                }
                else
                {
                    return Convert.ToDecimal(obj, System.Threading.Thread.CurrentThread.CurrentCulture);
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        public static decimal ToDecimal(object obj)
        {
            return ToDecimal(obj, 0m);
        }
        #endregion

        #region ToString
        public static string ToString(object obj, string defaultValue)
        {
            if (System.Convert.IsDBNull(obj) || obj == null)
            {
                return defaultValue;
            }
            else
            {
                return obj.ToString();
            }
        }

        public static string ToString(object obj)
        {
            return ToString(obj, string.Empty);
        }
        #endregion

        #region ToDateTime
        public static DateTime ToDateTime(object obj, DateTime defaultValue)
        {
            if (System.Convert.IsDBNull(obj) || obj == null)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    return Convert.ToDateTime(obj);
                }
                catch (Exception ex)
                {
                    return defaultValue;
                }
            }
        }

        public static DateTime ToDateTime(object obj)
        {
            return ToDateTime(obj, DateTime.MinValue);
        }

        public static DateTime ToDateTime(object obj, string pattern, DateTime defaultValue)
        {
            if (System.Convert.IsDBNull(obj) || obj == null)
            {
                return defaultValue;
            }
            else
            {
                try
                {
                    DateTime tempDate = defaultValue;
                    if(DateTime.TryParseExact(obj.ToString(), pattern, System.Threading.Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out tempDate))
                    {
                        return tempDate;
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
                catch (Exception ex)
                {
                    return defaultValue;
                }
            }
        }

        public static DateTime ToDateTime(object obj, string pattern)
        {
            return ToDateTime(obj, pattern, DateTime.Now);
        }
        #endregion

        #region ToBoolean
        public static bool ToBoolean(object obj)
        {
            return ToBoolean(obj, false);
        }

        public static bool ToBoolean(object obj, bool defaultValue)
        {
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    return defaultValue;
                }
                else
                {
                    return Convert.ToBoolean(obj);
                }
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion

        #region ToByte
        public static byte ToByte(object obj)
        {
            return ToByte(obj, byte.MinValue);
        }

        public static byte ToByte(object obj, byte defaultValue)
        {
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    return defaultValue;
                }
                else
                {
                    return Convert.ToByte(obj);
                }
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion

        #region HasDateTime
        public static bool HasDateTime(DateTime datetime)
        {
            return datetime != DateTime.MinValue ? true : false;
        }

        public static bool HasDateTime(DateTime? datetime)
        {
            return datetime.HasValue ? HasDateTime(datetime.Value) : false;
        }
        #endregion

        #region EnumToKeyValuePairs
        public static Dictionary<int, string> EnumToKeyValuePairs<T>()
        {
            Dictionary<int, string> table = new Dictionary<int, string>();
            string[] names = Enum.GetNames(typeof(T));
            for (int i = 0; i < names.Length; i++)
            {
                table.Add(i, names[i]);
            }
            return table;
        } 
        #endregion

        public static TimeSpan ToTimeSpan(object obj)
        {
            return ToTimeSpan(obj, TimeSpan.MinValue);
        }

        public static TimeSpan ToTimeSpan(object obj, TimeSpan defaultValue)
        {
            try
            {
                if (System.Convert.IsDBNull(obj) || obj == null)
                {
                    return defaultValue;
                }
                else
                {
                    TimeSpan time = TimeSpan.MinValue;
                    if(TimeSpan.TryParse(obj.ToString(), out time))
                    {
                        return time;
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
