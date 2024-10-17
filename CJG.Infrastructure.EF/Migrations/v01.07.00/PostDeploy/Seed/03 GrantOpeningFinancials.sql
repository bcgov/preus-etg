PRINT 'Reseting [GrantOpeningFinancials]'

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

UPDATE dbo.GrantOpeningFinancials
SET CurrentReservations = 0

	, AssessedCommitments = 0
	, AssessedCommitmentsCount = 0

	, OutstandingCommitments = 0
	, OutstandingCommitmentCount = 0

	, Cancellations = 0
	, CancellationsCount = 0

	, ClaimsAssessed = 0
	, ClaimsAssessedCount = 0

	, CurrentClaims = 0
	, CurrentClaimCount = 0

	, ClaimsDenied = 0
	, ClaimsDeniedCount = 0

-- Fix Grant Opening Financial Issuesa

-- CurrentReservations
-- The total current reservations of funds for applications under assessment and not yet approved or denied for this Grant Opening.
-- CurrentReservations = Sum TrainingPrograms.TotalEstimatedReimbursement WHERE
-- ApplicationStateInternal = PendingAssessment | UnderAssessment | RecommendedForApproval | RecommendedForDenial | ReturnedToAssessment | OfferIssued
-- For all applications that have been taken in to assessment but do not have an approved agreement yet
UPDATE gof
SET gof.CurrentReservations = ISNULL(crs.CurrentReservations, gof.CurrentReservations)
FROM dbo.GrantOpeningFinancials gof
INNER JOIN (
	SELECT ga.GrantOpeningId
		, SUM( tp.TotalEstimatedReimbursement ) AS CurrentReservations
	FROM dbo.GrantApplications ga
	INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
	WHERE ga.ApplicationStateInternal IN ( 2, 3, 4, 5, 6, 7 )
	GROUP BY ga.GrantOpeningId
	) crs ON gof.GrantOpeningId = crs.GrantOpeningId

-- AssessedCommitments
-- The sum of Assessed Commitments for approved agreements linked to a Grant Opening.
-- AssessedCommitments = Sum TrainingPrograms.AgreedCommitments WHERE
-- ! The grant file has an agreement that was approved abd not cancelled
-- ApplicationStateInternal = AgreementAccepted | ChangeRequest | ChangeRequestDenied | ChangeforApproval | ChangeReturned | ChangeforDenial | ClaimReturnedtoApplicant |
-- NewClaim | ClaimAssessEligibility | ClaimAssessReimbursement | ClaimApproved | ClaimDenied | CompletionReporting | Closed
UPDATE gof
SET  gof.AssessedCommitments = ISNULL(ac.AssessedCommitments, gof.AssessedCommitments)
	, gof.AssessedCommitmentsCount = ISNULL(ac.AssessedCommitmentsCount, gof.AssessedCommitmentsCount)
FROM dbo.GrantOpeningFinancials gof
INNER JOIN (
	SELECT ga.GrantOpeningId
		, SUM( tp.AgreedCommitment ) AS AssessedCommitments
		, COUNT( tp.AgreedCommitment ) AS AssessedCommitmentsCount
	FROM dbo.GrantApplications ga
	INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
	WHERE ga.ApplicationStateInternal IN ( 9, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 30, 31, 32 )
	GROUP BY ga.GrantOpeningId
	) ac ON gof.GrantOpeningId = ac.GrantOpeningId

-- OutstandingCommitments – Claim Management Dashboard Value ****
-- The total of current outstanding commitments for which no viable claims have been received.
-- Outstanding Commitments is shown on the Claims Management dashboard. 

