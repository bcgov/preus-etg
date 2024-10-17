PRINT 'UPDATE [GrantApplications] - Set Completion Report Id'

Update [GrantApplications]
Set CompletionReportId = (Select TOP 1 [Id] from [CompletionReports])
Where CompletionReportId = 0

CHECKPOINT