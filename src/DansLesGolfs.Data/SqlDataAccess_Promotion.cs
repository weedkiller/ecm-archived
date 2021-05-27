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

        public int SavePromotion(string promotionContent, string promotionTimecontent, int brandNameId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[AddPromotion]";

                    com.Parameters.AddWithValue("@PromotionContent", promotionContent);
                    com.Parameters.AddWithValue("@PromotionTimecontent", promotionTimecontent);
                    com.Parameters.AddWithValue("@BrandNameId", brandNameId);


                    SqlParameter PromotionId = new SqlParameter();
                    PromotionId.Direction = System.Data.ParameterDirection.Output;
                    PromotionId.ParameterName = "@PromotionId";
                    PromotionId.SqlDbType = System.Data.SqlDbType.Int;
                    com.Parameters.Add(PromotionId);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();

                    return Convert.ToInt32(PromotionId.Value);
                }
            }
        }

        public void UpdatePromotion(int? promotionId, string promotionImage, string promotionBrandImage, string promotionContent, string promotionTimecontent, int brandNameId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[UpdatePromotion]";

                    com.Parameters.AddWithValue("@PromotionContent", promotionContent);
                    com.Parameters.AddWithValue("@PromotionTimecontent", promotionTimecontent);
                    com.Parameters.AddWithValue("@BrandNameId", brandNameId);

                    if (!string.IsNullOrEmpty(promotionImage)) 
                    com.Parameters.AddWithValue("@PromotionImage", promotionImage);
                    if (!string.IsNullOrEmpty(promotionBrandImage)) 
                    com.Parameters.AddWithValue("@PromotionBrandImage", promotionBrandImage);

                    com.Parameters.AddWithValue("@PromotionId", promotionId);

                    com.Connection = cnn;
                    cnn.Open();
                    com.ExecuteNonQuery();

                }
            }
        }

        public PromotionModel GetPromotion(int? promotionId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[GetPromotion]";

                    com.Parameters.AddWithValue("@PromotionId", promotionId);


                    com.Connection = cnn;
                    cnn.Open();

                    PromotionModel result = new PromotionModel();

                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            result.PromotionId = Convert.ToInt32(rd["PromotionId"]);
                            result.PromotionImage = Convert.ToString(rd["PromotionImage"]);
                            result.PromotionBrandImage = Convert.ToString(rd["PromotionBrandImage"]);
                            result.PromotionContent = Convert.ToString(rd["PromotionContent"]);
                            result.PromotionTimecontent = Convert.ToString(rd["PromotionTimecontent"]);
                            result.BrandNameId = Convert.ToInt32(rd["BrandNameId"]); 
                        }
                    }

                    return result;

                }

            }
        }

        public List<PromotionModel> GetAllPromotion(int startIndex, int? maxRetriveItem, string sortColumnName, int? sortDirection, string searchTex)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[GetAllPromotion]";

                    com.Parameters.AddWithValue("@StartIndex", startIndex);
                    com.Parameters.AddWithValue("@MaxRetriveItem", maxRetriveItem);
                    com.Parameters.AddWithValue("@SortColumnName", sortColumnName);
                    com.Parameters.AddWithValue("@SortDirection", sortDirection);
                    com.Parameters.AddWithValue("@SearchText", searchTex);

                    com.Connection = cnn;
                    cnn.Open();

                    List<PromotionModel> resultlist = new List<PromotionModel>();

                    using (SqlDataReader rd = com.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            PromotionModel result = new PromotionModel();

                            result.PromotionId = Convert.ToInt32(rd["PromotionId"]);
                            result.PromotionImage = Convert.ToString(rd["PromotionImage"]);
                            result.PromotionBrandImage = Convert.ToString(rd["PromotionBrandImage"]);
                            result.PromotionContent = Convert.ToString(rd["PromotionContent"]);
                            result.PromotionTimecontent = Convert.ToString(rd["PromotionTimecontent"]);
                            result.BrandNameId = Convert.ToInt32(rd["BrandNameId"]);
                            result.BrandName = Convert.ToString(rd["BrandName"]);

                            result.ToltalItemCount = Convert.ToInt32(rd["ToltalItemCount"]);

                            resultlist.Add(result);

                        }
                    }

                    return resultlist;

                }

            }
        }

        public int DeletePromotionAdmin(int promotionId)
        {
            using (SqlConnection cnn = new SqlConnection(this.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[DeletePromotionAdmin]";

                    com.Parameters.AddWithValue("@PromotionId", promotionId);

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

    }
}
