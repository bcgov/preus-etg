PRINT 'Inserting [Completion Report Questions]'

IF (EXISTS (SELECT * FROM [dbo].[CompletionReports] WHERE Id = 2))
AND (EXISTS (SELECT * FROM [dbo].[CompletionReportGroups] WHERE CompletionReportId = 2))
AND (NOT EXISTS (SELECT * FROM [dbo].[CompletionReportQuestions] WHERE CompletionReportId = 2))
	BEGIN
		PRINT 'START Inserting...'
		SET IDENTITY_INSERT [dbo].[CompletionReportQuestions] ON
		INSERT INTO [dbo].[CompletionReportQuestions]
		  ([Id], [CompletionReportId], [Question], [Description], [Audience], [GroupId], [Sequence],
		  [IsRequired], [IsActive], [QuestionType], [DefaultText], [DefaultAnswerId], [ContinueIfAnswerId],
		  [StopIfAnswerId], [AnswerTableHeadings], [DateAdded], [DateUpdated],
		  [DisplayOnlyIfGoto],[NextQuestion])
		VALUES

		-- New columns:
		-- [DisplayOnlyIfGoto]	Bool: if true, do not display this question normally, only when [GoToQuestionAfter] or CompletionReportOptions.[NextQuestion] is used.
		-- [NextQuestion]	After this question has been answered, go to NextQuestion.
		--
		-- New question types: 6=Community, 7=NOCS, 8=NAICS

		  (15, 2, 'Have all participants completed the training?', 'For each participant that did not complete, indicate why they dropped out.', 0, 6, 1,
		  1, 1, 1, 'Please select a reason', NULL, NULL, NULL, 'Select All,Name of participant,Reason', GETUTCDATE(), NULL, 0, NULL)

		  ,(16, 2, 'Indicate employment status of this participant after the training.', '', 0, 7, 10,
		  1, 1, 2, 'Select employment status', NULL, NULL, NULL, 'Name of Participant, Employment Status', GETUTCDATE(), NULL, 0, NULL)

		-- Questions that execute only after an option has been selected. Q16 above starts the chain. The selected option, or the
		-- question's NextQuestion, determines the next question.
		  ,(17, 2, 'Full Time/Part Time', '', 0, 7, 11,
		  1, 1, 2, 'Select Full Time/Part Time', NULL, NULL, NULL, '', GETUTCDATE(), NULL, 1, NULL)

		  ,(18, 2, 'Type of Employment', '', 0, 7, 12,
		  1, 1, 2, 'Select Type of Employment', NULL, NULL, NULL, '', GETUTCDATE(), NULL, 1, NULL)

		  ,(19, 2, 'What program is this person enrolled in?', '', 0, 7, 13,
		  1, 1, 2, 'Select Program this person enrolled in', NULL, NULL, NULL, '', GETUTCDATE(), NULL, 1, NULL)

		  ,(20, 2, 'What program is this person enrolled in?', '', 0, 7, 14,
		  1, 1, 2, 'Select Program this person enrolled in', NULL, NULL, NULL, '', GETUTCDATE(), NULL, 1, NULL)

		  ,(21, 2, 'What type of employment does this person have?', '', 0, 7, 15,
		  1, 1, 2, 'Select Type of employment', NULL, NULL, NULL, '', GETUTCDATE(), NULL, 1, NULL)

		  ,(22, 2, 'Full Time/Part Time', '', 0, 7, 16,
		  1, 1, 2, 'Select Full Time/Part Time', NULL, NULL, NULL, '', GETUTCDATE(), NULL, 1, NULL)

		  ,(23, 2, 'Type of Employment', '', 0, 7, 17,
		  1, 1, 2, 'Select Type of Employment', NULL, NULL, NULL, '', GETUTCDATE(), NULL, 1, NULL)

		  ,(24, 2, 'What community does this participant work in?', '', 0, 7, 100,
		  1, 1, 6, 'Select community this participant works in', NULL, NULL, NULL, '', GETUTCDATE(), NULL, 1, 25)

		  ,(25, 2, 'What industry does this person work in?', '', 0, 7, 101,
		  1, 1, 7, 'Select an industry', NULL, NULL, NULL, '', GETUTCDATE(), NULL, 1, 26)

		  ,(26, 2, 'What occupation does this person work in?', '', 0, 7, 102,
		  1, 1, 8, 'Select an industry', NULL, NULL, NULL, '', GETUTCDATE(), NULL, 1, NULL)

		-- Page 3, single question.
		  ,(27, 2, 'For each participant, select additional outcomes that describe changes in their employment situation after they have completed training (select all that apply)', '', 0, 8, 1,
		  1, 1, 9, 'Please select all that apply', NULL, NULL, NULL, 'Name of Participant,Selections', GETUTCDATE(), NULL, 0, NULL)

		-- Employer questions. All on page 4.
		  ,(28, 2, 'The online system I used to apply and report on the Community Workforce Response Grant was user-friendly.','Description',1,9,1,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)
		  ,(29, 2, 'It was convenient to use BCeID to apply for the grant.','Description',1,9,2,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)
		  ,(30, 2, 'I received adequate support from the CWRG team in the process of my application.','Description',1,9,3,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)
		  ,(31, 2, 'The processing of my reimbursement was timely.','Description',1,9,4,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)
		  ,(32, 2, 'CWRG has three intake periods with distinct project delivery start dates. The project delivery start dates allowed for my intake aligned well with my project plan.','Description',1,9,5,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)
		  ,(33, 2, 'The maximum funding limit in my stream was adequate to meet my project needs.','Description',1,9,6,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)
		  ,(34, 2, 'Community Workforce Response Grant allowed my organization to respond to labour and skills needs that my community/industry has identified in a timely manner.','Description',1,9,7,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)

		  ,(35, 2, 'Participation in the Community Workforce Response Grant encouraged my organization to engage with stakeholders or partners in my community/industry (e.g., local governments, Indigenous partners, educational institutions, employers, industry and sector groups).','Description',1,9,8,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)
		  ,(36, 2, 'As a result of participation in the Community Workforce Response Grant, my organization was able to create new connections with stakeholders or Indigenous partners interested in addressing labour force needs in my community/industry.','Description',1,9,9,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)

		  ,(37, 2, 'Community Workforce Response Grant increased my organization''s capacity to respond to labour and skills needs in the future.','Description',1,9,10,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)
		  ,(38, 2, 'Community Workforce Response Grant helped position my organization as a key player in addressing labour force needs of my community/ industry.','Description',1,9,11,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)

		  ,(39, 2, 'To the best of my knowledge, skilled workers who completed this training program were available right when employers in the community/industry needed them.','Description',1,9,12,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)
		  ,(40, 2, 'To the best of my knowledge, the skills that participants received through this training were relevant to the needs of employers in my community/industry.','Description',1,9,13,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)

		  ,(41, 2, 'If there are continuing or future labour and skills needs in your community/industry, would you apply for the grant again?','Description',1,9,14,
		  1,1,2,'Please select an option',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)
		  ,(42, 2, 'What did you like best about the Community Workforce Response Grant program?','Description',1,9,15,
		  1,1,3,'Default text',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)
		  ,(43, 2, 'What would you change or improve about the program or process?','Description',1,9,16,
		  1,1,3,'Default text',NULL, NULL, NULL,'',GETUTCDATE(), NULL, 0, NULL)

		SET IDENTITY_INSERT [dbo].[CompletionReportQuestions] OFF
	END
ELSE
	BEGIN
		PRINT 'START Updating...'
		UPDATE [dbo].[CompletionReportQuestions]
		SET
		   [DateUpdated] = GETUTCDATE()
		  ,[DisplayOnlyIfGoto] = 1
		WHERE Id IN (17,18,19,20,21,22,23,24,25,26)

		UPDATE [dbo].[CompletionReportQuestions]
		SET
		   [DateUpdated] = GETUTCDATE()
		  ,[NextQuestion] = 25
		WHERE Id = 24

		UPDATE [dbo].[CompletionReportQuestions]
		SET
		   [DateUpdated] = GETUTCDATE()
		  ,[NextQuestion] = 26
		WHERE Id = 25
	END

PRINT 'Completed Inserting [Completion Report Questions]'