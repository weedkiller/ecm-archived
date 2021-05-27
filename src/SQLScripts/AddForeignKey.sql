IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EmailQue' AND COLUMN_NAME = 'IsResent')
BEGIN
	EXEC sp_rename 'EmailQue.IsResent', 'Resent', 'COLUMN'
END

SET IDENTITY_INSERT [GolfBrand] ON 
INSERT INTO [GolfBrand](GolfBrandId, GolfBrandName) VALUES(0, N'[None]')
SET IDENTITY_INSERT [GolfBrand] OFF

SET IDENTITY_INSERT [Site] ON 
INSERT INTO [Site](SiteId, GolfBrandId, StateId, RegionId, CountryId, UserId, ReservationAPI, Active, Visible) VALUES(0, 0, 0, 0, 0, 0, 0, 0, 0)
SET IDENTITY_INSERT [Site] OFF

INSERT INTO [SiteLang](SiteId, LangId, SiteName) VALUES(0, 1, N'[None]')

INSERT INTO CourseType(CourseTypeId, CourseTypeName) VALUES(0, N'[None]')

SET IDENTITY_INSERT [Course] ON 
INSERT INTO [Course](CourseId, CourseTypeId, [SiteId]) VALUES(0, 0, 0)
SET IDENTITY_INSERT [Course] OFF

SET IDENTITY_INSERT [Title] ON 
INSERT INTO [Title](TitleId, TitleName) VALUES(0, N'[None]')
SET IDENTITY_INSERT [Title] OFF

SET IDENTITY_INSERT [CustomerType] ON 
INSERT INTO [CustomerType](CustomerTypeId, ParentId, SiteId) VALUES(0, 0, 0)
SET IDENTITY_INSERT [CustomerType] OFF

SET IDENTITY_INSERT [Users] ON 
INSERT INTO [Users](UserId, UserTypeId, CustomerTypeId, SubCustomerTypeId, TitleId, CountryId, ShippingCountryId, CityId, ShippingCityId, ModifyUserId, SiteId)
VALUES(0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0)
SET IDENTITY_INSERT [Users] OFF

SET IDENTITY_INSERT [Country] ON 
INSERT INTO [Country](CountryId, CountryName) VALUES(0, N'[None]')
SET IDENTITY_INSERT [Country] OFF

INSERT INTO [Region](RegionId, CountryId, RegionName) VALUES(0, 0, N'[None]')

SET IDENTITY_INSERT [State] ON 
INSERT INTO [State](StateId, RegionId, StateName) VALUES(0, 0, N'[None]')
SET IDENTITY_INSERT [State] OFF

INSERT INTO [City](CityId, RegionId, CityName, PostalCode) VALUES(0, 0, N'[None]', N'[None]')

SET IDENTITY_INSERT [Address] ON 
INSERT INTO [Address](AddressId, CountryId, CityId, UserId, TitleId) VALUES(0, 1, 1, 0, 0)
SET IDENTITY_INSERT [Address] OFF

SET IDENTITY_INSERT [Supplier] ON 
INSERT INTO [Supplier](SupplierId, UserId, Active, SupplierName) VALUES(0, 0, 0, N'[None]')
SET IDENTITY_INSERT [Supplier] OFF

SET IDENTITY_INSERT [Tax] ON 
INSERT INTO [Tax](TaxID, TaxCode) VALUES(0, N'[None]')
SET IDENTITY_INSERT [Tax] OFF

SET IDENTITY_INSERT [Brand] ON 
INSERT INTO [Brand](BrandId, [BrandName]) VALUES(0, N'[None]')
SET IDENTITY_INSERT [Brand] OFF

INSERT INTO [ItemTypes](ItemTypeId, ItemTypeName) VALUES(0, N'[None]')

SET IDENTITY_INSERT [ItemCategory] ON 
INSERT INTO [ItemCategory](CategoryId, ItemTypeId, UserId, SiteId, CategoryName) VALUES(0, 0, 0, 0, N'[None]')
SET IDENTITY_INSERT [ItemCategory] OFF

SET IDENTITY_INSERT [Item] ON 
INSERT INTO [Item](ItemId, SiteId, UserId, ItemTypeId, CategoryId, TaxId, CourseId, SupplierId, BrandId) VALUES(0, 0, 0, 0, 0, 0, 0, 0, 0)
SET IDENTITY_INSERT [Item] OFF

SET IDENTITY_INSERT [CouponGroup] ON 
INSERT INTO [CouponGroup](CouponGroupId, CouponGroupName, TimesToUse, MinimumAmount, Reduction, CouponType, CouponUsageType, UsagePeriodType)
VALUES(0, N'[None]', 0, 0, 0, 0, 0, 0)
SET IDENTITY_INSERT [CouponGroup] OFF

SET IDENTITY_INSERT [Coupon] ON 
INSERT INTO [Coupon](CouponId, CouponGroupId, CouponCode) VALUES(0, 0, N'[None]')
SET IDENTITY_INSERT [Coupon] OFF

SET IDENTITY_INSERT EmailTemplate ON
INSERT INTO EmailTemplate(TemplateId, CategoryId, UserId, Active) VALUES(0, 0, 0, 1)
SET IDENTITY_INSERT EmailTemplate OFF

SET IDENTITY_INSERT Emailing ON
INSERT INTO Emailing(EmailId, SiteId, TemplateId, UserId, EmailName, Active) VALUES(0, 0, 0, 0, N'[None]', 1)
SET IDENTITY_INSERT Emailing OFF

SET IDENTITY_INSERT EmailQue ON
INSERT INTO EmailQue(EmailQueId, EmailId, CustomerId) VALUES(0, 0, 0)
SET IDENTITY_INSERT EmailQue OFF


---------------------------------------------------------------
ALTER TABLE [Users] DROP CONSTRAINT [PK_Users]
ALTER TABLE [Users] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [Users] ADD CONSTRAINT [PK_Users] PRIMARY KEY(UserId)

ALTER TABLE [Site] DROP CONSTRAINT [PK_Site]
ALTER TABLE [Site] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [Site] ADD CONSTRAINT [PK_Site] PRIMARY KEY(SiteId)

ALTER TABLE [City] DROP CONSTRAINT [PK__City__F2D21B7634C5E1A2]
ALTER TABLE [City] ADD CONSTRAINT [PK_City] PRIMARY KEY(CityId)

--------------- Address ------------------------
UPDATE it SET [CountryId] = 1
FROM [Address] it
LEFT JOIN Country ON Country.CountryId = it.CountryId
WHERE Country.CountryCode IS NULL

ALTER TABLE [Address] ALTER COLUMN [CountryId] INT NOT NULL

UPDATE it SET [CityId] = 1
FROM [Address] it
LEFT JOIN City ON City.CityId = it.CityId
WHERE City.CityName IS NULL

ALTER TABLE [Address] ALTER COLUMN CityId INT NOT NULL

UPDATE it SET [UserId] = 1
FROM [Address] it
LEFT JOIN [Users] ON [Users].[UserId] = it.UserId
WHERE [Users].[Email] IS NULL

ALTER TABLE [Address] ALTER COLUMN UserId BIGINT NOT NULL

UPDATE it SET [TitleId] = 1
FROM [Address] it
LEFT JOIN [Title] ON [Title].[TitleId] = it.TitleId
WHERE [Title].[TitleName] IS NULL

ALTER TABLE [Address] ALTER COLUMN [TitleId] INT NOT NULL

ALTER TABLE [Address] DROP CONSTRAINT [PK_Address]
ALTER TABLE [Address] ALTER COLUMN [AddressId] BIGINT NOT NULL
ALTER TABLE [Address] ADD CONSTRAINT [PK_Address] PRIMARY KEY(AddressId)

ALTER TABLE [Address] ADD CONSTRAINT [FK_Address_Country_CountryId] FOREIGN KEY(CountryId) REFERENCES Country(CountryId)
ALTER TABLE [Address] ADD CONSTRAINT [FK_Address_City_CityId] FOREIGN KEY(CityId) REFERENCES City(CityId)
ALTER TABLE [Address] ADD CONSTRAINT [FK_Address_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)
ALTER TABLE [Address] ADD CONSTRAINT [FK_Address_Title_TitleId] FOREIGN KEY(TitleId) REFERENCES Title(TitleId)

