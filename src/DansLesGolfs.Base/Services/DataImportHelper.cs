using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DansLesGolfs.Base.Services
{
    public static class DataImportHelper
    {
        public static DataTable ImportCsv(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            DataTable dt = new DataTable();

            using (OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + file.FullName + "\";Extended Properties='text;HDR=Yes;FMT=Delimited(,)';"))
            {
                using (OleDbCommand cmd = new OleDbCommand(string.Format("SELECT * FROM [{0}]", file.Name), con))
                {
                    con.Open();
                    using (OleDbDataAdapter adp = new OleDbDataAdapter(cmd))
                    {
                        adp.Fill(dt, file.Name);
                    }
                }
            }

            return dt;
        }

        public static DataSet ImportExcel(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            DataSet ds = new DataSet();

            using(FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelReader = null;

                if (file.Extension == ".xls")
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (file.Extension == ".xlsx")
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                if(excelReader != null)
                {
                    excelReader.IsFirstRowAsColumnNames = true;

                    ds = excelReader.AsDataSet();

                    excelReader.Close();
                }
            }

            return ds;
        }
    }
}
