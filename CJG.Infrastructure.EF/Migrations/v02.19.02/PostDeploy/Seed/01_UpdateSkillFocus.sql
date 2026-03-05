PRINT 'Start Updating [dbo].[SkillsFocus]'

-- Deactivate some skill focuses
UPDATE [dbo].[SkillsFocus]
SET IsActive = 0,
    DateUpdated = GETUTCDATE()
WHERE Id = 5
AND Caption = 'Apprenticeship Training'

UPDATE [dbo].[SkillsFocus]
SET IsActive = 0, 
    DateUpdated = GETUTCDATE()
WHERE Id = 8
AND Caption = 'Industry Recognized Credentials'

UPDATE [dbo].[SkillsFocus]
SET IsActive = 0, 
    DateUpdated = GETUTCDATE()
WHERE Id = 2
AND Caption = 'Management Skills'

UPDATE [dbo].[SkillsFocus]
SET IsActive = 0, 
    DateUpdated = GETUTCDATE()
WHERE Id = 7
AND Caption = 'Business Skills'

SET IDENTITY_INSERT [dbo].[SkillsFocus] ON 

-- Add a skill focus
INSERT INTO [dbo].[SkillsFocus] (Id, Caption, IsActive, RowSequence, DateAdded)
VALUES (9, 'Management and Business Skills', 1, 1, GETUTCDATE())

SET IDENTITY_INSERT [dbo].[SkillsFocus] OFF

-- Set Ordering of Focuses
UPDATE [dbo].[SkillsFocus]
SET RowSequence = 2, 
    DateUpdated = GETUTCDATE()
WHERE Id = 4
AND Caption = 'Soft Skills'

UPDATE [dbo].[SkillsFocus]
SET RowSequence = 3, 
    DateUpdated = GETUTCDATE()
WHERE Id = 1
AND Caption = 'Technical Skills'

UPDATE [dbo].[SkillsFocus]
SET RowSequence = 4, 
    DateUpdated = GETUTCDATE()
WHERE Id = 3
AND Caption = 'Essential Skills'

PRINT 'Done Updating [SkillsFocus]'
