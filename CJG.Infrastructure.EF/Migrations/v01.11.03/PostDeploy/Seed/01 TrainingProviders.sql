PRINT 'Update [TrainingProviders] - Fix OriginalTrainingProviderId'

-- Update all change request to originate from their parent.
UPDATE tp
SET
	tp.OriginalTrainingProviderId = cr.OriginalTrainingProviderId
FROM dbo.TrainingProviders tp
JOIN (
	SELECT
		tp.Id AS TrainingProviderId
		, crs.TrainingProviderId AS OriginalTrainingProviderId
	FROM (
		SELECT
			TrainingProgramId
			, COUNT(*) Providers
			, MIN(TrainingProviderId) AS TrainingProviderId
		FROM dbo.TrainingProgramTrainingProviders
		GROUP BY TrainingProgramId
		HAVING COUNT(*) > 1) AS crs
	JOIN dbo.TrainingProgramTrainingProviders tptp ON crs.TrainingProgramId = tptp.TrainingProgramId
	JOIN dbo.TrainingProviders tp ON tptp.TrainingProviderId = tp.Id AND crs.TrainingProviderId != tp.Id
	WHERE tp.OriginalTrainingProviderId IS NULL) AS cr ON tp.Id = cr.TrainingProviderId