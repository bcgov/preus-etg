PRINT 'INSERT [ProgramConfigurations]'

INSERT INTO dbo.[ProgramConfigurations] (
	[Caption]
    , [Description]
    , [ClaimTypeId]
	, [SkillsTrainingMaxEstimatedParticipantCosts]
	, [ESSMaxEstimatedParticipantCost]
    , [IsActive]
    , [DateAdded]
	, [DateUpdated]
) VALUES (
	'Employer Grant'
	, 'Employer grant program configuration which has a list of eligible expense types, where each expense must be assigned to a participant.  Only a single claim can be submitted, but it can be amended multiple times.'
	, 1
	, 0
	, 0
	, 1
	, GETUTCDATE()
	, GETUTCDATE()
), (
	'WDA Services'
	, 'New grant program configuration which has a dynamic application development process managed by the Service Categories.  Multiple claims can be submitted.'
	, 2
	, 10000
	, 5000
	, 1
	, GETUTCDATE()
	, GETUTCDATE()
)

