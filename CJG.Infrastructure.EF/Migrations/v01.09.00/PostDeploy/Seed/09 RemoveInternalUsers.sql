PRINT 'DELETE [InternalUsers]'

DELETE FROM dbo.[AspNetUserRoles]
WHERE UserId IN (SELECT
    a.ApplicationUserId
FROM dbo.[AspNetUsers] AS a
	JOIN dbo.[InternalUsers] AS b ON a.InternalUserId = b.Id
WHERE b.IDIR in ('mamason','alamping','shsaunde','dpenfold','RMATSUMO','gpovoroz'))

DELETE a
FROM dbo.[AspNetUsers] AS a
  JOIN dbo.[InternalUsers] AS b ON a.InternalUserId = b.Id
WHERE b.IDIR in ('mamason','alamping','shsaunde','dpenfold','RMATSUMO','gpovoroz')

DELETE FROM dbo.[InternalUsers]
WHERE IDIR in ('mamason','alamping','shsaunde','dpenfold','RMATSUMO','gpovoroz')