--------------- Ads ------------------------
UPDATE it SET [AdsetId] = 1
FROM [Ads] it
LEFT JOIN [Adset] ON [Adset].[AdsetId] = it.[AdsetId]
WHERE [Adset].[AdsetName] IS NULL

ALTER TABLE [Ads] ALTER COLUMN [AdsetId] INT NOT NULL

ALTER TABLE [Ads] ADD CONSTRAINT [FK_Ads_Adset_AdsetId] FOREIGN KEY(AdsetId) REFERENCES Adset(AdsetId)

--------------- BrandLang ------------------------
UPDATE it SET [BrandId] = 1
FROM [BrandLang] it
LEFT JOIN [Brand] ON [Brand].[BrandId] = it.[BrandId]
WHERE [Brand].[BrandName] IS NULL

ALTER TABLE [BrandLang] ALTER COLUMN [BrandId] INT NOT NULL

UPDATE it SET [LangId] = 1
FROM [BrandLang] it
LEFT JOIN [Language] ON [Language].[LangId] = it.[LangId]
WHERE [Language].[LangCode] IS NULL

ALTER TABLE [BrandLang] ALTER COLUMN [LangId] INT NOT NULL

ALTER TABLE [BrandLang] ADD CONSTRAINT [FK_BrandLang_Brand_BrandId] FOREIGN KEY(BrandId) REFERENCES Brand(BrandId)
ALTER TABLE [BrandLang] ADD CONSTRAINT [FK_BrandLang_Language_LangId] FOREIGN KEY(LangId) REFERENCES Language(LangId)

--------------- Category ------------------------
UPDATE it SET [SiteId] = 1
FROM [Category] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [Category] ALTER COLUMN [SiteId] BIGINT NOT NULL

