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

        public List<AccountOrder> GetAccountOrderList(int userId)
        {
            string sql = @"WITH CalculatedOrderItems AS (
                                SELECT OrderItemId, ItemName, OrderId, ItemId, UnitPrice, Quantity, ShippingCost, SiteId, ReserveDate, VatId, ItemCouponId, VatRate, 
                                ReductionRate, ReductionType, (CASE WHEN [ReductionType] = 0 THEN [ReductionRate] ELSE (([UnitPrice] * [ReductionRate]) / 100) END) AS Discount
                                FROM dbo.OrderItem
                            )
                            SELECT [Order].OrderId,CustomerId, TransactionId, [OrderNumber], [OrderDate], SUM((oi.UnitPrice - oi.Discount + oi.ShippingCost) * oi.Quantity) AS TotalPrice, 'Dans les Golfs' AS VendorName
                            FROM [Order]
                            INNER JOIN CalculatedOrderItems oi ON [Order].OrderId = oi.OrderId
                            WHERE [Order].Active = 1 AND CustomerId = @UserId AND [PaymentStatus] = N'success'
                            GROUP BY [Order].[OrderId], [OrderNumber], [OrderDate], [TransactionId], [CustomerId]
                            ORDER BY [Order].[OrderId] DESC";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@UserId", userId));

            List<AccountOrder> resultlist = new List<AccountOrder>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                resultlist.Add(new AccountOrder()
                {
                    VendorName = DataManager.ToString(row["VendorName"]),
                    OrderId = DataManager.ToInt(row["OrderId"]),
                    OrderDate = DataManager.ToDateTime(row["OrderDate"]),
                    TotalPrice = DataManager.ToDecimal(row["TotalPrice"]),
                    OrderNumber = DataManager.ToString(row["OrderNumber"])
                });
            }
            return resultlist;
        }


        public List<AccountOrderDetail> GetAccountOrderDetail(int orderId)
        {
            string sql = @"SELECT ISNULL(CategoryName, '-') AS CategoryName, ItemName, UnitPrice, Quantity, UnitPrice * Quantity AS TotalPrice
	                    FROM OrderItem
		                    LEFT OUTER JOIN Item ON OrderItem.ItemId = Item.ItemId
		                    LEFT OUTER JOIN ItemCategory ON Item.CategoryId = ItemCategory.CategoryId
	                    WHERE OrderItem.OrderId = @OrderId";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@OrderId", orderId));

            List<AccountOrderDetail> resultList = new List<AccountOrderDetail>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                AccountOrderDetail result = new AccountOrderDetail();

                result.CategoryName = DataManager.ToString(row["CategoryName"]);
                result.ItemName = DataManager.ToString(row["ItemName"]);
                result.UnitPrice = DataManager.ToDecimal(row["UnitPrice"]);
                result.Quantity = DataManager.ToInt(row["Quantity"]);
                result.ShippingCost = DataManager.ToDecimal(row["ShippingCost"]);
                result.TotalPrice = DataManager.ToDecimal(row["TotalPrice"]);
                resultList.Add(result);
            }

            return resultList;
        }


        //public List<UserMessageModel> GetUserMessageList(int startIndex, int maxRetriveIte, string sortColumnName, string sortDirection, string searchText, int? userId, int? messageId)
        //{
        //    using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
        //    {
        //        using (SqlCommand com = new SqlCommand())
        //        {
        //            com.CommandType = System.Data.CommandType.StoredProcedure;
        //            com.CommandText = "[dbo].[GetUserMessage]";

        //            com.Parameters.AddWithValue("@StartIndex", startIndex);
        //            com.Parameters.AddWithValue("@MaxRetriveItem", maxRetriveIte);
        //            com.Parameters.AddWithValue("@SortColumnName", sortColumnName);
        //            com.Parameters.AddWithValue("@SortDirection", sortDirection);
        //            com.Parameters.AddWithValue("@SearchText", searchText);
        //            com.Parameters.AddWithValue("@UserId", userId);
        //            com.Parameters.AddWithValue("@messageId", messageId);

        //            com.Connection = cnn;
        //            cnn.Open();

        //            List<UserMessageModel> resultlist = new List<UserMessageModel>();
        //            using (SqlDataReader rd = com.ExecuteReader())
        //            {
        //                while (rd.Read())
        //                {
        //                    UserMessageModel result = new UserMessageModel();
        //                    if (rd["MessageId"] != DBNull.Value)
        //                        result.MessageId = Convert.ToInt32(rd["MessageId"]);

        //                    if (rd["MessageTypeId"] != DBNull.Value)
        //                        result.MessageTypeId = Convert.ToInt32(rd["MessageTypeId"]);

        //                    result.Subject = Convert.ToString(rd["Subject"]);

        //                    if (rd["FromUserId"] != DBNull.Value)
        //                        result.FromUserId = Convert.ToInt32(rd["FromUserId"]);

        //                    if (rd["ToUserId"] != DBNull.Value)
        //                        result.ToUserId = Convert.ToInt32(rd["ToUserId"]);

        //                    result.Body = Convert.ToString(rd["Body"]);

        //                    if (rd["SentDate"] != DBNull.Value)
        //                        result.SentDate = Convert.ToDateTime(rd["SentDate"]);

        //                    if (rd["ReadDate"] != DBNull.Value)
        //                        result.ReadDate = Convert.ToDateTime(rd["ReadDate"]);

        //                    if (rd["Active"] != DBNull.Value)
        //                        result.Active = Convert.ToBoolean(rd["Active"]);

        //                    if (rd["HasAttachedFile"] != DBNull.Value)
        //                        result.HasAttachedFile = Convert.ToBoolean(rd["HasAttachedFile"]);

        //                    if (rd["IsFlag"] != DBNull.Value)
        //                        result.IsFlag = Convert.ToBoolean(rd["IsFlag"]);

        //                    if (rd["AttachedId"] != DBNull.Value)
        //                        result.AttachedId = Convert.ToInt32(rd["AttachedId"]);

        //                    result.BaseFileName = Convert.ToString(rd["BaseFileName"]);
        //                    result.FileExtension = Convert.ToString(rd["FileExtension"]);
        //                    result.FileName = Convert.ToString(rd["FileName"]);

        //                    if (rd["InsertDate"] != DBNull.Value)
        //                        result.InsertDate = Convert.ToDateTime(rd["InsertDate"]);

        //                    if (rd["ToltalItemCount"] != DBNull.Value)
        //                        result.ToltalItemCount = Convert.ToInt32(rd["ToltalItemCount"]);

        //                    result.FileName = Convert.ToString(rd["FileName"]);
        //                    result.FileExtension = Convert.ToString(rd["FileExtension"]);

        //                    result.FirstName = Convert.ToString(rd["FirstName"]);
        //                    result.Lastname = Convert.ToString(rd["LastName"]);

        //                    result.FromName = Convert.ToString(rd["FromName"]);
        //                    result.ToName = Convert.ToString(rd["ToName"]);

        //                    resultlist.Add(result);
        //                }
        //            }

        //            return resultlist;

        //        }
        //    }
        //}

        public int AddUserMessageAdmin(string subject, int fromUserId, int toUserId, string body, string baseFileName, string fileExtension, string RandomfileName)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[AddUserMessage]";

                    com.Parameters.AddWithValue("@Subject", subject);
                    com.Parameters.AddWithValue("@FromUserId", fromUserId);
                    com.Parameters.AddWithValue("@ToUserId", toUserId);
                    com.Parameters.AddWithValue("@Body", body);

                    if (!string.IsNullOrEmpty(baseFileName))
                        com.Parameters.AddWithValue("@BaseFileName", baseFileName);

                    if (!string.IsNullOrEmpty(fileExtension))
                        com.Parameters.AddWithValue("@FileExtension", fileExtension);

                    if (!string.IsNullOrEmpty(RandomfileName))
                        com.Parameters.AddWithValue("@RandomfileName", RandomfileName);

                    SqlParameter messageId = new SqlParameter();
                    messageId.Direction = System.Data.ParameterDirection.Output;
                    messageId.ParameterName = "@Result";
                    messageId.SqlDbType = System.Data.SqlDbType.Int;
                    com.Parameters.Add(messageId);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();

                    return Convert.ToInt32(messageId.Value);

                }
            }
        }


        public int DeleteUserMessageAdmin(int messageId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[DeleteUserMessageAdmin]";

                    com.Parameters.AddWithValue("@MessageId", messageId);

                    SqlParameter result = new SqlParameter();
                    result.Direction = System.Data.ParameterDirection.Output;
                    result.ParameterName = "@result";
                    result.SqlDbType = System.Data.SqlDbType.Int;
                    com.Parameters.Add(result);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();

                    return Convert.ToInt32(result.Value);
                }
            }
        }

        public int UpdateReadMessage(int messageId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[UpdateReadMessage]";

                    com.Parameters.AddWithValue("@MessageId", messageId);

                    SqlParameter result = new SqlParameter();
                    result.Direction = System.Data.ParameterDirection.Output;
                    result.ParameterName = "@result";
                    result.SqlDbType = System.Data.SqlDbType.Int;
                    com.Parameters.Add(result);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();

                    return Convert.ToInt32(result.Value);
                }
            }
        }

        public int UpdateFlagMessage(int messageId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[UpdateFlagMessage]";

                    com.Parameters.AddWithValue("@MessageId", messageId);

                    SqlParameter result = new SqlParameter();
                    result.Direction = System.Data.ParameterDirection.Output;
                    result.ParameterName = "@result";
                    result.SqlDbType = System.Data.SqlDbType.Int;
                    com.Parameters.Add(result);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();

                    return Convert.ToInt32(result.Value);
                }
            }
        }

        public dynamic GetTotalRecentOrdersByUserId(int userId)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddDays(-1);
            string sql = @"SELECT COUNT(*)
                          FROM [dbo].[Order]
                          WHERE [CustomerId] = @UserId AND ISNULL([IsNoticed], 0) = 0";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@UserId", userId),
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)), 0);
        }

        public int GetTotalUserMessageByUserId(int userId)
        {
            string sql = @"SELECT COUNT(*)
                          FROM [dbo].[UserMessage]
                          WHERE [ToUserId] = @UserId AND [ReadDate] Is null";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@UserId", userId)), 0);
        }

        public void UpdateOrderNoticeStatusByUserId(bool isNoticed, int userId)
        {
            string sql = "UPDATE [Order] SET [IsNoticed] = @IsNoticed WHERE [CustomerId] = @CustomerId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@IsNoticed", isNoticed),
                new SqlParameter("@CustomerId", userId));
        }

        public void UpdateConnectWithFacebook(int userId, string appid)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[UpdateConnectWithFacebook]";

                    com.Parameters.AddWithValue("@UserId", userId);
                    com.Parameters.AddWithValue("@AppId", appid);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();
                }
            }
        }

        public List<Creditcardassociations> GetCreditcardassociations()
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[GetCreditcardassociations]";

                    com.Connection = cnn;
                    cnn.Open();

                    List<Creditcardassociations> resultlist = new List<Creditcardassociations>();

                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Creditcardassociations result = new Creditcardassociations();

                            result.CardassociationsId = Convert.ToInt32(rd["CardassociationsId"]);
                            result.CardassociationsType = Convert.ToString(rd["CardassociationsType"]);

                            resultlist.Add(result);
                        }

                        return resultlist;
                    }


                }
            }
        }

        public List<InterestLang> GetInterestList(int langId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[GetInterest]";

                    com.Parameters.AddWithValue("@LangId", langId);

                    com.Connection = cnn;
                    cnn.Open();

                    List<InterestLang> resultlist = new List<InterestLang>();

                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            InterestLang result = new InterestLang();

                            result.InterestId = Convert.ToInt32(rd["InterestId"]);
                            result.InterestName = Convert.ToString(rd["InterestName"]);
                            result.InterestDesc = Convert.ToString(rd["InterestDesc"]);

                            resultlist.Add(result);
                        }

                        return resultlist;
                    }


                }
            }
        }

        public List<User> GetInterestList()
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[GetAllUser]";

                    com.Connection = cnn;
                    cnn.Open();

                    List<User> resultlist = new List<User>();

                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            User result = new User();

                            result.UserId = Convert.ToInt32(rd["UserId"]);
                            result.FirstName = Convert.ToString(rd["FirstName"]);
                            result.LastName = Convert.ToString(rd["LastName"]);

                            resultlist.Add(result);
                        }

                        return resultlist;
                    }


                }
            }
        }









        //public List<ItemDlgCard> GetItemDlgCardByItemTypeId(int itemTypeId, int startIndex, int? maxRetriveItem, string sortColumnName, int? sortDirection, string searchTex,int? itemId)
        //{
        //    using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
        //    {
        //        using (SqlCommand com = new SqlCommand())
        //        {
        //            com.CommandType = System.Data.CommandType.StoredProcedure;
        //            com.CommandText = "[dbo].[GetItemDlgCardByItemTypeId]";

        //            com.Parameters.AddWithValue("@ItemId", itemId);
        //            com.Parameters.AddWithValue("@ItemTypeId", itemTypeId);
        //            com.Parameters.AddWithValue("@StartIndex", startIndex);
        //            com.Parameters.AddWithValue("@MaxRetriveItem", maxRetriveItem);
        //            com.Parameters.AddWithValue("@SortColumnName", sortColumnName);
        //            com.Parameters.AddWithValue("@SortDirection", sortDirection);
        //            com.Parameters.AddWithValue("@SearchText", searchTex);

        //            com.Connection = cnn;
        //            cnn.Open();

        //            List<ItemDlgCard> listItemDlgCard = new List<ItemDlgCard>();

        //            using (SqlDataReader rd = com.ExecuteReader())
        //            {
        //                while (rd.Read())
        //                {
        //                    ItemDlgCard temDlgCard = new ItemDlgCard();

        //                    if (rd["ItemId"] != DBNull.Value)
        //                    temDlgCard.ItemId = Convert.ToInt32(rd["ItemId"]);

        //                    temDlgCard.ItemCode = Convert.ToString(rd["ItemCode"]);

        //                    if (rd["UserId"] != DBNull.Value)
        //                    temDlgCard.UserId = Convert.ToInt32(rd["UserId"]);

        //                    temDlgCard.ImageName = Convert.ToString(rd["ImageName"]);

        //                    temDlgCard.ToltalItemCount = Convert.ToInt32(rd["ToltalItemCount"]);

        //                    listItemDlgCard.Add(temDlgCard);

        //                }
        //            }

        //            return listItemDlgCard;
        //        }
        //    }
        //}


    }
}
