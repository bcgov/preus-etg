PRINT 'ALTER [NotificationTypes] - Settings columns to "Not Null"'

ALTER TABLE dbo.NotificationTypes
ALTER COLUMN NotificationTemplateId INTEGER NOT NULL

ALTER TABLE dbo.NotificationTypes
ALTER COLUMN NotificationTriggerId INTEGER NOT NULL

ALTER TABLE dbo.NotificationTypes
ALTER COLUMN ResendRule INTEGER NOT NULL

ALTER TABLE dbo.NotificationTypes			
ALTER COLUMN RecipientRule INTEGER NOT NULL

ALTER TABLE dbo.NotificationTypes
ALTER COLUMN ApprovalRule INTEGER NOT NULL

ALTER TABLE dbo.NotificationTypes
ALTER COLUMN ParticipantReportRule INTEGER NOT NULL

ALTER TABLE dbo.NotificationTypes
ALTER COLUMN ClaimReportRule INTEGER NOT NULL

ALTER TABLE dbo.NotificationTypes
ALTER COLUMN CompletionReportRule INTEGER NOT NULL

ALTER TABLE dbo.NotificationTypes
ALTER COLUMN RecipientRule INTEGER NOT NULL

CHECKPOINT