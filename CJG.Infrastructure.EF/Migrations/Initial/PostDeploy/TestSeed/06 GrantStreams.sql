PRINT 'Inserting [GrantStreams]'
SET IDENTITY_INSERT [dbo].[GrantStreams] ON
INSERT [dbo].[GrantStreams]
 ([Id], [StreamId], [StreamCriteriaId], [StreamObjectiveId],	[IsActive], [DateFirstUsed]) VALUES
 (1,	1,			1,					1,						1,			CAST(N'2017-01-01 08:00:00.000' AS DateTime))
,(2,	2,			1,					2,						1,			CAST(N'2017-01-01 08:00:00.000' AS DateTime))
,(3,	3,			1,					3,						1,			NULL)
,(4,	4,			2,					4,						1,			CAST(N'2017-01-01 08:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[GrantStreams] OFF