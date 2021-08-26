PRINT 'Update Completion Report Questions'

Update [CompletionReportQuestions]
Set [AnswerTableHeadings] = 'Name of participant,Reason|Name of participant,Participant National Occupation Classification (NOC) / Employer Name,Employer North American Industry Classification System (NAICS)'
Where [GroupId] = 2 And [QuestionType] = 5

CHECKPOINT