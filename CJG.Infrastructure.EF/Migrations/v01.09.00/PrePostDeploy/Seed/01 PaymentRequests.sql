PRINT 'Migrate [PaymentRequests]'

UPDATE pr
SET
	pr.GrantApplicationId = bpr.GrantApplicationId
FROM dbo.PaymentRequests pr
	INNER JOIN #PaymentRequests bpr ON pr.PaymentRequestBatchId = bpr.PaymentRequestBatchId
		AND pr.TrainingProgramId = bpr.TrainingProgramId

DROP TABLE #PaymentRequests