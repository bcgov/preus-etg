PRINT 'INSERT [RiskClassifications]'

SET IDENTITY_INSERT [dbo].[RiskClassifications] ON 
INSERT [dbo].[RiskClassifications]
 ([Id], [IsActive], [RowSequence], [Caption], [DateAdded], [DateUpdated]) VALUES
 (1, 1, 01, N'High', SYSDATETIME(), SYSDATETIME())
INSERT [dbo].[RiskClassifications]
 ([Id], [IsActive], [RowSequence], [Caption], [DateAdded], [DateUpdated]) VALUES
 (2, 1, 02, N'Medium', SYSDATETIME(), SYSDATETIME())
INSERT [dbo].[RiskClassifications]
 ([Id], [IsActive], [RowSequence], [Caption], [DateAdded], [DateUpdated]) VALUES
 (3, 1, 03, N'Low', SYSDATETIME(), SYSDATETIME())
SET IDENTITY_INSERT [dbo].[RiskClassifications] OFF
