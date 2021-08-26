PRINT 'Inserting [SkillsFocus]'
SET IDENTITY_INSERT [dbo].[SkillsFocus] ON 
INSERT [dbo].[SkillsFocus]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 0, N'Specialized or Technical Skills')
,(2, 1, 0, N'Management or Business Skills')
,(3, 1, 0, N'Essential Skills')
,(4, 1, 0, N'Soft Skills')
,(5, 1, 0, N'Apprenticeship Training')
,(6, 1, 0, N'Foundation Program')
SET IDENTITY_INSERT [dbo].[SkillsFocus] OFF