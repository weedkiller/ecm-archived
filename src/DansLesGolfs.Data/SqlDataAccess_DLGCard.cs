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
        public List<DLGCard> GetItemDlgCardByItemTypeId(int itemTypeId, int startIndex, int? maxRetriveItem, string sortColumnName, int? sortDirection, string searchTex, int? itemId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[GetItemDlgCardByItemTypeId]";

                    com.Parameters.AddWithValue("@ItemId", itemId);
                    com.Parameters.AddWithValue("@ItemTypeId", itemTypeId);
                    com.Parameters.AddWithValue("@StartIndex", startIndex);
                    com.Parameters.AddWithValue("@MaxRetriveItem", maxRetriveItem);
                    com.Parameters.AddWithValue("@SortColumnName", sortColumnName);
                    com.Parameters.AddWithValue("@SortDirection", sortDirection);
                    com.Parameters.AddWithValue("@SearchText", searchTex);

                    com.Connection = cnn;
                    cnn.Open();

                    List<DLGCard> listItemDlgCard = new List<DLGCard>();

                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            DLGCard temDlgCard = new DLGCard();

                            temDlgCard.ItemId = DataManager.ToInt(rd["ItemId"]);

                            temDlgCard.ItemCode = DataManager.ToString(rd["ItemCode"]);

                            temDlgCard.UserId = DataManager.ToInt(rd["UserId"]);

                            temDlgCard.ImageName = DataManager.ToString(rd["ImageName"]);

                            temDlgCard.ToltalItemCount = DataManager.ToInt(rd["ToltalItemCount"]);

                            temDlgCard.PersonalMessage = DataManager.ToString(rd["Message"]);

                            listItemDlgCard.Add(temDlgCard);

                        }
                    }

                    return listItemDlgCard;
                }
            }
        }

        public List<ItemPriceDlgCard> GetItemPriceDlgCard(int itemId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[GetItemPriceDlgCard]";

                    com.Parameters.AddWithValue("@ItemId", itemId);

                    com.Connection = cnn;
                    cnn.Open();

                    List<ItemPriceDlgCard> itemPriceDlgCardList = new List<ItemPriceDlgCard>();

                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            ItemPriceDlgCard itemPriceDlgCard = new ItemPriceDlgCard();

                            if (rd["ItemPriceId"] != DBNull.Value)
                                itemPriceDlgCard.ItemPriceId = Convert.ToInt32(rd["ItemPriceId"]);

                            if (rd["ItemId"] != DBNull.Value)
                                itemPriceDlgCard.ItemId = Convert.ToInt32(rd["ItemId"]);

                            if (rd["SiteId"] != DBNull.Value)
                                itemPriceDlgCard.SiteId = Convert.ToInt32(rd["SiteId"]);

                            if (rd["CustomerTypeId"] != DBNull.Value)
                                itemPriceDlgCard.CustomerTypeId = Convert.ToInt32(rd["CustomerTypeId"]);

                            if (rd["StartDate"] != DBNull.Value)
                                itemPriceDlgCard.StartDate = Convert.ToDateTime(rd["StartDate"]);

                            if (rd["EndDate"] != DBNull.Value)
                                itemPriceDlgCard.EndDate = Convert.ToDateTime(rd["EndDate"]);

                            if (rd["Price"] != DBNull.Value)
                                itemPriceDlgCard.Price = Convert.ToDouble(rd["Price"]);

                            if (rd["UserId"] != DBNull.Value)
                                itemPriceDlgCard.UserId = Convert.ToInt32(rd["UserId"]);


                            itemPriceDlgCardList.Add(itemPriceDlgCard);
                        }
                    }

                    return itemPriceDlgCardList;
                }
            }
        }

        public List<DLGCardStyle> GetDLGCardStyle(int itemId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[GetDLGCardStyle]";

                    com.Parameters.AddWithValue("@ItemId", itemId);

                    com.Connection = cnn;
                    cnn.Open();

                    List<DLGCardStyle> dlgcardstyleList = new List<DLGCardStyle>();

                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            DLGCardStyle dlgcardstyle = new DLGCardStyle();

                            if (rd["CardStyleId"] != DBNull.Value)
                                dlgcardstyle.CardStyleId = Convert.ToInt32(rd["CardStyleId"]);

                            dlgcardstyle.ImageName = Convert.ToString(rd["ImageName"]);

                            if (rd["ListNo"] != DBNull.Value)
                                dlgcardstyle.ListNo = Convert.ToInt32(rd["ListNo"]);

                            dlgcardstyleList.Add(dlgcardstyle);

                        }

                        return dlgcardstyleList;
                    }
                }
            }

        }

        public void SaveDlgCard(int itemId, string firstname, string lastname, string email, string personalMessage, int itemPriceDlgCardId, int userId, int dlgCardStyleId, string cardNumber, int orderId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[SaveDLGCard]";

                    com.Parameters.AddWithValue("@ItemId", itemId);
                    com.Parameters.AddWithValue("@SaleId", orderId);
                    com.Parameters.AddWithValue("@FirstName", firstname);
                    com.Parameters.AddWithValue("@LastName", lastname);
                    com.Parameters.AddWithValue("@Email", email);
                    com.Parameters.AddWithValue("@CardNumber", cardNumber);
                    com.Parameters.AddWithValue("@Message", personalMessage);
                    com.Parameters.AddWithValue("@itemPriceDlgCardId", itemPriceDlgCardId);
                    com.Parameters.AddWithValue("@UserId", userId);
                    com.Parameters.AddWithValue("@SelectedCardStyleId", dlgCardStyleId);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();
                }
            }

        }


        public int SaveDlgCardItemAdmin(AddDlgCard dlgmodel)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[SaveDlgCardItemAdmin]";

                    com.Parameters.AddWithValue("@CardName", dlgmodel.CardName);
                    com.Parameters.AddWithValue("@UserId", dlgmodel.UserId);

                    if (dlgmodel.ItemId.HasValue)
                        com.Parameters.AddWithValue("@ItemIdIn", dlgmodel.ItemId);

                    SqlParameter itemId = new SqlParameter();
                    itemId.Direction = System.Data.ParameterDirection.Output;
                    itemId.ParameterName = "@ItemId";
                    itemId.SqlDbType = System.Data.SqlDbType.Int;
                    com.Parameters.Add(itemId);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();

                    return Convert.ToInt32(itemId.Value);
                }
            }
        }

        public void SavePriceDlgCardItemAdmin(int itemId, float price, int userId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[SavePriceDlgCardItemAdmin]";

                    com.Parameters.AddWithValue("@ItemId", itemId);
                    com.Parameters.AddWithValue("@Price", price);
                    com.Parameters.AddWithValue("@UserId", userId);


                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();

                }
            }
        }

        public void SavePrimaryImageDlgCardAdmin(int itemId, string imageName, string baseName, string fileExtension)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[SavePrimaryImageDlgCardAdmin]";

                    com.Parameters.AddWithValue("@ItemId", itemId);
                    com.Parameters.AddWithValue("@ImageName", imageName);
                    com.Parameters.AddWithValue("@BaseName", baseName);
                    com.Parameters.AddWithValue("@FileExtension", fileExtension);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();

                }
            }
        }

        public void SaveStyleImageDlgCardAdmin(int itemId, string imageName, int listNo)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[SaveStyleImageDlgCardAdmin]";

                    com.Parameters.AddWithValue("@ItemId", itemId);
                    com.Parameters.AddWithValue("@ImageName", imageName);
                    com.Parameters.AddWithValue("@ListNo", listNo);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();
                }
            }
        }

        public void DeleteImageDlgCardAdmin(int itemId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[DeleteImageDlgCardAdmin]";

                    com.Parameters.AddWithValue("@ItemId", itemId);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();
                }
            }
        }

        public void DeletePriceDlgCardItemAdmin(int itemId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[DeletePriceDlgCardItemAdmin]";

                    com.Parameters.AddWithValue("@ItemId", itemId);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();

                }
            }
        }

        public int DeleteDlgCardAdmin(int itemId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[DeleteDlgCardAdmin]";

                    com.Parameters.AddWithValue("@ItemId", itemId);

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

        public Int64 RandomDigit()
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[RandomDigit]";

                    SqlParameter result = new SqlParameter();
                    result.Direction = System.Data.ParameterDirection.Output;
                    result.ParameterName = "@randigit";
                    result.SqlDbType = System.Data.SqlDbType.BigInt;
                    com.Parameters.Add(result);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();

                    return Convert.ToInt64(result.Value);
                }
            }
        }

        public List<DlgCardHistory> GetDlgCardHistory()
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[GetDlgCardHistory]";

                    com.Connection = cnn;
                    cnn.Open();

                    List<DlgCardHistory> dlgcardhistorylist = new List<DlgCardHistory>();
                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {

                            DlgCardHistory result = new DlgCardHistory();
                            result.DlgCardId = Convert.ToInt32(rd["DlgCardId"]);
                            result.ItemId = Convert.ToInt32(rd["ItemId"]);
                            result.SaleId = Convert.ToInt32(rd["SaleId"]);
                            result.FirstName = Convert.ToString(rd["FirstName"]);
                            result.LastName = Convert.ToString(rd["LastName"]);
                            result.Email = Convert.ToString(rd["Email"]);
                            result.CardNumber = Convert.ToString(rd["CardNumber"]);
                            result.Message = Convert.ToString(rd["Message"]);
                            result.BeginBalance = float.Parse(rd["BeginBalance"].ToString());
                            result.InsertDate = Convert.ToDateTime(rd["InsertDate"]);
                            result.UpdateDate = Convert.ToDateTime(rd["UpdateDate"]);
                            result.UserId = Convert.ToInt32(rd["UserId"]);
                            result.SelectedCardStyleId = Convert.ToInt32(rd["SelectedCardStyleId"]);

                            dlgcardhistorylist.Add(result);
                        }
                    }

                    return dlgcardhistorylist;

                }
            }

        }


        public List<DLGCardBalanceAdmin> DLGCardBalanceById(int dlgcardId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[DLGCardBalanceById]";

                    com.Parameters.AddWithValue("@DlgcardId", dlgcardId);

                    com.Connection = cnn;
                    cnn.Open();

                    List<DLGCardBalanceAdmin> dlgcardbalanceadmin = new List<DLGCardBalanceAdmin>();

                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            DLGCardBalanceAdmin result = new DLGCardBalanceAdmin();
                            result.Id = Convert.ToInt32(rd["Id"]);
                            result.DLGCardId = Convert.ToInt32(rd["DLGCardId"]);
                            result.UserId = Convert.ToInt32(rd["UserId"]);
                            result.ActionType = Convert.ToInt32(rd["ActionType"]);
                            result.Description = Convert.ToString(rd["Description"]);
                            result.Debit = Convert.ToDecimal(rd["Debit"].ToString());
                            result.Credit = Convert.ToDecimal(rd["Credit"].ToString());
                            result.Balance = Convert.ToDecimal(rd["Balance"].ToString());
                            result.InsertDate = Convert.ToDateTime(rd["InsertDate"]);
                            result.SaleId = Convert.ToInt32(rd["SaleId"]);

                            dlgcardbalanceadmin.Add(result);
                        }
                    }

                    return dlgcardbalanceadmin;
                }
            }
        }







    }
}
