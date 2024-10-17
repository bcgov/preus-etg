PRINT '02. Add [GrantStreamEligibilityAnswers]'

-- Add Eligibility Answers to existing Grant Applications. Identify the question being asked by RowSequence;
-- RowSequence = 1 is the existing Eligibility question, and RowSequence = 2 is the Insurance question.

-- 1. Eligibility question.
INSERT INTO dbo.GrantStreamEligibilityAnswers
	(GrantApplicationId, EligibilityAnswer, GrantStreamEligibilityQuestionId, DateAdded)
SELECT DISTINCT GA.Id as GrantApplicationId, GA.EligibilityConfirmed as EligibilityAnswer, GSEQ.Id as GrantStreamEligibilityQuestionId, GETUTCDATE()
FROM dbo.GrantApplications GA
JOIN dbo.GrantOpenings GRO on GRO.Id = GA.GrantOpeningId
JOIN dbo.GrantStreams GS on GS.Id = GRO.GrantStreamId
JOIN dbo.GrantStreamEligibilityQuestions GSEQ on GSEQ.GrantStreamId = GRO.GrantStreamId
WHERE GS.GrantProgramId = 3 AND GSEQ.IsActive = 1 AND GSEQ.RowSequence = 1 AND GA.EligibilityConfirmed IS NOT NULL

-- 2. Insurance question.
INSERT INTO dbo.GrantStreamEligibilityAnswers
	(GrantApplicationId, EligibilityAnswer, GrantStreamEligibilityQuestionId, DateAdded)
SELECT DISTINCT GA.Id as GrantApplicationId, GA.InsuranceConfirmed as EligibilityAnswer, GSEQ.Id as GrantStreamEligibilityQuestionId, GETUTCDATE()
FROM dbo.GrantApplications GA
JOIN dbo.GrantOpenings GRO on GRO.Id = GA.GrantOpeningId
JOIN dbo.GrantStreams GS on GS.Id = GRO.GrantStreamId
JOIN dbo.GrantStreamEligibilityQuestions GSEQ on GSEQ.GrantStreamId = GRO.GrantStreamId
WHERE GS.GrantProgramId = 3 AND GSEQ.IsActive = 1 AND GSEQ.RowSequence = 2 AND GA.InsuranceConfirmed IS NOT NULL

PRINT '02. Finished adding [GrantStreamEligibilityAnswers]'
