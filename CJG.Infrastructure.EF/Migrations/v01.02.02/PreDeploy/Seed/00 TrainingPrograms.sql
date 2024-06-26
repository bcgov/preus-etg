PRINT 'Preparing [TrainingProgramsTemp]'

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'TrainingPrograms'))
BEGIN
	CREATE TABLE [dbo].[TrainingProgramsTemp] (
		[Id] INT NOT NULL,
		[TrainingProviderId] INT,
		[RequestedTrainingProviderId] INT,
		CONSTRAINT PK_TrainingProgramsTemp PRIMARY KEY ([Id])
	)

	INSERT INTO [dbo].[TrainingProgramsTemp] ( [Id], [TrainingProviderId], [RequestedTrainingProviderId] )
	SELECT [Id], [TrainingProviderId], [RequestedTrainingProviderId]
	FROM [dbo].[TrainingPrograms]
END