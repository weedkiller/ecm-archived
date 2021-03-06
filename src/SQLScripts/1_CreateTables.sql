USE [DLG]
GO

-- Add New Table

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebContent]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[WebContent](
		[ContentId] [int] IDENTITY(1,1) NOT NULL,
		[ContentCategory] [nvarchar](200) NULL,
		[ContentKey] [nvarchar](200) NULL,
		[Active] [bit] NULL,
		[UserId] [int] NULL,
		[InsertDate] [datetime] NULL,
	 CONSTRAINT [PK_WebContent] PRIMARY KEY CLUSTERED 
	(
		[ContentId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SiteReview]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SiteReview](
	[SiteReviewId] [bigint] IDENTITY(1,1) NOT NULL,
	[SiteId] [bigint] NULL,
	[UserId] [int] NULL,
	[Rating] [tinyint] NULL,
	[Message] [ntext] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_SiteReview] PRIMARY KEY CLUSTERED 
(
	[SiteReviewId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[SiteReview] ADD  CONSTRAINT [DF_SiteReview_Score]  DEFAULT ((0)) FOR [Rating]

ALTER TABLE [dbo].[SiteReview] ADD  CONSTRAINT [DF_SiteReview_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]

END

GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Options]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[Options](
		[OptionKey] [nvarchar](50) NOT NULL,
		[OptionValue] [ntext] NULL,
	 CONSTRAINT [PK_Options] PRIMARY KEY CLUSTERED 
	(
		[OptionKey] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ModifierLang]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[ModifierLang](
		[ModifierId] [int] NOT NULL,
		[LangId] [int] NOT NULL,
		[ModifierName] [nvarchar](100) NULL,
		[ModifierDesc] [nvarchar](500) NULL,
	 CONSTRAINT [PK_ModifierLang] PRIMARY KEY CLUSTERED 
	(
		[ModifierId] ASC,
		[LangId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

END

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ModifierChoiceLang]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[ModifierChoiceLang](
		[ChoiceId] [int] NOT NULL,
		[LangId] [int] NOT NULL,
		[ChoiceName] [nvarchar](200) NULL,
	 CONSTRAINT [PK_ModifierChoiceLang] PRIMARY KEY CLUSTERED 
	(
		[ChoiceId] ASC,
		[LangId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

END

GO

IF EXISTS(SELECT * FROM sys.columns WHERE name = 'ModifierName' AND object_id = OBJECT_ID('Modifier'))
BEGIN
	ALTER TABLE Modifier DROP COLUMN ModifierName
END

IF EXISTS(SELECT * FROM sys.columns WHERE name = 'ModifierDesc' AND object_id = OBJECT_ID('Modifier'))
BEGIN
	ALTER TABLE Modifier DROP COLUMN ModifierDesc
END

GO

-- Update table field

DECLARE @@HasRow int
DECLARE @@table_name sysname
DECLARE @@column_name sysname

SET @@table_name = 'Users'
SET @@column_name = 'IsSubscriber'
SET @@HasRow = ( SELECT   COUNT(convert(sysname, c.name))
FROM     syscolumns c, sysobjects o
Where   c.id = o.id
    AND convert(sysname, o.name) = @@table_name
        AND convert(sysname, c.name) = @@column_name)
if ( @@HasRow = 0 )
BEGIN
    ALTER table Users Add IsSubscriber BIT NULL
END
 


GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Prefix' AND object_id = OBJECT_ID(N'ItemCategory'))
BEGIN
	ALTER TABLE ItemCategory ADD Prefix NVARCHAR(10)
END


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GolfBrand]') AND type in (N'U'))
BEGIN
		CREATE TABLE [dbo].[GolfBrand](
		[GolfBrandId] [int] IDENTITY(1,1) NOT NULL,
		[GolfBrandName] [nvarchar](50) NULL,
		[GolfBrandDesc] [nvarchar](200) NULL,
		[Active] [bit] NULL,
	 CONSTRAINT [PK_GolfBrand] PRIMARY KEY CLUSTERED 
	(
		[GolfBrandId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'GolfBrandId' AND object_id = OBJECT_ID(N'Site'))
BEGIN
	ALTER TABLE [Site] ADD GolfBrandId INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'AlbatrosCourseId' AND object_id = OBJECT_ID(N'Site'))
BEGIN
	ALTER TABLE [Site] ADD AlbatrosCourseId INT NULL
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Title]') AND type in (N'U'))
BEGIN
    DROP TABLE [Title]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Title]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[Title](
		[TitleId] [int] IDENTITY(1,1) NOT NULL,
		[InsertDate] [datetime] NULL,
		[Active] [bit] NULL,
	 CONSTRAINT [PK__Title__48BAC3E5] PRIMARY KEY CLUSTERED 
	(
		[TitleId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TitleLang]') AND type in (N'U'))
BEGIN
    DROP TABLE [TitleLang]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TitleLang]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[TitleLang](
		[TitleId] [int] NOT NULL,
		[LangId] [int] NOT NULL,
		[TitleName] [nvarchar](20) NULL,
	 CONSTRAINT [PK_TitleLang] PRIMARY KEY CLUSTERED 
	(
		[TitleId] ASC,
		[LangId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Title' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] DROP [Title]
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'TitleId' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] ADD [TitleId] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'PracticalInfo' AND object_id = OBJECT_ID(N'SiteLang'))
BEGIN
	ALTER TABLE [SiteLang] ADD [PracticalInfo] NTEXT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Accommodation' AND object_id = OBJECT_ID(N'SiteLang'))
BEGIN
	ALTER TABLE [SiteLang] ADD [Accommodation] NTEXT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Restaurant' AND object_id = OBJECT_ID(N'SiteLang'))
BEGIN
	ALTER TABLE [SiteLang] ADD [Restaurant] NTEXT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'RegionId' AND object_id = OBJECT_ID(N'Site'))
BEGIN
	ALTER TABLE [Site] ADD [RegionId] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'AlbatrosCourseId' AND object_id = OBJECT_ID(N'Site'))
BEGIN
	ALTER TABLE [Site] ADD [AlbatrosCourseId] INT NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Coupon]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Coupon](
	[CouponId] [bigint] IDENTITY(1,1) NOT NULL,
	[CouponGroupId] [bigint] NOT NULL,
	[CouponCode] [nvarchar](20) NULL,
	[InsertedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_Coupon] PRIMARY KEY CLUSTERED 
(
	[CouponId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'CouponId' AND object_id = OBJECT_ID(N'Order'))
BEGIN
	ALTER TABLE [Order] ADD [CouponId] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'IsUserCanSelectDate' AND object_id = OBJECT_ID(N'Item'))
BEGIN
	ALTER TABLE [Item] ADD [IsUserCanSelectDate] BIT NULL DEFAULT(1)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Creditcardassociations]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Creditcardassociations](
	[CardassociationsId] [int] NOT NULL,
	[CardassociationsType] [nvarchar](255) NOT NULL,
	[Active] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Creditcardassociations] PRIMARY KEY CLUSTERED 
(
	[CardassociationsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CouponGroup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CouponGroup](
	[CouponGroupId] [bigint] IDENTITY(1,1) NOT NULL,
	[CouponGroupName] [nvarchar](50) NULL,
	[CouponGroupDesc] [ntext] NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[Reduction] [decimal](15, 2) NULL,
	[CouponType] [int] NULL,
	[CouponUsageType] [int] NULL,
	[InsertedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_CouponGroup] PRIMARY KEY CLUSTERED 
(
	[CouponGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'TitleId' AND object_id = OBJECT_ID(N'Address'))
BEGIN
	ALTER TABLE [Address] ADD [TitleId] INT NULL
END
GO

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Civilite' AND object_id = OBJECT_ID(N'Address'))
BEGIN
	ALTER TABLE [Address] DROP COLUMN [Civilite]
END
GO

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'City' AND object_id = OBJECT_ID(N'Address'))
BEGIN
	ALTER TABLE [Address] DROP COLUMN [City]
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'CityId' AND object_id = OBJECT_ID(N'Address'))
BEGIN
	ALTER TABLE [Address] ADD [CityId] INT NULL
END
GO


/****** Object:  Table [dbo].[SponsorEmail]    Script Date: 3/30/2015 10:50:13 PM ******/
DROP TABLE [dbo].[SponsorEmail]
GO

/****** Object:  Table [dbo].[SponsorEmail]    Script Date: 3/30/2015 10:50:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SponsorEmail](
	[SponsorEmailId] [int] IDENTITY(1,1) NOT NULL,
	[Subject] [nvarchar](50) NULL,
	[Body] [ntext] NULL,
	[FromUserId] INT NULL,
	[ToEmail] nvarchar(50) NULL,
	[InsertDate] [datetime] NULL,
	[UpdateDate] [datetime] NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_SponsorEmail] PRIMARY KEY CLUSTERED 
(
	[SponsorEmailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

/********** CardType ******************/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CardType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CardType](
	[CardTypeId] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_CardType] PRIMARY KEY CLUSTERED 
(
	[CardTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CardTypeLang]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CardTypeLang](
	[CardTypeId] [int] NOT NULL,
	[LangId] [int] NOT NULL,
	[CardTypeName] [nvarchar](50) NULL,
 CONSTRAINT [PK_CardTypeLang] PRIMARY KEY CLUSTERED 
(
	[CardTypeId] ASC,
	[LangId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'CardTypeId' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] ADD [CardTypeId] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'CardId' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] ADD [CardId] NVARCHAR(50) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'PersonalAddress' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] ADD [PersonalAddress] NVARCHAR(500) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'PersonalPostalCode' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] ADD [PersonalPostalCode] NVARCHAR(50) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'PersonalCountryId' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] ADD [PersonalCountryId] NVARCHAR(50) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerGroupCustomer]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomerGroupCustomer](
	[CustomerId] [int] NOT NULL,
	[CustomerGroupId] [int] NOT NULL,
 CONSTRAINT [PK_CustomerGroupCustomer] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[CustomerGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'InvoiceName' AND object_id = OBJECT_ID(N'ItemLang'))
BEGIN
	ALTER TABLE [ItemLang] ADD [InvoiceName] NVARCHAR(500) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'SiteId' AND object_id = OBJECT_ID(N'MailingList'))
