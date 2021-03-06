USE DLG;
GO

ALTER PROCEDURE [dbo].[SearchGreenFee]
	@PageSize INT = NULL,
	@PageIndex INT = NULL,
	@RegionId INT = 0,
	@StateId INT = 0,
	@SiteId INT = 0,
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL,
	@TimeSlot INT = NULL,
	@IncludePractice BIT = NULL,
	@CategoryId INT = NULL,
	@NotIn NVARCHAR(MAX) = NULL,
	@LangId INT = 1
AS
BEGIN

	DECLARE @SQL NVARCHAR(MAX) = ''
	DECLARE @WITH NVARCHAR(MAX) = ''
	DECLARE @SELECT NVARCHAR(MAX) = ''
	DECLARE @FROM NVARCHAR(MAX) = ''
	DECLARE @WHERE NVARCHAR(MAX) = ''
	DECLARE @ORDER NVARCHAR(MAX) = ''
	DECLARE @FILTERS NVARCHAR(MAX) = ''
	
	SET NOCOUNT ON;

	SET @SELECT = ' [Item].*, [ItemLang].[ItemName], [ItemLang].[ItemShortDesc], img.[ItemImageId], img.[ListNo], img.[ImageName], img.[BaseName], img.[FileExtension], [SiteLang].[SiteName], ISNULL((SELECT AVG(Rating) FROM [ItemReview] WHERE [ItemReview].[ItemId] = [Item].[ItemId] AND [ItemReview].[IsApproved] = 1), 0) AS AverageRating, ISNULL(ipp.[Price], 0) AS PeriodPrice, ISNULL(idp.[Price], 0) AS SpecialPrice,
	(SELECT MIN([TeeSheet].[Discount]) AS [CheapestPrice] FROM [TeeSheet] WHERE [TeeSheet].[Price] > 0 AND [TeeSheet].[Discount] > 0 AND [TeeSheet].[ItemId] = [Item].[ItemId] AND [TeeSheet].[TeeSheetDate] >= GETDATE()) AS TeeSheetCheapestPrice, [Site].[AlbatrosCourseId]'

	SET @WITH = 'WITH GreenFees AS ('
	SET @WITH = @WITH + ' SELECT DISTINCT ' + @SELECT
	SET @WITH = @WITH + ' FROM [Item] 
	LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]
	LEFT JOIN [SiteLang] ON [Item].[SiteId] = [SiteLang].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) +  CHAR(13) + CHAR(10) +
	' LEFT JOIN [Site] ON [Site].[SiteId] = [Item].[SiteId] AND [Site].[Active] = 1 AND [Site].[Visible] = 1' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [State] ON [State].[StateId] = [Site].[StateId]' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [Region] ON [Region].[RegionId] = [State].[RegionId]' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [TeeSheet] ON [TeeSheet].[ItemId] = [Item].[ItemId]' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) +
	' LEFT JOIN [ItemImage] img ON img.[ItemImageId] = (SELECT TOP(1) [ItemImageId] FROM [ItemImage] WHERE [ItemImage].[ItemId] = [Item].[ItemId] ORDER BY [ItemImage].[ListNo], [ItemImage].[ItemImageId]) ' + CHAR(13) + CHAR(10) +
	' LEFT JOIN (SELECT [ItemId],[Price] FROM [ItemPrice] WHERE [PriceType] = 0 AND GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate])) ipp ON ipp.[ItemId] = [Item].[ItemId] ' + CHAR(13) + CHAR(10) +
	' LEFT JOIN (SELECT [ItemId],[Price] FROM [ItemPrice] WHERE [PriceType] = 1 AND GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate])) idp ON idp.[ItemId] = [Item].[ItemId] ' + CHAR(13) + CHAR(10) +
	' WHERE [Item].[Active] = 1 AND [Item].[IsPublish] = 1 AND [Item].[ItemTypeId] = 2 AND ISNULL([Site].[ReservationAPI], 0) > 0
		AND (PublishStartDate IS NULL OR PublishEndDate IS NULL OR (GETDATE() BETWEEN PublishStartDate AND PublishEndDate))' + CHAR(13) + CHAR(10)
	
		IF(@NotIn IS NOT NULL AND @NotIn <> '')
		BEGIN
			SET @WITH = @WITH + ' AND [Item].[ItemId] NOT IN(' + @NotIn + ')'
		END
		
		IF(@FromDate IS NOT NULL AND @ToDate IS NOT NULL)
			SET @WITH = @WITH + ' AND ([TeeSheet].[TeeSheetDate] BETWEEN ''' + CONVERT(VARCHAR, @FromDate, 21) +''' AND ''' + CONVERT(VARCHAR, @ToDate, 21) + ''')' + CHAR(13) + CHAR(10)

		IF(ISNULL(@SiteId, 0) > 0)
			SET @WITH = @WITH + ' AND Item.SiteId = ' + CAST(@SiteId AS NVARCHAR) + CHAR(13) + CHAR(10)
		ELSE IF(ISNULL(@StateId, 0) > 0)
			SET @WITH = @WITH + ' AND [Site].StateId = ' + CAST(@StateId AS NVARCHAR) + CHAR(13) + CHAR(10)
		ELSE IF(ISNULL(@RegionId, 0) > 0)
			SET @WITH = @WITH + ' AND [State].RegionId = ' + CAST(@RegionId AS NVARCHAR) + CHAR(13) + CHAR(10)
		
		IF(ISNULL(@CategoryId, 0) > 0)
			SET @WITH = @WITH + ' AND [Item].[CategoryId] = ' + CAST(@CategoryId AS NVARCHAR) + CHAR(13) + CHAR(10)
		IF(ISNULL(@TimeSlot, 0) > 0)
			SET @WITH = @WITH + ' AND [TeeSheet].[FromTime] = ''' + CAST(@TimeSlot AS NVARCHAR) + ':00'''
			
		IF(@IncludePractice IS NOT NULL)
			SET @WITH = @WITH + ' AND ISNULL([Item].[IncludePractice], 0) = ' + CAST(@IncludePractice AS NVARCHAR)
	 	
	 	IF(LEN(@WHERE) > 0)
			SET @WITH = @WITH + ' AND ([Item].[ItemCode] LIKE N''%' + @WHERE + '%'' OR [ItemLang].[ItemName] LIKE N''%' + @WHERE + '%'')'
		
	
	-- UNION Albatros TeeSheet...
	SET @WITH = @WITH + 'UNION'
	
	SET @WITH = @WITH + ' SELECT DISTINCT ' + @SELECT
	SET @WITH = @WITH + ' FROM [Item] 
	LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]
	LEFT JOIN [SiteLang] ON [Item].[SiteId] = [SiteLang].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) +  CHAR(13) + CHAR(10) +
	' LEFT JOIN [Site] ON [Site].[SiteId] = [Item].[SiteId] AND [Site].[Active] = 1 AND [Site].[Visible] = 1' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [State] ON [State].[StateId] = [Site].[StateId]' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [Region] ON [Region].[RegionId] = [State].[RegionId]' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) +
	' LEFT JOIN [ItemImage] img ON img.[ItemImageId] = (SELECT TOP(1) [ItemImageId] FROM [ItemImage] WHERE [ItemImage].[ItemId] = [Item].[ItemId] ORDER BY [ItemImage].[ListNo], [ItemImage].[ItemImageId]) ' + CHAR(13) + CHAR(10) +
	' LEFT JOIN (SELECT [ItemId],[Price] FROM [ItemPrice] WHERE [PriceType] = 0 AND GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate])) ipp ON ipp.[ItemId] = [Item].[ItemId] ' + CHAR(13) + CHAR(10) +
	' LEFT JOIN (SELECT [ItemId],[Price] FROM [ItemPrice] WHERE [PriceType] = 1 AND GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate])) idp ON idp.[ItemId] = [Item].[ItemId] ' + CHAR(13) + CHAR(10) +
	' WHERE [Item].[Active] = 1 AND [Item].[IsPublish] = 1 AND [Item].[ItemTypeId] = 2 AND ISNULL([Site].[ReservationAPI], 0) = 0
		AND (PublishStartDate IS NULL OR PublishEndDate IS NULL OR (GETDATE() BETWEEN PublishStartDate AND PublishEndDate))' + CHAR(13) + CHAR(10)
	
		IF(@NotIn IS NOT NULL AND @NotIn <> '')
		BEGIN
			SET @WITH = @WITH + ' AND [Item].[ItemId] NOT IN(' + @NotIn + ')'
		END
		
		IF(ISNULL(@SiteId, 0) > 0)
			SET @WITH = @WITH + ' AND Item.SiteId = ' + CAST(@SiteId AS NVARCHAR) + CHAR(13) + CHAR(10)
		ELSE IF(ISNULL(@StateId, 0) > 0)
			SET @WITH = @WITH + ' AND [Site].StateId = ' + CAST(@StateId AS NVARCHAR) + CHAR(13) + CHAR(10)
		ELSE IF(ISNULL(@RegionId, 0) > 0)
			SET @WITH = @WITH + ' AND [State].RegionId = ' + CAST(@RegionId AS NVARCHAR) + CHAR(13) + CHAR(10)
		
		IF(ISNULL(@CategoryId, 0) > 0)
			SET @WITH = @WITH + ' AND [Item].[CategoryId] = ' + CAST(@CategoryId AS NVARCHAR) + CHAR(13) + CHAR(10)
		--IF(ISNULL(@TimeSlot, 0) > 0)
		--	SET @WITH = @WITH + ' AND [TeeSheet].[FromTime] = ''' + CAST(@TimeSlot AS NVARCHAR) + ':00'''
			
		IF(@IncludePractice IS NOT NULL)
			SET @WITH = @WITH + ' AND ISNULL([Item].[IncludePractice], 0) = ' + CAST(@IncludePractice AS NVARCHAR)
	 	
	 	IF(LEN(@WHERE) > 0)
			SET @WITH = @WITH + ' AND ([Item].[ItemCode] LIKE N''%' + @WHERE + '%'' OR [ItemLang].[ItemName] LIKE N''%' + @WHERE + '%'')'
	
	SET @WITH = @WITH + ')' + CHAR(13) + CHAR(10) -- End of WITH Command
	
	SET @SQL = @WITH + 'SELECT '
	
	IF(ISNULL(@PageSize, 0) > 0)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@PageSize AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM ('
	
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY [ItemId] DESC) AS RowNumber, it.* FROM GreenFees it'
	
	SET @SQL = @SQL + ') AS it '
	
	SET @WHERE = ' WHERE 1 = 1'
	
	IF((@PageSize IS NOT NULL AND @PageSize > 0) AND (@PageIndex IS NOT NULL AND @PageIndex > 0))
	BEGIN
		SET @WHERE = @WHERE + ' AND RowNumber >= ' + CAST((@PageSize * (@PageIndex - 1)) AS NVARCHAR)
	END	
	
	IF((@PageSize IS NOT NULL AND @PageSize > 0) AND (@PageIndex IS NOT NULL AND @PageIndex > 0))
		SET @ORDER = ' ORDER BY RowNumber;' + CHAR(13) + CHAR(13)
	ELSE
		SET @ORDER = ' ORDER BY NEWID();' + CHAR(13) + CHAR(13)
		
	SET @SQL = @SQL + @ORDER
	
	EXEC SP_EXECUTESQL @SQL
	--PRINT @SQL
	
	IF(@PageSize IS NOT NULL AND @PageSize > 0)
		SET @SQL = @WITH + 'SELECT (COUNT(*) / ' + CAST(@PageSize AS NVARCHAR) + ') + 1 AS ''TotalPage'' FROM GreenFees '
	ELSE
		SET @SQL = 'SELECT 1 AS TotalPage'

	EXEC SP_EXECUTESQL @SQL
	--PRINT @SQL
	
	RETURN
END
