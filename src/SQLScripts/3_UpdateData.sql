GO
PRINT N'Altering [dbo].[WebContent]...';


GO
IF NOT EXISTS(	SELECT TOP 1 1 
				from INFORMATION_SCHEMA.COLUMNS 
				WHERE Table_Schema='dbo' AND Table_Name = 'WebContent' AND COLUMN_NAME = 'OrderNumber' )
BEGIN
ALTER TABLE [dbo].[WebContent]
    ADD [OrderNumber] INT NULL;
END

GO
PRINT N'Update complete.';


UPDATE [dbo].[WebContent] SET OrderNumber = 1 WHERE ContentId = 16 AND ContentCategory = 'FOOTER' 
UPDATE [dbo].[WebContent] SET OrderNumber = 2 WHERE ContentId = 5 AND ContentCategory = 'FOOTER' 
UPDATE [dbo].[WebContent] SET OrderNumber = 3 WHERE ContentId = 2 AND ContentCategory = 'FOOTER' 
UPDATE [dbo].[WebContent] SET OrderNumber = 4 WHERE ContentId = 7 AND ContentCategory = 'FOOTER' 
UPDATE [dbo].[WebContent] SET OrderNumber = 5 WHERE ContentId = 3 AND ContentCategory = 'FOOTER' 
UPDATE [dbo].[WebContent] SET OrderNumber = 6 WHERE ContentId = 6 AND ContentCategory = 'FOOTER' 
UPDATE [dbo].[WebContent] SET OrderNumber = 7 WHERE ContentId = 1 AND ContentCategory = 'FOOTER' 
UPDATE [dbo].[WebContent] SET OrderNumber = 8 WHERE ContentId = 4 AND ContentCategory = 'FOOTER' 

UPDATE [dbo].[WebContentLang] SET TopicName = 'The SAV' WHERE ContentId = 5 AND LangId = 1
UPDATE [dbo].[WebContentLang] SET TopicName = 'Le SAV' WHERE ContentId = 5 AND LangId = 2

IF EXISTS(SELECT ModifierId FROM Modifier WHERE ModifierName  = N'Level')
BEGIN
	UPDATE [Modifier] SET [ModifierName] = N'Level', [ModifierDesc] = N'Level', [UpdateDate] = CAST(0x0000A4150164B7F0 AS DateTime), [ControlType] = 0, [Active] = 1, [UserId] = 1, [ItemTypeId] = 1
END
ELSE
BEGIN
	INSERT [Modifier] ([ModifierName], [ModifierDesc], [UpdateDate], [ControlType], [Active], [UserId], [ItemTypeId]) VALUES (N'Level', N'Level', CAST(0x0000A4150164B7F0 AS DateTime), 0, 1, 1, 1)
END

UPDATE ItemCategory SET Prefix = N'PC' WHERE CategoryName = N'Programme Classic'
UPDATE ItemCategory SET Prefix = N'PS' WHERE CategoryName = N'Pass DLG Suprême'
UPDATE ItemCategory SET Prefix = N'PSC' WHERE CategoryName = N'Pack Suprême Classic'
UPDATE ItemCategory SET Prefix = N'PK' WHERE CategoryName = N'Packages'

TRUNCATE TABLE ModifierLang
INSERT [dbo].[ModifierLang] ([ModifierId], [LangId], [ModifierName], [ModifierDesc]) VALUES (1, 1, N'Dexterity', N'Dexterity')
INSERT [dbo].[ModifierLang] ([ModifierId], [LangId], [ModifierName], [ModifierDesc]) VALUES (1, 2, N'Dextérité', N'Dextérité')
INSERT [dbo].[ModifierLang] ([ModifierId], [LangId], [ModifierName], [ModifierDesc]) VALUES (2, 1, N'Genre', N'Genre')
INSERT [dbo].[ModifierLang] ([ModifierId], [LangId], [ModifierName], [ModifierDesc]) VALUES (2, 2, N'Genre', N'Genre')
INSERT [dbo].[ModifierLang] ([ModifierId], [LangId], [ModifierName], [ModifierDesc]) VALUES (3, 1, N'Shaft', N'Shaft')
INSERT [dbo].[ModifierLang] ([ModifierId], [LangId], [ModifierName], [ModifierDesc]) VALUES (3, 2, N'Shaft', N'Shaft')
INSERT [dbo].[ModifierLang] ([ModifierId], [LangId], [ModifierName], [ModifierDesc]) VALUES (4, 1, N'Shape', N'Shape')
INSERT [dbo].[ModifierLang] ([ModifierId], [LangId], [ModifierName], [ModifierDesc]) VALUES (4, 2, N'Shape', N'Shape')
INSERT [dbo].[ModifierLang] ([ModifierId], [LangId], [ModifierName], [ModifierDesc]) VALUES (5, 1, N'Level', N'Level')
INSERT [dbo].[ModifierLang] ([ModifierId], [LangId], [ModifierName], [ModifierDesc]) VALUES (5, 2, N'Niveau', N'Niveau')

