PRINT 'Start Insert default prioritization thresholds'

INSERT [dbo].[PrioritizationThresholds] (IndustryThreshold, RegionalThreshold, EmployeeCountThreshold, [DateAdded]) 
VALUES (2, 2.5, 50, GETDATE())

PRINT 'Done Insert default prioritization thresholds'