PRINT 'UPDATE [PaymentRequestBatchees]'

DECLARE @lastNumber INT

SELECT  TOP 1 @lastNumber = convert(INT, BatchNumber) FROM PaymentRequestBatches ORDER BY id DESC

IF @lastNumber >= 9999
BEGIN
	SET	@lastNumber = 1
END

SELECT RIGHT('0000'+CAST( @lastNumber + ROW_NUMBER() OVER( ORDER BY ID ASC) AS VARCHAR(4)),4) AS 'BatchNumber', Id
INTO #NewBatchNumbers
FROM PaymentRequestBatches WHERE (BatchNumber = '' or BatchNumber is null)

UPDATE a
SET a.BatchNumber = b.BatchNumber
FROM PaymentRequestBatches a 
JOIN #NewBatchNumbers b ON a.Id = b.Id

DROP TABLE #NewBatchNumbers