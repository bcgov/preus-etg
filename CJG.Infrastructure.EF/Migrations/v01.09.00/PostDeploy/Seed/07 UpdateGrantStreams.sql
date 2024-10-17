PRINT 'Update [GrantStreams]'

UPDATE dbo.GrantStreams
SET
	ProgramConfigurationId = CASE WHEN GrantProgramId = 3 THEN 2 ELSE 1 END