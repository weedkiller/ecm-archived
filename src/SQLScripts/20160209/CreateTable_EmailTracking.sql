USE ECM
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'EmailTracking')
	DROP TABLE [dbo].[EmailTracking];

CREATE TABLE [dbo].[EmailTracking](
	[EmailTrackingId] [bigint] IDENTITY(1,1) NOT NULL,
	[CampaignId] [bigint] NOT NULL,
	[EmailQueId] [bigint] NOT NULL,
	[Action] [nvarchar](50) NULL,
	[Value] [nvarchar](500) NULL,
	[IPAddress] [nvarchar](30) NULL,
	[Browser] [nvarchar](200) NULL,
	[Platform] [nvarchar](200) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_EmailTracking] PRIMARY KEY CLUSTERED 
(
	[EmailTrackingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]