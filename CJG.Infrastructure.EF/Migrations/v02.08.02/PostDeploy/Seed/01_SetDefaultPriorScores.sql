PRINT 'Start setting Default Priority Score'

UPDATE PrioritizationThresholds
SET IndustryAssignedScore = 1,
	RegionalThresholdAssignedScore = 1,
	EmployeeCountAssignedScore = 1,
	FirstTimeApplicantAssignedScore = 1,
	DateUpdated = GETUTCDATE()
WHERE Id > 0

PRINT 'End setting Default Priority Score'
