PRINT 'Updating [RateFormats]'

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .05))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.05, N'  5 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .10))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.10, N' 10 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .15))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.15, N' 15 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .20))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.20, N' 20 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .25))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.25, N' 25 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .30))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.30, N' 30 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .333333333333333))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.333333333333333, N'1/3')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .35))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.35, N' 35 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .40))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.40, N' 40 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .45))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.45, N' 45 %')
END
IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .50))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.50, N' 50 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .55))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.55, N' 55 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .60))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.60, N' 60 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .65))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.65, N' 65 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .70))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.70, N' 70 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .75))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.75, N' 75 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .80))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.80, N' 80 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .85))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.85, N' 85 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .90))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.90, N' 90 %')
END

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[RateFormats] WHERE [Rate] = .95))
BEGIN
	-- Adding missed reimbursement rates format
	INSERT INTO [dbo].[RateFormats] ([Rate], [Format])
	VALUES (.95, N' 95 %')
END

