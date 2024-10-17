PRINT 'Updating [GrantApplications] - Fix Participant Reporting'

-- Participant reporting should be enabled after agreement accepted and before claim submital.
UPDATE dbo.[GrantApplications]
SET CanReportParticipants = 1
WHERE ApplicationStateInternal IN (9, 16, 17, 18, 19, 20, 24, 25)
AND InvitationKey IS NOT NULL

-- We no longer expire PIF invitations at the end of delivery.
UPDATE dbo.[GrantApplications]
SET InvitationExpiresOn = NULL
WHERE InvitationKey IS NOT NULL
