PRINT 'INSERT [ServiceLineBreakdowns]'

SET IDENTITY_INSERT [dbo].[ServiceLineBreakdowns] ON

INSERT INTO dbo.[ServiceLineBreakdowns] (
	Id
    , Caption
    , Description
	, ServiceLineId
    , IsActive
    , RowSequence
	, DateAdded
	, DateUpdated
) VALUES (
	1
	, 'Reading'
	, ''	
	, 5
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
), (
	2
	, 'Writing'
	, ''	
	, 5
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
), (
	3
	, 'Numeracy'
	, ''	
	, 5
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
), (
	4
	, 'English'
	, ''	
	, 5
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
), (
	5
	, 'Basic Computer Skills'
	, ''	
	, 5
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
), (
	6
	, 'Occupational'
	, ''	
	, 7
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
)
, (
	7
	, 'Management and Business Skills'
	, ''	
	, 7
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
)
, (
	8
	, 'Technical Skills'
	, ''	
	, 7
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
), (
	9
	, 'Other'
	, ''	
	, 7
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
)

SET IDENTITY_INSERT [dbo].[ServiceLineBreakdowns] OFF

INSERT INTO ServiceLineBreakdowns (Description, ServiceLineId, Caption, IsActive, RowSequence, DateAdded, DateUpdated)
SELECT '', 8, Caption, 1, 0, GetDate(), GETDATE()
from InDemandOccupations