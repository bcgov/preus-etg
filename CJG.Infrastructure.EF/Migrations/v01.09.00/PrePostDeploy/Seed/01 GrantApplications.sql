PRINT 'Migrate [GrantApplications]'

UPDATE ga
SET
	ga.StartDate = bga.StartDate
	, ga.EndDate = bga.EndDate
	, ga.InvitationKey = bga.InvitationKey
	, ga.InvitationExpiresOn = bga.InvitationExpiresOn
	, ga.HoldPaymentRequests = bga.HoldPaymentRequests
FROM dbo.GrantApplications ga
	INNER JOIN #GrantApplications bga ON ga.Id = bga.Id

DROP TABLE #GrantApplications