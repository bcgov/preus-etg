PRINT 'Inserting [InternalUsers]'
SET IDENTITY_INSERT [dbo].[InternalUsers] ON
INSERT [dbo].[InternalUsers]
 ([Id], [FirstName], [LastName], [IDIR], [Email]) VALUES

-- Internal Business Users
 (1, N'Sushil',    N'Bhojwani',    N'sbhojwan', N'sushil.bhojwani@gov.bc.ca')

-- Internal Team Users
,(50, N'Ian',       N'Caesar',      N'icaesar',  N'ian.caesar@ccal.ca')
SET IDENTITY_INSERT [dbo].[InternalUsers] OFF