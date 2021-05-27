using System;
using System.Collections.Generic;
using System.Text;

namespace IG.Engine.EmailService.DataAccess
{
    public class DataFactory
    {
        public static string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
        }

        public static SqlDataAccess GetInstance()
        {
            return new SqlDataAccess(GetConnectionString());
        } 

    } // end class

}// end namespace