BEGIN
	ALTER TABLE [MailingList] ADD [SiteId] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'SiteId' AND object_id = OBJECT_ID(N'CustomerGroup'))
BEGIN
	ALTER TABLE [CustomerGroup] ADD [SiteId] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'SiteId' AND object_id = OBJECT_ID(N'Category'))
BEGIN
	ALTER TABLE [Category] ADD [SiteId] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'SiteId' AND object_id = OBJECT_ID(N'Impressum'))
BEGIN
	ALTER TABLE [Impressum] ADD [SiteId] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'CityId' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] ADD [CityId] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'ShippingCityId' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] ADD [ShippingCityId] INT NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Adset]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[Adset](
		[AdsetId] [int] NOT NULL,
		[AdsetName] [nvarchar](50) NULL,
		[Active] bit NULL
	 CONSTRAINT [PK_Adset] PRIMARY KEY CLUSTERED 
	(
		[AdsetId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Ads]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[Ads](
		[AdsId] [int] IDENTITY(1,1) NOT NULL,
		[AdsetId] INT NULL,
		[AdsName] [nvarchar](50) NULL,
		[LinkUrl] [nvarchar](255) NULL,
		[ImageUrl] [nvarchar](255) NULL,
		[ListNo] INT DEFAULT(0) NULL,
		[Active] bit NULL
	 CONSTRAINT [PK_Ads] PRIMARY KEY CLUSTERED 
	(
		[AdsId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END

GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'PhoneCountryCode' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] ADD [PhoneCountryCode] NVARCHAR(10) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'MobilePhoneCountryCode' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] ADD [MobilePhoneCountryCode] NVARCHAR(10) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'ShippingPhoneCountryCode' AND object_id = OBJECT_ID(N'Users'))
BEGIN
	ALTER TABLE [Users] ADD [ShippingPhoneCountryCode] NVARCHAR(10) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'PhoneCountryCode' AND object_id = OBJECT_ID(N'Address'))
BEGIN
	ALTER TABLE [Address] ADD [PhoneCountryCode] NVARCHAR(10) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'MobilePhoneCountryCode' AND object_id = OBJECT_ID(N'Address'))
