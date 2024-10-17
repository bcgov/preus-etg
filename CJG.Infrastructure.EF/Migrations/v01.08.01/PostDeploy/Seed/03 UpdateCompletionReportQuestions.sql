PRINT 'UPDATE COMPLETION REPORT QUESTIONS'
UPDATE CompletionReportQuestions
SET Question = REPLACE(Question, 'Canada-BC Job Grant', '@Model.ProgramName');
