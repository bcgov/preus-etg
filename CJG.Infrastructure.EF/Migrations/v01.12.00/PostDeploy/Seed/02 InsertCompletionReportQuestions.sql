PRINT 'Insert Completion Report Questions'

INSERT INTO [CompletionReportQuestions]
           ([CompletionReportId]
           ,[Question]
           ,[Description]
           ,[Audience]
           ,[GroupId]
           ,[Sequence]
           ,[IsRequired]
           ,[IsActive]
           ,[QuestionType]
           ,[DefaultText]
           ,[DefaultAnswerId]
           ,[ContinueIfAnswerId]
           ,[StopIfAnswerId]
           ,[AnswerTableHeadings]
           ,[DateAdded]
           ,[DateUpdated])
     VALUES
           (1
           ,'Use the check boxes to enter the services received by each participant and then mark each participant as completed. Scroll to the right to view and enter all services if required.'
           ,NULL
           ,0
           ,5
           ,1
           ,1
           ,1
           ,4
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,GETUTCDATE()
           ,GETUTCDATE())
GO
