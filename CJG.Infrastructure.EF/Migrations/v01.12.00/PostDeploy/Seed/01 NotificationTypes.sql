PRINT 'Update [NotificationTypes] - Change caption and description'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Application Submitted'
	, [Description] = 'Confirmation that the Grant Application has been submitted. '
WHERE [Caption] = 'InsSubmittionAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Offer Issued'
	, [Description] = 'Notification that the agreement must be accepted within 5 days of the Offer Issued date. '
WHERE [Caption] = 'InsIssuedOfferAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Grant Agreement Accepted'
	, [Description] = 'Confirmation that the Grant Application Agreement has been accepted. '
WHERE [Caption] = 'InsGrantAcceptedAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Accepting Agreement Deadline'
	, [Description] = 'The Grant Application Agreement has not yet been accepted and is 1 day past the Agreement Acceptance Date.'
WHERE [Caption] = 'SchAcceptingDeadlineAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Participant Reporting Reminder - 14 days'
	, [Description] = 'Participant reporting reminder date.  Notification sent to Application Administrator.'
WHERE [Caption] = 'SchBeforeTraining14DaysAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Participant Reporting Reminder - 5 days'
	, [Description] = 'Participant reporting due date.  Notification sent to Application Administrator.'
WHERE [Caption] = 'SchBeforeTraining5DaysAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'No Participants Reported'
	, [Description] = 'The training period has begun and no participants have been reported. '
WHERE [Caption] = 'SchAfterTraining1DayNoPartAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Training Started'
	, [Description] = 'The training period has begun and some or all participants have been reported.  May begin preparing a Claim. '
WHERE [Caption] = 'SchAfterTraining1DayHasPartAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Claim Submission Reminder'
	, [Description] = 'Claim submission reminder is sent 14 days after training starts. '
WHERE [Caption] = 'SchAfterTraining14DaysAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Claim Submission Expired with No Participants Reported'
	, [Description] = 'Claim submission due date has expired (31 days after start date) and no participants have been reported. '
WHERE [Caption] = 'SchAfterTraining31DaysNoPartAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Claim Submission Expired'
	, [Description] = 'Claim submission due date has expired (31 days after start date) and some or all participants have been reported. '
WHERE [Caption] = 'SchAfterTraining31DaysHasPartAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Completion Report Due'
	, [Description] = 'Training end date has passed, completion report is due. '
WHERE [Caption] = 'SchAfterTrainingEnd1DayAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Completion Report Deadline Passed'
	, [Description] = 'Completion reporting due date deadline has passed.  Notification sent to Application Administrator.'
WHERE [Caption] = 'SchAfterTrainingEnd31DayNoRepAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Change Request Approved'
	, [Description] = 'The change request for Training Provider has been approved.  Notification sent to Application Administrator.'
WHERE [Caption] = 'InsChangeTrainingProviderApproveAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Change Request Denied'
	, [Description] = 'The change request for Training Provider has been denied. '
WHERE [Caption] = 'InsChangeTrainingProviderDenyAdmin'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Ministry Cancelled Agreement'
	, [Description] = 'When the ministry cancels an accepted Grant Application Agreement.  Notification sent to the Application Administrator'
WHERE [Caption] = 'InsCancelledByMinsitry'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Claim Submitted'
	, [Description] = 'Notification that the Claim has been successfully submitted. '
WHERE [Caption] = 'InsClaimSubmitted'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Payment Requested'
	, [Description] = 'Notification that a paymentment has been requested (paid or reimbursed). '
WHERE [Caption] = 'InsPaymentRequested'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Claim Denied'
	, [Description] = 'The Claim has been denied. '
WHERE [Caption] = 'InsClaimDenied'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Claim Returned'
	, [Description] = 'The Claim has been returned for the specified reasons. '
WHERE [Caption] = 'InsClaimReturned'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Application Denied'
	, [Description] = 'The Grant Application has been denied. '
WHERE [Caption] = 'InsApplicationDenied'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Offer Withdrawn'
	, [Description] = 'The Grant Application Agreement has been withdrawn before being accepted. '
WHERE [Caption] = 'InsOfferWithdrawn'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Agreement Not Accepted - Funds Unavailable'
	, [Description] = 'The Grant Application was not able to be accepted because funding are no longer available. '
WHERE [Caption] = 'InsNotAccepted'

UPDATE dbo.[NotificationTypes]
SET [Caption] = 'Claim Approved'
	, [Description] = 'The Claim has been approved.'
WHERE [Caption] = 'InsClaimApproved'

-- Fix spelling mistake.
UPDATE dbo.[NotificationTypes]
SET [MilestoneDateName] = 'ClaimSubmitted'
WHERE [MilestoneDateName] = 'ClaimSumbitted'