PRINT 'INSERT [ParticipantEmploymentStatus]'

SET IDENTITY_INSERT [dbo].[ParticipantEmploymentStatus] ON
Insert Into [dbo].[ParticipantEmploymentStatus]
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
  'Unemployed',
  1, 
  1,
  GETUTCDATE(),
  GETUTCDATE()
), 
(
  2, 
  'Underemployed (part-time, seasonal, casual)',
  1, 
  2,
  GETUTCDATE(),
  GETUTCDATE()
),
(
  3, 
  'Employed and low skilled (less than high school education)',
  1, 
  3,
  GETUTCDATE(),
  GETUTCDATE()
),
(
  4, 
  'None of the above',
  1, 
  4,
  GETUTCDATE(),
  GETUTCDATE()
)
SET IDENTITY_INSERT [dbo].[ParticipantEmploymentStatus] OFF