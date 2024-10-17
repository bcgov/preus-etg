PRINT 'Updating [GrantApplications]'

-- If grant application submit dates are invalid and they have been submitted, it is impossible to remove them due to validation.
-- Update the submitted dates.

UPDATE tp
SET tp.[StartDate] = ga.[DateSubmitted]
FROM [dbo].[TrainingPrograms] tp
	INNER JOIN [dbo].[GrantApplications] ga ON tp.[GrantApplicationId] = ga.[Id] 
		AND ga.[DateSubmitted] IS NOT NULL
		AND ga.[DateSubmitted] > tp.[StartDate]