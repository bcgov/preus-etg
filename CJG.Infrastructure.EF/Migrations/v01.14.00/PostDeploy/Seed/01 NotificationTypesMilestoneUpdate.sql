PRINT 'UPDATE [NotificationTypes]'

Update [NotificationTypes]
Set MilestoneDateName = 'DateSubmitted'
Where Id = 1;

Update [NotificationTypes]
Set MilestoneDateName = 'NotApplicable'
Where Id in (2, 4, 14, 15, 21, 22, 23, 24, 25, 26, 27, 28);

Update [NotificationTypes]
Set MilestoneDateName = 'DateAccepted'
Where Id = 3;

Update [NotificationTypes]
Set MilestoneDateName = 'TrainingStartDate'
Where Id in (5, 6, 7, 8, 9, 10, 11, 16, 17, 18, 19, 29);

Update [NotificationTypes]
Set MilestoneDateName = 'TrainingEndDate'
Where Id in (12, 13);

CHECKPOINT