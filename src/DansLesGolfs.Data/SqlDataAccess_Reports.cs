using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DansLesGolfs.Data
{
    public partial class SqlDataAccess
    { 
        public List<SalesReport> GetSalesReport(DateTime? startDate, DateTime? endDate, long? siteId,string orderBy)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    #region set param
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[GetSalesReport]";

                    com.Parameters.AddWithValue("@FromDate", startDate);
                    com.Parameters.AddWithValue("@ToDate", endDate);
                    com.Parameters.AddWithValue("@SideId", siteId);
                    com.Parameters.AddWithValue("@OrderBy", orderBy);

                    com.Connection = cnn;
                    cnn.Open();

                    #endregion


                    List<SalesReport> listSalesReport = new List<SalesReport>();

                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            #region set model
                            SalesReport salesReport = new SalesReport();

                            salesReport.OrderId = Convert.ToInt32(rd["OrderId"]);
                            salesReport.OrderNumber = Convert.ToString(rd["OrderNumber"]);
                            salesReport.OrderDate = Convert.ToDateTime(rd["OrderDate"]);
                            salesReport.CustomerName = Convert.ToString(rd["CustomerName"]);
                            salesReport.UserId = Convert.ToInt32(rd["UserId"]);
                            salesReport.ItemCode = Convert.ToString(rd["ItemCode"]);
                            salesReport.ItemName = Convert.ToString(rd["ItemName"]);
                            salesReport.UnitPrice = float.Parse(rd["UnitPrice"].ToString());
                            salesReport.Quantity = float.Parse(rd["Quantity"].ToString());
                            salesReport.ExtendedPrice = float.Parse(rd["ExtendedPrice"].ToString());
                            salesReport.PaymentType = Convert.ToString(rd["PaymentType"].ToString());
                            salesReport.PaymentStatus = Convert.ToString(rd["PaymentStatus"].ToString());

                            listSalesReport.Add(salesReport);

                            #endregion

                        }
                    }

                    return listSalesReport;

                }
            }
        }

        public List<SalesReport> GetSalesReport(string startDate, string endDate, long? siteId, string orderBy)
        {
            var sales = new List<SalesReport>();

            try
            {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    #region set param
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[GetSalesReport]";

                    com.Parameters.AddWithValue("@FromDate", startDate);
                    com.Parameters.AddWithValue("@ToDate", endDate);
                    com.Parameters.AddWithValue("@SideId", siteId);
                    com.Parameters.AddWithValue("@OrderBy", orderBy);

                    com.Connection = cnn;
                    cnn.Open();

                    #endregion


                    List<SalesReport> listSalesReport = new List<SalesReport>();

                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            #region set model
                            SalesReport salesReport = new SalesReport();

                            salesReport.OrderId = Convert.ToInt32(rd["OrderId"]);
                            salesReport.OrderNumber = Convert.ToString(rd["OrderNumber"]);
                            salesReport.OrderDate = Convert.ToDateTime(rd["OrderDate"]);
                            salesReport.CustomerName = Convert.ToString(rd["CustomerName"]);
                            salesReport.UserId = Convert.ToInt32(rd["UserId"]);

                            salesReport.ExtendedPrice = float.Parse(rd["TotalPrice"].ToString());
                            salesReport.PaymentType = Convert.ToString(rd["PaymentType"].ToString());
                            salesReport.Active = Convert.ToBoolean(rd["Active"].ToString());
                            salesReport.PaymentStatus = Convert.ToString(rd["PaymentStatus"].ToString());

                            listSalesReport.Add(salesReport);

                            #endregion

                        }
                    }

                    return listSalesReport;

                }
            }
               // dataac
            }
            catch (Exception ex)
            {
                
                throw;
            }

            return sales;
        }

        public List<SaleItemReport> GetSaleItemsReport(jQueryDataTableParamModel param, string fromDate, string toDate, long siteId = 0, int langId = 1)
        {
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectSaleItemsReport",
                new SqlParameter("@Start", param.start),
                new SqlParameter("@Length", param.length),
                new SqlParameter("@Search", param.search),
                new SqlParameter("@Order", param.order),
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate),
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("@LangId", langId));

            param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

            List<SaleItemReport> list = new List<SaleItemReport>();
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new SaleItemReport(row));
            }

            return list;
        }
    }
}
