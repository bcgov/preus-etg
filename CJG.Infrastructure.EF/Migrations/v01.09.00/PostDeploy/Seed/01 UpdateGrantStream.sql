PRINT 'UPDATE [GrantStreams]'

UPDATE GrantStreams 
SET GrantProgramId = 1 
WHERE GrantProgramId IS NULL

UPDATE a
SET a.accountcodeid = b.accountcodeid
FROM GrantStreams a 
	INNER JOIN GrantPrograms b on a.GrantProgramId = b.Id
WHERE a.AccountCodeId IS NULL