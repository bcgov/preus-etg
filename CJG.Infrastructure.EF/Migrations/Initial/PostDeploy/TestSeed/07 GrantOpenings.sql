PRINT 'Inserting [GrantOpenings]'

DECLARE @Incrementor INT
DECLARE @IntValue INT

SET @Incrementor = 1

DECLARE @PublishDate AS DATETIME
DECLARE @OpeningDate AS DATETIME
DECLARE @ClosingDate AS DATETIME
DECLARE @GrantOpeningState AS INT

-- Get the Training Period Id for the year.
SET @IntValue = (SELECT TOP 1 [Id] FROM [dbo].[TrainingPeriods] WHERE DATEPART(YEAR, [StartDate]) = DATEPART(YEAR, GETDATE()) + @Incrementor - 1 ORDER BY [StartDate])

-- Add Grant Openings for the current year and the next.
WHILE @Incrementor <= 2
BEGIN
	-------------------------------------------------
	-- First Training Period
	SET @PublishDate = (SELECT DATEADD(MONTH, -5, [StartDate]) FROM [dbo].[TrainingPeriods] WHERE [Id] = @IntValue)
	SET @OpeningDate = (SELECT DATEADD(MONTH, -4, [StartDate]) FROM [dbo].[TrainingPeriods] WHERE [Id] = @IntValue)
	SET @ClosingDate = (SELECT [EndDate] FROM [dbo].[TrainingPeriods] WHERE [Id] = @IntValue)

	SET @GrantOpeningState = (SELECT CASE WHEN @ClosingDate <= GETUTCDATE() THEN 4 WHEN @OpeningDate <= GETUTCDATE() THEN 3 WHEN @PublishDate <= GETUTCDATE() THEN 2 ELSE 0 END)
	
	INSERT [dbo].[GrantOpenings]
	 ([PublishDate],	[OpeningDate],	[ClosingDate],	[TrainingPeriodId],		[GrantStreamId],	[State],					[IntakeTargetAmt], [PlanDeniedRate],	[PlanWithdrawnRate],	[PlanReductionRate],	[PlanSlippageRate], [PlanCancellationRate], [BudgetAllocationAmt]) VALUES
	 (@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue,				1,					@GrantOpeningState,			3200000,			0.041,				0.044,					0.035,					0.126,				0.034,					2500000)
	,(@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue,				2,					@GrantOpeningState,			6440000,			0.031,				0.043,					0.045,					0.136,				0.033,					5000000)
	,(@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue,				3,					@GrantOpeningState,			6440000,			0.031,				0.043,					0.045,					0.136,				0.033,					5000000)
	,(@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue,				4,					@GrantOpeningState,			0161000,			0.031,				0.043,					0.045,					0.136,				0.033,					0125000)
	
	-------------------------------------------------
	-- Second Training Period
	SET @PublishDate = (SELECT DATEADD(MONTH, -5, [StartDate]) FROM [dbo].[TrainingPeriods] WHERE [Id] = @IntValue + 1)
	SET @OpeningDate = (SELECT DATEADD(MONTH, -4, [StartDate]) FROM [dbo].[TrainingPeriods] WHERE [Id] = @IntValue + 1)
	SET @ClosingDate = (SELECT [EndDate] FROM [dbo].[TrainingPeriods] WHERE [Id] = @IntValue + 1)
	
	SET @GrantOpeningState = (SELECT CASE WHEN @ClosingDate <= GETUTCDATE() THEN 4 WHEN @OpeningDate <= GETUTCDATE() THEN 3 WHEN @PublishDate <= GETUTCDATE() THEN 2 ELSE 0 END)
	
	INSERT [dbo].[GrantOpenings]
	 ([PublishDate],	[OpeningDate],	[ClosingDate],	[TrainingPeriodId],		[GrantStreamId],	[State],					[IntakeTargetAmt], [PlanDeniedRate],	[PlanWithdrawnRate],	[PlanReductionRate],	[PlanSlippageRate], [PlanCancellationRate], [BudgetAllocationAmt]) VALUES
	 (@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue + 1,			1,					@GrantOpeningState,			3200000,			0.041,				0.044,					0.035,					0.126,				0.034,					2500000)
	,(@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue + 1,			2,					@GrantOpeningState,			6440000,			0.031,				0.043,					0.045,					0.136,				0.033,					5000000)
	,(@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue + 1,			3,					@GrantOpeningState,			6440000,			0.031,				0.043,					0.045,					0.136,				0.033,					5000000)
	,(@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue + 1,			4,					@GrantOpeningState,			0161000,			0.031,				0.043,					0.045,					0.136,				0.033,					0125000)

	-------------------------------------------------
	-- Third Training Period
	SET @PublishDate = (SELECT DATEADD(MONTH, -5, [StartDate]) FROM [dbo].[TrainingPeriods] WHERE [Id] = @IntValue + 2)
	SET @OpeningDate = (SELECT DATEADD(MONTH, -4, [StartDate]) FROM [dbo].[TrainingPeriods] WHERE [Id] = @IntValue + 2)
	SET @ClosingDate = (SELECT [EndDate] FROM [dbo].[TrainingPeriods] WHERE [Id] = @IntValue + 2)
	
	SET @GrantOpeningState = (SELECT CASE WHEN @ClosingDate <= GETUTCDATE() THEN 4 WHEN @OpeningDate <= GETUTCDATE() THEN 3 WHEN @PublishDate <= GETUTCDATE() THEN 2 ELSE 0 END)
	
	INSERT [dbo].[GrantOpenings]
	 ([PublishDate],	[OpeningDate],	[ClosingDate],	[TrainingPeriodId],		[GrantStreamId],	[State],					[IntakeTargetAmt], [PlanDeniedRate],	[PlanWithdrawnRate],	[PlanReductionRate],	[PlanSlippageRate], [PlanCancellationRate], [BudgetAllocationAmt]) VALUES
	 (@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue + 2,			1,					@GrantOpeningState,			3200000,			0.041,				0.044,					0.035,					0.126,				0.034,					2500000)
	,(@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue + 2,			2,					@GrantOpeningState,			6440000,			0.031,				0.043,					0.045,					0.136,				0.033,					5000000)
	,(@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue + 2,			3,					@GrantOpeningState,			6440000,			0.031,				0.043,					0.045,					0.136,				0.033,					5000000)
	,(@PublishDate,		@OpeningDate,	@ClosingDate,	@IntValue + 2,			4,					@GrantOpeningState,			0161000,			0.031,				0.043,					0.045,					0.136,				0.033,					0125000)
	-------------------------------------------------

	SET @IntValue = @IntValue + 3
	SET @Incrementor = @Incrementor + 1
END