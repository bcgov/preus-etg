PRINT 'INSERT [EligibleExpenseBreakdowns]'

INSERT INTO [dbo].[EligibleExpenseBreakdowns] (
	Description
	, EligibleExpenseTypeId
	, Caption
	, IsActive
	, DateAdded
	, ServiceLineId
	, EnableCost
	, RowSequence
)
SELECT 
	sl.Description
	, eet.Id
	, sl.Caption
	, sl.IsActive
	, GETUTCDATE()
	, sl.Id
	, CASE WHEN sc.ServiceTypeId = 1 THEN 1 ELSE 0 END
	, 0
FROM [dbo].[ServiceLines] sl 
	LEFT JOIN [dbo].[ServiceCategories] sc on sl.ServiceCategoryId = sc.Id 
	LEFT JOIN [dbo].[EligibleExpenseTypes] eet on eet.ServiceCategoryId = sc.Id

