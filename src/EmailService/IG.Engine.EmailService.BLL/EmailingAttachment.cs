using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace IG.Engine.EmailService.BLL
{
    // Role

    public class EmailingAttachment
    {
        #region Fields
        public long EmailingAttachmentId;
        public string FileName;
        public string BaseName;
        public string FileExtension;
        public string FilePath;
        #endregion

        #region Constructors
        public EmailingAttachment()
        {
        }

        public EmailingAttachment(DataRow dr)
        {
            EmailingAttachmentId = DataManager.ConvertToLong(dr["EmailingAttachmentId"]);
            FileName = DataManager.ConvertToString(dr["FileName"]);
            BaseName = DataManager.ConvertToString(dr["BaseName"]);
            FileExtension = DataManager.ConvertToString(dr["FileExtension"]);
            FilePath = DataManager.ConvertToString(dr["FilePath"]);
        }
        #endregion

    }

}
