PRINT 'Backup [PaymentRequests]'

SELECT 
	p.PaymentRequestBatchId
	, p.TrainingProgramId
	, tp.GrantApplicationId
INTO #PaymentRequests
FROM dbo.PaymentRequests p
	INNER JOIN dbo.TrainingPrograms tp ON p.TrainingProgramId = tp.Id