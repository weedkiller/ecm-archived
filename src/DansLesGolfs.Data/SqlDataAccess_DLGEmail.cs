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
        #region Users
        public bool IsExistsEmailOfECMUser(string email, long? skipId = null)
        {
            string sql = "SELECT COUNT(Email) FROM [Users] WHERE [Users].[Active] = 1 AND [Users].[Email]  = @Email AND [UserTypeId] IN(0, 1, 4, 5)";
            if (skipId.HasValue)
            {
                sql += " AND [UserId] <> " + skipId.Value;
            }
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@Email", email))) > 0;
        }
        #endregion

        #region Customer
        #endregion

        #region Customer Type

        public List<CustomerType> GetAllCustomerTypes(int langId = 1, long? siteId = null, long? parentId = null)
        {
            List<CustomerType> list = new List<CustomerType>();
            string sql = @"SELECT CustomerType.CustomerTypeId, CustomerTypeLang.CustomerTypeName FROM CustomerType
                           JOIN CustomerTypeLang ON CustomerTypeLang.CustomerTypeId = CustomerType.CustomerTypeId AND CustomerTypeLang.LangId = " + langId;
            sql += " WHERE 1 = 1";

            if (siteId.HasValue)
            {
                sql += " AND CustomerType.SiteId = " + siteId.Value;
            }

            if (parentId.HasValue)
            {
                sql += " AND ISNULL(CustomerType.ParentId, 0) = " + parentId.Value;
            }
            else
            {
                sql += " AND ISNULL(CustomerType.ParentId, 0) = 0";
            }
            sql += " ORDER BY CustomerTypeLang.CustomerTypeName";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new CustomerType()
                    {
                        CustomerTypeId = DataManager.ToLong(row["CustomerTypeId"]),
                        CustomerTypeName = DataManager.ToString(row["CustomerTypeName"])
                    });
                }
            }
            return list;
        }

        public IQueryable<CustomerTypeViewModel> GetListCustomerTypes(int langId = 1, long? siteId = null, long? parentId = null)
        {
            var db = GetModels();

            var query = from it in db.CustomerTypes
                        join ctl in db.CustomerTypeLangs.Where(it => it.LangId == langId) on it.CustomerTypeId equals ctl.CustomerTypeId into ctlj
                        from ctl in ctlj.DefaultIfEmpty()
                        join pctl in db.CustomerTypeLangs.Where(it => it.LangId == langId) on it.ParentId equals pctl.CustomerTypeId into pctlj
                        from pctl in pctlj.DefaultIfEmpty()
                        where it.CustomerTypeId > 0 && it.Active == true
                        select new CustomerTypeViewModel
                        {
                            CustomerTypeId = it.CustomerTypeId,
                            SiteId = it.SiteId,
                            ParentId = it.ParentId,
                            CustomerTypeName = ctl.CustomerTypeName,
                            ParentCustomerTypeName = pctl.CustomerTypeName
                        };

            if (siteId.HasValue)
            {
                query = query.Where(it => it.SiteId == siteId.Value);
            }

            if (parentId.HasValue)
            {
                query = query.Where(it => it.ParentId == parentId.Value);
            }

            return query.OrderBy(it => it.CustomerTypeName);
        }

        public List<CustomerType> GetAllNestedCustomerTypes(int langId = 1, long? siteId = null, long? skipId = null)
        {
            using (var db = GetModels())
            {
                var query = from it in db.CustomerTypes
                            join ctl in db.CustomerTypeLangs.Where(it => it.LangId == langId) on it.CustomerTypeId equals ctl.CustomerTypeId into ctlj
                            from ctl in ctlj.DefaultIfEmpty()
                            where it.Active == true
                            select new
                            {
                                CustomerType = it,
                                CustomerTypeName = ctl.CustomerTypeName
                            };

                if (siteId.HasValue)
                {
                    query = query.Where(it => it.CustomerType.SiteId == siteId.Value);
                }

                if (skipId.HasValue)
                {
                    query = query.Where(it => it.CustomerType.CustomerTypeId != skipId.Value);
                }

                foreach (var it in query)
                {
                    it.CustomerType.CustomerTypeName = it.CustomerTypeName;
                }

                return query.Select(it => it.CustomerType).ToList().OrderBy(it => it.CustomerTypeName).ToList();
            }
        }

        public int AddCustomerType(CustomerType obj)
        {
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                  "INSERT INTO CustomerType(SiteId, ParentId, Active, AffiliationTypeId, CreatedDate, UpdatedDate) VALUES(@SiteId, @ParentId, @Active, @AffiliationTypeId, @CreatedDate, @UpdatedDate) SELECT @@IDENTITY",
                  new SqlParameter("@SiteId", obj.SiteId),
                  new SqlParameter("@ParentId", obj.ParentId),
                  new SqlParameter("@Active", obj.Active),
                  new SqlParameter("@AffiliationTypeId", obj.AffiliationTypeId),
                  new SqlParameter("@CreatedDate", obj.CreatedDate),
                  new SqlParameter("@UpdatedDate", obj.UpdatedDate)), -1);
        }

        public int UpdateCustomerType(CustomerType obj)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                  "Update CustomerType Set [SiteId] = @SiteId, [ParentId] = @ParentId, [Active] = @Active, [UpdatedDate] = @UpdatedDate, [AffiliationTypeId] = @AffiliationTypeId Where CustomerTypeId = @CustomerTypeId",
                  new SqlParameter("@SiteId", obj.SiteId),
                  new SqlParameter("@ParentId", obj.ParentId),
                  new SqlParameter("@Active", obj.Active),
                  new SqlParameter("@AffiliationTypeId", obj.AffiliationTypeId),
                  new SqlParameter("@UpdatedDate", obj.UpdatedDate),
                  new SqlParameter("@CustomerTypeId", obj.CustomerTypeId));
        }

        public List<CustomerType> GetCustomerTypesDropDownList(int langId, int? parentId = null, long? siteId = null, long? skipId = null)
        {
            string sql = @"SELECT it.CustomerTypeId, cl.CustomerTypeName FROM CustomerType it
                           JOIN CustomerTypeLang cl ON cl.CustomerTypeId = it.CustomerTypeId AND cl.LangId = " + langId;

            sql += "WHERE it.Active = 1";

            if (siteId.HasValue)
            {
                sql += " AND it.SiteId = " + siteId.Value;
            }

            if (parentId.HasValue)
            {
                sql += " AND ISNULL(it.ParentId, 0) = " + parentId.Value;
            }
            else
            {
                sql += " AND ISNULL(it.ParentId, 0) = 0";
            }

            if (skipId.HasValue)
            {
                sql += " AND it.CustomerTypeId <> " + skipId.Value;
            }

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            List<CustomerType> customerTypes = new List<CustomerType>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                customerTypes.Add(new CustomerType()
                {
                    CustomerTypeId = DataManager.ToInt(row["CustomerTypeId"]),
                    CustomerTypeName = DataManager.ToString(row["CustomerTypeName"])
                });
            }

            return customerTypes;
        }

        public IQueryable<CustomerType> GetAllCustomerTypes(int? langId = null, long? siteId = null)
        {
            using (var db = GetModels())
            {
                var query = (from it in db.CustomerTypes
                             join ctl in db.CustomerTypeLangs.Where(ctl => ctl.LangId == langId) on it.CustomerTypeId equals ctl.CustomerTypeId into ctrlj
                             from ctl in ctrlj.DefaultIfEmpty()
                             select new
                             {
                                 CustomerType = it,
                                 CustomerTypeLang = ctl
                             });

                if (siteId.HasValue)
                {
                    query = query.Where(it => it.CustomerType.SiteId == siteId.Value);
                }

                foreach (var it in query)
                {
                    it.CustomerType.CustomerTypeName = it.CustomerTypeLang != null ? it.CustomerTypeLang.CustomerTypeName : "";
                }

                return query.Select(it => it.CustomerType);
            }
        }

        public CustomerType GetCustomerTypeById(long nCustomerTypeId, int langId = 1)
        {
            CustomerType obj = null;

            using (var db = GetModels())
            {
                var query = (from it in db.CustomerTypes
                             join ctl in db.CustomerTypeLangs.Where(ctl => ctl.LangId == langId) on it.CustomerTypeId equals ctl.CustomerTypeId into ctrlj
                             from ctl in ctrlj.DefaultIfEmpty()
                             where it.CustomerTypeId == nCustomerTypeId && it.Active == true
                             select new
                             {
                                 CustomerType = it,
                                 CustomerTypeLang = ctl
                             });

                foreach (var it in query)
                {
                    it.CustomerType.CustomerTypeName = it.CustomerTypeLang != null ? it.CustomerTypeLang.CustomerTypeName : "";
                }

                query.ToList().ForEach(it => obj = it.CustomerType);
            }

            return obj;
        }

        public List<CustomerType> GetCustomerTypesBySiteId(long siteId, int langId = 1)
        {
            List<CustomerType> list = null;

            using (var db = GetModels())
            {
                var query = (from it in db.CustomerTypes
                             join ctl in db.CustomerTypeLangs.Where(ctl => ctl.LangId == langId) on it.CustomerTypeId equals ctl.CustomerTypeId into ctrlj
                             from ctl in ctrlj.DefaultIfEmpty()
                             where it.SiteId == siteId && it.Active == true
                             select new
                             {
                                 CustomerType = it,
                                 CustomerTypeLang = ctl
                             });

                foreach (var it in query)
                {
                    it.CustomerType.CustomerTypeName = it.CustomerTypeLang != null ? it.CustomerTypeLang.CustomerTypeName : "";
                }

                list = query.Select(it => it.CustomerType).ToList();
            }

            return list;
        }

        public List<long> GetCustomerTypeIdsBySiteId(long groupId)
        {
            List<long> list = new List<long>();
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, @"SELECT CustomerTypeId FROM CustomerGroupCustomerType WHERE CustomerGroupId = " + groupId);
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(DataManager.ToLong(row[0]));
                }
            }

            return list;
        }


        public int? GetAffiliationTypeIdByCustomerTypeId(long customerTypeId)
        {
            int? affiliationTypeId = null;

            using (var db = GetModels())
            {
                affiliationTypeId = (from it in db.CustomerTypes
                                     where it.CustomerTypeId == customerTypeId && it.Active == true
                                     select it.AffiliationTypeId).SingleOrDefault();
            }

            return affiliationTypeId;
        }

        /// <summary>
        /// Sync customer types from Chronogolf API.
        /// </summary>
        /// <param name="siteId"></param>
        public void SyncCustomerTypes(long siteId)
        {
            int clubId = GetSiteChronogolfClubId(siteId);
            if (clubId <= 0)
                return;

            CustomerType customerType = null;
            var chronogolf = DataFactory.GetChronogolfInstance(clubId);
            List<ChronogolfAffiliateType> affiliationTypes = chronogolf.GetAllAffiliationTypes();

            using (var db = GetModels())
            {
                //if (!keep)
                //{
                //    DeleteCustomerTypeBySiteId(siteId);
                //}

                foreach (var a in affiliationTypes)
                {
                    customerType = (from it in db.CustomerTypes
                                    where it.AffiliationTypeId == a.id && it.SiteId == siteId
                                    select it).SingleOrDefault();

                    if (customerType == null)
                    {
                        customerType = new CustomerType();
                    }

                    // Save customer type.
                    customerType.AffiliationTypeId = a.id;
                    customerType.CustomerTypeName = a.name;
                    customerType.SiteId = siteId;
                    customerType.Active = true;
                    customerType.UpdatedDate = DateTime.Now;

                    if (customerType.CustomerTypeId <= 0)
                    {
                        customerType.CreatedDate = customerType.UpdatedDate;
                        customerType.CustomerTypeId = AddCustomerType(customerType);

                        // Save customer type lang.
                        SaveCustomerTypeLang(new CustomerTypeLang()
                        {
                            CustomerTypeId = customerType.CustomerTypeId,
                            CustomerTypeName = customerType.CustomerTypeName,
                            LangId = 1
                        });
                    }
                    else
                    {
                        UpdateCustomerType(customerType);

                        // Save customer type lang.
                        var langs = db.CustomerTypeLangs.Where(it => it.CustomerTypeId == customerType.CustomerTypeId);
                        if (langs.Any())
                        {
                            foreach (var lang in langs)
                            {
                                lang.CustomerTypeName = customerType.CustomerTypeName;
                                SaveCustomerTypeLang(lang);
                            }
                        }
                        else
                        {
                            SaveCustomerTypeLang(new CustomerTypeLang()
                            {
                                CustomerTypeId = customerType.CustomerTypeId,
                                CustomerTypeName = customerType.CustomerTypeName,
                                LangId = 1
                            });
                        }
                    }
                }
            }
        }

        public void DeleteCustomerTypeLangs(IEnumerable<long> customerTypeIds)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM CustomerTypeLang WHERE CustomerTypeId IN(" + String.Join(",", customerTypeIds) + ")");
        }

        public void RemoveCustomerTypeFromUser(IEnumerable<long> customerTypeIds)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "UPDATE [Users] SET [CustomerTypeId] = NULL WHERE CustomerTypeId IN(" + String.Join(",", customerTypeIds) + ")");
        }

        public int DeleteCustomerType(long nCustomerTypeId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "UPDATE CustomerType SET Active = 0 WHERE CustomerTypeId =" + nCustomerTypeId);
        }

        public int DeleteCustomerType(IEnumerable<long> nCustomerTypeIds)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "UPDATE CustomerType SET Active = 0 WHERE CustomerTypeId IN(" + String.Join(",", nCustomerTypeIds) + ")");
        }

        public int DeleteCustomerTypeBySiteId(long siteId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                   @"DELETE it FROM CustomerGroupCustomerType it
                    JOIN CustomerType ct ON ct.CustomerTypeId = it.CustomerTypeId
                    WHERE ct.SiteId = " + siteId);

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                   "UPDATE Users SET CustomerTypeId = NULL WHERE SiteId = " + siteId);

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                   @"DELETE it FROM CustomerTypeLang it
                    JOIN CustomerType ct ON ct.CustomerTypeId = it.CustomerTypeId
                    WHERE ct.SiteId = " + siteId);

            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "DELETE FROM CustomerType WHERE SiteId =" + siteId);
        }

        public int SaveCustomerTypeLang(CustomerTypeLang obj)
        {
            string sql = @"IF(EXISTS(SELECT [CustomerTypeId] FROM [CustomerTypeLang] WHERE [CustomerTypeId] = @CustomerTypeId AND [LangId] = @LangId))
                            BEGIN
                                UPDATE [CustomerTypeLang] SET [CustomerTypeName] = @CustomerTypeName WHERE [CustomerTypeId] = @CustomerTypeId AND [LangId] = @LangId;
                            END
                            ELSE
                            BEGIN
                                INSERT INTO [CustomerTypeLang]([CustomerTypeId], [LangId], [CustomerTypeName])
                                VALUES(@CustomerTypeId, @LangId, @CustomerTypeName) SELECT @@IDENTITY;
                            END";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CustomerTypeId", obj.CustomerTypeId),
                new SqlParameter("@LangId", obj.LangId),
                new SqlParameter("@CustomerTypeName", obj.CustomerTypeName));
        }

        public Dictionary<string, int> GetCustomerTypeIdLookupTable(long langId = 1, long? siteId = null)
        {
            Dictionary<string, int> table = new Dictionary<string, int>();
            string sql = @"SELECT DISTINCT CustomerTypeLang.CustomerTypeName, CustomerType.CustomerTypeId FROM CustomerType
                            JOIN CustomerTypeLang ON CustomerTypeLang.CustomerTypeId = CustomerType.CustomerTypeId AND CustomerTypeLang.LangId = " + langId;
            sql += " WHERE CustomerType.Active = 1";
            if (siteId.HasValue)
            {
                sql += " AND CustomerType.SiteId = " + siteId.Value;
            }
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (!table.ContainsKey(DataManager.ToString(row["CustomerTypeName"])))
                    table.Add(DataManager.ToString(row["CustomerTypeName"]), DataManager.ToInt(row["CustomerTypeId"]));
            }
            return table;
        }
        #endregion

        #region Permission

        public string GetModuleUrl(int nModuleId)
        {
            string sSql = "SELECT NavigateUrl FROM Module WHERE ModuleId = " + nModuleId.ToString();
            return DataManager.ToString(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));
        }

        public DataSet GetPermissionByUserTypeId(int nParentUserTypeId, int nUserTypeId, bool bIsMasterAdmin)
        {
            string sSql = "";

            if (bIsMasterAdmin)
            {
                sSql = "SELECT m.*, ISNULL(p.PermissionId, '0') as PermissionId, ISNULL(p.AllowedView, 0) as AllowedView, ISNULL(p.AllowedAdd, 0) as AllowedAdd, ISNULL(p.AllowedEdit, 0) as AllowedEdit, ISNULL(p.AllowedDelete, 0) as AllowedDelete, ISNULL(p.AllowedPrint, 0) as AllowedPrint, ISNULL(p.FullControl, 0) as FullControl ";
                sSql += " from vModule m left join Permission p ";
                sSql += " on m.ModuleId = p.ModuleId ";
                sSql += " and p.UserTypeId = " + nUserTypeId.ToString();
                sSql += " ORDER BY ModuleCategoryId";

                return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
            }
            else
            {
                sSql = "SELECT m.*, ISNULL(p.PermissionId, 0) as PermissionId, ISNULL(p.AllowedView, 0) as AllowedView, ISNULL(p.AllowedAdd, 0) as AllowedAdd, ISNULL(p.AllowedEdit, 0) as AllowedEdit, ISNULL(p.AllowedDelete, 0) as AllowedDelete, ISNULL(p.AllowedPrint, 0) as AllowedPrint, ISNULL(p.FullControl, 0) as FullControl, ISNULL(p2.AllowedAdd, 0) as AllowedAdd2, ISNULL(p2.AllowedEdit, 0) as AllowedEdit2, ISNULL(p2.AllowedDelete, 0) as AllowedDelete2, ISNULL(p2.AllowedPrint, 0) as AllowedPrint2 ";
                sSql += " from vModule m left join Permission p  on m.ModuleId = p.ModuleId and ISNULL(p.UserTypeId, @UserTypeId) = @UserTypeId left join Permission p2  on m.ModuleId = p2.ModuleId and ISNULL(p2.UserTypeId, @ParentUserTypeId) = @ParentUserTypeId ";
                sSql += " WHERE m.ModuleId IN (select ModuleId from Permission where UserTypeId = @ParentUserTypeId AND ISNULL(AllowedView, 0) = 1) ";
                sSql += " ORDER BY ModuleCategoryId ";

                return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql, new SqlParameter("@ParentUserTypeId", nParentUserTypeId), new SqlParameter("@UserTypeId", nUserTypeId));
            }

        }

        #endregion

        #region Category
        public IQueryable<Category> GetAllCategories(long? siteId = null)
        {
            using (var db = GetModels())
            {
                var query = db.Categories.Where(it => it.Active == true);

                if (siteId.HasValue)
                {
                    query = query.Where(it => it.SiteId == siteId.Value);
                }

                return query;
            }
        }
        public IQueryable<Category> GetAllCategories()
        {
            using (var db = GetModels())
            {
                return db.Categories.Where(it => it.Active == true);
            }
        }
        public string GetCategoryName(int nCategoryId)
        {
            return DataManager.ToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                " Select CategoryName From Category Where CategoryId=" + nCategoryId));
        }
        public bool IsExistCategoryName(string sCategoryName, int nCategoryId, int nContentType)
        {
            int nCount = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                string.Format("Select Count(*) From Category Where  Active=1 And CategoryId <>{0} And CategoryName ='{1}' And ContentType={2}", nCategoryId, sCategoryName, nContentType)), 0);

            if (nCount > 0)
                return true;
            else
                return false;

        }
        public bool IsExistCategoryName(string sCategoryName, int nContentType)
        {
            int nCount = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                string.Format("Select Count(*) From Category Where Active=1 And CategoryName ='{0}' And ContentType={1}", sCategoryName, nContentType)), 0);

            if (nCount > 0)
                return true;
            else
                return false;

        }
        public int DeleteCategory(long nCategoryId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Delete Category where CategoryId=" + nCategoryId);
        }
        public Category GetCategory(long nContentType)
        {
            Category obj = null;

            using (var db = GetModels())
            {
                db.Categories.Where(it => it.ContentType == nContentType).ToList().ForEach(it => obj = it);
            }

            return obj;
        }
        public DataSet GetCategory(int nContentType, string sCategoryname)
        {
            string sql = "Select *,Case Active when 1 then 'Aktive' else 'Inaktiv' end As ActiveName," +
                "	Case ContentType When 1 then 'Anzeige'  " +
                "	 When 2 then 'Infomation' " +
                "	 When 3 then 'Download'" +
                " end As ContentTypeName" +
                " From Category";

            sql += string.Format(" Where CategoryName Like '%{0}%'", sCategoryname);

            if (nContentType > 0)
                sql += string.Format(" And ContentType={0}", nContentType);


            sql += " Order by CategoryName";

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        }
        public DataSet GetCategoryActive()
        {
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text,
                "Select *,Case Active when 1 then 'Aktive' else 'Inaktiv' end As ActiveName From Category Where Active=1 Order by CategoryName");
        }
        public int AddCategory(Category pObj)
        {
            string sSql = "INSERT INTO Category ([CategoryName],  [Active],  [InsertDate],[ContentType])  	VALUES (@CategoryName,  @Active, GetDate(),@ContentType  ) SELECT @@Identity";

            return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@ContentType", pObj.ContentType),
                new SqlParameter("@CategoryName", pObj.CategoryName),
                new SqlParameter("@Active", pObj.Active)), 0);
        }

        public int UpdateCategory(Category pObj)
        {
            string sSql = "	UPDATE Category SET [CategoryName] = @CategoryName, [Active] = @Active, [UpdateDate] = GetDate(),ContentType=@ContentType Where [CategoryId] = @CategoryId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@CategoryId", pObj.CategoryId),
                new SqlParameter("@ContentType", pObj.ContentType),
                new SqlParameter("@CategoryName", pObj.CategoryName),
                new SqlParameter("@Active", pObj.Active));
        }

        public DataSet GetFileCategory(int iCType)
        {
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text,
                @"Select * From Category Where ContentType= " + iCType + " And Active=1 Order by ContentType,CategoryId");
        }

        public bool IsExistKategorie(int iUserId, int nCategoryId)
        {
            int nCount = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                string.Format("Select Count(*) From Katagorie Where  UserId={0}And CategoryId ={1}", iUserId, nCategoryId)), 0);

            if (nCount > 0)
                return true;
            else
                return false;

        }
        #endregion

        #region " Content"
        public int DeleteContent(int nContentId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Delete From Content Where ContentId=" + nContentId);
        }
        public Content GetContent(int nContentId)
        {
            Content obj = null;

            using (var db = GetModels())
            {
                db.Contents.Where(it => it.ContentId == nContentId).ToList().ForEach(it => obj = it);
            }

            return obj;
        }
        public DataSet GetContent(int nContentType, string sFilter)
        {
            string sql = string.Format("Select Content.*,Category.CategoryName From Content Inner Join Category On Category.CategoryId = Content.CategoryId Where Category.Active =1 {0}", sFilter);

            if (nContentType > 0)
                sql += " And Content.ContentType=" + nContentType;

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        }
        public DataSet GetInformation(string sql)
        {
            //string sql = string.Format("Select Content.*,Category.CategoryName From Content Inner Join Category On Category.CategoryId = Content.CategoryId Where Content.Active=1 And Content.ContentType={0} {1}", nContentType, sFilter);

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        }
        public int AddContent(Content pObj)
        {
            string sSql = @"INSERT INTO Content ([ContentType],  [CategoryId],  [Topic],  [ShortDetail],  
                        [Detail],  [TextDetail],  [FileName1],  [FileDescription1],  [FileName2],  [FileDescription2],  
                        [FileName3],  [FileDescription3],  [FileName4],  [FileDescription4],  
                        [FileName5],  [FileDescription5],  [Active],  [Private],  [AwayShow],  [InsertDate])  	
                        VALUES (@ContentType,  @CategoryId,  @Topic,  @ShortDetail,  
                        @Detail,  @TextDetail,  @FileName1,  @FileDescription1,  @FileName2,  @FileDescription2,  
                        @FileName3,  @FileDescription3,  @FileName4,  @FileDescription4,  
                        @FileName5,  @FileDescription5,  @Active,  @Private,  @AwayShow, GetDate() ) 
                        SELECT @@Identity";

            return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@ContentType", pObj.ContentType),
                new SqlParameter("@CategoryId", pObj.CategoryId),
                new SqlParameter("@Topic", pObj.Topic),
                new SqlParameter("@ShortDetail", pObj.ShortDetail),
                new SqlParameter("@Detail", pObj.Detail),
                new SqlParameter("@TextDetail", pObj.TextDetail),
                new SqlParameter("@FileName1", pObj.FileName1),
                new SqlParameter("@FileDescription1", pObj.FileDescription1),
                new SqlParameter("@FileName2", pObj.FileName2),
                new SqlParameter("@FileDescription2", pObj.FileDescription2),
                new SqlParameter("@FileName3", pObj.FileName3),
                new SqlParameter("@FileDescription3", pObj.FileDescription3),
                new SqlParameter("@FileName4", pObj.FileName4),
                new SqlParameter("@FileDescription4", pObj.FileDescription4),
                new SqlParameter("@FileName5", pObj.FileName5),
                new SqlParameter("@FileDescription5", pObj.FileDescription5),
                new SqlParameter("@Active", pObj.Active),
                new SqlParameter("@Private", pObj.Private),
                new SqlParameter("@AwayShow", pObj.AwayShow)), 0);
        }

        public int UpdateContent(Content pObj)
        {
            string sSql = @"	UPDATE Content SET [ContentType] = @ContentType, [CategoryId] = @CategoryId, 
                        [Topic] = @Topic, [ShortDetail] = @ShortDetail, [Detail] = @Detail, [TextDetail] = @TextDetail, 
                        [FileName1] = @FileName1, [FileDescription1] = @FileDescription1, 
                        [FileName2] = @FileName2, [FileDescription2] = @FileDescription2, 
                        [FileName3] = @FileName3, [FileDescription3] = @FileDescription3, 
                        [FileName4] = @FileName4, [FileDescription4] = @FileDescription4, 
                        [FileName5] = @FileName5, [FileDescription5] = @FileDescription5, [Active] = @Active, 
                        [Private] = @Private, [AwayShow] = @AwayShow, [UpdateDate] = GetDate() 
                        Where [ContentId] = @ContentId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@ContentId", pObj.ContentId),
                new SqlParameter("@ContentType", pObj.ContentType),
                new SqlParameter("@CategoryId", pObj.CategoryId),
                new SqlParameter("@Topic", pObj.Topic),
                new SqlParameter("@ShortDetail", pObj.ShortDetail),
                new SqlParameter("@Detail", pObj.Detail),
                new SqlParameter("@TextDetail", pObj.TextDetail),
                new SqlParameter("@FileName1", pObj.FileName1),
                new SqlParameter("@FileDescription1", pObj.FileDescription1),
                new SqlParameter("@FileName2", pObj.FileName2),
                new SqlParameter("@FileDescription2", pObj.FileDescription2),
                new SqlParameter("@FileName3", pObj.FileName3),
                new SqlParameter("@FileDescription3", pObj.FileDescription3),
                new SqlParameter("@FileName4", pObj.FileName4),
                new SqlParameter("@FileDescription4", pObj.FileDescription4),
                new SqlParameter("@FileName5", pObj.FileName5),
                new SqlParameter("@FileDescription5", pObj.FileDescription5),
                new SqlParameter("@Active", pObj.Active),
                new SqlParameter("@Private", pObj.Private),
                new SqlParameter("@AwayShow", pObj.AwayShow));
        }

        public bool IsExistFileName(string sFileName)
        {
            string sSql = @"Select count(ContentId) From Content Where FileName1 = '" + sFileName +
                        "' Or FileName2 = '" + sFileName +
                        "' Or FileName3 = '" + sFileName +
                        "' Or FileName4 = '" + sFileName +
                        "' Or FileName5 = '" + sFileName + "'";

            int nResult = DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

            if (nResult > 0)
                return true;
            else
                return false;
        }

        public bool IsExistFileNameInEmailTemplate(string sFileName)
        {
            string sSql = @"Select count(TemplateId) From EmailTemplate Where FileName1 = '" + sFileName +
                        "' Or FileName2 = '" + sFileName +
                        "' Or FileName3 = '" + sFileName +
                        "' Or FileName4 = '" + sFileName +
                        "' Or FileName5 = '" + sFileName + "'";

            int nResult = DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

            if (nResult > 0)
                return true;
            else
                return false;
        }
        #endregion

        #region " CustomerGroup "
        public CustomerGroup GetCustomerGroup(long id)
        {
            CustomerGroup obj = null;

            using (var db = GetModels())
            {
                db.CustomerGroups.Where(it => it.CustomerGroupId == id).ToList().ForEach(it => obj = it);
            }

            return obj;
        }

        public List<CustomerGroup> GetAllCustomerGroups(long? siteId = null)
        {
            using (var db = GetModels())
            {
                var query = db.CustomerGroups.Where(it => it.CustomerGroupId > 0 && it.Active == true);

                if (siteId.HasValue)
                {
                    query = query.Where(it => it.SiteId == siteId.Value);
                }

                return query.ToList();
            }
        }

        public IQueryable<CustomerGroup> GetListCustomerGroups(long? siteId = null)
        {
            var db = GetModels();
            var query = db.CustomerGroups.Where(it => it.CustomerGroupId > 0 && it.Active == true);

            if (siteId.HasValue)
            {
                query = query.Where(it => it.SiteId == siteId.Value);
            }

            return query;
        }

        public bool IsExistCustomerGroupName(string sCustomerGroupName)
        {
            int nCount = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                string.Format("Select Count(*) From CustomerGroup Where Active=1 And CustomerGroupName ='{0}'", sCustomerGroupName)), 0);

            if (nCount > 0)
                return true;
            else
                return false;

        }

        public bool IsExistCustomerGroupName(string sCustomerGroupName, int nCustomerGroupId)
        {
            int nCount = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                string.Format("Select Count(*) From CustomerGroup Where  Active=1 And CustomerGroupId <>{0} And CustomerGroupName ='{1}' ", nCustomerGroupId, sCustomerGroupName)), 0);

            if (nCount > 0)
                return true;
            else
                return false;

        }
        public int AddCustomerGroup(CustomerGroup pObj)
        {
            string sSql = "INSERT INTO CustomerGroup ([CustomerGroupName], [AutoSync], [SiteId],  [Active])  	VALUES (@CustomerGroupName, @AutoSync, @SiteId,  @Active ) SELECT @@Identity";

            return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@CustomerGroupName", pObj.CustomerGroupName),
                new SqlParameter("@AutoSync", pObj.AutoSync),
                new SqlParameter("@SiteId", pObj.SiteId),
                new SqlParameter("@Active", pObj.Active)), 0);
        }

        public int UpdateCustomerGroup(CustomerGroup pObj)
        {
            string sSql = "	UPDATE CustomerGroup SET [CustomerGroupName] = @CustomerGroupName, [AutoSync] = @AutoSync, [SiteId] = @SiteId, [Active] = @Active Where [CustomerGroupId] = @CustomerGroupId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@CustomerGroupId", pObj.CustomerGroupId),
                new SqlParameter("@CustomerGroupName", pObj.CustomerGroupName),
                new SqlParameter("@AutoSync", pObj.AutoSync),
                new SqlParameter("@SiteId", pObj.SiteId),
                new SqlParameter("@Active", pObj.Active));
        }

        public int DeleteCustomerGroup(long nCustomerGroupId)
        {
            DeleteAllCustomerGroupsCustomerTypes(nCustomerGroupId);
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                @"DELETE FROM CustomerGroupCustomer WHERE CustomerGroupId = @CustomerGroupId;
                DELETE FROM CustomerGroup WHERE CustomerGroupId = @CustomerGroupId;",
                new SqlParameter("@CustomerGroupId", nCustomerGroupId));
        }

        public int DeleteCustomerGroup(IEnumerable<long> nCustomerGroupIds)
        {
            string ids = String.Join(",", nCustomerGroupIds);
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "DELETE FROM CustomerGroupCustomer WHERE CustomerGroupId IN(" + ids + ");" +
                "DELETE FROM CustomerGroup WHERE CustomerGroupId IN(" + ids + ");");
        }

        #region DeleteCustomerGroupItemsByCustomerGroupId
        public void DeleteCustomerGroupItemsByCustomerGroupId(long id)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM [CustomerGroupCustomer] WHERE [CustomerGroupId] = " + id);
        }
        #endregion

        #region DeleteCustomerGroupItemsByCustomerId
        public void DeleteCustomerGroupCustomerByCustomerId(long id)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM [CustomerGroupCustomer] WHERE [CustomerId] = " + id);
        }
        #endregion

        #region AddCustomerGroupItem
        public void AddCustomerGroupCustomer(long customernGroupId, long customerId)
        {
            // Leave it just one command to make it work faster.
            string sql = @"IF NOT EXISTS(SELECT * FROM [CustomerGroupCustomer] WHERE [CustomerGroupId] = @CustomerGroupId AND [CustomerId] = @CustomerId)
                            BEGIN
                                INSERT INTO [CustomerGroupCustomer]([CustomerGroupId], [CustomerId]) VALUES(@CustomerGroupId, @CustomerId)
                            END";

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CustomerGroupId", customernGroupId),
                new SqlParameter("@CustomerId", customerId));
        }
        #endregion

        #region GetCustomerGroupsByCustomerId
        public List<int> GetCustomerGroupsByCustomerId(long id)
        {
            string sql = "SELECT CustomerGroupId FROM CustomerGroupCustomer WHERE CustomerId = " + id;
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
            List<int> customerGroupIds = new List<int>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                customerGroupIds.Add(DataManager.ToInt(row["CustomerGroupId"]));
            }
            return customerGroupIds;
        }
        #endregion

        #region GetCustomerPickerData
        public List<User> GetCustomerPickerData(string exclude)
        {
            string sql = @"SELECT [Users].[UserId], [Users].[FirstName], [Users].[LastName] FROM [Users] 
                           WHERE [Active] = 1 AND [UserTypeId] = " + UserType.Type.Customer;
            if (!String.IsNullOrEmpty(exclude.Trim()))
            {
                sql += " AND [Users].[UserId] NOT IN(" + exclude + ")";
            }

            List<User> customers = new List<User>();
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    customers.Add(new User()
                    {
                        UserId = DataManager.ToInt(row["UserId"]),
                        FirstName = DataManager.ToString(row["FirstName"]),
                        LastName = DataManager.ToString(row["LastName"])
                    });
                }
            }
            return customers;
        }
        #endregion

        #region GetCustomerByCustomerGroupId
        public List<User> GetCustomerByCustomerGroupId(int customerGroupId)
        {
            string sql = @"SELECT [UserId], [FirstName], [LastName] FROM [CustomerGroupCustomer] cgc
                            INNER JOIN [Users] c ON c.UserId = cgc.CustomerId
                            INNER JOIN [CustomerGroup] cg ON cg.CustomerGroupId = cgc.CustomerGroupId
                            WHERE c.SiteId = cg.SiteId AND cgc.CustomerGroupId = " + customerGroupId;
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            List<User> customers = new List<User>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    customers.Add(new User()
                    {
                        UserId = DataManager.ToInt(row["UserId"]),
                        FirstName = DataManager.ToString(row["FirstName"]),
                        LastName = DataManager.ToString(row["LastName"])
                    });
                }
            }
            return customers;
        }
        #endregion
        #endregion

        #region " MailingList "
        //#region GetAllMailingLists
        //public List<MailingList> GetAllMailingLists(jQueryDataTableParamModel param, long? siteId = null)
        //{
        //    List<MailingList> list = new List<MailingList>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectMailingListsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@SiteId", siteId));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new MailingList(row));
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //#endregion

        //public DataSet GetMailingList(string sFilter)
        //{

        //    String sql = @" Select *,Case [MailingList].Active when 1 then 'Aktive' else 'Inaktiv' end As ActiveName 
        //                    From [MailingList] " + sFilter + " Order by MailingListName";

        //    return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //}

        //public MailingList GetMailingListByID(long iMailingListId)
        //{
        //    string sql = "Select *" +
        //        " From [MailingList]" +
        //        " Where MailingListId=" + iMailingListId;

        //    //if (iCustomerGroup > 0)
        //    //    sql += string.Format(" And CustomerGroupId={0}", iCustomerGroup);

        //    //sql += " Order by UserId";

        //    //return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    if (ds != null && ds.Tables[0].Rows.Count > 0)
        //        return new MailingList(ds.Tables[0].Rows[0]);
        //    else
        //        return null;
        //}

        //public DataSet GetCustomerList(int iCustomerGroup)
        //{
        //    string sql = "Select UserId, Email,FirstName + ' ' + LastName As CustomerName" +
        //        " From [User]" +
        //        " Where Active=1 And UserTypeId=0";

        //    if (iCustomerGroup > 0)
        //        sql += string.Format(" And CustomerGroupId={0}", iCustomerGroup);

        //    sql += " Order by FirstName, LastName";

        //    return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //}

        //public bool IsExistMailingListName(string sMailingListName, int iMailingListId)
        //{
        //    int nCount = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
        //        string.Format("Select Count(*) From MailingList Where  Active=1 And MailingListId <>{0} And MailingListName='{1}'", iMailingListId, sMailingListName)), 0);
        //    if (nCount > 0)
        //        return true;
        //    else
        //        return false;

        //}

        //public int AddMailingList(MailingList pObj)
        //{
        //    string sSql = "INSERT INTO MailingList ([MailingListName], [SiteId],  [Active])  	VALUES (@MailingListName, @SiteId,  @Active ) SELECT @@Identity";

        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@MailingListName", pObj.MailingListName),
        //        new SqlParameter("@SiteId", pObj.SiteId),
        //        new SqlParameter("@Active", pObj.Active)), 0);
        //}

        //public int UpdateMailingList(MailingList pObj)
        //{
        //    string sSql = "	UPDATE MailingList SET [MailingListName] = @MailingListName, [SiteId] = @SiteId, [Active] = @Active Where [MailingListId] = @MailingListId";

        //    return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@MailingListId", pObj.MailingListId),
        //        new SqlParameter("@MailingListName", pObj.MailingListName),
        //        new SqlParameter("@SiteId", pObj.SiteId),
        //        new SqlParameter("@Active", pObj.Active));
        //}

        //public int UpdateMailingList(MailingList pObj, string sTemp)
        //{
        //    string sSql = "	UPDATE MailingList SET [MailingListName] = @MailingListName, [Active] = @Active Where [MailingListId] = @MailingListId";
        //    sSql += sTemp;

        //    return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@MailingListId", pObj.MailingListId),
        //        new SqlParameter("@MailingListName", pObj.MailingListName),
        //        new SqlParameter("@Active", pObj.Active));
        //}

        //public int DeleteMailingList(int nMailingListId)
        //{
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
        //        "Delete MailingList where MailingListId=" + nMailingListId + " Delete CustomerMailList where MailingListId=" + nMailingListId);
        //}

        //public int DeleteCustomerMailingList(int nMailingListId)
        //{
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "Delete From CustomerMailList Where MailingListId =" + nMailingListId);
        //}

        //public int DeleteCustomerMailingListByUserId(string sSql)
        //{
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));
        //}

        //public int UnSubscribeUserEmail(long nUserId)
        //{
        //    string sSql = "Delete From Katagorie Where UserId=" + nUserId;
        //    sSql += " Update [User] Set NewsletterType=0 Where UserId=" + nUserId;

        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sSql);

        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "Delete From CustomerMailList Where UserId=" + nUserId);
        //}

        public void UnSubscribeUserEmail(string email, long siteId)
        {
            string sql = "UPDATE [Users] SET [IsSubscriber] = 0, [IsReceiveEmailInfo] = 0 WHERE [Email] = @Email";
            if (siteId > 0)
            {
                sql += " AND [SiteId] = " + siteId;
            }

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@Email", email));
        }

        public void UnSubscribeUserEmail(string email, long siteId, long unsubscribeReasonId, string otherReason, long emailTrackingId)
        {
            string sql = "SELECT [UserId] FROM [Users] WHERE [Email] = @Email";
            DateTime now = DateTime.Now;

            if (siteId > 0)
            {
                sql += " AND [SiteId] = " + siteId;
            }

            long userId = DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@Email", email)));

            if (userId <= 0)
                return;

            sql = "UPDATE [Users] SET [IsSubscriber] = 0, [IsReceiveEmailInfo] = 0 WHERE [UserId] = " + userId;
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);

            sql = "INSERT INTO [EmailUnsubscription]([UserId], [UnsubscribeReasonId], [OtherReason], [EmailTrackingId], [CreatedDate], [UpdatedDate]) VALUES(@UserId, @UnsubscribeReasonId, @OtherReason, @EmailTrackingId, @CreatedDate, @UpdatedDate)";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@UserId", userId),
                new SqlParameter("@UnsubscribeReasonId", unsubscribeReasonId),
                new SqlParameter("@OtherReason", otherReason),
                new SqlParameter("@EmailTrackingId", emailTrackingId),
                new SqlParameter("@CreatedDate", now),
                new SqlParameter("@UpdatedDate", now));
        }

        //public int AddCustomerList(string sCList)
        //{
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sCList));
        //}

        //public int AddCustomerMailingList(int nMailingListId, int nUserId)
        //{
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
        //        string.Format("Insert into CustomerMailList (MailingListId,UserId) values({0},{1})", nMailingListId, nUserId));
        //}
        //public bool IsExistMailingList(int iMailingListId, int iUserId)
        //{
        //    string sSql = "Select Count(*) From [MailingList] Where UserId = " + iUserId + " AND MailingListId = " + iMailingListId;

        //    int nResult = DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

        //    if (nResult > 0)
        //        return true;
        //    else
        //        return false;
        //}
        //public bool IsExistCustomerMailingList(int iMailingListId, int iUserId)
        //{
        //    string sSql = "Select Count(*) From [CustomerMailList] Where UserId = " + iUserId + " AND MailingListId = " + iMailingListId;

        //    int nResult = DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

        //    if (nResult > 0)
        //        return true;
        //    else
        //        return false;
        //}
        //public DataSet GetKatagorieList()
        //{
        //    string sql = @"SELECT DISTINCT Category.CategoryName, Katagorie.CategoryId
        //                    FROM Category INNER JOIN
        //                    Katagorie ON Category.CategoryId = Katagorie.CategoryId
        //                    Order by Category.CategoryName";

        //    return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //}
        //public DataSet GetCustomerInCategory(string sFilter)
        //{
        //    string sSql = string.Format(@"SELECT CategoryName,Category.CategoryId, [User].UserId, [User].FirstName+' '+ [User].LastName as CustomerName,Email
        //                                    FROM Katagorie INNER JOIN
        //                                    [User] ON Katagorie.UserId = [User].UserId
        //                                    inner Join Category On Category.CategoryId = Katagorie.CategoryId
        //                                    WHERE [User].Active=1 and ISNULL(NewsletterType, 0) > 0 and isnull(IsVerified,0)=1 {0}
        //                                    ORDER BY CustomerName", sFilter);

        //    return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sSql);
        //}

        //public DataSet GetCustomerMailingList(long nMailingListId, int nLangId = 1)
        //{
        //    string sSql = @"SELECT Distinct [Users].UserId, [Users].Firstname, [Users].Lastname, [Users].Middlename, [Users].Email, ct.[CustomerTypeName] AS CustomerTypeName, sct.[CustomerTypeName] AS SubCustomerTypeName
        //        FROM CustomerMailList
        //        INNER JOIN [Users] ON CustomerMailList.UserId = [Users].UserId
        //        LEFT JOIN [CustomerTypeLang] ct ON ct.[CustomerTypeId] = [Users].[CustomerTypeId] AND ct.[LangId] = @LangId
        //        LEFT JOIN [CustomerTypeLang] sct ON sct.[CustomerTypeId] = [Users].[SubCustomerTypeId] AND sct.[LangId] = @LangId
        //        WHERE [Users].Active = 1 And MailingListId = @MailingListId AND ISNULL([Users].Email, '') <> '' ";
        //    return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@MailingListId", nMailingListId),
        //        new SqlParameter("@LangId", nLangId));
        //}
        //public DataSet GetCustomerKat(string sql)
        //{

        //    return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql);
        //}
        #endregion

        #region " Template "
        public int AddEmailTemplate(EmailTemplate pObj)
        {
            string sSql = @"INSERT INTO EmailTemplate ([CategoryId], [Active],  [UserId],  [InsertDate], [UpdateDate], FileName1, FileName2, FileName3, FileName4, FileName5, FileDescription1, FileDescription2, FileDescription3, FileDescription4, FileDescription5, FileUrl1, FileUrl2, FileUrl3, FileUrl4, FileUrl5)  	
                        VALUES (@CategoryId, @Active,  @UserId, GetDate(), GetDate(), @FileName1, @FileName2, @FileName3, @FileName4, @FileName5, @FileDescription1, @FileDescription2, @FileDescription3, @FileDescription4, @FileDescription5, @FileUrl1, @FileUrl2, @FileUrl3, @FileUrl4, @FileUrl5) 
                        SELECT @@Identity";

            return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@CategoryId", pObj.CategoryId),
                new SqlParameter("@Active", pObj.Active),
                new SqlParameter("@FileName1", pObj.FileName1),
                new SqlParameter("@FileName2", pObj.FileName2),
                new SqlParameter("@FileName3", pObj.FileName3),
                new SqlParameter("@FileName4", pObj.FileName4),
                new SqlParameter("@FileName5", pObj.FileName5),
                new SqlParameter("@FileDescription1", pObj.FileDescription1),
                new SqlParameter("@FileDescription2", pObj.FileDescription2),
                new SqlParameter("@FileDescription3", pObj.FileDescription3),
                new SqlParameter("@FileDescription4", pObj.FileDescription4),
                new SqlParameter("@FileDescription5", pObj.FileDescription5),
                new SqlParameter("@FileUrl1", pObj.FileUrl1),
                new SqlParameter("@FileUrl2", pObj.FileUrl2),
                new SqlParameter("@FileUrl3", pObj.FileUrl3),
                new SqlParameter("@FileUrl4", pObj.FileUrl4),
                new SqlParameter("@FileUrl5", pObj.FileUrl5),
                new SqlParameter("@UserId", pObj.UserId)), 0);
        }

        public int UpdateEmailTemplate(EmailTemplate pObj)
        {
            string sSql = @"UPDATE EmailTemplate SET [CategoryId] = @CategoryId, 
                        [Active] = @Active, [UserId] = @UserId, [UpdateDate] = GetDate(), FileName1 = @FileName1, FileName2 = @FileName2, FileName3 = @FileName3, FileName4 = @FileName4, FileName5 = @FileName5, FileDescription1 = @FileDescription1, FileDescription2 = @FileDescription2, FileDescription3 = @FileDescription3, FileDescription4 = @FileDescription4, FileDescription5 = @FileDescription5, FileUrl1 = @FileUrl1, FileUrl2 = @FileUrl2, FileUrl3 = @FileUrl3, FileUrl4 = @FileUrl4, FileUrl5 = @FileUrl5
                        Where [TemplateId] = @TemplateId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@TemplateId", pObj.TemplateId),
                new SqlParameter("@CategoryId", pObj.CategoryId),
                new SqlParameter("@Active", pObj.Active),
                new SqlParameter("@FileName1", pObj.FileName1),
                new SqlParameter("@FileName2", pObj.FileName2),
                new SqlParameter("@FileName3", pObj.FileName3),
                new SqlParameter("@FileName4", pObj.FileName4),
                new SqlParameter("@FileName5", pObj.FileName5),
                new SqlParameter("@FileDescription1", pObj.FileDescription1),
                new SqlParameter("@FileDescription2", pObj.FileDescription2),
                new SqlParameter("@FileDescription3", pObj.FileDescription3),
                new SqlParameter("@FileDescription4", pObj.FileDescription4),
                new SqlParameter("@FileDescription5", pObj.FileDescription5),
                new SqlParameter("@FileUrl1", pObj.FileUrl1),
                new SqlParameter("@FileUrl2", pObj.FileUrl2),
                new SqlParameter("@FileUrl3", pObj.FileUrl3),
                new SqlParameter("@FileUrl4", pObj.FileUrl4),
                new SqlParameter("@FileUrl5", pObj.FileUrl5),
                new SqlParameter("@UserId", pObj.UserId));
        }

        public int DeleteEmailTemplate(long nTemplateId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "UPDATE EmailTemplate SET Active = 0 WHERE TemplateId=" + nTemplateId);
        }

        public int DeleteEmailTemplate(IEnumerable<long> nTemplateIds)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "UPDATE EmailTemplate SET Active = 0 WHERE TemplateId IN(" + String.Join(",", nTemplateIds) + ")");
        }

        public EmailTemplate GetEmailTemplate(long templateId, int langId = 1)
        {
            EmailTemplate obj = null;

            using (var db = GetModels())
            {
                (from it in db.EmailTemplates
                 join etl in db.EmailTemplateLangs.Where(it => it.LangId == langId) on it.TemplateId equals etl.TemplateId into etlj
                 from etl in etlj.DefaultIfEmpty()
                 where it.TemplateId == templateId && it.Active == true
                 select new { EmailTemplate = it, EmailTemplateLang = etl }).ToList().ForEach(it =>
                 {
                     obj = it.EmailTemplate;
                     obj.TemplateName = it.EmailTemplateLang.TemplateName;
                     obj.Description = it.EmailTemplateLang.Description;
                     obj.Subject = it.EmailTemplateLang.Subject;
                     obj.EmailTemplateLangs.Add(it.EmailTemplateLang);
                 });
            }

            return obj;
        }

        public EmailTemplate GetEmailTemplateByKey(string key, int langId = 1)
        {
            EmailTemplate obj = new EmailTemplate();

            using (var db = GetModels())
            {
                (from it in db.EmailTemplates
                 join tl in db.EmailTemplateLangs.Where(it => it.LangId == langId) on it.TemplateId equals tl.TemplateId into tlj
                 from tl in tlj.DefaultIfEmpty()
                 where it.TemplateKey == key && it.Active == true
                 select new { EmailTemplate = it, EmailTemplateLang = tl }).ToList().ForEach(it =>
                 {
                     obj = it.EmailTemplate;
                     obj.EmailTemplateLangs.Add(it.EmailTemplateLang);
                 });
            }

            return obj;
        }

        public DataSet GetTemplate(string sFilter)
        {
            string sql = string.Format("Select * From EmailTemplate Where Active = 1 {0}", sFilter);

            //if (nTemplateType > 0)
            //    sql += " And Template.TemplateType=" + nTemplateType;

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        }

        #region GetAllEmailTemplates
        public IQueryable<EmailTemplateViewModel> GetAllEmailTemplates(int langId = 1)
        {
            var db = GetModels();
            return from it in db.EmailTemplates
                   join tl in db.EmailTemplateLangs.Where(it => it.LangId == langId) on it.TemplateId equals tl.TemplateId into tlj
                   from tl in tlj.DefaultIfEmpty()
                   where it.TemplateId > 0 && it.Active == true
                   orderby it.TemplateId descending
                   select new EmailTemplateViewModel { TemplateId = it.TemplateId, TemplateName = tl.TemplateName };
        }
        #endregion

        #region GetSelectableEmailTemplates
        public List<EmailTemplateViewModel> GetSelectableEmailTemplates(long? siteId, int langId = 1)
        {
            siteId = siteId.HasValue ? siteId.Value : 0;
            List<EmailTemplateViewModel> list = new List<EmailTemplateViewModel>();
            string sql = @"SELECT it.TemplateId, tl.TemplateName, ISNULL(st.TemplateId, 0) AS SiteTemplateId FROM EmailTemplate it
                           JOIN EmailTemplateLang tl ON tl.TemplateId = it.TemplateId AND tl.LangId = @LangId
                           LEFT JOIN SiteEmailTemplate st ON st.TemplateId = it.TemplateId AND st.SiteId = @SiteId
                           WHERE it.Active = 1";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("@LangId", langId));
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new EmailTemplateViewModel
                    {
                        TemplateId = DataManager.ToInt(row["TemplateId"]),
                        TemplateName = DataManager.ToString(row["TemplateName"]),
                        IsAvailable = DataManager.ToInt(row["SiteTemplateId"]) > 0
                    });
                }
            }
            return list;
        }
        #endregion

        #region GetAvailableEmailTemplates
        public List<EmailTemplateViewModel> GetAvailableEmailTemplates(int langId = 1, long siteId = 0)
        {
            List<EmailTemplateViewModel> list = new List<EmailTemplateViewModel>();
            string sql = @"SELECT it.TemplateId, tl.TemplateName FROM EmailTemplate it
                           JOIN EmailTemplateLang tl ON tl.TemplateId = it.TemplateId AND tl.LangId = @LangId
                           JOIN SiteEmailTemplate st ON st.TemplateId = it.TemplateId AND (st.SiteId = @SiteId OR @SiteId <= 0)
                           WHERE it.Active = 1";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("@LangId", langId));
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new EmailTemplateViewModel
                    {
                        TemplateId = DataManager.ToInt(row["TemplateId"]),
                        TemplateName = DataManager.ToString(row["TemplateName"]),
                        IsAvailable = true
                    });
                }
            }
            return list;
        }
        #endregion

        #region GetAllEmailTemplateLangs
        public IQueryable<EmailTemplateLang> GetListEmailTemplateLangs(int langId = 1)
        {
            var db = GetModels();
            return from it in db.EmailTemplateLangs
                   join t in db.EmailTemplates on it.TemplateId equals t.TemplateId
                   where t.Active == true && it.LangId == langId
                   orderby t.TemplateName
                   select it;
        }
        #endregion

        public int SaveEmailTemplateLang(EmailTemplateLang obj)
        {
            string sql = @"IF(EXISTS(SELECT [TemplateId] FROM [EmailTemplateLang] WHERE [TemplateId] = @TemplateId AND [LangId] = @LangId))
                            BEGIN
                                UPDATE [EmailTemplateLang] SET [TemplateName] = @TemplateName, [Description] = @Description, [Subject] = @Subject, [HtmlDetail] = @HtmlDetail WHERE [TemplateId] = @TemplateId AND [LangId] = @LangId;
                            END
                            ELSE
                            BEGIN
                                INSERT INTO [EmailTemplateLang]([TemplateId], [LangId], [TemplateName], [Description], [Subject], [HtmlDetail])
                                VALUES(@TemplateId, @LangId, @TemplateName, @Description, @Subject, @HtmlDetail) SELECT @@IDENTITY;
                            END";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@TemplateId", obj.TemplateId),
                new SqlParameter("@LangId", obj.LangId),
                new SqlParameter("@TemplateName", obj.TemplateName),
                new SqlParameter("@Description", obj.Description),
                new SqlParameter("@Subject", obj.Subject),
                new SqlParameter("@HtmlDetail", obj.HtmlDetail));
        }
        #endregion

        #region SiteEmailTemplate
        public void SaveSiteEmailTemplate(int siteId, long templateId)
        {
            string sql = @"IF NOT EXISTS(SELECT * FROM SiteEmailTemplate WHERE SiteId = @SiteId AND TemplateId = @TemplateId)
                           BEGIN
                                INSERT INTO SiteEmailTemplate(SiteId, TemplateId) VALUES(@SiteId, @TemplateId)
                           END";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("@TemplateId", templateId));
        }

        public void DeleteAllSiteEmailTemplatesBySiteId(long siteId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM SiteEmailTemplate WHERE SiteId = " + siteId);
        }
        #endregion

        #region " Emailing "
        public int DeleteEmailing(long nEmailId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Delete From Emailing Where EmailId=" + nEmailId);
        }
        public int DeleteEmailing(IEnumerable<long> nEmailIds)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Delete From Emailing Where EmailId IN (" + String.Join(",", nEmailIds) + ")");
        }
        public long AddEmailing(Emailing pObj)
        {
            string sSql = @"INSERT INTO Emailing (EmailName, [Subject], FromName, FromEmail, EmailFormatId, TemplateId, InsertDate, UpdateDate, Active, UserId, SiteId, StatusId, MjmlDetail, HtmlDetail, TextDetail, SendMailUsing, ScheduleDateTime)
                VALUES(@EmailName, @Subject, @FromName, @FromEmail, @EmailFormatId, @TemplateId, GETDATE(), NULL, 1, @UserId, @SiteId, @StatusId, @MjmlDetail, @HtmlDetail, @TextDetail, @SendMailUsing, @ScheduleDateTime)
                SELECT @@IDENTITY";

            return DataManager.ToLong(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@EmailName", pObj.EmailName),
                new SqlParameter("@Subject", pObj.Subject),
                new SqlParameter("@FromName", pObj.FromName),
                new SqlParameter("@FromEmail", pObj.FromEmail),
                new SqlParameter("@EmailFormatId", pObj.EmailFormatId),
                new SqlParameter("@TemplateId", pObj.TemplateId),
                new SqlParameter("@UserId", pObj.UserId),
                new SqlParameter("@SiteId", pObj.SiteId),
                new SqlParameter("@MjmlDetail", pObj.MjmlDetail),
                new SqlParameter("@HtmlDetail", pObj.HtmlDetail),
                new SqlParameter("@TextDetail", pObj.TextDetail),
                new SqlParameter("@SendMailUsing", pObj.SendMailUsing),
                new SqlParameter("@ScheduleDateTime", pObj.ScheduleDateTime),
                new SqlParameter("@StatusId", pObj.StatusId)), -1);
        }

        public long UpdateEmailing(Emailing pObj)
        {
            string sSql = @"UPDATE Emailing
                SET EmailName = @EmailName, [Subject] = @Subject, FromName = @FromName, FromEmail = @FromEmail, MjmlDetail = @MjmlDetail, HtmlDetail = @HtmlDetail, TextDetail = @TextDetail,
	                EmailFormatId = @EmailFormatId, TemplateId = @TemplateId, UserId = @UserId, SiteId = @SiteId, StatusId = @StatusId, SendMailUsing = @SendMailUsing, ScheduleDateTime = @ScheduleDateTime, UpdateDate = GETDATE()
                WHERE EmailId = @EmailId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@EmailId", pObj.EmailId),
                new SqlParameter("@EmailName", pObj.EmailName),
                new SqlParameter("@Subject", pObj.Subject),
                new SqlParameter("@FromName", pObj.FromName),
                new SqlParameter("@FromEmail", pObj.FromEmail),
                new SqlParameter("@EmailFormatId", pObj.EmailFormatId),
                new SqlParameter("@TemplateId", pObj.TemplateId),
                new SqlParameter("@UserId", pObj.UserId),
                new SqlParameter("@SiteId", pObj.SiteId),
                new SqlParameter("@MjmlDetail", pObj.MjmlDetail),
                new SqlParameter("@HtmlDetail", pObj.HtmlDetail),
                new SqlParameter("@TextDetail", pObj.TextDetail),
                new SqlParameter("@SendMailUsing", pObj.SendMailUsing),
                new SqlParameter("@ScheduleDateTime", pObj.ScheduleDateTime),
                new SqlParameter("@StatusId", pObj.StatusId));
        }

        public DataSet GetEmailings(string sFilter)
        {
            string sSql = "SELECT * FROM Emailing WHERE Active = 1 " + sFilter + " ORDER BY InsertDate DESC, EmailName";
            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
        }

        public Emailing GetEmailing(long nEmailId, int langId = 1)
        {
            Emailing obj = null;
            string sSql = @"SELECT Emailing.*, EmailTemplateLang.Name AS TemplateName
                FROM Emailing
                LEFT OUTER JOIN EmailTemplateLang ON Emailing.TemplateId = EmailTemplateLang.TemplateId AND EmailTemplateLang.LangId = @LangId
                WHERE EmailId = @EmailId";

            using (var db = GetModels())
            {
                (from it in db.Emailings
                 join etl in db.EmailTemplateLangs.Where(it => it.LangId == langId) on it.TemplateId equals etl.TemplateId into etlj
                 from etl in etlj.DefaultIfEmpty()
                 where it.EmailId == nEmailId
                 select new { Emailing = it, TemplateName = etl.TemplateName }).ToList().ForEach(it =>
                 {
                     obj = it.Emailing;
                     obj.TemplateName = it.TemplateName;
                 });
            }

            return obj;
        }

        public long GetEmaillingSiteId(long emailId)
        {
            string sql = "SELECT TOP(1) SiteId FROM Emailing WHERE EmailId = @EmailId";
            return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@EmailId", emailId)), 0);
        }

        public string GetEmailCampaignName(long id)
        {
            string sql = "SELECT EmailName FROM Emailing WHERE EmailId = @EmailId";
            return DataManager.ToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@EmailId", id)));
        }

        #endregion

        #region " EmailQue "

        public EmailQue GetEmailQue(long emailQueId)
        {
            EmailQue obj = null;
            using (var db = GetModels())
            {
                db.EmailQues.Where(it => it.EmailQueId == emailQueId).ToList().ForEach(it => obj = it);
            }
            return obj;
        }

        public long AddEmailQue(EmailQue pObj)
        {
            int emailQueId = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                "SELECT TOP(1) EmailQueId FROM EmailQue WHERE EmailId = @EmailId AND Email = @Email AND ISNULL(Status, 0) < 1 AND ISNULL(Resent, 0) < 3 ORDER BY EmailQueId DESC",
                new SqlParameter("@EmailId", pObj.EmailId),
                new SqlParameter("@Email", pObj.Email)), 0);

            if (emailQueId == 0)
            {
                string sSql = @"INSERT INTO EmailQue (EmailId, CustomerId, Email, [Status], InsertDate, SendDate, IsError, ReturnMessage, Resent, HtmlDetail, IsOpenMail, IsClickLink, IsUnsubscribe, UnsubscribeMailTo, UnsubscribeUrl, NetMessageCampaignId, ScheduleDateTime)
                    VALUES(@EmailId, @CustomerId, @Email, @Status, GETDATE(), @SendDate, @IsError, @ReturnMessage, @Resent, @HtmlDetail, @IsOpenMail, @IsClickLink, @IsUnsubscribe, @UnsubscribeMailTo, @UnsubscribeUrl, @NetMessageCampaignId, @ScheduleDateTime)
                    SELECT @@IDENTITY";

                return pObj.EmailQueId = DataManager.ToLong(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                    new SqlParameter("@EmailId", pObj.EmailId),
                    new SqlParameter("@CustomerId", pObj.CustomerId),
                    new SqlParameter("@Email", pObj.Email),
                    new SqlParameter("@Status", pObj.Status),
                    new SqlParameter("@SendDate", pObj.SendDate),
                    new SqlParameter("@IsError", pObj.IsError),
                    new SqlParameter("@ReturnMessage", pObj.ReturnMessage),
                    new SqlParameter("@Resent", pObj.Resent),
                    new SqlParameter("@HtmlDetail", pObj.HtmlDetail),
                    new SqlParameter("@IsOpenMail", pObj.IsOpenMail),
                    new SqlParameter("@IsClickLink", pObj.IsClickLink),
                    new SqlParameter("@IsUnsubscribe", pObj.IsUnsubscribe),
                    new SqlParameter("@UnsubscribeMailTo", pObj.UnsubscribeMailTo),
                    new SqlParameter("@UnsubscribeUrl", pObj.UnsubscribeUrl),
                    new SqlParameter("@NetMessageCampaignId", pObj.NetMessageCampaignId),
                    new SqlParameter("@ScheduleDateTime", pObj.ScheduleDateTime)));
            }
            else
            {
                string sSql = @"UPDATE EmailQue SET EmailId = @EmailId, CustomerId = @CustomerId, Email = @Email, [Status] = @Status, SendDate = @SendDate, IsError = @IsError, ReturnMessage = @ReturnMessage, Resent = @Resent, HtmlDetail = @HtmlDetail, IsOpenMail = @IsOpenMail, IsClickLink = @IsClickLink, IsUnsubscribe = @IsUnsubscribe, UnsubscribeMailTo = @UnsubscribeMailTo, UnsubscribeUrl = @UnsubscribeUrl, NetMessageCampaignId = @NetMessageCampaignId WHERE EmailQueId = @EmailQueId";

                SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                       new SqlParameter("@EmailQueId", pObj.EmailQueId),
                       new SqlParameter("@EmailId", pObj.EmailId),
                       new SqlParameter("@CustomerId", pObj.CustomerId),
                       new SqlParameter("@Email", pObj.Email),
                       new SqlParameter("@Status", pObj.Status),
                       new SqlParameter("@SendDate", pObj.SendDate),
                       new SqlParameter("@IsError", pObj.IsError),
                       new SqlParameter("@ReturnMessage", pObj.ReturnMessage),
                       new SqlParameter("@Resent", pObj.Resent),
                       new SqlParameter("@HtmlDetail", pObj.HtmlDetail),
                       new SqlParameter("@IsOpenMail", pObj.IsOpenMail),
                       new SqlParameter("@IsClickLink", pObj.IsClickLink),
                       new SqlParameter("@IsUnsubscribe", pObj.IsUnsubscribe),
                       new SqlParameter("@UnsubscribeMailTo", pObj.UnsubscribeMailTo),
                       new SqlParameter("@UnsubscribeUrl", pObj.UnsubscribeUrl),
                       new SqlParameter("@NetMessageCampaignId", pObj.NetMessageCampaignId));

                return emailQueId;
            }
        }

        public void UpdateEmailQueHtml(EmailQue emailQue)
        {
            string sSql = @"UPDATE [EmailQue] SET [Status] = @Status, [HtmlDetail] = @HtmlDetail, [UnsubscribeMailTo] = @UnsubscribeMailTo, [UnsubscribeUrl] = @UnsubscribeUrl WHERE [EmailQueId] = @EmailQueId";

            SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@EmailQueId", emailQue.EmailQueId),
                new SqlParameter("@Status", emailQue.Status),
                new SqlParameter("@HtmlDetail", emailQue.HtmlDetail),
                new SqlParameter("@UnsubscribeMailTo", emailQue.UnsubscribeMailTo),
                new SqlParameter("@UnsubscribeUrl", emailQue.UnsubscribeUrl));
        }

        public void DeleteEmailQue(long emailQueId)
        {
            string sql = @"DELETE it 
                           FROM EmailLog it
                           INNER JOIN EmailQue ON EmailQue.EmailQueId = it.EmailQueId
                           INNER JOIN Emailing ON Emailing.EmailId = EmailQue.EmailId
                           WHERE EmailQue.Status <> 1 AND Emailing.EmailId = @EmailQueId;

                           DELETE it
                           FROM EmailUnsubscription it
                           INNER JOIN EmailTracking ON EmailTracking.EmailTrackingId = it.EmailTrackingId
                           INNER JOIN EmailQue ON EmailQue.EmailQueId = EmailTracking.EmailQueId
                           INNER JOIN Emailing ON Emailing.EmailId = EmailQue.EmailId
                           WHERE EmailQue.Status <> 1 AND Emailing.EmailId = @EmailQueId;

                           DELETE it
                           FROM EmailTracking it
                           INNER JOIN EmailQue ON it.EmailQueId = it.EmailQueId
                           INNER JOIN Emailing ON Emailing.EmailId = EmailQue.EmailId
                           WHERE EmailQue.Status <> 1 AND Emailing.EmailId = @EmailQueId;

                           DELETE it FROM EmailQue it
                           INNER JOIN Emailing ON Emailing.EmailId = it.EmailId
                           WHERE it.Status <> 1 AND Emailing.EmailId = @EmailQueId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@EmailQueId", emailQueId));
        }

        public DataSet GetEmailingStatus(int nEmailId)
        {
            string sSql = @"SELECT COUNT(EmailQueId) AS TotalSentEmails FROM EmailQue WHERE EmailId = @EmailId
                      SELECT COUNT(EmailQueId) AS TotalOpens FROM EmailQue WHERE EmailId = @EmailId AND Status = 1 AND IsOpenMail = 1;
                      SELECT COUNT(EmailQueId) AS TotalClicks FROM EmailQue WHERE EmailId = @EmailId AND Status = 1 AND IsClickLink = 1;
                      SELECT COUNT(EmailQueId) AS TotalUnsubscribers FROM EmailQue WHERE EmailId = @EmailId AND Status = 1 AND IsUnsubscribe = 1;
                      SELECT COUNT(EmailQueId) AS TotalBounces FROM EmailQue WHERE EmailId = @EmailId AND Status = 1 AND IsBounced = 1;";

            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@EmailId", nEmailId));
        }

        public DataSet GetNetmessageSummaryReport(int nEmailId)
        {
            string sSql = @"SELECT COUNT(NetmessageReportRecordId) AS TotalSentEmails FROM NetmessageReportRecord it JOIN NetmessageReport r ON r.NetmessageReportId = it.NetmessageReportId WHERE ISNULL(it.IsSent, 0) = 1 AND r.EmailId = @EmailId
                            SELECT COUNT(NetmessageReportRecordId) AS TotalOpens FROM NetmessageReportRecord it JOIN NetmessageReport r ON r.NetmessageReportId = it.NetmessageReportId WHERE ISNULL(it.IsOpen, 0) = 1 AND r.EmailId = @EmailId
                            SELECT COUNT(NetmessageReportRecordId) AS TotalClicks FROM NetmessageReportRecord it JOIN NetmessageReport r ON r.NetmessageReportId = it.NetmessageReportId WHERE ISNULL(it.IsClick, 0) = 1 AND r.EmailId = @EmailId
                            SELECT COUNT(NetmessageReportRecordId) AS TotalUnsubscribers FROM NetmessageReportRecord it JOIN NetmessageReport r ON r.NetmessageReportId = it.NetmessageReportId WHERE ISNULL(it.IsUnsub, 0) = 1 AND r.EmailId = @EmailId
                            SELECT COUNT(NetmessageReportRecordId) AS TotalBounces FROM NetmessageReportRecord it JOIN NetmessageReport r ON r.NetmessageReportId = it.NetmessageReportId WHERE ISNULL(it.IsRadiate, 0) = 1 AND r.EmailId = @EmailId";

            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@EmailId", nEmailId));
        }

        public IQueryable<SendingEmailViewModel> GetEmailQueReportByEmailId(long emailId)
        {
            var db = GetModels();
            return from it in db.EmailQues
                   join u in db.Users on it.CustomerId equals u.UserId
                   where it.EmailId > 0
                   && it.EmailQueId > 0
                   && (it.Email != null && it.Email.Length > 0)
                   && it.EmailId == emailId
                   select new SendingEmailViewModel
                   {
                       EmailId = it.EmailId,
                       EmailQueId = it.EmailQueId,
                       Email = it.Email,
                       FirstName = u.FirstName,
                       LastName = u.LastName,
                       IsOpened = it.IsOpenMail,
                       IsClicked = it.IsClickLink,
                       IsUnsubscribed = it.IsUnsubscribe,
                       IsBounced = it.IsBounced,
                       Status = it.Status
                   };
        }

        public IQueryable<SendingEmailViewModel> GetNetmessageReportRecordsByEmailId(long emailId)
        {
            var db = GetModels();
            return from it in db.NetmessageReportRecords
                   join r in db.NetmessageReports on it.NetmessageReportId equals r.NetmessageReportId
                   where r.EmailId > 0
                   && (it.Email != null && it.Email.Length > 0)
                   && r.EmailId == emailId
                   select new SendingEmailViewModel
                   {
                       EmailId = r.EmailId,
                       EmailQueId = it.NetmessageReportRecordId,
                       Email = it.Email,
                       FirstName = it.FirstName,
                       LastName = it.LastName,
                       IsOpened = it.IsOpen,
                       IsClicked = it.IsClick,
                       IsUnsubscribed = it.IsUnsub,
                       IsBounced = it.IsRadiate,
                       Status = it.IsSent.HasValue && it.IsSent.Value ? 1 : 0
                   };
        }

        public void SetOpenMail(long campaignId, long emailQueId)
        {
            string sql = @"UPDATE it SET it.IsOpenMail = 1 FROM EmailQue it
                           JOIN Emailing e ON e.EmailId = it.EmailId
                           WHERE it.EmailQueId = @EmailQueId AND it.EmailId = @CampaignId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CampaignId", campaignId),
                new SqlParameter("@EmailQueId", emailQueId));
        }

        public void SetMailClickLink(long campaignId, long emailQueId)
        {
            string sql = @"UPDATE it SET it.IsOpenMail = 1, it.IsClickLink = 1 FROM EmailQue it
                           JOIN Emailing e ON e.EmailId = it.EmailId
                           WHERE it.EmailQueId = @EmailQueId AND it.EmailId = @CampaignId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CampaignId", campaignId),
                new SqlParameter("@EmailQueId", emailQueId));
        }

        public void SetMailUnsubscribe(long campaignId, long emailQueId)
        {
            string sql = @"UPDATE it SET it.IsOpenMail = 1, it.IsUnsubscribe = 1 FROM EmailQue it
                           JOIN Emailing e ON e.EmailId = it.EmailId
                           WHERE it.EmailQueId = @EmailQueId AND it.EmailId = @CampaignId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CampaignId", campaignId),
                new SqlParameter("@EmailQueId", emailQueId));
        }

        public void SetEmailQueBouncedByEmail(string email)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "UPDATE [EmailQue] SET IsBounced = 1, IsUnsubscribe = 1 WHERE [Email] = @Email",
                new SqlParameter("@Email", email));
        }

        public void AddUserToUnsubscriberListByEmail(string email)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "UPDATE [Users] SET IsSubscriber = 0, IsReceiveEmailInfo = 0 WHERE [Email] = @Email AND [UserTypeId] = @UserTypeId",
                new SqlParameter("@Email", email),
                new SqlParameter("@UserTypeId", UserType.Type.Customer));
        }
        #endregion

        #region "Email Tracking"
        public long AddEmailTracking(long campaignId, long emailQueId, string action, string value, string ip, string browser, string platform)
        {
            DateTime now = DateTime.Now;
            string sql = @"INSERT INTO EmailTracking(CampaignId, EmailQueId, Action, Value, IPAddress, Browser, Platform, CreatedDate, UpdatedDate) VALUES(@CampaignId, @EmailQueId, @Action, @Value, @IPAddress, @Browser, @Platform, @CreatedDate, @UpdatedDate) SELECT @@IDENTITY";
            return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CampaignId", campaignId),
                new SqlParameter("@EmailQueId", emailQueId),
                new SqlParameter("@Action", action),
                new SqlParameter("@Value", value),
                new SqlParameter("@IPAddress", ip),
                new SqlParameter("@Browser", browser),
                new SqlParameter("@Platform", platform),
                new SqlParameter("@CreatedDate", now),
                new SqlParameter("@UpdatedDate", now)));
        }
        #endregion

        #region " Email Service "

        public DataSet GetPendingEmail(int langId = 1)
        {
            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.StoredProcedure, "GetPendingEmail");
        }

        public int UpdateEmailQueStatus(int nEmailQueId, int nStatusId, DateTime dSendDate, bool bIsError, string sMessage, int resent)
        {
            string sSql = "UPDATE EmailQue SET Status = @Status, SendDate = @SendDate, IsError = @IsError, ReturnMessage = @ReturnMessage, Resent = @Resent WHERE EmailQueId = @EmailQueId";
            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@Status", nStatusId),
                new SqlParameter("@SendDate", dSendDate),
                new SqlParameter("@IsError", bIsError),
                new SqlParameter("@Resent", resent),
                new SqlParameter("@EmailQueId", nEmailQueId),
                new SqlParameter("@ReturnMessage", sMessage));
        }

        public DataSet GetFailedEmail(int langId = 1)
        {
            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.StoredProcedure, "GetFailedEmail");
        }

        public DataSet GetEmailKey()
        {
            string sSql = "SELECT * FROM EmailKey";
            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
        }

        #endregion

        #region " Impressum"
        //public string GetImpressum(int nImpressumId)
        //{
        //    return DataManager.ToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
        //        "Select Detail From Impressum Where ImpressumId=" + nImpressumId));
        //}

        //public int AddImpressum(Impressum obj)
        //{
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
        //          "INSERT INTO Impressum(Name, Detail, SiteId) VALUES(@Name, @Detail, @SiteId) SELECT @@IDENTITY"
        //        , new SqlParameter("@Name", obj.Name)
        //        , new SqlParameter("@Detail", obj.DetailBytes)
        //        , new SqlParameter("@SiteId", obj.SiteId)), -1);
        //}

        //public int UpdateImpressum(Impressum obj)
        //{
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
        //          "Update Impressum Set [Name] = @Name, [Detail] = @Detail, [SiteId] = @SiteId Where ImpressumId= @ImpressumId"
        //        , new SqlParameter("@Name", obj.Name)
        //        , new SqlParameter("@Detail", obj.DetailBytes)
        //        , new SqlParameter("@SiteId", obj.SiteId)
        //        , new SqlParameter("@ImpressumId", obj.ImpressumId));
        //}

        //public List<Impressum> GetAllImpressums(jQueryDataTableParamModel param, long? siteId = null)
        //{
        //    List<Impressum> list = new List<Impressum>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectImpressumsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@SiteId", siteId));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Impressum(row));
        //    }
        //    return list;
        //}

        //public Impressum GetImpressumById(long nImpressumId)
        //{
        //    Impressum obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectImpressum",
        //        new SqlParameter("ImpressumId", nImpressumId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new Impressum(ds.Tables[0].Rows[0]);
        //    }

        //    return obj;
        //}
        //public int DeleteImpressum(int nImpressumId)
        //{
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
        //        "Delete Impressum where ImpressumId =" + nImpressumId);
        //}

        //public List<Impressum> GetImpressumsList(long? siteId = null)
        //{
        //    List<Impressum> list = new List<Impressum>();
        //    string sql = "SELECT * FROM Impressum";
        //    if (siteId.HasValue)
        //    {
        //        sql += " WHERE SiteId = " + siteId.Value;
        //    }
        //    else
        //    {
        //        sql += " WHERE ISNULL(SiteId, 0) = 0";
        //    }
        //    sql += " ORDER BY Name";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Impressum(row));
        //    }
        //    return list;
        //}
        #endregion

        #region " Prefix Text"
        public int UpdatePrefixText(int nTitleId, string sText)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "UPDATE PrefixText Set Prefix=@Prefix Where TitleId=@TitleId"
            , new SqlParameter("@Prefix", sText)
            , new SqlParameter("@TitleId", nTitleId));
        }
        public DataSet GetPrefixText()
        {
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text,
                "Select * From PrefixText");
        }
        public string GetPrefixText(int nTitleId)
        {
            return DataManager.ToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                "Select Prefix From PrefixText Where TitleId=" + nTitleId));
        }
        #endregion

        public List<EmailTemplate> GetTemplatesList(int langId = 1)
        {
            List<EmailTemplate> list = new List<EmailTemplate>();
            string sql = @"SELECT it.TemplateId, it.TemplateName FROM EmailTemplateLang it
                            JOIN EmailTemplate e ON e.TemplateId = it.TemplateId AND e.Active = 1
                            WHERE it.LangId = @LangId ORDER BY it.TemplateName;";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@LangId", langId));
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new EmailTemplate()
                {
                    TemplateId = DataManager.ToInt(row["TemplateId"]),
                    TemplateName = DataManager.ToString(row["TemplateName"])
                });
            }
            return list;
        }

        #region Emailing
        public IQueryable<Emailing> GetAllEmailings(long? siteId = null)
        {
            var db = GetModels();
            var query = db.Emailings.Where(it => it.EmailId > 0 && it.Active == true);
            if (siteId.HasValue)
            {
                query = query.Where(it => it.SiteId == siteId.Value);
            }
            query = query.OrderByDescending(it => it.InsertDate);
            return query;
        }
        public IQueryable<EmailingViewModel> GetListEmailings(long? siteId = null)
        {
            var db = GetModels();
            var query = from it in db.Emailings
                        join s in db.SiteLangs on it.SiteId equals s.SiteId into sj
                        from s in sj.DefaultIfEmpty()
                        where it.EmailId > 0 && it.Active == true && s.LangId == 1
                        select new EmailingViewModel()
                        {
                            EmailId = it.EmailId,
                            SiteId = s.SiteId,
                            SiteName = s.SiteName,
                            EmailName = it.EmailName,
                            Subject = it.Subject,
                            InsertDate = it.InsertDate
                        };
            if (siteId.HasValue)
            {
                query = query.Where(it => it.SiteId == siteId.Value);
            }
            query = query.OrderByDescending(it => it.InsertDate);
            return query;
        }

        public void DeleteEmailListByEmailId(int p)
        {
            //            string sql = @"INSERT INTO EmailQue(EmailId, CustomerId, Email, Status, InsertDate, SendDate, IsError, ReturnMessage)
            //                            VALUES(@EmailId, @CustomerId, @Email, @Status, @InsertDate, @SendDate, @IsError, @ReturnMessage) SELECT @@IDENTITY";
            //            return DataManager.ToInt(SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
            //                new SqlParameter("@EmailId", ));
        }

        public List<User> GetAllCustomerInCustomerGroups(string customerGroupIds, int langId = 1, bool reverse = false)
        {
            int[] ids = customerGroupIds.Split(',').Select(it => DataManager.ToInt(it.Trim())).ToArray();
            var db = GetModels();
            var list = (from it in db.Users
                        where (it.Email != null && it.Email.Length > 0) && it.Active == true && it.IsSubscriber == true && it.IsReceiveEmailInfo == true && it.UserTypeId == UserType.Type.Customer

                        select it);

            if (reverse)
            {
                list = from it in list
                       from cgc in db.CustomerGroups
                       where !cgc.Users.Contains(it) && ids.Contains(cgc.CustomerGroupId) && it.SiteId == cgc.SiteId
                       select it;
            }
            else
            {
                list = from it in list
                       from cgc in db.CustomerGroups
                       where cgc.Users.Contains(it) && ids.Contains(cgc.CustomerGroupId) && it.SiteId == cgc.SiteId
                       select it;
            }
            return list.ToList();
        }

        public IQueryable<UserViewModel> GetListCustomerInCustomerGroups(string customerGroupIds, int langId = 1, bool reverse = false)
        {
            if (customerGroupIds == null)
                return null;

            int[] ids = customerGroupIds.Split(',').Select(it => DataManager.ToInt(it.Trim())).ToArray();
            var db = GetModels();
            var list = from it in db.Users
                       join ctl in db.CustomerTypeLangs.Where(it => it.LangId == langId) on it.CustomerTypeId equals ctl.CustomerTypeId into ctlj
                       from ctr in ctlj.DefaultIfEmpty()
                       join sctl in db.CustomerTypeLangs.Where(it => it.LangId == langId) on it.SubCustomerTypeId equals sctl.CustomerTypeId into sctlj
                       from sctr in sctlj.DefaultIfEmpty()
                       where (it.Email != null && it.Email.Length > 0) && it.Active == true && it.IsSubscriber == true && it.IsReceiveEmailInfo == true && it.UserTypeId == UserType.Type.Customer
                       select new
                       {
                           User = it,
                           CustomerType = ctr,
                           SubCustomerType = sctr
                       };

            if (reverse)
            {
                list = from it in list
                       from cgc in db.CustomerGroups
                       where !cgc.Users.Contains(it.User) && ids.Contains(cgc.CustomerGroupId) && it.User.SiteId == cgc.SiteId
                       select it;
            }
            else
            {
                list = from it in list
                       from cgc in db.CustomerGroups
                       where cgc.Users.Contains(it.User) && ids.Contains(cgc.CustomerGroupId) && it.User.SiteId == cgc.SiteId
                       select it;
            }
            return list.Select(it => new UserViewModel
            {
                UserId = it.User.UserId,
                SiteId = it.User.SiteId,
                Email = it.User.Email,
                FirstName = it.User.FirstName,
                LastName = it.User.LastName,
                CustomerTypeId = it.User.CustomerTypeId.HasValue ? it.User.CustomerTypeId.Value : 0,
                CustomerTypeName = it.CustomerType.CustomerTypeName,
                SubCustomerTypeId = it.User.SubCustomerTypeId,
                SubCustomerTypeName = it.SubCustomerType.CustomerTypeName,
                BirthDate = it.User.Birthdate.HasValue ? it.User.Birthdate.Value : DateTime.Today,
                Gender = it.User.Gender.HasValue ? it.User.Gender.Value : 0,
                GenderName = !it.User.Gender.HasValue || it.User.Gender == 0 ? Resources.Resources.Male : Resources.Resources.Female
            });
        }
        #endregion

        #region Email Template Variable
        public List<EmailTemplateVariable> GetAllEmailTemplateVariables()
        {
            using (var db = GetModels())
            {
                return db.EmailTemplateVariables.OrderBy(it => it.VariableId).ToList();
            }
        }
        #endregion

        #region UnsubscribeReason
        #region GetUnsubscribeReasons
        public List<UnsubscribeReason> GetUnsubscribeReasons()
        {
            using (var db = GetModels())
            {
                return db.UnsubscribeReasons.Where(it => it.Active == true).OrderBy(it => DataManager.ToInt(it.ListNo)).ToList();
            }
        }
        #endregion
        #endregion

        #region Site
        public SendMailUsing GetSiteSendMailMethod(long siteId)
        {
            return (SendMailUsing)DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT [SendMailUsing] FROM [Site] WHERE [SiteId] = @SiteId",
                new SqlParameter("@SiteId", siteId)));
        }
        #endregion


        #region EmailTemplateAttachment
        public int AddEmailTemplateAttachment(EmailTemplateAttachment pObj)
        {
            string sSql = @"INSERT INTO EmailTemplateAttachment (TemplateId, FileName, BaseName, FileExtension, FilePath)
                VALUES(@TemplateId, @FileName, @BaseName, @FileExtension, @FilePath)
                SELECT @@IDENTITY";

            return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@TemplateId", pObj.TemplateId),
                new SqlParameter("@FileName", pObj.FileName),
                new SqlParameter("@BaseName", pObj.BaseName),
                new SqlParameter("@FileExtension", pObj.FileExtension),
                new SqlParameter("@FilePath", pObj.FilePath)));
        }

        public int UpdateEmailTemplateAttachment(EmailTemplateAttachment pObj)
        {
            string sSql = @"UPDATE EmailTemplateAttachment
                SET TemplateId = @TemplateId, FileName = @FileName, BaseName = @BaseName, FileExtension = @FileExtension, FilePath = @FilePath
                WHERE EmailTemplateAttachmentId = @EmailTemplateAttachmentId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@EmailTemplateAttachmentId", pObj.EmailTemplateAttachmentId),
                new SqlParameter("@TemplateId", pObj.TemplateId),
                new SqlParameter("@FileName", pObj.FileName),
                new SqlParameter("@BaseName", pObj.BaseName),
                new SqlParameter("@FileExtension", pObj.FileExtension),
                new SqlParameter("@FilePath", pObj.FilePath));
        }

        public EmailTemplateAttachment GetEmailTemplateAttachment(long id)
        {
            EmailTemplateAttachment obj = null;
            var db = GetModels();
            List<EmailTemplateAttachment> list = (from it in db.EmailTemplateAttachments
                                                  where it.EmailTemplateAttachmentId == id
                                                  orderby it.FileName
                                                  select it).ToList();

            if (list.Any())
            {
                obj = list.First();
            }

            return obj;
        }

        public List<EmailTemplateAttachment> GetEmailTemplateAttachmentsByTemplateId(long templateId)
        {
            var db = GetModels();
            List<EmailTemplateAttachment> list = (from it in db.EmailTemplateAttachments
                                                  where it.TemplateId == templateId
                                                  orderby it.FileName
                                                  select it).ToList();
            return list;
        }

        public bool IsExistsEmailTemplateAttachments(long templateId)
        {
            return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT COUNT(EmailTemplateAttachmentId) FROM EmailTemplateAttachment WHERE TemplateId = " + templateId)) > 0;
        }

        public int DeleteEmailTemplateAttachment(long id)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM EmailTemplateAttachment WHERE EmailTemplateAttachmentId = " + id);
        }
        #endregion


        #region EmailingAttachment
        public int AddEmailingAttachment(EmailingAttachment pObj)
        {
            string sSql = @"INSERT INTO EmailingAttachment (EmailId, FileName, BaseName, FileExtension, FilePath)
                VALUES(@EmailId, @FileName, @BaseName, @FileExtension, @FilePath)
                SELECT @@IDENTITY";

            return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@EmailId", pObj.EmailId),
                new SqlParameter("@FileName", pObj.FileName),
                new SqlParameter("@BaseName", pObj.BaseName),
                new SqlParameter("@FileExtension", pObj.FileExtension),
                new SqlParameter("@FilePath", pObj.FilePath)));
        }

        public int UpdateEmailingAttachment(EmailingAttachment pObj)
        {
            string sSql = @"UPDATE EmailingAttachment
                SET EmailId = @EmailId, FileName = @FileName, BaseName = @BaseName, FileExtension = @FileExtension, FilePath = @FilePath
                WHERE EmailingAttachmentId = @EmailingAttachmentId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@EmailingAttachmentId", pObj.EmailingAttachmentId),
                new SqlParameter("@EmailId", pObj.EmailId),
                new SqlParameter("@FileName", pObj.FileName),
                new SqlParameter("@BaseName", pObj.BaseName),
                new SqlParameter("@FileExtension", pObj.FileExtension),
                new SqlParameter("@FilePath", pObj.FilePath));
        }

        public EmailingAttachment GetEmailingAttachment(long id)
        {
            EmailingAttachment obj = null;
            var db = GetModels();
            List<EmailingAttachment> list = (from it in db.EmailingAttachments
                                             where it.EmailingAttachmentId == id
                                             orderby it.FileName
                                             select it).ToList();

            if (list.Any())
            {
                obj = list.First();
            }

            return obj;
        }

        public List<EmailingAttachment> GetEmailingAttachmentsByEmailId(long emailId)
        {
            var db = GetModels();
            List<EmailingAttachment> list = (from it in db.EmailingAttachments
                                             where it.EmailId == emailId
                                             orderby it.FileName
                                             select it).ToList();
            return list;
        }

        public bool IsExistsEmailingAttachments(long emailId)
        {
            return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT COUNT(EmailingAttachmentId) FROM EmailingAttachment WHERE EmailId = " + emailId)) > 0;
        }

        public int DeleteEmailingAttachment(long id)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM EmailingAttachment WHERE EmailingAttachmentId = " + id);
        }
        #endregion

        #region NetMessageCampaign
        public List<NetMessageCampaign> GetPendingNetmessageCampaigns()
        {
            var db = GetModels();
            List<NetMessageCampaign> campaigns = (from it in db.NetMessageCampaigns
                                                  where it.Status.HasValue && it.Status == 0
                                                  orderby it.EmailId
                                                  select it).ToList();
            return campaigns;
        }

        public bool GetNetmessageSettingByEmailId(long emailId, out string username, out string password, out string accountName)
        {
            bool result = false;
            string sql = @"DECLARE @SiteId BIGINT;
                           SELECT @SiteId = SiteId FROM [Emailing] WHERE EmailId = @EmailId;
                            IF EXISTS(SELECT SiteId FROM [Site] WHERE SiteId = @SiteId AND IsUseGlobalNetmessageSettings = 0)
                               BEGIN
                                    SELECT NetmessageFTPUsername, NetmessageFTPPassword, NetmessageAccountName
                                    FROM [Site] WHERE [Site].[SiteId] = @SiteId
								END
                           ELSE
                               BEGIN
                                    SELECT OptionKey, OptionValue FROM [Options] WHERE [OptionKey] IN('NetmessageFTPUsername', 'NetmessageFTPPassword', 'NetmessageAccountName')
                               END";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@EmailId", emailId));

            if (ds.Tables[0].Rows.Count == 1)
            {
                DataRow row = ds.Tables[0].Rows[0];
                username = DataManager.ToString(row["NetmessageFTPUsername"]);
                password = DataManager.ToString(row["NetmessageFTPPassword"]);
                accountName = DataManager.ToString(row["NetmessageAccountName"]);
                result = true;
            }
            else if (ds.Tables[0].Rows.Count == 3)
            {
                username = string.Empty;
                password = string.Empty;
                accountName = string.Empty;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    switch (DataManager.ToString(row["OptionKey"]))
                    {
                        case "NetmessageFTPUsername":
                            username = DataManager.ToString(row["OptionValue"]);
                            break;
                        case "NetmessageFTPPassword":
                            password = DataManager.ToString(row["OptionValue"]);
                            break;
                        case "NetmessageAccountName":
                            accountName = DataManager.ToString(row["OptionValue"]);
                            break;
                    }
                }
                result = true;
            }
            else
            {
                username = string.Empty;
                password = string.Empty;
                accountName = string.Empty;
            }
            return result;
        }

        public List<NetMessageCampaign> GetNetmessageCampaignsByEmailId(long emailId)
        {
            List<NetMessageCampaign> list = null;
            using (var db = GetModels())
            {
                list = (from it in db.NetMessageCampaigns
                        where it.EmailId == emailId && it.Status.HasValue && it.Status.Value > -1 && it.JobNumber != null && it.JobNumber != ""
                        select it).ToList();
            }

            return list;
        }

        public long AddNetmessageCampaign(NetMessageCampaign obj)
        {
            string sql = @"INSERT INTO [dbo].[NetMessageCampaign] ([EmailId], [RefCode], [JobNumber], [CreatedDate], [UpdatedDate], [Status], [ErrorMessage])
                           VALUES(@EmailId, @RefCode, @JobNumber, @CreatedDate, @UpdatedDate, @Status, @ErrorMessage) SELECT @@IDENTITY";
            return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@EmailId", obj.EmailId),
                new SqlParameter("@RefCode", obj.RefCode),
                new SqlParameter("@JobNumber", obj.JobNumber),
                new SqlParameter("@CreatedDate", obj.CreatedDate),
                new SqlParameter("@UpdatedDate", obj.UpdatedDate),
                new SqlParameter("@Status", obj.Status),
                new SqlParameter("@ErrorMessage", obj.ErrorMessage)));
        }

        public void UpdateNetmessageCampaignJobNumber(long netmessageCampaignId, string jobNumber)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "UPDATE NetMessageCampaign SET JobNumber = @JobNumber WHERE NetMessageCampaignId = @NetMessageCampaignId",
                new SqlParameter("@NetMessageCampaignId", netmessageCampaignId),
                new SqlParameter("@JobNumber", jobNumber));
        }

        public void SaveNetmessageReport(NetmessageReport report)
        {
            // Check if exists report.
            string sql = "SELECT NetmessageReportId FROM NetmessageReport WHERE EmailId = @EmailId AND ReportDate = @ReportDate";
            long netmessageReportId = DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@EmailId", report.EmailId),
                new SqlParameter("@ReportDate", report.ReportDate)));

            if (netmessageReportId <= 0)
            {
                // Create a new report.
                sql = @"INSERT INTO [NetmessageReport]([EmailId], [ReportDate], [Active]) VALUES(@EmailId, @ReportDate, @Active) SELECT @@IDENTITY";
                netmessageReportId = DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                    new SqlParameter("@EmailId", report.EmailId),
                    new SqlParameter("@ReportDate", report.ReportDate),
                    new SqlParameter("@Active", report.Active)));
            }

            // Update report's records.
            if (netmessageReportId > 0)
            {
                foreach (var record in report.NetmessageReportRecords)
                {
                    record.NetmessageReportRecordId = DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, @"SELECT TOP(1) NetmessageReportRecordId FROM [NetmessageReportRecord] WHERE [NetmessageReportId] = @NetmessageReportId AND [Email] = @Email",
                        new SqlParameter("@NetmessageReportId", netmessageReportId),
                        new SqlParameter("@Email", record.Email)));

                    if (record.NetmessageReportRecordId > 0)
                    {
                        UpdateNetmessageReportRecord(netmessageReportId, record);
                    }
                    else
                    {
                        InsertNetmessageReportRecord(netmessageReportId, record);
                    }
                }
            }
        }

        private void UpdateNetmessageReportRecord(long netmessageReportId, NetmessageReportRecord record)
        {
            string sql = @"UPDATE [NetmessageReportRecord]
                           SET [NetmessageReportId] = @NetmessageReportId
                              ,[Email] = @Email
                              ,[FirstName] = @FirstName
                              ,[LastName] = @LastName
                              ,[Name] = @Name
                              ,[Gender] = @Gender
                              ,[Phone] = @Phone
                              ,[Mobile] = @Mobile
                              ,[Description] = @Description
                              ,[Profession] = @Profession
                              ,[Index] = @Index
                              ,[Field1] = @Field1
                              ,[Field2] = @Field2
                              ,[Field3] = @Field3
                              ,[Status] = @Status
                              ,[OpenTime] = @OpenTime
                              ,[UnsubTime] = @UnsubTime
                              ,[SubTime] = @SubTime
                              ,[RadiateTime] = @RadiateTime
                              ,[ViewTime] = @ViewTime
                              ,[ClickTime] = @ClickTime
                              ,[IsOpen] = @IsOpen
                              ,[IsUnsub] = @IsUnsub
                              ,[IsSub] = @IsSub
                              ,[IsRadiate] = @IsRadiate
                              ,[IsView] = @IsView
                              ,[IsClick] = @IsClick
                              ,[IsSent] = @IsSent
                              ,[SentTime] = @SentTime
                            WHERE [NetmessageReportRecordId] = @NetmessageReportRecordId";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@NetmessageReportId", netmessageReportId),
                new SqlParameter("@NetmessageReportRecordId", record.NetmessageReportRecordId),
                new SqlParameter("@Email", record.Email),
                new SqlParameter("@FirstName", record.FirstName),
                new SqlParameter("@LastName", record.LastName),
                new SqlParameter("@Name", record.Name),
                new SqlParameter("@Gender", record.Gender),
                new SqlParameter("@Phone", record.Phone),
                new SqlParameter("@Mobile", record.Mobile),
                new SqlParameter("@Description", record.Description),
                new SqlParameter("@Profession", record.Profession),
                new SqlParameter("@Index", record.Index),
                new SqlParameter("@Field1", record.Field1),
                new SqlParameter("@Field2", record.Field2),
                new SqlParameter("@Field3", record.Field3),
                new SqlParameter("@IsSent", record.IsSent),
                new SqlParameter("@IsOpen", record.IsOpen),
                new SqlParameter("@IsUnsub", record.IsUnsub),
                new SqlParameter("@IsSub", record.IsSub),
                new SqlParameter("@IsRadiate", record.IsRadiate),
                new SqlParameter("@IsView", record.IsView),
                new SqlParameter("@IsClick", record.IsClick),
                new SqlParameter("@SentTime", record.SentTime),
                new SqlParameter("@OpenTime", record.OpenTime),
                new SqlParameter("@UnsubTime", record.UnsubTime),
                new SqlParameter("@SubTime", record.SubTime),
                new SqlParameter("@RadiateTime", record.RadiateTime),
                new SqlParameter("@ViewTime", record.ViewTime),
                new SqlParameter("@ClickTime", record.ClickTime),
                new SqlParameter("@Status", record.Status));
        }

        private void InsertNetmessageReportRecord(long netmessageReportId, NetmessageReportRecord record)
        {
            string sql = @"INSERT INTO [NetmessageReportRecord]([NetmessageReportId]
                                                                   ,[Email]
                                                                   ,[FirstName]
                                                                   ,[LastName]
                                                                   ,[Name]
                                                                   ,[Gender]
                                                                   ,[Phone]
                                                                   ,[Mobile]
                                                                   ,[Description]
                                                                   ,[Profession]
                                                                   ,[Index]
                                                                   ,[Field1]
                                                                   ,[Field2]
                                                                   ,[Field3]
                                                                   ,[IsSent]
                                                                   ,[IsOpen]
                                                                   ,[IsUnsub]
                                                                   ,[IsSub]
                                                                   ,[IsRadiate]
                                                                   ,[IsView]
                                                                   ,[IsClick]
                                                                   ,[SentTime]
                                                                   ,[OpenTime]
                                                                   ,[UnsubTime]
                                                                   ,[SubTime]
                                                                   ,[RadiateTime]
                                                                   ,[ViewTime]
                                                                   ,[ClickTime]
                                                                   ,[Status])
                                                            VALUES(@NetmessageReportId
                                                                   ,@Email
                                                                   ,@FirstName
                                                                   ,@LastName
                                                                   ,@Name
                                                                   ,@Gender
                                                                   ,@Phone
                                                                   ,@Mobile
                                                                   ,@Description
                                                                   ,@Profession
                                                                   ,@Index
                                                                   ,@Field1
                                                                   ,@Field2
                                                                   ,@Field3
                                                                   ,@IsSent
                                                                   ,@IsOpen
                                                                   ,@IsUnsub
                                                                   ,@IsSub
                                                                   ,@IsRadiate
                                                                   ,@IsView
                                                                   ,@IsClick
                                                                   ,@SentTime
                                                                   ,@OpenTime
                                                                   ,@UnsubTime
                                                                   ,@SubTime
                                                                   ,@RadiateTime
                                                                   ,@ViewTime
                                                                   ,@ClickTime
                                                                   ,@Status) SELECT @@IDENTITY";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@NetmessageReportId", netmessageReportId),
                new SqlParameter("@Email", record.Email),
                new SqlParameter("@FirstName", record.FirstName),
                new SqlParameter("@LastName", record.LastName),
                new SqlParameter("@Name", record.Name),
                new SqlParameter("@Gender", record.Gender),
                new SqlParameter("@Phone", record.Phone),
                new SqlParameter("@Mobile", record.Mobile),
                new SqlParameter("@Description", record.Description),
                new SqlParameter("@Profession", record.Profession),
                new SqlParameter("@Index", record.Index),
                new SqlParameter("@Field1", record.Field1),
                new SqlParameter("@Field2", record.Field2),
                new SqlParameter("@Field3", record.Field3),
                new SqlParameter("@IsSent", record.IsSent),
                new SqlParameter("@IsOpen", record.IsOpen),
                new SqlParameter("@IsUnsub", record.IsUnsub),
                new SqlParameter("@IsSub", record.IsSub),
                new SqlParameter("@IsRadiate", record.IsRadiate),
                new SqlParameter("@IsView", record.IsView),
                new SqlParameter("@IsClick", record.IsClick),
                new SqlParameter("@SentTime", record.SentTime),
                new SqlParameter("@OpenTime", record.OpenTime),
                new SqlParameter("@UnsubTime", record.UnsubTime),
                new SqlParameter("@SubTime", record.SubTime),
                new SqlParameter("@RadiateTime", record.RadiateTime),
                new SqlParameter("@ViewTime", record.ViewTime),
                new SqlParameter("@ClickTime", record.ClickTime),
                new SqlParameter("@Status", record.Status));
        }

        public bool HasNetmessageCampaigns(int emailId)
        {
            string sql = @"SELECT COUNT(*) FROM NetmessageCampaign WHERE EmailId = " + emailId;
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql)) > 0;
        }
        #endregion

        #region Brand
        public IQueryable<BrandViewModel> GetListBrands()
        {
            var db = GetModels();

            var query = from it in db.Brands
                        where it.BrandId > 0 && it.Active == true
                        select new BrandViewModel
                        {
                            BrandId = it.BrandId,
                            BrandName = it.BrandName
                        };

            return query.OrderBy(it => it.BrandName);
        }

        public List<Brand> GetBrandDropDownList()
        {
            List<Brand> list = null;
            using (var db = GetModels())
            {
                list = (from it in db.Brands
                        where it.Active.HasValue && it.Active.Value
                        select it).ToList();
            }
            return list;
        }

        public Brand GetBrandById(long id)
        {
            Brand obj = null;

            using (var db = GetModels())
            {
                var query = (from it in db.Brands
                             where it.BrandId == id && it.Active == true
                             select it).ToList();

                if (query != null && query.Any())
                {
                    obj = query.First();
                }
            }

            return obj;
        }

        public int AddBrand(Brand obj)
        {
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                  "INSERT INTO Brand(BrandName, Active) VALUES(@BrandName, @Active) SELECT @@IDENTITY",
                  new SqlParameter("@BrandName", obj.BrandName),
                  new SqlParameter("@Active", obj.Active)), -1);
        }

        public int UpdateBrand(Brand obj)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                  "Update Brand Set [BrandName] = @BrandName, [Active] = @Active Where BrandId = @BrandId",
                  new SqlParameter("@BrandName", obj.BrandName),
                  new SqlParameter("@Active", obj.Active),
                  new SqlParameter("@BrandId", obj.BrandId));
        }

        public int DeleteBrand(long id)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "UPDATE Brand SET Active = 0 WHERE BrandId =" + id);
        }
        #endregion

        #region CustomerGroup
        public List<CustomerGroup> GetSyncAvailableCustomerGroups(long siteId)
        {
            List<CustomerGroup> customerGroups = null;
            using (var db = GetModels())
            {
                customerGroups = (from it in db.CustomerGroups.Include("CustomerTypes")
                                  where it.Active == true && it.CustomerTypes.Any()
                                  select it).ToList();
            }
            return customerGroups;
        }
        public List<CustomerGroup> GetAutoSyncCustomerGroups(long siteId)
        {
            List<CustomerGroup> customerGroups = null;
            using (var db = GetModels())
            {
                customerGroups = (from it in db.CustomerGroups.Include("CustomerTypes")
                                  where it.Active == true && it.CustomerTypes.Any() && it.AutoSync
                                  select it).ToList();
            }
            return customerGroups;
        }

        public void SyncCustomerGroup(long customerGroupId)
        {
            CustomerGroup group = GetCustomerGroupWithCustomerTypes(customerGroupId);
            if (group == null || group.SyncInProcess)
                return;

            int clubId = GetSiteChronogolfClubId(group.SiteId);
            if (clubId <= 0)
                return;

            ChronogolfGetAllCustomersResponse response = null;
            var chronogolf = DataFactory.GetChronogolfInstance(clubId);

            response = chronogolf.GetAllCustomers();
            if (response.Customers.Count > 0)
            {
                DoSyncCustomerGroup(response.Customers, group);
            }

            if(response.Errors != null)
            {
                throw response.Errors;
            }
        }

        private CustomerGroup GetCustomerGroupWithCustomerTypes(long customerGroupId)
        {
            CustomerGroup group = null;
            using (var db = GetModels())
            {
                group = (from it in db.CustomerGroups.Include("CustomerTypes")
                         where it.CustomerGroupId == customerGroupId && it.Active == true && it.CustomerTypes.Any()
                         select it).FirstOrDefault();
            }
            return group;
        }

        public void SyncAllCustomerGroups(long siteId, bool all = false)
        {
            int clubId = GetSiteChronogolfClubId(siteId);
            if (clubId <= 0)
                return;

            ChronogolfGetAllCustomersResponse response;
            ChronogolfDataAccess chronogolf = DataFactory.GetChronogolfInstance(clubId);

            List<Exception> exceptions = new List<Exception>();

            var customerGroups = all ? GetSyncAvailableCustomerGroups(siteId) : GetAutoSyncCustomerGroups(siteId);
            response = chronogolf.GetAllCustomers();
            if (response.Customers.Count <= 0)
                return;

            foreach (var group in customerGroups)
            {
                DoSyncCustomerGroup(response.Customers, group);

                if(response.Errors != null)
                {
                    exceptions.Add(response.Errors);
                }
            }

            // Throw easy-to-ready exception if exists.
            if(exceptions.Any())
            {
                string[] msgs = exceptions.Select(it => it.Message).ToArray();
                throw new Exception(String.Join(Environment.NewLine + Environment.NewLine + "*****************" + Environment.NewLine + Environment.NewLine, msgs));
            }
        }

        private void DoSyncCustomerGroup(List<ChronogolfCustomer> customers, CustomerGroup group)
        {
            var customerTypes = group.CustomerTypes;
            if (!customerTypes.Any())
                return;

            //if (!keep)
            //{
            //    DeleteCustomerGroupItemsByCustomerGroupId(group.CustomerGroupId);
            //}

            SetCustomerGroupSyncStatus(group.CustomerGroupId, true);

            foreach (var type in customerTypes)
            {
                SaveChronogolfCustomersIntoGroup(group.SiteId,
                    group.CustomerGroupId,
                    type.CustomerTypeId,
                    customers.Where(it => it.AffiliationTypeId == type.AffiliationTypeId).ToList()
                    );
            }

            SetCustomerGroupSyncStatus(group.CustomerGroupId, false);
        }

        private void SetCustomerGroupSyncStatus(int customerGroupId, bool syncInProcess)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "UPDATE CustomerGroup SET SyncInProcess = @SyncInProcess WHERE CustomerGroupId = @CustomerGroupId",
                new SqlParameter("@SyncInProcess", syncInProcess),
                new SqlParameter("@CustomerGroupId", customerGroupId));
        }

        private void SaveChronogolfCustomersIntoGroup(long siteId, int customerGroupId, long customerTypeId, List<ChronogolfCustomer> customers)
        {
            using (var db = GetModels())
            {
                foreach (var c in customers)
                {
                    try
                    {
                        // Getting customer by site ID, if not exists just creating new customer.
                        var customer = db.Users.Where(it => it.Email == c.Email && it.SiteId == siteId).FirstOrDefault();
                        if (customer == null)
                        {
                            customer = this.GetNewUser();
                        }

                        customer.Email = c.Email;
                        customer.FirstName = c.FirstName;
                        customer.LastName = c.LastName;
                        customer.Phone = c.Phone;
                        customer.MobilePhone = c.Phone;
                        customer.CustomerTypeId = customerTypeId;
                        customer.UserTypeId = UserType.Type.Customer;
                        customer.IsReceiveEmailInfo = true;
                        customer.IsSubscriber = true;
                        customer.SiteId = siteId;
                        customer.Active = true;

                        // Save customer.
                        if (customer.UserId > 0)
                        {
                            UpdateUser(customer);
                        }
                        else
                        {
                            customer.UserId = AddUser(customer);
                        }

                        // Adding customer to group, skip when duplicate.
                        AddCustomerGroupCustomer(customerGroupId, customer.UserId);
                    }
                    catch
                    {
                        // Skip.
                        continue;
                    }
                }
            }
        }

        public bool DeleteAllCustomerGroupsCustomerTypes(long id)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, @"DELETE FROM CustomerGroupCustomerType WHERE CustomerGroupId = @CustomerGroupId",
                new SqlParameter("@CustomerGroupId", id)) > 0;
        }

        public bool SaveCustomerGroupCustomerType(int customerGroupId, long customerTypeId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                @"IF NOT EXISTS (SELECT CustomerGroupId FROM CustomerGroupCustomerType WHERE CustomerGroupId = @CustomerGroupId AND CustomerTypeId = @CustomerTypeId)
                      BEGIN
                        INSERT INTO CustomerGroupCustomerType(CustomerGroupId, CustomerTypeId) VALUES(@CustomerGroupId, @CustomerTypeId) SELECT @@IDENTITY;
                      END",
                new SqlParameter("@CustomerGroupId", customerGroupId),
                new SqlParameter("@CustomerTypeId", customerTypeId)) > 0;
        }
        #endregion
    }
}
