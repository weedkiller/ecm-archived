using System;
using System.Collections.Generic; 
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{ 
    // User 
    public class User
    {
        #region Fields
        public int UserId;
        public int Title;
        public string FirstName;
        public string LastName;
        public string Username;
        public string Password;
        public int RoleId; 
        public string Email;
        public bool Active;
        public bool IsDelete;
        public int UserTypeId;
        public int CustomerTypeId;
        public string Address;
        public string Province;
        public string Country;
        public string Zip;
        public int NewsletterType;
        public int MemberType;
        public bool IsVerified;
        public string IPAddress;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public User()
        {
        }

        public User(DataRow dr)
        {
            UserId = DataManager.ConvertToInteger(dr["UserId"]);
            Title = DataManager.ConvertToInteger(dr["Title"]);
            FirstName = DataManager.ConvertToString(dr["FirstName"]);
            LastName = DataManager.ConvertToString(dr["LastName"]);
            Username = DataManager.ConvertToString(dr["Username"]);
            Password = DataManager.ConvertToString(dr["Password"]);
            RoleId = DataManager.ConvertToInteger(dr["RoleId"]); 
            Email = DataManager.ConvertToString(dr["Email"]);
            Active = DataManager.ConvertToBoolean(dr["Active"]);
            IsDelete = DataManager.ConvertToBoolean(dr["IsDelete"]);
            UserTypeId = DataManager.ConvertToInteger(dr["UserTypeId"]);
            CustomerTypeId = DataManager.ConvertToInteger(dr["CustomerTypeId"]);
            Address = DataManager.ConvertToString(dr["Address"]);
            Province = DataManager.ConvertToString(dr["Province"]);
            Country = DataManager.ConvertToString(dr["Country"]);
            Zip = DataManager.ConvertToString(dr["Zip"]);
            IPAddress = DataManager.ConvertToString(dr["IPAddress"],"");
            NewsletterType = DataManager.ConvertToInteger(dr["NewsletterType"]);
            MemberType = DataManager.ConvertToInteger(dr["MemberType"]);

            IsVerified = DataManager.ConvertToBoolean(dr["IsVerified"]);
        }

        #endregion

    } 

}
