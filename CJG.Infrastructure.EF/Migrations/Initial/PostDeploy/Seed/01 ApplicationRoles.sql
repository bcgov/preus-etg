PRINT 'Inserting [ApplicationRoles]'
INSERT [dbo].[ApplicationRoles]
 ([Id], [Discriminator], [Name]) VALUES
 (N'375fcd81-b33b-4676-a354-93f41fe0af6a', N'ApplicationRole', N'Assessor')
,(N'5310a4ac-5eed-4dfa-8603-f7c616bce47a', N'ApplicationRole', N'System Administrator')
,(N'b49fded2-066c-4b19-a32b-3d4fbe4797f9', N'ApplicationRole', N'Operations Manager')
,(N'b50459ba-2aad-459e-be19-0ab9b266ab7b', N'ApplicationRole', N'Measurement And Reporting')
,(N'b9e8e3c0-f5ff-4847-9984-efc7ca97f7af', N'ApplicationRole', N'Director')
,(N'c6661e89-55e8-4adb-ac1b-7cc654289114', N'ApplicationRole', N'Financial Administrator')
,(N'ea0a8bf0-0499-4cf8-b73e-4d7437f2ddb1', N'ApplicationRole', N'Director Of Finance')