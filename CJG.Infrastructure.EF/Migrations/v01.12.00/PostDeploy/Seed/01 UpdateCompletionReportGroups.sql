PRINT 'Update Completion Report Groups'

Update [CompletionReportGroups]
Set RowSequence = Id
Where Id < 3

Update [CompletionReportGroups]
Set RowSequence = Id + 1
Where Id > 2

INSERT INTO [CompletionReportGroups]
           ([Title]
           ,[DateAdded]
           ,[DateUpdated])
     VALUES
           ('Participant Services Received'
           ,GETUTCDATE()
           ,GETUTCDATE())
GO

Update [CompletionReportGroups]
Set ProgramTypeId = (Select TOP 1 [ProgramTypeId] from [GrantPrograms] Where [Name] = 'Community Workforce Response Grant'), RowSequence = 3
Where Id = 5 And ProgramTypeId is NULL