BEGIN
	ALTER TABLE [Address] ADD [MobilePhoneCountryCode] NVARCHAR(10) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Order' AND object_id = OBJECT_ID(N'RequestId'))
BEGIN
	ALTER TABLE [Order] ADD [RequestId] NVARCHAR(50) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerAddress]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[CustomerAddress](
		[CustomerId] [int] NOT NULL,
		[PersonalAddress] [nvarchar](500) NULL,
		[PersonalCity] [nvarchar](200) NULL,
		[PersonalPostalCode] [nvarchar](50) NULL,
		[PersonalCountryId] [int] NULL,
		[PersonalPhone] [nvarchar](200) NULL,
		[BusinessAddress] [nvarchar](500) NULL,
		[BusinessCity] [nvarchar](200) NULL,
		[BusinessPostalCode] [nvarchar](50) NULL,
		[BusinessCountryId] [int] NULL,
		[BusinessPhone] [nvarchar](200) NULL,
	 CONSTRAINT [PK_CustomerAddress] PRIMARY KEY CLUSTERED 
	(
		[CustomerId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomerGroupCustomer]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomerGroupCustomer](
	[CustomerId] [int] NOT NULL,
	[CustomerGroupId] [int] NOT NULL,
 CONSTRAINT [PK_CustomerGroupCustomer] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[CustomerGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'Subject' AND object_id = OBJECT_ID(N'SponsorEmail'))
BEGIN
	ALTER TABLE SponsorEmail ALTER COLUMN Subject nvarchar(100)
END
GO

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'ToEmail' AND object_id = OBJECT_ID(N'SponsorEmail'))
BEGIN
	ALTER TABLE SponsorEmail ALTER COLUMN ToEmail nvarchar(100)
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'LinkUrl' AND object_id = OBJECT_ID(N'SlideImage'))
BEGIN
	ALTER TABLE SlideImage ADD LinkUrl nvarchar(255)
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'FromDate' AND object_id = OBJECT_ID(N'Ads'))
BEGIN
	ALTER TABLE Ads ADD FromDate DateTime NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'ToDate' AND object_id = OBJECT_ID(N'Ads'))
