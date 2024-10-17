PRINT 'INSERT [ClaimTypes]'

INSERT INTO dbo.[ClaimTypes] (
	[Id]
    , [Caption]
    , [Description]
    , [IsAmendable]
    , [IsActive]
    , [RowSequence]
	, [DateAdded]
	, [DateUpdated]
) VALUES (
	1
	, 'Single Claim (Amendable)'
	, 'Single Claim (Amendable)'
	, '1'
	, '1'
	, '0'
	, GETUTCDATE()
	, GETUTCDATE()
), (
	2
	, 'Multiple Claims'
	, 'Multiple Claims'
	, '0'
	, '1'
	, '0'
	, GETUTCDATE()
	, GETUTCDATE()
)