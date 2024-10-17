PRINT 'Save Grant Program Notification Settings'

CREATE TABLE #NotificationTypes (
	[Id] INT NOT NULL
	, [Caption] NVARCHAR(128) NOT NULL 
	, [MilestoneDateName] NVARCHAR(128) NOT NULL 
	, [MilestoneDateOffset] INT NOT NULL
)

INSERT #NotificationTypes ( [Id], [Caption], [MilestoneDateName], [MilestoneDateOffset] ) 
VALUES
	(1, 'InsSubmittionAdmin', 'SubmittedDate', 0)
	,(2, 'InsIssuedOfferAdmin', 'OfferIssueDate', 0)
	,(3, 'InsGrantAcceptedAdmin', 'GrantAcceptedDate', 0)
	,(4, 'SchAcceptingDeadlineAdmin', 'OfferIssueDate', 5)
	,(5, 'SchBeforeTraining14DaysAdmin', 'TrainingStartDate', -14)
	,(6, 'SchBeforeTraining5DaysAdmin', 'TrainingStartDate', -5)
	,(7, 'SchAfterTraining1DayNoPartAdmin', 'TrainingStartDate', 1)
	,(8, 'SchAfterTraining1DayHasPartAdmin', 'TrainingStartDate', 1)
	,(9, 'SchAfterTraining14DaysAdmin', 'TrainingStartDate', 14)
	,(10, 'SchAfterTraining31DaysNoPartAdmin', 'TrainingStartDate', 31)
	,(11, 'SchAfterTraining31DaysHasPartAdmin', 'TrainingStartDate', 31)
	,(12, 'SchAfterTrainingEnd1DayAdmin', 'TrainingEndDate', 1)
	,(13, 'SchAfterTrainingEnd31DayNoRepAdmin', 'TrainingEndDate', 31)
	,(14, 'InsChangeTrainingProviderApproveAdmin', 'ChangeTrainingProviderApprove', 0)
	,(15, 'InsChangeTrainingProviderDenyAdmin', 'ChangeTrainingProviderDeny', 0)
	,(16, 'SchBeforeTraining14DaysEmp', 'TrainingStartDate', -14)
	,(17, 'SchBeforeTraining5DaysEmp', 'TrainingStartDate', -5)
	,(18, 'SchAfterTrainingEnd1DayEmp', 'TrainingEndDate', 1)
	,(19, 'SchAfterTrainingEnd31DayNoRepEmp', 'TrainingEndDate', 31)
	,(20, 'InsCancelledByMinsitry', 'DateCancelled', 0)
	,(21, 'InsClaimSubmitted', 'ClaimSumbitted', 0)
	,(22, 'InsPaymentRequested', 'PaymentRequested', 0)
	,(23, 'InsClaimDenied', 'ClaimDenied', 0)
	,(24, 'InsClaimReturned', 'ClaimReturned', 0)
	,(25, 'InsApplicationDenied', 'ApplicationDenied', 0)
	,(26, 'InsOfferWithdrawn', 'OfferWithdrawn', 0)
	,(27, 'InsNotAccepted', 'NotAccepted', 0)
	,(28, 'InsClaimApproved', 'ClaimApproved', 0)

-- Resync to the correctly notification type based on the name.
-- Capture the templates for each notification type.
CREATE TABLE #GrantProgramNotifications (
	[GrantProgramId] INT NOT NULL
	, [NotificationTypeId] INT NOT NULL
	, [NotificationTemplateId] INT NOT NULL
)

-- Moving the template from notification type to grant program notifications.
INSERT INTO #GrantProgramNotifications
SELECT
	[GrantProgramId]
	, [NotificationTypeId]
	, [NotificationTemplateId]
FROM (
	SELECT
		gnt.[GrantProgramId]
		, t.[Id] AS [NotificationTypeId]
		, nt.[NotificationTemplateId]
		, gnt.[TemplateCount]
		, ROW_NUMBER() OVER (PARTITION BY gnt.[GrantProgramId], t.[Id] ORDER BY gnt.[GrantProgramId], t.[Id], nt.[NotificationTemplateId]) AS [RowNumber]
	FROM (
		SELECT
			gpn.[GrantProgramId]
			, t.[Caption]
			, COUNT(*) AS [TemplateCount]
		FROM dbo.[GrantProgramNotifications] gpn
		JOIN dbo.[NotificationTypes] nt ON gpn.[NotificationTypeId] = nt.[Id]
		JOIN #NotificationTypes t ON nt.[NotificationTypeName] = t.[Caption]
		GROUP BY gpn.[GrantProgramId], t.[Caption]) AS gnt
	JOIN #NotificationTypes t ON gnt.[Caption] = t.[Caption]
	JOIN dbo.[NotificationTypes] nt ON gnt.[Caption] = nt.[NotificationTypeName]
	JOIN dbo.[NotificationTemplates] tt ON nt.[NotificationTemplateId] = tt.[Id]) notifications
WHERE ([GrantProgramId] IN (1, 2) AND [RowNumber] = 1)
	OR ([GrantProgramId] = 3 AND [RowNumber] = [TemplateCount])
--ORDER BY [GrantProgramId], [NotificationTypeId], [NotificationTemplateId]

-- Remove the duplicate notofication types.
DELETE FROM dbo.[NotificationTypes]
WHERE [Id] > 28

-- Clear the grant program notifications link
DELETE FROM dbo.[GrantProgramNotifications]

DROP TABLE #NotificationTypes



