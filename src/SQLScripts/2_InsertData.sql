TRUNCATE TABLE CustomerType
INSERT INTO CustomerType(CustomerTypeId, ParentId) VALUES(1, NULL)
INSERT INTO CustomerType(CustomerTypeId, ParentId) VALUES(2, NULL)
INSERT INTO CustomerType(CustomerTypeId, ParentId) VALUES(3, NULL)

TRUNCATE TABLE Adset
INSERT INTO Adset(AdsetId, AdsetName, Active) VALUES(1, N'Default', 1)
INSERT INTO Adset(AdsetId, AdsetName, Active) VALUES(2, N'Member Area', 1)
INSERT INTO Adset(AdsetId, AdsetName, Active) VALUES(3, N'Offer Detail Page', 1)
INSERT INTO Adset(AdsetId, AdsetName, Active) VALUES(4, N'DLG Shop List Page', 1)
INSERT INTO Adset(AdsetId, AdsetName, Active) VALUES(5, N'DLG Shop Detail Page', 1)

TRUNCATE TABLE Ads
INSERT INTO Ads(AdsetId, AdsName, LinkUrl, ImageUrl, Active) VALUES(1, N'Default1', N'//dlg.dev/DLGShopRegister', N'//dlg.dev/Assets/Front/img/banners/banner-event.jpg', 1)

TRUNCATE TABLE EmailTemplate
SET IDENTITY_INSERT EmailTemplate ON;
INSERT INTO EmailTemplate([TemplateId], [TemplateKey], [Active], [InsertDate], [UpdateDate])
VALUES(1, N'newsletter-welcome', 1, GETDATE(), GETDATE());
INSERT INTO EmailTemplate([TemplateId], [TemplateKey], [Active], [InsertDate], [UpdateDate])
VALUES(2, N'sponsorship', 1, GETDATE(), GETDATE());
INSERT INTO EmailTemplate([TemplateId], [TemplateKey], [Active], [InsertDate], [UpdateDate])
VALUES(3, N'order-confirmation', 1, GETDATE(), GETDATE());
SET IDENTITY_INSERT EmailTemplate OFF;

TRUNCATE TABLE EmailTemplateLang
INSERT INTO EmailTemplateLang([TemplateId], [LangId], [Name], [Description], [Subject])
VALUES(1, 1, N'Newsletter Welcome Email', N'Newsletter Welcome Email', N'Welcome to Dans les Golfs');
INSERT INTO EmailTemplateLang([TemplateId], [LangId], [Name], [Description], [Subject])
VALUES(2, 1, N'Sponsorship', N'Sponsorship', N'Sponsorship');
INSERT INTO EmailTemplateLang([TemplateId], [LangId], [Name], [Description], [Subject])
VALUES(3, 1, N'Order Confirmation', N'Order Confirmation', N'Confirmation de votre paiement par {!paymenttype}');

TRUNCATE TABLE EmailTemplateVariable
INSERT INTO EmailTemplateVariable([VariableName], [Description])
VALUES(N'name', N'Subscriber''s full name');
INSERT INTO EmailTemplateVariable([VariableName], [Description])
VALUES(N'firstname', N'Subscriber''s first name');
INSERT INTO EmailTemplateVariable([VariableName], [Description])
VALUES(N'lastname', N'Subscriber''s last name');
INSERT INTO EmailTemplateVariable([VariableName], [Description])
VALUES(N'email', N'Subscriber''s Email');
INSERT INTO EmailTemplateVariable([VariableName], [Description])
VALUES(N'order_number', N'Order''s Reference Number');
INSERT INTO EmailTemplateVariable([VariableName], [Description])
VALUES(N'order_subtotal', N'Order''s Subtotal');
INSERT INTO EmailTemplateVariable([VariableName], [Description])
VALUES(N'order_date', N'Order date');
INSERT INTO EmailTemplateVariable([VariableName], [Description])
VALUES(N'order_table', N'Order''s Detail Table');

TRUNCATE TABLE [dbo].[Menu]
GO
SET IDENTITY_INSERT [dbo].[Menu] ON 
GO
INSERT [dbo].[Menu] ([MenuId], [MenuName], [MenuPlacement], [Active]) VALUES (1, N'Main Menu', N'main-menu', 1)
GO
SET IDENTITY_INSERT [dbo].[Menu] OFF
GO
TRUNCATE TABLE [dbo].[MenuItem]
GO
SET IDENTITY_INSERT [dbo].[MenuItem] ON 
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (1, 1, 0, N'itemtype', N'Green Fee', N'2')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (2, 1, 0, N'itemtype', N'Séjour', N'3')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (3, 1, 0, N'itemtype', N'Stage', N'4')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (4, 1, 0, N'itemtype', N'Cartes De Practice', N'5')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (5, 1, 0, N'link', N'DLG Shop', N'/DLGShop')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (6, 1, 0, N'link', N'DLG Card', N'/DLGCard')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (7, 1, 0, N'link', N'Trouver Un Golf', N'/FindCourse')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (8, 1, 1, N'link', N'Programme Classic', N'/Item/programme-classic-leclub-golf')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (9, 1, 1, N'link', N'Pass DLG Suprême', N'/Item/pass-dlg-supreme')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (10, 1, 1, N'link', N'Pack Suprême Classic', N'/Item/pack-supreme-classic1')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (11, 1, 1, N'category', N'Packages', N'10')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (12, 1, 1, N'category', N'Green Fee', N'47')
GO
INSERT [dbo].[MenuItem] ([MenuItemId], [MenuId], [ParentId], [MenuType], [MenuTitle], [MenuValue]) VALUES (13, 1, 1, N'link', N'Carte Cadeau NGFGolf', N'/Item/carte-cadeau-ngfgolf')
GO
SET IDENTITY_INSERT [dbo].[MenuItem] OFF
GO

IF EXISTS(SELECT ContentId FROM WebContent WHERE [ContentCategory] = 'PAYMENT' AND [ContentKey] = 'payment-lydia-waitingmessage')
BEGIN
	DELETE b FROM WebContentLang b JOIN WebContent a ON a.ContentId = b.ContentId WHERE a.[ContentCategory] = 'PAYMENT' AND a.[ContentKey] = 'payment-lydia-waitingmessage'
	DELETE FROM WebContent WHERE [ContentCategory] = 'PAYMENT' AND [ContentKey] = 'payment-lydia-waitingmessage'
END
GO

INSERT INTO [WebContent]([ContentCategory], [ContentKey], [Active], [UserId], [InsertDate], [OrderNumber]) VALUES ('PAYMENT', 'payment-lydia-waitingmessage', 1, 1, GETDATE(), 0)
GO
INSERT INTO [WebContentLang] ([ContentId],[LangId],[TopicName],[ContentText],[UpdateDate],[UserId]) VALUES ((SELECT TOP(1) ContentId FROM WebContent WHERE [ContentCategory] = 'PAYMENT' AND [ContentKey] = 'payment-lydia-waitingmessage'),1,N'Votre paiement est en cours d''enregistrement, veuillez patienter.',N'Votre paiement est en cours d''enregistrement, veuillez patienter.',GETDATE(),1)
GO
INSERT INTO [WebContentLang] ([ContentId],[LangId],[TopicName],[ContentText],[UpdateDate],[UserId]) VALUES ((SELECT TOP(1) ContentId FROM WebContent WHERE [ContentCategory] = 'PAYMENT' AND [ContentKey] = 'payment-lydia-waitingmessage'),2,N'Your payment is being process, please wait.',N'Your payment is being process, please wait.',GETDATE(),1)
GO