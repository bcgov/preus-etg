PRINT '02. Updating [TrainingProviderTypes]'

UPDATE [dbo].TrainingProviderTypes
SET [ProofOfInstructorQualifications] = 0,
[CourseOutline] = 1
WHERE [Id] = 2

UPDATE [dbo].TrainingProviderTypes
SET [ProofOfInstructorQualifications] = 0,
[CourseOutline] = 1
WHERE [Id] = 3

UPDATE [dbo].TrainingProviderTypes
SET [ProofOfInstructorQualifications] = 0,
[CourseOutline] = 1
WHERE [Id] = 7

UPDATE [dbo].TrainingProviderTypes
SET [ProofOfInstructorQualifications] = 1,
[CourseOutline] = 1
WHERE [Id] = 8

UPDATE [dbo].TrainingProviderTypes
SET [ProofOfInstructorQualifications] = 0,
[CourseOutline] = 1
WHERE [Id] = 9

UPDATE [dbo].TrainingProviderTypes
SET [ProofOfInstructorQualifications] = 0,
[CourseOutline] = 1
WHERE [Id] = 10

UPDATE [dbo].TrainingProviderTypes
SET [ProofOfInstructorQualifications] = 1,
[CourseOutline] = 1
WHERE [Id] = 12

PRINT 'Updating [TrainingProviderTypes] -END'