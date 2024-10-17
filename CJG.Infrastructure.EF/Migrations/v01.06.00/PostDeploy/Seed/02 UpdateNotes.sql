PRINT 'Updating [Notes]'

insert into dbo.Notes (
		NoteTypeId
		, GrantApplicationId
		, CreatorId
		, Content
		, DateAdded
	)
select distinct 
	15
	, i.GrantApplicationId
	, null
	, 'Your claimed expenses have been adjusted for a system error that may have allowed you to report expenses in your claim that: were higher than what your agreement allow, not approved in your agreement, or rounded your participant assigned expenses causing your claim to exceed your agreement amount.  These expenses have been removed or adjusted in your claim to align to your agreement.'
	, GETUTCDATE()
from (
	select distinct
		i.GrantApplicationId
	from dbo._OriginalData i
	) as i

DECLARE @NewLineChar AS CHAR(2) = CHAR(13) + CHAR(10)

UPDATE c
SET c.ClaimAssessmentNotes = ISNULL(c.ClaimAssessmentNotes, '') + @NewLineChar + 'Your claimed expenses have been adjusted for a system error that may have allowed you to report expenses in your claim that: were higher than what your agreement allow, not approved in your agreement, or rounded your participant assigned expenses causing your claim to exceed your agreement amount.  These expenses have been removed or adjusted in your claim to align to your agreement.'
FROM dbo.Claims c
INNER JOIN (
	SELECT DISTINCT ClaimId
		, ClaimVersion
	FROM dbo._OriginalData
	) i ON c.Id = i.ClaimId AND c.ClaimVersion = i.ClaimVersion