IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'IsCommercial')
BEGIN
	ALTER TABLE [Site] ADD [IsCommercial] BIT NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'IsAssociative')
BEGIN
	ALTER TABLE [Site] ADD [IsAssociative] BIT NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'IsRegie')
BEGIN
	ALTER TABLE [Site] ADD [IsRegie] BIT NULL
END

-------------------------------- Management --------------------------------
IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'ManageRestaurantFlag')
BEGIN
	ALTER TABLE [Site] ADD [ManageRestaurantFlag] NVARCHAR(1) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'ManageProshopFlag')
BEGIN
	ALTER TABLE [Site] ADD [ManageProshopFlag] NVARCHAR(1) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'ManageFieldFlag')
BEGIN
	ALTER TABLE [Site] ADD [ManageFieldFlag] NVARCHAR(1) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'ManageGolfFlag')
BEGIN
	ALTER TABLE [Site] ADD [ManageGolfFlag] NVARCHAR(1) NULL
END

-------------------------------- Database --------------------------------
IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'ManagerPhone')
BEGIN
	ALTER TABLE [Site] ADD [ManagerPhone] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'RespReceptionPhone')
BEGIN
	ALTER TABLE [Site] ADD [RespReceptionPhone] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'RespReceptionPhone')
BEGIN
	ALTER TABLE [Site] ADD [RespReceptionPhone] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'GreenKeeperPhone')
BEGIN
	ALTER TABLE [Site] ADD [GreenKeeperPhone] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'RespProshopPhone')
BEGIN
	ALTER TABLE [Site] ADD [RespProshopPhone] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'RestaurateurPhone')
BEGIN
	ALTER TABLE [Site] ADD [RestaurateurPhone] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'AssociationPresidentPhone')
BEGIN
	ALTER TABLE [Site] ADD [AssociationPresidentPhone] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'ManagerEmail')
BEGIN
	ALTER TABLE [Site] ADD [ManagerEmail] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'RespReceptionEmail')
BEGIN
	ALTER TABLE [Site] ADD [RespReceptionEmail] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'RespReceptionEmail')
BEGIN
	ALTER TABLE [Site] ADD [RespReceptionEmail] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'GreenKeeperEmail')
BEGIN
	ALTER TABLE [Site] ADD [GreenKeeperEmail] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'RespProshopEmail')
BEGIN
	ALTER TABLE [Site] ADD [RespProshopEmail] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'RestaurateurEmail')
BEGIN
	ALTER TABLE [Site] ADD [RestaurateurEmail] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'AssociationPresidentEmail')
BEGIN
	ALTER TABLE [Site] ADD [AssociationPresidentEmail] NVARCHAR(50) NULL
END


-------------------------------- Commercial --------------------------------
IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'CommercialNBSubscriber')
BEGIN
	ALTER TABLE [Site] ADD [CommercialNBSubscriber] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'CommercialNBGF')
BEGIN
	ALTER TABLE [Site] ADD [CommercialNBGF] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'CommercialTurnover')
BEGIN
	ALTER TABLE [Site] ADD [CommercialTurnover] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'CommercialReservationSystem')
BEGIN
	ALTER TABLE [Site] ADD [CommercialReservationSystem] NVARCHAR(50) NULL
END

-------------------------------- Hotel --------------------------------
IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'HotelOnSite')
BEGIN
	ALTER TABLE [Site] ADD [HotelOnSite] NVARCHAR(50) NULL
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'HotelPartner')
BEGIN
	ALTER TABLE [Site] ADD [HotelPartner] NVARCHAR(50) NULL
END