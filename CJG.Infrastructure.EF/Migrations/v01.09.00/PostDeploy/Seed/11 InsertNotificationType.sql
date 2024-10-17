PRINT 'INSERT [NotificationType] - CWRG'

INSERT [dbo].[NotificationTypes] (
	[Id], [NotificationTemplateId], [MilestoneDateName], [MilestoneDateOffset], [NotificationTypeName], [RecipientType], [DateAdded], [DateUpdated], [Description], [IsActive]
) VALUES 
	  (29, 25, N'SubmittedDate', 0, N'InsSubmittionAdmin', 0, GETUTCDATE(), NULL, NULL, 1)
	, (30, 26, N'OfferIssueDate', 0, N'InsIssuedOfferAdmin', 0, GETUTCDATE(), NULL, NULL, 1)
	, (31, 27, N'GrantAcceptedDate', 0, N'InsGrantAcceptedAdmin', 0, GETUTCDATE(), NULL, NULL, 1)
	, (32, 28, N'OfferIssueDate', 5, N'SchAcceptingDeadlineAdmin', 0, GETUTCDATE(), NULL, NULL, 1)
	, (33, 29, N'TrainingStartDate', -14, N'SchBeforeTraining14DaysAdmin', 0, GETUTCDATE(), NULL, NULL, 0)
	, (34, 30, N'TrainingStartDate', -5, N'SchBeforeTraining5DaysAdmin', 0, GETUTCDATE(), NULL, NULL, 0)
	, (35, 31, N'TrainingStartDate', 1, N'SchAfterTraining1DayNoPartAdmin', 0, GETUTCDATE(), NULL, NULL, 1)
	, (36, 32, N'TrainingEndDate', 1, N'SchAfterTrainingEnd1DayAdmin', 0, GETUTCDATE(), NULL, NULL, 1)
	, (37, 33, N'TrainingEndDate', 31, N'SchAfterTrainingEnd31DayNoRepAdmin', 0, GETUTCDATE(), NULL, NULL, 1)
	, (38, 34, N'ChangeTrainingProviderApprove', 0, N'InsChangeTrainingProviderApproveAdmin', 0, GETUTCDATE(), NULL, NULL, 1)
	, (39, 35, N'ChangeTrainingProviderDeny', 0, N'InsChangeTrainingProviderDenyAdmin', 0, GETUTCDATE(), NULL, NULL, 1)
	, (40, 29, N'TrainingStartDate', -14, N'SchBeforeTraining14DaysEmp', 1, GETUTCDATE(), NULL, NULL, 0)
	, (41, 30, N'TrainingStartDate', -5, N'SchBeforeTraining5DaysEmp', 1, GETUTCDATE(), NULL, NULL, 0)
	, (42, 32, N'TrainingEndDate', 1, N'SchAfterTrainingEnd1DayEmp', 1, GETUTCDATE(), NULL, NULL, 1)
	, (43, 33, N'TrainingEndDate', 31, N'SchAfterTrainingEnd31DayNoRepEmp', 1, GETUTCDATE(), NULL, NULL, 1)
	, (44, 36, N'DateCancelled', 0, N'InsCancelledByMinsitry', 0, GETUTCDATE(), NULL, NULL, 1)
	, (45, 37, N'ClaimSumbitted', 0, N'InsClaimSubmitted', 0, GETUTCDATE(), NULL, NULL, 1)
	, (46, 38, N'PaymentRequested', 0, N'InsPaymentRequested', 0, GETUTCDATE(), NULL, NULL, 1)
	, (47, 39, N'ClaimDenied', 0, N'InsClaimDenied', 0, GETUTCDATE(), NULL, NULL, 1)
	, (48, 40, N'ClaimReturned', 0, N'InsClaimReturned', 0, GETUTCDATE(), NULL, NULL, 1)
	, (49, 41, N'ApplicationDenied', 0, N'InsApplicationDenied', 0, GETUTCDATE(), NULL, NULL, 1)
	, (50, 42, N'OfferWithdrawn', 0, N'InsOfferWithdrawn', 0, GETUTCDATE(), NULL, NULL, 1)
	, (51, 43, N'NotAccepted', 0, N'InsNotAccepted', 0, GETUTCDATE(), NULL, NULL, 1)
	, (52, 44, N'ClaimApproved', 0, N'InsClaimApproved', 0, GETUTCDATE(), NULL, NULL, 1)