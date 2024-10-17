PRINT 'INSERT [ApplicantOrganizationTypes]'

SET IDENTITY_INSERT [dbo].[ApplicantOrganizationTypes] ON

INSERT INTO [dbo].[ApplicantOrganizationTypes]
           ([Id]
		   ,[Caption]
           ,[IsActive]
           ,[RowSequence]
           ,[DateAdded]
           ,[DateUpdated])
VALUES
    (1
	,'Business, Industry, Sector Associations'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(2
	,'Civic, Social, and Fraternal Associations'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(3
	,'Individual and Family Social Services'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(4
	,'Job Training and Vocational Rehabilitation Services'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(5
	,'Labor Unions and Similar Labor Organizations'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(6
	,'Membership Organizations'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(7
	,'Political Organizations'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(8
	,'Professional Membership Organizations'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(9
	,'Religious Organizations'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(10
	,'Aboriginal Friendship Society'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(11
	,'Métis Nation BC'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(12
	,'Municipality'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(13
	,'County'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(14
	,'Township'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(15
	,'Agency'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(16
	,'Crown Corps'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(17
	,'Band Council'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(18
	,'Tribal Council'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(19
	,'FN Health Authority'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(20
	,'Indigenous Government'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())
,(21
	,'Other'
    ,1
    ,0
    ,GETUTCDATE()
    ,GETUTCDATE())

SET IDENTITY_INSERT [dbo].[ApplicantOrganizationTypes] OFF