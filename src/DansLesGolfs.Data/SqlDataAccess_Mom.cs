using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DansLesGolfs.BLL;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using DansLesGolfs.Base;

namespace DansLesGolfs.Data
{
    public partial class SqlDataAccess
    {

        #region Interested

        //public List<Interest> GetAllInterested()
        //{
        //    List<Interest> list = new List<Interest>();

        //    //string sql = "SELECT * FROM [Interest] JOIN InterestLang ON InterestLang.InterestId = Interest.InterestId WHERE [Active] = 1 ORDER BY Interest.InterestId DESC";
        //    //DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    //foreach (DataRow row in ds.Tables[0].Rows)
        //    //{
        //    //    list.Add(new Interest(row));
        //    //}

        //    return list;
        //}

        //public List<UserInterested> GetUserInterestedByUserId(int userId)
        //{
        //    List<UserInterested> list = new List<UserInterested>();

        //    string sql = @"SELECT * FROM [UserInterested] 
        //                 WHERE [UserId] = @UserId";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //         new SqlParameter("@UserId", userId));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new UserInterested(row));
        //    }

        //    return list;
        //}

        //public int AddUserInterested(int[] interestIds, int userId)
        //{
        //    for (int i = 0; i < interestIds.Length; i++)
        //    {
        //        string sql = @"INSERT INTO [UserInterested]([InterestId], [UserId]) VALUES(@InterestId, @UserId)";

        //        DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //           new SqlParameter("@InterestId", interestIds[i]),
        //           new SqlParameter("@UserId", userId)));
        //    }

        //    return 1;
        //}

        //public int DeleteUserInterested(int userId)
        //{
        //    string sql = "DELETE [UserInterested] WHERE [UserId] = @UserId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@UserId", userId));
        //}

        #endregion

        #region Sponsor Email

        //public List<Site> GetAllSponsorEmail()
        //{
        //    List<Site> list = new List<Site>();
        //    string sql = "SELECT [Site].*, [SiteLang].[SiteName] FROM [Site] LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] WHERE [Active] = 1 AND [LangId] = " + langId + " ORDER BY [SiteName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Site(row));
        //    }
        //    return list;
        //}

        //public Site GetSponsorEMailById(int id)
        //{
        //    Site obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectSiteById",
        //        new SqlParameter("@SiteId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new Site(ds.Tables[0].Rows[0]);
        //        obj.SiteName = DataManager.ToString(ds.Tables[0].Rows[0]["SiteName"]);
        //        obj.Description = DataManager.ToString(ds.Tables[0].Rows[0]["Description"]);
        //    }

        //    return obj;
        //}

        //public List<SponsorEmail> GetAllSponsorEmail(int? fromUserId)
        //{
        //    List<SponsorEmail> list = new List<SponsorEmail>();
        //    string sql = "SELECT * FROM SponsorEmail WHERE Active = 1 AND FromUserId = " + fromUserId;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new SponsorEmail(row));
        //    }
        //    return list;
        //}

