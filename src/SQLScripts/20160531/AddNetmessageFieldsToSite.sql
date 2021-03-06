IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'SendMailUsing')
BEGIN
	ALTER TABLE [Site] ADD [SendMailUsing] INT NUll
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'IsUseGlobalNetmessageSettings')
BEGIN
	ALTER TABLE [Site] ADD [IsUseGlobalNetmessageSettings] BIT NUll
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'NetmessageFTPUsername')
BEGIN
	ALTER TABLE [Site] ADD [NetmessageFTPUsername] NVARCHAR(255) NUll
END

IF NOT EXISTS(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Site' AND COLUMN_NAME = 'NetmessageFTPPassword')
BEGIN
	ALTER TABLE [Site] ADD [NetmessageFTPPassword] NVARCHAR(255) NUll
END