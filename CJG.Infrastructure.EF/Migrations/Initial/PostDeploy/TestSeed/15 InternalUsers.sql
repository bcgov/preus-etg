PRINT 'Inserting [InternalUsers]'
SET IDENTITY_INSERT [dbo].[InternalUsers] ON
INSERT [dbo].[InternalUsers]
 ([Id], [FirstName], [LastName], [IDIR], [Email]) VALUES
-- Business Users
 (2, N'Laura',     N'Elliot',      N'lelliott', N'laura.elliott@gov.bc.ca')
,(3, N'Maria',     N'Chen',        N'machen',   N'maria.chen@gov.bc.ca')
,(4, N'Nadaya',    N'Howells',     N'nhowells', N'nadaya.howells@gov.bc.ca')
,(5, N'Lisa',      N'Tabachuk',    N'ltabachu', N'lisa.tabachuk@gov.bc.ca')
,(6, N'Courtenay', N'Wilson',      N'couwilso', N'courtenay.wilson@gov.bc.ca')
,(7, N'Genevieve', N'Casault',     N'gcasault', N'genevieve.casault@gov.bc.ca')
,(8, N'Robyn',     N'Wood',        N'rmwood',   N'robyn.wood@gov.bc.ca')
,(9, N'Chris',     N'Clarke',      N'cclarke',  N'chris.clarke@gov.bc.ca')
,(10, N'Nicoleta',  N'Turtureanu',  N'nturture', N'nicoleta.turtureanu@gov.bc.ca')
,(11, N'Erhan',     N'Baydar',      N'ebaydar',  N'erhan.baydar@gov.bc.ca')
,(12, N'Michelle',  N'Beaubien',    N'mbeaubie', N'michelle.beaubien@gov.bc.ca')
 
-- Internal Team Users
,(51, N'Tim',       N'Gerhardt',    N'tigerhar', N'tim.gerhardt@gov.bc.ca')
,(52, N'Matthew',   N'Mason',       N'mamason',  N'matthew.mason@fcvinteractive.com')
,(53, N'Vance',     N'McColl',      N'vanmccol', N'vance.mccoll@avocette.com')
,(54, N'Jeremy',    N'Foster',      N'jefoster', N'jfoster@fosol.ca')
,(55, N'Adam',      N'Lamping',     N'alamping', N'adam.lamping@fcvinteractive.com')
,(56, N'Raman',     N'Samra',       N'rssamra',  N'raman.samra@avocette.com')
,(57, N'Shelly',    N'Saunders',    N'shsaunde', N'shelly.saunders@avocette.com')
,(58, N'Idir1',     N'TestAccount', N'CJFTest1', N'TU1@avocette.com')
,(59, N'Idir2',     N'TestAccount', N'CJFTest2', N'TU2@avocette.com')
,(60, N'Idir3',     N'TestAccount', N'CJFTest3', N'TU3@avocette.com')
,(61, N'Idir4',     N'TestAccount', N'CJFTest4', N'TU4@avocette.com')
,(62, N'Idir5',     N'TestAccount', N'CJFTest5', N'TU5@avocette.com')
,(63, N'Dave',      N'Penfold',     N'dpenfold', N'dave.penfold@fcvinteractive.com')
,(64, N'Ryota',     N'Matsumoto',   N'RMATSUMO', N'ryota.matsumoto@fcvinteractive.com')

SET IDENTITY_INSERT [dbo].[InternalUsers] OFF