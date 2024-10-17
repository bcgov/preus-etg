PRINT 'INSERT [ServiceCategories]'

SET IDENTITY_INSERT [dbo].[ServiceCategories] ON

INSERT INTO dbo.[ServiceCategories] (
	Id
    , Caption
    , Description
	, ServiceTypeId
    , Rate
	, AutoInclude
	, AllowMultiple
	, MinProviders
    , MaxProviders
	, MinPrograms
	, MaxPrograms
	, CompletionReport
	, IsActive
    , RowSequence
	, DateAdded
	, DateUpdated
) VALUES (
	1
	, 'Employment Assistance Services'
	, 'A variety of services that support individuals as they prepare to enter or re-enter the workforce or assist them to find a better job.  Examples include job search services, career counselling, and résumé writing assistance.'
	, 2
	, null
	, 1
	, 0
	, 1
	, 20
	, 0
	, 0
	, 1
	, 1
	, 1
	, GETUTCDATE()
	, GETUTCDATE()
), (
	2
	, 'Skills Training'
	, 'Training delivered to participants providing the skills necessary to be successful in the labour market.  Includes costs of books, training software, examinations and certificates and other course materials.'
	, 1
	, null
	, 1
	, 0
	, 0
	, 0
	, 1
	, 30
	, 0
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
), (
	3
	, 'Participant Financial Supports'
	, 'Financial supports and benefits for the participant to remove barriers to the participant''s success in the program.'
	, 2
	, null
	, 1
	, 0
	, 0
	, 0
	, 0
	, 0
	, 1
	, 1
	, 2
	, GETUTCDATE()
	, GETUTCDATE()
), (
	4
	, 'Employment Experience'
	, 'Activities that provide the participant with opportunities to gain employment experience and support them post-employment.'
	, 2
	, null
	, 1
	, 0
	, 0
	, 0
	, 0
	, 0
	, 1
	, 0
	, 3
	, GETUTCDATE()
	, GETUTCDATE()
), (
	5
	, 'Administration'
	, 'Administration fees offset costs associated with project management, outreach and recruitment of participants, claims and reporting.'
	, 3
	, null
	, 1
	, 0
	, 0
	, 0
	, 0
	, 0
	, 0
	, 1
	, 4
	, GETUTCDATE()
	, GETUTCDATE()
)

SET IDENTITY_INSERT [dbo].[ServiceCategories] OFF