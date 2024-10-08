PRINT 'Inserting [Organizations]'
SET IDENTITY_INSERT [dbo].[Organizations] ON
INSERT [dbo].[Organizations]
 ([Id], [BceIdGuid], [HeadOfficeAddressId], [OtherLegalStructure], [YearEstablished], [NumberOfEmployeesWorldwide], [LegalName], [DoingBusinessAs], [LegalStructureId], [NaicsId],
  [OrganizationTypeId], [NumberOfEmployeesInBC], [EmployerTypeCode], [AnnualTrainingBudget], [AnnualEmployeesTrained]) VALUES
 (1, N'10000000-0000-0000-0000-000000000001', 1, NULL, 2010, 50, N'FCV Interactive (Victoria)',  N'FCV Victoria',      1, 5, 1, 50, 1, 25000, 14)
,(2, N'10000000-0000-0000-0000-000000000002', 2, NULL, 2010, 50, N'FCV Interactive (Vancouver)', N'FCV Vancouver',     1, 5, 1, 50, 1, 45000, 19)
,(3, N'10000000-0000-0000-0000-000000000003', 3, NULL, 2010, 50, N'Avocette (Victoria)',         N'Avocette Victoria', 1, 5, 1, 50, 1, 35000, 12)
,(4, N'10000000-0000-0000-0000-000000000004', 4, NULL, 2010, 50, N'Avocette (New Westminster)',  N'Avocette',          1, 5, 1, 50, 1, 55000, 16)
SET IDENTITY_INSERT [dbo].[Organizations] OFF