PRINT 'Updating [TrainingProviders]'

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'TrainingProgramsTemp'))
BEGIN
	-- Moving the relationship from TrainingProgram.TrainingProviderId to TrainingProvider.TrainingProgramId.
	UPDATE [dbo].[TrainingProviders] SET
		[TrainingProgramId] = tg.[Id]
	FROM [dbo].[TrainingProviders] tp 
		INNER JOIN [dbo].[TrainingProgramsTemp] tg ON tp.[Id] = tg.[TrainingProviderId]

	-- Moving the relationship from TrainingProgram.RequestedTrainingProviderId to TrainingProvider.TrainingProgramId.
	UPDATE [dbo].[TrainingProviders] SET
		[TrainingProgramId] = tg.[Id]
	FROM [dbo].[TrainingProviders] tp 
		INNER JOIN [dbo].[TrainingProgramsTemp] tg ON tp.[Id] = tg.[RequestedTrainingProviderId]

	-- Remove temporary table used to store data.
	DROP TABLE [dbo].[TrainingProgramsTemp]
END