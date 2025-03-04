PRINT 'Inserting [GrantApplicationInternalStates]'

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[GrantApplicationInternalStates] WHERE [Id] = 28))
BEGIN
	SET IDENTITY_INSERT [dbo].[GrantApplicationInternalStates] ON
	INSERT [dbo].[GrantApplicationInternalStates]
	 ([Id], [Caption], [Description], [IsActive], [RowSequence], [DateAdded]) VALUES
	 ( 28, N'Returned to Applicant Unassessed', N' This application will be removed from the queue and applicant will receive the "Returned to Applicant Unassessed" notification. Application details will remain in the system but will not affect the budget.  The external state is "Returned to Applicant Unassessed"', 1, 0, GETDATE())
	SET IDENTITY_INSERT [dbo].[GrantApplicationInternalStates] OFF
END

PRINT 'DONE Inserting [GrantApplicationInternalStates]'