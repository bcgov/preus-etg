PRINT 'Delete Unused [GrantStreams]'

-- If there are no GrantOpenings associated with the GrantStream it can be deleted.
DELETE gs
FROM dbo.[GrantStreams] gs
LEFT JOIN dbo.[GrantOpenings] g ON gs.[Id] = g.[GrantStreamId]
WHERE g.[Id] IS NULL