PRINT '05. Inserting [DenialReasons]'

SET IDENTITY_INSERT [dbo].[DenialReasons] ON

INSERT [dbo].[DenialReasons] ([Id], [IsActive], [Caption] ,[GrantProgramId],[RowSequence],[DateAdded])
VALUES
	(1,1, N'Ineligible Employer (Provincial Government Employer)', 2,1,GETUTCDATE())
	,(2,1, N'Out of Province Training', 2,2,GETUTCDATE())
	,(3,1, N'Low Value for Money', 2,3,GETUTCDATE())
	,(4, 1,N'Training Not Relevant to Immediate Needs of the Business or Job', 2,4,GETUTCDATE())
	,(5,1, N'Foundational – Ineligible Training', 2,5,GETUTCDATE())
	,(6,1, N'Unemployed Participant Will Not Be Hired (All Streams)', 2,6,GETUTCDATE())
	,(7,1, N'Applied Under Incorrect Stream', 2,7,GETUTCDATE())
	,(8,1, N'ETT –"Impacted" Criteria Not Met', 2,8,GETUTCDATE())
	,(9,1, N'Ineligible Training Provider', 2,9,GETUTCDATE())
	,(10,1, N'Real or Perceived Conflict of Interest – Family Member', 2,10,GETUTCDATE())
	,(11,1, N'No Contact', 2,11,GETUTCDATE())
	,(12,1, N'Ineligible Training Activities', 2,12,GETUTCDATE())
	,(13,1, N'Business Not Operational', 2,13,GETUTCDATE())
	,(14,1, N'Training Toward a Diploma or Degree', 2,14,GETUTCDATE())
	,(15,1, N'Recurrent Training', 2,15,GETUTCDATE())
	,(16,1, N'Third-Party Application', 2,16,GETUTCDATE())
	,(17,1, N'Employer Maximum Reached', 2,17,GETUTCDATE())
	,(18,1, N'In-House Training - Third-Party Training Exists',2,18,GETUTCDATE())
	,(19,1, N'Application Submitted by Training Provider',3,1,GETUTCDATE())
	,(20,1, N'No Contact or Non-responsive', 3,2,GETUTCDATE())
	,(21,1, N'Recurrent Training', 3,3,GETUTCDATE())
	,(22,1, N'Ineligible Applicant for Cohort-based Training',3,4,GETUTCDATE())
	,(23,1, N'Low Value for Money', 3,5,GETUTCDATE())

SET IDENTITY_INSERT [dbo].[DenialReasons] OFF

PRINT 'Inserting [DenialReasons] - END'