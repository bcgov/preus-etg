PRINT 'Update [PaymentRequestBatches]'

UPDATE dbo.PaymentRequestBatches
SET GrantProgramId = 1
WHERE GrantProgramId IS NULL

