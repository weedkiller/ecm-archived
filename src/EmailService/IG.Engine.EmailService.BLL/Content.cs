using System;
using System.Collections.Generic; 
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{ 
    public class Content
    {
        #region Fields
        public int ContentId;
        public int ContentType;
        public int CategoryId;
        public string Topic;
        public string ShortDetail;
        public string Detail;
        public string TextDetail;
        public string FileName1;
        public string FileDescription1;
        public string FileName2;
        public string FileDescription2;
        public string FileName3;
        public string FileDescription3;
        public string FileName4;
        public string FileDescription4;
        public string FileName5;
        public string FileDescription5;
        public bool Active;
        public bool Private;
        public bool AwayShow; 
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public Content()
        {
        }

        public Content(DataRow dr)
        {
            ContentId = DataManager.ConvertToInteger(dr["ContentId"]);
            ContentType = DataManager.ConvertToInteger(dr["ContentType"]);
            CategoryId = DataManager.ConvertToInteger(dr["CategoryId"]);
            Topic = DataManager.ConvertToString(dr["Topic"]);
            ShortDetail = DataManager.ConvertToString(dr["ShortDetail"]);
            Detail = DataManager.ConvertToString(dr["Detail"]);
            TextDetail = DataManager.ConvertToString(dr["TextDetail"]);
            FileName1 = DataManager.ConvertToString(dr["FileName1"]);
            FileDescription1 = DataManager.ConvertToString(dr["FileDescription1"]);
            FileName2 = DataManager.ConvertToString(dr["FileName2"]);
            FileDescription2 = DataManager.ConvertToString(dr["FileDescription2"]);
            FileName3 = DataManager.ConvertToString(dr["FileName3"]);
            FileDescription3 = DataManager.ConvertToString(dr["FileDescription3"]);
            FileName4 = DataManager.ConvertToString(dr["FileName4"]);
            FileDescription4 = DataManager.ConvertToString(dr["FileDescription4"]);
            FileName5 = DataManager.ConvertToString(dr["FileName5"]);
            FileDescription5 = DataManager.ConvertToString(dr["FileDescription5"]);
            Active = DataManager.ConvertToBoolean(dr["Active"]);
            Private = DataManager.ConvertToBoolean(dr["Private"]);
            AwayShow = DataManager.ConvertToBoolean(dr["AwayShow"]); 
        }
        #endregion

        public bool IsExistAttachFile()
        {
            if (FileName1 == "" && FileName2 == "" && FileName3 == "" && FileName4 == "" && FileName5 == "")
                return false;

            return true;
        }
    } 

}
