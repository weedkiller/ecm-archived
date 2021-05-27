/****** Object:  Table [dbo].[EmailTemplateVariable]    Script Date: 2/22/2016 8:13:04 AM ******/
DROP TABLE [dbo].[EmailTemplateVariable]
GO
/****** Object:  Table [dbo].[EmailTemplateVariable]    Script Date: 2/22/2016 8:13:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailTemplateVariable](
	[VariableId] [int] IDENTITY(1,1) NOT NULL,
	[VariableName] [nvarchar](50) NULL,
	[Description] [nvarchar](500) NULL,
	[ListNo] [int] NULL,
 CONSTRAINT [PK_EmailTemplateVariable] PRIMARY KEY CLUSTERED 
(
	[VariableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[EmailTemplateVariable] ON 

GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (1, N'name', N'Subscriber''s full name', 1)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (2, N'firstname', N'Subscriber''s first name', 2)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (3, N'lastname', N'Subscriber''s last name', 3)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (4, N'email', N'Subscriber''s Email', 4)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (5, N'gender', N'Subscriber''s gender', 5)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (6, N'phone', N'Subscriber''s phone', 6)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (7, N'mobile', N'Subscriber''s mobile', 7)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (8, N'description', N'Subscriber''s description', 8)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (9, N'profession', N'Subscriber''s career', 9)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (10, N'index', N'Subscriber''s index level', 10)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (11, N'field1', N'Subscriber''s field 1', 11)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (12, N'field2', N'Subscriber''s field 2', 12)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (13, N'field3', N'Subscriber''s field 3', 13)
GO
INSERT [dbo].[EmailTemplateVariable] ([VariableId], [VariableName], [Description], [ListNo]) VALUES (14, N'unsubscribe', N'Unsubscribe Link', 14)
GO
SET IDENTITY_INSERT [dbo].[EmailTemplateVariable] OFF
GO
