using DansLesGolfs.Base;
using DansLesGolfs.BLL;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;

namespace DansLesGolfs.Data
{
    public partial class SqlDataAccess : IDisposable
    {
        #region Fields
        private string _connectionString;
        private string _entityConnectionString;
        private SqlTransaction trans;
        private SqlConnection conn;
        #endregion

        #region Properties
        public string ConnectionString
        {
            get { return _connectionString; }
            private set { _connectionString = value; }
        }
        public string EntityConnectionString
        {
            get { return _entityConnectionString; }
            private set { _entityConnectionString = value; }
        }

        public SqlTransaction Transaction
        {
            get
            {
                return trans;
            }
        }

        #endregion

        #region Constructors
        public SqlDataAccess(string connectionString)
        {
            _connectionString = connectionString;

            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder(connectionString);

            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";
            entityBuilder.ProviderConnectionString = sqlBuilder.ToString();
            entityBuilder.Metadata = @"res://*/Entities.DLGModel.csdl|
                                        res://*/Entities.DLGModel.ssdl|
                                        res://*/Entities.DLGModel.msl";
            _entityConnectionString = entityBuilder.ToString();
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            SqlConnection.ClearAllPools();
        }
        #endregion

        #region BeginTransaction()
        public void BeginTransaction()
        {
            conn = new SqlConnection(ConnectionString);
            conn.Open();
            trans = conn.BeginTransaction();
        }
        #endregion

        #region CommitTransaction()
        public void CommitTransaction()
        {
            trans.Commit();
            CloseConnection();
        }
        #endregion

        #region RollbackTransaction()
        public void RollbackTransaction()
        {
            if (trans != null)
                trans.Rollback();
            CloseConnection();
        }
        #endregion

        #region CloseConnection
        private void CloseConnection()
        {
            if (trans != null)
            {
                trans.Dispose();
                trans = null;
            }
            if (conn != null)
            {
                conn.Dispose();
                conn.Close();
                conn = null;
            }
        }
        #endregion

        #region ChangeConnectionString
        public void ChangeConnectionString(string connectionStringName)
        {
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }
        #endregion

        public DLGEntities GetModels()
        {
            var db = new DLGEntities(EntityConnectionString);
            db.Configuration.ProxyCreationEnabled = false;
            return db;
        }

        #region GetEntityConnectionString
        private static string GetEntityConnectionString(string connectionString, System.Data.Entity.DbContext db)
        {
            string provider = "System.Data.SqlClient";
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder(connectionString);
            string providerString = sqlBuilder.ToString();

            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder(db.Database.Connection.ConnectionString);
            entityBuilder.Provider = provider;
            entityBuilder.ProviderConnectionString = providerString;
            return entityBuilder.ToString();
        }
        #endregion

        #region GetSelectList
        public DataSet GetSelectList(string tableName, jQueryDataTableParamModel param)
        {
            return SqlHelper.ExecuteDataset(ConnectionString, "SelectList",
                new SqlParameter("@TableName", tableName),
                new SqlParameter("@Start", param.start),
                new SqlParameter("@Length", param.length),
                new SqlParameter("@Search", param.search),
                new SqlParameter("@Order", param.order));
        }
        #endregion

        #region Options
        /// <summary>
        /// Get all options.
        /// </summary>
        /// <returns>Dictionary of options in key-value pairs format.</returns>
        public Dictionary<string, string> GetOptions()
        {
            string sql = "SELECT  [OptionKey], [OptionValue] FROM [Options]";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
            Dictionary<string, string> options = new Dictionary<string, string>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                options.Add(DataManager.ToString(row["OptionKey"]), DataManager.ToString(row["OptionValue"]));
            }
            return options;
        }