BEGIN
	ALTER TABLE Ads ADD ToDate DateTime NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'AlbatrosUrl' AND object_id = OBJECT_ID(N'Site'))
BEGIN
	ALTER TABLE Site ADD AlbatrosUrl NVARCHAR(255) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'AlbatrosUsername' AND object_id = OBJECT_ID(N'Site'))
BEGIN
	ALTER TABLE Site ADD AlbatrosUsername NVARCHAR(50) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'AlbatrosPassword' AND object_id = OBJECT_ID(N'Site'))
BEGIN
	ALTER TABLE Site ADD AlbatrosPassword NVARCHAR(50) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Visible' AND object_id = OBJECT_ID(N'Site'))
BEGIN
	ALTER TABLE [Site] ADD [Visible] BIT DEFAULT(1) NULL;
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'SiteId' AND object_id = OBJECT_ID(N'OrderItem'))
BEGIN
	ALTER TABLE [OrderItem] ADD [SiteId] INT NULL;
END
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_TopNavLink_ListNo]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[TopNavLink] DROP CONSTRAINT [DF_TopNavLink_ListNo]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TopNavLink]') AND type in (N'U'))
DROP TABLE [dbo].[TopNavLink]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TopNavLink]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[TopNavLink](
		[TopNavLinkId] [int] IDENTITY(1,1) NOT NULL,
		[LinkUrl] [nvarchar](255) NULL,
		[ImageUrl] [nvarchar](255) NULL,
		[ListNo] [int] NULL DEFAULT(0),
	 CONSTRAINT [PK_TopNavLink] PRIMARY KEY CLUSTERED 
	(
		[TopNavLinkId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'ItemName' AND object_id = OBJECT_ID(N'OrderItem'))
BEGIN
	ALTER TABLE [OrderItem] ALTER COLUMN [ItemName] NVARCHAR(1000) NULL;
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailTemplate]') AND type in (N'U'))
DROP TABLE [dbo].[EmailTemplate]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailTemplate]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[EmailTemplate](
		[TemplateId] [int] IDENTITY(1,1) NOT NULL,
		[TemplateKey] [nvarchar](50) NULL,
		[CategoryId] [int] NULL,
		[UserId] [int] NULL,
		[Active] [bit] NULL,
		[InsertDate] [datetime] NULL,
		[UpdateDate] [datetime] NULL,
		[FileName1] [nvarchar](200) NULL,
		[FileName2] [nvarchar](200) NULL,
		[FileName3] [nvarchar](200) NULL,
		[FileName4] [nvarchar](200) NULL,
		[FileName5] [nvarchar](200) NULL,
		[FileDescription1] [nvarchar](300) NULL,
		[FileDescription2] [nvarchar](300) NULL,
		[FileDescription3] [nvarchar](300) NULL,
		[FileDescription4] [nvarchar](300) NULL,
		[FileDescription5] [nvarchar](300) NULL,
		[FileUrl1] [nvarchar](300) NULL,
		[FileUrl2] [nvarchar](300) NULL,
		[FileUrl3] [nvarchar](300) NULL,
		[FileUrl4] [nvarchar](300) NULL,
		[FileUrl5] [nvarchar](300) NULL,
	 CONSTRAINT [PK_EmailTemplate] PRIMARY KEY CLUSTERED 
	(
		[TemplateId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailTemplateLang]') AND type in (N'U'))
DROP TABLE [dbo].[EmailTemplateLang]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailTemplateLang]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[EmailTemplateLang](
		[TemplateId] [int] NOT NULL,
		[LangId] [int] NOT NULL,
		[Name] [nvarchar](100) NULL,
		[Description] [nvarchar](500) NULL,
		[Subject] [nvarchar](100) NULL,
		[HtmlDetail] [image] NULL,
		[TextDetail] [ntext] NULL,
	 CONSTRAINT [PK_EmailTemplateLang] PRIMARY KEY CLUSTERED 
	(
		[TemplateId] ASC, [LangId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailTemplateVariable]') AND type in (N'U'))
DROP TABLE [dbo].[EmailTemplateVariable]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailTemplateVariable]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[EmailTemplateVariable](
		[VariableId] [int] IDENTITY(1,1) NOT NULL,
		[VariableName] [nvarchar](50) NULL,
		[Description] [nvarchar](500) NULL,
	 CONSTRAINT [PK_EmailTemplateVariable] PRIMARY KEY CLUSTERED 
	(
		[VariableId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Menu]') AND type in (N'U'))
DROP TABLE [dbo].[Menu]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Menu]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Menu](
	[MenuId] [int] IDENTITY(1,1) NOT NULL,
	[MenuName] [nvarchar](50) NULL,
	[MenuPlacement] [nvarchar](50) NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[MenuId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'MenuItem') AND type in (N'U'))
DROP TABLE [dbo].MenuItem
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'MenuItem') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MenuItem](
	[MenuItemId] [int] IDENTITY(1,1) NOT NULL,
	[MenuId] [int] NULL,
	[ParentId] [int] NULL,
	[MenuType] [nvarchar](20) NULL,
	[MenuTitle] [nvarchar](50) NULL,
	[MenuValue] [nvarchar](255) NULL,
	[ListNo] [int] DEFAULT(0) NULL,
 CONSTRAINT [PK_MenuItem] PRIMARY KEY CLUSTERED 
(
	[MenuItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'ListNo' AND object_id = OBJECT_ID(N'MenuItem'))
BEGIN
	ALTER TABLE [MenuItem] ADD [ListNo] INT DEFAULT(0) NULL;
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Subject' AND object_id = OBJECT_ID(N'ItemReview'))
BEGIN
	ALTER TABLE [ItemReview] ADD [Subject] nvarchar(100)
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'Subject' AND object_id = OBJECT_ID(N'SiteReview'))
BEGIN
	ALTER TABLE [SiteReview] ADD [Subject] nvarchar(100)
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'IsApproved' AND object_id = OBJECT_ID(N'ItemReview'))
BEGIN
	ALTER TABLE [ItemReview] ADD [IsApproved] BIT DEFAULT(0)
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'IsApproved' AND object_id = OBJECT_ID(N'SiteReview'))
BEGIN
	ALTER TABLE [SiteReview] ADD [IsApproved] BIT DEFAULT(0)
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'PreBookingDays' AND object_id = OBJECT_ID(N'TeeSheet'))
BEGIN
	ALTER TABLE [TeeSheet] ADD [PreBookingDays] INT DEFAULT(0)
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'ReserveDate' AND object_id = OBJECT_ID(N'OrderItem'))
BEGIN
	ALTER TABLE [OrderItem] ADD [ReserveDate] DATETIME NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'VatId' AND object_id = OBJECT_ID(N'OrderItem'))
