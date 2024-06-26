PRINT 'Inserting [ApplicationRoleClaims]'
INSERT INTO dbo.ApplicationRoleClaims (ClaimId, RoleId)
 SELECT ac.ID, (SELECT TOP 1 Id FROM dbo.ApplicationRoles ar WHERE ar.Name = 'Director')                FROM dbo.ApplicationClaims ac WHERE ac.ClaimType = 'Privilege' AND ac.ClaimValue <> 'SM'

INSERT INTO dbo.ApplicationRoleClaims (ClaimId, RoleId)
 SELECT ac.ID, (SELECT TOP 1 Id FROM dbo.ApplicationRoles ar WHERE ar.Name = 'System Administrator')    FROM dbo.ApplicationClaims ac WHERE ac.ClaimType = 'Privilege'

INSERT INTO dbo.ApplicationRoleClaims (ClaimId, RoleId)
 SELECT ac.ID, (SELECT TOP 1 Id FROM dbo.ApplicationRoles ar WHERE ar.Name = 'Operations Manager')      FROM dbo.ApplicationClaims ac WHERE ac.ClaimType = 'Privilege' AND ac.ClaimValue IN ('IA1', 'IA2', 'IA4', 'IA5', 'UM1', 'PM1', 'PM2', 'TP1')

INSERT INTO dbo.ApplicationRoleClaims (ClaimId, RoleId)
 SELECT ac.ID, (SELECT TOP 1 Id FROM dbo.ApplicationRoles ar WHERE ar.Name = 'Assessor')                FROM dbo.ApplicationClaims ac WHERE ac.ClaimType = 'Privilege' AND ac.ClaimValue IN ('IA1', 'IA4', 'AM1', 'AM2', 'AM5', 'PM1', 'TP1')

INSERT INTO dbo.ApplicationRoleClaims (ClaimId, RoleId)
 SELECT ac.ID, (SELECT TOP 1 Id FROM dbo.ApplicationRoles ar WHERE ar.Name = 'Financial Administrator') FROM dbo.ApplicationClaims ac WHERE ac.ClaimType = 'Privilege' AND ac.ClaimValue IN ('IA1', 'PM1', 'PM2')

INSERT INTO dbo.ApplicationRoleClaims (ClaimId, RoleId)
 SELECT ac.ID, (SELECT TOP 1 Id FROM dbo.ApplicationRoles ar WHERE ar.Name = 'Director Of Finance')     FROM dbo.ApplicationClaims ac WHERE ac.ClaimType = 'Privilege' AND ac.ClaimValue IN ('IA1', 'PM1', 'PM2')