ALTER TABLE [Category] ADD CONSTRAINT [FK_Category_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- City ------------------------
UPDATE it SET [RegionId] = 1
FROM [City] it
LEFT JOIN [Region] ON [Region].[RegionId] = it.[RegionId]
WHERE [Region].[RegionId] IS NULL

ALTER TABLE [City] ALTER COLUMN [RegionId] INT NOT NULL

ALTER TABLE [City] ADD CONSTRAINT [FK_City_Region_RegionId] FOREIGN KEY(RegionId) REFERENCES Region(RegionId)

--------------- Content ------------------------
UPDATE it SET [CategoryId] = 1
FROM [Content] it
LEFT JOIN [Category] ON [Category].[CategoryId] = it.[CategoryId]
WHERE [Category].[CategoryId] IS NULL

ALTER TABLE [Content] ALTER COLUMN [CategoryId] INT NOT NULL

ALTER TABLE [Content] ADD CONSTRAINT [FK_Content_Category_CategoryId] FOREIGN KEY(CategoryId) REFERENCES Category(CategoryId)

--------------- CouponGroup ------------------------
UPDATE it SET [TimesToUse] = 0
FROM [CouponGroup] it
WHERE it.[TimesToUse] IS NULL

UPDATE it SET [CouponType] = 0
FROM [CouponGroup] it
WHERE it.[CouponType] IS NULL

UPDATE it SET CouponUsageType = 0
FROM [CouponGroup] it
WHERE it.CouponUsageType IS NULL

UPDATE it SET [Reduction] = 0
FROM [CouponGroup] it
WHERE it.[Reduction] IS NULL

UPDATE it SET [UsagePeriodType] = 0
FROM [CouponGroup] it
WHERE it.[UsagePeriodType] IS NULL

UPDATE it SET [MinimumAmount] = 0
FROM [CouponGroup] it
WHERE it.[MinimumAmount] IS NULL

ALTER TABLE [CouponGroup] ALTER COLUMN [TimesToUse] INT NOT NULL
ALTER TABLE [CouponGroup] ALTER COLUMN [CouponType] INT NOT NULL
ALTER TABLE [CouponGroup] ALTER COLUMN CouponUsageType INT NOT NULL
ALTER TABLE [CouponGroup] ALTER COLUMN [Reduction] DECIMAL(15,2) NOT NULL
ALTER TABLE [CouponGroup] ALTER COLUMN [UsagePeriodType] INT NOT NULL
ALTER TABLE [CouponGroup] ALTER COLUMN [MinimumAmount] MONEY NOT NULL

--------------- Coupon ------------------------
UPDATE it SET [CouponGroupId] = 0
FROM [Coupon] it
LEFT JOIN [CouponGroup] ON [CouponGroup].[CouponGroupId] = it.[CouponGroupId]
WHERE [CouponGroup].[CouponGroupId] IS NULL

ALTER TABLE [Coupon] DROP CONSTRAINT [PK_Coupon]
ALTER TABLE [Coupon] ALTER COLUMN [CouponId] BIGINT NOT NULL
ALTER TABLE [Coupon] ADD CONSTRAINT [PK_Coupon] PRIMARY KEY(CouponId)

ALTER TABLE [Coupon] ALTER COLUMN [CouponGroupId] BIGINT NOT NULL

ALTER TABLE [Coupon] ADD CONSTRAINT [FK_Coupon_CouponGroup_CouponGroupId] FOREIGN KEY(CouponGroupId) REFERENCES CouponGroup(CouponGroupId)

--------------- CouponGroupItem ------------------------
UPDATE it SET [CouponGroupId] = 0
FROM [CouponGroupItem] it
LEFT JOIN [CouponGroup] ON [CouponGroup].[CouponGroupId] = it.[CouponGroupId]
WHERE [CouponGroup].[CouponGroupId] IS NULL

ALTER TABLE [Item] DROP CONSTRAINT [PK_Item]
ALTER TABLE [Item] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [Item] ADD CONSTRAINT [PK_Item] PRIMARY KEY(ItemId)
ALTER TABLE [CouponGroupItem] ALTER COLUMN [CouponGroupId] BIGINT NOT NULL

ALTER TABLE [CouponGroupItem] ADD CONSTRAINT [FK_CouponGroupItem_CouponGroup_CouponGroupId] FOREIGN KEY(CouponGroupId) REFERENCES CouponGroup(CouponGroupId)
ALTER TABLE [CouponGroupItem] ADD CONSTRAINT [FK_CouponGroupItem_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)

--------------- CouponGroupItemCategory ------------------------
ALTER TABLE [CouponGroupItemCategory] DROP CONSTRAINT [PK_CouponGroupCategory]
ALTER TABLE [CouponGroupItemCategory] ALTER COLUMN [CouponGroupId] BIGINT NOT NULL
ALTER TABLE [CouponGroupItemCategory] ADD CONSTRAINT [PK_CouponGroupCategory] PRIMARY KEY(CouponGroupId, CategoryId)
ALTER TABLE [CouponGroupItemCategory] ALTER COLUMN [CouponGroupId] BIGINT NOT NULL

ALTER TABLE [CouponGroupItemCategory] ADD CONSTRAINT [FK_CouponGroupItemCategory_CouponGroup_CouponGroupId] FOREIGN KEY(CouponGroupId) REFERENCES CouponGroup(CouponGroupId)
ALTER TABLE [CouponGroupItemCategory] ADD CONSTRAINT [FK_CouponGroupItemCategory_Item_CategoryId] FOREIGN KEY(CategoryId) REFERENCES ItemCategory(CategoryId)

--------------- CouponGroupItemType ------------------------
ALTER TABLE [CouponGroupItemType] DROP CONSTRAINT [PK_CouponGroupItemType]
ALTER TABLE [CouponGroupItemType] ALTER COLUMN [CouponGroupId] BIGINT NOT NULL
ALTER TABLE [CouponGroupItemType] ALTER COLUMN [ItemTypeId] INT NOT NULL
ALTER TABLE [CouponGroupItemType] ADD CONSTRAINT [PK_CouponGroupItemType] PRIMARY KEY(CouponGroupId, ItemTypeId)
ALTER TABLE [CouponGroupItemType] ALTER COLUMN [CouponGroupId] BIGINT NOT NULL

ALTER TABLE [CouponGroupItemType] ADD CONSTRAINT [FK_CouponGroupItemType_CouponGroup_CouponGroupId] FOREIGN KEY(CouponGroupId) REFERENCES CouponGroup(CouponGroupId)
ALTER TABLE [CouponGroupItemType] ADD CONSTRAINT [FK_CouponGroupItemType_ItemTypes_ItemTypeId] FOREIGN KEY(ItemTypeId) REFERENCES ItemTypes(ItemTypeId)

--------------- Course ------------------------
ALTER TABLE [Course] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [Course] ALTER COLUMN [CourseTypeId] INT NOT NULL
ALTER TABLE [Course] ADD CONSTRAINT [FK_Course_CourseType_CourseTypeId] FOREIGN KEY(CourseTypeId) REFERENCES CourseType(CourseTypeId)
ALTER TABLE [Course] ADD CONSTRAINT [FK_Course_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- CustomerAddress ------------------------
ALTER TABLE [CustomerAddress] ALTER COLUMN [PersonalCountryId] INT NOT NULL
ALTER TABLE [CustomerAddress] ALTER COLUMN [BusinessCountryId] INT NOT NULL
ALTER TABLE [CustomerAddress] ADD CONSTRAINT [FK_CustomerAddress_Country_PersonalCountryId] FOREIGN KEY(PersonalCountryId) REFERENCES Country(CountryId)
ALTER TABLE [CustomerAddress] ADD CONSTRAINT [FK_CustomerAddress_Country_BusinessCountryId] FOREIGN KEY(BusinessCountryId) REFERENCES Country(CountryId)

--------------- CustomerCreditCard ------------------------
UPDATE it SET [UserId] = 0
FROM [CustomerCreditCard] it
LEFT JOIN [Users] ON [Users].[UserId] = it.[UserId]
WHERE [Users].[UserId] IS NULL

ALTER TABLE [CustomerCreditCard] ALTER COLUMN [UserId] BIGINT NOT NULL

ALTER TABLE [CustomerCreditCard] ADD CONSTRAINT [FK_CustomerCreditCard_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)


--------------- CustomerGroup ------------------------
UPDATE it SET [SiteId] = 0
FROM [CustomerGroup] it
LEFT JOIN [Site] ON [Site].[UserId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [CustomerGroup] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [CustomerGroup] ADD CONSTRAINT [FK_CustomerGroup_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- CustomerGroupCustomer ------------------------
TRUNCATE TABLE [CustomerGroupCustomer]

ALTER TABLE [CustomerGroupCustomer] DROP CONSTRAINT [PK_CustomerGroupCustomer]
ALTER TABLE [CustomerGroupCustomer] ALTER COLUMN [CustomerGroupId] INT NOT NULL
ALTER TABLE [CustomerGroupCustomer] ALTER COLUMN [CustomerId] BIGINT NOT NULL
ALTER TABLE [CustomerGroupCustomer] ADD CONSTRAINT [PK_CustomerGroupCustomer] PRIMARY KEY(CustomerGroupId, CustomerId)
ALTER TABLE [CustomerGroupCustomer] ADD CONSTRAINT [FK_CustomerGroupCustomer_Users_CustomerId] FOREIGN KEY(CustomerId) REFERENCES Users(UserId)
ALTER TABLE [CustomerGroupCustomer] ADD CONSTRAINT [FK_CustomerGroupCustomer_CustomerGroup_CustomerGroupId] FOREIGN KEY(CustomerGroupId) REFERENCES CustomerGroup(CustomerGroupId)

--------------- CustomerType ------------------------
ALTER TABLE [CustomerType] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [CustomerType] ADD CONSTRAINT [FK_CustomerType_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- CustomerTypeLang ------------------------
ALTER TABLE [CustomerTypeLang] DROP CONSTRAINT [PK_CustomerTypeLang]
ALTER TABLE [CustomerTypeLang] ALTER COLUMN [CustomerTypeId] BIGINT NOT NULL
ALTER TABLE [CustomerTypeLang] ALTER COLUMN [LangId] INT NOT NULL
ALTER TABLE [CustomerTypeLang] ADD CONSTRAINT [PK_CustomerTypeLang] PRIMARY KEY(CustomerTypeId, LangId)
ALTER TABLE [CustomerTypeLang] ADD CONSTRAINT [FK_CustomerTypeLang_CustomerType_CustomerTypeId] FOREIGN KEY(CustomerTypeId) REFERENCES CustomerType(CustomerTypeId)
ALTER TABLE [CustomerTypeLang] ADD CONSTRAINT [FK_CustomerTypeLang_Language_LangId] FOREIGN KEY(LangId) REFERENCES Language(LangId)

--------------- DLGCard ------------------------
TRUNCATE TABLE [DLGCard]
ALTER TABLE [DLGCard] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [DLGCard] ADD CONSTRAINT [FK_DLGCard_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)

--------------- DLGCardBalance ------------------------
TRUNCATE TABLE [DLGCardBalance]
ALTER TABLE [DLGCardBalance] ALTER COLUMN [DLGCardId] INT NOT NULL
ALTER TABLE [DLGCardBalance] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [DLGCardBalance] ADD CONSTRAINT [FK_DLGCardBalance_DLGCard_DLGCardId] FOREIGN KEY(DLGCardId) REFERENCES DLGCard(DLGCardId)
ALTER TABLE [DLGCardBalance] ADD CONSTRAINT [FK_DLGCardBalance_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)

--------------- DLGCardStyle ------------------------
TRUNCATE TABLE [DLGCardStyle]
ALTER TABLE [DLGCardStyle] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [DLGCardStyle] ADD CONSTRAINT [FK_DLGCardStyle_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)

--------------- EmailAttachment ------------------------
ALTER TABLE [EmailAttachment] ALTER COLUMN [TemplateId] INT NOT NULL
ALTER TABLE [EmailAttachment] ADD CONSTRAINT [FK_EmailAttachment_EmailTemplate_TemplateId] FOREIGN KEY(TemplateId) REFERENCES EmailTemplate(TemplateId)

--------------- Emailing ------------------------
ALTER TABLE [Emailing] DROP CONSTRAINT [PK_Email]
ALTER TABLE [Emailing] ALTER COLUMN EmailId BIGINT NOT NULL
ALTER TABLE [Emailing] ADD CONSTRAINT [PK_Email] PRIMARY KEY(EmailId)
DELETE FROM [Emailing] WHERE SiteId IS NULL
ALTER TABLE [Emailing] ALTER COLUMN [TemplateId] INT NOT NULL
ALTER TABLE [Emailing] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [Emailing] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [Emailing] ADD CONSTRAINT [FK_Emailing_EmailTemplate_TemplateId] FOREIGN KEY(TemplateId) REFERENCES EmailTemplate(TemplateId)
ALTER TABLE [Emailing] ADD CONSTRAINT [FK_Emailing_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)
ALTER TABLE [Emailing] ADD CONSTRAINT [FK_Emailing_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)

--------------- EmailingList ------------------------
ALTER TABLE [EmailingList] ALTER COLUMN [EmailId] BIGINT NOT NULL
ALTER TABLE [EmailingList] ADD CONSTRAINT [FK_EmailingList_Emailing_EmailId] FOREIGN KEY(EmailId) REFERENCES Emailing(EmailId)--------------- Emailing ------------------------

--------------- EmailQue ------------------------
TRUNCATE TABLE EmailQue

ALTER TABLE [EmailQue] DROP CONSTRAINT [PK_EmailQue]
ALTER TABLE [EmailQue] ALTER COLUMN EmailQueId BIGINT NOT NULL
ALTER TABLE [EmailQue] ADD CONSTRAINT [PK_EmailQue] PRIMARY KEY(EmailQueId)
ALTER TABLE [EmailQue] ALTER COLUMN [EmailId] BIGINT NOT NULL
ALTER TABLE [EmailQue] ALTER COLUMN [CustomerId] BIGINT NOT NULL
ALTER TABLE [EmailQue] ADD CONSTRAINT [FK_EmailQue_Emailing_EmailId] FOREIGN KEY(EmailId) REFERENCES Emailing(EmailId)
ALTER TABLE [EmailQue] ADD CONSTRAINT [FK_EmailQue_Users_CustomerId] FOREIGN KEY(CustomerId) REFERENCES Users(UserId)

--------------- EmailLog ------------------------
ALTER TABLE [EmailLog] ALTER COLUMN [EmailQueId] BIGINT NOT NULL
ALTER TABLE [EmailLog] ADD CONSTRAINT [FK_EmailLog_EmailQue_EmailQueId] FOREIGN KEY(EmailQueId) REFERENCES EmailQue(EmailQueId)

--------------- EmailTemplate ------------------------
UPDATE [EmailTemplate] SET CategoryId = 1 WHERE CategoryId = 0
ALTER TABLE [EmailTemplate] ALTER COLUMN [CategoryId] INT NOT NULL
ALTER TABLE [EmailTemplate] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [EmailTemplate] ADD CONSTRAINT [FK_EmailTemplate_Category_CategoryId] FOREIGN KEY(CategoryId) REFERENCES Category(CategoryId)
ALTER TABLE [EmailTemplate] ADD CONSTRAINT [FK_EmailTemplate_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)

--------------- EmailTemplateLang ------------------------
DELETE it
FROM [EmailTemplateLang] it
LEFT JOIN [EmailTemplate] ON [EmailTemplate].[TemplateId] = it.[TemplateId]
WHERE [EmailTemplate].[TemplateId] IS NULL

ALTER TABLE [EmailTemplateLang] ALTER COLUMN [TemplateId] INT NOT NULL
ALTER TABLE [EmailTemplateLang] ADD CONSTRAINT [FK_EmailTemplateLang_EmailTemplate_TemplateId] FOREIGN KEY(TemplateId) REFERENCES EmailTemplate(TemplateId)

--------------- EmailTracking ------------------------
TRUNCATE TABLE [EmailTracking]
ALTER TABLE [EmailTracking] ALTER COLUMN [CampaignId] BIGINT NOT NULL
ALTER TABLE [EmailTracking] ALTER COLUMN [EmailQueId] BIGINT NOT NULL
ALTER TABLE [EmailTracking] ADD CONSTRAINT [FK_EmailTemplateLang_Emailing_CampaignId] FOREIGN KEY(CampaignId) REFERENCES Emailing(EmailId)
ALTER TABLE [EmailTracking] ADD CONSTRAINT [FK_EmailTemplateLang_EmailQue_EmailQueId] FOREIGN KEY(EmailQueId) REFERENCES EmailQue(EmailQueId)

--------------- Impressum ------------------------
TRUNCATE TABLE [Impressum]
ALTER TABLE [Impressum] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [Impressum] ADD CONSTRAINT [FK_Impressum_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- Interest ------------------------
TRUNCATE TABLE [Interest]
ALTER TABLE [Interest] ALTER COLUMN [CategoryId] INT NOT NULL
ALTER TABLE [Interest] ADD CONSTRAINT [FK_Impressum_Category_CategoryId] FOREIGN KEY(CategoryId) REFERENCES Category(CategoryId)

--------------- InterestLang ------------------------
TRUNCATE TABLE [InterestLang]
ALTER TABLE [InterestLang] DROP CONSTRAINT [PK_InterestLang]
ALTER TABLE [InterestLang] ALTER COLUMN [InterestId] INT NOT NULL
ALTER TABLE [InterestLang] ALTER COLUMN [LangId] INT NOT NULL
ALTER TABLE [InterestLang] ADD CONSTRAINT [PK_InterestLang] PRIMARY KEY(InterestId, LangId)
ALTER TABLE [InterestLang] ADD CONSTRAINT [FK_InterestLang_Interest_InterestId] FOREIGN KEY(InterestId) REFERENCES Interest(InterestId)
ALTER TABLE [InterestLang] ADD CONSTRAINT [FK_InterestLang_Language_LangId] FOREIGN KEY(LangId) REFERENCES Language(LangId)

--------------- Item ------------------------
UPDATE [Item] SET [CategoryId] = 0 WHERE [CategoryId] IS NULL
UPDATE [Item] SET [SiteId] = 0 WHERE [SiteId] IS NULL
UPDATE [Item] SET [SupplierId] = 0 WHERE [SupplierId] IS NULL
UPDATE [Item] SET [BrandId] = 0 WHERE [BrandId] IS NULL
UPDATE [Item] SET [CourseId] = 0 WHERE [CourseId] IS NULL
UPDATE [Item] SET [UserId] = 0 WHERE [UserId] IS NULL

UPDATE it SET [CategoryId] = 0
FROM [Item] it
LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = it.[CategoryId]
WHERE [ItemCategory].[CategoryId] IS NULL

UPDATE it SET [UserId] = 0
FROM [Item] it
LEFT JOIN [Users] ON [Users].[UserId] = it.[UserId]
WHERE [Users].[UserId] IS NULL

UPDATE it SET [CourseId] = 0
FROM [Item] it
LEFT JOIN [Course] ON [Course].[CourseId] = it.[CourseId]
WHERE [Course].[CourseId] IS NULL

UPDATE it SET [ShippingTimeMin] = 0
FROM [Item] it
WHERE it.[ShippingTimeMin] IS NULL

UPDATE it SET [ShippingTimeMax] = 0
FROM [Item] it
WHERE it.[ShippingTimeMax] IS NULL

UPDATE it SET [Price] = 0
FROM [Item] it
WHERE it.[Price] IS NULL

UPDATE it SET [OldPrice] = 0
FROM [Item] it
WHERE it.[OldPrice] IS NULL

UPDATE it SET [ProductCost] = 0
FROM [Item] it
WHERE it.[ProductCost] IS NULL

ALTER TABLE [Item] ALTER COLUMN [CategoryId] INT NOT NULL
ALTER TABLE [Item] ALTER COLUMN [ItemTypeId] INT NOT NULL
ALTER TABLE [Item] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [Item] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [Item] ALTER COLUMN [TaxId] INT NOT NULL
ALTER TABLE [Item] ALTER COLUMN [SupplierId] INT NOT NULL
ALTER TABLE [Item] ALTER COLUMN [CourseId] INT NOT NULL
ALTER TABLE [Item] ALTER COLUMN [BrandId] INT NOT NULL

ALTER TABLE [Item] ALTER COLUMN [ShippingTimeMin] INT NOT NULL
ALTER TABLE [Item] ALTER COLUMN [ShippingTimeMax] INT NOT NULL
ALTER TABLE [Item] ALTER COLUMN [Price] MONEY NOT NULL
ALTER TABLE [Item] ALTER COLUMN [OldPrice] MONEY NOT NULL
ALTER TABLE [Item] ALTER COLUMN [ProductCost] MONEY NOT NULL

ALTER TABLE [Item] ADD CONSTRAINT [FK_Item_ItemCategory_CategoryId] FOREIGN KEY(CategoryId) REFERENCES ItemCategory(CategoryId)
ALTER TABLE [Item] ADD CONSTRAINT [FK_Item_ItemTypes_ItemTypeId] FOREIGN KEY(ItemTypeId) REFERENCES ItemTypes(ItemTypeId)
ALTER TABLE [Item] ADD CONSTRAINT [FK_Item_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)
ALTER TABLE [Item] ADD CONSTRAINT [FK_Item_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)
ALTER TABLE [Item] ADD CONSTRAINT [FK_Item_Tax_TaxId] FOREIGN KEY(TaxId) REFERENCES Tax(TaxId)
ALTER TABLE [Item] ADD CONSTRAINT [FK_Item_Supplier_SupplierId] FOREIGN KEY(SupplierId) REFERENCES Supplier(SupplierId)
ALTER TABLE [Item] ADD CONSTRAINT [FK_Item_Course_CourseId] FOREIGN KEY(CourseId) REFERENCES Course(CourseId)
ALTER TABLE [Item] ADD CONSTRAINT [FK_Item_Brand_BrandId] FOREIGN KEY(BrandId) REFERENCES Brand(BrandId)

--------------- ItemCategory ------------------------
UPDATE [ItemCategory] SET [ItemTypeId] = 0 WHERE [ItemTypeId] IS NULL
UPDATE [ItemCategory] SET [UserId] = 0 WHERE [UserId] IS NULL
UPDATE [ItemCategory] SET [SiteId] = 0 WHERE [SiteId] IS NULL

UPDATE it SET [UserId] = 0
FROM [ItemCategory] it
LEFT JOIN [Users] ON [Users].[UserId] = it.[UserId]
WHERE [Users].[UserId] IS NULL

ALTER TABLE [ItemCategory] ALTER COLUMN [ItemTypeId] INT NOT NULL
ALTER TABLE [ItemCategory] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [ItemCategory] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [ItemCategory] ADD CONSTRAINT [FK_ItemCategory_ItemTypes_ItemTypeId] FOREIGN KEY(ItemTypeId) REFERENCES ItemTypes(ItemTypeId)
ALTER TABLE [ItemCategory] ADD CONSTRAINT [FK_ItemCategory_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)
ALTER TABLE [ItemCategory] ADD CONSTRAINT [FK_ItemCategory_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)

--------------- ItemChoice ------------------------
TRUNCATE TABLE ItemChoice
ALTER TABLE [ItemChoice] DROP CONSTRAINT [PK_ItemChoice]
ALTER TABLE [ItemChoice] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [ItemChoice] ALTER COLUMN [ModifierId] INT NOT NULL
ALTER TABLE [ItemChoice] ALTER COLUMN [ChoiceId] INT NOT NULL
ALTER TABLE [ItemChoice] ADD CONSTRAINT [PK_ItemChoice] PRIMARY KEY(ItemId, ModifierId, ChoiceId)
ALTER TABLE [ItemChoice] ADD CONSTRAINT [FK_ItemChoice_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)
ALTER TABLE [ItemChoice] ADD CONSTRAINT [FK_ItemChoice_Modifier_ModifierId] FOREIGN KEY(ModifierId) REFERENCES Modifier(ModifierId)
ALTER TABLE [ItemChoice] ADD CONSTRAINT [FK_ItemChoice_ModifierChoice_ChoiceId] FOREIGN KEY(ChoiceId) REFERENCES ModifierChoice(ChoiceId)

--------------- ItemConversionTracking ------------------------
DELETE it FROM [ItemConversionTracking] it
LEFT JOIN [Item] ON [Item].[ItemId] = it.ItemId
WHERE [Item].[ItemId] IS NULL
ALTER TABLE [ItemConversionTracking] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [ItemConversionTracking] ADD CONSTRAINT [FK_ItemConversionTracking_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)

--------------- ItemImage ------------------------
DELETE it
FROM [ItemImage] it
LEFT JOIN [Item] ON [Item].[ItemId] = it.[ItemId]
WHERE ISNULL([Item].[ItemId], 0) = 0

ALTER TABLE [ItemImage] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [ItemImage] ADD CONSTRAINT [FK_ItemImage_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)

--------------- ItemLang ------------------------
DELETE it
FROM [ItemLang] it
LEFT JOIN [Item] ON [Item].[ItemId] = it.[ItemId]
WHERE ISNULL([Item].[ItemId], 0) = 0

ALTER TABLE [ItemLang] DROP CONSTRAINT [PK_ItemLang]
ALTER TABLE [ItemLang] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [ItemLang] ALTER COLUMN [LangId] INT NOT NULL
ALTER TABLE [ItemLang] ADD CONSTRAINT [PK_ItemLang] PRIMARY KEY(ItemId, LangId)
ALTER TABLE [ItemLang] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [ItemLang] ALTER COLUMN [LangId] INT NOT NULL
ALTER TABLE [ItemLang] ADD CONSTRAINT [FK_ItemLang_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)
ALTER TABLE [ItemLang] ADD CONSTRAINT [FK_ItemLang_Language_LangId] FOREIGN KEY(LangId) REFERENCES Language(LangId)

--------------- ItemModifier ------------------------
DELETE it
FROM [ItemModifier] it
LEFT JOIN [Item] ON [Item].[ItemId] = it.[ItemId]
WHERE ISNULL([Item].[ItemId], 0) = 0

ALTER TABLE [ItemModifier] DROP CONSTRAINT [PK_ItemModifier]
ALTER TABLE [ItemModifier] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [ItemModifier] ALTER COLUMN [ModifierId] INT NOT NULL
ALTER TABLE [ItemModifier] ADD CONSTRAINT [PK_ItemModifier] PRIMARY KEY(ItemId, ModifierId)
ALTER TABLE [ItemModifier] ADD CONSTRAINT [FK_ItemModifier_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)
ALTER TABLE [ItemModifier] ADD CONSTRAINT [FK_ItemModifier_Modifier_ModifierId] FOREIGN KEY(ModifierId) REFERENCES Modifier(ModifierId)

--------------- ItemPosition ------------------------
DELETE it
FROM [ItemPosition] it
LEFT JOIN [Item] ON [Item].[ItemId] = it.[ItemId]
WHERE ISNULL([Item].[ItemId], 0) = 0

ALTER TABLE [ItemPosition] DROP CONSTRAINT [PK_ItemPosition]
ALTER TABLE [ItemPosition] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [ItemPosition] ADD CONSTRAINT [PK_ItemPosition] PRIMARY KEY(ItemId)
ALTER TABLE [ItemPosition] ADD CONSTRAINT [FK_ItemPosition_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)

--------------- ItemPrice ------------------------
DELETE it
FROM [ItemPrice] it
LEFT JOIN [Item] ON [Item].[ItemId] = it.[ItemId]
WHERE ISNULL([Item].[ItemId], 0) = 0

UPDATE [ItemPrice] SET [SiteId] = 0 WHERE [SiteId] IS NULL
UPDATE [ItemPrice] SET [UserId] = 0 WHERE [UserId] IS NULL
UPDATE [ItemPrice] SET [ItemId] = 0 WHERE [ItemId] IS NULL

ALTER TABLE [ItemPrice] DROP CONSTRAINT [PK_ItemPrice]
ALTER TABLE [ItemPrice] ALTER COLUMN [ItemPriceId] BIGINT NOT NULL
ALTER TABLE [ItemPrice] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [ItemPrice] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [ItemPrice] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [ItemPrice] ADD CONSTRAINT [PK_ItemPrice] PRIMARY KEY(ItemPriceId)
ALTER TABLE [ItemPrice] ADD CONSTRAINT [FK_ItemPrice_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)
ALTER TABLE [ItemPrice] ADD CONSTRAINT [FK_ItemPrice_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)
ALTER TABLE [ItemPrice] ADD CONSTRAINT [FK_ItemPrice_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)

--------------- ItemReview ------------------------
DELETE it
FROM [ItemReview] it
LEFT JOIN [Item] ON [Item].[ItemId] = it.[ItemId]
WHERE ISNULL([Item].[ItemId], 0) = 0

DELETE it
FROM [ItemReview] it
LEFT JOIN [Users] ON [Users].[UserId] = it.[UserId]
WHERE ISNULL([Users].[UserId], 0) = 0

ALTER TABLE [ItemReview] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [ItemReview] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [ItemReview] ADD CONSTRAINT [FK_ItemReview_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)
ALTER TABLE [ItemReview] ADD CONSTRAINT [FK_ItemReview_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)

--------------- MailingList ------------------------
DELETE it
FROM [MailingList] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE ISNULL([Site].[SiteId], 0) = 0

ALTER TABLE [MailingList] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [MailingList] ADD CONSTRAINT [FK_MailingList_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- MailingList ------------------------
DELETE it
FROM [MenuItem] it
LEFT JOIN [Menu] ON [Menu].[MenuId] = it.[MenuId]
WHERE ISNULL([Menu].[MenuId], 0) = 0

ALTER TABLE [MenuItem] ALTER COLUMN [MenuId] INT NOT NULL
ALTER TABLE [MenuItem] ADD CONSTRAINT [FK_MenuItem_Menu_MenuId] FOREIGN KEY(MenuId) REFERENCES Menu(MenuId)

--------------- Modifier ------------------------
DELETE it
FROM [Modifier] it
LEFT JOIN [Users] ON [Users].[UserId] = it.[UserId]
WHERE ISNULL([Users].[UserId], 0) = 0

DELETE it
FROM [Modifier] it
LEFT JOIN [ItemTypes] ON [ItemTypes].[ItemTypeId] = it.[ItemTypeId]
WHERE ISNULL([ItemTypes].[ItemTypeId], 0) = 0

ALTER TABLE [Modifier] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [Modifier] ALTER COLUMN [ItemTypeId] INT NOT NULL
ALTER TABLE [Modifier] ADD CONSTRAINT [FK_Modifier_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)
ALTER TABLE [Modifier] ADD CONSTRAINT [FK_Modifier_ItemTypes_ItemTypeId] FOREIGN KEY(ItemTypeId) REFERENCES ItemTypes(ItemTypeId)

--------------- ModifierChoice ------------------------
ALTER TABLE [ModifierChoice] ALTER COLUMN [ModifierId] INT NOT NULL
ALTER TABLE [ModifierChoice] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [ModifierChoice] ADD CONSTRAINT [FK_ModifierChoice_Modifier_ModifierId] FOREIGN KEY(ModifierId) REFERENCES Modifier(ModifierId)
ALTER TABLE [ModifierChoice] ADD CONSTRAINT [FK_ModifierChoice_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)

--------------- ModifierChoiceLang ------------------------
ALTER TABLE [ModifierChoiceLang] DROP CONSTRAINT [PK_ModifierChoiceLang]
ALTER TABLE [ModifierChoiceLang] ALTER COLUMN [ChoiceId] INT NOT NULL
ALTER TABLE [ModifierChoiceLang] ALTER COLUMN [LangId] INT NOT NULL
ALTER TABLE [ModifierChoiceLang] ADD CONSTRAINT [PK_ModifierChoiceLang] PRIMARY KEY(ChoiceId, LangId)
ALTER TABLE [ModifierChoiceLang] ADD CONSTRAINT [FK_ModifierChoiceLang_ModifierChoice_ChoiceId] FOREIGN KEY(ChoiceId) REFERENCES ModifierChoice(ChoiceId)
ALTER TABLE [ModifierChoiceLang] ADD CONSTRAINT [FK_ModifierChoiceLang_Language_LangId] FOREIGN KEY(LangId) REFERENCES Language(LangId)

--------------- ModifierLang ------------------------
ALTER TABLE [ModifierLang] DROP CONSTRAINT [PK_ModifierLang]
ALTER TABLE [ModifierLang] ALTER COLUMN [ModifierId] INT NOT NULL
ALTER TABLE [ModifierLang] ALTER COLUMN [LangId] INT NOT NULL
ALTER TABLE [ModifierLang] ADD CONSTRAINT [PK_ModifierLang] PRIMARY KEY(ModifierId, LangId)
ALTER TABLE [ModifierLang] ADD CONSTRAINT [FK_ModifierLang_Modifier_ModifierId] FOREIGN KEY(ModifierId) REFERENCES Modifier(ModifierId)
ALTER TABLE [ModifierLang] ADD CONSTRAINT [FK_ModifierLang_Language_LangId] FOREIGN KEY(LangId) REFERENCES Language(LangId)

--------------- Order ------------------------
UPDATE it SET [CouponId] = 0
FROM [Order] it
LEFT JOIN [Coupon] ON [Coupon].[CouponId] = it.[CouponId]
WHERE [Coupon].[CouponId] IS NULL

UPDATE it SET [AddressId] = 0
FROM [Order] it
LEFT JOIN [Address] ON [Address].[AddressId] = it.[AddressId]
WHERE [Address].[AddressId] IS NULL

UPDATE it SET [CustomerId] = 0
FROM [Order] it
LEFT JOIN [Users] ON [Users].[UserId] = it.[CustomerId]
WHERE [Users].[UserId] IS NULL

UPDATE [Order] SET [CouponId] = 0 WHERE [CouponId] IS NULL
UPDATE [Order] SET [AddressId] = 0 WHERE [AddressId] IS NULL
UPDATE [Order] SET [CustomerId] = 0 WHERE [CustomerId] IS NULL
ALTER TABLE [Order] ALTER COLUMN [CustomerId] BIGINT NOT NULL
ALTER TABLE [Order] ALTER COLUMN [AddressId] BIGINT NOT NULL
ALTER TABLE [Order] ALTER COLUMN [CouponId] BIGINT NOT NULL
ALTER TABLE [Order] ADD CONSTRAINT [FK_Order_Users_CustomerId] FOREIGN KEY(CustomerId) REFERENCES Users(UserId)
ALTER TABLE [Order] ADD CONSTRAINT [FK_Order_Address_AddressId] FOREIGN KEY(AddressId) REFERENCES Address(AddressId)
ALTER TABLE [Order] ADD CONSTRAINT [FK_Order_Coupon_CouponId] FOREIGN KEY(CouponId) REFERENCES Coupon(CouponId)

--------------- OrderItem ------------------------
UPDATE it SET [SiteId] = 0
FROM [OrderItem] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

UPDATE it SET [ItemId] = 0
FROM [OrderItem] it
LEFT JOIN [Item] ON [Item].[ItemId] = it.[ItemId]
WHERE [Item].[ItemId] IS NULL

ALTER TABLE [OrderItem] ALTER COLUMN [OrderId] BIGINT NOT NULL
ALTER TABLE [OrderItem] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [OrderItem] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [OrderItem] ALTER COLUMN [VatId] INT NOT NULL
ALTER TABLE [OrderItem] ALTER COLUMN [ItemCouponId] BIGINT NOT NULL
ALTER TABLE [OrderItem] ADD CONSTRAINT [FK_OrderItem_Order_OrderId] FOREIGN KEY(OrderId) REFERENCES [Order](OrderId)
ALTER TABLE [OrderItem] ADD CONSTRAINT [FK_OrderItem_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)
ALTER TABLE [OrderItem] ADD CONSTRAINT [FK_OrderItem_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)
ALTER TABLE [OrderItem] ADD CONSTRAINT [FK_OrderItem_Tax_VatId] FOREIGN KEY(VatId) REFERENCES Tax(TaxId)
ALTER TABLE [OrderItem] ADD CONSTRAINT [FK_OrderItem_Coupon_ItemCouponId] FOREIGN KEY(ItemCouponId) REFERENCES Coupon(CouponId)

--------------- Region ------------------------
DELETE it
FROM [Region] it
LEFT JOIN [Country] ON [Country].[CountryId] = it.[CountryId]
WHERE [Country].[CountryId] IS NULL

ALTER TABLE [Region] ALTER COLUMN [CountryId] INT NOT NULL
ALTER TABLE [Region] ADD CONSTRAINT [FK_OrderItem_Country_CountryId] FOREIGN KEY(CountryId) REFERENCES Country(CountryId)

--------------- ShippingTypeLang ------------------------
DELETE it
FROM [ShippingTypeLang] it
LEFT JOIN [ShippingType] ON [ShippingType].[ShippingTypeId] = it.[ShippingTypeId]
WHERE [ShippingType].[ShippingTypeId] IS NULL

ALTER TABLE [ShippingTypeLang] ALTER COLUMN [ShippingTypeId] INT NOT NULL
ALTER TABLE [ShippingTypeLang] ADD CONSTRAINT [FK_OrderItem_ShippingType_ShippingTypeId] FOREIGN KEY(ShippingTypeId) REFERENCES ShippingType(ShippingTypeId)

--------------- SiteCentralLineSEO ------------------------
DELETE it
FROM [SiteCentralLineSEO] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [SiteCentralLineSEO] ADD CONSTRAINT [FK_SiteCentralLineSEO_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- SiteCommercialFollowUp ------------------------
DELETE it
FROM [SiteCommercialFollowUp] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [SiteCommercialFollowUp] ADD CONSTRAINT [FK_SiteCommercialFollowUp_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- SiteCommunication ------------------------
DELETE it
FROM [SiteCommunication] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [SiteCommunication] ADD CONSTRAINT [FK_SiteCommunication_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- SiteEvent ------------------------
DELETE it
FROM [SiteEvent] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [SiteEvent] ADD CONSTRAINT [FK_SiteEvent_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- SiteFinancial ------------------------
DELETE it
FROM [SiteFinancial] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [SiteFinancial] ADD CONSTRAINT [FK_SiteFinancial_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- SiteImage ------------------------
DELETE it
FROM [SiteImage] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [SiteImage] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [SiteImage] ADD CONSTRAINT [FK_SiteImage_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)

--------------- SiteLang ------------------------
DELETE it
FROM [SiteLang] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [SiteLang] DROP CONSTRAINT [PK_SiteLang]
ALTER TABLE [SiteLang] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [SiteLang] ALTER COLUMN [LangId] INT NOT NULL
ALTER TABLE [SiteLang] ADD CONSTRAINT [PK_SiteLang] PRIMARY KEY(SiteId, LangId)
ALTER TABLE [SiteLang] ADD CONSTRAINT [FK_SiteLang_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)
ALTER TABLE [SiteLang] ADD CONSTRAINT [FK_SiteLang_Language_LangId] FOREIGN KEY(LangId) REFERENCES Language(LangId)

--------------- SiteRestaurantProductCategory ------------------------
DELETE it
FROM [SiteRestaurantProductCategory] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [SiteRestaurantProductCategory] ADD CONSTRAINT [FK_SiteRestaurantProductCategory_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)
ALTER TABLE [SiteRestaurantProductCategory] ADD CONSTRAINT [FK_SiteRestaurantProductCategory_RestaurantProductCategory_RestaurantProductCategoryId] FOREIGN KEY(RestaurantProductCategoryId) REFERENCES RestaurantProductCategory(RestaurantProductCategoryId)

--------------- SiteRestaurantSupplier ------------------------
DELETE it
FROM [SiteRestaurantSupplier] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [SiteRestaurantSupplier] ADD CONSTRAINT [FK_SiteRestaurantSupplier_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)
ALTER TABLE [SiteRestaurantSupplier] ADD CONSTRAINT [FK_SiteRestaurantSupplier_RestaurantSupplier_RestaurantSupplierId] FOREIGN KEY(RestaurantSupplierId) REFERENCES RestaurantSupplier(RestaurantSupplierId)

--------------- SiteReview ------------------------
DELETE it
FROM [SiteReview] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [SiteReview] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [SiteReview] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [SiteReview] ADD CONSTRAINT [FK_SiteReview_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)
ALTER TABLE [SiteReview] ADD CONSTRAINT [FK_SiteReview_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)

--------------- State ------------------------
DELETE it
FROM [State] it
LEFT JOIN [Region] ON [Region].[RegionId] = it.[RegionId]
WHERE [Region].[RegionId] IS NULL

ALTER TABLE [State] ALTER COLUMN [RegionId] INT NOT NULL
ALTER TABLE [State] ADD CONSTRAINT [FK_State_Region_RegionId] FOREIGN KEY(RegionId) REFERENCES Region(RegionId)

--------------- Supplier ------------------------
UPDATE it SET [SupplierId] = 0
FROM [Item] it

UPDATE it SET Active = 0
FROM [Supplier] it
LEFT JOIN [Users] ON [Users].[UserId] = it.[UserId]
WHERE [Users].[UserId] IS NULL

ALTER TABLE [Supplier] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [Supplier] ADD CONSTRAINT [FK_Supplier_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)

--------------- TeeSheet ------------------------
DELETE it
FROM [TeeSheet] it
LEFT JOIN [Item] ON [Item].[ItemId] = it.[ItemId]
WHERE [Item].[ItemId] IS NULL

UPDATE it SET [CourseId] = 0
FROM [TeeSheet] it
LEFT JOIN [Course] ON [Course].[CourseId] = it.[CourseId]
WHERE [Course].[CourseId] IS NULL

ALTER TABLE [TeeSheet] ALTER COLUMN [CourseId] INT NOT NULL
ALTER TABLE [TeeSheet] ALTER COLUMN [ItemId] BIGINT NOT NULL
ALTER TABLE [TeeSheet] ADD CONSTRAINT [FK_TeeSheet_Course_CourseId] FOREIGN KEY(CourseId) REFERENCES Course(CourseId)
ALTER TABLE [TeeSheet] ADD CONSTRAINT [FK_TeeSheet_Item_ItemId] FOREIGN KEY(ItemId) REFERENCES Item(ItemId)

--------------- TitleLang ------------------------
DELETE it
FROM [TitleLang] it
LEFT JOIN [Title] ON [Title].[TitleId] = it.[TitleId]
WHERE [Title].[TitleId] IS NULL

ALTER TABLE [TitleLang] DROP CONSTRAINT [PK_TitleLang]
ALTER TABLE [TitleLang] ALTER COLUMN [TitleId] INT NOT NULL
ALTER TABLE [TitleLang] ALTER COLUMN [LangId] INT NOT NULL
ALTER TABLE [TitleLang] ADD CONSTRAINT [PK_TitleLang] PRIMARY KEY(TitleId, LangId)
ALTER TABLE [TitleLang] ADD CONSTRAINT [FK_TitleLang_Title_TitleId] FOREIGN KEY(TitleId) REFERENCES Title(TitleId)
ALTER TABLE [TitleLang] ADD CONSTRAINT [FK_TitleLang_Language_LangId] FOREIGN KEY(LangId) REFERENCES Language(LangId)

--------------- UserInterested ------------------------
TRUNCATE TABLE [UserInterested]
ALTER TABLE [UserInterested] DROP CONSTRAINT [PK_UserInterested]
ALTER TABLE [UserInterested] ALTER COLUMN [InterestId] INT NOT NULL
ALTER TABLE [UserInterested] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [UserInterested] ADD CONSTRAINT [PK_UserInterested] PRIMARY KEY(InterestId, UserId)
ALTER TABLE [UserInterested] ADD CONSTRAINT [FK_UserInterested_Interest_InterestId] FOREIGN KEY(InterestId) REFERENCES Interest(InterestId)
ALTER TABLE [UserInterested] ADD CONSTRAINT [FK_UserInterested_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)

--------------- UserMessage ------------------------
DELETE it
FROM [UserMessage] it
LEFT JOIN [Users] ON [Users].[UserId] = it.[FromUserId]
WHERE [Users].[UserId] IS NULL

DELETE it
FROM [UserMessage] it
LEFT JOIN [Users] ON [Users].[UserId] = it.[ToUserId]
WHERE [Users].[UserId] IS NULL

ALTER TABLE [UserMessage] DROP CONSTRAINT [PK_UserMessage]
ALTER TABLE [UserMessage] ALTER COLUMN [MessageId] BIGINT NOT NULL
ALTER TABLE [UserMessage] ADD CONSTRAINT [PK_UserMessage] PRIMARY KEY(MessageId)
ALTER TABLE [UserMessage] ALTER COLUMN [FromUserId] BIGINT NOT NULL
ALTER TABLE [UserMessage] ALTER COLUMN [ToUserId] BIGINT NOT NULL
ALTER TABLE [UserMessage] ADD CONSTRAINT [FK_UserMessage_Users_FromUserId] FOREIGN KEY(FromUserId) REFERENCES Users(UserId)
ALTER TABLE [UserMessage] ADD CONSTRAINT [FK_UserMessage_Users_ToUserId] FOREIGN KEY(ToUserId) REFERENCES Users(UserId)

--------------- UserMessageAttached ------------------------
DELETE it
FROM [UserMessageAttached] it
LEFT JOIN [UserMessage] ON [UserMessage].[MessageId] = it.[MessageId]
WHERE [UserMessage].[MessageId] IS NULL

ALTER TABLE [UserMessageAttached] DROP CONSTRAINT [PK_UserMessageAttached]
ALTER TABLE [UserMessageAttached] ALTER COLUMN [AttachedId] BIGINT NOT NULL
ALTER TABLE [UserMessageAttached] ADD CONSTRAINT [PK_UserMessageAttached] PRIMARY KEY(AttachedId)
ALTER TABLE [UserMessageAttached] ALTER COLUMN [MessageId] BIGINT NOT NULL
ALTER TABLE [UserMessageAttached] ADD CONSTRAINT [FK_UserMessageAttached_UserMessage_MessageId] FOREIGN KEY(MessageId) REFERENCES UserMessage(MessageId)

--------------- WebContent ------------------------
UPDATE it SET [UserId] = 0
FROM [WebContent] it
LEFT JOIN [Users] ON [Users].[UserId] = it.[UserId]
WHERE [Users].[UserId] IS NULL

ALTER TABLE [WebContent] DROP CONSTRAINT [PK_WebContent]
ALTER TABLE [WebContent] ALTER COLUMN [ContentId] BIGINT NOT NULL
ALTER TABLE [WebContent] ADD CONSTRAINT [PK_WebContent] PRIMARY KEY(ContentId)

--------------- Users ------------------------
UPDATE it SET [CustomerTypeId] = 0
FROM [Users] it
LEFT JOIN [CustomerType] ON [CustomerType].[CustomerTypeId] = it.[CustomerTypeId]
WHERE [CustomerType].[CustomerTypeId] IS NULL

UPDATE it SET [SubCustomerTypeId] = 0
FROM [Users] it
LEFT JOIN [CustomerType] ON [CustomerType].[CustomerTypeId] = it.[SubCustomerTypeId]
WHERE [CustomerType].[CustomerTypeId] IS NULL

UPDATE it SET [UserTypeId] = 2
FROM [Users] it
LEFT JOIN [UserTypes] ON [UserTypes].[UserTypeId] = it.[UserTypeId]
WHERE [UserTypes].[UserTypeId] IS NULL

UPDATE it SET [TitleId] = 0
FROM [Users] it
LEFT JOIN [Title] ON [Title].[TitleId] = it.[TitleId]
WHERE [Title].[TitleId] IS NULL

UPDATE it SET [CityId] = 0
FROM [Users] it
LEFT JOIN [City] ON [City].[CityId] = it.[CityId]
WHERE [City].[CityId] IS NULL

UPDATE it SET [ShippingCityId] = 0
FROM [Users] it
LEFT JOIN [City] ON [City].[CityId] = it.[ShippingCityId]
WHERE [City].[CityId] IS NULL

UPDATE it SET [CountryId] = 0
FROM [Users] it
LEFT JOIN [Country] ON [Country].[CountryId] = it.[CountryId]
WHERE [Country].[CountryId] IS NULL

UPDATE it SET [ShippingCountryId] = 0
FROM [Users] it
LEFT JOIN [Country] ON [Country].[CountryId] = it.[ShippingCountryId]
WHERE [Country].[CountryId] IS NULL

UPDATE it SET [SiteId] = 0
FROM [Users] it
LEFT JOIN [Site] ON [Site].[SiteId] = it.[SiteId]
WHERE [Site].[SiteId] IS NULL

ALTER TABLE [Users] ALTER COLUMN [CustomerTypeId] BIGINT NOT NULL
ALTER TABLE [Users] ALTER COLUMN [SubCustomerTypeId] BIGINT NOT NULL
ALTER TABLE [Users] ALTER COLUMN [SiteId] BIGINT NOT NULL
ALTER TABLE [Users] ALTER COLUMN [UserTypeId] INT NOT NULL
ALTER TABLE [Users] ALTER COLUMN [TitleId] INT NOT NULL
ALTER TABLE [Users] ALTER COLUMN [CityId] INT NOT NULL
ALTER TABLE [Users] ALTER COLUMN [CountryId] INT NOT NULL
ALTER TABLE [Users] ALTER COLUMN [ShippingCityId] INT NOT NULL
ALTER TABLE [Users] ALTER COLUMN [ShippingCountryId] INT NOT NULL
ALTER TABLE [Users] ALTER COLUMN [ModifyUserId] BIGINT NULL
ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_CustomerType_CustomerTypeId] FOREIGN KEY(CustomerTypeId) REFERENCES CustomerType(CustomerTypeId)
ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_CustomerType_SubCustomerTypeId] FOREIGN KEY(SubCustomerTypeId) REFERENCES CustomerType(CustomerTypeId)
ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_UserTypes_UserTypeId] FOREIGN KEY(UserTypeId) REFERENCES UserTypes(UserTypeId)
ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Site_SiteId] FOREIGN KEY(SiteId) REFERENCES Site(SiteId)
ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Title_TitleId] FOREIGN KEY(TitleId) REFERENCES Title(TitleId)
ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_City_CityId] FOREIGN KEY(CityId) REFERENCES City(CityId)
ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Country_CountryId] FOREIGN KEY(CountryId) REFERENCES Country(CountryId)
ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_City_ShippingCityId] FOREIGN KEY(ShippingCityId) REFERENCES City(CityId)
ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Country_ShippingCountryId] FOREIGN KEY(ShippingCountryId) REFERENCES Country(CountryId)

--------------- Site ------------------------
UPDATE it SET [GolfBrandId] = 0
FROM [Site] it
LEFT JOIN [GolfBrand] ON [GolfBrand].[GolfBrandId] = it.[GolfBrandId]
WHERE [GolfBrand].[GolfBrandId] IS NULL

UPDATE it SET [StateId] = 0
FROM [Site] it
LEFT JOIN [State] ON [State].[StateId] = it.[StateId]
WHERE [State].[StateId] IS NULL

UPDATE it SET [RegionId] = 0
FROM [Site] it
LEFT JOIN [Region] ON [Region].[RegionId] = it.[RegionId]
WHERE [Region].[RegionId] IS NULL

UPDATE it SET [CountryId] = 0
FROM [Site] it
LEFT JOIN [Country] ON [Country].[CountryId] = it.[CountryId]
WHERE [Country].[CountryId] IS NULL

UPDATE it SET [UserId] = 0
FROM [Site] it
LEFT JOIN [Users] ON [Users].[UserId] = it.[UserId]
WHERE [Users].[UserId] IS NULL

UPDATE it SET [ReservationAPI] = 0
FROM [Site] it
LEFT JOIN [ReservationAPI] ON [ReservationAPI].[APIId] = it.[ReservationAPI]
WHERE [ReservationAPI].[APIId] IS NULL

ALTER TABLE [Site] ALTER COLUMN [GolfBrandId] INT NOT NULL
ALTER TABLE [Site] ALTER COLUMN [StateId] INT NOT NULL
ALTER TABLE [Site] ALTER COLUMN [RegionId] INT NOT NULL
ALTER TABLE [Site] ALTER COLUMN [CountryId] INT NOT NULL
ALTER TABLE [Site] ALTER COLUMN [UserId] BIGINT NOT NULL
ALTER TABLE [Site] ALTER COLUMN [ReservationAPI] INT NOT NULL
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_GolfBrand_GolfBrandId] FOREIGN KEY(GolfBrandId) REFERENCES GolfBrand(GolfBrandId)
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_State_StateId] FOREIGN KEY(StateId) REFERENCES State(StateId)
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_Region_RegionId] FOREIGN KEY(RegionId) REFERENCES Region(RegionId)
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_Country_CountryId] FOREIGN KEY(CountryId) REFERENCES Country(CountryId)
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_Users_UserId] FOREIGN KEY(UserId) REFERENCES Users(UserId)
ALTER TABLE [Site] ADD CONSTRAINT [FK_Site_ReservationAPI_ReservationAPI] FOREIGN KEY(ReservationAPI) REFERENCES ReservationAPI(APIId)