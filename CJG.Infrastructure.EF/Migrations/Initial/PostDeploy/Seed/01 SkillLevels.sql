PRINT 'Inserting [SkillLevels]'
SET IDENTITY_INSERT [dbo].[SkillLevels] ON 
INSERT [dbo].[SkillLevels]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 1, N'Entry Level Skills')
,(2, 1, 2, N'Upskilling or Upgrading')
,(3, 1, 3, N'Maintenance for Current Job')
SET IDENTITY_INSERT [dbo].[SkillLevels] OFF