BEGIN
	ALTER TABLE [OrderItem] ADD [VatId] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'UsagePeriodType' AND object_id = OBJECT_ID(N'CouponGroup'))
BEGIN
	ALTER TABLE [CouponGroup] ADD [UsagePeriodType] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'TimesToUse' AND object_id = OBJECT_ID(N'CouponGroup'))
BEGIN
	ALTER TABLE [CouponGroup] ADD [TimesToUse] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'ItemCouponId' AND object_id = OBJECT_ID(N'OrderItem'))
BEGIN
	ALTER TABLE [OrderItem] ADD [ItemCouponId] BIGINT NULL
END
GO


/*************** CouponGroupItemCategory ***************************/
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CouponGroupItemCategory') AND type in (N'U'))
	DROP TABLE [CouponGroupItemCategory]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CouponGroupItemCategory') AND type in (N'U'))
BEGIN
	CREATE TABLE [CouponGroupItemCategory](
		[CouponGroupId] [int] NOT NULL,
		[CategoryId] [int] NOT NULL,
	 CONSTRAINT [PK_CouponGroupCategory] PRIMARY KEY CLUSTERED 
	(
		[CouponGroupId] ASC,
		[CategoryId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'VatRate' AND object_id = OBJECT_ID(N'OrderItem'))
BEGIN
	ALTER TABLE [OrderItem] ADD [VatRate] FLOAT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'ItemCouponId' AND object_id = OBJECT_ID(N'OrderItem'))
BEGIN
	ALTER TABLE [OrderItem] ADD [ItemCouponId] BIGINT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'ReductionRate' AND object_id = OBJECT_ID(N'OrderItem'))
