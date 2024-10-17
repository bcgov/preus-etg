PRINT 'Updating [RateFormats]'

-- Updating the calculated values with the updated reimbursement rates.

UPDATE [dbo].[RateFormats]
SET [Rate] = 0.666666666666667
WHERE [Format] = '2/3'
