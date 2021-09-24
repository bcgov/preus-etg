PRINT 'Inserting [FiscalYears]'
SET IDENTITY_INSERT [dbo].[FiscalYears] ON

DECLARE @Incrementor INT
DECLARE @IntValue INT

SET @Incrementor = 0

WHILE @Incrementor <= 20
BEGIN
	INSERT [dbo].[FiscalYears]
	 ([Id], [Caption], [NextAgreementNumber], [StartDate], [EndDate]) VALUES
	 (@Incrementor + 1,
	 CAST(DATEPART(YEAR, GETUTCDATE()) + (@Incrementor - 1) AS VARCHAR) + '/' + CAST(DATEPART(YEAR, GETUTCDATE()) + (@Incrementor) AS VARCHAR), 
	 50000,
	 CAST(CAST((DATEPART(YEAR, GETUTCDATE()) + @Incrementor - 1) AS VARCHAR) + N'-04-01T07:00:00.000' AS DateTime), 
	 CAST(CAST((DATEPART(YEAR, GETUTCDATE()) + @Incrementor) AS VARCHAR) + N'-03-31T07:00:00.000' AS DateTime))

	 SET @Incrementor = @Incrementor + 1
END
SET IDENTITY_INSERT [dbo].[FiscalYears] OFF
