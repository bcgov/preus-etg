PRINT 'Updating [ApplicationRoleClaims]'

insert into dbo.ApplicationRoleClaims (
	RoleId
	, ClaimId
) values (
	(select top 1 id from dbo.ApplicationRoles where [name] = 'Financial Clerk')
	, (select top 1 id from dbo.ApplicationClaims where [ClaimValue] = 'AM2')
)