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

        #region " CustomerCreditCard " 

        public int AddCustomerCreditCard(CustomerCreditCard pObj)
        {
            string sSql = @"INSERT INTO CustomerCreditCard ([UserId],  [CardTypeId], 
                        [CardNumberE],  [CardNumberX],  [CardHolderName],  [CardExpireX],  
                        [InsertDate],  [UpdateDate],  [Active]) VALUES (@UserId,  @CardTypeId,  
                        @CardNumberE,  @CardNumberX,  @CardHolderName,  
                        @CardExpireX,  GETDATE(), NULL, 1) SELECT @@Identity";

            return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@UserId", pObj.UserId),
                new SqlParameter("@CardTypeId", pObj.CardTypeId),
                new SqlParameter("@CardNumberE", pObj.CardNumberE),
                new SqlParameter("@CardNumberX", pObj.CardNumberX),
                new SqlParameter("@CardHolderName", pObj.CardHolderName),
                new SqlParameter("@CardExpireX", pObj.CardExpireX)), -1);
        }

        public int UpdateCustomerCreditCard(CustomerCreditCard pObj)
        {
            string sSql = @"UPDATE CustomerCreditCard SET [UserId] = @UserId, 
                            [CardTypeId] = @CardTypeId, [CardNumberE] = @CardNumberE, [CardNumberX] = @CardNumberX, 
                            [CardHolderName] = @CardHolderName, [CardExpireX] = @CardExpireX, [UpdateDate] = GETDATE() 
                            WHERE [CreditCardId] = @CreditCardId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@CreditCardId", pObj.CreditCardId),
                new SqlParameter("@UserId", pObj.UserId),
                new SqlParameter("@CardTypeId", pObj.CardTypeId),
                new SqlParameter("@CardNumberE", pObj.CardNumberE),
                new SqlParameter("@CardNumberX", pObj.CardNumberX),
                new SqlParameter("@CardHolderName", pObj.CardHolderName),
                new SqlParameter("@CardExpireX", pObj.CardExpireX));
        }


        public int UpdateNewletter(bool emailInfo, int userId)
        {
            string sSql = "UPDATE Users SET IsReceiveEmailInfo = @emailInfo WHERE UserId = @userId ";
            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@emailInfo", emailInfo),
                new SqlParameter("@userId", userId));
        }

        public int DeleteCustomerCredit(int nCreditCardId, int nUpdateUserId)
        {
            string sSql = "UPDATE CustomerCreditCard SET Active=0, UpdateUserId=@UpdateUserId WHERE CreditCardId=@CreditCardId";
            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@CreditCardId", nCreditCardId),
                new SqlParameter("@UpdateUserId", nUpdateUserId));
        }

        public DataSet GetCustomerCreditCards(int nUserId)
        {
            string sSql = "SELECT * FROM CustomerCreditCard WHERE UserId = @UserId AND Active = 1";
            return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@UserId", nUserId));
        }

        public CustomerCreditCard GetCustomerCreditCard(int nCreditCardId)
        {
            CustomerCreditCard obj = null;

            using (var db = GetModels())
            {
                db.CustomerCreditCards.Where(it => it.CreditCardId == nCreditCardId).ToList().ForEach(it => obj = it);
            }

            return obj;
        }

        #endregion

        //#region " UserMessage "

        //public int AddUserMessage(UserMessage pObj)
        //{
        //    string sSql = "INSERT INTO UserMessage ([MessageTypeId],  [Subject],  [FromUserId],  [ToUserId],  [Body],  [SentDate],  [Active],  [HasAttachedFile],  [IsFlag])  	VALUES (@MessageTypeId,  @Subject,  @FromUserId,  @ToUserId,  @Body,  @SentDate,  1,  @HasAttachedFile,  @IsFlag  ) SELECT @@Identity";

        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@MessageTypeId", pObj.MessageTypeId),
        //        new SqlParameter("@Subject", pObj.Subject),
        //        new SqlParameter("@FromUserId", pObj.FromUserId),
        //        new SqlParameter("@ToUserId", pObj.ToUserId),
        //        new SqlParameter("@Body", pObj.Body),
        //        new SqlParameter("@SentDate", pObj.SentDate),
        //        new SqlParameter("@HasAttachedFile", pObj.HasAttachedFile),
        //        new SqlParameter("@IsFlag", pObj.IsFlag)), -1);
        //}

        //public int UpdateUserMessageRead(int nMessageId, DateTime dReadDate)
        //{
        //    string sSql = "UPDATE UserMessage SET ReadDate = @ReadDate WHERE MessageId = @MessageId";
        //    return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@MessageId", nMessageId),
        //        new SqlParameter("@ReadDate", dReadDate));
        //}

        //public int UpdateUserMessageFlag(int nMessageId, bool bIsFlag)
        //{
        //    string sSql = "UPDATE UserMessage SET IsFlag = @IsFlag WHERE MessageId = @MessageId";
        //    return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@MessageId", nMessageId),
        //        new SqlParameter("@IsFlag", bIsFlag));
        //}

        //public DataSet GetUserMessages(int nUserId)
        //{
        //    //string sSql = "SELECT * FROM UserMessage JOIN Users ON Users.UserId = UserMessage.ToUserId WHERE UserMessage.Active = 1 AND ToUserId = @UserId ORDER BY SentDate DESC";
        //    string sSql = @"SELECT U1.*, U2.FirstName + ' ' + U2.LastName AS FromUserName,
	       //                 U3.FirstName + ' ' + U3.LastName AS ToUserName 
        //                    FROM UserMessage U1
	       //                 INNER JOIN Users U2 ON U1.FromUserId = U2.UserId
	       //                 INNER JOIN Users U3 ON U1.ToUserId = U3.UserId
        //                    WHERE U1.Active = 1 AND ToUserId = @UserId 
        //                    ORDER BY SentDate DESC";

        //    return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@UserId", nUserId));
        //}

        //public DataSet GetUserMessage(int nMessageId)
        //{
        //    //string sSql = "SELECT * FROM UserMessage WHERE Active = 1 AND MessageId = @MessageId";
        //    string sSql = @"SELECT U1.*, U2.FirstName + ' ' + U2.LastName AS FromUserName,
	       //                 U3.FirstName + ' ' + U3.LastName AS ToUserName 
        //                    FROM UserMessage U1
	       //                 INNER JOIN Users U2 ON U1.FromUserId = U2.UserId
	       //                 INNER JOIN Users U3 ON U1.ToUserId = U3.UserId
        //                    WHERE U1.Active = 1 AND MessageId = @MessageId";

        //    return SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@MessageId", nMessageId));

        //    //if (ds != null && ds.Tables[0].Rows.Count > 0)
        //    //    return new UserMessage(ds.Tables[0].Rows[0]);
        //    //else
        //    //    return null;
        //}

        //#endregion

        //#region " DLG Card "

        //public int AddDLGCardObj(DLGCardObj pObj)
        //{
        //    string sSql = "INSERT INTO DLGCard ([ItemId],  [SaleId],  [FirstName],  [LastName],  [Email],  [CardNumber],  [Message],  [BeginBalance],  [InsertDate],  [UpdateDate],  [UserId],  [Active])  	VALUES ( @ItemId,  @SaleId,  @FirstName,  @LastName,  @Email,  @CardNumber,  @Message,  @BeginBalance, GETDATE(),  NULL,  @UserId,  1 ) SELECT @@Identity";

        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@ItemId", pObj.ItemId),
        //        new SqlParameter("@SaleId", pObj.SaleId),
        //        new SqlParameter("@FirstName", pObj.FirstName),
        //        new SqlParameter("@LastName", pObj.LastName),
        //        new SqlParameter("@Email", pObj.Email),
        //        new SqlParameter("@CardNumber", pObj.CardNumber),
        //        new SqlParameter("@Message", pObj.Message),
        //        new SqlParameter("@BeginBalance", pObj.BeginBalance),
        //        new SqlParameter("@UserId", pObj.UserId)));
        //}

        //public DLGCardObj GetDLGCardObj(int nDlgCardId)
        //{
        //    string sSql = "SELECT * FROM DLGCard WHERE Active = 1 AND DLGCardId = @DLGCardId";
        //    DataSet ds = SqlHelper.ExecuteDataset(this.ConnectionString, CommandType.Text, sSql, new SqlParameter("@DLGCardId", nDlgCardId));
        //    if (ds != null && ds.Tables[0].Rows.Count > 0)
        //        return new DLGCardObj(ds.Tables[0].Rows[0]);
        //    else
        //        return null;
        //}


        //public int UpdateDLGCardActive(int nDLGCardId, bool bActive)
        //{
        //    string sSql = "UPDATE DLGCard SET Active = 0 WHERE DLGCardId = @DLGCardId";
        //    return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql, new SqlParameter("@DLGCardId", nDLGCardId));
        //}

        //#endregion

        //#region " DLGCardBalance "

        //public int AddDLGCardBalance(DLGCardBalance pObj)
        //{
        //    string sSql = "INSERT INTO DLGCardBalance ([DLGCardId],  [UserId],  [ActionType],  [Debit],  [Credit],  [Balance],  [InsertDate],  [SaleId],  [Active])  	VALUES ( @DLGCardId,  @UserId,  @ActionType,  @Debit,  @Credit,  @Balance,  @InsertDate,  @SaleId,  1 ) SELECT @@Identity";

        //    return DataManager.ToInt(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql,
        //        new SqlParameter("@DLGCardId", pObj.DLGCardId),
        //        new SqlParameter("@UserId", pObj.UserId),
        //        new SqlParameter("@ActionType", pObj.ActionType),
        //        new SqlParameter("@Debit", pObj.Debit),
        //        new SqlParameter("@Credit", pObj.Credit),
        //        new SqlParameter("@Balance", pObj.Balance),
        //        new SqlParameter("@InsertDate", pObj.InsertDate),
        //        new SqlParameter("@SaleId", pObj.SaleId)));
        //}

        //public decimal GetDLGCardBalance(int nDlgCardId)
        //{
        //    string sSql = "SELECT TOP 1 Balance FROM DLGCardBalance WHERE DLGCardId = @DLGCardId ORDER BY InsertDate DESC, ID DESC";
        //    return DataManager.ToDecimal(SqlHelper.ExecuteScalar(this.ConnectionString, CommandType.Text, sSql, new SqlParameter("@DLGCardId", nDlgCardId)));
        //}


        //#endregion

        #region " Site Footer "

        public List<Site> GetSiteForFooter(int nLangId)
        {
            List<Site> pResult = null;

            using (DLGEntities db = new DLGEntities(ConnectionString))
            {
                pResult = (from it in db.Sites
                           join sl in db.SiteLangs on it.SiteId equals sl.SiteId
                           where sl.LangId == nLangId
                           select new Site
                           {
                               SiteId = it.SiteId,
                               SiteName = sl.SiteName
                           }).ToList();
            }

            return pResult;
        }

        #endregion

        #region " SubScribe "

        public int SubScribeCustomer(string sEmail)
        {
            int nResult = 0;
            User pUser = this.GetUserByEmail(sEmail);
            if (pUser == null)
            {
                // Subscripe new user
                pUser = new User();
                pUser.Email = sEmail;
                pUser.FirstName = sEmail.Substring(0, sEmail.IndexOf("@") + 1);
                if (pUser.FirstName.Length == 0)
                    pUser.FirstName = sEmail;
                pUser.LastName = "";
                pUser.UserTypeId = UserType.Type.Customer;
                pUser.Password = StringHelper.RandomString(6);
                pUser.PasswordEncrypted = DataProtection.Encrypt(pUser.Password);
                pUser.Active = true;
                pUser.IsReceiveEmailInfo = true;
                pUser.IsSubscriber = true;
                pUser.InsertDate = DateTime.UtcNow;
                pUser.UpdateDate = DateTime.UtcNow;
                pUser.ExpiredDate = DateTime.UtcNow.AddDays(365);
                pUser.Birthdate = DateTime.UtcNow;
                pUser.RegisteredDate = DateTime.UtcNow;
                pUser.LastLoggedOn = DateTime.UtcNow;

                nResult = this.AddUser(pUser);
                if (nResult > 0)
                    AddDefaultInterseted(nResult);
            }
            else if (pUser.IsReceiveEmailInfo == false)
            {
                pUser.IsSubscriber = pUser.IsReceiveEmailInfo = true;
                nResult = UpdateUser(pUser);
            }
            else
            {
                pUser.IsReceiveEmailInfo = pUser.IsSubscriber = true;
                nResult = -2;
            }
            return nResult;
        }

        public int SubScribeReseller(string sEmail)
        {
            int nResult = 0;
            User pUser = this.GetUserByEmail(sEmail);
            if (pUser == null)
            {
                // Subscripe new user
                pUser = new User();
                pUser.Email = sEmail;
                pUser.FirstName = sEmail.Substring(0, sEmail.IndexOf("@") + 1);
                if (pUser.FirstName.Length == 0)
                    pUser.FirstName = sEmail;
                pUser.LastName = "";
                pUser.UserTypeId = UserType.Type.Reseller;
                pUser.Password = "";
                pUser.Active = true;
                pUser.IsReceiveEmailInfo = true;
                pUser.IsSubscriber = true;

                nResult = this.AddUser(pUser);
                if (nResult > 0)
                    AddDefaultInterseted(nResult);
            }
            else
            {
                // This email is already subscribed
                nResult = -2;
            }
            return nResult;
        }

        public User GetUserByEmail(string sEmail)
        {
            User obj = null;

            using (var db = GetModels())
            {
                db.Users.Where(it => it.Email == sEmail).ToList().ForEach(it => obj = it);
            }

            return obj;
        }

        public int AddUserInterested(int interestId, int nUserId)
        {
            string sSql = @"INSERT INTO UserInterested (InterestId, UserId) VALUES (@InterestId,@UserId)";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@InterestId", interestId)
                , new SqlParameter("@UserId", nUserId));
        }

        public int DeleteUserInterestedByUserId(int nUserId)
        {
            string sSql = @"DELETE from [dbo].[UserInterested] WHERE [UserId] = @UserId";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@UserId", nUserId));
        }


        private int AddDefaultInterseted(int nUserId)
        {
            string sSql = @"IF (SELECT COUNT(InterestId) FROM UserInterested WHERE UserId = @UserId) = 0
BEGIN 
	INSERT INTO UserInterested (InterestId, UserId) (SELECT InterestId, @UserID FROM Interest WHERE Active = 1)
END";

            return SqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.Text, sSql,
                new SqlParameter("@UserId", nUserId));
        }

        #endregion

    }
}
