PRINT 'Inserting [Completion Report Questions]'
SET IDENTITY_INSERT [dbo].[CompletionReportQuestions] ON
INSERT INTO [dbo].[CompletionReportQuestions]
  ([Id], [CompletionReportId], [Question], [Description], [Audience], [GroupId], [Sequence], [IsRequired], [IsActive], [QuestionType], [DefaultText], [DefaultAnswerId], [ContinueIfAnswerId], [StopIfAnswerId], [AnswerTableHeadings], [DateAdded], [DateUpdated])
    VALUES
  (1, 1, 'Have all participants completed the training?', 'Please select the participants that did not complete the training and the reason for not completing.', 0, 1, 1, 1, 1, 1, 'Please select a reason', NULL, NULL, NULL, 'Select All,Name of participant,Reason', GETUTCDATE(), NULL)
  ,
  (2, 1, 'Were all participants employed by you at the end of training?', 'Please select the participants that were not employed by you at the end of training and the reason', 0, 2, 1, 1, 1, 1, 'Please select a reason', NULL, NULL, NULL, 'Select All,Name of participant,Reason', GETUTCDATE(), NULL)
  ,
  (3, 1, 'What were the important outcomes of this training for participants?\nPlease select the important outcomes of the training.', 'If the same for all participants', 0, 3, 1, 1, 1, 2, 'Select a reason', NULL, NULL, NULL, 'Name of participant,Most important reason*,Second most important reason,Third most important reason', GETUTCDATE(), NULL)
  ,
  (4, 1, 'What were the important outcomes of this training for participants?\nPlease select the second most important outcomes of the training.', '', 0, 3, 1, 1, 1, 2, 'Select a reason', NULL, NULL, NULL, 'Name of participant,Most important reason*,Second most important reason,Third most important reason', GETUTCDATE(), NULL)
  ,
  (5, 1, 'What were the important outcomes of this training for participants?\nPlease select the third most important outcomes of the training.', '', 0, 3, 1, 1, 1, 2, 'Select a reason', NULL, NULL, NULL, 'Name of participant,Most important reason*,Second most important reason,Third most important reason', GETUTCDATE(), NULL)
  ,
  (6, 1, 'The training was relevant to my business.', 'Description for The training was relevant to my business question', 1, 4, 1, 1, 1, 2, 'Please select an option', NULL, NULL, NULL, '', GETUTCDATE(), NULL)
  ,
  (7, 1, 'The training was a good investment.', 'Description for The training was a good investment question', 1, 4, 2, 1, 1, 2, 'Please select an option', NULL, NULL, NULL, '', GETUTCDATE(), NULL)
  ,
  (8, 1, 'The Canada-BC Job Grant was easy to access.', 'Description for The training was relevant to my business question', 1, 4, 3, 1, 1, 2, 'Please select an option', NULL, NULL, NULL, '', GETUTCDATE(), NULL)
  ,
  (9, 1, 'I will continue to invest in training for employees due to my experience with the Canada-BC Job Grant.', 'Description for I will continue to invest in training for employees due to my experience with the Canada-BC Job Grant question', 1, 4, 4, 1, 1, 2, 'Please select an option', NULL, NULL, NULL, '', GETUTCDATE(), NULL)
  ,
  (10, 1, 'The processing of my application and reimbursement was timely.', 'Description for The processing of my application and reimbursement was timely question', 1, 4, 5, 1, 1, 2, 'Please select an option', NULL, NULL, NULL, '', GETUTCDATE(), NULL)
  ,
  (11, 1, 'The Canada-BC Job Grant met the skills training needs of my business.', 'Description for The Canada-BC Job Grant met the skills training needs of my business question', 1, 4, 6, 1, 1, 2, 'Please select an option', NULL, NULL, NULL, '', GETUTCDATE(), NULL)
  ,
  (12, 1, 'What did you like best about the Canada-BC Job Grant program?', 'Description for What did you like best about the Canada-BC Job Grant program question', 1, 4, 7, 1, 1, 3, '', NULL, NULL, NULL, '', GETUTCDATE(), NULL)
  ,
  (13, 1, 'What would you change or improve about the program or process?', 'Description for What would you change or improve about the program or process question', 1, 4, 8, 1, 1, 3, '', NULL, NULL, NULL, '', GETUTCDATE(), NULL)
SET IDENTITY_INSERT [dbo].[CompletionReportQuestions] OFF