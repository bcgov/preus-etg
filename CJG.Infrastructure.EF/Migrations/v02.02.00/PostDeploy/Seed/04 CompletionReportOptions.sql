PRINT 'Inserting [Completion Report Options]'

IF (EXISTS (SELECT * FROM [dbo].[CompletionReports] WHERE Id = 2))
AND (EXISTS (SELECT * FROM [dbo].[CompletionReportGroups] WHERE CompletionReportId = 2))
AND (EXISTS (SELECT * FROM [dbo].[CompletionReportQuestions] WHERE CompletionReportId = 2))
AND (NOT EXISTS (SELECT * FROM [dbo].[CompletionReportOptions] WHERE QuestionId >= 15))
	BEGIN
		PRINT 'START Inserting ... '

		INSERT INTO [dbo].[CompletionReportOptions]
		  ([QuestionId],[Answer],[Level],[TriggersNextLevel],[NextQuestion],
		  [Sequence],[IsActive],[DisplayOther],[DateAdded])
			VALUES

		(15, 'Yes',1, 0,0, 															1, 1, 0, GETUTCDATE())
		,(15, 'No',1, 1,0, 															2, 1, 0, GETUTCDATE())
		,(15, 'Found a full-time job',2, 0,0,									1, 1, 0, GETUTCDATE())
		,(15, 'Training conflicted with existing job',2, 0,0,					2, 1, 0, GETUTCDATE())
		,(15, 'Could not afford to remain in training',2, 0,0,					3, 1, 0, GETUTCDATE())
		,(15, 'Family reasons (e.g., taking care of child, relative)',2, 0,0,			4, 1, 0, GETUTCDATE())
		,(15, 'Health reasons',	2, 0,0,											5, 1, 0, GETUTCDATE())
		,(15, 'Personal reasons (e.g., lost interest)',	2, 0,0,					6, 1, 0, GETUTCDATE())
		,(15, 'Lack of transportation',	2, 0,0,									7, 1, 0, GETUTCDATE())
		,(15, 'Moved',2, 0,0,													8, 1, 0, GETUTCDATE())
		,(15, 'Enrolled in another training or education',2, 0,0,				9, 1, 0, GETUTCDATE())
		,(15, 'Other (specify)',2, 0,0,												10, 1, 1, GETUTCDATE())
		,(15, 'Unknown',2, 0,0,														11, 1, 0, GETUTCDATE())

		-- Q16 is always asked. Q17 to Q23 are optional questions that are asked in decision tree format.
		-- The selection option leads the the next question to ask.
		,(16,'Employed',1,0,17, 												1, 1, 1, GETUTCDATE())
		,(16,'Self-employed',1,0,24, 											2, 1, 1, GETUTCDATE())
		,(16,'In school or training',1,0,19,	 								3, 1, 1, GETUTCDATE())
		,(16,'Working and in school or training',1,0,20,						4, 1, 1, GETUTCDATE())
		,(16,'Unemployed but looking for work',1,0,0, 							5, 1, 1, GETUTCDATE())
		,(16,'Unemployed and not looking for work',1,0,0,						6, 1, 1, GETUTCDATE())
		,(16,'Unknown',1,0,0, 													7, 1, 1, GETUTCDATE())

		-- Questions for q16, option 1 (Employed)
		,(17,'Full-time',1,0,18, 											1, 1, 1, GETUTCDATE())
		,(17,'Part-time',1,0,18,	 										2, 1, 1, GETUTCDATE())
		,(17,'Unknown',1,0,24, 												3, 1, 1, GETUTCDATE())

		-- Questions for Q17, option 1,2
		,(18,'Temporary',1,0,24, 												1, 1, 1, GETUTCDATE())
		,(18,'Permanent',1,0,24, 												2, 1, 1, GETUTCDATE())
		,(18,'Casual',1,0,24, 												3, 1, 1, GETUTCDATE())
		,(18,'Seasonal',1,0,24, 												4, 1, 1, GETUTCDATE())
		,(18,'Unknown',1,0,24, 												5, 1, 1, GETUTCDATE())

		-- Questions for q16, option 3 (In school or training). There is no employment, thus questions end after this set.
		,(19,'Additional employment assistance services (e.g., resume writing, work experience, career counselling)',1,0,0, 		1, 1, 1, GETUTCDATE())
		,(19,'Training to achieve high school graduation',1,0,0, 				2, 1, 1, GETUTCDATE())
		,(19,'Post-secondary (university/college) education',1,0,0, 			3, 1, 1, GETUTCDATE())
		,(19,'Job-related training',1,0,0, 									4, 1, 1, GETUTCDATE())
		,(19,'Trades training',1,0,0, 											5, 1, 1, GETUTCDATE())
		,(19,'Training for personal interest',1,0,0, 							6, 1, 1, GETUTCDATE())
		,(19,'Unknown',1,0,0, 												7, 1, 1, GETUTCDATE())

		-- Questions for q16, option 4, (Working and in school or training). They all lead to Q21 for the employment questions.
		,(20,'Additional employment assistance services (e.g., resume writing, work experience, career counselling)',1,0,21, 		1, 1, 1, GETUTCDATE())
		,(20,'Training to achieve high school graduation',1,0,21, 			2, 1, 1, GETUTCDATE())
		,(20,'Post-secondary (university/college) education',1,0,21, 		3, 1, 1, GETUTCDATE())
		,(20,'Job-related training',1,0,21, 								4, 1, 1, GETUTCDATE())
		,(20,'Trades training',1,0,21, 										5, 1, 1, GETUTCDATE())
		,(20,'Training for personal interest',1,0,21, 						6, 1, 1, GETUTCDATE())
		,(20,'Unknown',1,0,21, 											7, 1, 1, GETUTCDATE())

		-- Questions for q20, Employment
		,(21,'Employed',1,0,22,											1, 1, 1, GETUTCDATE())
		,(21,'Self-employed',1,0,22,										2, 1, 1, GETUTCDATE())
		,(21,'Unknown',1,0,24,	 											3, 1, 1, GETUTCDATE())

		-- Questions for q21
		,(22,'Full time',1,0,23, 											1, 1, 1, GETUTCDATE())
		,(22,'Part time',1,0,23, 											2, 1, 1, GETUTCDATE())
		,(22,'Unknown',1,0,24, 												3, 1, 1, GETUTCDATE())

		-- Questions for q22
		,(23,'Temporary',1,0,24, 												1, 1, 1, GETUTCDATE())
		,(23,'Permanent',1,0,24, 												2, 1, 1, GETUTCDATE())
		,(23,'Casual',1,0,24, 												3, 1, 1, GETUTCDATE())
		,(23,'Seasonal',1,0,24, 												4, 1, 1, GETUTCDATE())
		,(23,'Unknown',1,0,24, 												5, 1, 1, GETUTCDATE())

		-- Q24 Q17, Community, no options.		-- Q25 Q18, NOCS, no options.		-- Q26 Q19, NAICS, no options.		-- Q27 Q20, Additional Outcomes, Page 3, single question		,(27,'Increased job security (i.e. training allowed them to maintain job)',		1,0,0, 	1, 1, 1, GETUTCDATE())
		,(27,'Got promoted',1,0,0,	 													2, 1, 1, GETUTCDATE())
		,(27,'Took on more responsibilities',1,0,0,	 									3, 1, 1, GETUTCDATE())
		,(27,'Got an increase in pay',1,0,0,											4, 1, 1, GETUTCDATE())
		,(27,'Improved employability (skills and knowledge needed to find and maintain a job)',1,0,0, 5, 1, 1, GETUTCDATE())
		,(27,'None of the above',1,0,0,													6, 1, 1, GETUTCDATE())
		,(27,'Unknown',1,0,0, 															7, 1, 1, GETUTCDATE())
		-- Options for page 4 questions.		,(28,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(28,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(28,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(28,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(28,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(28,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(28,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(28,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(28,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(29,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(29,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(29,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(29,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(29,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(29,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(29,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(29,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(29,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(30,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(30,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(30,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(30,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(30,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(30,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(30,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(30,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(30,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(31,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(31,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(31,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(31,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(31,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(31,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(31,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(31,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(31,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(32,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(32,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(32,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(32,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(32,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(32,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(32,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(32,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(32,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(33,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(33,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(33,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(33,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(33,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(33,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(33,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(33,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(33,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(34,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(34,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(34,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(34,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(34,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(34,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(34,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(34,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(34,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(35,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(35,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(35,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(35,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(35,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(35,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(35,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(35,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(35,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(36,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(36,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(36,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(36,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(36,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(36,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(36,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(36,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(36,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(37,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(37,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(37,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(37,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(37,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(37,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(37,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(37,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(37,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(38,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(38,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(38,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(38,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(38,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(38,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(38,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(38,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(38,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(39,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(39,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(39,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(39,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(39,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(39,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(39,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(39,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(39,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(40,'1 – Very untrue',		1,0,0,	1,1,0,GETUTCDATE())		,(40,'2 – Untrue',			1,0,0,	2,1,0,GETUTCDATE())		,(40,'3 – Somewhat untrue',	1,0,0,	3,1,0,GETUTCDATE())		,(40,'4 – Neutral',			1,0,0,	4,1,0,GETUTCDATE())		,(40,'5 – Somewhat true',	1,0,0,	5,1,0,GETUTCDATE())		,(40,'6 – True',			1,0,0,	6,1,0,GETUTCDATE())		,(40,'7 – Very True',		1,0,0,	7,1,0,GETUTCDATE())		,(40,'Don''t Know',			1,0,0,	8,1,0,GETUTCDATE())		,(40,'Not Applicable',		1,0,0,	9,1,0,GETUTCDATE())		,(41,'Yes',					1,0,0,	1,1,0,GETUTCDATE())		,(41,'No',					1,0,0,	2,1,0,GETUTCDATE())		-- Q42 and Q43 are text based questions.		/*			Now add defaults to the CompletionReportQuestions.			This must be added after the fact since there is a 2 way foreign key relationship between the tables.			Means we need to add the CompletionReportQuestions, then the CompletionReportOptions, then update the FK.		*/		update [dbo].[CompletionReportQuestions] set DefaultAnswerId=			(select Id from [dbo].[CompletionReportOptions] where questionid=15 and level=1 and Answer='No')			where id=15	ENDELSE	BEGIN	PRINT 'START Updating ... '	CREATE TABLE #CWRGOptions (RowID int not null primary key identity(1,1), [QuestionId] int, [Sequence] int, [NextQuestion] int)
	INSERT INTO #CWRGOptions ([QuestionId],[Sequence],[NextQuestion])
				VALUES  (16,1,17),
						(16,2,24),
						(16,3,19),
						(16,4,20),
						(17,1,18),
						(17,2,18),
						(17,3,24),
						(18,1,24),
						(18,2,24),
						(18,3,24),
						(18,4,24),
						(18,5,24),
						(20,1,21),
						(20,2,21),
						(20,3,21),
						(20,4,21),
						(20,5,21),
						(20,6,21),
						(20,7,21),
						(21,1,22),
						(21,2,22),
						(21,3,24),
						(22,1,23),
						(22,2,23),
						(22,3,24),
						(23,1,24),
						(23,2,24),
						(23,3,24),
						(23,4,24),
						(23,5,24)

	UPDATE cro
		SET cro.[NextQuestion] = o.[NextQuestion]
	FROM dbo.[CompletionReportOptions] cro INNER JOIN #CWRGOptions o
		ON cro.QuestionId = o.QuestionId
		AND cro.[Sequence] = o.[Sequence]

	DROP TABLE #CWRGOptions	ENDPRINT 'Completed Inserting [Completion Report Options]'