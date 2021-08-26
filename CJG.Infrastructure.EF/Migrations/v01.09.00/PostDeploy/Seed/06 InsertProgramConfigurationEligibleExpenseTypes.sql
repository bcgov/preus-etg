PRINT 'INSERT [ProgramConfigurationEligibleExpenseTypes]'

INSERT INTO dbo.[ProgramConfigurationEligibleExpenseTypes] (
	ProgramConfigurationId
	, EligibleExpenseTypeId
)
SELECT 
	CASE WHEN eet.ServiceCategoryId IS NULL THEN 1 ELSE 2 END 
	, eet.Id 
FROM [dbo].[EligibleExpenseTypes] eet
