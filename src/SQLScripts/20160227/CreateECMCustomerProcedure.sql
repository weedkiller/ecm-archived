USE DLG
GO

/****** Object:  StoredProcedure [dbo].[SelectCustomersList]    Script Date: 28/2/2559 16:36:27 ******/
DROP PROCEDURE [dbo].[SelectECMSubscribersList]
GO

/****** Object:  StoredProcedure [dbo].[SelectCustomersList]    Script Date: 28/2/2559 16:36:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SelectECMSubscribersList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;
	
	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY Email) AS RowNumber, * FROM [Users] '
	SET @SQL = @SQL + ' WHERE UserTypeId = 2 AND Active = 1 AND IsSubscriber = 1 AND IsReceiveEmailInfo = 1 '	

	IF(@SiteId IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND [SiteId] = ' + CAST(@SiteId AS NVARCHAR)
	END
		
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND (Email LIKE ''%' + @Search + '%'' OR Firstname LIKE ''%' + @Search + '%'' OR Lastname LIKE ''%' + @Search + '%'' OR Middlename LIKE ''%' + @Search + '%'')'
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM Users WHERE UserTypeId = 2 AND Active = 1 AND IsSubscriber = 1 AND IsReceiveEmailInfo = 1'
	
	exec sp_executesql @SQL
	
END


GO