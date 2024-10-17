PRINT 'Backup [NotificationTypes]'

-- Backup the notification types and delete the table so that it can be recreated with an IDENTITY primary key.

CREATE TABLE #NotificationTypes (
	[Id] INT NOT NULL
	, [Caption] NVARCHAR(128) NOT NULL 
	, [Description] NVARCHAR(500)
	, [MilestoneDateName] NVARCHAR(128) NOT NULL 
	, [MilestoneDateOffset] INT NOT NULL
	, [IsActive] BIT NOT NULL
	, [RowSequence] INT NOT NULL
	, [DateAdded] [datetime2](7) NOT NULL
)

-- Backup the notification types
INSERT INTO #NotificationTypes (
	[Id]
	, [Caption]
	, [Description]
	, [MilestoneDateName]
	, [MilestoneDateOffset]
	, [IsActive]
	, [RowSequence]
	, [DateAdded]
) SELECT 
	[Id]
	, [Caption]
	, [Description]
	, [MilestoneDateName]
	, [MilestoneDateOffset]
	, [IsActive]
	, [RowSequence]
	, [DateAdded]
FROM dbo.[NotificationTypes]

-- Remove foreign keys to NotificationTypes
ALTER TABLE dbo.[NotificationScheduleQueue]
	DROP CONSTRAINT [FK_dbo.NotificationScheduleQueue_dbo.NotificationTypes_NotificationTypeId]

ALTER TABLE dbo.[GrantProgramNotifications]
	DROP CONSTRAINT [FK_dbo.GrantProgramNotifications_dbo.NotificationTypes_NotificationTypeId]

-- Drop the notification types table so that it can be recreated with an IDENTITY primary key.
DROP TABLE dbo.[NotificationTypes]

-- Create new NotificationTypes
CREATE TABLE [dbo].[NotificationTypes] (
	[Id] [int] IDENTITY(1,1) NOT NULL
	, [MilestoneDateName] [nvarchar](64) NOT NULL
	, [MilestoneDateOffset] [int] NOT NULL
	, [Caption] [nvarchar](250) NOT NULL
	, [DateAdded] [datetime2](7) NOT NULL
	, [DateUpdated] [datetime2](7) NULL
	, [RowVersion] [timestamp] NOT NULL
	, [Description] [nvarchar](500) NULL
	, [IsActive] [bit] NOT NULL DEFAULT ((0))
	, [RowSequence] [int] NOT NULL DEFAULT ((0))
	CONSTRAINT [PK_dbo.NotificationTypes] PRIMARY KEY CLUSTERED (
		[Id] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

-- Reinsert the data
SET IDENTITY_INSERT [dbo].[NotificationTypes] ON

INSERT INTO dbo.[NotificationTypes] (
	[Id]
	, [Caption]
	, [Description]
	, [MilestoneDateName]
	, [MilestoneDateOffset]
	, [IsActive]
	, [RowSequence]
	, [DateAdded]
) SELECT 
	[Id]
	, [Caption]
	, [Description]
	, [MilestoneDateName]
	, [MilestoneDateOffset]
	, [IsActive]
	, [RowSequence]
	, [DateAdded]
FROM #NotificationTypes

SET IDENTITY_INSERT [dbo].[NotificationTypes] OFF

-- Relink foreign keys
ALTER TABLE dbo.[NotificationScheduleQueue]
	ADD CONSTRAINT [FK_dbo.NotificationScheduleQueue_dbo.NotificationTypes_NotificationTypeId] FOREIGN KEY ([NotificationTypeId]) REFERENCES dbo.[NotificationTypes] ([Id])

ALTER TABLE dbo.[GrantProgramNotifications]
	ADD CONSTRAINT [FK_dbo.GrantProgramNotifications_dbo.NotificationTypes_NotificationTypeId] FOREIGN KEY ([NotificationTypeId]) REFERENCES dbo.[NotificationTypes] ([Id])

-- Remove temporary table.
DROP TABLE #NotificationTypes

CHECKPOINT