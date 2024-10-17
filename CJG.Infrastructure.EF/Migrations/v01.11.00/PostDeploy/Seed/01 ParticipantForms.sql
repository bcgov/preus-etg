PRINT 'Update [ParticipantForms] - ReportOn'

-- All participants prior to September have been reported.
-- Assumption is that they were reported on the same day they were added.
UPDATE dbo.[ParticipantForms]
SET [ReportedOn] = [DateAdded]
WHERE [EmploymentStatusId] = 1
	AND [EIBenefitId] = 1
	AND [ReportedOn] IS NULL
	AND [DateAdded] < '2018-09-01'