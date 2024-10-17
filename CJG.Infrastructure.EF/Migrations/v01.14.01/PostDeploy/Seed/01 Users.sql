PRINT 'Updating [Users] - Profile Administrators';

-- Ensure all organizations have at least one profile administrator.
UPDATE u
SET u.[IsOrganizationProfileAdministrator] = 1
FROM dbo.[Users] u
INNER JOIN (	
	SELECT
		u.Id
		, ROW_NUMBER() OVER (PARTITION BY u.OrganizationId ORDER BY u.Id) AS rn
		, u.OrganizationId
	FROM dbo.[Users] u
	WHERE u.OrganizationId IN (
		SELECT
			DISTINCT o.Id
		FROM dbo.Organizations o
		JOIN dbo.Users u ON o.Id = u.OrganizationId
		where o.Id NOT IN (
			SELECT
				o.Id
			FROM dbo.Organizations o
			JOIN dbo.Users u ON o.Id = u.OrganizationId
			GROUP BY o.Id, u.IsOrganizationProfileAdministrator
			HAVING u.IsOrganizationProfileAdministrator = 1 -- Find all organizations that have at least one profile administrator.
		)
	)
) c ON u.Id = c.Id
WHERE c.rn = 1