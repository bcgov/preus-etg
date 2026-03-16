PRINT 'Start Adding Completion Report Options'

INSERT INTO CompletionReportOptions (QuestionId, Answer, Level, TriggersNextLevel, [Sequence], IsActive, DisplayOther, NextQuestion, DateAdded)
VALUES 
(3, 'Work-sharing', 1, 0, 9, 1, 0, 0, GETUTCDATE()),
(4, 'Work-sharing', 1, 0, 9, 1, 0, 0, GETUTCDATE()),
(5, 'Work-sharing', 1, 0, 9, 1, 0, 0, GETUTCDATE())

UPDATE CompletionReportOptions
SET [Sequence] = 10,
    DateUpdated = GETUTCDATE()
WHERE Answer = 'Not Applicable'
AND QuestionId IN (3, 4, 5)

PRINT 'Done Adding Completion Report Options'
