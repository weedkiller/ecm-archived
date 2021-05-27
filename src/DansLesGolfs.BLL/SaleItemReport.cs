using System;
using System.Collections.Generic; 
using System.Text;
using System.Data;
using DansLesGolfs.Base;

namespace DansLesGolfs.BLL
{ 

    public class SaleItemReport
    {
        #region Fields
        #endregion

        #region Properties
        public long OrderId { get; set; }
        public int RowNumber { get; set; }
        public string MonthName { get; set; }
        public int NumOfWeek { get; set; }
        public DateTime OrderDate { get; set; }
        public string TransactionId { get; set; }
        public string OrderNumber { get; set; }
        public string CategoryName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemTypeId { get; set; }

        public DateTime ReserveDate { get; set; }
        public string GolfName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PaymentType { get; set; }
        public int Qty { get; set; }
        public decimal TotalBasePrice { get; set; }
        public decimal Discount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalTTC { get; set; }
        public decimal TotalHT { get; set; }
        public string GolfBrandName { get; set; }
        public string PaymentStatus { get; set; }
        #endregion

        #region Constructors
        public SaleItemReport()
        {
        }

        public SaleItemReport(DataRow row)
        {
            OrderId = DataManager.ToLong(row["OrderId"]);
            RowNumber = DataManager.ToInt(row["RowNumber"]);
            MonthName = DataManager.ToString(row["MonthName"]);
            NumOfWeek = DataManager.ToInt(row["NumOfWeek"]);
            OrderDate = DataManager.ToDateTime(row["OrderDate"]);
            TransactionId = DataManager.ToString(row["TransactionId"]);
            OrderNumber = DataManager.ToString(row["OrderNumber"]);
            CategoryName = DataManager.ToString(row["CategoryName"]);
            ItemCode = DataManager.ToString(row["ItemCode"]);
            ItemName = DataManager.ToString(row["ItemName"]);
            ItemTypeId = DataManager.ToInt(row["ItemTypeId"]);
            ReserveDate = DataManager.ToDateTime(row["ReserveDate"]);
            GolfName = DataManager.ToString(row["GolfName"]);
            FirstName = DataManager.ToString(row["FirstName"]);
            LastName = DataManager.ToString(row["LastName"]);
            PaymentType = DataManager.ToString(row["PaymentType"]);
            Qty = DataManager.ToInt(row["Qty"]);
            TotalBasePrice = DataManager.ToDecimal(row["TotalBasePrice"]);
            Discount = DataManager.ToDecimal(row["Discount"]);
            ShippingCost = DataManager.ToDecimal(row["ShippingCost"]);
            TotalHT = DataManager.ToDecimal(row["TotalHT"]);
            TotalTTC = DataManager.ToDecimal(row["TotalTTC"]);
            GolfBrandName = DataManager.ToString(row["GolfBrandName"]);
            PaymentStatus = DataManager.ToString(row["PaymentStatus"]);
        }
        #endregion

    } 

}
