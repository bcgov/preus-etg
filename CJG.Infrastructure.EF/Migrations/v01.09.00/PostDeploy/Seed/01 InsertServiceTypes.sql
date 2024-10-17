PRINT 'INSERT [ServiceTypes]'

INSERT INTO dbo.[ServiceTypes] (
	Id
    , Caption
    , Description
    , IsActive
    , RowSequence
	, DateAdded
	, DateUpdated
) VALUES (
	1
	, 'Skills Training'
	, 'Allow for multiple training components.'
	, '1'
	, '0'
	, GETUTCDATE()
	, GETUTCDATE()
), (
	2
	, 'Employment Services and Supports'
	, 'Allow for multiple service providers.'
	, '1'
	, '0'
	, GETUTCDATE()
	, GETUTCDATE()
), (
	3
	, 'Administration'
	, 'Include additional eligible expense types.'
	, '1'
	, '0'
	, GETUTCDATE()
	, GETUTCDATE()
)