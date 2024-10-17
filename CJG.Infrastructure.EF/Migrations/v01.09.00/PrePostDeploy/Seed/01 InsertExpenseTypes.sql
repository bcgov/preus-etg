PRINT 'INSERT [ExpenseTypes]'

INSERT INTO dbo.[ExpenseTypes] (
	Id
	, Caption
    , Description
    , IsActive
    , RowSequence
	, DateAdded
	, DateUpdated
) VALUES (
    1
	, 'Participant Assigned Cost'
	, 'Participant Assigned Cost'
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
), (
	2
	, 'Participant Maximum Cost Limit'
	, 'Participant Maximum Cost Limit'
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
), (
	3
	, 'Not Participant Limited'
	, 'Not Participant Limited'
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
), (
	4
	, 'Auto Limit Estimated Costs'
	, 'Auto Limit Estimated Costs'
	, 1
	, 0
	, GETUTCDATE()
	, GETUTCDATE()
)