-- OutstandingCommitments = Sum TrainingPrograms.AgreedCommitments WHERE
-- ApplicationStateInternal = AgreementAccepted | ChangeRequest | ChangeRequestDenied | ChangeforApproval | ChangeReturned | ChangeforDenial | ClaimReturnedtoApplicant
-- AND (TrainingPrograms.Claims(**Allof**).ClaimState NOT IN (ClaimApproved | PaymentRequested | AmountOwing | ClaimPaid | AmountReceived) 
--
-- NOTE: If a claim is submitted by unassessed then the grant file state will be NewClaim and is excluded.
UPDATE gof
SET gof.OutstandingCommitments = ISNULL(oc.OutstandingCommitments, gof.OutstandingCommitments)
	, gof.OutstandingCommitmentCount = ISNULL(oc.OutstandingCommitmentCount, gof.OutstandingCommitmentCount)
FROM dbo.GrantOpeningFinancials gof
INNER JOIN (
	SELECT ga.GrantOpeningId
		, SUM( tp.AgreedCommitment ) AS OutstandingCommitments
		, COUNT( tp.AgreedCommitment ) AS OutstandingCommitmentCount
	FROM dbo.GrantApplications ga
	INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
	INNER JOIN (
		SELECT tp.Id AS TrainingProgramId
			, COUNT(c.ClaimVersion) AS NumberOfVersions
		FROM dbo.GrantApplications ga
		INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
		INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId
		GROUP BY tp.Id
	) cv ON tp.Id = cv.TrainingProgramId
	LEFT JOIN (
		SELECT tp.Id AS TrainingProgramId
			, COUNT(c.ClaimVersion) AS NumberOfValidClaims
		FROM dbo.GrantApplications ga
		INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
		INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId
		WHERE ClaimState NOT IN ( 25, 26, 27, 28, 29 )
		GROUP BY tp.Id
	) cd ON tp.Id = cd.TrainingProgramId
	WHERE ga.ApplicationStateInternal IN ( 9, 16, 17, 18, 19, 20, 24, 23 )
		AND cd.NumberOfValidClaims = cv.NumberOfVersions
	GROUP BY ga.GrantOpeningId
	) oc ON gof.GrantOpeningId = oc.GrantOpeningId
	
-- Cancellations
-- The sum of all agreed commitments for cancelled agreements.
-- Cancellations = Sum TrainingPrograms.AgreedCommitments WHERE
-- ApplicationStateInternal = CancelledbyMinistry | CancelledbyAgreementHolder
UPDATE gof
SET gof.Cancellations = ISNULL(c.Cancellations, gof.Cancellations)
	, gof.CancellationsCount = ISNULL(c.CancellationsCount, gof.CancellationsCount)
FROM dbo.GrantOpeningFinancials gof
INNER JOIN (
	SELECT ga.GrantOpeningId
		, SUM( tp.AgreedCommitment ) AS Cancellations
		, COUNT( tp.AgreedCommitment ) AS CancellationsCount
	FROM dbo.GrantApplications ga
	INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
	WHERE ga.ApplicationStateInternal IN ( 14, 15 )
	GROUP BY ga.GrantOpeningId
	) c ON gof.GrantOpeningId = c.GrantOpeningId

-- ClaimsAssessed – Claim Management Dashboard Value ****
-- The sum of total assessed reimbursement for claims that have been approved.
-- The Claims Assessed appears on the Claims Management Dashboard.
-- CurrentClaims = Sum Claims(Lasted Approved Claim).TotalAssessedReimbursement WHERE
-- ApplicationStateInternal = AgreementAccepted | ChangeRequest | ChangeRequestDenied | ChangeforApproval | ChangeReturned | ChangeforDenial | 
-- ClaimReturnedtoApplicant | NewClaim | ClaimAssessEligibility | ClaimAssessReimbursement | ClaimApproved | ClaimDenied | CompletionReporting | Closed
-- AND TrainingProgams.CurrentClaimVersion > 0 
-- AND TrainingPrograms.Claims(Last Approved Claim).ClaimState = (ClaimApproved | PaymentRequested | ClaimPaid | AmountOwing | AmountRecieved)
UPDATE gof
SET gof.ClaimsAssessed = ISNULL(ca.ClaimsAssessed, gof.ClaimsAssessed)
	, gof.ClaimsAssessedCount = ISNULL(ca.ClaimsAssessedCount, gof.ClaimsAssessedCount)
FROM dbo.GrantOpeningFinancials gof
INNER JOIN (
	SELECT ga.GrantOpeningId
		, SUM( c.TotalAssessedReimbursement ) AS ClaimsAssessed
		, COUNT( c.TotalAssessedReimbursement ) AS ClaimsAssessedCount
	FROM dbo.GrantApplications ga
	INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
	INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId
	INNER JOIN (
		SELECT c.Id AS ClaimId
			, MAX(c.ClaimVersion) AS CurrentClaimVersion
		FROM dbo.GrantApplications ga
		INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
		INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId
		WHERE c.ClaimState IN ( 25, 26, 27, 28, 29 )
		GROUP BY c.Id
	) cc ON c.Id = cc.ClaimId AND c.ClaimVersion = cc.CurrentClaimVersion
	GROUP BY ga.GrantOpeningId
	) ca ON gof.GrantOpeningId = ca.GrantOpeningId
	
-- CurrentClaims – Claim Management Dashboard value ****
-- The total current claims that are received and under assessment for an outstanding commitment where no claim has been approved yet.
-- The first claim approval records the ClaimsAssessed and stops showing a claim in CurrentClaims.  Subsequent amendments will not be summed
-- in ClaimsAssessed.  Current Claims is shown on the Claims Management dashboard.  
-- CurrentClaims = Sum TrainingPrograms(CurrentClaimVersion).TotalClaimReimbursement WHERE
-- ApplicationStateInternal = NewClaim | ClaimAssessEligibility | ClaimAssessReimbursement – a claim is under assessment
-- AND TrainingPrograms.Claims(**All**).ClaimState NOT IN (ClaimApproved | PaymentRequested | AmountOwing | ClaimPaid | Amount Received) – no claim has been approved yet.
UPDATE gof
SET gof.CurrentClaims = ISNULL(cc.CurrentClaims, gof.CurrentClaims)
	, gof.CurrentClaimCount = ISNULL(cc.CurrentClaimCount, gof.CurrentClaimCount)
FROM dbo.GrantOpeningFinancials gof
INNER JOIN (
	SELECT ga.GrantOpeningId
		, SUM( tp.AgreedCommitment ) AS CurrentClaims
		, COUNT( tp.AgreedCommitment ) AS CurrentClaimCount
	FROM dbo.GrantApplications ga
	INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
	INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId
	INNER JOIN (
		SELECT c.Id AS ClaimId
			, MAX(c.ClaimVersion) AS CurrentClaimVersion
		FROM dbo.GrantApplications ga
		INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
		INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId
		WHERE ga.ApplicationStateInternal IN ( 21, 22, 31 )
		GROUP BY c.Id
	) cc ON c.Id = cc.ClaimId AND c.ClaimVersion = cc.CurrentClaimVersion
	GROUP BY ga.GrantOpeningId
	) cc ON gof.GrantOpeningId = cc.GrantOpeningId

-- ClaimsDenied
UPDATE gof
SET gof.ClaimsDenied = ISNULL(cd.ClaimsDenied, gof.ClaimsDenied)
	, gof.ClaimsDeniedCount = ISNULL(cd.ClaimsDeniedCount, gof.ClaimsDeniedCount)
FROM dbo.GrantOpeningFinancials gof
INNER JOIN (
	SELECT ga.GrantOpeningId
		, SUM( c.TotalClaimReimbursement ) AS ClaimsDenied
		, COUNT( c.TotalClaimReimbursement ) AS ClaimsDeniedCount
	FROM dbo.GrantApplications ga
	INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
	INNER JOIN dbo.Claims c ON tp.Id = c.TrainingProgramId
	WHERE c.ClaimState IN ( 24 )
	GROUP BY ga.GrantOpeningId
	) cd ON gof.GrantOpeningId = cd.GrantOpeningId
