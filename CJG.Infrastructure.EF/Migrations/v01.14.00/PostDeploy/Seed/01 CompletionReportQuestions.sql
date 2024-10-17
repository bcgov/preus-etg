PRINT 'UPDATE [Completion Report Questions]'

Update [CompletionReportQuestions]
Set Question = 'What were the important outcomes of this training for participants?<br/>Please select the important outcome of the training.'
Where Id = 3;

Update [CompletionReportQuestions]
Set Question = 'What were the important outcomes of this training for participants?<br/>Please select the second most important outcome of the training.'
Where Id = 4;

Update [CompletionReportQuestions]
Set Question = 'What were the important outcomes of this training for participants?<br/>Please select the third most important outcome of the training.'
Where Id = 5;

CHECKPOINT