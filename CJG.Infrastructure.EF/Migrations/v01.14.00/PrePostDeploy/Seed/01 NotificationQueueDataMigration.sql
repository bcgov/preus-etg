PRINT 'INSERT into [NotificationQueue] - Moving data from Notification and NotificationScheduleQueue into NotificationQueue'

-- Moving data from Notification and NotificationScheduleQueue into NotificationQueue
INSERT INTO dbo.NotificationQueue ([NotificationTypeId], [GrantApplicationId], [OrganizationId], [BatchNumber], [EmailSubject], [EmailBody], [EmailRecipients], [EmailSender], [State], [ErrorMessage], [SendDate], [DateAdded], [DateUpdated])
SELECT nsq.NotificationTypeId, nsq.GrantApplicationId, ga.OrganizationId, 'V1.13', n.EmailSubject, n.EmailBody, n.EmailRecipients, n.EmailSender, 1, '', n.EmailSentDate, n.[DateAdded], n.[DateUpdated]
FROM dbo.Notifications as n
JOIN dbo.NotificationScheduleQueue as nsq on n.NotificationScheduleQueueId = nsq.Id
JOIN dbo.GrantApplications as ga on nsq.GrantApplicationId = ga.Id

CHECKPOINT