BEGIN
	ALTER TABLE [OrderItem] ADD [ReductionRate] MONEY NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'ReductionType' AND object_id = OBJECT_ID(N'OrderItem'))
BEGIN
	ALTER TABLE [OrderItem] ADD [ReductionType] INT NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'DiscountPrice' AND object_id = OBJECT_ID(N'OrderItem'))
BEGIN
	ALTER TABLE [OrderItem] ADD [DiscountPrice] MONEY NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'ReductionRate' AND object_id = OBJECT_ID(N'Order'))
BEGIN
	ALTER TABLE [Order] ADD [ReductionRate] MONEY NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'ReductionType' AND object_id = OBJECT_ID(N'Order'))
BEGIN
	ALTER TABLE [Order] ADD [ReductionType] INT NULL
END
GO

IF EXISTS(SELECT * FROM sys.columns WHERE name = N'DiscountPrice' AND object_id = OBJECT_ID(N'OrderItem'))
BEGIN
	ALTER TABLE [OrderItem] DROP COLUMN [DiscountPrice]
END
GO

/*************** CouponGroupItemType ***************************/
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CouponGroupItemType') AND type in (N'U'))
	DROP TABLE [CouponGroupItemType]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CouponGroupItemType') AND type in (N'U'))
BEGIN
	CREATE TABLE [CouponGroupItemType](
		[CouponGroupId] [int] NOT NULL,
		[ItemTypeId] [int] NOT NULL,
	 CONSTRAINT [PK_CouponGroupItemType] PRIMARY KEY CLUSTERED 
	(
		[CouponGroupId] ASC,
		[ItemTypeId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE name = N'MinimumAmount' AND object_id = OBJECT_ID(N'CouponGroup'))
BEGIN
	ALTER TABLE [CouponGroup] ADD [MinimumAmount] MONEY NULL
END
GO