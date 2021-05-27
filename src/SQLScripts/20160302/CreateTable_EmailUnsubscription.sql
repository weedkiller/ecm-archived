USE [ECM]
GO

/****** Object:  Table [dbo].[EmailUnsubscription]    Script Date: 2/3/2559 14:07:15 ******/
DROP TABLE [dbo].[EmailUnsubscription]
GO

/****** Object:  Table [dbo].[EmailUnsubscription]    Script Date: 2/3/2559 14:07:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmailUnsubscription](
	[EmailUnsubscriptionId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NULL,
	[UnsubscribeReasonId] [bigint] NULL,
	[EmailTrackingId] [bigint] NULL,
	[OtherReason] [ntext] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_EmailUnsubscription] PRIMARY KEY CLUSTERED 
(
	[EmailUnsubscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


