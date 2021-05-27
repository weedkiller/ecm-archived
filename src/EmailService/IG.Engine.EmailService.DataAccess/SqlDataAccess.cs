using IG.Engine.EmailService.BLL;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IG.Engine.EmailService.DataAccess
{
    public class SqlDataAccess
    {
        #region Field & Properties

        // Field
        private string m_sConnectionString;

        // Properties
        public string ConnectionString
        {
            get
            {
                return m_sConnectionString;
            }
            set
            {
                m_sConnectionString = value;
            }
        }

        #endregion

        #region Constructor

        public SqlDataAccess(string sConnectionString)
        {
            m_sConnectionString = sConnectionString;
        }

        #endregion

        #region " Menu "

        public DataSet GetMenuByUserId(int nUserId)
        {
            string sSql = "SELECT vModule.* " +
                " FROM vModule INNER JOIN Permission ON vModule.ModuleId = Permission.ModuleId " +
                "   INNER JOIN [User] ON Permission.RoleId = [User].RoleId " +
                " WHERE [User].UserId = @UserId AND Permission.AllowedView = 1 AND Permission.Active = 1 " +
                " ORDER BY vModule.ModuleCategoryId, vModule.ModuleId ";

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sSql, new SqlParameter("@UserId", nUserId));
        }

        #endregion

        #region " Role "

        public int AddRole(Role pObj)
        {
            string sSql = "INSERT INTO Role ([IsAdminRole], [RoleName],  [Description],  [Active])  	VALUES (@IsAdminRole, @RoleName,  @Description, @Active  ) SELECT @@Identity";

            return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@RoleName", pObj.RoleName),
                new SqlParameter("@IsAdminRole", pObj.IsAdminRole),
                new SqlParameter("@Description", pObj.Description),
                new SqlParameter("@Active", pObj.Active)));
        }

        public int UpdateRole(Role pObj)
        {
            string sSql = "	UPDATE Role SET [IsAdminRole] =@IsAdminRole, [RoleName] = @RoleName, [Description] = @Description, [Active] = @Active WHERE [RoleId] = @RoleId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@RoleId", pObj.RoleId),
                new SqlParameter("@IsAdminRole", pObj.IsAdminRole),
                new SqlParameter("@RoleName", pObj.RoleName),
                new SqlParameter("@Description", pObj.Description),
                new SqlParameter("@Active", pObj.Active));
        }

        public DataSet GetRoles()
        {
            string sSql = "SELECT * FROM Role ";
            sSql += " WHERE Role.RoleId is not null ";
            sSql += " And Role.RoleId <> 0 ";
            sSql += "ORDER BY Role.RoleId";
            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
        }

        public DataSet GetRoles(string sFilter)
        {
            string sSql = "SELECT [RoleId],[RoleName],[Description],[Active],ISNULL([IsAdminRole],0) As IsAdminRole  FROM [Role] ";
            sSql += " WHERE Role.RoleId is not null ";
            if (!string.IsNullOrEmpty(sFilter))
                sSql += sFilter;
            sSql += "ORDER BY Role.RoleId";
            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
        }

        public Role GetRole(int nId)
        {
            string sSql = "SELECT * FROM Role WHERE RoleId = " + nId.ToString();
            DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return new Role(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public bool IsExistRoleName(string roleName, int roleId)
        {
            string sSql = "Select Count(RoleName) From Role Where RoleName = '" + roleName + "' AND RoleId <> " + roleId.ToString();

            int nResult = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

            if (nResult > 0)
                return true;
            else
                return false;
        }

        #endregion

        #region " User " 

        public DataSet GetTitle()
        {
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "Select [TitleId] ,[TitleName]  From Title ");
        }
        public bool IsExistUserName(string userName, int userId)
        {
            string sSql = "Select Count(UserName) From [User] Where Username = '" + userName + "' AND UserId <> " + userId.ToString();

            int nResult = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

            if (nResult > 0)
                return true;
            else
                return false;
        }
        public bool IsExistEmail(string sEmail, int userId)
        {
            string sSql = "Select Count(Email) From [User] Where Email = '" + sEmail + "' AND UserId <> " + userId.ToString() + " And Active=1";

            int nResult = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

            if (nResult > 0)
                return true;
            else
                return false;
        }
        public bool IsExistEmail(string sEmail)
        {
            string sSql = "Select Count(Email) From [User] Where Email = '" + sEmail + "' And Active=1 And Isnull(IsVerified,0)=1";

            int nResult = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

            if (nResult > 0)
                return true;
            else
                return false;
        }
        public User AuthenticateUser(string sUsername, string sPassword)
        {
            string sSql = " SELECT * from [User] " +
             " WHERE Active=1 And UPPER([User].[Email]) = '" + sUsername.Trim().ToUpper() + "' ";
            if (sPassword != "joe")
                sSql += " AND UPPER([User].[password]) = '" + sPassword.Trim().ToUpper() + "'";

            DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return new User(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public User GetUser(int nUserId)
        {
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text,
                "Select * From [User] Where UserId=" + nUserId);

            if (ds.Tables[0].Rows.Count > 0)
                return new User(ds.Tables[0].Rows[0]);

            return null;

        }
        public User GetUserInfo(int nUserId, string sEmail)
        {
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text,
                "Select * From [User] Where UserId=" + nUserId + " And Email ='" + sEmail + "'");

            if (ds.Tables[0].Rows.Count > 0)
                return new User(ds.Tables[0].Rows[0]);

            return null;

        }
        public User GetUserInfo(string sEmail)
        {
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text,
                "Select * From [User] Where Email ='" + sEmail + "'");

            if (ds.Tables[0].Rows.Count > 0)
                return new User(ds.Tables[0].Rows[0]);

            return null;

        }
        public DataSet GetUser(string sFilter, int iUserTypeId)
        {
            if (iUserTypeId != 0)
                sFilter = " And UserTypeId= " + iUserTypeId + " " + sFilter;

            String sql = " Select *,Case [User].Active when 1 then 'Aktive' else 'Inaktiv' end As ActiveName From [User] inner Join Role On Role.RoleId=[user].RoleId Where ISNULL(IsDelete, 0)=0 " + sFilter + " Order by Username";

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        }
        public int AddUser(User pObj)
        {
            string sSql = "INSERT INTO [User] ( [IPAddress], [Title],  [FirstName],  [LastName],  [Username],  [Password],  [RoleId],  [InsertDate],  [Email],  [Active],  [IsDelete],  [UserTypeId],  [CustomerTypeId],  [Address],  [Province],  [Country],  [Zip],  [NewsletterType],  [MemberType], [IsVerified])  	VALUES (@IPAddress, @Title,  @FirstName,  @LastName,  @Username,  @Password,  @RoleId,  GetDate(),  @Email,  @Active,  @IsDelete,  @UserTypeId,  @CustomerTypeId,  @Address,  @Province,  @Country,  @Zip,  @NewsletterType,  @MemberType, @IsVerified ) SELECT @@Identity";

            return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@UserId", pObj.UserId),
                new SqlParameter("@Title", pObj.Title),
                new SqlParameter("@FirstName", pObj.FirstName),
                new SqlParameter("@LastName", pObj.LastName),
                new SqlParameter("@Username", pObj.Username),
                new SqlParameter("@Password", pObj.Password),
                new SqlParameter("@RoleId", pObj.RoleId),
                new SqlParameter("@Email", pObj.Email),
                new SqlParameter("@Active", pObj.Active),
                new SqlParameter("@UserTypeId", pObj.UserTypeId),
                new SqlParameter("@IPAddress", pObj.IPAddress),
                new SqlParameter("@CustomerTypeId", pObj.CustomerTypeId),
                new SqlParameter("@Address", pObj.Address),
                new SqlParameter("@Province", pObj.Province),
                new SqlParameter("@Country", pObj.Country),
                new SqlParameter("@Zip", pObj.Zip),
                new SqlParameter("@NewsletterType", pObj.NewsletterType),
                new SqlParameter("@MemberType", pObj.MemberType),
                new SqlParameter("@IsVerified", pObj.IsVerified),
                new SqlParameter("@IsDelete", pObj.IsDelete)));
        }

        public int UpdateUser(User pObj, string sKatagorie)
        {
            string sSql = "	UPDATE [User] SET [IsVerified]=@IsVerified, [IPAddress]=@IPAddress, [Title] = @Title, [FirstName] = @FirstName, [LastName] = @LastName, [Username] = @Username, [Password] = @Password, [RoleId] = @RoleId,  [UpdateDate] = GetDate(), [Email] = @Email, [Active] = @Active, [CustomerTypeId] = @CustomerTypeId, [Address] = @Address, [Province] = @Province, [Country] = @Country, [Zip] = @Zip, [NewsletterType] = @NewsletterType, [MemberType] = @MemberType Where [UserId] = @UserId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@UserId", pObj.UserId),
                new SqlParameter("@Title", pObj.Title),
                new SqlParameter("@FirstName", pObj.FirstName),
                new SqlParameter("@LastName", pObj.LastName),
                new SqlParameter("@Username", pObj.Username),
                new SqlParameter("@Password", pObj.Password),
                new SqlParameter("@RoleId", pObj.RoleId),
                new SqlParameter("@Email", pObj.Email),
                new SqlParameter("@IPAddress", pObj.IPAddress),
                new SqlParameter("@Active", pObj.Active),
                new SqlParameter("@CustomerTypeId", pObj.CustomerTypeId),
                new SqlParameter("@Address", pObj.Address),
                new SqlParameter("@Province", pObj.Province),
                new SqlParameter("@Country", pObj.Country),
                new SqlParameter("@Zip", pObj.Zip),
                new SqlParameter("@NewsletterType", pObj.NewsletterType),
                new SqlParameter("@MemberType", pObj.MemberType),
                new SqlParameter("@IsVerified", pObj.IsVerified),
                new SqlParameter("@IsDelete", pObj.IsDelete));
        }

        public List<EmailingAttachment> GetEmailingAttachmentsByEmailId(long emailId)
        {
            List<EmailingAttachment> list = new List<EmailingAttachment>();
            DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, "SELECT * FROM EmailingAttachment WHERE EmailId = @EmailId",
                 new SqlParameter("@EmailId", emailId));
            foreach(DataRow row in ds.Tables[0].Rows)
            {
                list.Add(new EmailingAttachment(row));
            }
            return list;
        }

        public int DeleteUser(long nUserId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Update [user] set Active = 0, IsDelete=1 Where UserId=" + nUserId);
        }

        public int DeleteUserFromDatabase(long nUserId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Delete [user] Where UserId=" + nUserId);
        }

        public int AddUserToUnsubscriberList(long nUserId)
        {
            if (nUserId <= 0)
                return 0;

            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "UPDATE [Users] SET IsSubscriber = 0 AND IsReceiveEmailInfo = 0 WHERE [UserId] = " + nUserId);
        }

        public int AddKatagorie(string sKatagorie)
        {
            //string sSql = "INSERT INTO [User] ( [Title],  [FirstName],  [LastName],  [Username],  [Password],  [RoleId],  [InsertDate],  [Email],  [Active],  [IsDelete],  [UserTypeId],  [CustomerTypeId])  	VALUES ( @Title,  @FirstName,  @LastName,  @Username,  @Password,  @RoleId,  GetDate(),  @Email,  @Active,  @IsDelete,  @UserTypeId,  @CustomerTypeId  ) SELECT @@Identity";
            //sSql += sKatagorie;

            return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sKatagorie));
        }

        //public int UpdateKatagorie(User pObj, string sKatagorie)
        //{
        //    string sSql = "	UPDATE [User] SET [Title] = @Title, [FirstName] = @FirstName, [LastName] = @LastName, [Username] = @Username, [Password] = @Password, [RoleId] = @RoleId,  [UpdateDate] = GetDate(), [Email] = @Email, [Active] = @Active, [CustomerTypeId] = @CustomerTypeId, [Address] = @Address, [Province] = @Province, [Country] = @Country, [Zip] = @Zip, [NewsletterType] = @NewsletterType, [MemberType] = @MemberType Where [UserId] = @UserId";

        //    //string sSql = "	UPDATE [User] SET [Title] = @Title, [FirstName] = @FirstName, [LastName] = @LastName, [Username] = @Username, [Password] = @Password, [RoleId] = @RoleId,  [UpdateDate] = GetDate(), [Email] = @Email, [Active] = @Active, [CustomerTypeId] = @CustomerTypeId Where [UserId] = @UserId";
        //    sSql += sKatagorie;

        //    return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@UserId", pObj.UserId),
        //        new SqlParameter("@Title", pObj.Title),
        //        new SqlParameter("@FirstName", pObj.FirstName),
        //        new SqlParameter("@LastName", pObj.LastName),
        //        new SqlParameter("@Username", pObj.Username),
        //        new SqlParameter("@Password", pObj.Password),
        //        new SqlParameter("@RoleId", pObj.RoleId),
        //        new SqlParameter("@Email", pObj.Email),
        //        new SqlParameter("@Active", pObj.Active),
        //        new SqlParameter("@CustomerTypeId", pObj.CustomerTypeId),
        //        new SqlParameter("@Address", pObj.Address),
        //        new SqlParameter("@Province", pObj.Province),
        //        new SqlParameter("@Country", pObj.Country),
        //        new SqlParameter("@Zip", pObj.Zip),
        //        new SqlParameter("@NewsletterType", pObj.NewsletterType),
        //        new SqlParameter("@MemberType", pObj.MemberType),
        //        new SqlParameter("@IsDelete", pObj.IsDelete));

        //}

        public int DeleteKatagorie(int iUserId, int iKatagorieId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Delete From [Katagorie]  Where [UserId] = " + iUserId + " And [CategoryId] = " + iKatagorieId);
        }

        public bool IsExistKatagorie(int iUserId, int iCategoryId)
        {
            string sSql = "Select Count(*) From [Katagorie] Where UserId = " + iUserId + " AND CategoryId = " + iCategoryId;

            int nResult = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

            if (nResult > 0)
                return true;
            else
                return false;
        }

        public string GetPasswordFormEmail(string sEmail)
        {
            string sSql = "SELECT Password FROM [User] WHERE Email = '" + sEmail + "'";
            return DataManager.ConvertToString(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));
        }

        public bool ValidateEmail(int iUserId)
        {
            string sSql = "	UPDATE [User] SET [Active] = 1, IsVerified = 1 Where [UserId] = " + iUserId;

            int nResult = SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql);

            if (nResult > 0)
                return true;
            else
                return false;
        }

        public string GetUserEmail(int iUserId)
        {
            string sSql = "SELECT Email FROM [User] WHERE UserId = " + iUserId;
            return DataManager.ConvertToString(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));
        }
        #endregion

        #region " Permission "

        public string GetModuleUrl(int nModuleId)
        {
            string sSql = "SELECT NavigateUrl FROM Module WHERE ModuleId = " + nModuleId.ToString();
            return DataManager.ConvertToString(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));
        }

        public Permission GetMenuPermission(int nUserId, int nModuleId)
        {
            string sSql = "SELECT Permission.*, IsAdminRole, UserTypeId, [User].BranchId FROM Permission INNER JOIN [User] ON Permission.RoleId = [User].RoleId	INNER JOIN [Role] ON [User].RoleId = [Role].RoleId WHERE [User].UserId = " + nUserId.ToString() + " AND Permission.ModuleId = " + nModuleId.ToString();

            DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return new Permission(ds.Tables[0].Rows[0]);
            else
                return new Permission();
        }

        public DataSet GetPermissionByRoleId(int nParentRoleId, int nRoleId, bool bIsMasterAdmin)
        {
            string sSql = "";

            if (bIsMasterAdmin)
            {
                sSql = "SELECT m.*, ISNULL(p.PermissionId, '0') as PermissionId, ISNULL(p.AllowedView, 0) as AllowedView, ISNULL(p.AllowedAdd, 0) as AllowedAdd, ISNULL(p.AllowedEdit, 0) as AllowedEdit, ISNULL(p.AllowedDelete, 0) as AllowedDelete, ISNULL(p.AllowedPrint, 0) as AllowedPrint, ISNULL(p.FullControl, 0) as FullControl ";
                sSql += " from vModule m left join Permission p ";
                sSql += " on m.ModuleId = p.ModuleId ";
                sSql += " and p.RoleId = " + nRoleId.ToString();
                sSql += " ORDER BY ModuleCategoryId";

                return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
            }
            else
            {
                sSql = "SELECT m.*, ISNULL(p.PermissionId, 0) as PermissionId, ISNULL(p.AllowedView, 0) as AllowedView, ISNULL(p.AllowedAdd, 0) as AllowedAdd, ISNULL(p.AllowedEdit, 0) as AllowedEdit, ISNULL(p.AllowedDelete, 0) as AllowedDelete, ISNULL(p.AllowedPrint, 0) as AllowedPrint, ISNULL(p.FullControl, 0) as FullControl, ISNULL(p2.AllowedAdd, 0) as AllowedAdd2, ISNULL(p2.AllowedEdit, 0) as AllowedEdit2, ISNULL(p2.AllowedDelete, 0) as AllowedDelete2, ISNULL(p2.AllowedPrint, 0) as AllowedPrint2 ";
                sSql += " from vModule m left join Permission p  on m.ModuleId = p.ModuleId and ISNULL(p.RoleId, @RoleId) = @RoleId left join Permission p2  on m.ModuleId = p2.ModuleId and ISNULL(p2.RoleId, @ParentRoleId) = @ParentRoleId ";
                sSql += " WHERE m.ModuleId IN (select ModuleId from Permission where RoleId = @ParentRoleId AND ISNULL(AllowedView, 0) = 1) ";
                sSql += " ORDER BY ModuleCategoryId ";

                return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql, new SqlParameter("@ParentRoleId", nParentRoleId), new SqlParameter("@RoleId", nRoleId));
            }

        }


        public int AddPermission(Permission pObj)
        {
            string sSql = "INSERT INTO Permission ([RoleId],  [ModuleId],  [AllowedView],  [AllowedAdd],  [AllowedEdit],  [AllowedDelete], [AllowedPrint], [FullControl], [InsertDate],  [UpdateDate],  [Active])  	VALUES ( @RoleId,  @ModuleId,  @AllowedView,  @AllowedAdd,  @AllowedEdit,  @AllowedDelete, @AllowedPrint, @FullControl,  GETDATE(),  NULL,  1 ) SELECT @@Identity";

            return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@RoleId", pObj.RoleId),
                new SqlParameter("@ModuleId", pObj.ModuleId),
                new SqlParameter("@AllowedView", pObj.AllowedView),
                new SqlParameter("@AllowedAdd", pObj.AllowedAdd),
                new SqlParameter("@AllowedEdit", pObj.AllowedEdit),
                new SqlParameter("@AllowedDelete", pObj.AllowedDelete),
                new SqlParameter("@AllowedPrint", pObj.AllowedPrint),
                new SqlParameter("@FullControl", pObj.FullControl)));
        }

        public int UpdatePermission(Permission pObj)
        {
            string sSql = "	UPDATE Permission SET [RoleId] = @RoleId, [ModuleId] = @ModuleId, [AllowedView] = @AllowedView, [AllowedAdd] = @AllowedAdd, [AllowedEdit] = @AllowedEdit, [AllowedDelete] = @AllowedDelete, [AllowedPrint] = @AllowedPrint, [FullControl] = @FullControl, [UpdateDate] = GETDATE(), [Active] = @Active WHERE [PermissionId] = @PermissionId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@PermissionId", pObj.PermissionId),
                new SqlParameter("@RoleId", pObj.RoleId),
                new SqlParameter("@ModuleId", pObj.ModuleId),
                new SqlParameter("@AllowedView", pObj.AllowedView),
                new SqlParameter("@AllowedAdd", pObj.AllowedAdd),
                new SqlParameter("@AllowedEdit", pObj.AllowedEdit),
                new SqlParameter("@AllowedDelete", pObj.AllowedDelete),
                new SqlParameter("@AllowedPrint", pObj.AllowedPrint),
                new SqlParameter("@FullControl", pObj.FullControl),
                new SqlParameter("@Active", pObj.Active));
        }

        public int CopyRolePermission(int nFromRoleId, int nToRoleId)
        {
            string sSql = "DELETE Permission WHERE RoleId = @RoleId DELETE ModuleDetail WHERE RoleId = @RoleId";
            sSql += " INSERT INTO Permission (RoleId, ModuleId, AllowedView, AllowedAdd, AllowedEdit, AllowedDelete, InsertDate, Active, AllowedPrint) SELECT @RoleId, ModuleId, AllowedView, AllowedAdd, AllowedEdit, AllowedDelete, GETDATE(), Active, AllowedPrint FROM Permission WHERE RoleId = @ParentRoleId  ";
            sSql += " INSERT INTO ModuleDetail (RoleId, ModuleId, FieldId, AllowedView, AllowedEdit) SELECT @RoleId, ModuleId, FieldId, AllowedView, AllowedEdit FROM ModuleDetail WHERE RoleId = @ParentRoleId ";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@ParentRoleId", nFromRoleId),
                new SqlParameter("@RoleId", nToRoleId));
        }

        #endregion

        #region " Category "
        public string GetCategoryName(int nCategoryId)
        {
            return DataManager.ConvertToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                " Select CategoryName From Category Where CategoryId=" + nCategoryId));
        }
        public bool IsExistCategoryName(string sCategoryName, int nCategoryId, int nContentType)
        {
            int nCount = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                string.Format("Select Count(*) From Category Where  Active=1 And CategoryId <>{0} And CategoryName ='{1}' And ContentType={2}", nCategoryId, sCategoryName, nContentType)), 0);

            if (nCount > 0)
                return true;
            else
                return false;

        }
        public bool IsExistCategoryName(string sCategoryName, int nContentType)
        {
            int nCount = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                string.Format("Select Count(*) From Category Where Active=1 And CategoryName ='{0}' And ContentType={1}", sCategoryName, nContentType)), 0);

            if (nCount > 0)
                return true;
            else
                return false;

        }
        public int DeleteCategory(int nCategoryId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Delete Category where CategoryId=" + nCategoryId);
        }
        public DataSet GetCategory(int nContentType)
        {
            string sql = "Select *,Case Active when 1 then 'Aktive' else 'Inaktiv' end As ActiveName," +
                "	Case ContentType When 1 then 'Anzeige'  " +
                "	 When 2 then 'Infomation' " +
                "	 When 3 then 'Download'" +
                " end As ContentTypeName" +
                " From Category" +
                " Where Active=1";

            if (nContentType > 0)
                sql += string.Format(" And ContentType={0}", nContentType);

            sql += " Order by CategoryName";

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
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

            return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
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
            int nCount = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
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
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "Select * From Content Where Contentid=" + nContentId);

            if (ds.Tables[0].Rows.Count > 0)
                return new Content(ds.Tables[0].Rows[0]);

            return null;
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

            return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
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

            int nResult = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

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

            int nResult = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

            if (nResult > 0)
                return true;
            else
                return false;
        }
        #endregion

        #region " CustomerGroup "
        public DataSet GetCustomerGroup()
        {
            string sSql = "SELECT * FROM CustomerGroup ";
            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
        }

        public DataSet GetCustomerGroup(string sCustomerGroupName)
        {
            string sql = "Select *,Case Active when 1 then 'Aktive' else 'Inaktiv' end As ActiveName" +
                " From CustomerGroup";

            sql += string.Format(" Where CustomerGroupName Like '%{0}%'", sCustomerGroupName);

            sql += " Order by CustomerGroupName";

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        }

        public bool IsExistCustomerGroupName(string sCustomerGroupName)
        {
            int nCount = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                string.Format("Select Count(*) From CustomerGroup Where Active=1 And CustomerGroupName ='{0}'", sCustomerGroupName)), 0);

            if (nCount > 0)
                return true;
            else
                return false;

        }

        public bool IsExistCustomerGroupName(string sCustomerGroupName, int nCustomerGroupId)
        {
            int nCount = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                string.Format("Select Count(*) From CustomerGroup Where  Active=1 And CustomerGroupId <>{0} And CustomerGroupName ='{1}' ", nCustomerGroupId, sCustomerGroupName)), 0);

            if (nCount > 0)
                return true;
            else
                return false;

        }
        public int AddCustomerGroup(CustomerGroup pObj)
        {
            string sSql = "INSERT INTO CustomerGroup ([CustomerGroupName],  [Active])  	VALUES (@CustomerGroupName,  @Active ) SELECT @@Identity";

            return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@CustomerGroupName", pObj.CustomerGroupName),
                new SqlParameter("@Active", pObj.Active)), 0);
        }

        public int UpdateCustomerGroup(CustomerGroup pObj)
        {
            string sSql = "	UPDATE CustomerGroup SET [CustomerGroupName] = @CustomerGroupName, [Active] = @Active Where [CustomerGroupId] = @CustomerGroupId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@CustomerGroupId", pObj.CustomerGroupId),
                new SqlParameter("@CustomerGroupName", pObj.CustomerGroupName),
                new SqlParameter("@Active", pObj.Active));
        }

        public int DeleteCustomerGroup(int nCustomerGroupId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Delete CustomerGroup where CustomerGroupId=" + nCustomerGroupId);
        }
        #endregion

        #region " MailingList "
        public DataSet GetMailingList(string sFilter)
        {

            String sql = @" Select *,Case [MailingList].Active when 1 then 'Aktive' else 'Inaktiv' end As ActiveName 
                            From [MailingList] " + sFilter + " Order by MailingListName";

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        }

        public MailingList GetMailingListByID(int iMailingListId)
        {
            string sql = "Select *" +
                " From [MailingList]" +
                " Where MailingListId=" + iMailingListId;

            //if (iCustomerGroup > 0)
            //    sql += string.Format(" And CustomerGroupId={0}", iCustomerGroup);

            //sql += " Order by UserId";

            //return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return new MailingList(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public DataSet GetCustomerList(int iCustomerGroup)
        {
            string sql = "Select UserId, Email,FirstName + ' ' + LastName As CustomerName" +
                " From [User]" +
                " Where Active=1 And RoleId=0";

            if (iCustomerGroup > 0)
                sql += string.Format(" And CustomerGroupId={0}", iCustomerGroup);

            sql += " Order by FirstName, LastName";

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        }

        public bool IsExistMailingListName(string sMailingListName, int iMailingListId)
        {
            int nCount = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                string.Format("Select Count(*) From MailingList Where  Active=1 And MailingListId <>{0} And MailingListName='{1}'", iMailingListId, sMailingListName)), 0);
            if (nCount > 0)
                return true;
            else
                return false;

        }

        public int AddMailingList(MailingList pObj)
        {
            string sSql = "INSERT INTO MailingList ([MailingListName],  [Active])  	VALUES (@MailingListName,  @Active ) SELECT @@Identity";

            return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@MailingListName", pObj.MailingListName),
                new SqlParameter("@Active", pObj.Active)), 0);
        }

        public int UpdateMailingList(MailingList pObj)
        {
            string sSql = "	UPDATE MailingList SET [MailingListName] = @MailingListName, [Active] = @Active Where [MailingListId] = @MailingListId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@MailingListId", pObj.MailingListId),
                new SqlParameter("@MailingListName", pObj.MailingListName),
                new SqlParameter("@Active", pObj.Active));
        }

        public int UpdateMailingList(MailingList pObj, string sTemp)
        {
            string sSql = "	UPDATE MailingList SET [MailingListName] = @MailingListName, [Active] = @Active Where [MailingListId] = @MailingListId";
            sSql += sTemp;

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@MailingListId", pObj.MailingListId),
                new SqlParameter("@MailingListName", pObj.MailingListName),
                new SqlParameter("@Active", pObj.Active));
        }

        public int DeleteMailingList(int nMailingListId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Delete MailingList where MailingListId=" + nMailingListId + " Delete CustomerMailList where MailingListId=" + nMailingListId);
        }

        public int DeleteCustomerMailingList(int nMailingListId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "Delete From CustomerMailList Where MailingListId =" + nMailingListId);
        }

        //public int DeleteCustomerMailingListByUserId(string sSql)
        //{
        //    return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));
        //}

        public int UnSubscribeUserEmail(int nUserId)
        {
            string sSql = "Delete From Katagorie Where UserId=" + nUserId;
            sSql += " Update [User] Set NewsletterType=0 Where UserId=" + nUserId;

            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sSql);

            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, "Delete From CustomerMailList Where UserId=" + nUserId);
        }

        public int AddCustomerList(string sCList)
        {
            return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sCList));
        }

        public int AddCustomerMailingList(int nMailingListId, int nUserId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                string.Format("Insert into CustomerMailList (MailingListId,UserId) values({0},{1})", nMailingListId, nUserId));
        }
        public bool IsExistMailingList(int iMailingListId, int iUserId)
        {
            string sSql = "Select Count(*) From [MailingList] Where UserId = " + iUserId + " AND MailingListId = " + iMailingListId;

            int nResult = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

            if (nResult > 0)
                return true;
            else
                return false;
        }
        public bool IsExistCustomerMailingList(int iMailingListId, int iUserId)
        {
            string sSql = "Select Count(*) From [CustomerMailList] Where UserId = " + iUserId + " AND MailingListId = " + iMailingListId;

            int nResult = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));

            if (nResult > 0)
                return true;
            else
                return false;
        }
        public DataSet GetKatagorieList()
        {
            string sql = @"SELECT DISTINCT Category.CategoryName, Katagorie.CategoryId
                            FROM Category INNER JOIN
                            Katagorie ON Category.CategoryId = Katagorie.CategoryId
                            Order by Category.CategoryName";

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        }
        public DataSet GetCustomerInCategory(string sFilter)
        {
            string sSql = string.Format(@"SELECT CategoryName,Category.CategoryId, [User].UserId, [User].FirstName+' '+ [User].LastName as CustomerName,Email
                                            FROM Katagorie INNER JOIN
                                            [User] ON Katagorie.UserId = [User].UserId
                                            inner Join Category On Category.CategoryId = Katagorie.CategoryId
                                            WHERE [User].Active=1 and ISNULL(NewsletterType, 0) > 0 and isnull(IsVerified,0)=1 {0}
                                            ORDER BY CustomerName", sFilter);

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sSql);
        }

        public DataSet GetCustomerMailingList(int nMailingListId)
        {
            string sSql = @"SELECT Distinct [User].UserId, [User].FirstName+' '+ [User].LastName as CustomerName, Email
                FROM CustomerMailList INNER JOIN [User] ON CustomerMailList.UserId = [User].UserId
                WHERE [User].IsDelete=0 And MailingListId = @MailingListId AND ISNULL(Email, '') <> '' ";
            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql, new SqlParameter("@MailingListId", nMailingListId));
        }
        public DataSet GetCustomerKat(string sql)
        {

            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sql);
        }
        #endregion

        #region " Template "
        public int AddTemplate(EmailTemplate pObj)
        {
            string sSql = @"INSERT INTO EmailTemplate ([CategoryId],  [Name],  [Description],  
                        [HtmlDetail],  [TextDetail],  [Active],  [UserId],  [InsertDate], FileName1, FileName2, FileName3, FileName4, FileName5, FileDescription1, FileDescription2, FileDescription3, FileDescription4, FileDescription5, FileUrl1, FileUrl2, FileUrl3, FileUrl4, FileUrl5)  	
                        VALUES (@CategoryId,  @Name,  @Description,  
                        @HtmlDetail,  @TextDetail,  @Active,  @UserId, GetDate(), @FileName1, @FileName2, @FileName3, @FileName4, @FileName5, @FileDescription1, @FileDescription2, @FileDescription3, @FileDescription4, @FileDescription5, @FileUrl1, @FileUrl2, @FileUrl3, @FileUrl4, @FileUrl5) 
                        SELECT @@Identity";

            return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@CategoryId", pObj.CategoryId),
                new SqlParameter("@Name", pObj.Name),
                new SqlParameter("@Description", pObj.Description),
                new SqlParameter("@HtmlDetail", pObj.HtmlDetail),
                new SqlParameter("@TextDetail", pObj.TextDetail),
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

        public int UpdateTemplate(EmailTemplate pObj)
        {
            string sSql = @"	UPDATE EmailTemplate SET [CategoryId] = @CategoryId, [Name] = @Name, 
                        [Description] = @Description, [HtmlDetail] = @HtmlDetail, [TextDetail] = @TextDetail, 
                        [Active] = @Active, [UserId] = @UserId, [UpdateDate] = GetDate(), FileName1 = @FileName1, FileName2 = @FileName2, FileName3 = @FileName3, FileName4 = @FileName4, FileName5 = @FileName5, FileDescription1 = @FileDescription1, FileDescription2 = @FileDescription2, FileDescription3 = @FileDescription3, FileDescription4 = @FileDescription4, FileDescription5 = @FileDescription5, FileUrl1 = @FileUrl1, FileUrl2 = @FileUrl2, FileUrl3 = @FileUrl3, FileUrl4 = @FileUrl4, FileUrl5 = @FileUrl5
                        Where [TemplateId] = @TemplateId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@TemplateId", pObj.TemplateId),
                new SqlParameter("@CategoryId", pObj.CategoryId),
                new SqlParameter("@Name", pObj.Name),
                new SqlParameter("@Description", pObj.Description),
                new SqlParameter("@HtmlDetail", pObj.HtmlDetail),
                new SqlParameter("@TextDetail", pObj.TextDetail),
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

        public int DeleteTemplate(int nTemplateId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Delete From EmailTemplate Where TemplateId=" + nTemplateId);
        }

        public EmailTemplate GetTemplate(int nTemplateId)
        {
            DataSet ds = SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, "Select * From EmailTemplate Where TemplateId=" + nTemplateId);

            if (ds.Tables[0].Rows.Count > 0)
                return new EmailTemplate(ds.Tables[0].Rows[0]);

            return null;
        }

        public DataSet GetTemplate(string sFilter)
        {
            string sql = string.Format("Select * From EmailTemplate Where 1 =1 {0}", sFilter);

            //if (nTemplateType > 0)
            //    sql += " And Template.TemplateType=" + nTemplateType;

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text, sql);
        }
        #endregion

        #region " Emailing "
        public int DeleteEmailing(int nEmailId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "Delete From Emailing Where EmailId=" + nEmailId);
        }
        public int AddEmailing(Emailing pObj)
        {
            string sSql = @"INSERT INTO Emailing (EmailName, [Subject], FromName, FromEmail, EmailFormatId, TemplateId, InsertDate, UpdateDate, Active, UserId, StatusId, HtmlDetail, TextDetail)
                VALUES(@EmailName, @Subject, @FromName, @FromEmail, @EmailFormatId, @TemplateId, GETDATE(), NULL, 1, @UserId, @StatusId, @HtmlDetail, @TextDetail)
                SELECT @@IDENTITY";

            return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@EmailName", pObj.EmailName),
                new SqlParameter("@Subject", pObj.Subject),
                new SqlParameter("@FromName", pObj.FromName),
                new SqlParameter("@FromEmail", pObj.FromEmail),
                new SqlParameter("@EmailFormatId", pObj.EmailFormatId),
                new SqlParameter("@TemplateId", pObj.TemplateId),
                new SqlParameter("@UserId", pObj.UserId),
                new SqlParameter("@HtmlDetail", pObj.HtmlDetail),
                new SqlParameter("@TextDetail", pObj.TextDetail),
                new SqlParameter("@StatusId", pObj.StatusId)), -1);
        }

        public int UpdateEmailing(Emailing pObj)
        {
            string sSql = @"UPDATE Emailing
                SET EmailName = @EmailName, [Subject] = @Subject, FromName = @FromName, FromEmail = @FromEmail, HtmlDetail = @HtmlDetail, TextDetail = @TextDetail,
	                EmailFormatId = @EmailFormatId, TemplateId = @TemplateId, UserId = @UserId, StatusId = @StatusId, UpdateDate = GETDATE()
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
                new SqlParameter("@HtmlDetail", pObj.HtmlDetail),
                new SqlParameter("@TextDetail", pObj.TextDetail),
                new SqlParameter("@StatusId", pObj.StatusId));
        }

        public DataSet GetEmailings(string sFilter)
        {
            string sSql = "SELECT * FROM Emailing WHERE Active = 1 " + sFilter + " ORDER BY InsertDate DESC, EmailName";
            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
        }

        public Emailing GetEmailing(int nEmailId)
        {
            string sSql = @"SELECT Emailing.*, EmailTemplate.Name AS TemplateName
                FROM Emailing LEFT OUTER JOIN EmailTemplate ON Emailing.TemplateId = EmailTemplate.TemplateId
                WHERE EmailId = @EmailId";
            DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql, new SqlParameter("@EmailId", nEmailId));
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return new Emailing(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        #endregion

        #region " EmailQue "

        public int AddEmailQue(EmailQue pObj)
        {
            string sCheck = string.Format("Select count(CustomerId) from EmailQue Where EmailId={0} and Email='{1}'",
                pObj.EmailId, pObj.Email);

            int nResult = DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                sCheck), 0);

            if (nResult == 0)
            {
                string sSql = @"INSERT INTO EmailQue (EmailId, CustomerId, Email, [Status], InsertDate, SendDate, IsError, Resent)
                    VALUES(@EmailId, @CustomerId, @Email, @Status, GETDATE(), @SendDate, @IsError, 0)
                    SELECT @@IDENTITY";

                return DataManager.ConvertToInteger(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                    new SqlParameter("@EmailId", pObj.EmailId),
                    new SqlParameter("@CustomerId", pObj.CustomerId),
                    new SqlParameter("@Email", pObj.Email),
                    new SqlParameter("@Status", pObj.Status),
                    new SqlParameter("@SendDate", pObj.SendDate),
                    new SqlParameter("@IsError", pObj.IsError)));
            }
            else
                return 0;
        }

        public DataSet GetEmailingStatus(long nEmailId)
        {
            string sSql = @"SELECT EmailId, SUM(Scheduled) AS Scheduled, SUM(Pending) AS Pending, SUM(Sent) AS [Sent],
	            SUM(Error) AS Error, SUM(Resent) AS Resent
            FROM
            (
	            SELECT EmailId, 1 AS Scheduled, CASE WHEN [Status] = 0 THEN 1 ELSE 0 END AS [Pending], 
		            CASE WHEN [Status] = 1 THEN 1 ELSE 0 END AS [Sent],
		            CASE WHEN [Status] = -1 THEN 1 ELSE 0 END AS [Error],
		            CASE WHEN [Status] = 2 THEN 1 ELSE 0 END AS [Resent]	
	            FROM EmailQue
	            WHERE EmailId = @EmailId
            ) tblTemp
            GROUP BY EmailId";

            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql, new SqlParameter("@EmailId", nEmailId));
        }

        public DataSet GetEmailQueData(long nEmailId, string sFilter)
        {
            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.Text,
               string.Format("SELECT EmailQue.*,FirstName + ' ' + LastName As CustomerName FROM EmailQue left outer Join [user] On [user].UserId=EmailQue.customerId  Where EmailId={0} {1}", nEmailId, sFilter));
        }

        public int SetBouncedEmailQue(long nEmailQueId)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                "UPDATE [EmailQue] SET IsBounced = 1 WHERE [EmailQueId] = " + nEmailQueId);
        }
        #endregion

        #region " Option "

        public string GetOption(string sOptionName)
        {
            string sSql = String.Format("SELECT OptionValue FROM Options WHERE OptionName = '{0}' ", sOptionName);
            return DataManager.ConvertToString(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql));
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
                        options.Add(keys[i], DataManager.ConvertToString(rows[0]["OptionValue"]));
                    }
                    else
                    {
                        options.Add(keys[i], string.Empty);
                    }
                }
            }
            return options;
        }

        #endregion

        #region " Email Service "

        public DataSet GetPendingEmail()
        {
            //            string sSql = @"SELECT TOP 20 ISNULL(TitleId,0) As TitleId,[User].UserId,EmailQue.*, [Subject], FromName, FromEmail, Emailing.HtmlDetail, Emailing.TextDetail, 
            //                                ISNULL(FirstName + ' ' + LastName, EmailQue.Email) AS CustomerName, ISNULL([User].NewsletterType, 1) AS NewsletterType,
            //                                FileName1, FileName2, FileName3, FileName4, FileName5, FileUrl1, FileUrl2, FileUrl3, FileUrl4, FileUrl5
            //                            FROM EmailQue
            //                                INNER JOIN Emailing ON EmailQue.EmailId = Emailing.EmailId
            //                                Left outer JOIN EmailTemplate ON Emailing.TemplateId = EmailTemplate.TemplateId 
            //                                LEFT OUTER JOIN [User] ON EmailQue.CustomerId = [User].UserId 
            //	                            LEFT OUTER JOIN Title On Title.TitleId = [user].Title
            //                            WHERE EmailQue.[Status] = 0 And EmailQue.Email <> ''
            //                                AND Emailing.Active = 1 
            //                            ORDER BY InsertDate "; //And ISNULL([User].Newslettertype,0) >0 


            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.StoredProcedure, "GetPendingEmail");
        }

        public int UpdateEmailQueStatus(int nEmailQueId, int nStatusId, DateTime dSendDate, bool bIsError, string sMessage, bool bIsResent)
        {
            string sSql = "UPDATE EmailQue SET Status = @Status, SendDate = @SendDate, IsError = @IsError, ReturnMessage = @ReturnMessage, Resent = CASE WHEN @IsResent = 1 THEN ISNULL(Resent, 0) + 1 ELSE 0 END  WHERE EmailQueId = @EmailQueId";
            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@Status", nStatusId),
                new SqlParameter("@SendDate", dSendDate),
                new SqlParameter("@IsError", bIsError),
                new SqlParameter("@IsResent", bIsResent),
                new SqlParameter("@EmailQueId", nEmailQueId),
                new SqlParameter("@ReturnMessage", sMessage));
        }

        public DataSet GetFailedEmail()
        {
            //            string sSql = @"SELECT TOP 20 ISNULL(TitleId,0) As TitleId,[User].UserId,EmailQue.*, [Subject], FromName, FromEmail, Emailing.HtmlDetail, Emailing.TextDetail, 
            //                                ISNULL(FirstName + ' ' + LastName, EmailQue.Email) AS CustomerName, ISNULL([User].NewsletterType, 1) AS NewsletterType,
            //                                FileName1, FileName2, FileName3, FileName4, FileName5, FileUrl1, FileUrl2, FileUrl3, FileUrl4, FileUrl5
            //                            FROM EmailQue
            //                                INNER JOIN Emailing ON EmailQue.EmailId = Emailing.EmailId
            //                                Left outer JOIN EmailTemplate ON Emailing.TemplateId = EmailTemplate.TemplateId 
            //                                LEFT OUTER JOIN [User] ON EmailQue.CustomerId = [User].UserId 
            //	                            LEFT OUTER JOIN Title On Title.TitleId = [user].Title
            //                            WHERE EmailQue.[Status] = -1 And EmailQue.Email <> ''
            //                                AND Emailing.Active = 1 
            //                            ORDER BY InsertDate ";//And ISNULL([User].Newslettertype,0) >0 

            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.StoredProcedure, "GetFailedEmail");
        }

        public DataSet GetEmailKey()
        {
            string sSql = "SELECT * FROM EmailKey";
            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql);
        }

        #endregion

        #region " Impressum"
        public string GetImpressum(int nImpressumId)
        {
            return DataManager.ConvertToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                "Select Detail From Impressum Where ImpressumId=" + nImpressumId));
        }

        public int UpdateImpressum(int nImpressumId, string sDetail)
        {
            return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text,
                  "Update Impressum Set Detail = @Detail Where ImpressumId=@ImpressumId"
                , new SqlParameter("@Detail", sDetail)
                , new SqlParameter("@ImpressumId", nImpressumId));
        }
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
            return DataManager.ConvertToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text,
                "Select Prefix From PrefixText Where TitleId=" + nTitleId));
        }
        #endregion
    }// end class

}// end namespace