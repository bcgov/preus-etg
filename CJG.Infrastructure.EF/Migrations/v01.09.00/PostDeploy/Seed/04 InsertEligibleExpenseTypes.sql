PRINT 'INSERT [EligibleExpenseTypes]'

INSERT INTO [dbo].[EligibleExpenseTypes] (
	Caption
    , Description
    , Rate
	, AutoInclude
	, AllowMultiple
	, MinProviders
    , MaxProviders
	, IsActive
	, ExpenseTypeId
	, ServiceCategoryId
	, DateAdded
	, RowSequence
) 
SELECT 
	sc.Caption
    , sc.Description
	, sc.Rate
	, sc.AutoInclude
	, sc.AllowMultiple
	, sc.MinProviders
	, sc.MaxProviders
	, sc.IsActive
	, 3
	, sc.Id
	, GETUTCDATE()
	, RowSequence
FROM [dbo].[ServiceCategories] sc