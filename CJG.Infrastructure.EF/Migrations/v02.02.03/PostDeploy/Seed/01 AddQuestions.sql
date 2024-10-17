PRINT '01. Add [GrantStreamEligibilityQuestions]'

-- Add the questions from the existing streams
-- Eligibility question added if the current question is not null in the Grant Stream

INSERT INTO dbo.GrantStreamEligibilityQuestions
	(GrantStreamId, EligibilityRequirements, EligibilityQuestion, IsActive,EligibilityPositiveAnswerRequired, Rowsequence, DateAdded)
SELECT DISTINCT Id, EligibilityRequirements, EligibilityQuestion, EligibilityEnabled, EligibilityRequired, 1, GETUTCDATE()
FROM dbo.GrantStreams WHERE EligibilityQuestion IS NOT null

-- Add the default insurance question, only for programId 3, which is CWRG.
INSERT INTO dbo.GrantStreamEligibilityQuestions
	(GrantStreamId, EligibilityRequirements, EligibilityQuestion, IsActive,	EligibilityPositiveAnswerRequired, Rowsequence, DateAdded)
SELECT DISTINCT ID, N'', N'As the applicant, does your organization have the appropriate liability insurance to cover the skills-training project on your premises and/or other locations as required?',
	1, 1, 2, GETUTCDATE()
FROM dbo.GrantStreams
WHERE GrantProgramId= 3

PRINT '01. Finished adding [GrantStreamEligibilityQuestions]'
