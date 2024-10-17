PRINT 'Insert Settings'

INSERT INTO [dbo].[Settings]
           ([Key]
           ,[Value]
           ,[ValueType]
           ,[DateAdded])
     VALUES
           ('CheckPrivateSectorsOn'
           ,'2019/04/03 12:00:00AM'
           ,'System.DateTime'
           ,GETUTCDATE())
GO
