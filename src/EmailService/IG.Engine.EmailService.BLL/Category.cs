using System;
using System.Collections.Generic; 
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{ 

    public class Category
    {
        #region Fields
        public int CategoryId;
        public string CategoryName;
        public bool Active;
        public int ContentType;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public Category()
        {
        }

        public Category(DataRow dr)
        {
            CategoryId = DataManager.ConvertToInteger(dr["CategoryId"]);
            CategoryName = DataManager.ConvertToString(dr["CategoryName"]);
            Active = DataManager.ConvertToBoolean(dr["Active"]);
            ContentType = DataManager.ConvertToInteger(dr["ContentType"]); 
        }
        #endregion

    } 

}