TRUNCATE TABLE ModifierChoiceLang
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (1, 1, N'Left Hand')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (1, 2, N'Left Hand')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (2, 1, N'Right Hand')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (2, 2, N'Right Hand')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (3, 1, N'Man')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (3, 2, N'Homme')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (4, 1, N'Woman')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (4, 2, N'Femme')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (5, 1, N'Junior')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (5, 2, N'Junior')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (6, 1, N'Steel')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (6, 2, N'Steel')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (7, 1, N'Aluminum')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (7, 2, N'Aluminum')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (8, 1, N'Composite')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (8, 2, N'Composite')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (9, 1, N'Graphite')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (9, 2, N'Graphite')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (10, 1, N'Titanium')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (10, 2, N'Titanium')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (11, 1, N'Wood')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (11, 2, N'Wood')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (12, 1, N'New')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (12, 2, N'Neuf')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (13, 1, N'Used')
INSERT [dbo].[ModifierChoiceLang] ([ChoiceId], [LangId], [ChoiceName]) VALUES (13, 2, N'Occasion')

/****** Object:  Table [dbo].[TitleLang]    Script Date: 02/06/2015 16:05:46 ******/
INSERT [dbo].[TitleLang] ([TitleId], [LangId], [TitleName]) VALUES (1, 1, N'Mr.')
INSERT [dbo].[TitleLang] ([TitleId], [LangId], [TitleName]) VALUES (2, 1, N'Ms.')
INSERT [dbo].[TitleLang] ([TitleId], [LangId], [TitleName]) VALUES (3, 1, N'Mrs.')
INSERT [dbo].[TitleLang] ([TitleId], [LangId], [TitleName]) VALUES (1, 2, N'M.')
INSERT [dbo].[TitleLang] ([TitleId], [LangId], [TitleName]) VALUES (2, 2, N'Frau')
INSERT [dbo].[TitleLang] ([TitleId], [LangId], [TitleName]) VALUES (3, 2, N'Firma')
/****** Object:  Table [dbo].[Title]    Script Date: 02/06/2015 16:05:46 ******/
SET IDENTITY_INSERT [dbo].[Title] ON
INSERT [dbo].[Title] ([TitleId], [InsertDate], [Active]) VALUES (1, CAST(0x0000A2D200FF0111 AS DateTime), 1)
INSERT [dbo].[Title] ([TitleId], [InsertDate], [Active]) VALUES (2, CAST(0x0000A2D200FF0111 AS DateTime), 1)
INSERT [dbo].[Title] ([TitleId], [InsertDate], [Active]) VALUES (3, CAST(0x0000A2D200FF0111 AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[Title] OFF

-- Reset Ability to show calendar for items
UPDATE [dbo].[Item] SET IsUserCanSelectDate = 1 WHERE IsUserCanSelectDate IS NULL


-- [dbo].[Creditcardassociations]
INSERT INTO [dbo].[Creditcardassociations]([CardassociationsId],[CardassociationsType],[Active],[CreateDate])VALUES(1,'American Express',1,GETUTCDATE())
INSERT INTO [dbo].[Creditcardassociations]([CardassociationsId],[CardassociationsType],[Active],[CreateDate])VALUES(2,'China UnionPay',1,GETUTCDATE())
INSERT INTO [dbo].[Creditcardassociations]([CardassociationsId],[CardassociationsType],[Active],[CreateDate])VALUES(3,'Diners Club',1,GETUTCDATE())
INSERT INTO [dbo].[Creditcardassociations]([CardassociationsId],[CardassociationsType],[Active],[CreateDate])VALUES(4,'Discover Card',1,GETUTCDATE())
INSERT INTO [dbo].[Creditcardassociations]([CardassociationsId],[CardassociationsType],[Active],[CreateDate])VALUES(5,'Entrust Bankcard',1,GETUTCDATE())
INSERT INTO [dbo].[Creditcardassociations]([CardassociationsId],[CardassociationsType],[Active],[CreateDate])VALUES(6,'Japan Credit Bureau',1,GETUTCDATE())
INSERT INTO [dbo].[Creditcardassociations]([CardassociationsId],[CardassociationsType],[Active],[CreateDate])VALUES(7,'MasterCard',1,GETUTCDATE())
INSERT INTO [dbo].[Creditcardassociations]([CardassociationsId],[CardassociationsType],[Active],[CreateDate])VALUES(8,'Visa',1,GETUTCDATE())

-- Fixed wrong name of city
UPDATE City SET CityName = N'La Rochelle' WHERE CityName LIKE N'%Rochelle%' AND PostalCode = '17000'

-- Clear 
DELETE FROM TeeSheet WHERE Price = 0