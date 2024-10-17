PRINT 'INSERT [UnderRepresentedPopulations]'

SET IDENTITY_INSERT [dbo].[UnderRepresentedPopulations] ON
Insert Into [dbo].[UnderRepresentedPopulations]
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
  'Indigenous persons',
  1, 
  0,
  GETUTCDATE(),
  GETUTCDATE()
), 
(
  2, 
  'Landed immigrants to Canada within the past five years',
  1, 
  0,
  GETUTCDATE(),
  GETUTCDATE()
),
(
  3, 
  'Women training in underrepresented fields such as Trades, Natural Resources or Technology',
  1, 
  0,
  GETUTCDATE(),
  GETUTCDATE()
),
(
  4, 
  'Youth (aged 15 to 29)',
  1, 
  0,
  GETUTCDATE(),
  GETUTCDATE()
)

SET IDENTITY_INSERT [dbo].[UnderRepresentedPopulations] OFF