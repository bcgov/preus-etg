PRINT 'Inserting [GrantOpeningIntakes]'

DECLARE @Incrementor INT

SET @Incrementor = 1

WHILE @Incrementor <= 24
BEGIN
	INSERT [dbo].[GrantOpeningIntakes]
		([GrantOpeningId], [NewCount], [NewAmt], [PendingAssessmentCount], [PendingAssessmentAmt], [UnderAssessmentCount], [UnderAssessmentAmt],
		[DeniedCount], [DeniedAmt], [WithdrawnCount], [WithdrawnAmt], [ReductionsCount], [ReductionsAmt], [CommitmentCount], [CommitmentAmt]) VALUES
		(@Incrementor, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

	SET @Incrementor = @Incrementor + 1
END