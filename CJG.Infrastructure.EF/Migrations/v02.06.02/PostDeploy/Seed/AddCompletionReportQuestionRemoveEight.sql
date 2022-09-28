PRINT 'Starting Update of ETG Completion Report Questions'

PRINT 'Inserting [Completion Report Question]'

UPDATE [CompletionReportQuestions]
SET IsActive = 0,
DateUpdated = GETUTCDATE()
WHERE CompletionReportId = 1
AND GroupId = 4
  
PRINT 'Deactivating Existing [Completion Report Questions]'

INSERT INTO [CompletionReportQuestions] ([CompletionReportId], [Question], Description, Audience, GroupId, [Sequence], IsRequired, IsActive, QuestionType, DefaultText, DateAdded, DisplayOnlyIfGoto)
VALUES (1, 
'Do you have any suggestions on how we could improve the Employer Training Grant Program?<br/>For example, was the website clear; was the online application easy to complete and submit; was the Eligibility Criteria clear and easy to follow?',
'Open-ended question for ETG Employers', 1, 4, 1, 1, 1, 3, 
'<p><strong>Please take a few minutes to complete the survey question. Your response will help improve future services provided by the Employer Training Grant program:</strong></p><h3>Opportunity to Provide Feedback</h3>', 
GETUTCDATE(), 0)

PRINT 'Done Update of ETG Completion Report Questions'
