PRINT 'Updating [NotificationTemplates]'

UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = REPLACE([EmailBody], 'isPayment', 'IsPayment')
WHERE [EmailBody] LIKE '%isPayment%'

-- Fixing syntax error that occurs when parsing the Razor syntax. Hopefully for the last time.
UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = REPLACE([EmailBody], '@(Model.ProgramCode)@("@gov.bc.ca")', '@Model.ProgramEmail')
WHERE [EmailBody] LIKE '%@(Model.ProgramCode)@%'

UPDATE dbo.[NotificationTemplates]
SET	[EmailBody] = REPLACE([EmailBody], '@(Model.ProgramCode)@gov.bc.ca', '@Model.ProgramEmail')
WHERE [EmailBody] LIKE '%@(Model.ProgramCode)@%'

-- Replace variable names
UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = REPLACE([EmailBody], '@Model.ApplicationAdministratorAddress', '@Raw(Model.ApplicantAddress)')
WHERE [EmailBody] LIKE '%@Model.ApplicationAdministratorAddress%'

-- Fix case issue
UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = REPLACE([EmailBody], 'Model.NumberofParticipants', 'Model.NumberOfParticipants')
WHERE [EmailBody] LIKE '%Model.NumberofParticipants%'

-- Fix case issue
UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = REPLACE([EmailBody], 'Model.MaximumNumberofParticipants', 'Model.MaximumNumberOfParticipants')
WHERE [EmailBody] LIKE '%Model.MaximumNumberofParticipants%'

-- Fix wrong keyword
UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = REPLACE([EmailBody], 'Model.ApplicationAdministratorAddress', 'Model.ApplicantAddress')
WHERE [EmailBody] LIKE '%Model.ApplicationAdministratorAddress%'

-- Fix wrong keyword
UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = REPLACE([EmailBody], 'Model.ReimbursementClaimDeniedReason', 'Model.ClaimDeniedReason')
WHERE [EmailBody] LIKE '%Model.ReimbursementClaimDeniedReason%'

-- Fix wrong keyword
UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = REPLACE([EmailBody], 'Model.ReimbursementClaimReturnedReason', 'Model.ClaimReturnedReason')
WHERE [EmailBody] LIKE '%Model.ReimbursementClaimReturnedReason%'

-- Fix wrong keyword
UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = REPLACE([EmailBody], 'Model.ReimbursementClaimApprovedReason', 'Model.ClaimApprovedReason')
WHERE [EmailBody] LIKE '%Model.ReimbursementClaimApprovedReason%'

-- Fix wrong keyword
UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = REPLACE([EmailBody], 'Model.CRDeniedReason', 'Model.ChangeRequestResults')
WHERE [EmailBody] LIKE '%Model.CRDeniedReason%'

-- Fix wrong keyword
UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = REPLACE([EmailBody], 'Model.CRReason', 'Model.ChangeRequestResults')
WHERE [EmailBody] LIKE '%Model.CRReason%'

-- Update Change Request Denied Template
UPDATE dbo.[NotificationTemplates]
SET [EmailBody] = '<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p>Your training provider change request for Grant Agreement #@Model.FileNumber, "@Model.TrainingProgramTitle", has been denied for the following reason:</p><p>@Model.ChangeRequestDeniedReason </p>@Raw(Model.ChangeRequestResults)<p>You may Login at: <a href="@Model.BaseURL">@Model.BaseURL</a> to submit another request.</p><p>@Model.ProgramName Team</p></body></html>'
WHERE [Id] = 15

CHECKPOINT