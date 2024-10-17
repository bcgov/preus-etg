PRINT 'Updating [ClaimEligibleCosts]'
-- We incorrectly copied EligibleCost.AddedByAssessor into ClaimEligibleCost.AddedByAssessor.
-- A this point in time there are no assessments, so there will be no additional line items.
UPDATE [dbo].[ClaimEligibleCosts]
SET [AddedByAssessor] = 0