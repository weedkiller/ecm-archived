CREATE TABLE [dbo].[ReservationAPI](
	[APIId] [int] NOT NULL,
	[APICode] [nvarchar](50) NOT NULL,
	[APIName] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[APIId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [ReservationAPI]([APIId], [APICode], [APIName])
VALUES(0, 'DLG', 'Dans Les Golfs')
GO

INSERT INTO [ReservationAPI]([APIId], [APICode], [APIName])
VALUES(1, 'ALBATROS', 'Albatros')
GO

INSERT INTO [ReservationAPI]([APIId], [APICode], [APIName])
VALUES(2, 'PRIMA', 'Prima Golfs')
GO

ALTER TABLE [Site] ADD [ReservationAPI] INT NULL
GO

ALTER TABLE [Site] ADD [PrimaBookerId] NVARCHAR(50) NULL
GO

ALTER TABLE [Site] ADD [PrimaServerUrl] NVARCHAR(255) NULL
GO

UPDATE [Site] SET ReservationAPI = 0 WHERE ISNULL(AlbatrosCourseId, 0) = 0
GO

UPDATE [Site] SET ReservationAPI = 1 WHERE ISNULL(AlbatrosCourseId, 0) > 0
GO