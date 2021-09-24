PRINT 'UPDATE [PaymentRequests] - Delete payment, update claim and application state, reset financials'

DECLARE @TotalClaimReimbursement decimal(19, 2)
DECLARE @TotalAssessedReimbursement decimal(19, 2);
DECLARE @AgreedCommitment decimal(19, 2)
DECLARE @GrantApplicationId int

SELECT @GrantApplicationId = id 
FROM GrantApplications 
WHERE FileNumber = '1851216'

SELECT @AgreedCommitment = AgreedCommitment 
FROM TrainingPrograms 
WHERE GrantApplicationId = @GrantApplicationId

SELECT 
	@totalClaimReimbursement = TotalClaimReimbursement
	, @totalAssessedReimbursement =TotalAssessedReimbursement
FROM Claims 
WHERE ID = 915;

UPDATE Claims SET ClaimState = 21 WHERE Id = 915
UPDATE GrantApplications SET ApplicationStateExternal = 21, ApplicationStateInternal = 31 WHERE Id = @GrantApplicationId

IF @TotalClaimReimbursement IS NULL
BEGIN
  SELECT @TotalClaimReimbursement = 0
End
 
IF @totalAssessedReimbursement IS NULL 
BEGIN
 SELECT @totalAssessedReimbursement = 0
END 

-- Reset financial information to before the claim and payment were approved.
UPDATE GrantOpeningFinancials SET currentClaimCount +=  1, CurrentClaims +=  @totalClaimReimbursement WHERE GrantOpeningId = 1
UPDATE GrantOpeningFinancials SET ClaimsAssessedCount -= 1,ClaimsAssessed -=  @totalAssessedReimbursement  WHERE GrantOpeningId = 1

DELETE pr
FROM dbo.GrantApplications ga
INNER JOIN dbo.TrainingPrograms tp ON ga.Id = tp.GrantApplicationId
INNER JOIN dbo.PaymentRequests pr ON tp.Id = pr.TrainingProgramId
WHERE FileNumber = '1851216'


