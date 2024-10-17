PRINT 'Updating [Streams]'

-- Increasing the precision of the reimbursement rates to handle values in the hundreds of thousands of dollars.

UPDATE [dbo].[Streams]
SET [ReimbursementRate] = 0.666666666666667
WHERE ABS([ReimbursementRate] - 0.6666667) < 0.00001
	OR ABS([ReimbursementRate] - 0.666666686534882) < 0.00001 -- When changing the float to a double it appears to have automatically changed the value.