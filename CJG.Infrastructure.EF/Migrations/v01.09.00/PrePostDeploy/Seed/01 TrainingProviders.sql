PRINT 'Migrate [TrainingProviders]'

-- Link the training provider to the training program.
INSERT INTO dbo.TrainingProgramTrainingProviders (
	TrainingProgramId
	, TrainingProviderId
)
SELECT DISTINCT
	CASE WHEN btp.TrainingProgramId IS NULL THEN tp.Id ELSE btp.TrainingProgramId END
	, btp.Id
FROM #TrainingProviders btp
LEFT JOIN dbo.GrantApplications ga ON btp.GrantApplicationId = ga.Id
LEFT JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
WHERE btp.TrainingProgramId IS NOT NULL 
	OR tp.Id IS NOT NULL

-- If the training provider isn't linked to a training program then link it to the grant application (it's one or the other, not both).
UPDATE tp
SET
	tp.GrantApplicationId = CASE WHEN btp.TrainingProgramId IS NULL THEN btp.GrantApplicationId ELSE NULL END
	, tp.TrainingProviderState = CASE WHEN btp.TrainingProviderState = 0 THEN 0 ELSE btp.TrainingProviderState - 1 END
FROM dbo.TrainingProviders tp
	INNER JOIN #TrainingProviders btp ON tp.Id = btp.Id

DROP TABLE #TrainingProviders