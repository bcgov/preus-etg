PRINT 'Updating [GrantApplicationInternalStates]'

-- Remove Internal State for ExpectingClaimAmendment as part of 131 - Claim Creation.
IF (EXISTS (SELECT TOP 1 * FROM [dbo].[GrantApplicationInternalStates] WHERE [Id] = 26))
BEGIN
	DELETE FROM [dbo].[GrantApplicationInternalStates]
	WHERE [Id] = 26
END

-- Updating Internal State for ClaimAssessEligibility as part of 266 - Claim Assessment.
IF (EXISTS (SELECT TOP 1 * FROM [dbo].[GrantApplicationInternalStates] WHERE [Id] = 22))
BEGIN
	UPDATE [dbo].[GrantApplicationInternalStates]
	SET [Caption] = N'Claim Assess Eligibility',
		[Description] = N'An Assessor has selected a claim for eligibility assessment and is assigned to the grant file as the assessor. 
			The external state is "Claim Submitted"'
	WHERE [Id] = 22
END

-- Adding Internal State for ClaimAssessReimbursement as part of 266 - Claim Assessment.
IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[GrantApplicationInternalStates] WHERE [Id] = 31))
BEGIN
	SET IDENTITY_INSERT [dbo].[GrantApplicationInternalStates] ON 
	INSERT [dbo].[GrantApplicationInternalStates]
		([Id], [Caption], [Description], [IsActive], [RowSequence]) VALUES
		(31, N'Claim Assess Reimbursement', N'An Assessor has selected a claim for reimbursement assessmennt and is assigned to the grant file as the assessor. 
			The external state is "Claim Submitted"', 1, 0)
	SET IDENTITY_INSERT [dbo].[GrantApplicationInternalStates] OFF
END