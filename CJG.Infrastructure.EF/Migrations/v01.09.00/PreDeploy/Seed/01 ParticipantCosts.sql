PRINT 'Backup [ParticipantCosts]'

SELECT 
	pc.ParticipantEnrollmentId
	, pe.ParticipantFormId
INTO #ParticipantCosts
FROM dbo.ParticipantCosts pc
	INNER JOIN dbo.ParticipantEnrollments pe ON pc.ParticipantEnrollmentId = pe.Id

