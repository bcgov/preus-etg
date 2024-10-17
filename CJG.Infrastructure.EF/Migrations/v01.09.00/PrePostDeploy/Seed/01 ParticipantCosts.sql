PRINT 'Migrate [ParticipantCosts]'

UPDATE pc
SET
	pc.ParticipantFormId = bpc.ParticipantFormId
FROM dbo.ParticipantCosts pc
	INNER JOIN #ParticipantCosts bpc ON pc.ParticipantEnrollmentId = bpc.ParticipantEnrollmentId

DROP TABLE #ParticipantCosts