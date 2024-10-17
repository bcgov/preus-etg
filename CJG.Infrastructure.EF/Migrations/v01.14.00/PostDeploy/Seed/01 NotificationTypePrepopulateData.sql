PRINT 'UPDATE [NotificationTypes] - Adding new data'

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 1, PreviousApplicationState = 0, CurrentApplicationState = 1, ResendRule = 1,
ApprovalRule = 0, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Application Submitted';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 2, PreviousApplicationState = 5, CurrentApplicationState = 7, ResendRule = 1,
ApprovalRule = 1, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Offer Issued';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 3, PreviousApplicationState = null, CurrentApplicationState = 9, ResendRule = 1,
ApprovalRule = 2, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Grant Agreement Accepted';

UPDATE dbo.NotificationTypes 
SET 
MilestoneDateName = 'AgreementIssuedDate',
NotificationTemplateId = 4, PreviousApplicationState = null, CurrentApplicationState = 7, ResendRule = 0,
ApprovalRule = 0, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Accepting Agreement Deadline';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 5, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 1, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Participant Reporting Reminder - 14 days';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 5, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 1, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Participant Reporting Reminder - 5 days';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 7, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 1, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'No Participants Reported';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 8, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 4, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Training Started';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 9, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 4, ClaimReportRule = 1, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Claim Submission Reminder';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 10, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 1, ClaimReportRule = 1, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Claim Submission Expired with No Participants Reported';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 11, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 2, ClaimReportRule = 1, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Claim Submission Expired';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 12, PreviousApplicationState = null, CurrentApplicationState = 32, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 0, ClaimReportRule = 2, CompletionReportRule = 1,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Completion Report Due';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 13, PreviousApplicationState = null, CurrentApplicationState = 32, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 0, ClaimReportRule = 2, CompletionReportRule = 1,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Completion Report Deadline Passed';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 14, PreviousApplicationState = 17, CurrentApplicationState = 9, ResendRule = 1,
ApprovalRule = 2, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Change Request Approved';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 15, PreviousApplicationState = 18, CurrentApplicationState = 20, ResendRule = 1,
ApprovalRule = 2, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Change Request Denied';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 10, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 1, ClaimReportRule = 1, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Claim reporting past due with no participants reported - 60';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 10, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 1, ClaimReportRule = 1, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Claim reporting past due with no participants reported - 90';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 10, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 2, ClaimReportRule = 1, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Claim reporting past due with some participants reported - 60';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 10, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 2, ClaimReportRule = 1, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Claim reporting past due with some participants reported - 90';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 15, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 2, ClaimReportRule = 1, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Claim reporting past due with some participants reported - 90';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 16, PreviousApplicationState = null, CurrentApplicationState = 14, ResendRule = 1,
ApprovalRule = 0, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Ministry Cancelled Agreement';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 17, PreviousApplicationState = null, CurrentApplicationState = 21, ResendRule = 1,
ApprovalRule = 2, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Claim Submitted';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 18, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 1,
ApprovalRule = 2, ParticipantReportRule = 0, ClaimReportRule = 6, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Payment Requested';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 19, PreviousApplicationState = null, CurrentApplicationState = 24, ResendRule = 1,
ApprovalRule = 2, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Claim Denied';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 20, PreviousApplicationState = null, CurrentApplicationState = 23, ResendRule = 1,
ApprovalRule = 2, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Claim Returned';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 21, PreviousApplicationState = 6, CurrentApplicationState = 11, ResendRule = 1,
ApprovalRule = 1, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Application Denied';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 22, PreviousApplicationState = 7, CurrentApplicationState = 8, ResendRule = 1,
ApprovalRule = 0, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Offer Withdrawn';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 23, PreviousApplicationState = null, CurrentApplicationState = 10, ResendRule = 1,
ApprovalRule = 1, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Agreement Not Accepted - Funds Unavailable';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 24, PreviousApplicationState = 31, CurrentApplicationState = 25, ResendRule = 1,
ApprovalRule = 2, ParticipantReportRule = 0, ClaimReportRule = 0, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 1
WHERE Caption = 'Claim Approved';

UPDATE dbo.NotificationTypes 
SET 
NotificationTemplateId = 10, PreviousApplicationState = null, CurrentApplicationState = null, ResendRule = 2,
ApprovalRule = 2, ParticipantReportRule = 4, ClaimReportRule = 1, CompletionReportRule = 0,
RecipientRule = 0, DateUpdated = GETUTCDATE(), NotificationTriggerId = 2
WHERE Caption = 'Claim reporting past due with all participants reported - 30';

CHECKPOINT