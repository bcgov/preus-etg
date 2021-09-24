PRINT 'Inserting [DeliveryPartnerServices]'
SET IDENTITY_INSERT [dbo].[DeliveryPartnerServices] ON 
INSERT [dbo].[DeliveryPartnerServices]
 ([Id], [IsActive], [RowSequence], [Caption]) VALUES
 (1, 1, 0, N'Canada-BC Job Grant program information')
,(2, 1, 0, N'Grant application eligibility or capacity assessment')
,(3, 1, 0, N'Skills or training needs assessment')
,(4, 1, 0, N'Training program planning')
,(5, 1, 0, N'Application development')
SET IDENTITY_INSERT [dbo].[DeliveryPartnerServices] OFF