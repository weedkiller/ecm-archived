using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DansLesGolfs.Base;

namespace DansLesGolfs.Data
{
    public class DataFactory
    {
        #region GetConnectionString
        /// <summary>
        /// Get Connection String from configuration file.
        /// </summary>
        /// <param name="connectionStringName">Connection String Name.</param>
        /// <returns>Connection String.</returns>
        public static string GetConnectionString(string connectionStringName = "ConnectionString")
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ToString();
        }
        #endregion

        #region GetInstance
        /// <summary>
        /// Get SqlDataAccess Instance.
        /// </summary>
        /// <param name="connectionStringName">Connection String Name.</param>
        /// <returns>SqlDataAccess Instance.</returns>
        public static SqlDataAccess GetInstance(string connectionStringName = "ConnectionString")
        {
            return new SqlDataAccess(GetConnectionString(connectionStringName));
        }
        #endregion

        public static AlbatrosSettings GetAlbatrosSettings()
        {
            AlbatrosSettings settings = new AlbatrosSettings()
            {
                Url = System.Configuration.ConfigurationManager.AppSettings["AlbatrosWebServiceUrl"],
                Login = System.Configuration.ConfigurationManager.AppSettings["AlbatrosUsername"],
                Password = System.Configuration.ConfigurationManager.AppSettings["AlbatrosPassword"],
                Protocol = System.Configuration.ConfigurationManager.AppSettings["AlbatrosProtocol"]
            };
            return settings;
        }

        public static AlbatrosDataAccess GetAlbatrosInstance()
        {
            return new AlbatrosDataAccess(GetAlbatrosSettings());
        }

        public static ChronogolfDataAccess GetChronogolfInstance(int clubId)
        {
            string tokenUrl = System.Configuration.ConfigurationManager.AppSettings["ChronogolfTokenUrl"];
            string tokenSecret = System.Configuration.ConfigurationManager.AppSettings["ChronogolfTokenSecret"];
            ChronogolfDataAccess chronogolf = new ChronogolfDataAccess(tokenUrl, tokenSecret, clubId);
            return chronogolf;
        }
    }
}
