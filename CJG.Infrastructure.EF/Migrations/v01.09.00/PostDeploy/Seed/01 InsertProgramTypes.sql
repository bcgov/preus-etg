PRINT 'INSERT [ProgramTypes]'

INSERT INTO dbo.[ProgramTypes] (
	Id
    , Caption
    , Description
    , IsActive
    , RowSequence
	, DateAdded
	, DateUpdated
) VALUES (
	1
	, 'Employer Grant'
	, 'Employer grant programs are for a single dedicated training program.'
	, '1'
	, '0'
	, GETUTCDATE()
	, GETUTCDATE()
), (
	2
	, 'WDA Service'
	, 'Grant application process is dynamically managed by the Service Categories.'
	, '1'
	, '0'
	, GETUTCDATE()
	, GETUTCDATE()
)