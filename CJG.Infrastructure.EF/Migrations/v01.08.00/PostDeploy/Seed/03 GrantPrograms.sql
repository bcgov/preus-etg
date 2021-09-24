PRINT 'Updating [GrantPrograms]'

SET IDENTITY_INSERT [dbo].[GrantPrograms] ON

INSERT INTO dbo.[GrantPrograms] (
	[Id]
	, [AccountCodeId]
	, [Name]
	, [Abbreviation]
	, [Description]
	, [Message]
	, [ShowMessage]
	, [CanReportParticipants]
	, [CanReportSponsors]
	, [State]
	, [ApplicantDeclarationTemplateId]
	, [ApplicantCoverLetterTemplateId]
	, [ApplicantScheduleATemplateId]
	, [ApplicantScheduleBTemplateId]
	, [ParticipantConsentTemplateId]
	, [ExpenseAuthorityId]
	, [RequestedBy]
	, [ProgramPhone]
	, [DocumentPrefix]
) VALUES (
	1
	, 1
	, 'Canada-B.C. Job Grant'
	, 'CJG'
	, 'Payment on receipt of claim for eligible training expenses under the Canada-B.C. Job Grant, as per Agreement.'
	, NULL
	, 0
	, 1
	, 1
	, 1
	, (SELECT TOP 1 Id FROM dbo.[DocumentTemplates] WHERE DocumentType = 4) -- ApplicantDeclarationTemplateId
	, (SELECT TOP 1 Id FROM dbo.[DocumentTemplates] WHERE DocumentType = 1) -- ApplicantCoverLetterTemplateId
	, (SELECT TOP 1 Id FROM dbo.[DocumentTemplates] WHERE DocumentType = 2) -- ApplicantScheduleATemplateId
	, (SELECT TOP 1 Id FROM dbo.[DocumentTemplates] WHERE DocumentType = 3) -- ApplicantScheduleBTemplateId
	, (SELECT TOP 1 Id FROM dbo.[DocumentTemplates] WHERE DocumentType = 5) -- ParticipantConsentTemplateId
	, (SELECT TOP 1 Id FROM dbo.[InternalUsers] iu WHERE iu.IDIR = 'gcasault')
	, 'Canada-B.C. Job Grant Unit, Labour Market Programs Branch, Ministry of Advanced Education, Skills and Training'
	, '(250) 387-4428'
	, 'CJG'
), (
	2
	, 2
	, 'B.C. Employer Training Grant'
	, 'ETG'
	, 'Payment on receipt of claim for eligible training expenses under the Canada-B.C. Job Grant, as per Agreement.'
	, NULL
	, 0
	, 1
	, 1
	, 1
	, (SELECT TOP 1 Id FROM dbo.[DocumentTemplates] WHERE DocumentType = 4) -- ApplicantDeclarationTemplateId
	, (SELECT TOP 1 Id FROM dbo.[DocumentTemplates] WHERE DocumentType = 1) -- ApplicantCoverLetterTemplateId
	, (SELECT TOP 1 Id FROM dbo.[DocumentTemplates] WHERE DocumentType = 2) -- ApplicantScheduleATemplateId
	, (SELECT TOP 1 Id FROM dbo.[DocumentTemplates] WHERE DocumentType = 3) -- ApplicantScheduleBTemplateId
	, (SELECT TOP 1 Id FROM dbo.[DocumentTemplates] WHERE DocumentType = 5) -- ParticipantConsentTemplateId
	, (SELECT TOP 1 Id FROM dbo.[InternalUsers] iu WHERE iu.IDIR = 'gcasault')
	, 'Canada-B.C. Job Grant Unit, Labour Market Programs Branch, Ministry of Advanced Education, Skills and Training'
	, '(250) 387-4428'
	, 'CJG'
)

SET IDENTITY_INSERT [dbo].[GrantPrograms] OFF