PRINT 'Update Completion Report Questions'

Update [CompletionReportQuestions]
Set [QuestionType] = 5,
	[AnswerTableHeadings] = 'Name of participant,Reason, |Name of participant,Participant National Occupation Classification (NOC) / Employer Name,Employer North American Industry Classification System (NAICS)',
	[Description] = 'For each participant not employed after training, check "Not Employed" and select the reason below.|For each participant employed after training, enter the participant NOC, employer name, and employer NAICS below.'
Where [GroupId] = 2 And [QuestionType] = 1
