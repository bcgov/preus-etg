PRINT 'Updating [RateFormats]'

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = 1))
BEGIN
	-- Adding missed reimbursement rates format (4629).
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (1, '100 %')
END