PRINT 'Fix Collation'

-- [NotificationTypes]
ALTER TABLE dbo.[NotificationTypes]
ALTER COLUMN [MilestoneDateName] NVARCHAR(64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

ALTER TABLE dbo.[NotificationTypes]
ALTER COLUMN [NotificationTypeName] NVARCHAR(128) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

ALTER TABLE dbo.[NotificationTypes]
ALTER COLUMN [Description] NVARCHAR(500) COLLATE SQL_Latin1_General_CP1_CI_AS
