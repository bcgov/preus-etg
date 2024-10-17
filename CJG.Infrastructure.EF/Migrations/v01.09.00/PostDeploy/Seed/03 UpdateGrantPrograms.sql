PRINT 'UPDATE [GrantPrograms]'

UPDATE [dbo].[GrantPrograms] 
SET
	ProgramConfigurationId = 1
	, ProgramTypeId = 1

UPDATE [dbo].[GrantPrograms]
SET 
	[DateImplemented] = GETUTCDATE()
WHERE [State] = 1