PRINT 'Updating Grant Application Completion report type for existing applications'

-- Set the completion report ID to Completion report 2 if:
-- This is a CWRG grant application (check if GrantPrograms.ProgramCode = 'CWRG')
-- but those are not Closed in state form.
-- Delivery End date is >= April 1, 2021

IF (EXISTS (SELECT * FROM [dbo].[CompletionReports] WHERE Id = 2))
BEGIN
	PRINT 'START Updating...'

	UPDATE GA
	SET GA.[CompletionReportId] = 2,
		GA.[DateUpdated] = GETUTCDATE()
	FROM [dbo].[GrantApplications] GA
	join [dbo].[GrantOpenings] GRO on ga.GrantOpeningId = GRO.Id
	join [dbo].[GrantStreams] GS on GRO.GrantStreamId = GS.Id
	join [dbo].[GrantPrograms] GP on GS.GrantProgramId = GP.Id
	WHERE GP.ProgramCode = 'CWRG'
	AND GA.ApplicationStateExternal <> 30
	AND GA.EndDate >= '2021-04-01'

END

PRINT 'Completed Updating Grant Application Completion report type for existing applications'