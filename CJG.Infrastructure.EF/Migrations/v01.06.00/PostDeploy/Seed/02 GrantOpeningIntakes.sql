PRINT 'Reseting [GrantOpeningIntakes]'

/***
Draft = 0,
New = 1,
PendingAssessment = 2,
UnderAssessment = 3,
ReturnedToAssessment = 4,
RecommendedForApproval = 5,
RecommendedForDenial = 6,
OfferIssued = 7,
OfferWithdrawn = 8,
AgreementAccepted = 9,
Unfunded = 10,
ApplicationDenied = 11,
AgreementRejected = 12,
ApplicationWithdrawn = 13,
CancelledByMinistry = 14,
CancelledByAgreementHolder = 15,
ChangeRequest = 16,
ChangeForApproval = 17,
ChangeForDenial = 18,
ChangeReturned = 19,
ChangeRequestDenied = 20,
NewClaim = 21,
ClaimAssessEligibility = 22,
ClaimReturnedToApplicant = 23,
ClaimDenied = 24,
ClaimApproved = 25,
Closed = 30,
ClaimAssessReimbursement = 31,
CompletionReporting = 32
*/

UPDATE dbo.GrantOpeningIntakes
SET NewAmt = 0
	, NewCount = 0

	, PendingAssessmentAmt = 0
	, PendingAssessmentCount = 0

	, UnderAssessmentAmt = 0
	, UnderAssessmentCount = 0

	, DeniedAmt = 0
	, DeniedCount = 0

	, WithdrawnAmt = 0
	, WithdrawnCount = 0

	, ReductionsAmt = 0

-- Grant Opening Intakes
-- New
UPDATE goi
SET goi.NewAmt = ISNULL(na.NewAmt, goi.NewAmt)
	, goi.NewCount = ISNULL(na.NewCount, goi.NewCount)
FROM dbo.GrantOpeningIntakes goi
INNER JOIN (
		SELECT ga.GrantOpeningId
			, SUM(tp.TotalEstimatedReimbursement) AS NewAmt
			, COUNT(tp.TotalEstimatedReimbursement) AS NewCount
		FROM dbo.TrainingPrograms tp
		INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
		WHERE ga.ApplicationStateInternal IN ( 1 )
		GROUP BY ga.GrantOpeningId
	) AS na ON goi.GrantOpeningId = na.GrantOpeningId
	
-- Pending
UPDATE goi
SET goi.PendingAssessmentAmt = ISNULL(pa.PendingAssessmentAmt, goi.PendingAssessmentAmt)
	, goi.PendingAssessmentCount = ISNULL(pa.PendingAssessmentCount, goi.PendingAssessmentCount)
FROM dbo.GrantOpeningIntakes goi
INNER JOIN (
		SELECT ga.GrantOpeningId
			, SUM(tp.TotalEstimatedReimbursement) AS PendingAssessmentAmt
			, COUNT(tp.TotalEstimatedReimbursement) AS PendingAssessmentCount
		FROM dbo.TrainingPrograms tp
		INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
		WHERE ga.ApplicationStateInternal IN ( 2 )
		GROUP BY ga.GrantOpeningId
	) AS pa ON goi.GrantOpeningId = pa.GrantOpeningId
	
-- UnderAssessment
-- UnderAssessmentAmt = Sum(TrainingPrograms.TotalEstimatedReimbursement where ApplicationStateInternal= (UnderAssessment | ReturnedToAssessment | RecommendedForApproval | RecommendedForDenial | OfferIssued )
UPDATE goi
SET goi.UnderAssessmentAmt = ISNULL(ua.UnderAssessmentAmt, goi.UnderAssessmentAmt)
	, goi.UnderAssessmentCount = ISNULL(ua.UnderAssessmentCount, goi.UnderAssessmentCount)
FROM dbo.GrantOpeningIntakes goi
INNER JOIN (
		SELECT ga.GrantOpeningId
			, SUM(tp.TotalEstimatedReimbursement) AS UnderAssessmentAmt
			, COUNT(tp.TotalEstimatedReimbursement) AS UnderAssessmentCount
		FROM dbo.TrainingPrograms tp
		INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
		WHERE ga.ApplicationStateInternal IN ( 3, 4, 5, 6, 7 )
		GROUP BY ga.GrantOpeningId
	) AS ua ON goi.GrantOpeningId = ua.GrantOpeningId

-- Denied
UPDATE goi
SET goi.DeniedAmt = ISNULL(da.DeniedAmt, goi.DeniedAmt)
	, goi.DeniedCount = ISNULL(da.DeniedCount, goi.DeniedCount)
FROM dbo.GrantOpeningIntakes goi
INNER JOIN (
		SELECT ga.GrantOpeningId
			, SUM(tp.TotalEstimatedReimbursement) AS DeniedAmt
			, COUNT(tp.TotalEstimatedReimbursement) AS DeniedCount
		FROM dbo.TrainingPrograms tp
		INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
		WHERE ga.ApplicationStateInternal IN ( 11 )
		GROUP BY ga.GrantOpeningId
	) AS da ON goi.GrantOpeningId = da.GrantOpeningId

-- Withdrawn
-- Applications Where ApplicationStateInternal = (OfferWithdrawn | AgreementRejected | ApplicationWithdrawn)
UPDATE goi
SET goi.WithdrawnAmt = ISNULL(wa.WithdrawnAmt, goi.WithdrawnAmt)
	, goi.WithdrawnCount = ISNULL(wa.WithdrawnCount, goi.WithdrawnCount)
FROM dbo.GrantOpeningIntakes goi
INNER JOIN (
		SELECT ga.GrantOpeningId
			, SUM(tp.TotalEstimatedReimbursement) AS WithdrawnAmt
			, COUNT(tp.TotalEstimatedReimbursement) AS WithdrawnCount
		FROM dbo.TrainingPrograms tp
		INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
		WHERE ga.ApplicationStateInternal IN ( 8, 12, 13 )
		GROUP BY ga.GrantOpeningId
	) AS wa ON goi.GrantOpeningId = wa.GrantOpeningId

-- Reductions
-- ReductionsAmt = Sum (TrainingPrograms.TotalEstimatedReimbursement) - Sum(TrainingPrograms.AgreedCommitment) 
-- where ApplicationStateInternal = ( AgreementAccepted | ChangeRequest | ChangeForApproval | ChangeForDenial | ChangeReturned | ChangeRequestDenied | NewClaim | ClaimAssessEligibility | ClaimAssessReimbursement | ClaimReturnedtoApplicant | ClaimDenied | ClaimApproved | Closed | CompletionReporting)
UPDATE goi
SET goi.ReductionsAmt = ISNULL(ra.ReductionsAmt, goi.ReductionsAmt)
FROM dbo.GrantOpeningIntakes goi
INNER JOIN (
		SELECT ga.GrantOpeningId
			, SUM(tp.TotalEstimatedReimbursement) - SUM(tp.AgreedCommitment) AS ReductionsAmt
		FROM dbo.TrainingPrograms tp
		INNER JOIN dbo.GrantApplications ga ON tp.GrantApplicationId = ga.Id
		WHERE ga.ApplicationStateInternal IN ( 9, 14, 15, 16, 17, 18, 20, 21, 22, 31, 23, 24, 25, 30, 32 )
		GROUP BY ga.GrantOpeningId
	) AS ra ON goi.GrantOpeningId = ra.GrantOpeningId
