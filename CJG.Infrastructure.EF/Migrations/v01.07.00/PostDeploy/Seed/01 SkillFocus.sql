PRINT 'Updating [SkillsFocus]'

UPDATE [dbo].[SkillsFocus] -- Changed from "Specialized or Technical Skills"
SET [Isactive] = 1
	, [RowSequence] = 0
	, [Caption] = N'Technical Skills'
WHERE [Id] = 1

UPDATE [dbo].[SkillsFocus] -- Changed from "Management or Business Skills"
SET [Isactive] = 1
	, [RowSequence] = 0
	, [Caption] = N'Management Skills'
WHERE [Id] = 2

UPDATE [dbo].[SkillsFocus] -- Make "Foundation Program" inactive
SET [Isactive] = 0
WHERE [Id] = 6

SET IDENTITY_INSERT [dbo].[SkillsFocus] ON 

INSERT [dbo].[SkillsFocus] (
	[Id], [IsActive], [RowSequence], [Caption]
) VALUES (
	7, 1, 0, N'Business Skills'
), (
	8, 1, 0, N'Industry Recognized Credentials'
)

SET IDENTITY_INSERT [dbo].[SkillsFocus] OFF