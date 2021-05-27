  using System;
  using System.Text;
  using System.Security.Cryptography;
  using System.IO;

namespace IG.Engine.EmailService.BLL
{
    public class DataProtection
    {

        private static string m_sKey = "MMMMMMMM";

        public static string Encrypt(string sInput)
        {
            string strValue = "";
            if (!(m_sKey == ""))
            {
                if (m_sKey.Length < 16)
                {
                    m_sKey += "XXXXXXXXXXXXXXXX".Substring(0, 16 - m_sKey.Length);
                }
                else
                {
                    if (m_sKey.Length > 16)
                    {
                        m_sKey = m_sKey.Substring(0, 16);
                    }
                }
                byte[] byteKey = Encoding.UTF8.GetBytes(m_sKey.Substring(0, 8));
                byte[] byteVector = Encoding.UTF8.GetBytes(m_sKey.Substring(8));
                byte[] byteData = Encoding.UTF8.GetBytes(sInput);
                DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
                MemoryStream objMemoryStream = new MemoryStream();
                CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateEncryptor(byteKey, byteVector), CryptoStreamMode.Write);
                objCryptoStream.Write(byteData, 0, byteData.Length);
                objCryptoStream.FlushFinalBlock();
                strValue = Convert.ToBase64String(objMemoryStream.ToArray());
            }
            else
            {
                strValue = sInput;
            }
            return strValue;
        }

        public static string Decrypt(string sInput)
        {
            string strValue = "";
            if (!(m_sKey == ""))
            {
                if (m_sKey.Length < 16)
                {
                    m_sKey += "XXXXXXXXXXXXXXXX".Substring(0, 16 - m_sKey.Length);
                }
                else
                {
                    if (m_sKey.Length > 16)
                    {
                        m_sKey = m_sKey.Substring(0, 16);
                    }
                }
                byte[] byteKey = Encoding.UTF8.GetBytes(m_sKey.Substring(0, 8));
                byte[] byteVector = Encoding.UTF8.GetBytes(m_sKey.Substring(8));
                byte[] byteData = new byte[sInput.Length + 1];
                try
                {
                    byteData = Convert.FromBase64String(sInput);
                }
                catch
                {
                    strValue = sInput;
                }
                if (strValue.Length == 0)
                {
                    try
                    {
                        DESCryptoServiceProvider objDES = new DESCryptoServiceProvider();
                        MemoryStream objMemoryStream = new MemoryStream();
                        CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDES.CreateDecryptor(byteKey, byteVector), CryptoStreamMode.Write);
                        objCryptoStream.Write(byteData, 0, byteData.Length);
                        objCryptoStream.FlushFinalBlock();
                        strValue = Encoding.UTF8.GetString(objMemoryStream.ToArray());
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                strValue = sInput;
            }
            return strValue;
        }

    }
}