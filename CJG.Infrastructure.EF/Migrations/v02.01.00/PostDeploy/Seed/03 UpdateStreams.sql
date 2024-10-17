PRINT '03. Update [GrantStreams]'

UPDATE dbo.GrantStreams
SET
	BusinessCaseIsEnabled = 1,
	BusinessCaseRequired = 1,
	BusinessCaseInternalHeader = 'Business Case:',
	BusinessCaseExternalHeader = 'Business Case',
	BusinessCaseUserGuidance = N'Training must be relevant to the immediate needs of the business and the participant''s job; and the immediate result of training must be improved job-related skills leading to a job for an unemployed person or a better job for a current employee.',
	BusinessCaseTemplateURL = N'https://www.workbc.ca/getmedia/34b85da3-6374-4d4e-80ea-e1f58cda9c55/ETG-Business-Case-Template.aspx'
Where GrantProgramId = 2

PRINT 'Update [GrantStreams] END'