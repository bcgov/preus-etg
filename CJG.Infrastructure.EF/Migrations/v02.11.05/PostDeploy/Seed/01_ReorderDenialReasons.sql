PRINT 'Reorder denial reasons alphabetically'

UPDATE DenialReasons
SET RowSequence = dr.NewRowSequence,
    DateUpdated = GETUTCDATE()
FROM (
      SELECT Id AS DenialReasonId, RowSequence, ROW_NUMBER() OVER (ORDER BY Caption) AS NewRowSequence
      FROM DenialReasons
      WHERE GrantProgramId = 2 -- ETG Only
      ) dr
WHERE Id = dr.DenialReasonId
