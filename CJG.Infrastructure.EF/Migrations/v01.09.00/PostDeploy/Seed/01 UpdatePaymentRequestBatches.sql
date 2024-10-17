PRINT 'UPDATE [PaymentRequestBatches]'

UPDATE prb
SET prb.GrantProgramId = gs.grantprogramid
FROM dbo.PaymentRequestBatches prb 
	INNER JOIN dbo.PaymentRequests pr ON prb.id = pr.PaymentRequestBatchId
	INNER JOIN dbo.GrantApplications ga ON pr.GrantApplicationId = ga.Id
	INNER JOIN dbo.GrantOpenings g ON g.Id = ga.GrantOpeningId
	INNER JOIN dbo.GrantStreams gs ON gs.id = g.GrantStreamId

UPDATE a
SET 
	a.BatchRequestDescription = b.BatchRequestDescription,
	a.ExpenseAuthorityId = b.ExpenseAuthorityId,
	a.RequestedBy = b.RequestedBy,
	a.ProgramPhone = b.ProgramPhone,
	a.DocumentPrefix = b.DocumentPrefix,
	a.ExpenseAuthorityName = c.FirstName + ' ' + c.LastName,
	a.IssuedByName = d.FirstName + ' '+ d.LastName
FROM dbo.PaymentRequestBatches a
	INNER JOIN dbo.GrantPrograms b ON a.GrantProgramId = b.Id
	INNER JOIN dbo.InternalUsers c ON b.ExpenseAuthorityId = c.Id
	INNER JOIN dbo.InternalUsers d ON d.Id = a.IssuedById