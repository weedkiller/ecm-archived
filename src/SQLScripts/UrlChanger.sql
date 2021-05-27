DECLARE @OldName NVARCHAR(255), @NewName NVARCHAR(255)
SET @OldName = N'vps185045.ovh.net'
SET @NewName = N'www.danslesgolfs.com'

SELECT TABLE_NAME, COLUMN_NAME ,
'UPDATE ' + TABLE_NAME + ' SET ' + COLUMN_NAME + ' = REPLACE(' + COLUMN_NAME + ', N''' + @OldName + ''', N''' + @NewName + ''');' AS Commands
FROM INFORMATION_SCHEMA.COLUMNS
WHERE COLUMN_NAME LIKE '%Url%'
ORDER BY TABLE_NAME, COLUMN_NAME

UPDATE EmailTemplateLang SET Name = REPLACE(Name, N'vps185045.ovh.net', N'www.danslesgolfs.com');
UPDATE EmailTemplateLang SET [Description] = REPLACE([Description], N'vps185045.ovh.net', N'www.danslesgolfs.com');
UPDATE EmailTemplateLang SET HtmlDetail = CAST(REPLACE(CAST(HtmlDetail AS NVARCHAR(MAX)), N'vps185045.ovh.net', N'www.danslesgolfs.com') AS NTEXT);
UPDATE EmailTemplateLang SET TextDetail = CAST(REPLACE(CAST(TextDetail AS NVARCHAR(MAX)), N'vps185045.ovh.net', N'www.danslesgolfs.com') AS NTEXT);

UPDATE WebContentLang SET ContentText = CAST(REPLACE(CAST(ContentText AS NVARCHAR(MAX)), N'vps185045.ovh.net', N'www.danslesgolfs.com') AS NTEXT);