using System;
using System.Collections.Generic;
using System.Text;
using IG.Engine.EmailService.DataAccess;

namespace IG.Engine.EmailService
{
    public class EmailItem
    {
        public string DisplayName = string.Empty;
        public string From = string.Empty;
        public string Tos = string.Empty;
        public string Subject = string.Empty;
        public string Body = string.Empty;
        public string CC = string.Empty;
        public string BCC = string.Empty;
        public List<string> AttachFiles = new List<string>() ;
        public string Host = string.Empty;
        public string UserName = string.Empty;
        public string Password = string.Empty;
        public int Port =0;
        public string FromDisplayName = string.Empty;
        public bool IsBodyHtml = true;
        public bool IsEnableSSL = false;
        public bool IsUseVERP = false;
        public string ReturnPath = string.Empty;
        public string UnsubscribeMailTo = string.Empty;


        public EmailItem()
        { 
            Host = GlobalData.Host;
            Port = GlobalData.Port;
            UserName = GlobalData.UserName;
            Password = GlobalData.Password;
        }

    }
  
}
