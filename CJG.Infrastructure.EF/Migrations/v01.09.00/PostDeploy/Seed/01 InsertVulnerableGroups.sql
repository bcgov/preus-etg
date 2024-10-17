PRINT 'INSERT [VulnerableGroups]'

SET IDENTITY_INSERT [dbo].[VulnerableGroups] ON
Insert Into [dbo].[VulnerableGroups]
(
	Id,
	Caption,
	IsActive,
	RowSequence,
	DateAdded,
	DateUpdated
)
Values 
( 
  1, 
  'Individuals facing barriers to employment such as former inmates, chronically unemployed, etc.',
  1, 
  0,
  GETUTCDATE(),
  GETUTCDATE()
), 
(
  2, 
  'Older Workers 55+',
  1, 
  0,
  GETUTCDATE(),
  GETUTCDATE()
),
(
  3, 
  'Persons with disabilities',
  1, 
  0,
  GETUTCDATE(),
  GETUTCDATE()
),
(
  4, 
  'Refugees and protected persons',
  1, 
  0,
  GETUTCDATE(),
  GETUTCDATE()
),
(
  5, 
  'Youth at risk including youth in care or former youth in care (aged 15 to 29)',
  1, 
  0,
  GETUTCDATE(),
  GETUTCDATE()
)

SET IDENTITY_INSERT [dbo].[VulnerableGroups] OFF