        /// <summary>
        /// Get options by specific keys.
        /// </summary>
        /// <param name="keys">Option keys</param>
        /// <returns>Dictionary of options in key-value pairs format.</returns>
        public Dictionary<string, string> GetOptions(params string[] keys)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            var keyStrings = (keys != null || keys.Any()) ? String.Join(",", keys.Select(it => "'" + it.Trim() + "'")) : "";
            if (!String.IsNullOrEmpty(keyStrings) && !String.IsNullOrWhiteSpace(keyStrings))
            {
                string sql = "SELECT  [OptionKey], [OptionValue] FROM [Options] WHERE [OptionKey] IN (" + keyStrings + ")";
                DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
                DataRow[] rows = null;
                for (int i = 0, n = keys.Length; i < n; i++)
                {
                    rows = ds.Tables[0].Select("OptionKey = '" + keys[i] + "'");
                    if (rows.Any())
                    {
                        options.Add(keys[i], DataManager.ToString(rows[0]["OptionValue"]));
                    }
                    else
                    {
                        options.Add(keys[i], string.Empty);
                    }
                }
            }
            return options;
        }

        /// <summary>
        /// Get option by key
        /// </summary>
        /// <param name="key">Option key</param>
        /// <returns>Option value</returns>
        public string GetOption(string key)
        {
            string sql = "SELECT TOP(1) [OptionValue] FROM [Options] WHERE [OptionKey] = @OptionKey";
            return DataManager.ToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@OptionKey", key)));
        }

        #region SaveOption
        /// <summary>
        /// Save option by key & value.
        /// </summary>
        /// <param name="key">Option key.</param>
        /// <param name="value">Option value.</param>
        /// <returns>True if save success, otherwise is False.</returns>
        public bool SaveOption(string key, object value)
        {
            string valueStr = value == null ? string.Empty : value.ToString();
            string sql = @"IF EXISTS(SELECT [OptionKey] FROM [Options] WHERE [OptionKey] = @OptionKey)
                                BEGIN
                                    UPDATE [Options] SET [OptionValue] = @OptionValue WHERE [OptionKey] = @OptionKey
                                END
                            ELSE
                                BEGIN
                                    INSERT INTO [Options]([OptionKey], [OptionValue]) VALUES(@OptionKey, @OptionValue)
                                END";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@OptionKey", key),
                new SqlParameter("@OptionValue", valueStr)) > 0;
        }
        #endregion

        public bool SaveOptions(Dictionary<string, string> options)
        {
            string sql = string.Empty;
            bool result = true;

            foreach (var it in options)
            {
                sql = @"IF EXISTS(SELECT [OptionKey] FROM [Options] WHERE [OptionKey] = @OptionKey)
                            BEGIN
                                UPDATE [Options] SET [OptionValue] = @OptionValue WHERE [OptionKey] = @OptionKey;
                            END
                        ELSE
                            BEGIN
                                INSERT INTO [Options]([OptionKey], [OptionValue]) VALUES(@OptionKey, @OptionValue);
                            END
                            SELECT 1;";
                if (SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                    new SqlParameter("@OptionKey", it.Key),
                    new SqlParameter("@OptionValue", it.Value)) <= 0)
                {
                    result = false;
                }
            }

            return result;
        }
        #endregion

        #region Title

        #region GetItemTitlesDropDownList
        public List<Title> GetItemTitlesDropDownList(int langId = 1)
        {
            List<Title> list = new List<Title>();
            string sql = @"SELECT [Title].[TitleId], [TitleLang].[TitleName] FROM [Title]
                            LEFT JOIN [TitleLang] ON [TitleLang].[TitleId] = [Title].[TitleId] AND [TitleLang].[LangId] = @LangId
                            WHERE [Active] = 1 ORDER BY [TitleId]";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@LangID", langId));

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new Title()
                {
                    TitleId = DataManager.ToInt(row["TitleId"]),
                    TitleName = DataManager.ToString(row["TitleName"])
                });
            }
            return list;
        }
        #endregion

        #endregion

        #region Users



        #region GetNewUser
        public User GetNewUser()
        {
            DateTime now = DateTime.Now;
            return new User()
            {
                ExpiredDate = now.AddYears(100),
                InsertDate = now,
                UpdateDate = now,
                RegisteredDate = now,
                Birthdate = now,
                Active = true,
                IsSubscriber = true,
                IsReceiveEmailInfo = true,
                UserTypeId = UserType.Type.Customer
            };
        }
        #endregion

        #region GetAllUsers
        public IQueryable<User> GetAllUsers(jQueryDataTableParamModel param, int langId = 1)
        {
            using (var db = GetModels())
            {
                var list = (from it in db.Users

                            join s in db.SiteLangs.Where(it => it.LangId == langId) on it.SiteId equals s.SiteId into sj
                            from s in sj.DefaultIfEmpty()

                            where it.Active == true && (it.UserTypeId == (int)UserType.Type.SuperAdmin || it.UserTypeId == (int)UserType.Type.Admin)

                            select new
                            {
                                User = it,
                                Site = s
                            });

                foreach (var it in list)
                {
                    it.User.SiteName = it.Site != null ? it.Site.SiteName : "";
                }

                return list.Select(it => it.User);
            }
        }

        #endregion

        #region GetAllCustomer
        public IQueryable<User> GetAllCustomers(long? siteId = null, int? langId = 1)
        {
            using (var db = GetModels())
            {
                var list = (from it in db.Users

                            join s in db.SiteLangs.Where(it => it.LangId == langId) on it.SiteId equals s.SiteId into sj
                            from s in sj.DefaultIfEmpty()

                            where it.Active == true && (it.UserTypeId == (int)UserType.Type.Customer)

                            select new
                            {
                                User = it,
                                Site = s
                            });

                if (siteId.HasValue)
                {
                    list = list.Where(it => it.Site.SiteId == siteId.Value);
                }

                foreach (var it in list)
                {
                    it.User.SiteName = it.Site != null ? it.Site.SiteName : "";
                }

                return list.Select(it => it.User);
            }
        }

        #endregion
        #region GetAllResellers
        public IQueryable<User> GetAllResellers(int langId = 1)
        {
            using (var db = GetModels())
            {
                var list = (from it in db.Users

                            join s in db.SiteLangs.Where(it => it.LangId == langId) on it.SiteId equals s.SiteId into sj
                            from s in sj.DefaultIfEmpty()

                            where it.Active == true && (it.UserTypeId == (int)UserType.Type.Reseller)

                            select new
                            {
                                User = it,
                                Site = s
                            });

                foreach (var it in list)
                {
                    it.User.SiteName = it.Site != null ? it.Site.SiteName : "";
                }

                return list.Select(it => it.User);
            }
        }
        #endregion
        #region GetAllECMSubscribers
        public List<User> GetAllECMSubscribers(long? siteId = null, int langId = 1)
        {
            using (var db = GetModels())
            {
                var list = (from it in db.Users

                            join s in db.SiteLangs.Where(it => it.LangId == langId) on it.SiteId equals s.SiteId into sj
                            from s in sj.DefaultIfEmpty()

                            where it.Active == true && it.UserTypeId == (int)UserType.Type.Customer && it.IsSubscriber == true && it.IsReceiveEmailInfo == true

                            select new
                            {
                                SubScriber = it,
                                Site = s
                            });

                if (siteId.HasValue)
                {
                    list = list.Where(it => it.SubScriber.SiteId == siteId.Value);
                }

                foreach (var it in list)
                {
                    it.SubScriber.SiteName = it.Site != null ? it.Site.SiteName : "";
                }

                return list.Select(it => it.SubScriber).ToList();
            }
        }
        #endregion
        #region GetListECMSubscribers
        public IQueryable<UserViewModel> GetListECMSubscribers(long? siteId = null, int langId = 1)
        {
            var db = GetModels();
            var list = (from it in db.Users

                        join s in db.SiteLangs.Where(it => it.LangId == langId) on it.SiteId equals s.SiteId into sj
                        from s in sj.DefaultIfEmpty()

                        join ct in db.CustomerTypeLangs.Where(it => it.LangId == langId) on it.CustomerTypeId equals ct.CustomerTypeId into ctj
                        from ct in ctj.DefaultIfEmpty()

                        join sct in db.CustomerTypeLangs.Where(it => it.LangId == langId) on it.SubCustomerTypeId equals sct.CustomerTypeId into sctj
                        from sct in sctj.DefaultIfEmpty()

                        where it.UserId > 0 && it.Active == true && it.UserTypeId == (int)UserType.Type.Customer && it.IsSubscriber == true && it.IsReceiveEmailInfo == true && (it.Email != null && it.Email.Length > 0)

                        select new UserViewModel
                        {
                            UserId = it.UserId,
                            UserTypeId = it.UserTypeId,
                            Email = it.Email,
                            FirstName = it.FirstName,
                            LastName = it.LastName,
                            SiteName = s.SiteName,
                            CustomerTypeName = ct.CustomerTypeName,
                            SubCustomerTypeName = sct.CustomerTypeName,
                            SiteId = it.SiteId,
                            CustomerTypeId = it.CustomerTypeId.HasValue ? it.CustomerTypeId.Value : 0,
                            SubCustomerTypeId = it.SubCustomerTypeId,
                            InsertDate = it.InsertDate
                        });

            if (siteId.HasValue)
            {
                list = list.Where(it => it.SiteId == siteId.Value);
            }

            return list;
        }
        #endregion
        #region GetAllECMUnsubscribers
        public IQueryable<User> GetAllECMUnsubscribers(jQueryDataTableParamModel param, long? siteId = null, int? langId = 1)
        {
            using (var db = GetModels())
            {
                var list = (from it in db.Users

                            join s in db.SiteLangs.Where(it => it.LangId == langId) on it.SiteId equals s.SiteId into sj
                            from s in sj.DefaultIfEmpty()

                            where it.Active == true && it.UserTypeId == (int)UserType.Type.Customer && (!it.IsSubscriber.HasValue || it.IsSubscriber == false) && (!it.IsReceiveEmailInfo.HasValue || it.IsReceiveEmailInfo == false)

                            select new
                            {
                                SubScriber = it,
                                Site = s
                            });

                if (siteId.HasValue)
                {
                    list = list.Where(it => it.SubScriber.SiteId == siteId.Value);
                }

                foreach (var it in list)
                {
                    it.SubScriber.SiteName = it.Site != null ? it.Site.SiteName : "";
                }

                return list.Select(it => it.SubScriber);
            }
        }
        #endregion
        #region GetListECMUnsubscribers
        public IQueryable<UserViewModel> GetListECMUnsubscribers(long? siteId = null, int langId = 1)
        {
            var db = GetModels();
            var list = (from it in db.Users

                        join s in db.SiteLangs.Where(it => it.LangId == langId) on it.SiteId equals s.SiteId into sj
                        from s in sj.DefaultIfEmpty()

                        join ct in db.CustomerTypeLangs.Where(it => it.LangId == langId) on it.CustomerTypeId equals ct.CustomerTypeId into ctj
                        from ct in ctj.DefaultIfEmpty()

                        join sct in db.CustomerTypeLangs.Where(it => it.LangId == langId) on it.SubCustomerTypeId equals sct.CustomerTypeId into sctj
                        from sct in sctj.DefaultIfEmpty()

                        where it.UserId > 0 && it.Active == true && it.UserTypeId == (int)UserType.Type.Customer && (!it.IsSubscriber.HasValue || it.IsSubscriber == false) && (!it.IsReceiveEmailInfo.HasValue || it.IsReceiveEmailInfo == false)

                        select new UserViewModel
                        {
                            UserId = it.UserId,
                            UserTypeId = it.UserTypeId,
                            Email = it.Email,
                            FirstName = it.FirstName,
                            LastName = it.LastName,
                            SiteName = s.SiteName,
                            CustomerTypeName = ct.CustomerTypeName,
                            SubCustomerTypeName = sct.CustomerTypeName,
                            SiteId = it.SiteId,
                            CustomerTypeId = it.CustomerTypeId.HasValue ? it.CustomerTypeId.Value : 0,
                            SubCustomerTypeId = it.SubCustomerTypeId,
                            InsertDate = it.InsertDate
                        });

            if (siteId.HasValue)
            {
                list = list.Where(it => it.SiteId == siteId.Value);
            }

            return list;
        }
        #endregion
        #region GetAllECMUsers
        public List<User> GetAllECMUsers(jQueryDataTableParamModel param, long? siteId = null, int? langId = 1)
        {
            using (var db = GetModels())
            {
                var list = (from it in db.Users

                            join s in db.SiteLangs.Where(it => it.LangId == langId) on it.SiteId equals s.SiteId into sj
                            from s in sj.DefaultIfEmpty()

                            where it.Active == true

                            select new
                            {
                                User = it,
                                Site = s
                            });

                if (siteId.HasValue)
                {
                    int[] userTypes = new int[] { UserType.Type.Reseller, UserType.Type.SiteManager, UserType.Type.Staff };
                    list = list.Where(it => it.User.SiteId == siteId.Value && userTypes.Contains(it.User.UserTypeId));
                }
                else
                {
                    int[] userTypes = new int[] { UserType.Type.SuperAdmin, UserType.Type.Admin, UserType.Type.Reseller, UserType.Type.SiteManager, UserType.Type.Staff };
                }

                foreach (var it in list)
                {
                    it.User.SiteName = it.Site != null ? it.Site.SiteName : "";
                }

                return list.Select(it => it.User).ToList();
            }
        }
        #endregion

        #region GetListECMUsers
        public IQueryable<UserViewModel> GetListECMUsers(long? siteId = null, int? langId = 1)
        {
            var db = GetModels();
            var list = (from it in db.Users

                        join s in db.SiteLangs.Where(it => it.LangId == langId) on it.SiteId equals s.SiteId into sj
                        from s in sj.DefaultIfEmpty()

                        where it.UserId > 0 && it.Active == true

                        select new UserViewModel
                        {
                            UserId = it.UserId,
                            UserTypeId = it.UserTypeId,
                            Email = it.Email,
                            FirstName = it.FirstName,
                            LastName = it.LastName,
                            SiteName = s.SiteName,
                            SiteId = it.SiteId,
                            CustomerTypeId = it.CustomerTypeId.HasValue ? it.CustomerTypeId.Value : 0,
                            SubCustomerTypeId = it.SubCustomerTypeId
                        });

            if (siteId.HasValue)
            {
                int[] userTypes = new int[] { UserType.Type.Reseller, UserType.Type.SiteManager, UserType.Type.Staff };
                list = list.Where(it => it.SiteId == siteId.Value && userTypes.Contains(it.UserTypeId));
            }
            else
            {
                int[] userTypes = new int[] { UserType.Type.SuperAdmin, UserType.Type.Admin, UserType.Type.Reseller, UserType.Type.SiteManager, UserType.Type.Staff };
                list = list.Where(it => userTypes.Contains(it.UserTypeId));
            }

            return list;
        }
        #endregion

        #region GetUser
        public User GetUser(long id)
        {
            User obj = null;

            using (var db = GetModels())
            {
                db.Users.Where(it => it.UserId == id).ToList().ForEach(it =>
                {
                    it.PasswordEncrypted = it.Password;
                    it.Password = DataProtection.Decrypt(it.PasswordEncrypted);
                    obj = it;
                });
            }

            return obj;
        }
        #endregion

        #region GetAlbatrosPlayerUidByUserId
        public string GetAlbatrosPlayerUidByUserId(long userId)
        {
            return DataManager.ToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT [AlbatrosPlayerUid] FROM [Users] WHERE [UserId] = " + userId));
        }
        #endregion

        #region AddUser
        public int AddUser(User obj)
        {
            string sql = @"INSERT INTO [Users]([UserTypeId]
           ,[CustomerTypeId]
           ,[SubCustomerTypeId]
           ,[TitleId]
           ,[FirstName]
           ,[MiddleName]
           ,[LastName]
           ,[Birthdate]
           ,[Gender]
           ,[Email]
           ,[Password]
           ,[Address]
           ,[Street]
           ,[City]
           ,[PostalCode]
           ,[CityId]
           ,[CountryId]
           ,[Phone]
           ,[PhoneCountryCode]
           ,[MobilePhone]
           ,[MobilePhoneCountryCode]
           ,[Remarks]
           ,[IsSubscriber]
           ,[IsReceiveEmailInfo]
           ,[ShippingAddress]
           ,[ShippingStreet]
           ,[ShippingCity]
           ,[ShippingPostalCode]
           ,[ShippingCityId]
           ,[ShippingCountryId]
           ,[ShippingPhone]
           ,[ShippingPhoneCountryCode]
           ,[InsertDate]
           ,[UpdateDate]
           ,[Active]
           ,[ModifyUserId]
           ,[RegisteredDate]
           ,[ExpiredDate]
           ,[FBAccount]
           ,[LastLoggedOn]
           ,[SiteId]
           ,[CompanyName]
           ,[Fax]
           ,[Career]
           ,[LicenseNumber]
           ,[Index]
           ,[CustomField1]
           ,[CustomField2]
           ,[CustomField3]) VALUES(@UserTypeId
           ,@CustomerTypeId
           ,@SubCustomerTypeId
           ,@TitleId
           ,@FirstName
           ,@MiddleName
           ,@LastName
           ,@Birthdate
           ,@Gender
           ,@Email
           ,@Password
           ,@Address
           ,@Street
           ,@City
           ,@PostalCode
           ,@CityId
           ,@CountryId
           ,@Phone
           ,@PhoneCountryCode
           ,@MobilePhone
           ,@MobilePhoneCountryCode
           ,@Remarks
           ,@IsSubscriber
           ,@IsReceiveEmailInfo
           ,@ShippingAddress
           ,@ShippingStreet
           ,@ShippingCity
           ,@ShippingPostalCode
           ,@ShippingCityId
           ,@ShippingCountryId
           ,@ShippingPhone
           ,@ShippingPhoneCountryCode
           ,@InsertDate
           ,@UpdateDate
           ,@Active
           ,@ModifyUserId
           ,@RegisteredDate
           ,@ExpiredDate
           ,@FBAccount
           ,@LastLoggedOn
           ,@SiteId
           ,@CompanyName
           ,@Fax
           ,@Career
           ,@LicenseNumber
           ,@Index
           ,@CustomField1
           ,@CustomField2
           ,@CustomField3) SELECT @@IDENTITY";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@UserTypeId", obj.UserTypeId),
                new SqlParameter("@CustomerTypeId", obj.CustomerTypeId),
                new SqlParameter("@SubCustomerTypeId", obj.SubCustomerTypeId),
                new SqlParameter("@TitleId", obj.TitleId),
                new SqlParameter("@Firstname", obj.FirstName),
                new SqlParameter("@Middlename", obj.MiddleName),
                new SqlParameter("@Lastname", obj.LastName),
                new SqlParameter("@Email", obj.Email),
                new SqlParameter("@Password", obj.PasswordEncrypted),
                new SqlParameter("@Birthdate", obj.Birthdate),
                new SqlParameter("@Gender", obj.Gender),
                new SqlParameter("@Address", obj.Address),
                new SqlParameter("@Street", obj.Street),
                new SqlParameter("@City", obj.City),
                new SqlParameter("@PostalCode", obj.PostalCode),
                new SqlParameter("@CityId", obj.CityId),
                new SqlParameter("@CountryId", obj.CountryId),
                new SqlParameter("@Phone", obj.Phone),
                new SqlParameter("@PhoneCountryCode", obj.PhoneCountryCode),
                new SqlParameter("@MobilePhone", obj.MobilePhone),
                new SqlParameter("@MobilePhoneCountryCode", obj.MobilePhoneCountryCode),
                new SqlParameter("@Remarks", obj.Remarks),
                new SqlParameter("@IsSubscriber", obj.IsSubscriber),
                new SqlParameter("@IsReceiveEmailInfo", obj.IsReceiveEmailInfo),
                new SqlParameter("@ShippingAddress", obj.ShippingAddress),
                new SqlParameter("@ShippingStreet", obj.ShippingStreet),
                new SqlParameter("@ShippingCity", obj.ShippingCity),
                new SqlParameter("@ShippingPostalCode", obj.ShippingPostalCode),
                new SqlParameter("@ShippingCityId", obj.ShippingCityId),
                new SqlParameter("@ShippingCountryId", obj.ShippingCountryId),
                new SqlParameter("@ShippingPhone", obj.ShippingPhone),
                new SqlParameter("@ShippingPhoneCountryCode", obj.ShippingPhoneCountryCode),
                new SqlParameter("@Active", obj.Active),
                new SqlParameter("@InsertDate", obj.InsertDate),
                new SqlParameter("@UpdateDate", obj.UpdateDate),
                new SqlParameter("@RegisteredDate", obj.RegisteredDate),
                new SqlParameter("@ExpiredDate", obj.ExpiredDate),
                new SqlParameter("@LastLoggedOn", obj.LastLoggedOn),
                new SqlParameter("@ModifyUserId", obj.ModifyUserId),
                new SqlParameter("@SiteId", obj.SiteId),
                new SqlParameter("@FBAccount", obj.FBAccount),
                new SqlParameter("@CompanyName", obj.CompanyName),
                new SqlParameter("@Fax", obj.Fax),
                new SqlParameter("@Career", obj.Career),
                new SqlParameter("@LicenseNumber", obj.LicenseNumber),
                new SqlParameter("@Index", obj.Index),
                new SqlParameter("@CustomField1", obj.CustomField1),
                new SqlParameter("@CustomField2", obj.CustomField2),
                new SqlParameter("@CustomField3", obj.CustomField3)), -1);
        }
        #endregion

        #region UpdateUser
        public int UpdateUser(User obj)
        {
            string sql = @"UPDATE [Users]
                           SET [UserTypeId] = @UserTypeId
                              ,[CustomerTypeId] = @CustomerTypeId
                              ,[SubCustomerTypeId] = @SubCustomerTypeId
                              ,[TitleId] = @TitleId
                              ,[FirstName] = @FirstName
                              ,[MiddleName] = @MiddleName
                              ,[LastName] = @LastName
                              ,[Birthdate] = @Birthdate
                              ,[Gender] = @Gender
                              ,[Email] = @Email        
                              ,[Password] = @Password                     
                              ,[Address] = @Address
                              ,[Street] = @Street
                              ,[City] = @City
                              ,[CityId] = @CityId
                              ,[PostalCode] = @PostalCode
                              ,[CountryId] = @CountryId
                              ,[Phone] = @Phone
                              ,[PhoneCountryCode] = @PhoneCountryCode
                              ,[MobilePhone] = @MobilePhone
                              ,[MobilePhoneCountryCode] = @MobilePhoneCountryCode
                              ,[Remarks] = @Remarks
                              ,[IsSubscriber] = @IsSubscriber
                              ,[IsReceiveEmailInfo] = @IsReceiveEmailInfo
                              ,[ShippingAddress] = @ShippingAddress
                              ,[ShippingStreet] = @ShippingStreet
                              ,[ShippingCity] = @ShippingCity
                              ,[ShippingCityId] = @ShippingCityId
                              ,[ShippingPostalCode] = @ShippingPostalCode
                              ,[ShippingCountryId] = @ShippingCountryId
                              ,[ShippingPhone] = @ShippingPhone
                              ,[InsertDate] = @InsertDate
                              ,[UpdateDate] = @UpdateDate
                              ,[Active] = @Active
                              ,[ModifyUserId] = @ModifyUserId
                              ,[RegisteredDate] = @RegisteredDate
                              ,[ExpiredDate] = @ExpiredDate
                              ,[FBAccount] = @FBAccount
                              ,[LastLoggedOn] = @LastLoggedOn
                              ,[SiteId] = @SiteId
                              ,[CompanyName] = @CompanyName
                              ,[Fax] = @Fax
                              ,[Career] = @Career
                              ,[LicenseNumber] = @LicenseNumber
                              ,[Index] = @Index
                              ,[CustomField1] = @CustomField1
                              ,[CustomField2] = @CustomField2
                              ,[CustomField3] = @CustomField3
                            WHERE [UserId] = @UserId";
            SqlConnection.ClearAllPools();
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@UserId", obj.UserId),
                new SqlParameter("@UserTypeId", obj.UserTypeId),
                new SqlParameter("@CustomerTypeId", obj.CustomerTypeId),
                new SqlParameter("@SubCustomerTypeId", obj.SubCustomerTypeId),
                new SqlParameter("@TitleId", obj.TitleId),
                new SqlParameter("@Firstname", obj.FirstName),
                new SqlParameter("@Middlename", obj.MiddleName),
                new SqlParameter("@Lastname", obj.LastName),
                new SqlParameter("@Email", obj.Email),
                new SqlParameter("@Password", obj.PasswordEncrypted),
                new SqlParameter("@Birthdate", obj.Birthdate),
                new SqlParameter("@Gender", obj.Gender),
                new SqlParameter("@Address", obj.Address),
                new SqlParameter("@Street", obj.Street),
                new SqlParameter("@City", obj.City),
                new SqlParameter("@CityId", obj.CityId),
                new SqlParameter("@PostalCode", obj.PostalCode),
                new SqlParameter("@CountryId", obj.CountryId),
                new SqlParameter("@Phone", obj.Phone),
                new SqlParameter("@PhoneCountryCode", obj.PhoneCountryCode),
                new SqlParameter("@MobilePhone", obj.MobilePhone),
                new SqlParameter("@MobilePhoneCountryCode", obj.MobilePhoneCountryCode),
                new SqlParameter("@Remarks", obj.Remarks),
                new SqlParameter("@IsSubscriber", obj.IsSubscriber),
                new SqlParameter("@IsReceiveEmailInfo", obj.IsReceiveEmailInfo),
                new SqlParameter("@ShippingAddress", obj.ShippingAddress),
                new SqlParameter("@ShippingStreet", obj.ShippingStreet),
                new SqlParameter("@ShippingCity", obj.ShippingCity),
                new SqlParameter("@ShippingPostalCode", obj.ShippingPostalCode),
                new SqlParameter("@ShippingCityId", obj.ShippingCityId),
                new SqlParameter("@ShippingCountryId", obj.ShippingCountryId),
                new SqlParameter("@ShippingPhone", obj.ShippingPhone),
                new SqlParameter("@ShippingPhoneCountryCode", obj.ShippingPhoneCountryCode),
                new SqlParameter("@Active", obj.Active),
                new SqlParameter("@InsertDate", obj.InsertDate),
                new SqlParameter("@UpdateDate", obj.UpdateDate),
                new SqlParameter("@RegisteredDate", obj.RegisteredDate),
                new SqlParameter("@ExpiredDate", obj.ExpiredDate),
                new SqlParameter("@LastLoggedOn", obj.LastLoggedOn),
                new SqlParameter("@ModifyUserId", obj.ModifyUserId),
                new SqlParameter("@SiteId", obj.SiteId),
                new SqlParameter("@CompanyName", obj.CompanyName),
                new SqlParameter("@Fax", obj.Fax),
                new SqlParameter("@Career", obj.Career),
                new SqlParameter("@LicenseNumber", obj.LicenseNumber),
                new SqlParameter("@Index", obj.Index),
                new SqlParameter("@FBAccount", obj.FBAccount),
                new SqlParameter("@CustomField1", obj.CustomField1),
                new SqlParameter("@CustomField2", obj.CustomField2),
                new SqlParameter("@CustomField3", obj.CustomField3));
        }
        #endregion

        #region DeleteUser
        public int DeleteUser(IEnumerable<long> ids)
        {
            using (var db = GetModels())
            {
                var user = db.Users.Where(it => ids.Contains(it.UserId) && it.UserTypeId != (int)UserType.Type.SuperAdmin);
                if (user.Any())
                {
                    user.First().Active = false;
                }
                return db.SaveChanges();
            }
        }
        public int DeleteUser(long id)
        {
            using (var db = GetModels())
            {
                var user = db.Users.Where(it => it.UserId == id && it.UserTypeId != (int)UserType.Type.SuperAdmin);
                if (user.Any())
                {
                    user.First().Active = false;
                }
                return db.SaveChanges();
            }
        }
        #endregion

        #region UserAuthentication
        public User UserAuthentication(string email, string password, int[] userTypes)
        {
            User user = null;

            int[] allowUserTypes = new int[] { 0, 1, 3, 4, 5 };

            using (var db = GetModels())
            {
                var query = db.Users.Where(it => it.Email == email && it.Password == password && allowUserTypes.Contains(it.UserTypeId));

                if (userTypes != null && userTypes.Any())
                {
                    query = query.Where(it => userTypes.Contains(it.UserTypeId));
                }

                query.OrderBy(it => it.UserId).ToList().ForEach(it => user = it);
            }

            return user;
        }
        #endregion

        #region IsExistsEmail
        public bool IsExistsEmail(string email, long? skipId = null)
        {
            string sql = "SELECT COUNT(Email) FROM [Users] WHERE [Users].[Active] = 1 AND [Users].[Email]  = @Email";
            if (skipId.HasValue)
            {
                sql += " AND [UserId] <> " + skipId.Value;
            }
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@Email", email))) > 0;
        }
        #endregion

        #region IsExistsUsers
        public bool IsExistsUsers(string email, string firstName, string lastName)
        {
            string sql = @"SELECT COUNT(UserId) FROM Users WHERE Email = @Email AND FirstName = @FirstName AND LastName = @LastName AND Active = 1";
            return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@Email", email),
                new SqlParameter("@FirstName", firstName),
                new SqlParameter("@LastName", lastName))) > 0;
        }
        public bool IsExistsUsers(string email, string firstName, string lastName, long siteId)
        {
            string sql = @"SELECT COUNT(UserId) FROM Users WHERE Email = @Email AND FirstName = @FirstName AND LastName = @LastName AND SiteId = @SiteId AND Active = 1";
            return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@Email", email),
                new SqlParameter("@FirstName", firstName),
                new SqlParameter("@LastName", lastName),
                new SqlParameter("@SiteId", siteId))) > 0;
        }
        #endregion

        #region IsExistsUsersInGroup
        public bool IsExistsUsersInGroup(string email, string firstName, string lastName, long skipCustomerGroupId = 0)
        {
            string sql = @"SELECT COUNT(it.CustomerId) AS NumOfCustomers FROM [CustomerGroupCustomer] it
                            JOIN [Users] u ON u.UserId = it.CustomerId AND u.Active = 1
                            WHERE u.[Email] = @Email AND u.[FirstName] = @FirstName AND u.[LastName] = @LastName AND it.[CustomerGroupId] = @CustomerGroupId";
            return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@Email", email),
                new SqlParameter("@FirstName", firstName),
                new SqlParameter("@LastName", lastName),
                new SqlParameter("@CustomerGroupId", skipCustomerGroupId))) > 0;
        }
        #endregion

        #region IsExistsFacebookId
        public bool IsExistsFacebookId(string facebookId)
        {
            string sql = "SELECT COUNT(*) FROM [Users] WHERE FacebookId = @FacebookId";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@FacebookId", facebookId))) > 0;
        }
        #endregion

        #region GetUserByFacebookId
        public User GetUserByFacebookId(string facebookId)
        {
            User obj = null;

            using (var db = GetModels())
            {
                db.Users.Where(it => it.FBAccount == facebookId).ToList().ForEach(it => obj = it);
            }

            return obj;
        }
        #endregion

        #region CheckPassword
        public bool CheckPassword(string Password)
        {
            string sql = "SELECT 1 FROM [Users] WHERE [Password] = @Password";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@Password", Password));

            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }

            return false;

        }
        #endregion

        #region UpdatePassword
        public int UpdatePassword(string Password, int Userid)
        {
            string sql = @"UPDATE [Users]
                           SET [Password] = @Password
                            WHERE [UserId] = @UserId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@UserId", Userid),
                new SqlParameter("@Password", Password)
               );
        }
        #endregion

        #region CheckAccountActivation
        public bool CheckAccountActivation(string email, string code)
        {
            string sql = "SELECT COUNT([UserId]) FROM [Users] WHERE [Email] = @Email AND [ActivationCode] = @ActivationCode";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@Email", email),
                new SqlParameter("@ActivationCode", code))) > 0;
        }
        #endregion

        #region AddSubscriber
        public void AddSubscriber(string email)
        {
            string sql = @"IF NOT EXISTS(SELECT [UserId] FROM [Users] WHERE [Email] = @Email)
                           BEGIN
                                INSERT INTO [Users]([Email], [UserTypeId]) VALUES(@Email, @UserTypeId) SELECT @@IDENTITY;
                           END";
            SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@Email", email),
                new SqlParameter("@UserTypeId", UserType.Type.Customer));
        }
        #endregion
        #endregion

        #region UserType
        #region GetAllUserTypes
        public IQueryable<UserType> GetAllUserTypes(int langId = 1)
        {
            using (var db = GetModels())
            {
                return db.UserTypes.OrderBy(it => it.UserTypeName);
            }
        }
        #endregion

        #region GetUserType
        public UserType GetUserType(long id)
        {
            UserType obj = null;

            using (var db = GetModels())
            {
                db.UserTypes.Where(it => it.UserTypeId == id).ToList().ForEach(it => obj = it);
            }

            return obj;
        }
        #endregion

        #region AddUserType
        public int AddUserType(UserType obj)
        {
            string sql = "INSERT INTO [UserTypes](UserTypeId, UserTypeName) VALUES(@UserTypeId, @UserTypeName) SELECT @UserTypeId";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@UserTypeId", obj.UserTypeId),
               new SqlParameter("@UserTypeName", obj.UserTypeName))) > 0 ? obj.UserTypeId : -1;
        }
        #endregion

        #region UpdateUserType
        public int UpdateUserType(UserType obj)
        {
            string sql = "UPDATE [UserTypes] SET UserTypeName = @UserTypeName WHERE UserTypeId = @UserTypeId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@UserTypeId", obj.UserTypeId),
               new SqlParameter("@UserTypeName", obj.UserTypeName));
        }
        #endregion

        #region DeleteUserType
        public int DeleteUserType(int id)
        {
            string sql = "DELETE FROM [UserTypes] WHERE UserTypeId = @UserTypeId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@UserTypeId", id));
        }
        #endregion
        #endregion

        #region Course
        #region GetAllCourses
        public List<Course> GetAllCourses()
        {
            using (var db = GetModels())
            {
                return db.Courses.Where(it => it.Active == true).ToList();
            }
        }
        public List<Course> GetAllCoursesBySiteId(long siteId)
        {
            using (var db = GetModels())
            {
                return (from it in db.Courses
                        join s in db.Sites on it.SiteId equals s.SiteId
                        where it.SiteId == siteId
                        select it).ToList();
            }
        }
        public List<Course> GetAllCourses(int langId = 1)
        {
            using (var db = GetModels())
            {
                return (from it in db.Courses
                        where it.Active == true
                        select it).ToList();
            }
        }
        #endregion

        #region GetCourse
        public Course GetCourse(long id)
        {
            Course obj = null;

            using (var db = GetModels())
            {
                db.Courses.Where(it => it.CourseId == id).ToList().ForEach(it => obj = it);
            }

            return obj;
        }
        #endregion

        #region AddCourse
        public int AddCourse(Course obj)
        {
            string sql = "INSERT INTO [Course](CourseTypeId, SiteId, CourseName, CourseDesc, Holes, DefaultPrice, Duration, StartTime, EndTime, InsertDate, UpdateDate, Active, UserId) VALUES(@CourseTypeId, @SiteId, @CourseName, @CourseDesc, @Holes, @DefaultPrice, @Duration, @StartTime, @EndTime, @InsertDate, @UpdateDate, @Active, @UserId) SELECT @@IDENTITY";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@CourseTypeId", obj.CourseTypeId),
               new SqlParameter("@SiteId", obj.SiteId),
               new SqlParameter("@CourseName", obj.CourseName),
               new SqlParameter("@CourseDesc", obj.CourseDesc),
               new SqlParameter("@Holes", obj.Holes),
               new SqlParameter("@DefaultPrice", obj.DefaultPrice),
               new SqlParameter("@Duration", obj.Duration),
               new SqlParameter("@StartTime", obj.StartTime),
               new SqlParameter("@EndTime", obj.EndTime),
               new SqlParameter("@InsertDate", obj.InsertDate),
               new SqlParameter("@UpdateDate", obj.UpdateDate),
               new SqlParameter("@Active", obj.Active),
               new SqlParameter("@UserId", obj.UserId)));
        }
        #endregion

        #region UpdateCourse
        public int UpdateCourse(Course obj)
        {
            string sql = "UPDATE [Course] SET SiteId = @SiteId, CourseTypeId = @CourseTypeId, CourseName = @CourseName, CourseDesc = @CourseDesc, Holes = @Holes, DefaultPrice = @DefaultPrice, Duration = @Duration, StartTime = @StartTime, EndTime = @EndTime, UpdateDate = @UpdateDate, Active = @Active, [UserId] = @UserId WHERE CourseId = @CourseId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@CourseId", obj.CourseId),
               new SqlParameter("@CourseTypeId", obj.CourseTypeId),
               new SqlParameter("@SiteId", obj.SiteId),
               new SqlParameter("@CourseName", obj.CourseName),
               new SqlParameter("@CourseDesc", obj.CourseDesc),
               new SqlParameter("@Holes", obj.Holes),
               new SqlParameter("@DefaultPrice", obj.DefaultPrice),
               new SqlParameter("@Duration", obj.Duration),
               new SqlParameter("@StartTime", obj.StartTime),
               new SqlParameter("@EndTime", obj.EndTime),
               new SqlParameter("@UpdateDate", obj.UpdateDate),
               new SqlParameter("@Active", obj.Active),
               new SqlParameter("@UserId", obj.UserId));
        }
        #endregion

        #region DeleteCourse
        public int DeleteCourse(int id)
        {
            string sql = "UPDATE [Course] SET Active = 0 WHERE CourseId = @CourseId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CourseId", id));
        }
        #endregion

        #region GetCoursesListBySiteId
        public List<Course> GetCoursesListBySiteId(long siteId)
        {
            string sql = "SELECT [CourseId], [CourseName] FROM [Course] WHERE [SiteId] = @SiteId ORDER BY [CourseName]";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", siteId));

            List<Course> list = new List<Course>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new Course()
                    {
                        CourseId = DataManager.ToInt(row["CourseId"]),
                        CourseName = DataManager.ToString(row["CourseName"])
                    });
                }
            }
            return list;
        }
        #endregion

        #region GetDefaultPriceByCourseId
        public decimal GetDefaultPriceByCourseId(long courseId)
        {
            string sql = "SELECT [DefaultPrice] FROM [Course] WHERE [CourseID] = " + courseId;
            return DataManager.ToDecimal(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql), 0);
        }
        #endregion
        #endregion

        #region CourseType
        #region GetAllCourseTypes
        public List<CourseType> GetAllCourseTypes()
        {
            using (var db = new DLGEntities(ConnectionString))
            {
                return (from it in db.CourseTypes
                        orderby it.CourseTypeName
                        select it).ToList();
            }
        }
        #endregion

        #region GetCourseType
        public CourseType GetCourseType(long id)
        {
            CourseType obj = null;

            using (var db = new DLGEntities(ConnectionString))
            {
                (from it in db.CourseTypes
                 where it.CourseTypeId == id
                 select it).ToList().ForEach(it => obj = it);
            }

            return obj;
        }
        #endregion

        #region AddCourseType
        public int AddCourseType(CourseType obj)
        {
            string sql = "INSERT INTO [CourseType](CourseTypeName) VALUES(@CourseTypeName) SELECT @@IDENTITY";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@CourseTypeName", obj.CourseTypeName)));
        }
        #endregion

        #region UpdateCourseType
        public int UpdateCourseType(CourseType obj)
        {
            string sql = "UPDATE [CourseType] SET [CourseTypeName] = @CourseTypeName WHERE CourseTypeId = @CourseTypeId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@CourseTypeId", obj.CourseTypeId),
               new SqlParameter("@CourseTypeName", obj.CourseTypeName));
        }
        #endregion

        #region DeleteCourseType
        public int DeleteCourseType(int id)
        {
            string sql = "DELETE FROM [CourseType] WHERE [CourseTypeId] = @CourseTypeId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CourseTypeId", id));
        }
        #endregion
        #endregion

        #region Site
        #region GetAllSites
        public IQueryable<Site> GetAllSites(int langId = 1)
        {
            var db = GetModels();
            var list = from it in db.Sites
                       join sl in db.SiteLangs.Where(s => s.LangId == langId) on it.SiteId equals sl.SiteId into slj
                       from sl in slj.DefaultIfEmpty()
                       orderby sl.SiteName
                       where it.Active == true
                       select new
                       {
                           Site = it,
                           SiteLang = sl
                       };

            foreach (var it in list)
            {
                if (it.SiteLang != null)
                {
                    it.Site.SiteName = it.SiteLang.SiteName;
                }
            }

            return list.Select(it => it.Site);
        }
        public IQueryable<SiteViewModel> GetListSites(int langId = 1)
        {
            var db = GetModels();
            return from it in db.Sites
                   join sl in db.SiteLangs.Where(s => s.LangId == langId) on it.SiteId equals sl.SiteId into slj
                   from sl in slj.DefaultIfEmpty()
                   orderby sl.SiteName
                   where it.SiteId > 0 && it.Active == true
                   select new SiteViewModel
                   {
                       SiteId = it.SiteId,
                       SiteName = sl.SiteName
                   };
        }
        public IQueryable<Site> GetAllDropDownSites(int langId = 1)
        {
            using (var db = GetModels())
            {
                var list = from it in db.Sites
                           join sl in db.SiteLangs.Where(s => s.LangId == langId) on it.SiteId equals sl.SiteId into slj
                           from sl in slj.DefaultIfEmpty()
                           orderby sl.SiteName
                           where it.Active == true && it.Visible == true
                           select new
                           {
                               SiteId = it.SiteId,
                               SiteLang = sl
                           };

                return list.Select(it => new Site() { SiteId = it.SiteId, SiteName = it.SiteLang != null ? it.SiteLang.SiteName : "" });
            }
        }
        public IQueryable<Site> GetAvailableDropDownSites(int langId = 1)
        {
            using (var db = GetModels())
            {
                var list = from it in db.Sites

                           join i in db.Items on it.SiteId equals i.SiteId

                           join sl in db.SiteLangs.Where(s => s.LangId == langId) on it.SiteId equals sl.SiteId into slj
                           from sl in slj.DefaultIfEmpty()

                           orderby sl.SiteName
                           where it.Active == true && it.Visible == true && i.Active == true && i.IsPublish == true

                           select new
                           {
                               Site = it,
                               SiteLang = sl
                           };

                foreach (var it in list)
                {
                    if (it.SiteLang != null)
                    {
                        it.Site.LangId = it.SiteLang.LangId;
                        it.Site.SiteName = it.SiteLang.SiteName;
                    }
                }

                return list.Select(it => it.Site);
            }
        }
        public Dictionary<int, int> GetAvailableDropDownSitesByItemType()
        {
            Dictionary<int, int> list = new Dictionary<int, int>();
            string sql = @"SELECT DISTINCT [Site].[SiteId], [Item].[ItemTypeId] FROM [Site]
                            INNER JOIN [Item] ON [Item].[SiteId] = [Site].[SiteId]
                            LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId]
                            WHERE [Site].[Active] = 1 AND [Site].[Visible] = 1
                            AND [Item].[Active] = 1 AND [Item].[IsPublish] = 1
                            ORDER BY [SiteLang].[SiteName]";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(DataManager.ToInt(row["SiteId"]), DataManager.ToInt(row["ItemTypeId"]));
            }
            return list;
        }

        public IQueryable<Site> GetAllSitesByCountryId(int countryId, int langId = 1)
        {
            using (var db = GetModels())
            {
                var list = from it in db.Sites
                           join sl in db.SiteLangs.Where(s => s.LangId == langId) on it.SiteId equals sl.SiteId into slj
                           from sl in slj.DefaultIfEmpty()
                           orderby sl.SiteName
                           where it.Active == true && it.Visible == true && it.CountryId == countryId
                           select new
                           {
                               Site = it,
                               SiteLang = sl
                           };

                foreach (var it in list)
                {
                    if (it.SiteLang != null)
                    {
                        it.Site.SiteName = it.SiteLang.SiteName;
                    }
                }

                return list.Select(it => it.Site);
            }
        }

        public IQueryable<Site> GetAllSitesByStateId(int stateId, int langId = 1)
        {
            using (var db = GetModels())
            {
                var list = from it in db.Sites
                           join sl in db.SiteLangs.Where(s => s.LangId == langId) on it.SiteId equals sl.SiteId into slj
                           from sl in slj.DefaultIfEmpty()
                           orderby sl.SiteName
                           where it.Active == true && it.Visible == true && it.StateId == stateId
                           select new
                           {
                               Site = it,
                               SiteLang = sl
                           };

                foreach (var it in list)
                {
                    if (it.SiteLang != null)
                    {
                        it.Site.SiteName = it.SiteLang.SiteName;
                    }
                }

                return list.Select(it => it.Site);
            }
        }
        #endregion

        #region GetSite
        public Site GetSite(long id, int langId = 1)
        {
            Site obj = null;

            using (var db = GetModels())
            {
                db.Sites.Where(it => it.SiteId == id).ToList().ForEach(it => obj = it);
            }

            return obj;
        }
        #endregion

        #region GetSiteChronogolfClubId
        public int GetSiteChronogolfClubId(long id)
        {
            int? clubId = 0;
            using (var db = GetModels())
            {
                clubId = db.Sites.Where(it => it.SiteId == id).Select(it => it.ChronogolfClubId).SingleOrDefault();
            }

            return clubId.HasValue ? clubId.Value : 0;
        }
        #endregion

        #region GetSiteSMTPSettings
        public bool GetSiteSMTPSettings(long siteId, out string server, out string username, out string password, out int port, out bool enableSsl, out bool useVERP, out string bouncedReturnEmail)
        {
            string sql = "SELECT SMTPServer, SMTPUsername, SMTPPassword, SMTPPort, SMTPUseSSL, SMTPUseVERP, BouncedReturnEmail FROM Site WHERE ISNULL(IsUseGlobalSMTPSettings, 0) = 0 AND SiteId = " + siteId;
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                server = DataManager.ToString(ds.Tables[0].Rows[0]["SMTPServer"]);
                username = DataManager.ToString(ds.Tables[0].Rows[0]["SMTPUsername"]);
                password = DataManager.ToString(ds.Tables[0].Rows[0]["SMTPPassword"]);
                port = DataManager.ToInt(ds.Tables[0].Rows[0]["SMTPPort"], 25);
                enableSsl = DataManager.ToBoolean(ds.Tables[0].Rows[0]["SMTPUseSSL"]);
                useVERP = DataManager.ToBoolean(ds.Tables[0].Rows[0]["SMTPUseVERP"]);
                bouncedReturnEmail = DataManager.ToString(ds.Tables[0].Rows[0]["BouncedReturnEmail"]);

                return !String.IsNullOrWhiteSpace(server) && !String.IsNullOrWhiteSpace(username);
            }
            else
            {
                var options = GetOptions("SMTPServer", "SMTPUsername", "SMTPPassword", "SMTPPort", "SMTPUseSSL", "SMTPUseVERP", "BouncedReturnEmail");
                if (options != null || options.Count == 5)
                {
                    server = options["SMTPServer"];
                    username = options["SMTPUsername"];
                    password = options["SMTPPassword"];
                    port = DataManager.ToInt(options["SMTPPort"]);
                    enableSsl = DataManager.ToBoolean(options["SMTPUseSSL"]);
                    useVERP = DataManager.ToBoolean(options["SMTPUseVERP"]);
                    bouncedReturnEmail = options["BouncedReturnEmail"];

                    return true;
                }
                else
                {
                    server = string.Empty;
                    username = string.Empty;
                    password = string.Empty;
                    port = 25;
                    enableSsl = false;
                    useVERP = false;
                    bouncedReturnEmail = string.Empty;

                    return false;
                }
            }
        }
        #endregion

        public void GetSiteDefaultSender(long siteId, out string defaultSenderName, out string defaultSenderEmail)
        {
            string sql = "SELECT DefaultSenderName, DefaultSenderEmail From [Site] WHERE ISNULL(IsUseGlobalSMTPSettings, 0) = 0 AND [SiteId] = @SiteId";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", siteId));

            if (ds.Tables[0].Rows.Count > 0)
            {
                defaultSenderName = DataManager.ToString(ds.Tables[0].Rows[0]["DefaultSenderName"]);
                defaultSenderEmail = DataManager.ToString(ds.Tables[0].Rows[0]["DefaultSenderEmail"]);
            }
            else
            {
                defaultSenderName = System.Configuration.ConfigurationManager.AppSettings["DefaultSenderName"];
                defaultSenderEmail = System.Configuration.ConfigurationManager.AppSettings["DefaultSenderEmail"];
            }
        }

        public List<long> GetManuallySMTPSiteIds()
        {
            using (var db = GetModels())
            {
                return (from it in db.Sites
                        where it.Active == true && (it.IsUseGlobalSMTPSettings == false && it.SMTPServer != null && it.SMTPServer != "" && it.SMTPUsername != null && it.SMTPUsername != "" && it.SMTPPassword != null && it.SMTPPassword != "")
                        select it.SiteId).ToList();
            }
        }

        #region GetSiteById
        public Site GetSiteById(long id, int langId = 1)
        {
            Site obj = null;

            using (var db = GetModels())
            {
                var list = from it in db.Sites
                           join sl in db.SiteLangs.Where(s => s.LangId == langId) on it.SiteId equals sl.SiteId into slj
                           from sl in slj.DefaultIfEmpty()
                           orderby sl.SiteName
                           where it.Active == true && it.SiteId == id
                           select new
                           {
                               Site = it,
                               SiteLang = sl
                           };

                foreach (var it in list)
                {
                    if (it.SiteLang != null)
                    {
                        it.Site.SiteName = it.SiteLang.SiteName;
                        it.Site.Description = it.SiteLang.Description;
                        it.Site.Accommodation = it.SiteLang.Accommodation;
                        it.Site.Restaurant = it.SiteLang.Restaurant;
                    }
                }

                list.ToList().ForEach(it => obj = it.Site);
            }

            return obj;
        }
        #endregion

        #region AddSite
        public int AddSite(Site obj)
        {
            string sql = @"INSERT INTO [Site]([SiteSlug], [ReservationAPI], [PrimaAPIKey], [PrimaClubKey], [AlbatrosCourseId], [AlbatrosUrl], [AlbatrosUsername], [AlbatrosPassword], [AlbatrosClubId], [ChronogolfClubId], [GolfBrandId], [Address], [Street], [City], [StateId], [RegionId], [PostalCode], [CountryId], [Phone], [Fax], [Website], [Email], [FB], [InsertDate], [UpdateDate], [Visible], [Active], [UserId], [Latitude], [Longitude], [IsUseGlobalSMTPSettings], [SMTPServer], [SMTPUsername], [SMTPPassword], [SMTPPort], [SMTPUseSSL], [SMTPUseVERP], [BouncedReturnEmail], [DefaultSenderName], [DefaultSenderEmail], [IsTrackingOpenMail], [IsTrackingLinkClick], [IsUseGlobalNetmessageSettings], [NetmessageFTPUsername], [NetmessageFTPPassword], [NetmessageAccountName], [IsCommercial], [IsAssociative], [IsRegie], [ManageRestaurantFlag], [ManageProshopFlag], [ManageFieldFlag], [ManageGolfFlag], [ManagerPhone], [RespReceptionPhone], [GreenKeeperPhone], [RespProshopPhone], [RestaurateurPhone], [AssociationPresidentPhone], [ManagerEmail], [RespReceptionEmail], [GreenKeeperEmail], [RespProshopEmail], [RestaurateurEmail], [AssociationPresidentEmail], [CommercialNBSubscriber], [CommercialNBGF], [CommercialTurnover], [CommercialReservationSystem], [HotelOnSite], [HotelPartner], [LydiaVendorToken], [LydiaVendorId])
                VALUES(@SiteSlug, @ReservationAPI, @PrimaAPIKey, @PrimaClubKey, @AlbatrosCourseId, @AlbatrosUrl, @AlbatrosUsername, @AlbatrosPassword, @AlbatrosClubId, @ChronogolfClubId, @GolfBrandId, @Address, @Street, @City, @StateId, @RegionId, @PostalCode, @CountryId, @Phone, @Fax, @Website, @Email, @FB, @InsertDate, @UpdateDate, @Visible, @Active, @UserId, @Latitude, @Longitude, @IsUseGlobalSMTPSettings, @SMTPServer, @SMTPUsername, @SMTPPassword, @SMTPPort, @SMTPUseSSL, @SMTPUseVERP, @BouncedReturnEmail, @DefaultSenderName, @DefaultSenderEmail, @IsTrackingOpenMail, @IsTrackingLinkClick, @IsUseGlobalNetmessageSettings, @NetmessageFTPUsername, @NetmessageFTPPassword, @NetmessageAccountName, @IsCommercial, @IsAssociative, @IsRegie, @ManageRestaurantFlag, @ManageProshopFlag, @ManageFieldFlag, @ManageGolfFlag, @ManagerPhone, @RespReceptionPhone, @GreenKeeperPhone, @RespProshopPhone, @RestaurateurPhone, @AssociationPresidentPhone, @ManagerEmail, @RespReceptionEmail, @GreenKeeperEmail, @RespProshopEmail, @RestaurateurEmail, @AssociationPresidentEmail, @CommercialNBSubscriber, @CommercialNBGF, @CommercialTurnover, @CommercialReservationSystem, @HotelOnSite, @HotelPartner, @LydiaVendorToken, @LydiaVendorId) SELECT @@IDENTITY";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@SiteSlug", obj.SiteSlug),
               new SqlParameter("@ReservationAPI", obj.ReservationAPI),
               new SqlParameter("@PrimaAPIKey", obj.PrimaAPIKey),
               new SqlParameter("@PrimaClubKey", obj.PrimaClubKey),
               new SqlParameter("@AlbatrosCourseId", obj.AlbatrosCourseId),
               new SqlParameter("@AlbatrosUrl", obj.AlbatrosUrl),
               new SqlParameter("@AlbatrosUsername", obj.AlbatrosUsername),
               new SqlParameter("@AlbatrosPassword", obj.AlbatrosPassword),
               new SqlParameter("@AlbatrosClubId", obj.AlbatrosClubId),
               new SqlParameter("@ChronogolfClubId", obj.ChronogolfClubId),
               new SqlParameter("@GolfBrandId", obj.GolfBrandId),
               new SqlParameter("@Address", obj.Address),
               new SqlParameter("@Street", obj.Street),
               new SqlParameter("@City", obj.City),
               new SqlParameter("@StateId", obj.StateId),
               new SqlParameter("@RegionId", obj.RegionId),
               new SqlParameter("@PostalCode", obj.PostalCode),
               new SqlParameter("@CountryId", obj.CountryId),
               new SqlParameter("@Phone", obj.Phone),
               new SqlParameter("@Fax", obj.Fax),
               new SqlParameter("@Website", obj.Website),
               new SqlParameter("@Email", obj.Email),
               new SqlParameter("@FB", obj.FB),
               new SqlParameter("@InsertDate", obj.InsertDate),
               new SqlParameter("@UpdateDate", obj.UpdateDate),
               new SqlParameter("@Visible", obj.Visible),
               new SqlParameter("@Active", obj.Active),
               new SqlParameter("@UserId", obj.UserId),
               new SqlParameter("@Latitude", obj.Latitude),
               new SqlParameter("@Longitude", obj.Longitude),
               new SqlParameter("@IsUseGlobalSMTPSettings", obj.IsUseGlobalSMTPSettings),
               new SqlParameter("@SMTPServer", obj.SMTPServer),
               new SqlParameter("@SMTPUsername", obj.SMTPUsername),
               new SqlParameter("@SMTPPassword", obj.SMTPPassword),
               new SqlParameter("@SMTPPort", obj.SMTPPort),
               new SqlParameter("@SMTPUseSSL", obj.SMTPUseSSL),
               new SqlParameter("@SMTPUseVERP", obj.SMTPUseVERP),
               new SqlParameter("@BouncedReturnEmail", obj.BouncedReturnEmail),
               new SqlParameter("@DefaultSenderName", obj.DefaultSenderName),
               new SqlParameter("@DefaultSenderEmail", obj.DefaultSenderEmail),
               new SqlParameter("@IsTrackingOpenMail", obj.IsTrackingOpenMail),
               new SqlParameter("@IsTrackingLinkClick", obj.IsTrackingLinkClick),
               new SqlParameter("@IsUseGlobalNetmessageSettings", obj.IsUseGlobalNetmessageSettings),
               new SqlParameter("@NetmessageFTPUsername", obj.NetmessageFTPUsername),
               new SqlParameter("@NetmessageFTPPassword", obj.NetmessageFTPPassword),
               new SqlParameter("@NetmessageAccountName", obj.NetmessageAccountName),
               new SqlParameter("@IsCommercial", obj.IsCommercial),
               new SqlParameter("@IsAssociative", obj.IsAssociative),
               new SqlParameter("@IsRegie", obj.IsRegie),
               new SqlParameter("@ManageRestaurantFlag", obj.ManageRestaurantFlag),
               new SqlParameter("@ManageProshopFlag", obj.ManageProshopFlag),
               new SqlParameter("@ManageFieldFlag", obj.ManageFieldFlag),
               new SqlParameter("@ManageGolfFlag", obj.ManageGolfFlag),
               new SqlParameter("@ManagerPhone", obj.ManagerPhone),
               new SqlParameter("@RespReceptionPhone", obj.RespReceptionPhone),
               new SqlParameter("@GreenKeeperPhone", obj.GreenKeeperPhone),
               new SqlParameter("@RespProshopPhone", obj.RespProshopPhone),
               new SqlParameter("@RestaurateurPhone", obj.RestaurateurPhone),
               new SqlParameter("@AssociationPresidentPhone", obj.AssociationPresidentPhone),
               new SqlParameter("@ManagerEmail", obj.ManagerEmail),
               new SqlParameter("@RespReceptionEmail", obj.RespReceptionEmail),
               new SqlParameter("@GreenKeeperEmail", obj.GreenKeeperEmail),
               new SqlParameter("@RespProshopEmail", obj.RespProshopEmail),
               new SqlParameter("@RestaurateurEmail", obj.RestaurateurEmail),
               new SqlParameter("@AssociationPresidentEmail", obj.AssociationPresidentEmail),
               new SqlParameter("@CommercialNBSubscriber", obj.CommercialNBSubscriber),
               new SqlParameter("@CommercialNBGF", obj.CommercialNBGF),
               new SqlParameter("@CommercialTurnover", obj.CommercialTurnover),
               new SqlParameter("@CommercialReservationSystem", obj.CommercialReservationSystem),
               new SqlParameter("@HotelOnSite", obj.HotelOnSite),
               new SqlParameter("@HotelPartner", obj.HotelPartner),
               new SqlParameter("@LydiaVendorToken", obj.LydiaVendorToken),
               new SqlParameter("@LydiaVendorId", obj.LydiaVendorId)));
        }
        #endregion

        #region UpdateSite
        public int UpdateSite(Site obj)
        {
            string sql = "UPDATE [Site] SET [SiteSlug] = @SiteSlug, [ReservationAPI] = @ReservationAPI, [PrimaAPIKey] = @PrimaAPIKey, [PrimaClubKey] = @PrimaClubKey, [AlbatrosCourseId] = @AlbatrosCourseId, [AlbatrosUrl] = @AlbatrosUrl, [AlbatrosUsername] = @AlbatrosUsername, [AlbatrosPassword] = @AlbatrosPassword, [AlbatrosClubId] = @AlbatrosClubId, [ChronogolfClubId] = @ChronogolfClubId, [GolfBrandId] = @GolfBrandId, [Address] = @Address, [Street] = @Street, [City] = @City, [StateId] = @StateId, [RegionId] = @RegionId, [PostalCode] = @PostalCode, [CountryId] = @CountryId, [Phone] = @Phone, [Fax] = @Fax, [Website] = @Website, [Email] = @Email, [FB] = @FB, [UpdateDate] = GETDATE(), [UserId] = @UserId, [Latitude] = @Latitude, [Longitude] = @Longitude, [Visible] = @Visible, [IsUseGlobalSMTPSettings] = @IsUseGlobalSMTPSettings, [SMTPServer] = @SMTPServer, [SMTPUsername] = @SMTPUsername, [SMTPPassword] = @SMTPPassword, [SMTPPort] = @SMTPPort, [SMTPUseSSL] = @SMTPUseSSL, [SMTPUseVERP] = @SMTPUseVERP, [BouncedReturnEmail] = @BouncedReturnEmail, [DefaultSenderName] = @DefaultSenderName, [DefaultSenderEmail] = @DefaultSenderEmail, [IsTrackingOpenMail] = @IsTrackingOpenMail, [IsTrackingLinkClick] = @IsTrackingLinkClick, [IsUseGlobalNetmessageSettings] = @IsUseGlobalNetmessageSettings, [NetmessageFTPUsername] = @NetmessageFTPUsername, [NetmessageFTPPassword] = @NetmessageFTPPassword, [NetmessageAccountName] = @NetmessageAccountName, [IsCommercial] = @IsCommercial, [IsAssociative] = @IsAssociative, [IsRegie] = @IsRegie, [ManageRestaurantFlag] = @ManageRestaurantFlag, [ManageProshopFlag] = @ManageProshopFlag, [ManageFieldFlag] = @ManageFieldFlag, [ManageGolfFlag] = @ManageGolfFlag, [ManagerPhone] = @ManagerPhone, [RespReceptionPhone] = @RespReceptionPhone, [GreenKeeperPhone] = @GreenKeeperPhone, [RespProshopPhone] = @RespProshopPhone, [RestaurateurPhone] = @RestaurateurPhone, [AssociationPresidentPhone] = @AssociationPresidentPhone, [ManagerEmail] = @ManagerEmail, [RespReceptionEmail] = @RespReceptionEmail, [GreenKeeperEmail] = @GreenKeeperEmail, [RespProshopEmail] = @RespProshopEmail, [RestaurateurEmail] = @RestaurateurEmail, [AssociationPresidentEmail] = @AssociationPresidentEmail, [CommercialNBSubscriber] = @CommercialNBSubscriber, [CommercialNBGF] = @CommercialNBGF, [CommercialTurnover] = @CommercialTurnover, [CommercialReservationSystem] = @CommercialReservationSystem, [HotelOnSite] = @HotelOnSite, [HotelPartner] = @HotelPartner, LydiaVendorToken = @LydiaVendorToken, LydiaVendorId = @LydiaVendorId WHERE SiteId = @SiteId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@SiteId", obj.SiteId),
               new SqlParameter("@SiteSlug", obj.SiteSlug),
               new SqlParameter("@ReservationAPI", obj.ReservationAPI),
               new SqlParameter("@PrimaAPIKey", obj.PrimaAPIKey),
               new SqlParameter("@PrimaClubKey", obj.PrimaClubKey),
               new SqlParameter("@AlbatrosCourseId", obj.AlbatrosCourseId),
               new SqlParameter("@AlbatrosUrl", obj.AlbatrosUrl),
               new SqlParameter("@AlbatrosUsername", obj.AlbatrosUsername),
               new SqlParameter("@AlbatrosPassword", obj.AlbatrosPassword),
               new SqlParameter("@AlbatrosClubId", obj.AlbatrosClubId),
               new SqlParameter("@ChronogolfClubId", obj.ChronogolfClubId),
               new SqlParameter("@GolfBrandId", obj.GolfBrandId),
               new SqlParameter("@Address", obj.Address),
               new SqlParameter("@Street", obj.Street),
               new SqlParameter("@City", obj.City),
               new SqlParameter("@StateId", obj.StateId),
               new SqlParameter("@RegionId", obj.RegionId),
               new SqlParameter("@PostalCode", obj.PostalCode),
               new SqlParameter("@CountryId", obj.CountryId),
               new SqlParameter("@Phone", obj.Phone),
               new SqlParameter("@Fax", obj.Fax),
               new SqlParameter("@Website", obj.Website),
               new SqlParameter("@Email", obj.Email),
               new SqlParameter("@FB", obj.FB),
               new SqlParameter("@UserId", obj.UserId),
               new SqlParameter("@Latitude", obj.Latitude),
               new SqlParameter("@Longitude", obj.Longitude),
               new SqlParameter("@Visible", obj.Visible),
               new SqlParameter("@IsUseGlobalSMTPSettings", obj.IsUseGlobalSMTPSettings),
               new SqlParameter("@SMTPServer", obj.SMTPServer),
               new SqlParameter("@SMTPUsername", obj.SMTPUsername),
               new SqlParameter("@SMTPPassword", obj.SMTPPassword),
               new SqlParameter("@SMTPPort", obj.SMTPPort),
               new SqlParameter("@SMTPUseSSL", obj.SMTPUseSSL),
               new SqlParameter("@SMTPUseVERP", obj.SMTPUseVERP),
               new SqlParameter("@BouncedReturnEmail", obj.BouncedReturnEmail),
               new SqlParameter("@DefaultSenderName", obj.DefaultSenderName),
               new SqlParameter("@DefaultSenderEmail", obj.DefaultSenderEmail),
               new SqlParameter("@IsTrackingOpenMail", obj.IsTrackingOpenMail),
               new SqlParameter("@IsTrackingLinkClick", obj.IsTrackingLinkClick),
               new SqlParameter("@IsUseGlobalNetmessageSettings", obj.IsUseGlobalNetmessageSettings),
               new SqlParameter("@NetmessageFTPUsername", obj.NetmessageFTPUsername),
               new SqlParameter("@NetmessageFTPPassword", obj.NetmessageFTPPassword),
               new SqlParameter("@NetmessageAccountName", obj.NetmessageAccountName),
               new SqlParameter("@IsCommercial", obj.IsCommercial),
               new SqlParameter("@IsAssociative", obj.IsAssociative),
               new SqlParameter("@IsRegie", obj.IsRegie),
               new SqlParameter("@ManageRestaurantFlag", obj.ManageRestaurantFlag),
               new SqlParameter("@ManageProshopFlag", obj.ManageProshopFlag),
               new SqlParameter("@ManageFieldFlag", obj.ManageFieldFlag),
               new SqlParameter("@ManageGolfFlag", obj.ManageGolfFlag),
               new SqlParameter("@ManagerPhone", obj.ManagerPhone),
               new SqlParameter("@RespReceptionPhone", obj.RespReceptionPhone),
               new SqlParameter("@GreenKeeperPhone", obj.GreenKeeperPhone),
               new SqlParameter("@RespProshopPhone", obj.RespProshopPhone),
               new SqlParameter("@RestaurateurPhone", obj.RestaurateurPhone),
               new SqlParameter("@AssociationPresidentPhone", obj.AssociationPresidentPhone),
               new SqlParameter("@ManagerEmail", obj.ManagerEmail),
               new SqlParameter("@RespReceptionEmail", obj.RespReceptionEmail),
               new SqlParameter("@GreenKeeperEmail", obj.GreenKeeperEmail),
               new SqlParameter("@RespProshopEmail", obj.RespProshopEmail),
               new SqlParameter("@RestaurateurEmail", obj.RestaurateurEmail),
               new SqlParameter("@AssociationPresidentEmail", obj.AssociationPresidentEmail),
               new SqlParameter("@CommercialNBSubscriber", obj.CommercialNBSubscriber),
               new SqlParameter("@CommercialNBGF", obj.CommercialNBGF),
               new SqlParameter("@CommercialTurnover", obj.CommercialTurnover),
               new SqlParameter("@CommercialReservationSystem", obj.CommercialReservationSystem),
               new SqlParameter("@HotelOnSite", obj.HotelOnSite),
               new SqlParameter("@HotelPartner", obj.HotelPartner),
               new SqlParameter("@LydiaVendorToken", obj.LydiaVendorToken),
               new SqlParameter("@LydiaVendorId", obj.LydiaVendorId));
        }
        #endregion

        #region DeleteSite
        public int DeleteSite(long id)
        {
            string sql = "UPDATE [Site] SET [Active] = 0 WHERE [SiteId] = @SiteId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", id));
        }
        public int DeleteSite(IEnumerable<long> ids)
        {
            string sql = "UPDATE [Site] SET [Active] = 0 WHERE [SiteId] IN(" + String.Join(",", ids) + ")";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        }
        #endregion

        #region GetSitesDropDownListData
        public List<Site> GetSitesDropDownListData(int langId = 1)
        {
            List<Site> list = new List<Site>();
            string sql = @"SELECT [Site].[SiteId], [SiteLang].[SiteName], [Site].[AlbatrosCourseId] FROM [Site]
                            LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] AND [SiteLang].[LangId] = @LangId
                            WHERE [Active] = 1 ORDER BY [SiteName]";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@LangID", langId));

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new Site()
                {
                    SiteId = DataManager.ToInt(row["SiteId"]),
                    AlbatrosCourseId = DataManager.ToInt(row["AlbatrosCourseId"]),
                    SiteName = DataManager.ToString(row["SiteName"])
                });
            }
            return list;
        }
        #endregion

        //public Site GetSiteBySlug(string slug, int langId = 1)
        //{
        //    Site site = null;
        //    DataSet ds = null;
        //    string sql = @"SELECT TOP(1) Site.*, SiteLang.SiteName, SiteLang.Description, SiteLang.PracticalInfo, SiteLang.Accommodation, SiteLang.Restaurant, SiteLang.[LangId]
        //                    FROM [Site]
        //                    LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] AND ([SiteLang].[LangId] = @LangId)
        //                    WHERE SiteSlug = @SiteSlug";
        //    ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@SiteSlug", slug),
        //        new SqlParameter("@LangId", langId));
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        DataRow row = ds.Tables[0].Rows[0];
        //        site = new Site(row);
        //        site.SiteName = DataManager.ToString(row["SiteName"]);
        //        site.Description = DataManager.ToString(row["Description"]);
        //        site.PracticalInfo = DataManager.ToString(row["PracticalInfo"]);
        //        site.Accommodation = DataManager.ToString(row["Accommodation"]);
        //        site.Restaurant = DataManager.ToString(row["Restaurant"]);
        //        site.SiteImages = new List<SiteImage>();
        //        site.SiteReviews = new List<SiteReview>();

        //        // Get Site Images.
        //        sql = "SELECT [SiteImage].* FROM [SiteImage] WHERE [SiteId] = " + site.SiteId + " ORDER BY [ListNo], [SiteImageId];";
        //        sql += "SELECT AVG(Rating) AS AverageRating, COUNT(SiteReviewId) AS ReviewNumber, [SiteId] FROM [SiteReview] WHERE [IsApproved] = 1 AND [SiteId] = " + site.SiteId + " GROUP BY [SiteId];";
        //        sql += @"SELECT TOP(3) [SiteReview].*, [Users].[Firstname] AS [ReviewerName], [City].[CityName] AS [ReviewerCityName] FROM [SiteReview] 
        //                INNER JOIN [Users] ON [Users].[UserId] = [SiteReview].[UserId] 
        //                LEFT JOIN [City] ON [City].[CityId] = [Users].[CityId] 
        //                WHERE [IsApproved] = 1 AND [SiteReview].[SiteId] = " + site.SiteId + " ORDER BY [UpdatedDate] DESC;";
        //        ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            foreach (DataRow imgRow in ds.Tables[0].Rows)
        //            {
        //                site.SiteImages.Add(new SiteImage(imgRow));
        //            }
        //        }
        //        if (ds.Tables[1].Rows.Count > 0)
        //        {
        //            site.AverageRating = DataManager.ToFloat(ds.Tables[1].Rows[0]["AverageRating"], 0);
        //            site.ReviewNumber = DataManager.ToInt(ds.Tables[1].Rows[0]["ReviewNumber"], 0);
        //        }
        //        if (ds.Tables[2].Rows.Count > 0)
        //        {
        //            foreach (DataRow reviewRow in ds.Tables[2].Rows)
        //            {
        //                site.SiteReviews.Add(new SiteReview(reviewRow));
        //            }
        //        }
        //    }
        //    return site;
        //}

        public bool IsExistsSiteSlug(string slug, long? siteId)
        {
            string sql = "SELECT COUNT(SiteId) FROM [Site] WHERE [SiteSlug] = @SiteSlug";
            if (siteId.HasValue)
            {
                sql += " AND [SiteId] <> " + siteId.Value;
            }
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteSlug", slug))) > 0;
        }

        #region GetSitesListBySiteId
        public List<Site> GetSitesListByCountryId(int countryId, int langId = 1)
        {
            string sql = @"SELECT [Site].[SiteId], [SiteLang].[SiteName] FROM [Site] 
                           INNER JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId]
                           WHERE [CountryId] = @CountryId AND [SiteLang].[LangId] = @LangId AND [Site].[Active] = 1 AND [Site].[Visible] = 1
                           ORDER BY [SiteName]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CountryId", countryId),
                new SqlParameter("@LangId", langId));

            List<Site> list = new List<Site>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new Site()
                    {
                        SiteId = DataManager.ToInt(row["SiteId"]),
                        SiteName = DataManager.ToString(row["SiteName"])
                    });
                }
            }
            return list;
        }
        #endregion

        #region GetSitesListByStateId
        public List<Site> GetSitesListByStateId(int stateId, int regionId = 0, int countryId = 0, int langId = 1)
        {
            string sql = @"SELECT [Site].[SiteId], [SiteLang].[SiteName] FROM [Site] 
                           INNER JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] AND [SiteLang].[LangId] = @LangId
                           INNER JOIN [State] ON [State].[StateId] = [Site].[StateId]
                           INNER JOIN [Region] ON [Region].[RegionId] = [State].[RegionId]
                           INNER JOIN [Country] ON [Country].[CountryId] = [Region].[CountryId]
                           WHERE [Site].[Active] = 1 AND [Site].[Visible] = 1";
            if (stateId > 0)
            {
                sql += " AND [Site].[StateId] = " + stateId;
            }
            else if (regionId > 0)
            {
                sql += " AND [State].[RegionId] = " + regionId;
            }
            else if (countryId > 0)
            {
                sql += " AND [Region].[CountryId] = " + countryId;
            }
            sql += " ORDER BY [SiteName], [Country].[CountryId], [Region].[RegionName], [State].[StateName]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@StateId", stateId),
                new SqlParameter("@LangId", langId));

            List<Site> list = new List<Site>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new Site()
                    {
                        SiteId = DataManager.ToInt(row["SiteId"]),
                        SiteName = DataManager.ToString(row["SiteName"])
                    });
                }
            }
            return list;
        }
        #endregion

        #region GetSiteIdBySlug
        /// <summary>
        /// Get Site ID by Site Slug
        /// </summary>
        /// <param name="slug">Site Slug</param>
        /// <returns>Site ID</returns>
        public int GetSiteIdBySlug(string slug)
        {
            string sql = "SELECT [SiteId] FROM [Site] WHERE [SiteSlug] = @SiteSlug";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteSlug", slug)));
        }
        #endregion

        #region GetAlbatrosSiteIds
        public List<int> GetAlbatrosSiteIds()
        {
            string sql = "SELECT SiteId FROM Site WHERE AlbatrosCourseId > 0 ORDER BY SiteId";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
            List<int> siteIds = new List<int>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                siteIds.Add(DataManager.ToInt(row["SiteId"]));
            }
            return siteIds;
        }
        #endregion

        #region GetReservationAPIBySiteId
        /// <summary>
        /// Get Reservation API Type by SiteId
        /// </summary>
        /// <param name="siteId">SiteId</param>
        /// <returns>Reservation API Type</returns>
        public ReservationAPIType GetReservationAPIBySiteId(long siteId)
        {
            string sql = "SELECT ISNULL(APIId, 0) AS APIId FROM Site it JOIN ReservationAPI api ON api.APIId = it.ReservationAPI WHERE it.SiteId = " + siteId;
            int apiId = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql), 0);
            return (ReservationAPIType)apiId;
        }
        #endregion

        #region GetSitePinData
        public DataSet GetSitePinData(int langId = 1, int? countryId = null, int? regionId = null, int? stateId = null, long? siteId = null)
        {
            string sql = @"SELECT [Site].[SiteId], [Site].[Latitude], [Site].[Longitude], [SiteLang].[SiteName], [SiteLang].[Description], [Site].[SiteSlug]
                        FROM [Site] 
                        LEFT OUTER JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] AND [SiteLang].[LangId] = @LangId
                        LEFT OUTER JOIN [State] ON [State].[StateId] = [Site].[StateId]
                        LEFT OUTER JOIN [Region] ON [Region].[RegionId] = [State].[RegionId]
                        LEFT OUTER JOIN [Country] ON [Country].[CountryId] = [Region].[CountryId]
                        WHERE [Site].[Active] = 1 AND [Site].[Visible] = 1 ";
            if (siteId.HasValue && siteId.Value > 0)
            {
                sql += " AND [Site].[SiteId] = " + siteId.Value;
            }
            else if (stateId.HasValue && stateId.Value > 0)
            {
                sql += " AND [Site].[StateId] = " + stateId.Value;
            }
            else if (regionId.HasValue && regionId.Value > 0)
            {
                sql += " AND [State].[RegionId] = " + regionId.Value;
            }
            else if (countryId.HasValue && countryId.Value > 0)
            {
                sql += " AND [Region].[CountryId] = " + countryId.Value;
            }
            sql += " ORDER BY [Site].[SiteId];";
            sql += "SELECT [ItemId], [ItemTypeId], [SiteId] FROM [Item] WHERE [Active] = 1 AND [IsPublish] = 1 AND (ISNULL([PublishStartDate], GETDATE()) <= GETDATE() AND DATEADD(DAY, 1, ISNULL([PublishEndDate], GETDATE())) >= GETDATE());";
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@LangId", langId));

        }
        #endregion

        #region GetAllSitesWithAlbatrosClubIds
        public List<Site> GetAllSitesWithAlbatrosClubIds(int langId = 1)
        {
            List<Site> list = new List<Site>();
            string sql = "SELECT [Site].[SiteId], [SiteLang].[SiteName] as SiteName, [Site].[AlbatrosClubId] FROM [Site] LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] AND [SiteLang].[LangId] = @LangId WHERE [Active] = 1 AND [Visible] = 1 ORDER BY [SiteLang].[SiteName]";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@LangId", langId));

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new Site()
                {
                    SiteId = DataManager.ToLong(row["SiteId"]),
                    SiteName = DataManager.ToString(row["SiteName"]),
                    AlbatrosClubId = DataManager.ToLong(row["AlbatrosClubId"])
                });
            }
            return list;
        }
        #endregion

        #region UpdateSiteAlbatrosClubId
        public void UpdateSiteAlbatrosClubId(long siteId, long clubId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "UPDATE [Site] SET [AlbatrosClubId] = @ClubId WHERE [SiteId] = @SiteId",
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("@ClubId", clubId));
        }
        #endregion

        #region GetAllSiteIds
        public List<long> GetAllSiteIds()
        {
            List<long> siteIds = new List<long>();

            using (var db = GetModels())
            {
                siteIds = db.Sites.Where(it => it.Active == true).Select(it => it.SiteId).ToList();
            }

            return siteIds;
        }
        #endregion

        #region GetAllSitesWithChronogolfClubId
        /// <summary>
        /// Get all sites that have Chronogolf Club Id.
        /// </summary>
        /// <returns></returns>
        public List<long> GetAllSitesWithChronogolfClubId()
        {
            List<long> siteIds = null;

            using (var db = GetModels())
            {
                siteIds = db.Sites.Where(it => it.Active == true
                && it.ChronogolfClubId.HasValue
                && it.ChronogolfClubId.Value > 0).Select(it => it.SiteId).ToList();
            }

            return siteIds;
        }
        #endregion

        #region SaveCustomerType
        #endregion

        #endregion

        #region SiteImage
        public long AddSiteImage(SiteImage obj)
        {
            string sql = @"INSERT INTO [SiteImage](SiteId, ImageName, ListNo, BaseName, FileExtension) VALUES(@SiteId, @ImageName, 
               (SELECT COUNT(*) FROM [SiteImage] WHERE [SiteId] = @SiteId)
            , @BaseName, @FileExtension) SELECT @@IDENTITY";
            return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", obj.SiteId),
                new SqlParameter("@ImageName", obj.SiteId),
                new SqlParameter("@BaseName", obj.BaseName),
                new SqlParameter("@FileExtension", obj.FileExtension)), -1);
        }

        public int DeleteSiteImagesBySiteId(int id)
        {
            string sql = "DELETE FROM [SiteImage] WHERE [SiteId] = " + id;
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        }

        public long SaveSiteImage(SiteImage obj)
        {
            return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, "SaveSiteImage",
                new SqlParameter("@SiteImageId", obj.SiteImageId),
                new SqlParameter("@SiteId", obj.SiteId),
                new SqlParameter("@ImageName", obj.ImageName),
                new SqlParameter("@ListNo", obj.ListNo),
                new SqlParameter("@BaseName", obj.BaseName),
                new SqlParameter("@FileExtension", obj.FileExtension)), -1);
        }

        public IQueryable<SiteImage> GetSiteImagesBySiteId(long id)
        {
            using (var db = GetModels())
            {
                return db.SiteImages.Where(it => it.SiteId == id);
            }
        }

        public long DeleteSiteImage(long id)
        {
            string sql = "DELETE FROM [SiteImage] WHERE [SiteImageId] = " + id;
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        }

        public SiteImage GetSiteImage(long id)
        {
            SiteImage obj = null;

            using (var db = GetModels())
            {
                db.SiteImages.Where(it => it.SiteImageId == id).ToList().ForEach(it => obj = it);
            }

            return obj;
        }
        #endregion

        #region SiteLang
        public SiteLang GetSiteLang(long SiteId, int langId)
        {
            SiteLang obj = new SiteLang();
            obj.LangId = langId;
            obj.SiteId = SiteId;

            using (var db = GetModels())
            {
                (from it in db.SiteLangs
                 where it.SiteId == SiteId && it.LangId == langId
                 select it).ToList().ForEach(it => obj = it);
            }

            return obj;
        }

        public int DeleteSiteLangBySiteId(long SiteId)
        {
            string sql = "DELETE FROM [SiteLang] WHERE [SiteId] = " + SiteId;
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        }

        public int SaveSiteLang(SiteLang obj)
        {
            string sql = @"IF(EXISTS(SELECT [SiteId] FROM [SiteLang] WHERE [SiteId] = @SiteId AND [LangId] = @LangId))
                            BEGIN
                                UPDATE [SiteLang] SET [SiteName] = @SiteName, [Description] = @Description, [PracticalInfo] = @PracticalInfo, [Accommodation] = @Accommodation, [Restaurant] = @Restaurant WHERE [SiteId] = @SiteId AND [LangId] = @LangId;
                            END
                            ELSE
                            BEGIN
                                INSERT INTO [SiteLang]([SiteId], [LangId], [SiteName], [Description], [PracticalInfo], [Accommodation], [Restaurant])
                                VALUES(@SiteId, @LangId, @SiteName, @Description, @PracticalInfo, @Accommodation, @Restaurant) SELECT @@IDENTITY;
                            END";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", obj.SiteId),
                new SqlParameter("@LangId", obj.LangId),
                new SqlParameter("@SiteName", obj.SiteName),
                new SqlParameter("@Description", obj.Description),
                new SqlParameter("@PracticalInfo", obj.PracticalInfo),
                new SqlParameter("@Accommodation", obj.Accommodation),
                new SqlParameter("@Restaurant", obj.Restaurant));
        }
        #endregion

        #region SiteReview
        //#region GetAllSiteReviews
        //public List<SiteReview> GetAllSiteReviews()
        //{
        //    List<SiteReview> list = new List<SiteReview>();
        //    string sql = "SELECT * FROM [SiteReviews] ORDER BY [SiteReviewId]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new SiteReview(row));
        //    }
        //    return list;
        //}

        //public List<SiteReview> GetAllSiteReviews(jQueryDataTableParamModel param, int langId = 1)
        //{
        //    List<SiteReview> list = new List<SiteReview>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectSiteReviewsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@LangId", langId));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new SiteReview(row));
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //#endregion

        //#region GetSiteReview
        //public SiteReview GetSiteReview(long itemId, int userId)
        //{
        //    SiteReview obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectSiteReview",
        //        new SqlParameter("@SiteId", itemId),
        //        new SqlParameter("@UserId", userId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new SiteReview(ds.Tables[0].Rows[0]);
        //    }

        //    return obj;
        //}
        //#endregion

        //#region AddSiteReview
        //public int AddSiteReview(SiteReview obj)
        //{
        //    string sql = "INSERT INTO [SiteReviews](Rating, Subject, Message, UserId, SiteId, IsApproved) VALUES(@Rating, @Message, @UserId, @SiteId, @IsApproved) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@Rating", obj.Rating),
        //       new SqlParameter("@Subject", obj.Subject),
        //       new SqlParameter("@Message", obj.Message),
        //       new SqlParameter("@UserId", obj.UserId),
        //       new SqlParameter("@SiteId", obj.SiteId),
        //       new SqlParameter("@IsApproved", obj.IsApproved)));
        //}
        //#endregion

        //#region UpdateSiteReview
        //public int UpdateSiteReview(SiteReview obj)
        //{
        //    string sql = "UPDATE [SiteReviews] SET [Rating] = @Rating, [Subject] = @Subject, [Message] = @Message, [IsApproved] = @IsApproved WHERE [UserId] = @UserId, [SiteId] = @SiteId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@Rating", obj.Rating),
        //       new SqlParameter("@Subject", obj.Subject),
        //       new SqlParameter("@Message", obj.Message),
        //       new SqlParameter("@UserId", obj.UserId),
        //       new SqlParameter("@SiteId", obj.SiteId),
        //       new SqlParameter("@IsApproved", obj.IsApproved));
        //}
        //#endregion

        //#region DeleteSiteReview
        //public int DeleteSiteReview(long itemId, int userId)
        //{
        //    string sql = "DELETE FROM [SiteReviews] WHERE [SiteId] = @SiteId AND  [UserId] = @UserId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@SiteId", itemId),
        //        new SqlParameter("@UserId", userId));
        //}
        //#endregion

        //public void SaveSiteReview(long siteId, int userId, byte rating, string subject, string message, out int averageRating, out int reviewNumber)
        //{
        //    string sql = "INSERT INTO [SiteReview]([Rating], [Subject], [Message], [UserId], [SiteId], [IsApproved]) VALUES(@Rating, @Subject, @Message, @UserId, @SiteId, 0)";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@SiteId", siteId),
        //        new SqlParameter("@UserId", userId),
        //        new SqlParameter("@Rating", rating),
        //        new SqlParameter("@Subject", subject),
        //        new SqlParameter("@Message", message));

        //    averageRating = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT AVG([Rating]) FROM [SiteReview] WHERE [IsApproved] = 1 AND [SiteId] = @SiteId",
        //        new SqlParameter("@SiteId", siteId)), 0);

        //    reviewNumber = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT COUNT([SiteReviewId]) FROM [SiteReview] WHERE [IsApproved] = 1 AND [SiteId] = @SiteId",
        //        new SqlParameter("@SiteId", siteId)), 0);
        //}

        //public List<SiteReview> GetSiteReviewsBySiteId(long itemId)
        //{
        //    List<SiteReview> reviews = new List<SiteReview>();
        //    string sql = @"SELECT TOP(3) [SiteReview].*, [Users].[Firstname] + ' ' + [Users].[Lastname] AS [ReviewerName] FROM [SiteReview]
        //                    INNER JOIN [Users] ON [Users].[UserId] = [SiteReview].[UserId]
        //                    LEFT JOIN [City] ON [City].[CityId] = [Users].[CityId] 
        //                    WHERE [IsApproved] = 1 AND [SiteReview].[SiteId] = " + itemId +
        //                    " ORDER BY [UpdatedDate] DESC";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        reviews.Add(new SiteReview(row));
        //    }
        //    return reviews;
        //}


        //#region DeleteSiteReview
        //public void DeleteSiteReview(IEnumerable<int> ids)
        //{
        //    if (!ids.Any())
        //        return;

        //    string sql = "DELETE FROM [SiteReview] WHERE [SiteReviewId] IN(" + string.Join(",", ids) + ")";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}
        //#endregion

        //#region UpdateSiteReviewApproval
        //public void UpdateSiteReviewApproval(int id, bool isApproved)
        //{
        //    string sql = "UPDATE [SiteReview] SET [IsApproved] = @IsApproved WHERE [SiteReviewId] = @SiteReviewId";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@SiteReviewId", id),
        //        new SqlParameter("@IsApproved", isApproved));
        //}
        //#endregion
        #endregion

        #region SiteFinancial
        public SiteFinancial GetSiteFinancial(long siteId, int fiscalYear)
        {
            SiteFinancial obj = new SiteFinancial() { SiteId = siteId, FiscalYear = fiscalYear, PaymentDate = DateTime.Today, PaymentType = 0 };

            using (var db = GetModels())
            {
                db.SiteFinancials.Where(it => it.SiteId == siteId && it.FiscalYear == fiscalYear).ToList().ForEach(it => obj = it);
            }

            return obj;
        }

        public void SaveSiteFinancial(SiteFinancial obj)
        {
            string sql = @"IF EXISTS(SELECT * FROM [SiteFinancial] WHERE [SiteId] = @SiteId AND [FiscalYear] = @FiscalYear)
                           BEGIN
                                UPDATE [SiteFinancial] SET [MemberAmount] = @MemberAmount, [PaymentType] = @PaymentType, [PaymentDate] = @PaymentDate, [ClassicCard] = @ClassicCard, [ClassicCardTurnover] = @ClassicCardTurnover, [GolfCardTurnover] = @GolfCardTurnover, [ClassicCorpoTurnover] = @ClassicCorpoTurnover, [RFAPractice] = @RFAPractice, [RFAProshop] = @RFAProshop, [RFAField] = @RFAField, [RFAServices] = @RFAServices, [RFACarts] = @RFACarts, [RFARestauration] = @RFARestauration, [RFATotal] = @RFATotal, [AdsPackVisibility] = @AdsPackVisibility, [AdsAdditionalServices] = @AdsAdditionalServices, [AdsShow] = @AdsShow, [CommisionGFOnline] = @CommisionGFOnline, [UpdatedBy] = @UpdatedBy, [UpdatedDate] = @UpdatedDate, [Active] = @Active WHERE [SiteId] = @SiteId AND [FiscalYear] = @FiscalYear
                           END
                           ELSE
                           BEGIN
                                INSERT INTO [SiteFinancial]([SiteId], [FiscalYear], [MemberAmount], [PaymentType], [PaymentDate], [ClassicCard], [ClassicCardTurnover], [GolfCardTurnover], [ClassicCorpoTurnover], [RFAPractice], [RFAProshop], [RFAField], [RFAServices], [RFACarts], [RFARestauration], [RFATotal], [AdsPackVisibility], [AdsAdditionalServices], [AdsShow], [CommisionGFOnline], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Active])
                                VALUES(@SiteId, @FiscalYear, @MemberAmount, @PaymentType, @PaymentDate, @ClassicCard, @ClassicCardTurnover, @GolfCardTurnover, @ClassicCorpoTurnover, @RFAPractice, @RFAProshop, @RFAField, @RFAServices, @RFACarts, @RFARestauration, @RFATotal, @AdsPackVisibility, @AdsAdditionalServices, @AdsShow, @CommisionGFOnline, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate, @Active)
                           END";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", obj.SiteId),
                new SqlParameter("@FiscalYear", obj.FiscalYear),
                new SqlParameter("@MemberAmount", obj.MemberAmount),
                new SqlParameter("@PaymentType", obj.PaymentType),
                new SqlParameter("@PaymentDate", obj.PaymentDate),
                new SqlParameter("@ClassicCard", obj.ClassicCard),
                new SqlParameter("@ClassicCardTurnover", obj.ClassicCardTurnover),
                new SqlParameter("@GolfCardTurnover", obj.GolfCardTurnover),
                new SqlParameter("@ClassicCorpoTurnover", obj.ClassicCorpoTurnover),
                new SqlParameter("@RFAPractice", obj.RFAPractice),
                new SqlParameter("@RFAProshop", obj.RFAProshop),
                new SqlParameter("@RFAField", obj.RFAField),
                new SqlParameter("@RFAServices", obj.RFAServices),
                new SqlParameter("@RFACarts", obj.RFACarts),
                new SqlParameter("@RFARestauration", obj.RFARestauration),
                new SqlParameter("@RFATotal", obj.RFATotal),
                new SqlParameter("@AdsPackVisibility", obj.AdsPackVisibility),
                new SqlParameter("@AdsAdditionalServices", obj.AdsAdditionalServices),
                new SqlParameter("@AdsShow", obj.AdsShow),
                new SqlParameter("@CommisionGFOnline", obj.CommisionGFOnline),
                new SqlParameter("@CreatedBy", obj.CreatedBy),
                new SqlParameter("@CreatedDate", obj.CreatedDate),
                new SqlParameter("@UpdatedBy", obj.UpdatedBy),
                new SqlParameter("@UpdatedDate", obj.UpdatedDate),
                new SqlParameter("@Active", obj.Active));
        }
        #endregion

        #region SiteEvent
        public SiteEvent GetSiteEvent(long siteId, int fiscalYear)
        {
            SiteEvent obj = new SiteEvent() { SiteId = siteId, FiscalYear = fiscalYear, ChallengeDate = DateTime.Today, HasChallenge = false, PingClassicTourDate = DateTime.Today, HasPingClassicTour = false };

            using (var db = GetModels())
            {
                db.SiteEvents.Where(it => it.SiteId == siteId && it.FiscalYear == fiscalYear).ToList().ForEach(it => obj = it);
            }

            return obj;
        }

        public void SaveSiteEvent(SiteEvent obj)
        {
            string sql = @"IF EXISTS(SELECT * FROM [SiteEvent] WHERE [SiteId] = @SiteId AND [FiscalYear] = @FiscalYear)
                           BEGIN
                                UPDATE [SiteEvent] SET [HasChallenge] = @HasChallenge, [ChallengeDate] = @ChallengeDate, [HasPingClassicTour] = @HasPingClassicTour, [PingClassicTourDate] = @PingClassicTourDate, [HasCongressOwner] = @HasCongressOwner, [HasCongressManager] = @HasCongressManager, [HasCongressGreenKeeper] = @HasCongressGreenKeeper, [HasCongressRestaurateur] = @HasCongressRestaurateur, [HasCongressRespProshop] = @HasCongressRespProshop, [HasShowroomManager] = @HasShowroomManager, [HasShowroomRespProshop] = @HasShowroomRespProshop, [UpdatedBy] = @UpdatedBy, [UpdatedDate] = @UpdatedDate, [Active] = @Active WHERE [SiteId] = @SiteId AND [FiscalYear] = @FiscalYear
                           END
                           ELSE
                           BEGIN
                                INSERT INTO [SiteEvent]([SiteId], [FiscalYear], [HasChallenge], [ChallengeDate], [HasPingClassicTour], [PingClassicTourDate], [HasCongressOwner], [HasCongressManager], [HasCongressGreenKeeper], [HasCongressRestaurateur], [HasCongressRespProshop], [HasShowroomManager], [HasShowroomRespProshop], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Active])
                                VALUES(@SiteId, @FiscalYear, @HasChallenge, @ChallengeDate, @HasPingClassicTour, @PingClassicTourDate, @HasCongressOwner, @HasCongressManager, @HasCongressGreenKeeper, @HasCongressRestaurateur, @HasCongressRespProshop, @HasShowroomManager, @HasShowroomRespProshop, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate, @Active)
                           END";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", obj.SiteId),
                new SqlParameter("@FiscalYear", obj.FiscalYear),
                new SqlParameter("@HasChallenge", obj.HasChallenge),
                new SqlParameter("@ChallengeDate", obj.ChallengeDate),
                new SqlParameter("@HasPingClassicTour", obj.HasPingClassicTour),
                new SqlParameter("@PingClassicTourDate", obj.PingClassicTourDate),
                new SqlParameter("@HasCongressOwner", obj.HasCongressOwner),
                new SqlParameter("@HasCongressManager", obj.HasCongressManager),
                new SqlParameter("@HasCongressGreenKeeper", obj.HasCongressGreenKeeper),
                new SqlParameter("@HasCongressRestaurateur", obj.HasCongressRestaurateur),
                new SqlParameter("@HasCongressRespProshop", obj.HasCongressRespProshop),
                new SqlParameter("@HasShowroomManager", obj.HasShowroomManager),
                new SqlParameter("@HasShowroomRespProshop", obj.HasShowroomRespProshop),
                new SqlParameter("@CreatedBy", obj.CreatedBy),
                new SqlParameter("@CreatedDate", obj.CreatedDate),
                new SqlParameter("@UpdatedBy", obj.UpdatedBy),
                new SqlParameter("@UpdatedDate", obj.UpdatedDate),
                new SqlParameter("@Active", obj.Active));
        }
        #endregion

        #region SiteCommunication
        public SiteCommunication GetSiteCommunication(long siteId, int fiscalYear)
        {
            SiteCommunication obj = new SiteCommunication() { SiteId = siteId, FiscalYear = fiscalYear, NewslettersSendDate = DateTime.Today };

            using (var db = GetModels())
            {
                db.SiteCommunications.Where(it => it.SiteId == siteId && it.FiscalYear == fiscalYear).ToList().ForEach(it => obj = it);
            }

            return obj;
        }

        public void SaveSiteCommunication(SiteCommunication obj)
        {
            string sql = @"IF EXISTS(SELECT * FROM [SiteCommunication] WHERE [SiteId] = @SiteId AND [FiscalYear] = @FiscalYear)
                           BEGIN
                                UPDATE [SiteCommunication] SET [UsePageGuide] = @UsePageGuide, [UsePageSite] = @UsePageSite, [UseStaysOffers] = @UseStaysOffers, [NewslettersOrder] = @NewslettersOrder, [NewslettersSendDate] = @NewslettersSendDate, [NewslettersSendDetail] = @NewslettersSendDetail, [UseTerminal] = @UseTerminal, [UseShowBooklet] = @UseShowBooklet, [UseLogoOnSite] = @UseLogoOnSite, [UseTotem] = @UseTotem, [UseGuideFlyers] = @UseGuideFlyers, [UseSheet] = @UseSheet, [UpdatedBy] = @UpdatedBy, [UpdatedDate] = @UpdatedDate, [Active] = @Active WHERE [SiteId] = @SiteId AND [FiscalYear] = @FiscalYear
                           END
                           ELSE
                           BEGIN
                                INSERT INTO [SiteCommunication]([SiteId], [FiscalYear], [UsePageGuide], [UsePageSite], [UseStaysOffers], [NewslettersOrder], [NewslettersSendDate], [NewslettersSendDetail], [UseTerminal], [UseShowBooklet], [UseLogoOnSite], [UseTotem], [UseGuideFlyers], [UseSheet], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Active])
                                VALUES(@SiteId ,@FiscalYear ,@UsePageGuide ,@UsePageSite ,@UseStaysOffers ,@NewslettersOrder ,@NewslettersSendDate ,@NewslettersSendDetail ,@UseTerminal ,@UseShowBooklet ,@UseLogoOnSite ,@UseTotem ,@UseGuideFlyers ,@UseSheet, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate, @Active)
                           END";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", obj.SiteId),
                new SqlParameter("@FiscalYear", obj.FiscalYear),
                new SqlParameter("@UsePageGuide", obj.UsePageGuide),
                new SqlParameter("@UsePageSite", obj.UsePageSite),
                new SqlParameter("@UseStaysOffers", obj.UseStaysOffers),
                new SqlParameter("@NewslettersOrder", obj.NewslettersOrder),
                new SqlParameter("@NewslettersSendDate", obj.NewslettersSendDate),
                new SqlParameter("@NewslettersSendDetail", obj.NewslettersSendDetail),
                new SqlParameter("@UseTerminal", obj.UseTerminal),
                new SqlParameter("@UseShowBooklet", obj.UseShowBooklet),
                new SqlParameter("@UseLogoOnSite", obj.UseLogoOnSite),
                new SqlParameter("@UseTotem", obj.UseTotem),
                new SqlParameter("@UseGuideFlyers", obj.UseGuideFlyers),
                new SqlParameter("@UseSheet", obj.UseSheet),
                new SqlParameter("@CreatedBy", obj.CreatedBy),
                new SqlParameter("@CreatedDate", obj.CreatedDate),
                new SqlParameter("@UpdatedBy", obj.UpdatedBy),
                new SqlParameter("@UpdatedDate", obj.UpdatedDate),
                new SqlParameter("@Active", obj.Active));
        }
        #endregion

        #region SiteCommunication
        public SiteCentralLineSEO GetSiteCentralLineSEO(long siteId, int fiscalYear)
        {
            SiteCentralLineSEO obj = new SiteCentralLineSEO() { SiteId = siteId, FiscalYear = fiscalYear, InformationRequestDate = DateTime.Today };

            using (var db = GetModels())
            {
                db.SiteCentralLineSEOs.Where(it => it.SiteId == siteId && it.FiscalYear == fiscalYear).ToList().ForEach(it => obj = it);
            }

            return obj;
        }

        public void SaveSiteCentralLineSEO(SiteCentralLineSEO obj)
        {
            string sql = @"IF EXISTS(SELECT * FROM [SiteCentralLineSEO] WHERE [SiteId] = @SiteId AND [FiscalYear] = @FiscalYear)
                           BEGIN
                                UPDATE [SiteCentralLineSEO] SET [InvoiceEntryField] = @InvoiceEntryField, [InvoiceEntryRestaurant] = @InvoiceEntryRestaurant, [InvoiceEntryOther] = @InvoiceEntryOther, [InformationRequestDate] = @InformationRequestDate, [BeingCalledByProductResp] = @BeingCalledByProductResp, [InformationRequestOther] = @InformationRequestOther, [UpdatedBy] = @UpdatedBy, [UpdatedDate] = @UpdatedDate, [Active] = @Active WHERE [SiteId] = @SiteId AND [FiscalYear] = @FiscalYear
                           END
                           ELSE
                           BEGIN
                                INSERT INTO [SiteCentralLineSEO]([SiteId], [FiscalYear], [InvoiceEntryField], [InvoiceEntryRestaurant], [InvoiceEntryOther], [InformationRequestDate], [BeingCalledByProductResp], [InformationRequestOther], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Active])
                                VALUES(@SiteId, @FiscalYear, @InvoiceEntryField, @InvoiceEntryRestaurant, @InvoiceEntryOther, @InformationRequestDate, @BeingCalledByProductResp, @InformationRequestOther, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate, @Active)
                           END";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", obj.SiteId),
                new SqlParameter("@FiscalYear", obj.FiscalYear),
                new SqlParameter("@InvoiceEntryField", obj.InvoiceEntryField),
                new SqlParameter("@InvoiceEntryRestaurant", obj.InvoiceEntryRestaurant),
                new SqlParameter("@InvoiceEntryOther", obj.InvoiceEntryOther),
                new SqlParameter("@InformationRequestDate", obj.InformationRequestDate),
                new SqlParameter("@BeingCalledByProductResp", obj.BeingCalledByProductResp),
                new SqlParameter("@InformationRequestOther", obj.InformationRequestOther),
                new SqlParameter("@CreatedBy", obj.CreatedBy),
                new SqlParameter("@CreatedDate", obj.CreatedDate),
                new SqlParameter("@UpdatedBy", obj.UpdatedBy),
                new SqlParameter("@UpdatedDate", obj.UpdatedDate),
                new SqlParameter("@Active", obj.Active));
        }
        #endregion

        #region SiteCommercialFollowUp
        public SiteCommercialFollowUp GetSiteCommercialFollowUp(long siteId, DateTime visitDate)
        {
            SiteCommercialFollowUp obj = new SiteCommercialFollowUp() { SiteId = siteId, VisitDate = visitDate };

            using (var db = GetModels())
            {
                db.SiteCommercialFollowUps.Where(it => it.SiteId == siteId && it.VisitDate == visitDate).ToList().ForEach(it => obj = it);
            }
            return obj;
        }

        public void SaveSiteCommercialFollowUp(SiteCommercialFollowUp obj)
        {
            string sql = @"IF EXISTS(SELECT * FROM [SiteCommercialFollowUp] WHERE [SiteId] = @SiteId AND [VisitDate] = @VisitDate)
                           BEGIN
                                UPDATE [SiteCommercialFollowUp] SET [ContractualTerms] = @ContractualTerms, [NeedsInCommunicationTools] = @NeedsInCommunicationTools, [CardSeller] = @CardSeller, [HotelPartnerSeeking] = @HotelPartnerSeeking, [AdsSpotSelling] = @AdsSpotSelling, [SellGFOnline] = @SellGFOnline, [CentralPurchasingPoint] = @CentralPurchasingPoint, [FormationNeeds] = @FormationNeeds, [ConsultingNeeds] = @ConsultingNeeds, [FidelityShop] = @FidelityShop, [RegionalProspectingPoint] = @RegionalProspectingPoint, [UpdatedBy] = @UpdatedBy, [UpdatedDate] = @UpdatedDate, [Active] = @Active WHERE [SiteId] = @SiteId AND [VisitDate] = @VisitDate
                           END
                           ELSE
                           BEGIN
                                INSERT INTO [SiteCommercialFollowUp]([SiteId], [VisitDate], [ContractualTerms], [NeedsInCommunicationTools], [CardSeller], [HotelPartnerSeeking], [AdsSpotSelling], [SellGFOnline], [CentralPurchasingPoint], [FormationNeeds], [ConsultingNeeds], [FidelityShop], [RegionalProspectingPoint], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Active])
                                VALUES(@SiteId, @VisitDate, @ContractualTerms, @NeedsInCommunicationTools, @CardSeller, @HotelPartnerSeeking, @AdsSpotSelling, @SellGFOnline, @CentralPurchasingPoint, @FormationNeeds, @ConsultingNeeds, @FidelityShop, @RegionalProspectingPoint, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate, @Active)
                           END";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", obj.SiteId),
                new SqlParameter("@VisitDate", obj.VisitDate),
                new SqlParameter("@ContractualTerms", obj.ContractualTerms),
                new SqlParameter("@NeedsInCommunicationTools", obj.NeedsInCommunicationTools),
                new SqlParameter("@CardSeller", obj.CardSeller),
                new SqlParameter("@HotelPartnerSeeking", obj.HotelPartnerSeeking),
                new SqlParameter("@AdsSpotSelling", obj.AdsSpotSelling),
                new SqlParameter("@SellGFOnline", obj.SellGFOnline),
                new SqlParameter("@CentralPurchasingPoint", obj.CentralPurchasingPoint),
                new SqlParameter("@FormationNeeds", obj.FormationNeeds),
                new SqlParameter("@ConsultingNeeds", obj.ConsultingNeeds),
                new SqlParameter("@FidelityShop", obj.FidelityShop),
                new SqlParameter("@RegionalProspectingPoint", obj.RegionalProspectingPoint),
                new SqlParameter("@CreatedBy", obj.CreatedBy),
                new SqlParameter("@CreatedDate", obj.CreatedDate),
                new SqlParameter("@UpdatedBy", obj.UpdatedBy),
                new SqlParameter("@UpdatedDate", obj.UpdatedDate),
                new SqlParameter("@Active", obj.Active));
        }
        #endregion

        #region RestaurantSupplier
        public List<RestaurantSupplier> GetRestaurantSupplierDropDownListData()
        {
            List<RestaurantSupplier> list = new List<RestaurantSupplier>();
            string sql = @"SELECT [RestaurantSupplierId], [SupplierName] FROM [RestaurantSupplier]
                            WHERE [Active] = 1 ORDER BY [SupplierName]";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new RestaurantSupplier()
                {
                    RestaurantSupplierId = DataManager.ToLong(row["RestaurantSupplierId"]),
                    SupplierName = DataManager.ToString(row["SupplierName"])
                });
            }
            return list;
        }

        public List<long> GetSiteRestaurantSupplierIds(long siteId, int fiscalYear)
        {
            string sql = "SELECT RestaurantSupplierId FROM SiteRestaurantSupplier WHERE SiteId = @SiteId AND FiscalYear = @FiscalYear ORDER BY RestaurantSupplierId";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("@FiscalYear", fiscalYear));
            List<long> ids = new List<long>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ids.Add(DataManager.ToInt(row["RestaurantSupplierId"]));
            }
            return ids;
        }

        public void DeleteAllSiteRestaurantSuppliers(long siteId, int fiscalYear)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM SiteRestaurantSupplier WHERE SiteId = @SiteId AND FiscalYear = @FiscalYear",
                new SqlParameter("SiteId", siteId),
                new SqlParameter("FiscalYear", fiscalYear));
        }

        public void SaveSiteRestaurantSupplier(long siteId, long restaurantSupplierId, int fiscalYear)
        {
            string sql = @"IF NOT EXISTS(SELECT * FROM SiteRestaurantSupplier WHERE SiteId = @SiteId AND RestaurantSupplierId = @RestaurantSupplierId AND FiscalYear = @FiscalYear)
                            BEGIN
                                INSERT INTO SiteRestaurantSupplier(SiteId, RestaurantSupplierId, FiscalYear) VALUES(@SiteId, @RestaurantSupplierId, @FiscalYear)
                            END";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("@RestaurantSupplierId", restaurantSupplierId),
                new SqlParameter("@FiscalYear", fiscalYear));
        }

        #region GetAllRestaurantSuppliers
        public IQueryable<RestaurantSupplier> GetAllRestaurantSuppliers()
        {
            using (var db = GetModels())
            {
                return db.RestaurantSuppliers.Where(it => it.Active == true).OrderBy(it => it.SupplierName);
            }
        }
        #endregion

        #region GetRestaurantSupplier
        public RestaurantSupplier GetRestaurantSupplier(long id)
        {
            RestaurantSupplier obj = null;

            using (var db = GetModels())
            {
                db.RestaurantSuppliers.Where(it => it.RestaurantSupplierId == id).ToList().ForEach(it => obj = it);
            }

            return obj;
        }
        #endregion

        #region AddRestaurantSupplier
        public int AddRestaurantSupplier(RestaurantSupplier obj)
        {
            string sql = "INSERT INTO [RestaurantSupplier]([SupplierName], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Active]) VALUES(@SupplierName, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate, @Active) SELECT @@IDENTITY";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@SupplierName", obj.SupplierName),
               new SqlParameter("@CreatedBy", obj.CreatedBy),
               new SqlParameter("@CreatedDate", obj.CreatedDate),
               new SqlParameter("@UpdatedBy", obj.UpdatedBy),
               new SqlParameter("@UpdatedDate", obj.UpdatedDate),
               new SqlParameter("@Active", obj.Active)));
        }
        #endregion

        #region UpdateRestaurantSupplier
        public int UpdateRestaurantSupplier(RestaurantSupplier obj)
        {
            string sql = "UPDATE [RestaurantSupplier] SET [SupplierName] = @SupplierName, [UpdatedBy] = @UpdatedBy, [UpdatedDate] = @UpdatedDate, [Active] = @Active WHERE [RestaurantSupplierId] = @RestaurantSupplierId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@RestaurantSupplierId", obj.RestaurantSupplierId),
               new SqlParameter("@SupplierName", obj.SupplierName),
               new SqlParameter("@UpdatedBy", obj.UpdatedBy),
               new SqlParameter("@UpdatedDate", obj.UpdatedDate),
               new SqlParameter("@Active", obj.Active));
        }
        #endregion

        #region DeleteRestaurantSupplier
        public int DeleteRestaurantSupplier(int id)
        {
            string sql = "UPDATE [RestaurantSupplier] SET [Active] = 0 WHERE [RestaurantSupplierId] = @RestaurantSupplierId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@RestaurantSupplierId", id));
        }
        #endregion
        #endregion

        #region RestaurantProductCategory
        public List<RestaurantProductCategory> GetRestaurantProductCategoryDropDownListData()
        {
            List<RestaurantProductCategory> list = new List<RestaurantProductCategory>();
            string sql = @"SELECT [RestaurantProductCategoryId], [CategoryName] FROM [RestaurantProductCategory]
                            WHERE [Active] = 1 ORDER BY [CategoryName]";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new RestaurantProductCategory()
                {
                    RestaurantProductCategoryId = DataManager.ToLong(row["RestaurantProductCategoryId"]),
                    CategoryName = DataManager.ToString(row["CategoryName"])
                });
            }
            return list;
        }

        public List<long> GetSiteRestaurantProductCategoryIds(long siteId, int fiscalYear)
        {
            string sql = "SELECT RestaurantProductCategoryId FROM SiteRestaurantProductCategory WHERE SiteId = @SiteId AND FiscalYear = @FiscalYear ORDER BY RestaurantProductCategoryId";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("@FiscalYear", fiscalYear));
            List<long> ids = new List<long>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ids.Add(DataManager.ToInt(row["RestaurantProductCategoryId"]));
            }
            return ids;
        }

        public void DeleteAllSiteRestaurantProductCategories(long siteId, int fiscalYear)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM SiteRestaurantProductCategory WHERE SiteId = @SiteId AND FiscalYear = @FiscalYear",
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("FiscalYear", fiscalYear));
        }

        public void SaveSiteRestaurantProductCategory(long siteId, long restaurantProductCategoryId, int fiscalYear)
        {
            string sql = @"IF NOT EXISTS(SELECT * FROM SiteRestaurantProductCategory WHERE SiteId = @SiteId AND RestaurantProductCategoryId = @RestaurantProductCategoryId AND FiscalYear = @FiscalYear)
                            BEGIN
                                INSERT INTO SiteRestaurantProductCategory(SiteId, RestaurantProductCategoryId, FiscalYear) VALUES(@SiteId, @RestaurantProductCategoryId, @FiscalYear)
                            END";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("@RestaurantProductCategoryId", restaurantProductCategoryId),
                new SqlParameter("@FiscalYear", fiscalYear));
        }

        #region GetAllRestaurantProductCategories
        public IQueryable<RestaurantProductCategory> GetAllRestaurantProductCategories()
        {
            using (var db = GetModels())
            {
                return db.RestaurantProductCategories.Where(it => it.Active == true).OrderBy(it => it.CategoryName);
            }
        }
        #endregion

        #region GetRestaurantProductCategory
        public RestaurantProductCategory GetRestaurantProductCategory(long id)
        {
            RestaurantProductCategory obj = null;

            using (var db = GetModels())
            {
                db.RestaurantProductCategories.Where(it => it.RestaurantProductCategoryId == id).ToList().ForEach(it => obj = it);
            }

            return obj;
        }
        #endregion

        #region AddRestaurantProductCategory
        public int AddRestaurantProductCategory(RestaurantProductCategory obj)
        {
            string sql = "INSERT INTO [RestaurantProductCategory]([CategoryName], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Active]) VALUES(@CategoryName, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate, @Active) SELECT @@IDENTITY";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@CategoryName", obj.CategoryName),
               new SqlParameter("@CreatedBy", obj.CreatedBy),
               new SqlParameter("@CreatedDate", obj.CreatedDate),
               new SqlParameter("@UpdatedBy", obj.UpdatedBy),
               new SqlParameter("@UpdatedDate", obj.UpdatedDate),
               new SqlParameter("@Active", obj.Active)));
        }
        #endregion

        #region UpdateRestaurantProductCategory
        public int UpdateRestaurantProductCategory(RestaurantProductCategory obj)
        {
            string sql = "UPDATE [RestaurantProductCategory] SET [CategoryName] = @CategoryName, [UpdatedBy] = @UpdatedBy, [UpdatedDate] = @UpdatedDate, [Active] = @Active WHERE [RestaurantProductCategoryId] = @RestaurantProductCategoryId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@RestaurantProductCategoryId", obj.RestaurantProductCategoryId),
               new SqlParameter("@CategoryName", obj.CategoryName),
               new SqlParameter("@UpdatedBy", obj.UpdatedBy),
               new SqlParameter("@UpdatedDate", obj.UpdatedDate),
               new SqlParameter("@Active", obj.Active));
        }
        #endregion

        #region DeleteRestaurantProductCategory
        public int DeleteRestaurantProductCategory(int id)
        {
            string sql = "UPDATE [RestaurantProductCategory] SET [Active] = 0 WHERE [RestaurantProductCategoryId] = @RestaurantProductCategoryId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@RestaurantProductCategoryId", id));
        }
        #endregion
        #endregion

        #region Interest
        //#region GetAllInterests
        //public List<Interest> GetAllInterests()
        //{
        //    List<Interest> list = new List<Interest>();
        //    string sql = "SELECT * FROM [Interest] JOIN InterestLang ON InterestLang.InterestId = Interest.InterestId WHERE [Active] = 1 ORDER BY Interest.InterestId DESC";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Interest(row));
        //    }
        //    return list;
        //}

        //public List<Interest> GetAllInterests(jQueryDataTableParamModel param, int langId = 1)
        //{
        //    List<Interest> list = new List<Interest>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectInterestsList",
        //        new SqlParameter("@LangId", langId));

        //    Interest obj;
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        obj = new Interest(row);
        //        obj.InterestName = DataManager.ToString(row["InterestName"]);
        //        obj.InterestDesc = DataManager.ToString(row["InterestDesc"]);
        //        obj.InterestLangs = new List<InterestLang>();
        //        obj.InterestLangs.Add(new InterestLang()
        //        {
        //            InterestName = DataManager.ToString(row["InterestName"]),
        //            InterestDesc = DataManager.ToString(row["InterestDesc"])
        //        });
        //        obj.CategoryName = DataManager.ToString(row["CategoryName"]);
        //        list.Add(obj);
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetInterest
        //public Interest GetInterest(long id)
        //{
        //    Interest obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectInterest",
        //        new SqlParameter("@InterestId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new Interest(ds.Tables[0].Rows[0]);
        //        obj.InterestLangs = new List<InterestLang>();
        //        if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
        //        {
        //            foreach (DataRow row in ds.Tables[1].Rows)
        //            {
        //                obj.InterestLangs.Add(new InterestLang(row));
        //            }
        //        }
        //    }

        //    return obj;
        //}
        //#endregion

        //#region AddInterest
        //public int AddInterest(Interest obj)
        //{
        //    string sql = "INSERT INTO [Interest]([CategoryId], [Active]) VALUES(@CategoryId, @Active) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@CategoryId", obj.CategoryId),
        //       new SqlParameter("@Active", obj.Active)));
        //}
        //#endregion

        //#region UpdateInterest
        //public int UpdateInterest(Interest obj)
        //{
        //    string sql = "UPDATE [Interest] SET [CategoryId] = @CategoryId, [Active] = @Active WHERE InterestId = @InterestId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@InterestId", obj.InterestId),
        //       new SqlParameter("@CategoryId", obj.CategoryId),
        //       new SqlParameter("@Active", obj.Active));
        //}
        //#endregion

        //#region DeleteInterest
        //public int DeleteInterest(int id)
        //{
        //    string sql = "UPDATE [Interest] SET [Active] = 0 WHERE [InterestId] = @InterestId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@InterestId", id));
        //}
        //#endregion
        #endregion

        #region InterestLang
        //public InterestLang GetInterestLang(int InterestId, int langId)
        //{
        //    InterestLang obj = null;
        //    string sql = "SELECT * FROM [InterestLang] WHERE [InterestId] = " + InterestId + " AND [LangId] = " + langId;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new InterestLang(ds.Tables[0].Rows[0]);
        //    }
        //    else
        //    {
        //        obj = new InterestLang();
        //        obj.LangId = langId;
        //        obj.InterestId = InterestId;
        //    }
        //    return obj;
        //}

        //public int DeleteInterestLangByInterestId(int InterestId)
        //{
        //    string sql = "DELETE FROM [InterestLang] WHERE [InterestId] = " + InterestId;
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}

        //public int SaveInterestLang(InterestLang obj)
        //{
        //    string sql = @"IF(EXISTS(SELECT [InterestId] FROM [InterestLang] WHERE [InterestId] = @InterestId AND [LangId] = @LangId))
        //                    BEGIN
        //                        UPDATE [InterestLang] SET [InterestName] = @InterestName, [InterestDesc] = @InterestDesc WHERE [InterestId] = @InterestId AND [LangId] = @LangId;
        //                    END
        //                    ELSE
        //                    BEGIN
        //                        INSERT INTO [InterestLang]([InterestId], [LangId], [InterestName], [InterestDesc])
        //                        VALUES(@InterestId, @LangId, @InterestName, @InterestDesc) SELECT @@IDENTITY;
        //                    END";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@InterestId", obj.InterestId),
        //        new SqlParameter("@LangId", obj.LangId),
        //        new SqlParameter("@InterestName", obj.InterestName),
        //        new SqlParameter("@InterestDesc", obj.InterestDesc)), -1);
        //}
        #endregion

        #region User Interested

        #endregion

        #region Item
        //#region GetAllItems
        //public List<Item> GetAllItems()
        //{
        //    List<Item> list = new List<Item>();
        //    string sql = "SELECT * FROM [Item] ORDER BY [ItemCode]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Item(row));
        //    }

        //    return list;
        //}
        //public List<Item> GetAllItems(jQueryDataTableParamModel param, ItemType.Type itemType, long? siteId = null, int langId = 1)
        //{
        //    List<Item> list = new List<Item>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItemsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@ItemTypeId", (int)itemType),
        //        new SqlParameter("@LangId", langId),
        //        new SqlParameter("@SideId", siteId));

        //    Item item = null;
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        item = new Item(row);
        //        item.ItemName = DataManager.ToString(row["ItemName"]);
        //        item.CategoryName = DataManager.ToString(row["CategoryName"]);
        //        item.SiteName = DataManager.ToString(row["SiteName"]);
        //        list.Add(item);
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //public List<Item> GetAllGreenFees(jQueryDataTableParamModel param, long? siteId = null, int langId = 1)
        //{
        //    List<Item> list = new List<Item>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItemsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@ItemTypeId", (int)ItemType.Type.Product),
        //        new SqlParameter("@LangId", langId),
        //        new SqlParameter("@SiteId", siteId));

        //    Item item = null;
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        item = new Item(row);
        //        item.ItemName = DataManager.ToString(row["ItemName"]);
        //        item.CategoryName = DataManager.ToString(row["CategoryName"]);
        //        item.SiteName = DataManager.ToString(row["SiteName"]);
        //        list.Add(item);
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //public List<Item> GetAllStayPackages(jQueryDataTableParamModel param, long? siteId = null, int langId = 1)
        //{
        //    List<Item> list = new List<Item>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItemsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@ItemTypeId", (int)ItemType.Type.Product),
        //        new SqlParameter("@LangId", langId),
        //        new SqlParameter("@SiteId", siteId));

        //    Item item = null;
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        item = new Item(row);
        //        item.ItemName = DataManager.ToString(row["ItemName"]);
        //        item.SiteName = DataManager.ToString(row["SiteName"]);
        //        list.Add(item);
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //public List<Item> GetAllGolfLessons(jQueryDataTableParamModel param, long? siteId = null, int langId = 1)
        //{
        //    List<Item> list = new List<Item>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItemsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@ItemTypeId", (int)ItemType.Type.Product),
        //        new SqlParameter("@LangId", langId),
        //        new SqlParameter("@SiteId", siteId));

        //    Item item = null;
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        item = new Item(row);
        //        item.ItemName = DataManager.ToString(row["ItemName"]);
        //        item.SiteName = DataManager.ToString(row["SiteName"]);
        //        list.Add(item);
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //public List<Item> GetAllDrivingRanges(jQueryDataTableParamModel param, long? siteId = null, int langId = 1)
        //{
        //    List<Item> list = new List<Item>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItemsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@ItemTypeId", (int)ItemType.Type.Product),
        //        new SqlParameter("@LangId", langId),
        //        new SqlParameter("@SiteId", siteId));

        //    Item item = null;
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        item = new Item(row);
        //        item.ItemName = DataManager.ToString(row["ItemName"]);
        //        item.SiteName = DataManager.ToString(row["SiteName"]);
        //        list.Add(item);
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //#endregion

        //#region GetItem
        //public Item GetItem(long id, int langId = 1)
        //{
        //    Item obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItem",
        //        new SqlParameter("@ItemId", id),
        //        new SqlParameter("@LangId", langId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        DataRow row = ds.Tables[0].Rows[0];
        //        obj = new Item(row);
        //        obj.SiteName = DataManager.ToString(row["SiteName"]);
        //        //obj.SiteSlug = DataManager.ToString(row["SiteSlug"]);
        //        obj.StateName = DataManager.ToString(row["StateName"]);
        //        obj.AlbatrosCourseId = DataManager.ToInt(row["AlbatrosCourseId"]);
        //        obj.AlbatrosClubId = DataManager.ToLong(row["AlbatrosClubId"]);
        //        obj.SpecialPrice = DataManager.ToDecimal(row["SpecialPrice"]);
        //        obj.TaxRate = DataManager.ToFloat(row["TaxRate"]);
        //        //obj.ItemMinDate = DataManager.ToDateTime(row["ItemMaxDate"]);
        //        obj.ItemMaxDate = DataManager.ToDateTime(row["ItemMaxDate"]);
        //        obj.ItemImages = new List<ItemImage>();
        //        obj.ItemImages.Add(new ItemImage(row));
        //        obj.ItemLangs = new List<ItemLang>();
        //        obj.SiteLangs = new List<SiteLang>();

        //        if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
        //        {
        //            foreach (DataRow dr in ds.Tables[1].Rows)
        //            {
        //                obj.ItemLangs.Add(new ItemLang(dr));
        //            }
        //            obj.ItemLangs.Where(it => it.LangId == langId).ToList().ForEach(it =>
        //            {
        //                obj.ItemName = it.ItemName;
        //                obj.InvoiceName = it.InvoiceName;
        //            });
        //        }

        //        if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
        //        {
        //            foreach (DataRow dr in ds.Tables[2].Rows)
        //            {
        //                obj.SiteLangs.Add(new SiteLang(dr));
        //            }
        //            obj.SiteLangs.Where(it => it.LangId == langId).ToList().ForEach(it =>
        //            {
        //                obj.SiteName = it.SiteName;
        //            });
        //        }

        //        if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
        //        {
        //            obj.AverageRating = DataManager.ToByte(ds.Tables[3].Rows[0]["AverageRating"]);
        //        }

        //        obj.ItemReviews = new List<ItemReview>();
        //        if (ds.Tables.Count > 4 && ds.Tables[4].Rows.Count > 0)
        //        {
        //            foreach (DataRow dr in ds.Tables[4].Rows)
        //            {
        //                obj.ItemReviews.Add(new ItemReview(dr));
        //            }
        //        }

        //        obj.Course = new Course();
        //        if (ds.Tables.Count > 5 && ds.Tables[5].Rows.Count > 0)
        //        {
        //            obj.Course = new Course(ds.Tables[5].Rows[0]);
        //        }
        //        else
        //        {
        //            obj.Course = new Course();
        //        }

        //        obj.ItemPrices = new List<ItemPrice>();
        //        if (ds.Tables.Count > 6 && ds.Tables[6].Rows.Count > 0)
        //        {
        //            foreach (DataRow dr in ds.Tables[6].Rows)
        //            {
        //                obj.ItemPrices.Add(new ItemPrice(dr));
        //            }
        //        }
        //    }

        //    return obj;
        //}
        //#endregion

        //#region GetItemsLatLng
        //public DataSet GetItemsLatLng(int langId = 1, int? countryId = null, int? regionId = null, int? stateId = null, long? siteId = null)
        //{
        //    string sql = @"SELECT [Item].[ItemId], [Item].[ItemSlug], [Site].[Latitude], [Site].[Longitude], [Item].[ItemTypeId], [ItemLang].[ItemName], [ItemLang].[ItemShortDesc], [SiteLang].[SiteName], [SiteLang].[Description] AS SiteDescription, [Site].[SiteSlug]
        //                FROM [Item] 
        //                LEFT OUTER JOIN [Site] ON [Site].[SiteId] = [Item].[SiteId] AND [Site].[Visible] = 1 AND [Site].[Active] = 1
        //                LEFT OUTER JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] AND [SiteLang].[LangId] = @LangId
        //                LEFT OUTER JOIN [State] ON [State].[StateId] = [Site].[StateId]
        //                LEFT OUTER JOIN [Region] ON [Region].[RegionId] = [State].[RegionId]
        //                LEFT OUTER JOIN [Country] ON [Country].[CountryId] = [Region].[CountryId]
        //                LEFT OUTER JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = @LangId 
        //                WHERE [Item].[Active] = 1 ";
        //    if (siteId.HasValue && siteId.Value > 0)
        //    {
        //        sql += " AND [Item].[SiteId] = " + siteId.Value;
        //    }
        //    else if (stateId.HasValue && stateId.Value > 0)
        //    {
        //        sql += " AND [Site].[StateId] = " + stateId.Value;
        //    }
        //    else if (regionId.HasValue && regionId.Value > 0)
        //    {
        //        sql += " AND [State].[RegionId] = " + regionId.Value;
        //    }
        //    else if (countryId.HasValue && countryId.Value > 0)
        //    {
        //        sql += " AND [Region].[CountryId] = " + countryId.Value;
        //    }
        //    sql += " ORDER BY [Item].[ItemId]";
        //    return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@LangId", langId));
        //}
        //#endregion

        #region AddItem
        public int AddItem(Item obj)
        {
            string sql = @"INSERT INTO [Item](
                            [CategoryId]
                            ,[ItemTypeId]
                            ,[ItemCode]
                            ,[ItemSlug]
                            ,[Shape]
                            ,[Genre]
                            ,[Dexterity]
                            ,[Shaft]
                            ,[InsertDate]
                            ,[UpdateDate]
                            ,[Active]
                            ,[UserId]
                            ,[SiteId]
                            ,[CourseId]
                            ,[Price]
                            ,[OldPrice]
                            ,[ProductCost]
                            ,[IsAllowReview]
                            ,[IsPublish]
                            ,[PublishStartDate]
                            ,[PublishEndDate]
                            ,[TaxId]
                            ,[UnitInStock]
                            ,[IsShowOnHomepage]
                            ,[IncludePractice]
                            ,[IncludeAccommodation]
                            ,[AvailableStartDate]
                            ,[AvailableEndDate]
                            ,[SupplierId]
                            ,[ShippingCost]
                            ,[ShippingTimeMin]
                            ,[ShippingTimeMax]
                            ,[CanReturn]
                            ,[RefundWithIn]
                            ,[ShippingTypeId]
                            ,[BrandId]
                            ,[Size]
                            ,[PromotionStatusId]
                            ,[IsUserCanSelectDate]) 

                            VALUES(
                            @CategoryId
                            ,@ItemTypeId
                            ,@ItemCode
                            ,@ItemSlug
                            ,@Shape
                            ,@Genre
                            ,@Dexterity
                            ,@Shaft
                            ,@InsertDate
                            ,@UpdateDate
                            ,@Active
                            ,@UserId
                            ,@SiteId
                            ,@CourseId
                            ,@Price
                            ,@OldPrice
                            ,@ProductCost
                            ,@IsAllowReview
                            ,@IsPublish
                            ,@PublishStartDate
                            ,@PublishEndDate
                            ,@TaxId
                            ,@UnitInStock
                            ,@IsShowOnHomepage
                            ,@IncludePractice
                            ,@IncludeAccommodation
                            ,@AvailableStartDate
                            ,@AvailableEndDate
                            ,@SupplierId
                            ,@ShippingCost
                            ,@ShippingTimeMin
                            ,@ShippingTimeMax
                            ,@CanReturn
                            ,@RefundWithIn
                            ,@ShippingTypeId
                            ,@BrandId
                            ,@Size
                            ,@PromotionStatusId
                            ,@IsUserCanSelectDate) 
                            SELECT @@IDENTITY";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@CategoryId", obj.CategoryId),
               new SqlParameter("@ItemCode", obj.ItemCode),
               new SqlParameter("@ItemTypeId", obj.ItemTypeId),
               new SqlParameter("@ItemSlug", obj.ItemSlug),
               new SqlParameter("@Shape", obj.Shape),
               new SqlParameter("@Genre", obj.Genre),
               new SqlParameter("@Dexterity", obj.Dexterity),
               new SqlParameter("@Shaft", obj.Shaft),
               new SqlParameter("@InsertDate", obj.InsertDate),
               new SqlParameter("@UpdateDate", obj.UpdateDate),
               new SqlParameter("@Active", obj.Active),
               new SqlParameter("@UserId", obj.UserId),
               new SqlParameter("@SiteId", obj.SiteId),
               new SqlParameter("@CourseId", obj.CourseId),
               new SqlParameter("@Price", obj.Price),
               new SqlParameter("@OldPrice", obj.OldPrice),
               new SqlParameter("@ProductCost", obj.ProductCost),
               new SqlParameter("@SpecialPrice", obj.SpecialPrice),
               new SqlParameter("@SpecialPriceStartDate", obj.SpecialPriceStartDate),
               new SqlParameter("@SpecialPriceEndDate", obj.SpecialPriceEndDate),
               new SqlParameter("@IsAllowReview", obj.IsAllowReview),
               new SqlParameter("@IsPublish", obj.IsPublish),
               new SqlParameter("@PublishStartDate", obj.PublishStartDate),
               new SqlParameter("@PublishEndDate", obj.PublishEndDate),
               new SqlParameter("@TaxId", obj.TaxId),
               new SqlParameter("@UnitInStock", obj.UnitInStock),
               new SqlParameter("@IsShowOnHomepage", obj.IsShowOnHomepage),
               new SqlParameter("@IncludePractice", obj.IncludePractice),
               new SqlParameter("@IncludeAccommodation", obj.IncludeAccommodation),
               new SqlParameter("@AvailableStartDate", obj.AvailableStartDate),
               new SqlParameter("@AvailableEndDate", obj.AvailableEndDate),
               new SqlParameter("@SupplierId", obj.SupplierId),
               new SqlParameter("@ShippingCost", obj.ShippingCost),
               new SqlParameter("@ShippingTimeMin", obj.ShippingTimeMin),
               new SqlParameter("@ShippingTimeMax", obj.ShippingTimeMax),
               new SqlParameter("@CanReturn", obj.CanReturn),
               new SqlParameter("@RefundWithIn", obj.RefundWithIn),
               new SqlParameter("@ShippingTypeId", obj.ShippingTypeId),
               new SqlParameter("@BrandId", obj.BrandId),
               new SqlParameter("@Size", obj.Size),
               new SqlParameter("@PromotionStatusId", obj.PromotionStatusId),
               new SqlParameter("@IsUserCanSelectDate", obj.IsUserCanSelectDate)));
        }
        #endregion

        #region UpdateItem
        public int UpdateItem(Item obj)
        {
            string sql = @"UPDATE [Item] SET
                          [CategoryId] = @CategoryId
                          ,[ItemTypeId] = @ItemTypeId
                          ,[ItemCode] = @ItemCode
                          ,[ItemSlug] = @ItemSlug
                          ,[Shape] = @Shape
                          ,[Genre] = @Genre
                          ,[Dexterity] = @Dexterity
                          ,[Shaft] = @Shaft
                          ,[UpdateDate] = @UpdateDate
                          ,[Active] = @Active
                          ,[UserId] = @UserId
                          ,[SiteId] = @SiteId
                          ,[CourseId] = @CourseId
                          ,[Price] = @Price
                          ,[OldPrice] = @OldPrice
                          ,[ProductCost] = @ProductCost
                          ,[IsAllowReview] = @IsAllowReview
                          ,[IsPublish] = @IsPublish
                          ,[PublishStartDate] = @PublishStartDate
                          ,[PublishEndDate] = @PublishEndDate
                          ,[TaxId] = @TaxId
                          ,[UnitInStock] = @UnitInStock
                          ,[IsShowOnHomepage] = @IsShowOnHomepage
                          ,[IncludePractice] = @IncludePractice
                          ,[IncludeAccommodation] = @IncludeAccommodation
                          ,[AvailableStartDate] = @AvailableStartDate
                          ,[AvailableEndDate] = @AvailableEndDate
                          ,[SupplierId] = @SupplierId
                          ,[ShippingCost] = @ShippingCost
                          ,[ShippingTimeMin] = @ShippingTimeMin
                          ,[ShippingTimeMax] = @ShippingTimeMax
                          ,[CanReturn] = @CanReturn
                          ,[RefundWithIn] = @RefundWithIn
                          ,[ShippingTypeId] = @ShippingTypeId
                          ,[BrandId] = @BrandId
                          ,[Size] = @Size
                          ,[PromotionStatusId] = @PromotionStatusId
                          ,[IsUserCanSelectDate] = @IsUserCanSelectDate
                            WHERE [ItemId] = @ItemId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@ItemId", obj.ItemId),
               new SqlParameter("@CategoryId", obj.CategoryId),
               new SqlParameter("@ItemTypeId", obj.ItemTypeId),
               new SqlParameter("@ItemCode", obj.ItemCode),
               new SqlParameter("@ItemSlug", obj.ItemSlug),
               new SqlParameter("@Shape", obj.Shape),
               new SqlParameter("@Genre", obj.Genre),
               new SqlParameter("@Dexterity", obj.Dexterity),
               new SqlParameter("@Shaft", obj.Shaft),
               new SqlParameter("@UpdateDate", obj.UpdateDate),
               new SqlParameter("@Active", obj.Active),
               new SqlParameter("@UserId", obj.UserId),
               new SqlParameter("@SiteId", obj.SiteId),
               new SqlParameter("@CourseId", obj.CourseId),
               new SqlParameter("@Price", obj.Price),
               new SqlParameter("@OldPrice", obj.OldPrice),
               new SqlParameter("@ProductCost", obj.ProductCost),
               new SqlParameter("@SpecialPrice", obj.SpecialPrice),
               new SqlParameter("@SpecialPriceStartDate", obj.SpecialPriceStartDate),
               new SqlParameter("@SpecialPriceEndDate", obj.SpecialPriceEndDate),
               new SqlParameter("@IsAllowReview", obj.IsAllowReview),
               new SqlParameter("@IsPublish", obj.IsPublish),
               new SqlParameter("@PublishStartDate", obj.PublishStartDate),
               new SqlParameter("@PublishEndDate", obj.PublishEndDate),
               new SqlParameter("@TaxId", obj.TaxId),
               new SqlParameter("@UnitInStock", obj.UnitInStock),
               new SqlParameter("@IsShowOnHomepage", obj.IsShowOnHomepage),
               new SqlParameter("@IncludePractice", obj.IncludePractice),
               new SqlParameter("@IncludeAccommodation", obj.IncludeAccommodation),
               new SqlParameter("@AvailableStartDate", obj.AvailableStartDate),
               new SqlParameter("@AvailableEndDate", obj.AvailableEndDate),
               new SqlParameter("@SupplierId", obj.SupplierId),
               new SqlParameter("@ShippingCost", obj.ShippingCost),
               new SqlParameter("@ShippingTimeMin", obj.ShippingTimeMin),
               new SqlParameter("@ShippingTimeMax", obj.ShippingTimeMax),
               new SqlParameter("@CanReturn", obj.CanReturn),
               new SqlParameter("@RefundWithIn", obj.RefundWithIn),
               new SqlParameter("@ShippingTypeId", obj.ShippingTypeId),
               new SqlParameter("@BrandId", obj.BrandId),
               new SqlParameter("@Size", obj.Size),
               new SqlParameter("@PromotionStatusId", obj.PromotionStatusId),
               new SqlParameter("@IsUserCanSelectDate", obj.IsUserCanSelectDate));
        }
        #endregion

        //#region DeleteItem
        //public int DeleteItem(int id)
        //{
        //    string sql = "UPDATE [Item] SET [Active] = 0 WHERE [ItemId] = @ItemId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemId", id));
        //}
        //#endregion

        //#region GetAllItemAndTypes
        //public DataSet GetAllItemAndTypes()
        //{
        //    string sql = "SELECT [ItemId], [ItemTypeId], [SiteId] FROM [Item] WHERE [Active] = 1";
        //    return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //}
        //#endregion

        //#region GetItemRunningNumber
        //public string GetItemRunningNumber(int itemTypeId, int digit = 6, int? itemCategoryId = null)
        //{
        //    if (digit < 1)
        //        digit = 1;

        //    string sql = "SELECT COUNT([ItemId]) FROM [Item] WHERE [ItemTypeId] = " + itemTypeId + " AND [Active] = 1";
        //    int latestNumber = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql));
        //    sql = "SELECT TOP(1) [Prefix] FROM [ItemCategory] WHERE [ItemTypeId] = " + itemTypeId;
        //    if (itemCategoryId.HasValue && itemCategoryId.Value > 0)
        //    {
        //        sql += " AND [CategoryId] = " + itemCategoryId.Value;
        //    }
        //    sql += " ORDER BY [CategoryName]";
        //    string prefix = DataManager.ToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql));
        //    string digitCode = string.Empty;
        //    for (int i = 0; i < digit; i++)
        //    {
        //        digitCode += "0";
        //    }

        //    return string.Format("{0}{1:" + digitCode + "}", prefix, latestNumber);
        //}
        //#endregion

        #region GetPaymentGatewayByItemId
        public bool GetPaymentGatewayByItemId(long itemId, out string vendorToken, out string privateKey)
        {
            string sql = @"SELECT [Site].[LydiaVendorToken], [Site].[LydiaVendorId] FROM [Site]
                           JOIN [Item] ON [Item].[SiteId] = [Site].[SiteId]
                           WHERE [Item].[ItemId] = @ItemId";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@ItemId", itemId));

            if (ds.Tables[0].Rows.Count > 0)
            {
                vendorToken = DataManager.ToString(ds.Tables[0].Rows[0]["LydiaVendorToken"]);
                privateKey = DataManager.ToString(ds.Tables[0].Rows[0]["LydiaVendorId"]);
                return true;
            }
            else
            {
                vendorToken = string.Empty;
                privateKey = string.Empty;
                return false;
            }
        }
        #endregion

        #endregion

        #region ItemType
        //#region GetAllItemType
        //public List<ItemType> GetAllItemType()
        //{
        //    List<ItemType> list = new List<ItemType>();
        //    string sql = "SELECT * FROM [ItemType] ORDER BY [ItemTypeId]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemType(row));
        //    }
        //    return list;
        //}
        //public List<ItemType> GetAllItemType(jQueryDataTableParamModel param, int langId = 1)
        //{
        //    List<ItemType> list = new List<ItemType>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItemTypeList",
        //        new SqlParameter("@LangId", langId));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemType(row));
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetItemType
        //public ItemType GetItemType(long id)
        //{
        //    ItemType obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItemType",
        //        new SqlParameter("@ItemTypeId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new ItemType(ds.Tables[0].Rows[0]);
        //    }

        //    return obj;
        //}
        //#endregion

        //#region AddItemType
        //public int AddItemType(ItemType obj)
        //{
        //    string sql = "INSERT INTO [ItemType](ItemTypeName) VALUES(@ItemTypeName) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@ItemTypeName", obj.ItemTypeName)));
        //}
        //#endregion

        //#region UpdateItemType
        //public int UpdateItemType(ItemType obj)
        //{
        //    string sql = "UPDATE [ItemType] SET [ItemTypeName] = @ItemTypeName WHERE ItemTypeId = @ItemTypeId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@ItemTypeId", obj.ItemTypeId),
        //       new SqlParameter("@ItemTypeName", obj.ItemTypeName));
        //}
        //#endregion

        //#region DeleteItemType
        //public int DeleteItemType(int id)
        //{
        //    string sql = "DELETE FROM [ItemType] WHERE [CourseTypeId] = @ItemTypeId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemTypeId", id));
        //}
        //#endregion
        #endregion

        #region ItemCategory
        //#region GetAllItemCategorys
        //public List<ItemCategory> GetAllItemCategories(int itemTypeId = 0)
        //{
        //    List<ItemCategory> list = new List<ItemCategory>();
        //    string sql = "SELECT * FROM [ItemCategory] WHERE [Active] = 1";
        //    if (itemTypeId > 0)
        //    {
        //        sql += " AND [ItemTypeId] = " + itemTypeId;
        //    }
        //    sql += " ORDER BY [CategoryName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemCategory(row));
        //    }
        //    return list;
        //}
        //public List<ItemCategory> GetAllItemCategories(jQueryDataTableParamModel param, long? siteId)
        //{
        //    List<ItemCategory> list = new List<ItemCategory>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItemCategoriesList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@SiteId", siteId));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemCategory(row));
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //public List<ItemCategory> GetAllGreenFeeCategories(jQueryDataTableParamModel param, long? siteId)
        //{
        //    List<ItemCategory> list = new List<ItemCategory>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectGreenFeeCategoriesList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@SiteId", siteId));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemCategory(row));
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //public List<ItemCategory> GetAllGolfLessonCategories(jQueryDataTableParamModel param, long? siteId)
        //{
        //    List<ItemCategory> list = new List<ItemCategory>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectGolfLessonCategoriesList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@SiteId", siteId));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemCategory(row));
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //#endregion

        //#region GetItemCategoriesDropDownList
        //public List<ItemCategory> GetItemCategoriesDropDownList(int? itemTypeId = null, int? parent = null)
        //{
        //    List<ItemCategory> list = new List<ItemCategory>();
        //    string sql = @"SELECT [CategoryId], [CategoryName] FROM [ItemCategory]
        //                    WHERE [Active] = 1";

        //    if (parent.HasValue && parent.Value > 0)
        //    {
        //        sql += " AND [ParentCategoryId] = " + parent.Value;
        //    }
        //    else
        //    {
        //        sql += " AND ISNULL([ParentCategoryId], 0) = 0";
        //    }
        //    if (itemTypeId.HasValue)
        //    {
        //        sql += " AND [ItemTypeId] = " + itemTypeId.Value;
        //    }
        //    sql += " ORDER BY [CategoryName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemCategory()
        //        {
        //            CategoryId = DataManager.ToInt(row["CategoryId"]),
        //            CategoryName = DataManager.ToString(row["CategoryName"])
        //        });
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetItemcategoryByItemTypeId
        //public List<ItemCategory> GetItemcategoryByItemTypeId(int itemTypeId)
        //{
        //    List<ItemCategory> list = new List<ItemCategory>();
        //    string sql = @"SELECT [CategoryId], [CategoryName], [ParentCategoryId] FROM [ItemCategory]
        //                    WHERE [Active] = 1 AND [ItemTypeId] = " + itemTypeId;

        //    sql += " ORDER BY [CategoryName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemCategory()
        //        {
        //            CategoryId = DataManager.ToInt(row["CategoryId"]),
        //            CategoryName = DataManager.ToString(row["CategoryName"]),
        //            ParentCategoryId = DataManager.ToInt(row["ParentCategoryId"])
        //        });
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetItemSubCategoriesDropDownList
        //public List<ItemCategory> GetAllItemCategoriesDropDownList(int? itemTypeId = null)
        //{
        //    List<ItemCategory> list = new List<ItemCategory>();
        //    string sql = @"SELECT [CategoryId], [CategoryName] FROM [ItemCategory]
        //                    WHERE [Active] = 1 AND ISNULL([ParentCategoryId], 0) <> 0";
        //    if (itemTypeId.HasValue)
        //    {
        //        sql += " AND [ItemTypeId] = " + itemTypeId.Value;
        //    }
        //    sql += " ORDER BY [CategoryName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemCategory()
        //        {
        //            CategoryId = DataManager.ToInt(row["CategoryId"]),
        //            CategoryName = DataManager.ToString(row["CategoryName"])
        //        });
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetItemCategory
        //public ItemCategory GetItemCategory(long id)
        //{
        //    ItemCategory obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItemCategory",
        //        new SqlParameter("@CategoryId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new ItemCategory(ds.Tables[0].Rows[0]);
        //    }

        //    return obj;
        //}
        //#endregion

        //#region AddItemCategory
        //public int AddItemCategory(ItemCategory obj)
        //{
        //    string sql = "INSERT INTO [ItemCategory](CategoryName, Prefix, ItemTypeId, InsertDate, UpdateDate, Active, UserId, ParentCategoryId, SiteId) VALUES(@CategoryName, @Prefix, @ItemTypeId, @InsertDate, @UpdateDate, @Active, @UserId, @ParentCategoryId, @SiteId) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@CategoryName", obj.CategoryName),
        //       new SqlParameter("@Prefix", obj.Prefix),
        //       new SqlParameter("@ItemTypeId", obj.ItemTypeId),
        //       new SqlParameter("@InsertDate", obj.InsertDate),
        //       new SqlParameter("@UpdateDate", obj.UpdateDate),
        //       new SqlParameter("@Active", obj.Active),
        //       new SqlParameter("@UserId", obj.UserId),
        //       new SqlParameter("@ParentCategoryId", obj.ParentCategoryId),
        //       new SqlParameter("@SiteId", obj.SiteId)));
        //}
        //#endregion

        //#region UpdateItemCategory
        //public int UpdateItemCategory(ItemCategory obj)
        //{
        //    string sql = "UPDATE [ItemCategory] SET [CategoryName] = @CategoryName, [Prefix] = @Prefix, ItemTypeId = @ItemTypeId, InsertDate = @InsertDate, UpdateDate = @UpdateDate, Active = @Active, UserId = @UserId, ParentCategoryId = @ParentCategoryId, SiteId = @SiteId WHERE [CategoryId] = @CategoryId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@CategoryId", obj.CategoryId),
        //       new SqlParameter("@CategoryName", obj.CategoryName),
        //       new SqlParameter("@Prefix", obj.Prefix),
        //       new SqlParameter("@ItemTypeId", obj.ItemTypeId),
        //       new SqlParameter("@InsertDate", obj.InsertDate),
        //       new SqlParameter("@UpdateDate", obj.UpdateDate),
        //       new SqlParameter("@Active", obj.Active),
        //       new SqlParameter("@UserId", obj.UserId),
        //       new SqlParameter("@ParentCategoryId", obj.ParentCategoryId),
        //       new SqlParameter("@SiteId", obj.SiteId));
        //}
        //#endregion

        //#region DeleteItemCategory
        //public int DeleteItemCategory(int id)
        //{
        //    string sql = "UPDATE [ItemCategory] SET [Active] = 0 WHERE [CategoryId] = @CategoryId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@CategoryId", id));
        //}
        //#endregion

        //#region GetItemCategoriesKeyValuePairs
        //public Dictionary<int, string> GetItemCategoriesKeyValuePairs(int itemTypeId)
        //{
        //    Dictionary<int, string> list = new Dictionary<int, string>();
        //    string sql = "SELECT * FROM [ItemCategory] WHERE [ItemTypeId] = " + itemTypeId;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(DataManager.ToInt(row["CategoryId"]), DataManager.ToString(row["CategoryName"]));
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetSubItemCategoriesByItemCategoryId
        //public List<ItemCategory> GetSubItemCategoriesByItemCategoryId(int? itemTypeId = null, int? parent = 0)
        //{
        //    List<ItemCategory> list = new List<ItemCategory>();
        //    string sql = @"SELECT [CategoryId], [CategoryName] FROM [ItemCategory]
        //                    WHERE [Active] = 1 AND [ParentCategoryId] = " + (parent.HasValue ? parent.Value : 0);
        //    if (itemTypeId.HasValue)
        //    {
        //        sql += " AND [ItemTypeId] = " + itemTypeId.Value;
        //    }
        //    sql += " ORDER BY [CategoryName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemCategory()
        //        {
        //            CategoryId = DataManager.ToInt(row["CategoryId"]),
        //            CategoryName = DataManager.ToString(row["CategoryName"])
        //        });
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetItemCategoryIdByName
        //public int GetItemCategoryIdByName(string name, int itemType = 0)
        //{
        //    string sql = "SELECT TOP(1) [CategoryId] FROM [ItemCategory] WHERE [CategoryName] LIKE '%" + name + "%'";
        //    if (itemType > 0)
        //    {
        //        sql += " AND [ItemTypeId] = " + itemType;
        //    }
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql), 0);
        //}
        //#endregion

        #endregion

        #region ItemPrice

        public long DeleteItemPriceByItemId(long id)
        {
            string sql = "DELETE FROM [ItemPrice] WHERE [ItemId] = " + id;
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        }

        #region SaveItemPrice
        public int SaveItemPrice(ItemPrice obj)
        {
            string sql = @"IF(EXISTS(SELECT [ItemId] FROM [ItemPrice] WHERE [ItemId] = @ItemId AND [StartDate] = @StartDate AND [EndDate] = @EndDate AND [PriceType] = @PriceType))
                            BEGIN
                                UPDATE [ItemPrice] SET [ItemId] = @ItemId,[StartDate] = @StartDate,[EndDate] = @EndDate,[Price] = @Price,[InsertDate] = @InsertDate,[UpdateDate] = @UpdateDate,[Active] = @Active,[PriceType] = @PriceType WHERE [ItemId] = @ItemId AND [StartDate] = @StartDate AND [EndDate] = @EndDate AND [PriceType] = @PriceType
                            END
                            ELSE
                            BEGIN
                                INSERT INTO [ItemPrice]([ItemId],[StartDate],[EndDate],[Price],[InsertDate],[UpdateDate],[Active],[PriceType])
                                VALUES(@ItemId,@StartDate,@EndDate,@Price,@InsertDate,@UpdateDate,@Active,@PriceType) SELECT @@IDENTITY;
                            END";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@ItemId", obj.ItemId),
                new SqlParameter("@StartDate", obj.StartDate),
                new SqlParameter("@EndDate", obj.EndDate),
                new SqlParameter("@Price", obj.Price),
                new SqlParameter("@InsertDate", obj.InsertDate),
                new SqlParameter("@UpdateDate", obj.UpdateDate),
                new SqlParameter("@Active", obj.Active),
                new SqlParameter("@PriceType", obj.PriceType));
        }
        #endregion

        #endregion

        #region ItemTrackingCode
        public string GetItemConversionTrackingCode(long itemId, string type)
        {
            string columnName = "TrackingCode";
            switch (type.ToLower())
            {
                case "addtocart":
                case "cart":
                    columnName += "AddToCart";
                    break;
                case "initialcheckout":
                case "checkout":
                    columnName += "InitialCheckout";
                    break;
                case "purchase":
                    columnName += "Purchase";
                    break;
                default:
                    columnName += "ViewContent";
                    break;
            }

            string sql = "SELECT " + columnName + " FROM [ItemConversionTracking] WHERE [ItemId] = @ItemId";
            return DataManager.ToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@ItemId", itemId)));
        }

        public void SaveItemConversionTrackingCode(ItemConversionTracking obj)
        {
            string sql = @"IF EXISTS(SELECT [ItemId] FROM [ItemConversionTracking] WHERE [ItemId] = @ItemId)
                        BEGIN
                            UPDATE [ItemConversionTracking] SET [TrackingCodeViewContent] = @TrackingCodeViewContent, [TrackingCodeAddToCart] = @TrackingCodeAddToCart, [TrackingCodeInitialCheckout] = @TrackingCodeInitialCheckout, [TrackingCodePurchase] = @TrackingCodePurchase
                            WHERE [ItemId] = @ItemId
                        END
                        ELSE
                        BEGIN
                            INSERT INTO [ItemConversionTracking]([ItemId], [TrackingCodeViewContent], [TrackingCodeAddToCart], [TrackingCodeInitialCheckout], [TrackingCodePurchase]) VALUES(@ItemId, @TrackingCodeViewContent, @TrackingCodeAddToCart, @TrackingCodeInitialCheckout, @TrackingCodePurchase)
                        END";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@ItemId", obj.ItemId),
                new SqlParameter("@TrackingCodeViewContent", obj.TrackingCodeViewContent),
                new SqlParameter("@TrackingCodeAddToCart", obj.TrackingCodeAddToCart),
                new SqlParameter("@TrackingCodeInitialCheckout", obj.TrackingCodeInitialCheckout),
                new SqlParameter("@TrackingCodePurchase", obj.TrackingCodePurchase));
        }

        public ItemConversionTracking GetItemConversionTrackingByItemId(long itemId)
        {
            ItemConversionTracking obj = new ItemConversionTracking();

            using (var db = GetModels())
            {
                db.ItemConversionTrackings.Where(it => it.ItemId == itemId).ToList().ForEach(it => obj = it);
            }

            return obj;
        }
        #endregion

        #region Supplier
        //#region GetAllSuppliers
        //public List<Supplier> GetAllSuppliers()
        //{
        //    List<Supplier> list = new List<Supplier>();
        //    string sql = "SELECT * FROM [Supplier] WHERE [Active] = 1 ORDER BY [SupplierName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Supplier(row));
        //    }
        //    return list;
        //}
        //public List<Supplier> GetAllSuppliers(jQueryDataTableParamModel param)
        //{
        //    List<Supplier> list = new List<Supplier>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectSuppliersList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Supplier(row));
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetSupplier
        //public Supplier GetSupplier(long id)
        //{
        //    Supplier obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectSupplier",
        //        new SqlParameter("@SupplierId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new Supplier(ds.Tables[0].Rows[0]);
        //    }

        //    return obj;
        //}
        //#endregion

        //#region AddSupplier
        //public int AddSupplier(Supplier obj)
        //{
        //    string sql = "INSERT INTO [Supplier](SupplierName, InsertDate, UpdateDate, Active, UserId) VALUES(@SupplierName, @InsertDate, @UpdateDate, @Active, @UserId) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@SupplierName", obj.SupplierName),
        //       new SqlParameter("@InsertDate", obj.InsertDate),
        //       new SqlParameter("@UpdateDate", obj.UpdateDate),
        //       new SqlParameter("@Active", obj.Active),
        //       new SqlParameter("@UserId", obj.UserId)));
        //}
        //#endregion

        //#region UpdateSupplier
        //public int UpdateSupplier(Supplier obj)
        //{
        //    string sql = "UPDATE [Supplier] SET [SupplierName] = @SupplierName, InsertDate = @InsertDate, UpdateDate = @UpdateDate, Active = @Active, UserId = @UserId WHERE [SupplierId] = @SupplierId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@SupplierId", obj.SupplierId),
        //       new SqlParameter("@SupplierName", obj.SupplierName),
        //       new SqlParameter("@InsertDate", obj.InsertDate),
        //       new SqlParameter("@UpdateDate", obj.UpdateDate),
        //       new SqlParameter("@Active", obj.Active),
        //       new SqlParameter("@UserId", obj.UserId));
        //}
        //#endregion

        //#region DeleteSupplier
        //public int DeleteSupplier(int id)
        //{
        //    string sql = "UPDATE [Supplier] SET [Active] = 0 WHERE [SupplierId] = @SupplierId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@SupplierId", id));
        //}
        //#endregion
        #endregion

        #region Country
        public List<Country> GetAllCountries()
        {
            using (var db = GetModels())
            {
                return db.Countries.OrderBy(it => it.CountryName).ToList();
            }
        }
        public List<Country> GetAvailableCountries()
        {
            using (var db = GetModels())
            {
                return (from it in db.Countries
                        join r in db.Regions on it.CountryId equals r.CountryId
                        join st in db.States on r.RegionId equals st.RegionId
                        join s in db.Sites on st.StateId equals s.StateId
                        join i in db.Items on s.SiteId equals i.SiteId
                        where i.Active == true && i.IsPublish == true
                        select it).ToList();
            }
        }
        public List<Tuple<int, int>> GetAvailableCountriesByItemType()
        {
            string sql = @"SELECT DISTINCT [Country].[CountryId], [Item].[ItemTypeId] FROM [Country]
                           INNER JOIN [Region] ON [Region].[CountryId] = [Country].[CountryId]
                           INNER JOIN [State] ON [State].[RegionId] = [Region].[RegionId]
                           INNER JOIN [Site] ON [Site].[StateId] = [State].[StateId]
                           INNER JOIN [Item] ON [Item].[SiteId] = [Site].[SiteId]
                           WHERE [Item].[Active] = 1 AND [Item].[IsPublish] = 1";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(Tuple.Create(DataManager.ToInt(row["ItemTypeId"]), DataManager.ToInt(row["CountryId"])));
            }
            return list;
        }
        #endregion

        #region ItemLang
        //public ItemLang GetItemLang(long ItemId, int langId)
        //{
        //    ItemLang obj = null;
        //    string sql = "SELECT * FROM [ItemLang] WHERE [ItemId] = " + ItemId + " AND [LangId] = " + langId;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new ItemLang(ds.Tables[0].Rows[0]);
        //    }
        //    else
        //    {
        //        obj = new ItemLang();
        //        obj.LangId = langId;
        //        obj.ItemId = ItemId;
        //    }
        //    return obj;
        //}

        //public int DeleteItemLangByItemId(int itemId)
        //{
        //    string sql = "DELETE FROM [ItemLang] WHERE [ItemId] = " + itemId;
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}

        //public int SaveItemLang(ItemLang obj)
        //{
        //    string sql = @"IF(EXISTS(SELECT [ItemId] FROM [ItemLang] WHERE [ItemId] = @ItemId AND [LangId] = @LangId))
        //                    BEGIN
        //                        UPDATE [ItemLang] SET [ItemName] = @ItemName, [ItemDesc] = @ItemDesc, [ItemShortDesc] = @ItemShortDesc, [TrainingArea] = @TrainingArea, [Accommodation] = @Accommodation, [InvoiceName] = @InvoiceName, [UpdateDate] = @UpdateDate, [UserId] = @UserId WHERE [ItemId] = @ItemId AND [LangId] = @LangId;
        //                    END
        //                    ELSE
        //                    BEGIN
        //                        INSERT INTO [ItemLang]([ItemId], [LangId], [ItemName], [ItemDesc], [ItemShortDesc], [TrainingArea], [Accommodation], [InvoiceName], [UpdateDate], [UserId])
        //                        VALUES(@ItemId, @LangId, @ItemName, @ItemDesc, @ItemShortDesc, @TrainingArea, @Accommodation, @InvoiceName, @UpdateDate, @UserId) SELECT @@IDENTITY;
        //                    END";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemId", obj.ItemId),
        //        new SqlParameter("@LangId", obj.LangId),
        //        new SqlParameter("@ItemName", obj.ItemName),
        //        new SqlParameter("@InvoiceName", obj.InvoiceName),
        //        new SqlParameter("@ItemDesc", obj.ItemDesc),
        //        new SqlParameter("@ItemShortDesc", obj.ItemShortDesc),
        //        new SqlParameter("@TrainingArea", obj.TrainingArea),
        //        new SqlParameter("@Accommodation", obj.Accommodation),
        //        new SqlParameter("@UpdateDate", obj.UpdateDate),
        //        new SqlParameter("@UserId", obj.UserId));
        //}
        #endregion

        #region Tax
        //#region GetAllTaxs
        //public List<Tax> GetAllTaxes()
        //{
        //    List<Tax> list = new List<Tax>();
        //    string sql = "SELECT * FROM [Tax] ORDER BY [TaxName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Tax(row));
        //    }
        //    return list;
        //}
        //public List<Tax> GetAllTaxes(jQueryDataTableParamModel param)
        //{
        //    List<Tax> list = new List<Tax>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectTaxesList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Tax(row));
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetTax
        //public Tax GetTax(long id)
        //{
        //    Tax obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectTax",
        //        new SqlParameter("@TaxId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new Tax(ds.Tables[0].Rows[0]);
        //    }

        //    return obj;
        //}
        //#endregion

        //#region AddTax
        //public int AddTax(Tax obj)
        //{
        //    string sql = "INSERT INTO [Tax](TaxCode, TaxName, TaxPercent) VALUES(@TaxCode, @TaxName, @TaxPercent) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@TaxCode", obj.TaxCode),
        //       new SqlParameter("@TaxName", obj.TaxName),
        //       new SqlParameter("@TaxPercent", obj.TaxPercent)), -1);
        //}
        //#endregion

        //#region UpdateTax
        //public int UpdateTax(Tax obj)
        //{
        //    string sql = "UPDATE [Tax] SET [TaxCode] = @TaxCode, [TaxName] = @TaxName, [TaxPercent] = @TaxPercent WHERE TaxId = @TaxId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@TaxId", obj.TaxID),
        //       new SqlParameter("@TaxCode", obj.TaxCode),
        //       new SqlParameter("@TaxName", obj.TaxName),
        //       new SqlParameter("@TaxPercent", obj.TaxPercent));
        //}
        //#endregion

        //#region DeleteTax
        //public int DeleteTax(int id)
        //{
        //    string sql = "DELETE FROM [Tax] WHERE [TaxId] = @TaxId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@TaxId", id));
        //}
        //#endregion
        #endregion

        #region Message
        //#region GetAllMessageByUserId
        //public List<UserMessage> GetAllMessageByUserId(int userId)
        //{
        //    string sql = "SELECT * FROM [UserMessage] WHERE [FromUserId] = @userId OR [ToUserId] = @userId";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@userId", userId));
        //    List<UserMessage> list = new List<UserMessage>();
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new UserMessage(row));
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetMessageByUserName
        ////public List<Message> GetMessageByUserName(string userName)
        ////{
        ////    string sql = "SELECT * FROM [Message] WHERE [FromUserId] = @userId OR [ToUserId] = @userId";
        ////    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        ////        new SqlParameter("@userId", userId));
        ////    List<Message> list = new List<Message>();
        ////    foreach (DataRow row in ds.Tables[0].Rows)
        ////    {
        ////        list.Add(new Message(row));
        ////    }
        ////    return list;
        ////}
        //#endregion
        #endregion

        #region ItemImage
        //public long AddItemImage(ItemImage obj)
        //{
        //    string sql = @"INSERT INTO [ItemImage](ItemId, ImageName, ListNo, BaseName, FileExtension) VALUES(@ItemId, @ImageName, 
        //       (SELECT COUNT(*) FROM [ItemImage] WHERE [ItemId] = @ItemId)
        //    , @BaseName, @FileExtension) SELECT @@IDENTITY";
        //    return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemId", obj.ItemId),
        //        new SqlParameter("@ImageName", obj.ItemId),
        //        new SqlParameter("@BaseName", obj.BaseName),
        //        new SqlParameter("@FileExtension", obj.FileExtension)), -1);
        //}

        //public int DeleteItemImagesByItemId(int id)
        //{
        //    string sql = "DELETE FROM [ItemImage] WHERE [ItemId] = " + id;
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}

        //public long SaveItemImage(ItemImage obj)
        //{
        //    return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, "SaveItemImage",
        //        new SqlParameter("@ItemImageId", obj.ItemImageId),
        //        new SqlParameter("@ItemId", obj.ItemId),
        //        new SqlParameter("@ImageName", obj.ImageName),
        //        new SqlParameter("@ListNo", obj.ListNo),
        //        new SqlParameter("@BaseName", obj.BaseName),
        //        new SqlParameter("@FileExtension", obj.FileExtension)), -1);
        //}

        //public List<ItemImage> GetItemImagesByItemId(long id)
        //{
        //    string sql = "SELECT * FROM [ItemImage] WHERE [ItemId] = " + id;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    List<ItemImage> list = new List<ItemImage>();
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemImage(row));
        //    }
        //    return list;
        //}

        //public string GetImageThumbnailByItemId(long id)
        //{
        //    string imageUrl = "";
        //    string sql = "SELECT [BaseName], [FileExtension] FROM [ItemImage] WHERE [ItemId] = " + id;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        imageUrl = DataManager.ToString(ds.Tables[0].Rows[0]["BaseName"]) + "_t" + DataManager.ToString(ds.Tables[0].Rows[0]["FileExtension"]);
        //    }
        //    return imageUrl;
        //}

        //public long DeleteItemImage(int id)
        //{
        //    string sql = "DELETE FROM [ItemImage] WHERE [ItemImageId] = " + id;
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}
        #endregion

        #region Product
        //private List<Item> AssignLatestItemDataSet(DataSet ds, out int totalPages)
        //{
        //    List<Item> list = new List<Item>();
        //    Item item = null;

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            item = new Item();
        //            item.ItemId = DataManager.ToLong(row["ItemId"]);
        //            item.ItemTypeId = DataManager.ToInt(row["ItemTypeId"]);
        //            item.ItemSlug = DataManager.ToString(row["ItemSlug"]);
        //            item.AlbatrosCourseId = DataManager.ToInt(row["AlbatrosCourseId"]);
        //            item.Price = DataManager.ToDecimal(row["Price"]);
        //            item.CheapestPeriodPrice = DataManager.ToDecimal(row["CheapestPeriodPrice"]);
        //            item.PeriodPrice = DataManager.ToDecimal(row["PeriodPrice"]);
        //            item.SpecialPrice = DataManager.ToDecimal(row["SpecialPrice"]);

        //            if (item.ItemTypeId == (int)ItemType.Type.GreenFee)
        //            {
        //                item.TeeSheetCheapestPrice = DataManager.ToDecimal(row["TeeSheetCheapestPrice"]);
        //            }

        //            item.SiteName = DataManager.ToString(row["SiteName"]);
        //            item.AverageRating = DataManager.ToFloat(row["AverageRating"]);
        //            item.ItemLangs = new List<ItemLang>();
        //            item.ItemLangs.Add(new ItemLang());
        //            item.ItemLangs.FirstOrDefault().ItemName = DataManager.ToString(row["ItemName"]);
        //            item.ItemLangs.FirstOrDefault().ItemShortDesc = DataManager.ToString(row["ItemShortDesc"]);
        //            item.ItemImages = new List<ItemImage>();
        //            item.ItemImages.Add(new ItemImage(row));
        //            list.Add(item);
        //        }
        //    }
        //    if (ds.Tables[1].Rows.Count > 0)
        //    {
        //        totalPages = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalPage"], 1);
        //    }
        //    else
        //    {
        //        totalPages = 1;
        //    }
        //    return list;
        //}
        //public List<Item> GetLatestItems(out int totalPages, int pageSize = 0, int pageIndex = 1, int? countryId = 0, int? regionId = 0, int? stateId = 0, long? siteId = 0, string searchText = "", bool? includeAccommodation = null, string departureMonth = "", int itemTypeId = 0, int? categoryId = 0, int langId = 1, string notIn = "")
        //{
        //    DateTime? departureMonthStart = null, departureMonthEnd = null;
        //    if (!string.IsNullOrEmpty(departureMonth) && departureMonth.Length == 6)
        //    {
        //        int year = DataManager.ToInt(departureMonth.Substring(0, 4), DateTime.Today.Year);
        //        int month = DataManager.ToInt(departureMonth.Substring(4, 2), 1);
        //        departureMonthStart = new DateTime(year, month, 1);
        //        departureMonthEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        //    }
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectLatestItems",
        //        new SqlParameter("@PageSize", pageSize),
        //        new SqlParameter("@PageIndex", pageIndex),
        //        new SqlParameter("@CountryId", countryId),
        //        new SqlParameter("@RegionId", regionId),
        //        new SqlParameter("@StateId", stateId),
        //        new SqlParameter("@SiteId", siteId),
        //        new SqlParameter("@SkipId", null),
        //        new SqlParameter("@SearchText", searchText),
        //        new SqlParameter("@IncludeAccommodation", includeAccommodation),
        //        new SqlParameter("@DepartureMonthStart", departureMonthStart),
        //        new SqlParameter("@DepartureMonthEnd", departureMonthEnd),
        //        new SqlParameter("@ItemTypeId", itemTypeId),
        //        new SqlParameter("@CategoryId", categoryId),
        //        new SqlParameter("@NotIn", notIn),
        //        new SqlParameter("@LangId", langId));

        //    List<Item> list = AssignLatestItemDataSet(ds, out totalPages);
        //    return list;
        //}

        //public List<Item> GetLatestDLGItems(out int totalPages, int pageSize, int page, string searchText = "", int langId = 1, int? level = 0, string shape = null, string genre = null, int? shaft = 0, int? dexterity = 0, int? itemCategory = 0, int? itemSubCategory = 0, int? brandId = 0, long? siteId = 0)
        //{
        //    List<Item> list = new List<Item>();
        //    Item item = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectLatestDLGItems",
        //        new SqlParameter("@PageSize", pageSize),
        //        new SqlParameter("@PageIndex", page),
        //        new SqlParameter("@Level", level),
        //        new SqlParameter("@Shape", shape),
        //        new SqlParameter("@Genre", genre),
        //        new SqlParameter("@Dexterity", dexterity),
        //        new SqlParameter("@Shaft", shaft),
        //        new SqlParameter("@SkipId", null),
        //        new SqlParameter("@SearchText", searchText),
        //        new SqlParameter("@CategoryId", itemCategory),
        //        new SqlParameter("@SubCategoryId", itemSubCategory),
        //        new SqlParameter("@LangId", langId),
        //        new SqlParameter("@BrandNameId", brandId),
        //        new SqlParameter("@SiteId", siteId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            item = new Item(row);
        //            item.SiteName = DataManager.ToString(row["SiteName"]);
        //            item.ItemLangs = new List<ItemLang>();
        //            item.ItemLangs.Add(new ItemLang());
        //            item.ItemLangs.FirstOrDefault().ItemName = DataManager.ToString(row["ItemName"]);
        //            item.ItemLangs.FirstOrDefault().ItemDesc = DataManager.ToString(row["ItemDesc"]);
        //            item.ItemLangs.FirstOrDefault().ItemShortDesc = DataManager.ToString(row["ItemShortDesc"]);
        //            item.ItemLangs.FirstOrDefault().TrainingArea = DataManager.ToString(row["TrainingArea"]);
        //            item.ItemLangs.FirstOrDefault().Accommodation = DataManager.ToString(row["Accommodation"]);
        //            item.PeriodPrice = DataManager.ToDecimal(row["PeriodPrice"]);
        //            item.SpecialPrice = DataManager.ToDecimal(row["SpecialPrice"]);
        //            item.ItemImages = new List<ItemImage>();
        //            item.ItemImages.Add(new ItemImage(row));
        //            list.Add(item);
        //        }
        //    }
        //    if (ds.Tables[1].Rows.Count > 0)
        //    {
        //        totalPages = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalPage"], 1);
        //    }
        //    else
        //    {
        //        totalPages = 1;
        //    }
        //    return list;
        //}

        //public List<Item> GetItemsByItemTypeId(int itemTypeId, int pageSize = 0, long skipId = 0, int langId = 1)
        //{
        //    List<Item> list = new List<Item>();
        //    Item item = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectRandomItems",
        //        new SqlParameter("@PageSize", pageSize),
        //        new SqlParameter("@PageIndex", null),
        //        new SqlParameter("@CountryId", null),
        //        new SqlParameter("@SiteId", null),
        //        new SqlParameter("@CourseId", null),
        //        new SqlParameter("@SkipId", skipId),
        //        new SqlParameter("@SearchText", ""),
        //        new SqlParameter("@ItemTypeId", itemTypeId),
        //        new SqlParameter("@CategoryId", null),
        //        new SqlParameter("@LangId", langId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            item = new Item();
        //            item.ItemId = DataManager.ToLong(row["ItemId"]);
        //            item.ItemTypeId = DataManager.ToInt(row["ItemTypeId"]);
        //            item.ItemSlug = DataManager.ToString(row["ItemSlug"]);
        //            item.SiteName = DataManager.ToString(row["SiteName"]);
        //            item.AverageRating = DataManager.ToFloat(row["AverageRating"]);
        //            item.Price = DataManager.ToDecimal(row["Price"]);
        //            item.CheapestPeriodPrice = DataManager.ToDecimal(row["CheapestPeriodPrice"]);
        //            item.PeriodPrice = DataManager.ToDecimal(row["PeriodPrice"]);
        //            item.SpecialPrice = DataManager.ToDecimal(row["SpecialPrice"]);

        //            if (item.ItemTypeId == (int)ItemType.Type.GreenFee)
        //            {
        //                item.TeeSheetCheapestPrice = DataManager.ToDecimal(row["TeeSheetCheapestPrice"]);
        //            }
        //            item.ItemLangs = new List<ItemLang>();
        //            item.ItemLangs.Add(new ItemLang());
        //            item.ItemLangs[0].ItemName = DataManager.ToString(row["ItemName"]);
        //            item.ItemLangs[0].ItemShortDesc = DataManager.ToString(row["ItemShortDesc"]);
        //            item.ItemImages = new List<ItemImage>();
        //            item.ItemImages.Add(new ItemImage(row));

        //            list.Add(item);
        //        }
        //    }
        //    return list;
        //}

        //public List<Item> GetItemsBySiteId(long siteId, int pageSize = 0, int langId = 1)
        //{
        //    List<Item> list = new List<Item>();
        //    Item item = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectRandomItems",
        //        new SqlParameter("@PageSize", pageSize),
        //        new SqlParameter("@PageIndex", null),
        //        new SqlParameter("@CountryId", null),
        //        new SqlParameter("@SiteId", siteId),
        //        new SqlParameter("@CourseId", null),
        //        new SqlParameter("@SkipId", null),
        //        new SqlParameter("@SearchText", ""),
        //        new SqlParameter("@ItemTypeId", null),
        //        new SqlParameter("@CategoryId", null),
        //        new SqlParameter("@LangId", langId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            item = new Item();
        //            item.ItemId = DataManager.ToLong(row["ItemId"]);
        //            item.ItemTypeId = DataManager.ToInt(row["ItemTypeId"]);
        //            item.ItemSlug = DataManager.ToString(row["ItemSlug"]);
        //            item.SiteName = DataManager.ToString(row["SiteName"]);
        //            item.AverageRating = DataManager.ToFloat(row["AverageRating"]);
        //            item.Price = DataManager.ToDecimal(row["Price"]);
        //            item.PeriodPrice = DataManager.ToDecimal(row["PeriodPrice"]);
        //            item.SpecialPrice = DataManager.ToDecimal(row["SpecialPrice"]);
        //            item.ItemLangs = new List<ItemLang>();
        //            item.ItemLangs.Add(new ItemLang());
        //            item.ItemLangs.FirstOrDefault().ItemName = DataManager.ToString(row["ItemName"]);
        //            item.ItemLangs.FirstOrDefault().ItemShortDesc = DataManager.ToString(row["ItemShortDesc"]);
        //            item.ItemImages = new List<ItemImage>();
        //            item.ItemImages.Add(new ItemImage(row));

        //            list.Add(item);
        //        }
        //    }
        //    return list;
        //}

        //public List<Item> SearchGreenFee(out int totalPages, int pageSize = 0, int pageIndex = 1, int? regionId = 0, int? stateId = 0, long? siteId = 0, int? timeSlot = 1, DateTime? fromDate = null, DateTime? toDate = null, int? cat = null, bool? includePractice = null, string where = "", int langId = 1, string notIn = "")
        //{
        //    List<Item> list = new List<Item>();
        //    Item item = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SearchGreenFee",
        //        new SqlParameter("@PageSize", pageSize), // Show all products
        //        new SqlParameter("@PageIndex", null),
        //        //new SqlParameter("@PageSize", pageSize),
        //        //new SqlParameter("@PageIndex", pageIndex),
        //        new SqlParameter("@RegionId", regionId),
        //        new SqlParameter("@StateId", stateId),
        //        new SqlParameter("@SiteId", siteId),
        //        new SqlParameter("@FromDate", fromDate),
        //        new SqlParameter("@ToDate", toDate),
        //        new SqlParameter("@TimeSlot", timeSlot),
        //        new SqlParameter("@IncludePractice", includePractice),
        //        new SqlParameter("@CategoryId", cat),
        //        new SqlParameter("@NotIn", notIn),
        //        new SqlParameter("@LangId", langId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            item = new Item(row);
        //            item.AlbatrosCourseId = DataManager.ToInt(row["AlbatrosCourseId"]);
        //            item.SiteName = DataManager.ToString(row["SiteName"]);
        //            item.AverageRating = DataManager.ToFloat(row["AverageRating"]);
        //            item.PeriodPrice = DataManager.ToDecimal(row["PeriodPrice"]);
        //            item.SpecialPrice = DataManager.ToDecimal(row["SpecialPrice"]);
        //            item.TeeSheetCheapestPrice = DataManager.ToDecimal(row["TeeSheetCheapestPrice"]);
        //            item.ItemLangs = new List<ItemLang>();
        //            item.ItemLangs.Add(new ItemLang());
        //            item.ItemLangs.FirstOrDefault().ItemName = DataManager.ToString(row["ItemName"]);
        //            item.ItemLangs.FirstOrDefault().ItemShortDesc = DataManager.ToString(row["ItemShortDesc"]);
        //            item.ItemImages = new List<ItemImage>();
        //            item.ItemImages.Add(new ItemImage(row));
        //            list.Add(item);
        //        }
        //    }
        //    if (ds.Tables[1].Rows.Count > 0)
        //    {
        //        totalPages = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalPage"], 1);
        //    }
        //    else
        //    {
        //        totalPages = 1;
        //    }
        //    return list;
        //}

        //public Item GetItemBySlug(string slug, int langId = 1)
        //{
        //    Item item = null;
        //    DataSet ds = null;
        //    string sql = @"WITH PeriodPrices
        //    AS
        //    (
        //     SELECT [ItemId], [Price] AS [Price]
        //     FROM [ItemPrice]
        //     WHERE GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]) AND [PriceType] = 0 AND [Price] > 0
        //    ),
        //    CheapestPeriodPrices
        //    AS
        //    (
        //     SELECT [ItemId], MIN([Price]) AS [Price]
        //     FROM [ItemPrice]
        //     WHERE [PriceType] = 0 AND [Price] > 0 AND DATEADD(SECOND, -1, DATEADD(DAY, 1, [EndDate])) > GETDATE()
        //     GROUP BY [ItemId]
        //    ),
        //    MinPricingDate
        //    AS
        //    (
        //     SELECT [ItemId], Min([StartDate]) AS [MinDate]
        //     FROM [ItemPrice]
        //     WHERE [Price] > 0 AND ([StartDate] <= GETDATE() AND DATEADD(SECOND, -1, DATEADD(DAY, 1, [EndDate])) > GETDATE())
        //     GROUP BY [ItemId]
        //    ),
        //    MaxPricingDate
        //    AS
        //    (
        //     SELECT [ItemId], MAX([EndDate]) AS [MaxDate]
        //     FROM [ItemPrice]
        //     WHERE [Price] > 0 AND DATEADD(SECOND, -1, DATEADD(DAY, 1, [EndDate])) > GETDATE()
        //     GROUP BY [ItemId]
        //    ),
        //    SpecialPrices
        //    AS
        //    (
        //     SELECT [ItemId], MIN([Price]) AS [Price]
        //     FROM [ItemPrice] p1
        //     WHERE GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]) AND [PriceType] = 0 AND [Price] > 0
        //     GROUP BY [ItemId]
        //    ),
        //    TeeSheetCheapestPrice
        //    AS
        //    (
        //     SELECT [ItemId], MIN([Discount]) AS [CheapestPrice] FROM [TeeSheet] WHERE [Price] > 0 AND [Discount] > 0 AND [TeeSheetDate] >= GETDATE() GROUP BY [ItemId]
        //    )
        //    SELECT TOP(1) Item.*, ItemLang.ItemName, ItemLang.ItemShortDesc, ItemLang.ItemDesc, ItemLang.Accommodation, ItemLang.TrainingArea, ItemLang.UserId, ItemLang.[LangId], ItemLang.UpdateDate AS ItemLangUpdateDate, ISNULL([SiteLang].[SiteName], 'N/A') AS SiteName, [Site].[SiteSlug], ISNULL([State].[StateName], 'N/A') AS StateName, ISNULL([Tax].[TaxPercent], 0) AS [TaxRate], ISNULL(idp.[Price], 0) AS [SpecialPrice], ISNULL(ipp.[Price], 0) AS [PeriodPrice], ISNULL(icpp.[Price], 0) AS [CheapestPeriodPrice], ISNULL(tp.[CheapestPrice], 0) AS [TeeSheetCheapestPrice], ISNULL(minpd.[MinDate], GETDATE()) AS [ItemMinDate], ISNULL(maxpd.[MaxDate], GETDATE()) AS [ItemMaxDate], ISNULL([Brand].[BrandName], N'N/A') AS BrandName, ISNULL([ShippingTypeLang].[ShippingTypeName], ISNULL([ShippingType].[ShippingTypeName], N'-')) AS ShippingTypeName, [Site].[AlbatrosCourseId], [Site].[AlbatrosClubId]
        //                    FROM [Item]
        //                    LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND ([ItemLang].[LangId] = @LangId)
        //                    LEFT JOIN [Site] ON [Site].[SiteId] = [Item].[SiteId]
        //                    LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] AND ([SiteLang].[LangId] = @LangId)
        //                    LEFT JOIN [Tax] ON [Tax].[TaxId] = [Item].[TaxId]
        //                    LEFT JOIN [State] ON [Site].[StateId] = [State].[StateId]
        //                    LEFT JOIN [Brand] ON [Brand].[BrandId] = [Item].[BrandId]
        //                    LEFT JOIN [ShippingType] ON [ShippingType].[ShippingTypeId] = [Item].[ShippingTypeId]
        //                    LEFT JOIN [ShippingTypeLang] ON [ShippingTypeLang].[ShippingTypeId] = [ShippingType].[ShippingTypeId] AND [ShippingTypeLang].[LangId] = @LangId
        //                 LEFT JOIN PeriodPrices ipp ON ipp.[ItemId] = [Item].[ItemId]
        //                    LEFT JOIN CheapestPeriodPrices icpp ON icpp.[ItemId] = [Item].[ItemId]
        //                 LEFT JOIN TeeSheetCheapestPrice tp ON tp.[ItemId] = [Item].[ItemId]
        //                    LEFT JOIN MinPricingDate minpd ON minpd.[ItemId] = [Item].[ItemId]
        //                    LEFT JOIN MaxPricingDate maxpd ON maxpd.[ItemId] = [Item].[ItemId]
        //                 LEFT JOIN (SELECT [ItemId],[Price] FROM [ItemPrice] WHERE [PriceType] = 1 AND GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate])) idp ON idp.[ItemId] = [Item].[ItemId]
        //                    WHERE [Item].[Active] = 1 AND ItemSlug = @ItemSlug AND [Item].[IsPublish] = 1 AND (ISNULL([PublishStartDate], GETDATE()) <= GETDATE() AND DATEADD(DAY, 1, ISNULL([PublishEndDate], GETDATE())) >= GETDATE());";
        //    ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemSlug", slug),
        //        new SqlParameter("@LangId", langId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        DataRow row = ds.Tables[0].Rows[0];
        //        item = new Item(row);
        //        item.SiteName = DataManager.ToString(row["SiteName"]);
        //        item.SiteSlug = DataManager.ToString(row["SiteSlug"]);
        //        item.StateName = DataManager.ToString(row["StateName"]);
        //        item.AlbatrosCourseId = DataManager.ToInt(row["AlbatrosCourseId"]);
        //        item.AlbatrosClubId = DataManager.ToLong(row["AlbatrosClubId"]);
        //        item.CheapestPeriodPrice = DataManager.ToDecimal(row["CheapestPeriodPrice"]);
        //        item.TeeSheetCheapestPrice = DataManager.ToDecimal(row["TeeSheetCheapestPrice"]);
        //        item.ItemMinDate = DataManager.ToDateTime(row["ItemMinDate"]);
        //        item.ItemMaxDate = DataManager.ToDateTime(row["ItemMaxDate"]);
        //        item.PeriodPrice = DataManager.ToDecimal(row["PeriodPrice"]);
        //        item.SpecialPrice = DataManager.ToDecimal(row["SpecialPrice"]);
        //        item.BrandName = DataManager.ToString(row["BrandName"]);
        //        item.ShippingTypeName = DataManager.ToString(row["ShippingTypeName"]);
        //        item.ItemLangs = new List<ItemLang>();
        //        item.ItemImages = new List<ItemImage>();
        //        item.ItemReviews = new List<ItemReview>();
        //        item.ItemLangs.Add(new ItemLang());
        //        item.ItemLangs.FirstOrDefault().ItemName = DataManager.ToString(row["ItemName"]);
        //        item.ItemLangs.FirstOrDefault().ItemDesc = DataManager.ToString(row["ItemDesc"]);
        //        item.ItemLangs.FirstOrDefault().ItemShortDesc = DataManager.ToString(row["ItemShortDesc"]);
        //        item.ItemLangs.FirstOrDefault().Accommodation = DataManager.ToString(row["Accommodation"]);
        //        item.ItemLangs.FirstOrDefault().TrainingArea = DataManager.ToString(row["TrainingArea"]);
        //        item.ItemLangs.FirstOrDefault().UserId = DataManager.ToInt(row["UserId"]);
        //        item.ItemLangs.FirstOrDefault().LangId = langId;

        //        // Get Item Images.
        //        sql = "SELECT [ItemImage].* FROM [ItemImage] WHERE [ItemId] = " + item.ItemId + " ORDER BY [ListNo], [ItemImageId];";
        //        sql += "SELECT AVG(Rating) AS AverageRating, COUNT(ItemReviewId) AS ReviewNumber, [ItemId] FROM [ItemReview] WHERE [IsApproved] = 1 AND [ItemId] = " + item.ItemId + " GROUP BY [ItemId];";
        //        sql += @"SELECT TOP(3) [ItemReview].*, [Users].[Firstname] AS [ReviewerName], [City].[CityName] AS [ReviewerCityName] FROM [ItemReview] 
        //                INNER JOIN [Users] ON [Users].[UserId] = [ItemReview].[UserId] 
        //                LEFT JOIN [City] ON [City].[CityId] = [Users].[CityId] 
        //                WHERE [IsApproved] = 1 AND [ItemId] = " + item.ItemId + " ORDER BY [UpdatedDate] DESC;";
        //        sql += "SELECT [Course].* FROM [Course] INNER JOIN [Item] ON [Item].[CourseId] = [Course].[CourseId] WHERE [ItemId] = " + item.ItemId;
        //        ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            foreach (DataRow imgRow in ds.Tables[0].Rows)
        //            {
        //                item.ItemImages.Add(new ItemImage(imgRow));
        //            }
        //        }
        //        if (ds.Tables[1].Rows.Count > 0)
        //        {
        //            item.AverageRating = DataManager.ToFloat(ds.Tables[1].Rows[0]["AverageRating"], 0);
        //            item.ReviewNumber = DataManager.ToInt(ds.Tables[1].Rows[0]["ReviewNumber"], 0);
        //        }
        //        if (ds.Tables[2].Rows.Count > 0)
        //        {
        //            foreach (DataRow reviewRow in ds.Tables[2].Rows)
        //            {
        //                item.ItemReviews.Add(new ItemReview(reviewRow));
        //            }
        //        }
        //        if (ds.Tables[3].Rows.Count > 0)
        //        {
        //            item.Course = new Course(ds.Tables[3].Rows[0]);
        //        }

        //        if (item.ItemTypeId == (int)ItemType.Type.GreenFee)
        //        {
        //            sql = "SELECT COUNT(*) FROM [TeeSheet] WHERE [TeeSheetDate] = @Date";
        //            item.HasTeeSheet = DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //                new SqlParameter("@Date", DateTime.Today))) > 0;
        //        }
        //    }
        //    return item;
        //}

        //public bool IsExistsItemSlug(string slug, long? itemId)
        //{
        //    string sql = "SELECT COUNT(ItemId) FROM [Item] WHERE [ItemSlug] = @ItemSlug";
        //    if (itemId.HasValue)
        //    {
        //        sql += " AND [ItemId] <> " + itemId.Value;
        //    }
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemSlug", slug))) > 0;
        //}

        //public bool GetItemPricesByDate(long itemId, DateTime d, out decimal price, out decimal periodPrice, out decimal specialPrice, out decimal cheapestPrice, out decimal cheapestPeriodPrice, out decimal teeSheetCheapestPrice)
        //{
        //    bool result = false;
        //    price = 0;
        //    periodPrice = 0;
        //    specialPrice = 0;
        //    cheapestPrice = 0;
        //    cheapestPeriodPrice = 0;
        //    teeSheetCheapestPrice = 0;
        //    string sql = @"WITH PeriodPrices
        //    AS
        //    (
        //     SELECT [ItemId], MIN([Price]) AS [Price]
        //     FROM [ItemPrice]
        //     WHERE (@Date BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]))
        //                    AND [PriceType] = 0 AND [Price] > 0
        //     GROUP BY [ItemId]
        //    ),
        //                CheapestPeriodPrices
        //    AS
        //    (
        //     SELECT [ItemId], MIN([Price]) AS [Price]
        //     FROM [ItemPrice] WHERE [PriceType] = 0 AND (DATEADD(SECOND, -1, DATEADD(DAY, 1, [EndDate])) >= @Date)
        //     GROUP BY [ItemId]
        //    ),
        //    SpecialPrices
        //    AS
        //    (
        //     SELECT [ItemId], MIN([Price]) AS [Price]
        //     FROM [ItemPrice]
        //     WHERE (@Date BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]))
        //                    AND [PriceType] = 1 AND [Price] > 0
        //     GROUP BY [ItemId]
        //    ),
        //    TeeSheetCheapestPrice
        //    AS
        //    (
        //     SELECT [ItemId], MIN([Price] - [Discount]) AS [CheapestPrice] FROM [TeeSheet] WHERE [Price] > 0 AND [TeeSheetDate] >= @Date GROUP BY [ItemId]
        //    )
        //    SELECT ISNULL([item].[Price], 0) AS Price, ISNULL(ipp.[Price], 0) AS PeriodPrice, ISNULL(idp.[Price], 0) AS SpecialPrice, ISNULL(icpp.[Price], 0) AS CheapestPeriodPrice, ISNULL(tp.[CheapestPrice], 0) AS TeeSheetCheapestPrice FROM [Item]
        //                 LEFT JOIN PeriodPrices ipp ON ipp.[ItemId] = [Item].[ItemId]
        //                 LEFT JOIN SpecialPrices idp ON idp.[ItemId] = [Item].[ItemId]
        //                 LEFT JOIN CheapestPeriodPrices icpp ON icpp.[ItemId] = [Item].[ItemId]
        //                 LEFT JOIN TeeSheetCheapestPrice tp ON tp.[ItemId] = [Item].[ItemId]
        //                    WHERE [Item].[ItemId] = @ItemId AND [Item].[PublishEndDate] >= GETDATE()";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemId", itemId),
        //        new SqlParameter("@Date", d));
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        DataRow row = ds.Tables[0].Rows[0];
        //        decimal p = 0, pp = 0, sp = 0, cp = 0, cpp = 0, tcp = 0;
        //        p = DataManager.ToDecimal(row["Price"]);
        //        pp = DataManager.ToDecimal(row["PeriodPrice"]);
        //        sp = DataManager.ToDecimal(row["SpecialPrice"]);
        //        cpp = DataManager.ToDecimal(row["CheapestPeriodPrice"]);
        //        tcp = DataManager.ToDecimal(row["TeeSheetCheapestPrice"]);

        //        if (sp > 0)
        //        {
        //            cp = sp;
        //            if (pp > 0)
        //            {
        //                cp = cp < pp ? cp : pp;
        //                cp = cp < p ? cp : p;
        //            }
        //            else
        //            {
        //                cp = cp < p ? cp : p;
        //            }
        //        }
        //        else if (pp > 0)
        //        {
        //            cp = pp;
        //            cp = cp < p ? p : cp;
        //        }
        //        else
        //        {
        //            cp = p;
        //        }

        //        price = p;
        //        periodPrice = pp;
        //        specialPrice = sp;
        //        cheapestPrice = cp;
        //        cheapestPeriodPrice = cpp;
        //        teeSheetCheapestPrice = tcp;

        //        result = true;
        //    }
        //    return result;
        //}

        //public List<Item> GetItemPickerData(string exclude, int langId = 1)
        //{
        //    string sql = @"SELECT [Item].[ItemId], [Item].[ItemCode], [ItemLang].[ItemName], [ItemType].[ItemTypeName], [SiteLang].[SiteName]  FROM [Item] 
        //                    LEFT JOIN [ItemType] ON [ItemType].[ItemTypeId] = [Item].[ItemTypeId]
        //                    LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = @LangId
        //                    LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Item].[SiteId] AND [SiteLang].[LangId] = @LangId
        //                    WHERE [Item].[Active] = 1 AND [Item].[ItemTypeId] <> " + (int)ItemType.Type.DLGCard;
        //    if (!String.IsNullOrEmpty(exclude.Trim()))
        //    {
        //        sql += " AND [Item].[ItemId] NOT IN(" + exclude + ")";
        //    }
        //    List<Item> items = new List<Item>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@LangId", langId));
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            items.Add(new Item()
        //            {
        //                ItemId = DataManager.ToLong(row["ItemId"]),
        //                ItemCode = DataManager.ToString(row["ItemCode"]),
        //                ItemName = DataManager.ToString(row["ItemName"]),
        //                SiteName = DataManager.ToString(row["SiteName"]),
        //                ItemTypeName = DataManager.ToString(row["ItemTypeName"])
        //            });
        //        }
        //    }
        //    return items;
        //}
        #endregion

        #region Coupon

        //#region GetAllCouponGroups
        //public List<CouponGroup> GetAllCouponGroups()
        //{
        //    List<CouponGroup> list = new List<CouponGroup>();
        //    string sql = "SELECT * FROM [CouponGroup] WHERE [Active] = 1 ORDER BY [CouponGroupCode]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new CouponGroup(row));
        //    }
        //    return list;
        //}
        //public List<CouponGroup> GetAllCouponGroups(jQueryDataTableParamModel param, int langId = 1)
        //{
        //    List<CouponGroup> list = new List<CouponGroup>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectCouponGroupsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new CouponGroup(row)
        //        {
        //            CouponsCount = DataManager.ToString(row["CouponsCount"])
        //        });
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //#endregion

        //#region GetCouponGroup
        //public CouponGroup GetCouponGroup(long id, int langId = 1)
        //{
        //    string sql = "SELECT * FROM [CouponGroup] WHERE [CouponGroupId] = @CouponGroupId;";
        //    sql += @"SELECT [Item].[ItemId], [ItemLang].[ItemName], [ItemType].[ItemTypeName], [SiteLang].[SiteName] FROM [CouponGroupItem]
        //            INNER JOIN [CouponGroup] ON [CouponGroup].[CouponGroupId] = [CouponGroupItem].[CouponGroupId]
        //            INNER JOIN [Item] ON [Item].[ItemId] = [CouponGroupItem].[ItemId]
        //            LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = @LangId
        //            LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Item].[SiteId] AND [SiteLang].[LangId] = @LangId
        //            LEFT JOIN [ItemType] ON [ItemType].[ItemTypeId] = [Item].[ItemTypeId]
        //            WHERE [CouponGroupItem].[CouponGroupId] = @CouponGroupId AND [CouponGroup].[Active] = 1
        //            ORDER BY [ItemLang].[ItemName], [ItemType].[ItemTypeName], [SiteLang].[SiteName];";
        //    sql += @"SELECT [CouponGroupItemCategory].[CategoryId] FROM [CouponGroupItemCategory]
        //            INNER JOIN [CouponGroup] ON [CouponGroup].[CouponGroupId] = [CouponGroupItemCategory].[CouponGroupId]
        //            WHERE [CouponGroupItemCategory].[CouponGroupId] = @CouponGroupId AND [CouponGroup].[Active] = 1;";
        //    sql += @"SELECT [CouponGroupItemType].[ItemTypeId] FROM [CouponGroupItemType]
        //            INNER JOIN [CouponGroup] ON [CouponGroup].[CouponGroupId] = [CouponGroupItemType].[CouponGroupId]
        //            WHERE [CouponGroupItemType].[CouponGroupId] = @CouponGroupId AND [CouponGroup].[Active] = 1;";
        //    CouponGroup obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@CouponGroupId", id),
        //        new SqlParameter("@LangId", langId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new CouponGroup(ds.Tables[0].Rows[0]);
        //        if (ds.Tables.Count > 1)
        //        {
        //            foreach (DataRow row in ds.Tables[1].Rows)
        //            {
        //                obj.Items.Add(new Item()
        //                {
        //                    ItemId = DataManager.ToLong(row["ItemId"]),
        //                    ItemName = DataManager.ToString(row["ItemName"]),
        //                    ItemTypeName = DataManager.ToString(row["ItemTypeName"]),
        //                    SiteName = DataManager.ToString(row["SiteName"]),
        //                });
        //            }
        //        }
        //        if (ds.Tables.Count > 2)
        //        {
        //            foreach (DataRow row in ds.Tables[2].Rows)
        //            {
        //                obj.ItemCategoryIds.Add(DataManager.ToInt(row["CategoryId"]));
        //            }
        //        }
        //        if (ds.Tables.Count > 3)
        //        {
        //            foreach (DataRow row in ds.Tables[3].Rows)
        //            {
        //                obj.ItemTypeIds.Add(DataManager.ToInt(row["ItemTypeId"]));
        //            }
        //        }
        //    }

        //    return obj;
        //}
        //#endregion

        //#region AddCouponGroup
        //public int AddCouponGroup(CouponGroup obj)
        //{
        //    string sql = "INSERT INTO [CouponGroup](CouponGroupName, CouponGroupDesc, StartDate, EndDate, Reduction, MinimumAmount, CouponType, CouponUsageType, UsagePeriodType, TimesToUse, InsertedDate, UpdatedDate, Active) VALUES(@CouponGroupName, @CouponGroupDesc, @StartDate, @EndDate, @Reduction, @MinimumAmount, @CouponType, @CouponUsageType, @UsagePeriodType, @TimesToUse, @InsertedDate, @UpdatedDate, @Active) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@CouponGroupName", obj.CouponGroupName),
        //       new SqlParameter("@CouponGroupDesc", obj.CouponGroupDesc),
        //       new SqlParameter("@StartDate", obj.StartDate),
        //       new SqlParameter("@EndDate", obj.EndDate),
        //       new SqlParameter("@Reduction", obj.Reduction),
        //       new SqlParameter("@MinimumAmount", obj.MinimumAmount),
        //       new SqlParameter("@CouponType", obj.CouponType),
        //       new SqlParameter("@CouponUsageType", obj.CouponUsageType),
        //       new SqlParameter("@UsagePeriodType", obj.UsagePeriodType),
        //       new SqlParameter("@TimesToUse", obj.TimesToUse),
        //       new SqlParameter("@InsertedDate", obj.InsertedDate),
        //       new SqlParameter("@UpdatedDate", obj.UpdatedDate),
        //       new SqlParameter("@Active", obj.Active)));
        //}
        //#endregion

        //#region UpdateCouponGroup
        //public int UpdateCouponGroup(CouponGroup obj)
        //{
        //    string sql = "UPDATE [CouponGroup] SET CouponGroupName = @CouponGroupName, CouponGroupDesc = @CouponGroupDesc, StartDate = @StartDate, EndDate = @EndDate, Reduction = @Reduction, MinimumAmount = @MinimumAmount, CouponType = @CouponType, CouponUsageType = @CouponUsageType, UsagePeriodType = @UsagePeriodType, TimesToUse = @TimesToUse, InsertedDate = @InsertedDate, UpdatedDate = @UpdatedDate, Active = @Active WHERE CouponGroupId = @CouponGroupId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@CouponGroupId", obj.CouponGroupId),
        //       new SqlParameter("@CouponGroupName", obj.CouponGroupName),
        //       new SqlParameter("@CouponGroupDesc", obj.CouponGroupDesc),
        //       new SqlParameter("@StartDate", obj.StartDate),
        //       new SqlParameter("@EndDate", obj.EndDate),
        //       new SqlParameter("@Reduction", obj.Reduction),
        //       new SqlParameter("@MinimumAmount", obj.MinimumAmount),
        //       new SqlParameter("@CouponType", obj.CouponType),
        //       new SqlParameter("@CouponUsageType", obj.CouponUsageType),
        //       new SqlParameter("@UsagePeriodType", obj.UsagePeriodType),
        //       new SqlParameter("@TimesToUse", obj.TimesToUse),
        //       new SqlParameter("@InsertedDate", obj.InsertedDate),
        //       new SqlParameter("@UpdatedDate", obj.UpdatedDate),
        //       new SqlParameter("@Active", obj.Active));
        //}
        //#endregion

        //#region DeleteCouponGroup
        //public int DeleteCouponGroup(int id)
        //{
        //    string sql = "UPDATE [CouponGroup] SET Active = 0 WHERE CouponGroupId = @CouponGroupId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@CouponGroupId", id));
        //}
        //#endregion

        //#region GetAllCoupons
        //public List<Coupon> GetAllCoupons()
        //{
        //    List<Coupon> list = new List<Coupon>();
        //    string sql = "SELECT * FROM [Coupon] WHERE [Active] = 1 ORDER BY [CouponCode]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Coupon(row));
        //    }
        //    return list;
        //}
        //public List<Coupon> GetAllCoupons(jQueryDataTableParamModel param, int langId = 1)
        //{
        //    List<Coupon> list = new List<Coupon>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectCouponsList");

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Coupon(row));
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetCoupon
        //public Coupon GetCoupon(int id)
        //{
        //    Coupon obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectCoupon",
        //        new SqlParameter("@CouponId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new Coupon(ds.Tables[0].Rows[0]);
        //    }

        //    return obj;
        //}
        //#endregion

        //#region GetCouponByCouponCode
        //public Coupon GetCouponByCouponCode(string code)
        //{
        //    string sql = @"SELECT Coupon.*, CouponGroup.StartDate, CouponGroup.EndDate, CouponGroup.Reduction, CouponGroup.MinimumAmount, CouponGroup.CouponType, CouponGroup.CouponUsageType, CouponGroup.UsagePeriodType, CouponGroup.TimesToUse
        //                   FROM Coupon 
        //                   INNER JOIN [CouponGroup] ON [CouponGroup].[CouponGroupId] = [Coupon].[CouponGroupId] WHERE [CouponCode] = @CouponCode;

        //                    SELECT [Item].[ItemId] FROM [CouponGroupItem]
        //                    INNER JOIN [CouponGroup] ON [CouponGroup].[CouponGroupId] = [CouponGroupItem].[CouponGroupId]
        //                    INNER JOIN [Coupon] ON [Coupon].[CouponGroupId] = [CouponGroupItem].[CouponGroupId]
        //                    INNER JOIN [Item] ON [Item].[ItemId] = [CouponGroupItem].[ItemId]
        //                    WHERE [Coupon].[CouponCode] = @CouponCode AND [CouponGroup].[Active] = 1;

        //                    SELECT [CouponGroupItemCategory].[CategoryId] FROM [CouponGroupItemCategory]
        //                    INNER JOIN [CouponGroup] ON [CouponGroup].[CouponGroupId] = [CouponGroupItemCategory].[CouponGroupId]
        //                    INNER JOIN [Coupon] ON [Coupon].[CouponGroupId] = [CouponGroupItemCategory].[CouponGroupId]
        //                    WHERE [Coupon].[CouponCode] = @CouponCode AND [CouponGroup].[Active] = 1;

        //                    SELECT [CouponGroupItemType].[ItemTypeId] FROM [CouponGroupItemType]
        //                    INNER JOIN [CouponGroup] ON [CouponGroup].[CouponGroupId] = [CouponGroupItemType].[CouponGroupId]
        //                    INNER JOIN [Coupon] ON [Coupon].[CouponGroupId] = [CouponGroupItemType].[CouponGroupId]
        //                    WHERE [Coupon].[CouponCode] = @CouponCode AND [CouponGroup].[Active] = 1;";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@CouponCode", code));
        //    Coupon coupon = null;
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        DataRow row = ds.Tables[0].Rows[0];
        //        coupon = new Coupon(row);
        //        coupon.CouponGroup = new CouponGroup()
        //        {
        //            CouponGroupId = DataManager.ToLong(row["CouponGroupId"]),
        //            StartDate = DataManager.ToDateTime(row["StartDate"]),
        //            EndDate = DataManager.ToDateTime(row["EndDate"]),
        //            Reduction = DataManager.ToDecimal(row["Reduction"]),
        //            MinimumAmount = DataManager.ToDecimal(row["MinimumAmount"]),
        //            CouponType = DataManager.ToInt(row["CouponType"]),
        //            CouponUsageType = DataManager.ToInt(row["CouponUsageType"]),
        //            UsagePeriodType = DataManager.ToInt(row["UsagePeriodType"]),
        //            TimesToUse = DataManager.ToInt(row["TimesToUse"])
        //        };
        //        if (ds.Tables.Count > 1)
        //        {
        //            foreach (DataRow rowItem in ds.Tables[1].Rows)
        //            {
        //                coupon.CouponGroup.Items.Add(new Item()
        //                {
        //                    ItemId = DataManager.ToLong(rowItem["ItemId"])
        //                });
        //            }
        //        }
        //        if (ds.Tables.Count > 2)
        //        {
        //            foreach (DataRow rowItemCategory in ds.Tables[2].Rows)
        //            {
        //                coupon.CouponGroup.ItemCategoryIds.Add(DataManager.ToInt(rowItemCategory["CategoryId"]));
        //            }
        //        }
        //        if (ds.Tables.Count > 3)
        //        {
        //            foreach (DataRow rowItemType in ds.Tables[3].Rows)
        //            {
        //                coupon.CouponGroup.ItemTypeIds.Add(DataManager.ToInt(rowItemType["ItemTypeId"]));
        //            }
        //        }
        //    }
        //    return coupon;
        //}
        //#endregion

        //#region IsExistsCouponCode
        //public bool IsExistsCouponCode(string couponCode)
        //{
        //    string sql = "SELECT COUNT(CouponId) FROM Coupon WHERE CouponCode = @CouponCode AND [Active] = 1";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@CouponCode", couponCode)), 0) > 0;
        //}
        //#endregion

        //#region AddCoupon
        //public int AddCoupon(Coupon obj)
        //{
        //    string sql = @"IF NOT EXISTS(SELECT CouponId FROM Coupon WHERE CouponCode = @CouponCode)
        //                   BEGIN
        //                        INSERT INTO [Coupon](CouponCode, CouponGroupId, InsertedDate, UpdatedDate, Active) VALUES(@CouponCode, @CouponGroupId, @InsertedDate, @UpdatedDate, @Active) SELECT @@IDENTITY
        //                   END";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@CouponCode", obj.CouponCode),
        //       new SqlParameter("@CouponGroupId", obj.CouponGroupId),
        //       new SqlParameter("@InsertedDate", obj.InsertedDate),
        //       new SqlParameter("@UpdatedDate", obj.UpdatedDate),
        //       new SqlParameter("@Active", obj.Active)));
        //}
        //#endregion

        //#region ImportCoupons
        //public void ImportCoupons(int couponGroupId, string[] coupons)
        //{
        //    DateTime now = DateTime.Now;
        //    Coupon obj = new Coupon()
        //    {
        //        CouponGroupId = couponGroupId,
        //        InsertedDate = now,
        //        UpdatedDate = now,
        //        Active = true
        //    };
        //    foreach (string coupon in coupons)
        //    {
        //        obj.CouponCode = coupon;
        //        AddCoupon(obj);
        //    }
        //}
        //#endregion

        //#region UpdateCoupon
        //public int UpdateCoupon(Coupon obj)
        //{
        //    string sql = "UPDATE [Coupon] SET CouponGroupId = @CouponGroupId, CouponCode = @CouponCode, InsertedDate = @InsertedDate, UpdatedDate = @UpdatedDate, Active = @Active WHERE CouponId = @CouponId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@CouponId", obj.CouponId),
        //       new SqlParameter("@CouponGroupId", obj.CouponGroupId),
        //       new SqlParameter("@CouponCode", obj.CouponCode),
        //       new SqlParameter("@InsertedDate", obj.InsertedDate),
        //       new SqlParameter("@UpdatedDate", obj.UpdatedDate),
        //       new SqlParameter("@Active", obj.Active));
        //}
        //#endregion

        //#region DeleteCoupon
        //public int DeleteCoupon(long id)
        //{
        //    string sql = "UPDATE [Coupon] SET Active = 0 WHERE CouponId = @CouponId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@CouponId", id));
        //}
        //public int DeleteCouponByIds(long[] ids)
        //{
        //    if (ids == null || !ids.Any())
        //        return -1;

        //    string idsStr = String.Join(",", ids);
        //    string sql = "UPDATE [Coupon] SET Active = 0 WHERE CouponId IN(" + idsStr + ")";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}
        //#endregion

        //#region GetCouponsListBySiteId
        //public List<Coupon> GetCouponsListBySiteId(long siteId)
        //{
        //    string sql = "SELECT [CouponId], [CouponName] FROM [Coupon] WHERE [SiteId] = @SiteId ORDER BY [CouponName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@SiteId", siteId));

        //    List<Coupon> list = new List<Coupon>();
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            list.Add(new Coupon()
        //            {
        //                CouponId = DataManager.ToInt(row["CouponId"]),
        //                CouponCode = DataManager.ToString(row["CouponCode"])
        //            });
        //        }
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetDefaultPriceByCouponId
        //public decimal GetDefaultPriceByCouponId(long couponId)
        //{
        //    string sql = "SELECT [DefaultPrice] FROM [Coupon] WHERE [CouponID] = " + couponId;
        //    return DataManager.ToDecimal(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql), 0);
        //}
        //#endregion

        //#region GetCouponsByCouponGroupId
        //public List<Coupon> GetCouponsByCouponGroupId(long id)
        //{
        //    string sql = "SELECT * FROM [Coupon] WHERE [CouponGroupId] = @CouponGroupId AND [Active] = 1 ORDER BY [CouponCode] ASC";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("CouponGroupId", id));

        //    List<Coupon> coupons = new List<Coupon>();
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        coupons.Add(new Coupon(row));
        //    }
        //    return coupons;
        //}
        //#endregion

        //#region GetCouponsPagingByCouponGroupId
        //public List<Coupon> GetCouponsPagingByCouponGroupId(long id, int page, int pageSize, out int totalPages)
        //{
        //    string sql = "SELECT TOP(@PageSize) it.* FROM (SELECT ROW_NUMBER() OVER(ORDER BY [Coupon].CouponCode) AS RowNumber, [Coupon].[CouponId], [Coupon].[CouponCode], (CASE WHEN [OrderId] IS NULL THEN 1 ELSE 0 END) AS [IsAvailable] FROM [Coupon] LEFT JOIN [Order] ON [Order].[CouponId] = [Coupon].[CouponId] WHERE [CouponGroupId] = @CouponGroupId AND [Coupon].[Active] = 1) AS it WHERE it.RowNumber > @Start ORDER BY it.RowNumber;";
        //    sql += "SELECT COUNT([CouponId]) AS TotalRows FROM [Coupon] WHERE [CouponGroupId] = @CouponGroupId AND [Active] = 1;";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@CouponGroupId", id),
        //        new SqlParameter("@PageSize", pageSize),
        //        new SqlParameter("@Start", page * pageSize));

        //    List<Coupon> coupons = new List<Coupon>();
        //    if (ds.Tables.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            coupons.Add(new Coupon()
        //            {
        //                CouponId = DataManager.ToLong(row["CouponId"]),
        //                CouponCode = DataManager.ToString(row["CouponCode"]),
        //                IsAvailable = DataManager.ToBoolean(row["IsAvailable"])
        //            });
        //        }
        //    }
        //    if (ds.Tables.Count > 1)
        //    {
        //        totalPages = (int)Math.Ceiling(DataManager.ToDouble(ds.Tables[1].Rows[0]["TotalRows"]) / pageSize);
        //    }
        //    else
        //    {
        //        totalPages = 1;
        //    }
        //    return coupons;
        //}
        //#endregion

        //#region DeleteCouponGroupItemsByCouponId
        //public void DeleteCouponGroupItemsByCouponGroupId(int id)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM [CouponGroupItem] WHERE [CouponGroupId] = " + id);
        //}
        //#endregion

        //#region AddCouponGroupItem
        //public void AddCouponGroupItem(long couponGroupId, long itemId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "INSERT INTO [CouponGroupItem]([CouponGroupId], [ItemId]) VALUES(@CouponGroupId, @ItemId)",
        //        new SqlParameter("@CouponGroupId", couponGroupId),
        //        new SqlParameter("@ItemId", itemId));
        //}
        //#endregion

        //#region DeleteCouponGroupItemsCategoriesByCouponId
        //public void DeleteCouponGroupItemsCategoriesByCouponId(int id)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM [CouponGroupItemCategory] WHERE [CouponGroupId] = " + id);
        //}
        //#endregion

        //#region AddCouponGroupItemCategory
        //public void AddCouponGroupItemCategory(long couponGroupId, long itemCategoryId)
        //{
        //    string sql = @"IF NOT EXISTS(SELECT * FROM [CouponGroupItemCategory] WHERE [CouponGroupId] = @CouponGroupId AND [CategoryId] = @CategoryId)
        //                    INSERT INTO [CouponGroupItemCategory]([CouponGroupId], [CategoryId]) VALUES(@CouponGroupId, @CategoryId)";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@CouponGroupId", couponGroupId),
        //        new SqlParameter("@CategoryId", itemCategoryId));
        //}
        //#endregion

        //#region DeleteCouponGroupItemsTypesByCouponId
        //public void DeleteCouponGroupItemsTypesByCouponId(int id)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM [CouponGroupItemType] WHERE [CouponGroupId] = " + id);
        //}
        //#endregion

        //#region AddCouponGroupItemType
        //public void AddCouponGroupItemType(long couponGroupId, long itemTypeId)
        //{
        //    string sql = @"IF NOT EXISTS(SELECT * FROM [CouponGroupItemType] WHERE [CouponGroupId] = @CouponGroupId AND [ItemTypeId] = @ItemTypeId)
        //                    INSERT INTO [CouponGroupItemType]([CouponGroupId], [ItemTypeId]) VALUES(@CouponGroupId, @ItemTypeId)";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@CouponGroupId", couponGroupId),
        //        new SqlParameter("@ItemTypeId", itemTypeId));
        //}
        //#endregion

        //#region CheckCouponUsagePeriod
        //public bool CheckCouponUsagePeriod(Coupon coupon)
        //{
        //    string where = string.Empty;
        //    DateTime startDate = DateTime.Today;
        //    DateTime endDate = DateTime.Today;
        //    int timesToUse = coupon.CouponGroup.TimesToUse;
        //    if ((CouponUsagePeriodType)coupon.CouponGroup.UsagePeriodType == CouponUsagePeriodType.ByDay)
        //    {
        //        startDate = DateTime.Today;
        //        endDate = startDate.AddDays(1).AddMilliseconds(-1);
        //        where += " AND ([Order].[OrderDate] BETWEEN @StartDate AND @EndDate)";
        //    }
        //    else if ((CouponUsagePeriodType)coupon.CouponGroup.UsagePeriodType == CouponUsagePeriodType.ByWeek)
        //    {
        //        int dateDiff = DateTime.Today.DayOfWeek == DayOfWeek.Sunday ? ((int)DateTime.Today.DayOfWeek) - 1 : 6;
        //        startDate = DateTime.Today.AddDays(dateDiff * -1);
        //        endDate = startDate.AddDays(7);
        //        where += " AND ([Order].[OrderDate] BETWEEN @StartDate AND @EndDate)";
        //    }
        //    else
        //    {
        //        timesToUse = 1;
        //        startDate = DateTime.Today;
        //        endDate = DateTime.Today;
        //        where += " AND (@StartDate = @EndDate)";
        //    }
        //    string sql = @"WITH OrderCoupon AS (
        //                     SELECT COUNT([OrderId]) AS OrderCouponCount
        //                     FROM [Order]
        //                     WHERE [CouponId] = @CouponId " + where + @"
        //                    ),
        //                    OrderItemCoupon AS (
        //                     SELECT COUNT([OrderItem].[OrderItemId]) AS OrderItemCouponCount FROM [OrderItem]
        //                     INNER JOIN [Order] ON [Order].[OrderId] = [OrderItem].[OrderId]
        //                     WHERE [OrderItem].[ItemCouponId] = @CouponId " + where + @"
        //                    )
        //                    SELECT * FROM OrderCoupon, OrderItemCoupon";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@CouponId", coupon.CouponId),
        //        new SqlParameter("@StartDate", startDate),
        //        new SqlParameter("@EndDate", endDate));
        //    int orderCoupons = 0, orderItemCoupons = 0;
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        DataRow row = ds.Tables[0].Rows[0];
        //        orderCoupons = DataManager.ToInt(row["OrderCouponCount"]);
        //        orderItemCoupons = DataManager.ToInt(row["OrderItemCouponCount"]);

        //    }
        //    return orderCoupons < timesToUse && orderItemCoupons < timesToUse;
        //}
        //#endregion

        //#region GetCouponCodesByCouponGroupId
        //public List<string> GetCouponCodesByCouponGroupId(long couponGroupId)
        //{
        //    string sql = @"SELECT DISTINCT [CouponCode] FROM [Coupon]
        //                   INNER JOIN [Order] ON [Order].[CouponId] <> [Coupon].[CouponId]
        //                   WHERE CouponGroupId = " + couponGroupId + " ORDER BY [CouponCode]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    List<string> couponCodes = new List<string>();
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            couponCodes.Add(DataManager.ToString(row["CouponCode"]));
        //        }
        //    }
        //    return couponCodes;
        //}
        //#endregion
        #endregion

        #region ItemImage
        //public ItemImage GetItemImage(int id)
        //{
        //    string sql = "SELECT * FROM [ItemImage] WHERE [ItemImageId] = " + id;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    ItemImage obj = null;
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new ItemImage(ds.Tables[0].Rows[0]);
        //    }
        //    return obj;
        //}
        #endregion

        #region ItemPostion
        //#region SaveItemPosition
        //public void SaveItemPosition(ItemPosition itemPosition)
        //{
        //    string sql = @"IF(EXISTS(SELECT [ItemId] FROM [ItemPosition] WHERE [ItemId] = @ItemId))
        //                   BEGIN
        //                        UPDATE [ItemPosition] SET [Latitude] = @Latitude, [Longitude] = @Longitude WHERE [ItemId] = @ItemId;
        //                   END
        //                   ELSE
        //                   BEGIN
        //                        INSERT INTO [ItemPosition]([ItemId], [Latitude], [Longitude]) VALUES(@ItemId, @Latitude, @Longitude) SELECT @@IDENTITY;
        //                   END";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemId", itemPosition.ItemId),
        //        new SqlParameter("@Latitude", itemPosition.Latitude),
        //        new SqlParameter("@Longitude", itemPosition.Longitude));
        //}
        //#endregion
        #endregion

        #region Slide Image
        //public int DeleteSlideImages()
        //{
        //    string sql = "DELETE FROM [SlideImage]";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}

        //public int AddSlideImage(SlideImage obj)
        //{
        //    string sql = "INSERT INTO [SlideImage](ImageName, ListNo) VALUES(@ImageName, @ListNo)";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ImageName", obj.ImageName),
        //        new SqlParameter("@ListNo", obj.ListNo));
        //}

        //public List<SlideImage> GetSlideImages()
        //{
        //    List<SlideImage> list = new List<SlideImage>();
        //    string sql = "SELECT * FROM [SlideImage] ORDER BY [ListNo]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new SlideImage(row));
        //    }
        //    return list;
        //}

        //public int DeleteAllSlideImages()
        //{
        //    string sql = "DELETE FROM [SlideImage]";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}

        //public bool AddSlideImages(string[] imageNames, string[] descriptions, string[] linkUrls)
        //{
        //    string sql = "INSERT INTO SlideImage(ImageName, Description, LinkUrl, ListNo) VALUES(@ImageName, @Description, @LinkUrl, @ListNo);";
        //    int length = imageNames.Length;
        //    bool isSuccess = true;
        //    try
        //    {
        //        BeginTransaction();
        //        for (int i = 0, len = imageNames.Length; i < len; i++)
        //        {
        //            isSuccess = SqlHelper.ExecuteNonQuery(Transaction, CommandType.Text, sql,
        //                new SqlParameter("@ImageName", imageNames[i]),
        //                new SqlParameter("@Description", descriptions[i]),
        //                new SqlParameter("@LinkUrl", linkUrls[i]),
        //                new SqlParameter("@ListNo", i)) > 0;
        //            if (!isSuccess)
        //                break;
        //        }
        //        if (isSuccess)
        //        {
        //            CommitTransaction();
        //        }
        //        else
        //        {
        //            RollbackTransaction();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RollbackTransaction();
        //        throw ex;
        //    }
        //    return isSuccess;
        //}
        #endregion

        #region DrivingRange

        //#region GetItem
        //public Item GetDrivingRange(long id)
        //{
        //    Item obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectDrivingRange",
        //        new SqlParameter("@ItemId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new Item(ds.Tables[0].Rows[0]);
        //        obj.ItemLangs = new List<ItemLang>();
        //        if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
        //        {
        //            foreach (DataRow row in ds.Tables[1].Rows)
        //            {
        //                obj.ItemLangs.Add(new ItemLang(row));
        //            }
        //        }
        //        if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
        //        {
        //            obj.itemPosition = new ItemPosition(ds.Tables[2].Rows[0]);
        //        }
        //    }

        //    return obj;
        //}
        //#endregion
        #endregion

        #region Stay Package
        //#region GetStayPackage
        //public Item GetStayPackage(int id)
        //{
        //    Item obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectStayPackage",
        //        new SqlParameter("@ItemId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new Item(ds.Tables[0].Rows[0]);
        //        obj.ItemLangs = new List<ItemLang>();
        //        if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
        //        {
        //            foreach (DataRow row in ds.Tables[1].Rows)
        //            {
        //                obj.ItemLangs.Add(new ItemLang(row));
        //            }
        //        }
        //        if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
        //        {
        //            obj.itemPosition = new ItemPosition(ds.Tables[2].Rows[0]);
        //        }
        //    }

        //    return obj;
        //}
        //#endregion
        #endregion

        #region ItemReview
        //#region GetAllItemReviews
        //public List<ItemReview> GetAllItemReviews()
        //{
        //    List<ItemReview> list = new List<ItemReview>();
        //    string sql = "SELECT * FROM [ItemReviews] ORDER BY [ItemReviewId]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemReview(row));
        //    }
        //    return list;
        //}
        //public List<ItemReview> GetAllItemReviews(jQueryDataTableParamModel param, int langId = 1)
        //{
        //    List<ItemReview> list = new List<ItemReview>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItemReviewsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order),
        //        new SqlParameter("@LangId", langId));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new ItemReview(row));
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);

        //    return list;
        //}
        //#endregion

        //#region GetItemReview
        //public ItemReview GetItemReview(long itemId, int userId)
        //{
        //    ItemReview obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectItemReview",
        //        new SqlParameter("@ItemId", itemId),
        //        new SqlParameter("@UserId", userId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new ItemReview(ds.Tables[0].Rows[0]);
        //    }

        //    return obj;
        //}
        //#endregion

        //#region AddItemReview
        //public int AddItemReview(ItemReview obj)
        //{
        //    string sql = "INSERT INTO [ItemReviews](Rating, Subject, Message, UserId, ItemId, IsApproved) VALUES(@Rating, @Message, @UserId, @ItemId, @IsApproved) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@Rating", obj.Rating),
        //       new SqlParameter("@Subject", obj.Subject),
        //       new SqlParameter("@Message", obj.Message),
        //       new SqlParameter("@UserId", obj.UserId),
        //       new SqlParameter("@ItemId", obj.ItemId),
        //       new SqlParameter("@IsApproved", obj.IsApproved)));
        //}
        //#endregion

        //#region UpdateItemReview
        //public int UpdateItemReview(ItemReview obj)
        //{
        //    string sql = "UPDATE [ItemReviews] SET [Rating] = @Rating, [Subject] = @Subject, [Message] = @Message, [IsApproved] = @IsApproved WHERE [UserId] = @UserId, [ItemId] = @ItemId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@Rating", obj.Rating),
        //       new SqlParameter("@Subject", obj.Subject),
        //       new SqlParameter("@Message", obj.Message),
        //       new SqlParameter("@UserId", obj.UserId),
        //       new SqlParameter("@ItemId", obj.ItemId),
        //       new SqlParameter("@IsApproved", obj.IsApproved));
        //}
        //#endregion

        //#region DeleteItemReview
        //public int DeleteItemReview(long itemId, int userId)
        //{
        //    string sql = "DELETE FROM [ItemReviews] WHERE [ItemId] = @ItemId AND  [UserId] = @UserId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemId", itemId),
        //        new SqlParameter("@UserId", userId));
        //}
        //#endregion

        //#region SaveItemReview
        //public void SaveItemReview(long itemId, int userId, byte rating, string subject, string message, out int averageRating, out int reviewNumber)
        //{
        //    string sql = "INSERT INTO [ItemReview]([Rating], [Subject], [Message], [UserId], [ItemId], [IsApproved]) VALUES(@Rating, @Subject, @Message, @UserId, @ItemId, 0)";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemId", itemId),
        //        new SqlParameter("@UserId", userId),
        //        new SqlParameter("@Rating", rating),
        //        new SqlParameter("@Subject", subject),
        //        new SqlParameter("@Message", message));

        //    averageRating = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT AVG([Rating]) FROM [ItemReview] WHERE [IsApproved] = 1 AND [ItemId] = @ItemId",
        //        new SqlParameter("@ItemId", itemId)), 0);

        //    reviewNumber = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT COUNT([ItemReviewId]) FROM [ItemReview] WHERE [IsApproved] = 1 AND [ItemId] = @ItemId",
        //        new SqlParameter("@ItemId", itemId)), 0);
        //}
        //#endregion

        //#region GetItemReviewsByItemId
        //public List<ItemReview> GetItemReviewsByItemId(long itemId)
        //{
        //    List<ItemReview> reviews = new List<ItemReview>();
        //    string sql = @"SELECT TOP(3) [ItemReview].*, [Users].[Firstname] AS [ReviewerName], [City].[CityName] AS [ReviewerCityName] FROM [ItemReview]
        //                    INNER JOIN [Users] ON [Users].[UserId] = [ItemReview].[UserId]
        //                    LEFT JOIN [City] ON [City].[CityId] = [Users].[CityId] 
        //                    WHERE [IsApproved] = 1 AND [ItemId] = " + itemId +
        //                    " ORDER BY [UpdatedDate] DESC";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        reviews.Add(new ItemReview(row));
        //    }
        //    return reviews;
        //}
        //#endregion

        //#region DeleteItemReview
        //public void DeleteItemReview(IEnumerable<int> ids)
        //{
        //    if (!ids.Any())
        //        return;

        //    string sql = "DELETE FROM [ItemReview] WHERE [ItemReviewId] IN(" + string.Join(",", ids) + ")";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}
        //#endregion

        //#region UpdateItemReviewApproval
        //public void UpdateItemReviewApproval(int id, bool isApproved)
        //{
        //    string sql = "UPDATE [ItemReview] SET [IsApproved] = @IsApproved WHERE [ItemReviewId] = @ItemReviewId";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemReviewId", id),
        //        new SqlParameter("@IsApproved", isApproved));
        //}
        //#endregion
        #endregion

        #region Order
        //#region GetAllOrders
        //public List<Order> GetAllOrders()
        //{
        //    List<Order> list = new List<Order>();
        //    string sql = "SELECT * FROM [Order] ORDER BY [OrderName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Order(row));
        //    }
        //    return list;
        //}
        //public List<Order> GetAllOrders(jQueryDataTableParamModel param, int langId = 1)
        //{
        //    List<Order> list = new List<Order>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectOrdersList",
        //        new SqlParameter("@LangId", langId));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Order(row));
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetOrder
        //public Order GetOrder(int id)
        //{
        //    Order obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectOrder",
        //        new SqlParameter("@OrderId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new Order(ds.Tables[0].Rows[0]);
        //        obj.OrderItems = new List<OrderItem>();
        //    }

        //    if (ds.Tables[0].Rows.Count > 1)
        //    {
        //        foreach (DataRow row in ds.Tables[1].Rows)
        //        {
        //            obj.OrderItems.Add(new OrderItem(row));
        //        }
        //    }

        //    return obj;
        //}
        //#endregion

        //#region DeleteOrder
        //public int DeleteOrder(int id)
        //{
        //    string sql = "DELETE FROM [Order] WHERE [OrderId] = @OrderId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@OrderId", id));
        //}
        //#endregion

        //#region SaveOrder
        //public long SaveOrder(Order obj)
        //{
        //    string sql = @"IF(EXISTS(SELECT [OrderId] FROM [Order] WHERE [OrderId] = @OrderId))
        //                    BEGIN
        //                        UPDATE [Order] SET [TransactionId] = @TransactionId, [OrderNumber] = @OrderNumber, [CouponId] = @CouponId, [CustomerId] = @CustomerId, [OrderDate] = @OrderDate, [AddressId] = @AddressId, [CreatedDate] = @CreatedDate, [ModifiedDate] = @ModifiedDate, [Active] = @Active, [IsNoticed] = @IsNoticed, [PaymentType] = @PaymentType, [PaymentStatus] = @PaymentStatus, [RequestId] = @RequestId, [ReductionRate] = @ReductionRate, [ReductionType] = @ReductionType WHERE [OrderId] = @OrderId;
        //                        SELECT @OrderId;
        //                    END
        //                    ELSE
        //                    BEGIN
        //                        INSERT INTO [Order]([TransactionId], [OrderNumber], [CouponId], [CustomerId], [OrderDate], [AddressId], [CreatedDate], [ModifiedDate], [Active], [IsNoticed], [PaymentType], [PaymentStatus], [RequestId], [ReductionRate], [ReductionType])
        //                        VALUES(@TransactionId, @OrderNumber, @CouponId, @CustomerId, @OrderDate, @AddressId, @CreatedDate, @ModifiedDate, @Active, @IsNoticed, @PaymentType, @PaymentStatus, @RequestId, @ReductionRate, @ReductionType) SELECT @@IDENTITY;
        //                    END";
        //    obj.OrderId = DataManager.ToLong(SqlHelper.ExecuteScalar(Transaction, CommandType.Text, sql,
        //        new SqlParameter("@OrderId", obj.OrderId),
        //        new SqlParameter("@TransactionId", obj.TransactionId),
        //        new SqlParameter("@OrderNumber", obj.OrderNumber),
        //        new SqlParameter("@CouponId", obj.CouponId),
        //        new SqlParameter("@ReductionRate", obj.ReductionRate),
        //        new SqlParameter("@ReductionType", obj.ReductionType),
        //        new SqlParameter("@CustomerId", obj.CustomerId),
        //        new SqlParameter("@OrderDate", obj.OrderDate),
        //        new SqlParameter("@AddressId", obj.AddressId),
        //        new SqlParameter("@CreatedDate", obj.CreatedDate),
        //        new SqlParameter("@ModifiedDate", obj.ModifiedDate),
        //        new SqlParameter("@Active", obj.Active),
        //        new SqlParameter("@IsNoticed", obj.IsNoticed),
        //        new SqlParameter("@PaymentType", obj.PaymentType),
        //        new SqlParameter("@PaymentStatus", obj.PaymentStatus),
        //        new SqlParameter("@RequestId", obj.RequestId)), -1);

        //    if (obj.OrderId > 0 && obj.OrderItems != null && obj.OrderItems.Any())
        //    {
        //        sql = "DELETE FROM [OrderItem] WHERE [OrderId] = @OrderId";
        //        SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //            new SqlParameter("@OrderId", obj.OrderId));

        //        sql = @"IF(EXISTS(SELECT [OrderItemId] FROM [OrderItem] WHERE [OrderItemId] = @OrderItemId))
        //                BEGIN
        //                    UPDATE [OrderItem] SET [ItemId] = @ItemId, [OrderId] = @OrderId, [SiteId] = @SiteId, [ItemName] = @ItemName, [Quantity] = @Quantity, [UnitPrice] = @UnitPrice, [ReserveDate] = @ReserveDate, [ShippingCost] = @ShippingCost, [VatId] = @VatId, [VatRate] = @VatRate, [ItemCouponId] = @ItemCouponId, [ReductionRate] = @ReductionRate, [ReductionType] = @ReductionType WHERE [OrderItemId] = @OrderItemId;
        //                    SELECT @OrderItemId;
        //                END
        //                ELSE
        //                BEGIN
        //                    INSERT INTO [OrderItem]([OrderId], [ItemId], [SiteId], [ItemName], [Quantity], [UnitPrice], [ReserveDate], [ShippingCost], [VatId], [VatRate], [ItemCouponId], [ReductionRate], [ReductionType]) VALUES(@OrderId, @ItemId, @SiteId, @ItemName, @Quantity, @UnitPrice, @ReserveDate, @ShippingCost, @VatId, @VatRate, @ItemCouponId, @ReductionRate, @ReductionType) SELECT @@IDENTITY;
        //                END";
        //        foreach (OrderItem i in obj.OrderItems)
        //        {
        //            i.OrderId = obj.OrderId;
        //            i.OrderItemId = DataManager.ToLong(SqlHelper.ExecuteScalar(Transaction, CommandType.Text, sql,
        //                new SqlParameter("@OrderItemId", i.OrderItemId),
        //                new SqlParameter("@OrderId", obj.OrderId),
        //                new SqlParameter("@ItemId", i.ItemId),
        //                new SqlParameter("@SiteId", i.SiteId),
        //                new SqlParameter("@ItemName", i.ItemName),
        //                new SqlParameter("@Quantity", i.Quantity),
        //                new SqlParameter("@UnitPrice", i.UnitPrice),
        //                new SqlParameter("@ReserveDate", i.ReserveDate),
        //                new SqlParameter("@ShippingCost", i.ShippingCost),
        //                new SqlParameter("@VatId", i.VatId),
        //                new SqlParameter("@VatRate", i.VatRate),
        //                new SqlParameter("@ItemCouponId", i.ItemCouponId),
        //                new SqlParameter("@ReductionRate", i.ReductionRate),
        //                new SqlParameter("@ReductionType", i.ReductionType)), -1);
        //        }

        //        // Collect Site Email.
        //        var siteIds = obj.OrderItems.Select(it => it.SiteId).Distinct();
        //        if (siteIds != null && siteIds.Any())
        //        {
        //            Dictionary<int, string> siteEmails = GetSiteEmailsBySiteIds(siteIds);
        //            foreach (var entry in siteEmails)
        //            {
        //                obj.OrderItems.Where(it => it.SiteId == entry.Key).ToList().ForEach(it => it.SiteEmail = entry.Value);
        //            }
        //        }
        //    }

        //    return obj.OrderId;
        //}

        //private Dictionary<int, string> GetSiteEmailsBySiteIds(IEnumerable<long> siteIds)
        //{
        //    Dictionary<int, string> siteEmails = new Dictionary<int, string>();
        //    if (siteIds != null && siteIds.Any())
        //    {
        //        string sql = "SELECT [SiteId], [Email] From [Site] WHERE [SiteId] IN(" + String.Join(",", siteIds) + ")";
        //        DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            foreach (DataRow row in ds.Tables[0].Rows)
        //            {
        //                siteEmails.Add(DataManager.ToInt(row["SiteId"]), DataManager.ToString(row["Email"]));
        //            }
        //        }
        //    }
        //    return siteEmails;
        //}
        //#endregion

        //#region GetOrderByOrderId
        //public Order GetOrderByOrderId(long orderId)
        //{
        //    Order order = null;
        //    OrderItem orderItem = null;
        //    string sql = @"SELECT [Order].*, [Users].[Firstname], [Users].[Middlename], [Users].[Lastname], 
        //                    [Address].[Firstname] AS ShippingFirstname, [Address].[Lastname] AS ShippingLastname
        //                    FROM [Order]
        //                    LEFT JOIN [Users] ON [Users].[UserId] = [Order].[CustomerId]
        //                    LEFT JOIN [Address] ON [Address].[AddressId] = [Order].[AddressId]
        //                    WHERE [OrderId] = @OrderId;
        //                    SELECT [OrderItem].*, [Site].[Email], ISNULL([ItemCategory].[CategoryName], '-') AS [CategoryName],
        //                    [Tax].[TaxPercent] AS [VatRate], [ItemConversionTracking].[TrackingCodePurchase] AS [ConversionTrackingCode]
        //                    FROM [OrderItem] 
        //                    LEFT JOIN [Site] ON [Site].[SiteId] = [OrderItem].[SiteId]
        //                    LEFT JOIN [Item] ON [Item].[ItemId] = [OrderItem].[ItemId]
        //                    LEFT JOIN [ItemConversionTracking] ON [ItemConversionTracking].[ItemId] = [OrderItem].[ItemId]
        //                    LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]
        //                    LEFT JOIN [Tax] ON [Tax].[TaxId] = [OrderItem].[VatId]
        //                    WHERE [OrderId] = @OrderId ORDER BY [ItemName];";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //                new SqlParameter("@OrderId", orderId));
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        order = new Order(row);
        //        order.BuyerName = DataManager.ToString(row["Firstname"]) + " " + DataManager.ToString(row["Lastname"]);
        //        order.ShippingReceiverName = DataManager.ToString(row["ShippingFirstname"]) + " " + DataManager.ToString(row["ShippingLastname"]);
        //        order.OrderItems = new List<OrderItem>();

        //        foreach (DataRow rowItem in ds.Tables[1].Rows)
        //        {
        //            orderItem = new OrderItem(rowItem);
        //            orderItem.Item = GetItem(orderItem.ItemId);
        //            orderItem.SiteEmail = DataManager.ToString(rowItem["Email"]);
        //            orderItem.CategoryName = DataManager.ToString(rowItem["CategoryName"]);
        //            orderItem.VatRate = DataManager.ToFloat(rowItem["VatRate"]);
        //            orderItem.ConversionTrackingCode = DataManager.ToString(rowItem["ConversionTrackingCode"]);
        //            order.OrderItems.Add(orderItem);
        //        }
        //    }

        //    if (orderItem != null && !string.IsNullOrEmpty(orderItem.Item.DlgCardStyleId.ToString()))
        //    {
        //        var listStyleimage = GetItemDlgCardByItemTypeId(6, 0, null, null, null, null, (Convert.ToInt32(orderItem.ItemId)));

        //        foreach (var data in listStyleimage)
        //        {

        //            orderItem.Item.ItemImages.Add(new ItemImage
        //            {
        //                ImageName = data.ImageName,
        //                BaseName = data.ImageName
        //            });

        //        }
        //    }
        //    return order;
        //}
        //#endregion

        //#region GetOrderByTransactionId
        //public Order GetOrderByTransactionId(string transactionId, string orderNumber = "")
        //{
        //    Order order = null;
        //    string sql = "SELECT * FROM [Order] WHERE [TransactionId] = '" + transactionId + "'";
        //    if (!String.IsNullOrEmpty(orderNumber.Trim()))
        //    {
        //        sql += " AND [OrderNumber] = '" + orderNumber + "'";
        //    }
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        order = new Order(ds.Tables[0].Rows[0]);
        //    }
        //    else
        //    {
        //        order = new Order()
        //        {
        //            OrderId = 0,
        //            OrderNumber = string.Empty,
        //            TransactionId = string.Empty,
        //            OrderDate = DateTime.Now,
        //            CreatedDate = DateTime.Now,
        //            ModifiedDate = DateTime.Now,
        //            Active = true
        //        };
        //    }
        //    return order;
        //}
        //#endregion

        //#region GetTransactionId
        //public string GetTransactionId()
        //{
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, "SELECT COUNT(*) + 1 AS LatestTransactionId FROM [Order]"), 0).ToString("000000");
        //}
        //#endregion

        //#region UpdateOrderPaymentStatus
        //public int UpdateOrderPaymentStatus(string OrderId, bool IsSucess)
        //{

        //    var update = " N'' ";
        //    if (IsSucess)
        //    {
        //        update = " N'success' ";
        //    }
        //    else
        //    {
        //        update = " N'pending'";
        //    }
        //    string sql = "Update [Order] SET [PaymentStatus] = " + update + " Where [OrderId] = " + OrderId;
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}

        //#endregion

        //#region GetTotalOrdersByUserId
        ///// <summary>
        ///// Get total orders count by User ID.
        ///// </summary>
        ///// <param name="userId">User ID that you want to fetch total orders number.</param>
        ///// <returns>total orders number</returns>
        //public int GetTotalOrdersByUserId(int userId)
        //{
        //    string sql = "SELECT COUNT(OrderId) FROM [Order] WHERE [Active] = 1 AND [IsNoticed] = 0 AND [CustomerId] = " + userId;
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql));
        //}
        //#endregion

        //#region GetUserNotifications
        //public void GetUserNotifications(long userId, out int totalOrders, out int totalMessages, out int totalReferals)
        //{
        //    string sql = @"SELECT COUNT(OrderId) AS NotificationCount FROM [Order] WHERE [Active] = 1 AND [IsNoticed] = 0 AND [CustomerId] = @UserId;";
        //    sql += @"SELECT COUNT(*) AS NotificationCount FROM [dbo].[UserMessage] WHERE [ToUserId] = @UserId AND [ReadDate] Is null;";
        //    sql += @"SELECT COUNT([SponsorEmailId]) AS NotificationCount FROM [SponsorEmail] WHERE [Active] = 1 AND [FromUserId] = @UserId;";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@UserId", userId));

        //    totalOrders = DataManager.ToInt(ds.Tables[0].Rows[0]["NotificationCount"]);
        //    totalMessages = DataManager.ToInt(ds.Tables[1].Rows[0]["NotificationCount"]);
        //    totalReferals = DataManager.ToInt(ds.Tables[2].Rows[0]["NotificationCount"]);
        //}
        //#endregion

        //#region GetTotalOrderProfit
        //public decimal GetTotalOrderProfit(DateTime from, DateTime to)
        //{
        //    decimal total = 0;
        //    string sql = @"WITH [PreCalculatedOrderItems] AS (
        //                    SELECT [OrderItemId], [OrderItem].[OrderId], [OrderDate], [UnitPrice], [Quantity], [ShippingCost], 
        //                    (CASE WHEN [OrderItem].[ReductionType] = 0 THEN [OrderItem].[ReductionRate] ELSE (([UnitPrice] * [OrderItem].[ReductionRate]) / 100) END) AS Discount
        //                    FROM  [OrderItem]
        //                    INNER JOIN [Order] ON [Order].[OrderId] = [OrderItem].[OrderId] AND [Order].[Active] = 1),
        //                    [CalculatedOrderItems] AS (
        //                    SELECT [OrderId], [OrderDate], CONVERT(decimal(10, 2), (UnitPrice - Discount + ShippingCost) * Quantity) AS TotalTTC
        //                    FROM  [PreCalculatedOrderItems])
        //                    SELECT ISNULL(SUM(TotalTTC), 0) AS TotalPrice FROM [CalculatedOrderItems]
        //                    WHERE [OrderDate] BETWEEN @From AND DATEADD(DAY, 1, @To)";
        //    total = DataManager.ToDecimal(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@From", from),
        //        new SqlParameter("@To", to)));
        //    return total;
        //}
        //#endregion

        //#region SetLydiaPaymentStatus
        //public void SetLydiaPaymentStatus(long orderId, string paymentStatus)
        //{
        //    string sql = @"UPDATE [Order] SET [PaymentStatus] = @PaymentStatus, [ModifiedDate] = @ModifiedDate WHERE [OrderId] = @OrderId";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@OrderId", orderId),
        //        new SqlParameter("@PaymentStatus", paymentStatus),
        //        new SqlParameter("@ModifiedDate", DateTime.Now));
        //}
        //public void SetLydiaPaymentStatus(string requestId, string paymentStatus)
        //{
        //    string sql = @"UPDATE [Order] SET [PaymentStatus] = @PaymentStatus, [ModifiedDate] = @ModifiedDate WHERE [RequestId] = @RequestId AND [PaymentType] IN('lydia', 'creditcard')";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@RequestId", requestId),
        //        new SqlParameter("@PaymentStatus", paymentStatus),
        //        new SqlParameter("@ModifiedDate", DateTime.Now));
        //}
        //#endregion

        //#region UpdateOrder
        //public int UpdateOrder(long orderId, string paymentStatus)
        //{
        //    string sql = "UPDATE [Order] SET [PaymentStatus] = @PaymentStatus WHERE [OrderId] = @OrderId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@OrderId", orderId),
        //        new SqlParameter("@PaymentStatus", paymentStatus));
        //}
        //#endregion

        //#region UpdateOrderItem
        //public int UpdateOrderItem(long orderItemId, string itemName, decimal unitPrice, int quantity)
        //{
        //    string sql = "UPDATE [OrderItem] SET [ItemName] = @ItemName, [UnitPrice] = @UnitPrice, [Quantity] = @Quantity WHERE [OrderItemId] = @OrderItemId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@OrderItemId", orderItemId),
        //        new SqlParameter("@ItemName", itemName),
        //        new SqlParameter("@UnitPrice", unitPrice),
        //        new SqlParameter("@Quantity", quantity));
        //}
        //#endregion
        #endregion

        #region TeeSheet

        //#region GetTeeSheetsByItemId
        //public List<TeeSheet> GetTeeSheetsByItemId(long id)
        //{
        //    string sql = @"SELECT [TeeSheet].* FROM [TeeSheet]
        //                 WHERE [TeeSheet].[ItemId] = @ItemId AND [TeeSheet].[TeeSheetDate] >= @MinDate
        //                 ORDER BY [TeeSheet].[TeeSheetDate], [TeeSheet].[FromTime]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemId", id),
        //        new SqlParameter("@MinDate", DateTime.Today));

        //    List<TeeSheet> list = new List<TeeSheet>();
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new TeeSheet(row));
        //    }
        //    return list;
        //}
        //#endregion

        //#region DeleteTeeSheetsByItemId
        //public int DeleteTeeSheetsByItemId(long id)
        //{
        //    string sql = @"DELETE FROM [TeeSheet] WHERE [ItemId] = @ItemId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemId", id));
        //}
        //public int DeleteTeeSheetsByItemId(long id, DateTime from, DateTime to)
        //{
        //    string sql = @"DELETE FROM [TeeSheet] WHERE [ItemId] = @ItemId AND [TeeSheetDate] BETWEEN @FromDate AND @ToDate";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemId", id),
        //        new SqlParameter("@FromDate", from),
        //        new SqlParameter("@ToDate", to));
        //}
        //#endregion

        //#region AddTeeSheets
        //public void SaveTeeSheets(IEnumerable<TeeSheet> teeSheets)
        //{
        //    string sql = string.Empty;
        //    foreach (TeeSheet teeSheet in teeSheets)
        //    {
        //        sql = @"IF EXISTS(SELECT [TeeSheetId] FROM [TeeSheet] WHERE [ItemId] = @ItemId AND [TeeSheetDate] = @TeeSheetDate AND [FromTime] = @FromTime AND [ToTime] = @ToTime)
        //                BEGIN
        //                    UPDATE [TeeSheet] SET [ItemId] = @ItemId, [CourseId] = @CourseId, [TeeSheetDay] = @TeeSheetDay, [TeeSheetDate] = @TeeSheetDate, [FromTime] = @FromTime, [ToTime] = @ToTime, [Price] = @Price, [Discount] = @Discount, [PreBookingDays] = @PreBookingDays
        //                    WHERE [ItemId] = @ItemId AND [TeeSheetDate] = @TeeSheetDate AND [FromTime] = @FromTime AND [ToTime] = @ToTime
        //                END
        //                ELSE
        //                BEGIN
        //                    INSERT INTO [TeeSheet]([ItemId], [CourseId], [TeeSheetDay], [TeeSheetDate], [FromTime], [ToTime], [Price], [Discount], [PreBookingDays]) VALUES(@ItemId, @CourseId, @TeeSheetDay, @TeeSheetDate, @FromTime, @ToTime, @Price, @Discount, @PreBookingDays)
        //                END";
        //        SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //            new SqlParameter("@ItemId", teeSheet.ItemId),
        //            new SqlParameter("@CourseId", teeSheet.CourseId),
        //            new SqlParameter("@TeeSheetDay", teeSheet.TeeSheetDay),
        //            new SqlParameter("@TeeSheetDate", teeSheet.TeeSheetDate),
        //            new SqlParameter("@FromTime", teeSheet.FromTime),
        //            new SqlParameter("@ToTime", teeSheet.ToTime),
        //            new SqlParameter("@Price", teeSheet.Price),
        //            new SqlParameter("@Discount", teeSheet.Discount),
        //            new SqlParameter("@PreBookingDays", teeSheet.PreBookingDays));
        //    }
        //}
        //#endregion

        //#region GetTeeSheetByTeeSheetId
        //public TeeSheet GetTeeSheetByTeeSheetId(long teeSheetId)
        //{
        //    TeeSheet teeSheet = null;
        //    string sql = @"SELECT [TeeSheet].* FROM [TeeSheet]
        //                 WHERE [TeeSheetId] = @TeeSheetId";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@TeeSheetId", teeSheetId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        teeSheet = new TeeSheet(ds.Tables[0].Rows[0]);
        //    }
        //    return teeSheet;
        //}
        //#endregion

        //#region GetTeeSheetCheapestPrice
        //public decimal GetTeeSheetCheapestPrice(long itemId)
        //{
        //    string sql = @"SELECT MIN([Discount]) AS [CheapestPrice] FROM [TeeSheet] WHERE [ItemId] = @ItemId AND [Price] > 0 AND [Discount] > 0 AND [TeeSheetDate] >= GETDATE()";
        //    return DataManager.ToDecimal(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ItemId", itemId)));
        //}
        //#endregion

        #endregion

        //#region DLG Card

        //#region Add DLG Card

        //public int AddDLgCard(DLGCard obj)
        //{
        //    string sql = @"INSERT INTO [DLGCard]([CardId], [Active], [UserId]) 
        //                VALUES(@CardId, @Active, @UserId)";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@CardId", obj.CardId),
        //       new SqlParameter("@Active", obj.Active),
        //       new SqlParameter("@UserId", obj.UserId)));
        //}

        //#endregion

        //#region UpdateItem

        //public int UpdateItem(DLGCard obj)
        //{
        //    string sql = @"UPDATE [DLGCard] SET
        //                  [CardId] = @CategoryId
        //                  ,[Amount] = @ItemTypeId
        //                  ,[Qualtity] = @ItemCode
        //                  ,[UpdateDate] = @UpdateDate
        //                  ,[Active] = @Active
        //                  ,[UserId] = @UserId
        //                    WHERE [CardId] = @CardId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@CardId", obj.CardId),
        //       new SqlParameter("@Amount", obj.Amount),
        //       new SqlParameter("@Qualtity", obj.Qualtity),
        //       new SqlParameter("@UpdateDate", obj.UpdateDate),
        //       new SqlParameter("@Active", obj.Active),
        //       new SqlParameter("@UserId", obj.UserId));
        //}

        //#endregion

        //#endregion

        //#region DLG Card Style

        //public long AddCardImage(DLGCardStyle obj)
        //{
        //    string sql = @"INSERT INTO [DLGCardStyle](CardId, ImageName, ListNo, BaseName, FileExtension) VALUES(@CardId, @ImageName,
        //       (SELECT COUNT(*) FROM [DLGCardStyle] WHERE [CardId] = @CardId)
        //    , @BaseName, @FileExtension) SELECT @@IDENTITY";

        //    return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@CardId", obj.CardId),
        //        new SqlParameter("@ImageName", obj.ImageName),
        //        new SqlParameter("@BaseName", obj.BaseName),
        //        new SqlParameter("@FileExtension", obj.FileExtension)), -1);
        //}

        //public int DeleteCardByCardId(int id)
        //{
        //    string sql = "DELETE FROM [DLGCardStyle] WHERE [CardId] = " + id;
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}

        //public long SaveCardImage(DLGCardStyle obj)
        //{
        //    return DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, "SaveCardImage",
        //        new SqlParameter("@CardId", obj.CardId),
        //        new SqlParameter("@ImageName", obj.ImageName),
        //        new SqlParameter("@BaseName", obj.BaseName),
        //        new SqlParameter("@ListNo", obj.ListNo),
        //        new SqlParameter("@FileExtension", obj.FileExtension)), -1);
        //}

        //public IQueryable<DLGCardStyle> GetCardByCardId(int id)
        //{
        //    using (var db = GetModels())
        //    {
        //        return db.DLGCardStyles.Where(it => it.CardId == id);
        //    }
        //}

        //public long DeleteCardImage(int id)
        //{
        //    string sql = "DELETE FROM [DLGCardStyle] WHERE [CardStyleId] = " + id;
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}

        //public DLGCardStyle GetCardImage(long id)
        //{
        //    DLGCardStyle obj = new DLGCardStyle();
        //    using (var db = GetModels())
        //    {
        //        db.DLGCardStyles.Where(it => it.CardStyleId == id).ToList().ForEach(it => obj = it);
        //    }
        //    return obj;
        //}

        //#endregion

        #region Address
        //#region AddAddress
        //public int AddAddress(Address obj)
        //{
        //    string sql = @"INSERT INTO [Address](
        //   [Street]
        //   ,[CityId]
        //   ,[PostalCode]
        //   ,[CountryId]
        //   ,[Phone]

        //   ,[Remarks]
        //    ,[UserId],[AddressName],[Floor],[Firstname],[Lastname]
        //   ) VALUES(@Street
        //   ,@CityId
        //   ,@PostalCode
        //   ,@CountryId
        //   ,@Phone

        //   ,@Remarks 
        //   ,@UserId,@AddressName,@Floor,@Firstname,@Lastname
        //   ) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@Street", obj.Street),
        //        new SqlParameter("@CityId", obj.CityId),
        //        new SqlParameter("@PostalCode", obj.PostalCode),
        //        new SqlParameter("@CountryId", obj.CountryId),
        //        new SqlParameter("@Phone", obj.Phone),
        //        new SqlParameter("@Firstname", obj.Firstname),
        //        new SqlParameter("@Lastname", obj.Lastname),
        //        new SqlParameter("@AddressName", obj.AddressName),
        //        new SqlParameter("@Floor", obj.Floor),
        //        new SqlParameter("@Remarks", obj.Remarks),
        //        new SqlParameter("@UserId", obj.UserId)), -1);
        //}
        //#endregion

        //#region UpdateAddress
        //public int UpdateAddress(Address obj)
        //{
        //    string sql = @"UPDATE [Address]
        //                   SET 
        //                      [Address] = @Address
        //                      ,[Street] = @Street
        //                      ,[CityId] = @CityId
        //                      ,[PostalCode] = @PostalCode
        //                      ,[CountryId] = @CountryId
        //                      ,[Phone] = @Phone
        //                      ,[MobilePhone] = @MobilePhone
        //                      ,[Remarks] = @Remarks
        //                      ,[AddressName] = @AddressName
        //                      ,[Floor] = @Floor
        //                      ,[Firstname] = @Firstname
        //                      ,[Lastname] = @Lastname
        //                    WHERE [AddressId] = @AddressId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@AddressId", obj.AddressId),
        //        new SqlParameter("@Address", obj.Address1),
        //        new SqlParameter("@Street", obj.Street),
        //        new SqlParameter("@CityId", obj.CityId),
        //        new SqlParameter("@PostalCode", obj.PostalCode),
        //        new SqlParameter("@CountryId", obj.CountryId),
        //        new SqlParameter("@Phone", obj.Phone),
        //        new SqlParameter("@MobilePhone", obj.MobilePhone),
        //        new SqlParameter("@Remarks", obj.Remarks),
        //        new SqlParameter("@Floor", obj.Floor),
        //        new SqlParameter("@AddressName", obj.AddressName),
        //        new SqlParameter("@Firstname", obj.Firstname),
        //        new SqlParameter("@Lastname", obj.Lastname),
        //        new SqlParameter("@Userid", obj.UserId)
        //        );
        //}
        //#endregion

        //#region DeleteAddress
        //public int DeleteAddress(long id)
        //{
        //    string sql = "DELETE FROM [Address] WHERE [AddressId] = @AddressId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@AddressId", id));
        //}

        //#region GetAddress
        //public Address GetAddress(long id)
        //{
        //    string sql = @"SELECT TOP(1) [Address].*, [City].[CityName], [Country].[CountryName] FROM [Address]
        //                    LEFT OUTER JOIN [City] ON [City].[CityId] = [Address].[CityId]
        //                    LEFT OUTER JOIN [Country] ON [Country].[CountryId] = [Address].[CountryId]
        //                    WHERE Addressid = @Addressid";
        //    Address obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@Addressid", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new Address(ds.Tables[0].Rows[0]);
        //    }

        //    return obj;
        //}
        //#endregion

        //#region GetAddressByUserID
        //public List<Address> GetAddressByUserID(long userId)
        //{
        //    List<Address> list = new List<Address>();
        //    string sql = @"SELECT Address.*, Country.CountryName, City.CityName 
        //                            FROM Address 
        //                            LEFT JOIN City on Address.CityId = City.CityId
        //                            LEFT JOIN Country on Address.CountryId = Country.CountryId  WHERE Userid = @UserId";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@UserId", userId));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow item in ds.Tables[0].Rows)
        //        {
        //            list.Add(new Address(item));
        //        }

        //    }

        //    return list;
        //}
        //#endregion

        //public List<Country> GetAddressBy()
        //{
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectCountriesList");
        //    List<Country> list = new List<Country>();
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Country(row));
        //    }
        //    return list;
        //}
        //#endregion

        //#region GetAddressByAddressId
        //public Address GetAddressByAddressId(long addressId)
        //{
        //    string sql = "SELECT * FROM [Address] WHERE [AddressId] = " + addressId;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    Address address = null;
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        address = new Address(ds.Tables[0].Rows[0]);
        //    }
        //    return address;
        //}
        //#endregion

        //#region SaveAddress
        //public void SaveAddress(Address obj)
        //{
        //    string sql = @"IF EXISTS(SELECT [AddressId] FROM [Address] WHERE [AddressId] = @AddressId)
        //                    BEGIN
        //                        UPDATE [Address] SET [Firstname] = @Firstname,[Lastname] = @Lastname,[TitleId] = @TitleId,[Complement] = @Complement,[AddressName] = @AddressName,[Address] = @Address,[Street] = @Street,[CityId] = @CityId,[PostalCode] = @PostalCode,[CountryId] = @CountryId,[Country] = @Country,[Phone] = @Phone,[PhoneCountryCode] = @PhoneCountryCode,[MobilePhone] = @MobilePhone,[MobilePhoneCountryCode] = @MobilePhoneCountryCode,[Floor] = @Floor,[Remarks] = @Remarks,[UserId] = @UserId
        //                        WHERE [AddressId] = @AddressId
        //                    END
        //                    ELSE
        //                    BEGIN
        //                        INSERT INTO [Address] ([TitleId],[Firstname],[Lastname],[Complement],[AddressName],[Address],[Street],[CityId],[PostalCode],[CountryId],[Country],[Phone],[PhoneCountryCode],[MobilePhone],[MobilePhoneCountryCode],[Floor],[Remarks],[UserId])
        //                        VALUES (@TitleId, @Firstname, @Lastname, @Complement, @AddressName, @Address, @Street, @CityId, @PostalCode, @CountryId, @Country, @Phone, @PhoneCountryCode, @MobilePhone, @MobilePhoneCountryCode, @Floor, @Remarks, @UserId)
        //                    END";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@AddressId", obj.AddressId),
        //        new SqlParameter("@Firstname", obj.Firstname),
        //        new SqlParameter("@TitleId", obj.TitleId),
        //        new SqlParameter("@Lastname", obj.Lastname),
        //        new SqlParameter("@Complement", obj.Complement),
        //        new SqlParameter("@AddressName", obj.AddressName),
        //        new SqlParameter("@Address", obj.Address1),
        //        new SqlParameter("@Street", obj.Street),
        //        new SqlParameter("@CityId", obj.CityId),
        //        new SqlParameter("@PostalCode", obj.PostalCode),
        //        new SqlParameter("@CountryId", obj.CountryId),
        //        new SqlParameter("@Country", obj.Country),
        //        new SqlParameter("@Phone", obj.Phone),
        //        new SqlParameter("@PhoneCountryCode", obj.PhoneCountryCode),
        //        new SqlParameter("@MobilePhone", obj.MobilePhone),
        //        new SqlParameter("@MobilePhoneCountryCode", obj.MobilePhoneCountryCode),
        //        new SqlParameter("@Floor", obj.Floor),
        //        new SqlParameter("@Remarks", obj.Remarks),
        //        new SqlParameter("@UserId", obj.UserId));
        //}
        //#endregion

        //#region GetCountryNameByCountryId
        //public string GetCountryNameByCountryId(int countryId)
        //{
        //    string sql = @"SELECT TOP(1) [CountryName] FROM [Country]
        //                    WHERE [CountryId] = @CountryId";
        //    return DataManager.ToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("CountryId", countryId)), String.Empty);
        //}
        //#endregion

        #region GetCountryIdByCountryName
        public int GetCountryIdByCountryName(string countryName)
        {
            string sql = @"SELECT TOP(1) [CountryId] FROM [Country]
                            WHERE [CountryName] = @CountryName";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("CountryName", countryName)), 1);
        }
        #endregion

        #region GetCountryIdLookupTable
        public Dictionary<string, int> GetCountryIdLookupTable()
        {
            Dictionary<string, int> table = new Dictionary<string, int>();
            string sql = "SELECT DISTINCT CountryName, CountryId FROM Country ORDER BY CountryId";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                table.Add(DataManager.ToString(row["CountryName"]), DataManager.ToInt(row["CountryId"]));
            }
            return table;
        }
        #endregion
        #endregion

        #region State

        #region GetStatesDropDownListData
        public List<State> GetStatesDropDownListData(int langId = 1)
        {
            List<State> list = new List<State>();
            string sql = @"SELECT [State].[StateId], [StateName] FROM [State] ORDER BY [StateName]";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@LangID", langId));

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new State()
                {
                    StateId = DataManager.ToInt(row["StateId"]),
                    StateName = DataManager.ToString(row["StateName"])
                });
            }
            return list;
        }
        #endregion

        #region GetAllStates
        public List<State> GetAllStates()
        {
            string sql = @"SELECT [State].[StateId], [State].[StateName], [State].[RegionId] FROM [State] 
                           ORDER BY [StateName]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            List<State> list = new List<State>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new State()
                    {
                        StateId = DataManager.ToInt(row["StateId"]),
                        StateName = DataManager.ToString(row["StateName"]),
                        RegionId = DataManager.ToInt(row["RegionId"])
                    });
                }
            }
            return list;
        }
        #endregion

        #region GetAvailableStates
        public List<State> GetAvailableStates()
        {
            string sql = @"SELECT DISTINCT [State].[StateId], [State].[StateName], [State].[RegionId] FROM [State] 
                          INNER JOIN [Site] ON [Site].[StateId] = [State].[StateId]
                          INNER JOIN [Item] ON [Item].[SiteId] = [Site].[SiteId]
                          WHERE [Item].[Active] = 1 AND [Item].[IsPublish] = 1
                          ORDER BY [StateName]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            List<State> list = new List<State>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new State()
                    {
                        StateId = DataManager.ToInt(row["StateId"]),
                        StateName = DataManager.ToString(row["StateName"]),
                        RegionId = DataManager.ToInt(row["RegionId"])
                    });
                }
            }
            return list;
        }
        #endregion

        #region GetAvailableStatesByItemType
        public List<Tuple<int, int>> GetAvailableStatesByItemType()
        {
            string sql = @"SELECT DISTINCT [State].[StateId], [Item].[ItemTypeId] FROM [State] 
                          INNER JOIN [Site] ON [Site].[StateId] = [State].[StateId]
                          INNER JOIN [Item] ON [Item].[SiteId] = [Site].[SiteId]
                          WHERE [Item].[Active] = 1 AND [Item].[IsPublish] = 1
                          ORDER BY [StateId]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(Tuple.Create(DataManager.ToInt(row["ItemTypeId"]), DataManager.ToInt(row["StateId"])));
                }
            }
            return list;
        }
        #endregion

        #region GetAllStatesByCountryId
        public List<State> GetAllStatesByCountryId(int countryId)
        {
            using (var db = GetModels())
            {
                return (from it in db.States
                        join r in db.Regions on it.RegionId equals r.RegionId
                        where r.CountryId == countryId
                        orderby it.StateName
                        select it).ToList();
            }
        }

        public List<State> GetStatesListByCountryId(int countryId)
        {
            string sql = @"SELECT [State].[StateId], [State].[StateName] FROM [State] 
                           INNER JOIN [Region] ON [Region].[RegionId] = [State].[RegionId]
                           WHERE [Region].[CountryId] = @CountryId
                           ORDER BY [StateName]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CountryId", countryId));

            List<State> list = new List<State>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new State()
                    {
                        StateId = DataManager.ToInt(row["StateId"]),
                        StateName = DataManager.ToString(row["StateName"])
                    });
                }
            }
            return list;
        }

        public List<State> GetStatesListByRegionId(int regionId, int countryId = 0)
        {
            string sql = @"SELECT [State].[StateId], [State].[StateName] FROM [State]
                           INNER JOIN [Region] ON [Region].[RegionId] = [State].[RegionId]";
            if (regionId > 0)
            {
                sql += " WHERE [State].[RegionId] = " + regionId;
            }
            else if (countryId > 0)
            {
                sql += " WHERE [Region].[CountryId] = " + countryId;
            }
            sql += " ORDER BY [StateName]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            List<State> list = new List<State>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new State()
                    {
                        StateId = DataManager.ToInt(row["StateId"]),
                        StateName = DataManager.ToString(row["StateName"])
                    });
                }
            }
            return list;
        }
        #endregion

        #endregion

        #region City
        #region GetAllCities
        public List<City> GetAllCities()
        {
            using (var db = GetModels())
            {
                return db.Cities.OrderBy(it => it.CityName).ToList();
            }
        }
        #endregion

        #region GetCityByCountryId
        public List<City> GetCityByCountryId(int countryId)
        {
            using (var db = GetModels())
            {
                return (from it in db.Cities
                        join r in db.Regions on it.RegionId equals r.RegionId
                        join c in db.Countries on r.CountryId equals c.CountryId
                        orderby it.CityName
                        select it).ToList();
            }
        }
        #endregion

        #region GetCityIdByCityName
        public int GetCityIdByCityName(string cityName, int countryId)
        {
            string sql = @"SELECT TOP(1) [CityId] FROM [City]
                           INNER JOIN [Region] ON [Region].[RegionId] = [City].[RegionId]
                           INNER JOIN [Country] ON [Country].[CountryId] = [Region].[CountryId]
                           WHERE [City].[CityName] = @CityName AND [Country].[CountryId] = @CountryId";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("CityName", cityName),
                new SqlParameter("CountryId", countryId)), 0);
        }
        #endregion

        #region GetDataByPostalCode
        public void GetDataByPostalCode(string postalCode, out int cityId, out string country, out List<City> cities)
        {
            string sql = @"SELECT [City].[RegionId], [City].[CityId], [CountryName] FROM [City]
                        JOIN [Region] ON [Region].[RegionId] = [City].[RegionId]
                        JOIN [Country] ON [Country].[CountryId] = [Region].[CountryId]
                        WHERE [City].[PostalCode] = @PostalCode";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@PostalCode", postalCode));
            if (ds.Tables[0].Rows.Count > 0)
            {
                cityId = DataManager.ToInt(ds.Tables[0].Rows[0]["CityId"]);
                country = DataManager.ToString(ds.Tables[0].Rows[0]["CountryName"]);
                cities = new List<City>();
                sql = "SELECT [City].[CityId], [City].[CityName] FROM [City] WHERE [City].[RegionId] = @RegionId";
                ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@RegionId", DataManager.ToInt(ds.Tables[0].Rows[0]["RegionId"])));
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    cities.Add(new City()
                    {
                        CityId = DataManager.ToInt(row["CityId"]),
                        CityName = DataManager.ToString(row["CityName"]),
                    });
                }
            }
            else
            {
                cityId = 0;
                country = string.Empty;
                cities = new List<City>();
            }
        }
        #endregion

        #region GetCityByCityName
        public int GetCityByCityName(string cityName, string postalCode = "")
        {
            string sql = "SELECT TOP(1) CityId FROM City WHERE CityName = @CityName";
            if (!String.IsNullOrWhiteSpace(postalCode))
            {
                sql += " AND PostalCode = @PostalCode";
            }
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CityName", cityName),
                new SqlParameter("@PostalCode", postalCode)), 0);
        }
        #endregion
        #endregion

        #region Region
        public List<Region> GetAllRegions()
        {
            string sql = @"SELECT [RegionId], [RegionName], [CountryId] FROM [Region]
                           ORDER BY [RegionName]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            List<Region> list = new List<Region>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new Region()
                    {
                        RegionId = DataManager.ToInt(row["RegionId"]),
                        RegionName = DataManager.ToString(row["RegionName"]),
                        CountryId = DataManager.ToInt(row["CountryId"])
                    });
                }
            }
            return list;
        }
        public List<Region> GetAvailableRegions()
        {
            string sql = @"SELECT DISTINCT [Region].[RegionId], [Region].[RegionName], [Region].[CountryId] FROM [Region]
                           INNER JOIN [Site] ON [Site].[RegionId] = [Region].[RegionId]
                           INNER JOIN [Item] ON [Item].[SiteId] = [Site].[SiteId]
                           WHERE [Item].[Active] = 1 AND [Item].[IsPublish] = 1
                           ORDER BY [RegionName]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            List<Region> list = new List<Region>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new Region()
                    {
                        RegionId = DataManager.ToInt(row["RegionId"]),
                        RegionName = DataManager.ToString(row["RegionName"]),
                        CountryId = DataManager.ToInt(row["CountryId"])
                    });
                }
            }
            return list;
        }
        public Dictionary<int, int> GetAvailableRegionsByItemType()
        {
            string sql = @"SELECT DISTINCT [Region].[RegionId], [Item].[ItemTypeId] FROM [Region]
                           INNER JOIN [Site] ON [Site].[RegionId] = [Region].[RegionId]
                           INNER JOIN [Item] ON [Item].[SiteId] = [Site].[SiteId]
                           WHERE [Item].[Active] = 1 AND [Item].[IsPublish] = 1
                           ORDER BY [RegionName]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            Dictionary<int, int> list = new Dictionary<int, int>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(DataManager.ToInt(row["RegionId"]), DataManager.ToInt(row["ItemTypeId"]));
                }
            }
            return list;
        }

        #region GetStatesListByCountryId
        public List<Region> GetAllRegionsByCountryId(int countryId)
        {
            string sql = @"SELECT [Region].[RegionId], [Region].[RegionName] FROM [Region] 
                           WHERE [Region].[CountryId] = @CountryId
                           ORDER BY [RegionName]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@CountryId", countryId));

            List<Region> list = new List<Region>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new Region()
                    {
                        RegionId = DataManager.ToInt(row["RegionId"]),
                        RegionName = DataManager.ToString(row["RegionName"])
                    });
                }
            }
            return list;
        }
        #endregion

        #region GetRegionsListByCountryId
        public List<Region> GetRegionsListByCountryId(int countryId, int langId = 1)
        {
            string sql = @"SELECT [Region].[RegionId], [Region].[RegionName] FROM [Region] ";
            if (countryId > 0)
            {
                sql += " WHERE [Region].[CountryId] = " + countryId;
            }
            sql += " ORDER BY [CountryId], [RegionName]";

            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

            List<Region> list = new List<Region>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new Region()
                    {
                        RegionId = DataManager.ToInt(row["RegionId"]),
                        RegionName = DataManager.ToString(row["RegionName"])
                    });
                }
            }
            return list;
        }
        #endregion


        #endregion

        #region Brand
        //public List<Brand> GetAllBrands()
        //{
        //    List<Brand> list = new List<Brand>();
        //    string sql = "SELECT * FROM [Brand] ORDER BY [BrandName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Brand(row));
        //    }
        //    return list;
        //}
        //public List<Brand> GetAllBrands(jQueryDataTableParamModel param, int langId = 1)
        //{
        //    List<Brand> list = new List<Brand>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectBrandsList",
        //        new SqlParameter("@Start", param.start),
        //        new SqlParameter("@Length", param.length),
        //        new SqlParameter("@Search", param.search),
        //        new SqlParameter("@Order", param.order));

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Brand(row));
        //    }

        //    param.recordsTotal = DataManager.ToInt(ds.Tables[1].Rows[0]["TotalRows"]);
        //    return list;
        //}

        //#region GetBrand
        //public Brand GetBrand(long id)
        //{
        //    Brand obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "SELECT * FROM [Brand] WHERE [BrandId] = @BrandId",
        //        new SqlParameter("@BrandId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new Brand(ds.Tables[0].Rows[0]);
        //    }

        //    return obj;
        //}
        //#endregion

        //#region AddBrand
        //public int AddBrand(Brand obj)
        //{
        //    string sql = "INSERT INTO [Brand]([BrandName], [BrandDesc], [BrandImage], [Active]) VALUES(@BrandName, @BrandDesc, @BrandImage, @Active) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@BrandName", obj.BrandName),
        //       new SqlParameter("@BrandDesc", obj.BrandDesc),
        //       new SqlParameter("@BrandImage", obj.BrandImage),
        //       new SqlParameter("@Active", obj.Active)));
        //}
        //#endregion

        //#region UpdateBrand
        //public int UpdateBrand(Brand obj)
        //{
        //    string sql = "UPDATE [Brand] SET [BrandName] = @BrandName, [BrandDesc] = @BrandDesc, [BrandImage] = @BrandImage, [Active] = @Active WHERE [BrandId] = @BrandId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@BrandId", obj.BrandId),
        //       new SqlParameter("@BrandName", obj.BrandName),
        //       new SqlParameter("@BrandDesc", obj.BrandDesc),
        //       new SqlParameter("@BrandImage", obj.BrandImage),
        //       new SqlParameter("@Active", obj.Active));
        //}
        //#endregion

        //#region DeleteBrand
        //public int DeleteBrand(int id)
        //{
        //    string sql = "UPDATE [Brand] SET [Active] = 0 WHERE [BrandId] = @BrandId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@BrandId", id));
        //}
        //#endregion
        //#region GetBrandDropDownListData
        //public List<Brand> GetBrandDropDownListData()
        //{
        //    List<Brand> list = new List<Brand>();
        //    string sql = @"SELECT [Brand].[BrandId], [Brand].[BrandName] FROM [Brand]
        //                    WHERE [Active] = 1 ORDER BY [BrandName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Brand()
        //        {
        //            BrandId = DataManager.ToInt(row["BrandId"]),
        //            BrandName = DataManager.ToString(row["BrandName"])
        //        });
        //    }
        //    return list;
        //}

        //#region SaveBrandImage
        //public void SaveBrandImage(int id, string imageName)
        //{
        //    string sql = "UPDATE [Brand] SET [BrandImage] = @BrandImage WHERE [BrandId] = @BrandId";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@BrandImage", imageName),
        //        new SqlParameter("@BrandId", id));
        //}
        //#endregion

        //#region GetBrandsWithSaleItems
        //public List<Brand> GetBrandsWithSaleItems()
        //{
        //    List<Brand> list = new List<Brand>();
        //    string sql = @"SELECT [Brand].*, 0 AS [SaleItems] FROM [Brand]
        //                    WHERE [Active] = 1 ORDER BY [BrandName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    Brand brand = null;
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        brand = new Brand(row);
        //        brand.SaleItems = DataManager.ToInt(row["SaleItems"]);
        //        list.Add(brand);
        //    }
        //    return list;
        //}
        //#endregion
        //#endregion
        #endregion

        #region GolfBrand
        //public List<GolfBrand> GetAllGolfBrands()
        //{
        //    List<GolfBrand> list = new List<GolfBrand>();
        //    string sql = "SELECT * FROM [GolfBrand] ORDER BY [GolfBrandName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new GolfBrand(row));
        //    }
        //    return list;
        //}
        //public List<GolfBrand> GetAllGolfBrands(jQueryDataTableParamModel param, int langId = 1)
        //{
        //    List<GolfBrand> list = new List<GolfBrand>();
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, "SelectGolfBrandsList");

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new GolfBrand(row));
        //    }
        //    return list;
        //}

        //#region GetGolfBrand
        //public GolfBrand GetGolfBrand(long id)
        //{
        //    GolfBrand obj = null;
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "SELECT * FROM [GolfBrand] WHERE [GolfBrandId] = @GolfBrandId",
        //        new SqlParameter("@GolfBrandId", id));

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        obj = new GolfBrand(ds.Tables[0].Rows[0]);
        //    }

        //    return obj;
        //}
        //#endregion

        //#region AddGolfBrand
        //public int AddGolfBrand(GolfBrand obj)
        //{
        //    string sql = "INSERT INTO [GolfBrand]([GolfBrandName], [GolfBrandDesc], [Active]) VALUES(@GolfBrandName, @GolfBrandDesc, @Active) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@GolfBrandName", obj.GolfBrandName),
        //       new SqlParameter("@GolfBrandDesc", obj.GolfBrandDesc),
        //       new SqlParameter("@Active", obj.Active)));
        //}
        //#endregion

        //#region UpdateGolfBrand
        //public int UpdateGolfBrand(GolfBrand obj)
        //{
        //    string sql = "UPDATE [GolfBrand] SET [GolfBrandName] = @GolfBrandName, [GolfBrandDesc] = @GolfBrandDesc, [Active] = @Active WHERE [GolfBrandId] = @GolfBrandId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@GolfBrandId", obj.GolfBrandId),
        //       new SqlParameter("@GolfBrandName", obj.GolfBrandName),
        //       new SqlParameter("@GolfBrandDesc", obj.GolfBrandDesc),
        //       new SqlParameter("@Active", obj.Active));
        //}
        //#endregion

        //#region DeleteGolfBrand
        //public int DeleteGolfBrand(int id)
        //{
        //    string sql = "UPDATE [GolfBrand] SET [Active] = 0 WHERE [GolfBrandId] = @GolfBrandId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@GolfBrandId", id));
        //}
        //#endregion
        //#region GetGolfBrandDropDownListData
        //public List<GolfBrand> GetGolfBrandDropDownListData()
        //{
        //    List<GolfBrand> list = new List<GolfBrand>();
        //    string sql = @"SELECT [GolfBrand].[GolfBrandId], [GolfBrand].[GolfBrandName] FROM [GolfBrand]
        //                    WHERE [Active] = 1 ORDER BY [GolfBrandName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new GolfBrand()
        //        {
        //            GolfBrandId = DataManager.ToInt(row["GolfBrandId"]),
        //            GolfBrandName = DataManager.ToString(row["GolfBrandName"])
        //        });
        //    }
        //    return list;
        //}

        //#region SaveGolfBrandImage
        //public void SaveGolfBrandImage(int id, string imageName)
        //{
        //    string sql = "UPDATE [GolfBrand] SET [GolfBrandImage] = @GolfBrandImage WHERE [GolfBrandId] = @GolfBrandId";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@GolfBrandImage", imageName),
        //        new SqlParameter("@GolfBrandId", id));
        //}
        //#endregion

        //#region GetGolfBrandsWithSaleItems
        //public List<GolfBrand> GetGolfBrandsWithSaleItems()
        //{
        //    List<GolfBrand> list = new List<GolfBrand>();
        //    string sql = @"SELECT * FROM [GolfBrand]
        //                    WHERE [Active] = 1 ORDER BY [GolfBrandName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new GolfBrand(row));
        //    }
        //    return list;
        //}
        //#endregion
        //#endregion
        #endregion

        #region Adset
        ///// <summary>
        ///// Get all active Adsets.
        ///// </summary>
        ///// <returns>All active Adsets.</returns>
        //public List<Adset> GetAllAdsets()
        //{
        //    string sql = "SELECT * FROM [Adset] WHERE [Active] = 1 ORDER BY [AdsetName]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        //    List<Adset> Adsets = new List<Adset>();
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        Adsets.Add(new Adset(row));
        //    }
        //    return Adsets;
        //}

        ///// <summary>
        ///// Check Adset exists.
        ///// </summary>
        ///// <param name="AdsetName">Adset Name</param>
        ///// <returns>True if there is exists Adset, otherwise return False.</returns>
        //public bool IsExistsAdsetName(string AdsetName)
        //{
        //    string sql = "SELECT COUNT([AdsetId]) FROM [Adset] WHERE [Active] = 1 AND [AdsetName] = @AdsetName";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@AdsetName", AdsetName))) > 0;
        //}

        ///// <summary>
        ///// Save Adset
        ///// </summary>
        ///// <param name="AdsetName">Adset Name</param>
        ///// <returns>True is save success, otherwise return False.</returns>
        //public int AddAdset(string AdsetName)
        //{
        //    string sql = "INSERT INTO [Adset]([AdsetName]) VALUES(@AdsetName) @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@AdsetName", AdsetName)));
        //}

        ///// <summary>
        ///// Delete Adset by Adset id
        ///// </summary>
        ///// <param name="AdsetId">Adset Id</param>
        //public void DeleteAdset(int AdsetId)
        //{
        //    string sql = "DELETE FROM [Adset] WHERE [AdsetId] = @AdsetId";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@AdsetId", AdsetId));
        //}
        #endregion

        #region Ads
        ///// <summary>
        ///// Get all ads by Adset Id
        ///// </summary>
        ///// <param name="AdsetId">Adset Id</param>
        ///// <returns>All ads by Adset Id</returns>
        //public List<Ad> GetAdsByAdsetId(int AdsetId)
        //{
        //    string sql = "SELECT * FROM [Ads] WHERE [Active] = 1 AND [AdsetId] = @AdsetId ORDER BY [ListNo], [AdsId]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@AdsetId", AdsetId));
        //    List<Ad> ads = new List<Ad>();
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        ads.Add(new Ad(row));
        //    }
        //    return ads;
        //}

        ///// <summary>
        ///// Get All Ads
        ///// </summary>
        ///// <returns>All Ads</returns>
        //public List<Ad> GetAllAds()
        //{
        //    var db = GetModels();
        //    var list = (from it in db.Ads
        //                join adset in db.Adsets on it.AdsetId equals adset.AdsetId
        //                where it.Active == true && adset.Active == true
        //                orderby it.AdsetId
        //                orderby it.AdsId
        //                select new { Ad = it, Adset = adset }).ToList();

        //    List<Ad> ads = new List<Ad>();

        //    list.ForEach(it =>
        //    {
        //        it.Ad.AdsetName = it.Adset.AdsetName;
        //        ads.Add(it.Ad);
        //    });

        //    return ads;
        //}

        ///// <summary>
        ///// Save list no. for each ads
        ///// </summary>
        ///// <param name="topNavLinkId">Ads ID</param>
        ///// <param name="listNo">List No.</param>
        //public void SaveAdsListNo(int adsId, int listNo)
        //{
        //    string sql = "UPDATE [Ads] SET [ListNo] = @ListNo WHERE [AdsId] = @AdsId";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ListNo", listNo),
        //        new SqlParameter("@AdsId", adsId));
        //}

        ///// <summary>
        ///// Delete Ads By ID
        ///// </summary>
        ///// <param name="adsId">Ads ID</param>
        //public void DeleteAds(int adsId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "DELETE FROM [Ads] WHERE [AdsId] = " + adsId);
        //}

        //public void SaveAds(int adsId, int adsetId, string adsName, string linkUrl, string imageUrl, int listNo, DateTime fromDate, DateTime toDate)
        //{
        //    string sql = "";
        //    if (adsId > 0)
        //    {
        //        sql = "UPDATE [Ads] SET AdsetId = @AdsetId, AdsName = @AdsName, LinkUrl = @LinkUrl, ImageUrl = @ImageUrl, ListNo = @ListNo, FromDate = @FromDate, ToDate = @ToDate WHERE AdsId = @AdsId";
        //    }
        //    else
        //    {
        //        sql = "INSERT INTO [Ads](AdsetId, AdsName, LinkUrl, ImageUrl, ListNo, FromDate, ToDate, Active) VALUES(@AdsetId, @AdsName, @LinkUrl, @ImageUrl, @ListNo, @FromDate, @ToDate, 1)";
        //    }
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@AdsetId", adsetId),
        //        new SqlParameter("@AdsName", adsName),
        //        new SqlParameter("@LinkUrl", linkUrl),
        //        new SqlParameter("@ImageUrl", imageUrl),
        //        new SqlParameter("@ListNo", listNo),
        //        new SqlParameter("@FromDate", fromDate),
        //        new SqlParameter("@ToDate", toDate),
        //        new SqlParameter("@AdsId", adsId));
        //}
        #endregion

        #region TopNavLink
        //#region GetAllTopNavLinks
        //public List<TopNavLink> GetAllTopNavLinks()
        //{
        //    List<TopNavLink> list = new List<TopNavLink>();
        //    string sql = "SELECT * FROM [TopNavLink] ORDER BY [ListNo], [TopNavLinkId]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new TopNavLink(row));
        //    }
        //    return list;
        //}
        //#endregion

        //#region AddTopNavLink
        //public int AddTopNavLink(TopNavLink obj)
        //{
        //    string sql = "INSERT INTO [TopNavLink]([LinkUrl], [ImageUrl], [ListNo]) VALUES(@LinkUrl, @ImageUrl, @ListNo) SELECT @@IDENTITY";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@LinkUrl", obj.LinkUrl),
        //       new SqlParameter("@ImageUrl", obj.ImageUrl),
        //       new SqlParameter("@ListNo", obj.ListNo)));
        //}
        //#endregion

        //#region UpdateTopNavLink
        //public int UpdateTopNavLink(TopNavLink obj)
        //{
        //    string sql = "UPDATE [TopNavLink] SET [LinkUrl] = @LinkUrl, [ImageUrl] = @ImageUrl, [ListNo] = @ListNo WHERE [TopNavLinkId] = @TopNavLinkId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@TopNavLinkId", obj.TopNavLinkId),
        //       new SqlParameter("@LinkUrl", obj.LinkUrl),
        //       new SqlParameter("@ImageUrl", obj.ImageUrl),
        //       new SqlParameter("@ListNo", obj.ListNo));
        //}
        //#endregion

        //#region SaveTopNavLink
        //public int SaveTopNavLink(TopNavLink obj)
        //{
        //    string sql = @"IF EXISTS(SELECT [TopNavLinkId] FROM [TopNavLink] WHERE [TovNavLinkId] = @TopNavLinkId)
        //                    BEGIN
        //                        UPDATE [TopNavLink] SET [LinkUrl] = @LinkUrl, [ImageUrl] = @ImageUrl, [ListNo] = @ListNo WHERE [TopNavLinkId] = @TopNavLinkId;
        //                        SELECT @TopNavLinkId;
        //                    END
        //                    ELSE
        //                    BEGIN
        //                        INSERT INTO [TopNavLink]([LinkUrl], [ImageUrl], [ListNo]) VALUES(@LinkUrl, @ImageUrl, @ListNo); SELECT @@IDENTITY;
        //                    END";
        //    obj.TopNavLinkId = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@TopNavLinkId", obj.TopNavLinkId),
        //       new SqlParameter("@LinkUrl", obj.LinkUrl),
        //       new SqlParameter("@ImageUrl", obj.ImageUrl),
        //       new SqlParameter("@ListNo", obj.ListNo)), 0);

        //    return obj.TopNavLinkId;
        //}

        //public void SaveTopNavLink(int topNavLinkId, string linkUrl, string imageUrl, int listNo)
        //{
        //    string sql = @"IF EXISTS(SELECT [TopNavLinkId] FROM [TopNavLink] WHERE [TopNavLinkId] = @TopNavLinkId)
        //                    BEGIN
        //                        UPDATE [TopNavLink] SET [LinkUrl] = @LinkUrl, [ImageUrl] = @ImageUrl, [ListNo] = @ListNo WHERE [TopNavLinkId] = @TopNavLinkId;
        //                        SELECT @TopNavLinkId;
        //                    END
        //                    ELSE
        //                    BEGIN
        //                        INSERT INTO [TopNavLink]([LinkUrl], [ImageUrl], [ListNo]) VALUES(@LinkUrl, @ImageUrl, @ListNo); SELECT @@IDENTITY;
        //                    END";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //       new SqlParameter("@TopNavLinkId", topNavLinkId),
        //       new SqlParameter("@LinkUrl", linkUrl),
        //       new SqlParameter("@ImageUrl", imageUrl),
        //       new SqlParameter("@ListNo", listNo));
        //}
        //#endregion

        //#region SaveTopNavLinkListNo
        //public void SaveTopNavLinkListNo(int topNavLinkId, int listNo)
        //{
        //    string sql = "UPDATE [TopNavLink] SET [ListNo] = @ListNo WHERE [TopNavLinkId] = @TopNavLinkId";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@ListNo", listNo),
        //        new SqlParameter("@TopNavLinkId", topNavLinkId));
        //}
        //#endregion

        //#region DeleteTopNavLink
        //public int DeleteTopNavLink(int id)
        //{
        //    string sql = "DELETE FROM [TopNavLink] WHERE [TopNavLinkId] = @TopNavLinkId";
        //    return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@TopNavLinkId", id));
        //}
        //#endregion
        #endregion

        #region Menu
        //public Menu GetMenuByPlacement(string placement)
        //{
        //    string sql = @"SELECT TOP(1) * FROM [Menu] WHERE [MenuPlacement] = @MenuPlacement;
        //                   SELECT * FROM [MenuItem]
        //                   INNER JOIN [Menu] ON [Menu].[MenuId] = [MenuItem].[MenuId]
        //                   WHERE [Menu].[MenuPlacement] = @MenuPlacement;";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@MenuPlacement", placement));

        //    return AssignMenuObjects(ds);
        //}
        //public Menu GetMenuByMenuId(int menuId)
        //{
        //    string sql = @"SELECT TOP(1) * FROM [Menu] WHERE [MenuId] = @MenuId;
        //                   SELECT * FROM [MenuItem]
        //                   WHERE [MenuItem].[MenuId] = @MenuId;";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@MenuId", menuId));

        //    return AssignMenuObjects(ds);
        //}

        //private Menu AssignMenuObjects(DataSet ds)
        //{
        //    Menu menu = null;
        //    if (ds.Tables[0].Rows.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
        //    {
        //        menu = new Menu();
        //    }
        //    else
        //    {
        //        menu = new Menu(ds.Tables[0].Rows[0]);
        //        if (ds.Tables.Count > 1)
        //        {
        //            DataRow[] masterRows = ds.Tables[1].Select("ParentId = 0");
        //            MenuItem item = null;
        //            foreach (DataRow row in masterRows)
        //            {
        //                item = new MenuItem(row);
        //                menu.MenuItems.Add(item);
        //                GetMenuItemByParentId(ds.Tables[1], item);
        //            }
        //        }
        //    }
        //    return menu;
        //}

        //private void GetMenuItemByParentId(DataTable dt, MenuItem parentItem)
        //{
        //    DataRow[] childrenRows = dt.Select("ParentId = " + parentItem.MenuItemId);
        //    if (childrenRows.Length <= 0)
        //        return;

        //    MenuItem item = null;
        //    foreach (DataRow row in childrenRows)
        //    {
        //        item = new MenuItem(row);
        //        parentItem.Children.Add(item);
        //        GetMenuItemByParentId(dt, item);
        //    }
        //}

        //public List<Menu> GetMenuDropDownList()
        //{
        //    List<Menu> list = new List<Menu>();
        //    string sql = @"SELECT [Menu].[MenuId], [Menu].[MenuName] FROM [Menu]
        //                    WHERE [Active] = 1 ORDER BY [Menu].[MenuId]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);

        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        list.Add(new Menu()
        //        {
        //            MenuId = DataManager.ToInt(row["MenuId"]),
        //            MenuName = DataManager.ToString(row["MenuName"])
        //        });
        //    }
        //    return list;
        //}

        //public void SaveMenuItems(List<MenuItem> items)
        //{
        //    foreach (MenuItem item in items)
        //    {
        //        item.MenuItemId = SaveMenuItem(item);
        //        if (item.MenuItemId > 0 && item.Children != null && item.Children.Any())
        //        {
        //            item.Children.ForEach(it => it.ParentId = item.MenuItemId);
        //            SaveMenuItems(item.Children);
        //        }
        //    }
        //}

        //public int SaveMenuItem(MenuItem item)
        //{
        //    string sql = @"IF EXISTS(SELECT MenuItemId FROM [MenuItem] WHERE [MenuItemId] = @MenuItemId)
        //                       BEGIN
        //                            UPDATE [MenuItem] SET [MenuId] = @MenuId, [ParentId] = @ParentId, [MenuType] = @MenuType, [MenuTitle] = @MenuTitle, [MenuValue] = @MenuValue WHERE [MenuItemId] = @MenuItemId;
        //                            SELECT @MenuItemId;
        //                       END
        //                   ELSE
        //                       BEGIN
        //                            INSERT INTO [MenuItem]([MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue])
        //                            VALUES(@MenuId, @ParentId, @MenuType, @MenuTitle, @MenuValue);
        //                            SELECT @@IDENTITY;
        //                       END";
        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@MenuItemId", item.MenuItemId),
        //        new SqlParameter("@MenuId", item.MenuId),
        //        new SqlParameter("@ParentId", item.ParentId),
        //        new SqlParameter("@MenuType", item.MenuType),
        //        new SqlParameter("@MenuTitle", item.MenuTitle),
        //        new SqlParameter("@MenuValue", item.MenuValue)), 0);
        //}

        //public MenuItem GetMenuItem(int menuItemId)
        //{
        //    MenuItem item = null;
        //    string sql = "SELECT * FROM [MenuItem] WHERE [MenuItemId] = @MenuItemId;";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@MenuItemId", menuItemId));
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        item = new MenuItem(ds.Tables[0].Rows[0]);
        //    }
        //    return item;
        //}

        //public void DeleteMenuItemsByIds(string deletedIds)
        //{
        //    string sql = "DELETE FROM [MenuItem] WHERE [MenuItemId] IN(" + deletedIds + ")";
        //    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        //}

        //public void SaveMenu(Menu menu)
        //{
        //    string sql = @"IF EXISTS(SELECT [MenuId] FROM [Menu] WHERE [MenuId] = @MenuId)
        //                  BEGIN
        //                    UPDATE [Menu] SET [MenuName] = @MenuName, [MenuPlacement] = @MenuPlacement, [Active] = @Active WHERE [MenuId] = @MenuId;
        //                    SELECT @MenuId;
        //                  END
        //                  ELSE
        //                  BEGIN
        //                    INSERT INTO [Menu]([MenuName], [MenuPlacement], [Active]) VALUES(@MenuName, @MenuPlacement, @Active); SELECT @@IDENTITY;
        //                  END";
        //    menu.MenuId = DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@MenuId", menu.MenuId),
        //        new SqlParameter("@MenuName", menu.MenuName),
        //        new SqlParameter("@MenuPlacement", menu.MenuPlacement),
        //        new SqlParameter("@Active", menu.Active)));

        //    if (menu.MenuId > 0 && menu.MenuItems != null && menu.MenuItems.Any())
        //    {
        //        SaveMenuItems(menu.MenuItems);
        //    }
        //}
        #endregion

        #region ReservationAPI
        #region GetAllReservationAPIs
        public List<ReservationAPI> GetAllReservationAPIs()
        {
            using (var db = GetModels())
            {
                return db.ReservationAPIs.OrderBy(it => it.APIId).ToList();
            }
        }
        #endregion

        #region AddReservationAPI
        public int AddReservationAPI(ReservationAPI obj)
        {
            string sql = "INSERT INTO [ReservationAPI]([APICode], [APIName]) VALUES(@APICode, @APIName) SELECT @@IDENTITY";
            return DataManager.ToInt(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@APICode", obj.APICode),
               new SqlParameter("@APIName", obj.APIName)));
        }
        #endregion

        #region UpdateReservationAPI
        public int UpdateReservationAPI(ReservationAPI obj)
        {
            string sql = "UPDATE [ReservationAPI] SET [APICode] = @APICode, [APIName] = @APIName WHERE [APIId] = @APIId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
               new SqlParameter("@APIId", obj.APIId),
               new SqlParameter("@APICode", obj.APICode),
               new SqlParameter("@APIName", obj.APIName));
        }
        #endregion

        #region DeleteBrand
        public int DeleteReservationAPI(int id)
        {
            string sql = "DELETE FROM [ReservationAPI] WHERE [APIId] = @APIId";
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@APIId", id));
        }
        #endregion
        #endregion

        public void UpdateUserPassword()
        {
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "SELECT UserId, Password2 FROM Users WHERE Password2 IS NOT NULL");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "UPDATE Users SET [Password] = @Password WHERE [UserId] = @UserId",
                        new SqlParameter("@Password", DataProtection.Encrypt(DataManager.ToString(row["Password2"]))),
                        new SqlParameter("@UserId", DataManager.ToLong(row["UserId"])));
                }
            }
        }

        public Site GetSiteAlbatrosSettingBySiteId(long siteId)
        {
            Site site = new Site();
            string sql = "SELECT AlbatrosUrl, AlbatrosUsername, AlbatrosPassword, Email FROM [Site] WHERE [SiteId] = @SiteId";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", siteId));
            if (ds.Tables[0].Rows.Count > 0)
            {
                site.AlbatrosUrl = DataManager.ToString(ds.Tables[0].Rows[0]["AlbatrosUrl"]);
                site.AlbatrosUsername = DataManager.ToString(ds.Tables[0].Rows[0]["AlbatrosUsername"]);
                site.AlbatrosPassword = DataManager.ToString(ds.Tables[0].Rows[0]["AlbatrosPassword"]);
                site.Email = DataManager.ToString(ds.Tables[0].Rows[0]["Email"]);
            }
            return site;
        }

        public Site GetSitePrimaSettingBySiteId(long siteId)
        {
            Site site = new Site();
            string sql = "SELECT PrimaAPIKey, PrimaClubKey FROM [Site] WHERE [SiteId] = @SiteId";
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
                new SqlParameter("@SiteId", siteId));
            if (ds.Tables[0].Rows.Count > 0)
            {
                site.PrimaAPIKey = DataManager.ToString(ds.Tables[0].Rows[0]["PrimaAPIKey"]);
                site.PrimaClubKey = DataManager.ToString(ds.Tables[0].Rows[0]["PrimaClubKey"]);
            }
            return site;
        }

        //public List<Item> GetProductSiteMap(int langId = 1)
        //{
        //    string sql = @"SELECT [Item].[ItemId], [Item].[ItemSlug], [ItemLang].[ItemName], [ItemLang].[InvoiceName], [Item].[UpdateDate], [Item].[IsShowOnHomepage], [Item].[PublishStartDate], [Item].[PublishEndDate]
        //                    FROM [Item]
        //                    LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND ([ItemLang].[LangId] = @LangId)
        //                    WHERE [Item].[Active] = 1 AND [Item].[IsShowOnHomepage] = 1 AND GETDATE() BETWEEN [Item].[PublishStartDate] AND DATEADD(DAY, 1, [Item].[PublishEndDate])
        //                    ORDER BY [Item].[ItemId] DESC;
        //                   SELECT * FROM [ItemImage]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@LangId", langId));
        //    List<Item> items = new List<Item>();
        //    List<ItemImage> itemImages = new List<ItemImage>();
        //    Item item;
        //    foreach (DataRow row in ds.Tables[1].Rows)
        //    {
        //        itemImages.Add(new ItemImage(row));
        //    }
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        item = new Item();
        //        item.ItemId = DataManager.ToLong(row["ItemId"]);
        //        item.ItemSlug = DataManager.ToString(row["ItemSlug"]);
        //        item.ItemName = DataManager.ToString(row["ItemName"]);
        //        item.InvoiceName = DataManager.ToString(row["InvoiceName"]);
        //        item.UpdateDate = DataManager.ToDateTime(row["UpdateDate"]);
        //        item.ItemImages = itemImages.Where(it => it.ItemId == item.ItemId).ToList();
        //        items.Add(item);
        //    }
        //    return items;
        //}

        //public List<Site> GetGolfSiteMap(int langId = 1)
        //{
        //    string sql = @"SELECT [Site].[SiteId], [Site].[SiteSlug], [SiteLang].[SiteName], [Site].[UpdateDate]
        //                    FROM [Site]
        //                    LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] AND ([SiteLang].[LangId] = @LangId)
        //                    WHERE [Site].[Active] = 1
        //                    ORDER BY [Site].[SiteId] DESC;
        //                   SELECT * FROM [SiteImage]";
        //    DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql,
        //        new SqlParameter("@LangId", langId));
        //    List<Site> sites = new List<Site>();
        //    List<SiteImage> siteImages = new List<SiteImage>();
        //    Site site;
        //    foreach (DataRow row in ds.Tables[1].Rows)
        //    {
        //        siteImages.Add(new SiteImage(row));
        //    }
        //    foreach (DataRow row in ds.Tables[0].Rows)
        //    {
        //        site = new Site();
        //        site.SiteId = DataManager.ToInt(row["SiteId"]);
        //        site.SiteSlug = DataManager.ToString(row["SiteSlug"]);
        //        site.SiteName = DataManager.ToString(row["SiteName"]);
        //        site.UpdateDate = DataManager.ToDateTime(row["UpdateDate"]);
        //        site.SiteImages = siteImages.Where(it => it.SiteId == site.SiteId).ToList();
        //        sites.Add(site);
        //    }
        //    return sites;
        //}

        public User GetSampleCustomerInMailingList(int mailingListId)
        {
            long userId = 0;
            string sql = "SELECT TOP(1) [CustomerMailList].[UserId] FROM [CustomerMailList] JOIN [Users] ON [Users].[UserId] = [CustomerMailList].[UserId] AND [Users].[Active] = 1 WHERE [CustomerMailList].[MailingListId] = " + mailingListId + " ORDER BY NEWID()";
            userId = DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql), 0);

            if (userId <= 0)
            {
                sql = "SELECT TOP(1) * FROM [Users] WHERE [Active] = 1 AND [UserTypeId] = " + UserType.Type.Customer + " ORDER BY NEWID();";
                userId = DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql), 0);
            }

            return GetUser(userId);
        }

        public User GetSampleCustomerInCustomerGroup(int customerGroupId)
        {
            long userId = 0;
            string sql = "SELECT TOP(1) [CustomerGroupCustomer].[CustomerId] FROM [CustomerGroupCustomer] JOIN [Users] ON [Users].[UserId] = [CustomerGroupCustomer].[CustomerId] AND [Users].[Active] = 1 WHERE [CustomerGroupCustomer].[CustomerGroupId] = " + customerGroupId + " ORDER BY NEWID()";
            userId = DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql), 0);

            if (userId <= 0)
            {
                sql = "SELECT TOP(1) * FROM [Users] WHERE [Active] = 1 AND [UserTypeId] = " + UserType.Type.Customer + " ORDER BY NEWID();";
                userId = DataManager.ToLong(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql), 0);
            }

            return GetUser(userId);
        }

        public List<int> GetSelectedAffiliationTypesByGroupId(long groupId)
        {
            List<int> customerTypes = new List<int>();
            var db = GetModels();
            var groups = db.CustomerGroups.Where(it => it.CustomerGroupId == groupId);
            foreach (var group in groups)
            {
                customerTypes.AddRange(group.CustomerTypes.Select(it => it.AffiliationTypeId));
            }
            return customerTypes;
        }
    }

}
