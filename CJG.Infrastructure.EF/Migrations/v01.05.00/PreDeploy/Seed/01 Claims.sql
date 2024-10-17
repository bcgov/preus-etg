PRINT 'Deleting [Claims]'
-- Clearing all claims since there shouldn't have been any created in the first place.
-- We do this because the DACPAC will fail to update these tables if there is data in them.
DELETE FROM [dbo].[ClaimReceipts]
DELETE FROM [dbo].[ParticipantCosts]
DELETE FROM [dbo].[ClaimEligibleCosts]
DELETE FROM [dbo].[Claims]