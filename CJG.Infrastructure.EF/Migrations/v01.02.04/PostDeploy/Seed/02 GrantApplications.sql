PRINT 'Updating [GrantApplications]'

-- Updating the current Grant Applications with the new reimbursement rates.
-- This can be done because no agreements have been issued.

UPDATE ga
SET [ReimbursementRate] = s.[ReimbursementRate]
FROM  [dbo].[GrantApplications] ga
INNER JOIN [dbo].[GrantOpenings] g ON ga.[GrantOpeningId] = g.[Id]
INNER JOIN [dbo].[GrantStreams] gs ON g.[GrantStreamId] = gs.[Id]
INNER JOIN [dbo].[Streams] s ON gs.[StreamId] = s.[Id]