        //public int GetTotalSponsorEmailsByUserId(int fromUserId)
        //{
        //    List<SponsorEmail> list = new List<SponsorEmail>();
        //    string sql = "SELECT COUNT([SponsorEmailId]) FROM [SponsorEmail] WHERE [Active] = 1 AND [FromUserId] = " + fromUserId;
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql));
        //}

        //public SponsorEmail GetSponsorEmail()
        //{
        //    SponsorEmail obj = null;

        //    string sSql = "SELECT * FROM SponsorEmail WHERE Active = 1";
        //    DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new SponsorEmail(ds.Tables[0].Rows[0]);
        //        obj.SponsorEmailId = DataManager.ToInt(ds.Tables[0].Rows[0]["SponsorEmailId"]);
        //        obj.Subject = DataManager.ToString(ds.Tables[0].Rows[0]["Subject"]);
        //        obj.Body = DataManager.ToString(ds.Tables[0].Rows[0]["Body"]);
        //    }

        //    return obj;
        //}

        //public int AddSponsorEmail(SponsorEmail obj)
        //{
        //    string sql = @"INSERT INTO [SponsorEmail]([Subject], [Body],[FromUserId],[ToEmail],[InsertDate],[Active]) 
        //                 VALUES(@Subject, @Body,@FromUserId,@ToEmail,@InsertDate,@Active) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@Subject", obj.Subject),
        //       new SqlParameter("@Body", obj.Body),
        //       new SqlParameter("@FromUserId", obj.FromuserId),
        //       new SqlParameter("@ToEmail", obj.ToEmail),
        //       new SqlParameter("@InsertDate", DateTime.Now),
        //       new SqlParameter("@Active", 1)));
        //}

        //public int UpdateSponsorEmail(SponsorEmail obj)
        //{
        //    string sql = "UPDATE [SponsorEmail] SET [Subject] = @Subject, [Body] = @Body, [UpdateDate] = GETDATE() WHERE SponsorEmailId = @SponsorEmailId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@SponsorEmailId", obj.SponsorEmailId),
        //       new SqlParameter("@Subject", obj.Subject),
        //       new SqlParameter("@Body", obj.Body));
        //}

        //public int DeleteSponsorEmail(int id)
        //{
        //    string sql = "UPDATE [SponsorEmail] SET [Active] = 0 WHERE [SponsorEmailId] = @SponsorEmailId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@SponsorEmailId", id));
        //}

        #endregion

        #region Web Content
        //public List<WebContent> GetAllWebContent(jQueryDataTableParamModel param, int langId = 1)
        //{
        //    List<WebContent> list = new List<WebContent>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectWebContentsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@LangId", langId));

        //    WebContent item = null;
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        item = new WebContent(row);
        //        item.TopicName = DataManager.ToString(row["TopicName"]);
        //        list.Add(item);
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}

        //public List<WebContent> GetAllWebContent(string categoryName = "", int langId = 1)
        //{
        //    List<WebContent> list = new List<WebContent>();

        //    string sql = @"SELECT w.*, wl.TopicName FROM [WebContent] w
        //                  LEFT JOIN [WebContentLang] wl ON wl.[ContentId] = w.[ContentId]
        //                 WHERE w.[Active] = 1 AND wl.[LangId] = " + langId;
        //    if (!String.IsNullOrEmpty(categoryName))
        //    {
        //        sql += " AND [ContentCategory] = '" + categoryName + "'";
        //    }
        //    sql += " ORDER BY [OrderNumber] asc, [TopicName], [ContentId]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    WebContent content = null;
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        content = new WebContent(row);
        //        content.TopicName = DataManager.ToString(row["TopicName"]);
        //        list.Add(content);
        //    }

        //    return list;
        //}

        //public int AddWebContent(WebContent obj)
        //{
        //    string sql = @"INSERT INTO [WebContent] ([ContentCategory], [ContentKey], [Active], [UserId], [InsertDate]) 
        //                 VALUES(@ContentCategory, @ContentKey, @Active, @UserId, @InsertDate) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@ContentCategory", obj.ContentCategory),
        //       new SqlParameter("@ContentKey", obj.ContentKey),
        //       new SqlParameter("@Active", obj.Active),
        //       new SqlParameter("@UserId", obj.UserId),
        //       new SqlParameter("@InsertDate", obj.InsertDate)));

        //}

        //public int UpdateWebContent(WebContent obj)
        //{
        //    string sql = @"UPDATE [WebContent] SET 
        //                [ContentCategory] = @ContentCategory,
        //                [ContentKey] = @ContentKey,
        //                [Active] = @Active,
        //                [UserId] = @UserId,
        //                [InsertDate] = @InsertDate
        //                WHERE ContentId = @ContentId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@ContentId", obj.ContentId),
        //       new SqlParameter("@ContentCategory", obj.ContentCategory),
        //       new SqlParameter("@ContentKey", obj.ContentKey),
        //       new SqlParameter("@Active", obj.Active),
        //       new SqlParameter("@UserId", obj.UserId),
        //       new SqlParameter("@InsertDate", obj.InsertDate));
        //}

        //public List<ItemTypeCategory> GetWebContentCategory(int langId)
        //{
        //    List<ItemTypeCategory> list = new List<ItemTypeCategory>();
        //    string sql = "";
        //    if (langId == 2)
        //    {
        //         sql = @"select Item.ItemTypeId,ItemLang.ItemName,ItemTypeNameFr as ItemTypeName,Item.ItemSlug
        //                FROM Item
        //                inner join ItemLang on Item.ItemId = ItemLang.ItemId
        //                LEFT OUTER join ItemCategory ON Item.CategoryId = ItemCategory.CategoryId 
        //                INNER JOIN ItemType ON Item.ItemTypeId = ItemType.ItemTypeId 
        //                where Item.Active = 1 
        //                AND Item.ItemTypeId <> 1
        //                AND ItemLang.LangId =" + langId +
        //                    " ORDER BY Item.ItemTypeId, Item.ItemId desc";
        //    }
        //    else
        //    {
        //        sql = @"select Item.ItemTypeId,ItemLang.ItemName,ItemTypeNameFr as ItemTypeName,Item.ItemSlug
        //                FROM Item
        //                inner join ItemLang on Item.ItemId = ItemLang.ItemId
        //                LEFT OUTER join ItemCategory ON Item.CategoryId = ItemCategory.CategoryId 
        //                INNER JOIN ItemType ON Item.ItemTypeId = ItemType.ItemTypeId 
        //                where Item.Active = 1 
        //                AND Item.ItemTypeId <> 1
        //                AND ItemLang.LangId =" + langId +
        //                   " ORDER BY Item.ItemTypeId, Item.ItemId desc";
        //    }

        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //            new SqlParameter("@LangId", langId));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemTypeCategory(row)
        //        {
        //            ItemName = DataManager.ToString(row["ItemName"]),
        //            ItemSlug = DataManager.ToString(row["ItemSlug"])
        //        });
        //    }
        //    return list;
        //}

        //public WebContent GetWebContentById(long id)
        //{
        //    WebContent obj = null;
        //    string sql = @"SELECT * FROM [WebContent] 
        //                 WHERE WebContent.ContentId = @ContentId;

        //                 SELECT * FROM [WebContentLang]
        //                 WHERE [ContentId] = @ContentId ORDER BY [LangId]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ContentId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new WebContent(ds.Tables[0].Rows[0]);
        //    }

        //    obj.WebContentLangs = new List<WebContentLang>();
        //    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[1].Rows)
        //        {
        //            obj.WebContentLangs.Add(new WebContentLang(row));
        //        }
        //    }

        //    return obj;
        //}

        //public WebContent GetWebContentByKey(string contentKey, int langId = 1, string contentCategory = "")
        //{
        //    WebContent obj = null;
        //    string sql = @"SELECT wc.ContentId, wc.ContentCategory, wc.ContentKey, ISNULL(wcl.TopicName, 'Untitled') AS TopicName, ISNULL(wcl.ContentText, '') AS ContentText, wc.InsertDate, wc.UserId FROM [WebContent] wc
        //                 INNER JOIN (SELECT ContentId, TopicName, ContentText FROM [WebContentLang] WHERE [LangId] = @LangId) wcl ON wcl.[ContentId] = wc.[ContentId]
        //                 WHERE wc.[ContentKey] = @ContentKey";
        //    if(!String.IsNullOrEmpty(contentCategory))
        //    {
        //        sql += " AND wc.[ContentCategory] = @ContentCategory";
        //    }
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ContentCategory", contentCategory),
        //        new SqlParameter("@ContentKey", contentKey),
        //        new SqlParameter("@LangId", langId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        DataRow row = ds.Tables[0].Rows[0];
        //        obj = new WebContent()
        //        {
        //            ContentId = DataManager.ToInt(row["ContentId"]),
        //            ContentCategory = DataManager.ToString(row["ContentCategory"]),
        //            ContentKey = DataManager.ToString(row["ContentKey"]),
        //            InsertDate = DataManager.ToDateTime(row["InsertDate"]),
        //            UserId = DataManager.ToInt(row["UserId"]),
        //            TopicName = DataManager.ToString(row["TopicName"]),
        //            ContentText = DataManager.ToString(row["ContentText"])
        //        };
        //    }
        //    else
        //    {
        //        obj = new WebContent()
        //        {
        //            ContentId = -1,
        //            ContentCategory = contentCategory,
        //            ContentKey = contentKey,
        //            InsertDate = DateTime.Now,
        //            UserId = -1,
        //            TopicName = string.Empty,
        //            ContentText = string.Empty
        //        };
        //    }

        //    return obj;
        //}

        //public int AddWebContentLang(WebContentLang obj)
        //{
        //    string sql = @"INSERT INTO [WebContentLang] ([ContentId], [LangId], [TopicName], [ContentText], [UserId], [UpdateDate]) 
        //                 VALUES(@ContentId, @LangId, @TopicName, @ContentText, @UserId, @UpdateDate) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@ContentId", obj.ContentId),
        //       new SqlParameter("@LangId", obj.LangId),
        //       new SqlParameter("@TopicName", obj.TopicName),
        //       new SqlParameter("@UserId", obj.UserId),
        //       new SqlParameter("@ContentText", obj.ContentText),
        //       new SqlParameter("@UpdateDate", obj.UpdateDate)));

        //}

        //public int UpdateWebContentLang(WebContentLang obj)
        //{
        //    string sql = @"UPDATE [WebContentLang] SET 
        //                [ContentId] = @ContentId, 
        //                [LangId] = @LangId, 
        //                [TopicName] = @TopicName, 
        //                [ContentText] = @ContentText, 
        //                [UserId] = @UserId, 
        //                [UpdateDate] = @UpdateDate 
        //                WHERE ContentId = @ContentId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@ContentId", obj.ContentId),
        //       new SqlParameter("@LangId", obj.LangId),
        //       new SqlParameter("@TopicName", obj.TopicName),
        //       new SqlParameter("@ContentText", obj.ContentText),
        //       new SqlParameter("@UserId", obj.UserId),
        //       new SqlParameter("@UpdateDate", obj.UpdateDate));
        //}

        //public long DeleteWebContentLangByContentId(long id)
        //{
        //    string sql = "DELETE FROM [WebContentLang] WHERE [ContentId] = " + id;
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}


        #endregion
    }
}
