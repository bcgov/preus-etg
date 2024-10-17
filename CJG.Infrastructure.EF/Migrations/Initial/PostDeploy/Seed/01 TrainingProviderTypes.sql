PRINT 'Inserting [TrainingProviderTypes]'
SET IDENTITY_INSERT [dbo].[TrainingProviderTypes] ON 
INSERT [dbo].[TrainingProviderTypes]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 0, N'Public Post-Secondary Institution')
,(2, 1, 0, N'BC School District Training Organization')
,(3, 1, 0, N'Registered Trade / Technical School')
,(4, 1, 0, N'Union Hall')
,(5, 1, 0, N'Industry Recognized Safety Trainer')
,(6, 1, 0, N'BC Private Post-Secondary Institution - Registered with Private Training Institutions Branch')
,(7, 1, 0, N'Private Training Provider - Registered with Private Training Institutions Branch')
,(8, 1, 0, N'Private Training Provider - Not Registered with Private Training Institutions Branch')
SET IDENTITY_INSERT [dbo].[TrainingProviderTypes] OFF