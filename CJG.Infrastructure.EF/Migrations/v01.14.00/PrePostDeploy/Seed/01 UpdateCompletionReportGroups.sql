PRINT 'UPDATE [CompletionReportGroups] - Set Completion Report Id'

Update [CompletionReportGroups]
Set CompletionReportId = (Select TOP 1 [Id] from [CompletionReports])
Where CompletionReportId = 0


CHECKPOINT