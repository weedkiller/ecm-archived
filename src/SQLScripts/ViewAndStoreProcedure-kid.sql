GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetInterest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetInterest]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllUser]




/****** Object:  StoredProcedure [dbo].[GetInterest]    Script Date: 3/22/2015 7:05:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		kit
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetInterest]
	@LangId INT 
AS
BEGIN

	SET NOCOUNT ON;

   SELECT il.[InterestId]
      ,il.[LangId]
      ,il.[InterestName]
      ,il.[InterestDesc]
	FROM [dbo].[InterestLang] il
	INNER JOIN [dbo].[Interest] i ON il.InterestId = i.InterestId 
	WHERE [LangId] = @LangId
	AND [i].[Active] = 1
END

GO
ALTER PROCEDURE [dbo].[GetUserMessage]
	@StartIndex INT = NULL,
    @MaxRetriveItem INT = NULL,
    @SortColumnName NVARCHAR(50) = NULL,
    @SortDirection INT = NULL,
	@SearchText NVARCHAR(50) = NULL,
	@UserId INT = NULL,
	@messageId INT = NULL
	AS
	BEGIN
		SET NOCOUNT ON;
		WITH CTE_Result AS
	(
		SELECT [Umess].[MessageId],[Umess].[MessageTypeId] ,[Umess].[Subject],[Umess].[FromUserId],[Umess].[ToUserId],[Umess].[Body],
		[Umess].[SentDate],[Umess].[ReadDate],[Umess].[Active],[Umess].[HasAttachedFile],[Umess].[IsFlag]
		,[UmessAtt].[AttachedId],[UmessAtt].[BaseFileName],[UmessAtt].[FileExtension],[UmessAtt].[FileName],[UmessAtt].[InsertDate],
		us.FirstName,us.LastName,
		(SELECT [FirstName] + ' ' + [LastName] from [dbo].[Users] WHERE [userId] =[Umess].[FromUserId]) as FromName
		,(SELECT [FirstName] + ' ' + [LastName] from [dbo].[Users] WHERE [userId] =[Umess].[ToUserId]) as ToName
		,ROW_NUMBER() OVER(
										ORDER BY 
												CASE WHEN @SortDirection = 0 THEN 
													CASE 
														WHEN @SortColumnName IS NULL THEN  [Umess].[MessageId]
													END
												END ASC, 
												CASE WHEN @SortDirection = 1 THEN 
													CASE 
															WHEN @SortColumnName IS NULL THEN [Umess].[MessageId]
													END
												END DESC
									) AS RowNumber
		  FROM [dbo].[UserMessage] Umess
		  Left JOIN [dbo].[UserMessageAttached] UmessAtt ON Umess.MessageId = UmessAtt.MessageId
		  INNER JOIN [dbo].[Users] us ON Umess.FromUserId = us.UserId 
		  WHERE (@SearchText IS NULL OR [Umess].[Subject] LIKE '%' + @SearchText +'%')
		  AND (@UserId IS NULL OR [Umess].ToUserId = @UserId)
		  AND (@messageId IS NULL OR [Umess].MessageId = @messageId)
	)

	SELECT *,  (SELECT MAX(RowNumber) FROM CTE_Result) ToltalItemCount
	FROM CTE_Result
	WHERE 
			RowNumber > @StartIndex
			AND  (@MaxRetriveItem IS NULL OR @MaxRetriveItem <= 0 OR (RowNumber <=  (@StartIndex + @MaxRetriveItem)))
	ORDER BY [MessageId] DESC 
	END

	GO

	CREATE PROCEDURE [dbo].[GetAllUser]
	AS
	BEGIN

		SET NOCOUNT ON;

		SELECT
		   [UserId]
		  ,[FirstName]
		  ,[LastName]
	  FROM [dbo].[Users]
	  WHERE [Active] = 1

	END
	GO

ALTER PROCEDURE [dbo].[AddUserMessage]
@Subject NVARCHAR(500) = null,
@FromUserId INT = null,
@ToUserId INT = null,
@Body NVARCHAR(500) = null,
@BaseFileName NVARCHAR(500) = null,
@FileExtension NVARCHAR(50) = null,
@RandomfileName NVARCHAR(500) = null,
@Result INT OUT
AS
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#Tempuser') IS NOT NULL DROP Table #Tempuser

	CREATE TABLE #Tempuser (UserId Int);

	IF @ToUserId = -1
	BEGIN
		INSERT INTO #Tempuser (UserId)
		select [UserId] FROM [dbo].[Users]
	END
	ELSE
	BEGIN
		INSERT INTO #Tempuser (UserId)
		select [UserId] FROM [dbo].[Users] WHERE [UserId] = @ToUserId
	END


	DECLARE @tempuserid int 
	DECLARE @MessageId int
	Declare @hasfile bit = 0

	IF @FileExtension IS NOT NULL
	BEGIN
		set @hasfile = 1
	END

	while EXISTS(SELECT TOP 1 1 FROM #Tempuser)
	BEGIN
		--Get row from cursor to process
		SELECT TOP 1 @tempuserid = [UserId]  FROM #Tempuser  

		INSERT INTO [dbo].[UserMessage]
           (
           [Subject]
           ,[FromUserId]
           ,[ToUserId]
           ,[Body]
           ,[SentDate]
           ,[ReadDate]
           ,[Active]
		   ,[HasAttachedFile]
           )
     VALUES
           (@Subject 
           ,@FromUserId
           ,@tempuserid
           ,@Body
           ,GETUTCDATE()
           ,null
           ,1
		   ,@hasfile)

		 SET @MessageId =  SCOPE_IDENTITY()

		 If @FileExtension IS NOT NULL
		 BEGIN
			 INSERT INTO [dbo].[UserMessageAttached]
			   ([MessageId]
			   ,[BaseFileName]
			   ,[FileExtension]
			   ,[FileName]
			   ,[InsertDate])
			 VALUES
				   (@MessageId
				   ,@BaseFileName
				   ,@FileExtension
				   ,@RandomfileName
				   ,GETUTCDATE())
		 END

		--Remove processed row from cursor.
		DELETE FROM #Tempuser WHERE [UserId] = @tempuserid  
	END
	DROP TABLE #Tempuser
	                                 
         
	SET @Result = 1	

END

GO
alter PROCEDURE [dbo].[GetUserMessage]
	@StartIndex INT = NULL,
    @MaxRetriveItem INT = NULL,
    @SortColumnName NVARCHAR(50) = NULL,
    @SortDirection INT = NULL,
	@SearchText NVARCHAR(50) = NULL,
	@UserId INT = NULL,
	@messageId INT = NULL
	AS
	BEGIN
		SET NOCOUNT ON;
		WITH CTE_Result AS
	(
		SELECT [Umess].[MessageId],[Umess].[MessageTypeId] ,[Umess].[Subject],[Umess].[FromUserId],[Umess].[ToUserId],[Umess].[Body],
		[Umess].[SentDate],[Umess].[ReadDate],[Umess].[Active],[Umess].[HasAttachedFile],[Umess].[IsFlag]
		,[UmessAtt].[AttachedId],[UmessAtt].[BaseFileName],[UmessAtt].[FileExtension],[UmessAtt].[FileName],[UmessAtt].[InsertDate],
		us.FirstName,us.LastName,
		(SELECT [FirstName] + ' ' + [LastName] from [dbo].[Users] WHERE [userId] =[Umess].[FromUserId]) as FromName
		,(SELECT [FirstName] + ' ' + [LastName] from [dbo].[Users] WHERE [userId] =[Umess].[ToUserId]) as ToName
		,ROW_NUMBER() OVER(
										ORDER BY 
												CASE WHEN @SortDirection = 0 THEN 
													CASE 
														WHEN @SortColumnName IS NULL THEN  [Umess].[MessageId]
													END
												END ASC, 
												CASE WHEN @SortDirection = 1 THEN 
													CASE 
															WHEN @SortColumnName IS NULL THEN [Umess].[MessageId]
													END
												END DESC
									) AS RowNumber
		  FROM [dbo].[UserMessage] Umess
		  Left JOIN [dbo].[UserMessageAttached] UmessAtt ON Umess.MessageId = UmessAtt.MessageId
		  INNER JOIN [dbo].[Users] us ON Umess.FromUserId = us.UserId 
		  WHERE (@SearchText IS NULL OR [Umess].[Subject] LIKE '%' + @SearchText +'%')
		  AND (@UserId IS NULL OR [Umess].ToUserId = @UserId)
		  AND (@messageId IS NULL OR [Umess].MessageId = @messageId)
	)

	SELECT *,  (SELECT MAX(RowNumber) FROM CTE_Result) ToltalItemCount
	FROM CTE_Result
	WHERE 
			RowNumber > @StartIndex
			AND  (@MaxRetriveItem IS NULL OR @MaxRetriveItem <= 0 OR (RowNumber <=  (@StartIndex + @MaxRetriveItem)))
	ORDER BY [MessageId] DESC 
	END
	GO


/****** Object:  View [dbo].[uv_SaleOrderReport]    Script Date: 05/24/2015 15:20:32 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[uv_SaleOrderReport]'))
DROP VIEW [dbo].[uv_SaleOrderReport]
GO
/****** Object:  View [dbo].[uv_SaleOrderReport]    Script Date: 05/24/2015 15:20:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[uv_SaleOrderReport]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[uv_SaleOrderReport]
AS

select Users.SiteId ,
dbo.Users.FirstName + '' '' + dbo.Users.LastName AS CustomerName, dbo.Users.UserId, 
(select  SUM( orderitem.UnitPrice * OrderItem.Quantity)   from OrderItem 
where OrderId = it.OrderId) as TotalPrice , 
 it.OrderId, it.OrderNumber, it.OrderDate , it.PaymentType, it.PaymentStatus
 from dbo.[Order] as it
INNER JOIN  dbo.Users ON it.CustomerId = dbo.Users.UserId 

'
GO
 
	

ALTER PROCEDURE [dbo].[GetSalesReport]
@FromDate nvarchar(256) = null,
@ToDate	 nvarchar(256) = null,
@SideId INT = null,
@OrderBy  nvarchar(256) = null 
AS 
Begin
DECLARE @SQLQuery AS NVARCHAR(500)
  
  
SET @SQLQuery = 'select 
SiteId,CustomerName,UserId,IsNull(TotalPrice,0) AS TotalPrice,
OrderId,OrderNumber,
OrderDate,PaymentType,PaymentStatus
 from uv_SaleOrderReport WHERE  OrderDate BETWEEN 
 
  CONVERT(datetime,''' +@FromDate +' 00:00'',103)
 AND 
  CONVERT(datetime,''' +@ToDate +' 23:59'',103) '
   
   
    IF(@SideId IS NOT NULL) 
        SET @SQLQuery = @SQLQuery + ' AND SiteId = ' + CAST(@SideId AS NVARCHAR)
        
    IF(@OrderBy IS NOT NULL) OR (LEN(@OrderBy) > 0) 
        SET @SQLQuery = @SQLQuery + ' Order By ' + @OrderBy
	  
/* Execute Transact-SQL String */
print @SQLQuery;
 EXECUTE(@SQLQuery)
		  
		  
END 
GO 
