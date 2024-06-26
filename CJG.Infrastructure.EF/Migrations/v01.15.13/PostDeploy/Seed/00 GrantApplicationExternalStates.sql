PRINT 'Inserting [GrantApplicationExternalStates]'

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[GrantApplicationExternalStates] WHERE [Id] = 28))
BEGIN
	SET IDENTITY_INSERT [dbo].[GrantApplicationExternalStates] ON
	INSERT [dbo].[GrantApplicationExternalStates]
	 ([Id], [Caption], [Description], [IsActive], [RowSequence], [DateAdded]) VALUES
	 ( 28, N'Returned to Applicant Unassessed', N' This application will be removed from the queue and applicant will receive the "Returned to Applicant Unassessed" notification. Application details will remain in the system but will not affect the budget.', 1, 0, GETDATE())
	SET IDENTITY_INSERT [dbo].[GrantApplicationExternalStates] OFF
END

PRINT 'DONE Inserting [GrantApplicationExternalStates]'