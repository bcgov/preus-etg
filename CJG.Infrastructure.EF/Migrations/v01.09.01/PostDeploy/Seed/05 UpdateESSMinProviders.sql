PRINT 'UPDATE ESS [ServiceCategories] AND  [EligibleExpenseTypes]'

UPDATE ServiceCategories
SET MinProviders= 0
WHERE id=1

UPDATE EligibleExpenseTypes
Set MinProviders = 0
where id = 10