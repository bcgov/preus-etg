PRINT 'Inserting [GrantOpeningFinancials]'

DECLARE @Incrementor INT

SET @Incrementor = 1

WHILE @Incrementor <= 24
BEGIN
	INSERT [dbo].[GrantOpeningFinancials]
		([GrantOpeningId], [CurrentReservations], [AssessedCommitments], [OutstandingCommitments], [Cancellations], [ClaimsReceived], [ClaimsAssessed], [CurrentClaims], [ClaimsDenied], [PaymentRequests]) VALUES
		(@Incrementor, 0, 0, 0, 0, 0, 0, 0, 0, 0)

	SET @Incrementor = @Incrementor + 1
END