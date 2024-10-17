PRINT 'Updating [ServiceLines] - Fix EnableCost'

-- Skills Training Service Lines should allow entering costs for the breakdowns.
UPDATE sl
SET sl.EnableCost = 1
FROM dbo.[ServiceLines] sl
INNER JOIN dbo.[ServiceCategories] sc on sl.ServiceCategoryId = sc.Id
WHERE sc.ServiceTypeId = 1
