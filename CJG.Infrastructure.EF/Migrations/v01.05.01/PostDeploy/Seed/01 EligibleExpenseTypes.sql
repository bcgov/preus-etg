PRINT 'Updating [EligibleExpenseTypes]'
UPDATE eet
SET eet.[Description] = eet.[Caption]
FROM [dbo].[EligibleExpenseTypes] eet

UPDATE [dbo].[EligibleExpenseTypes]
SET [Description] = N'Tuition fees or fees charged by a third party trainer'
WHERE [Id] = 1
