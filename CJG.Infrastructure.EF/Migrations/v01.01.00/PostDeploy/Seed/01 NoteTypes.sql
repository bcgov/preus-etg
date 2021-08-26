PRINT 'Inserting [NoteTypes]'

-- Disable and update NoteTypes
UPDATE [dbo].[NoteTypes] SET
	[RowSequence] = 00,
	[IsActive] = 0,
	[Caption] = N'AA'
WHERE [Id] = 1

UPDATE [dbo].[NoteTypes] SET
	[RowSequence] = 00,
	[IsActive] = 0,
	[Caption] = N'CL'
WHERE [Id] = 2

UPDATE [dbo].[NoteTypes] SET
	[RowSequence] = 00,
	[IsActive] = 0,
	[Caption] = N'AF'
WHERE [Id] = 3

UPDATE [dbo].[NoteTypes] SET
	[RowSequence] = 00,
	[IsActive] = 0,
	[Caption] = N'AC'
WHERE [Id] = 4

UPDATE [dbo].[NoteTypes] SET
	[RowSequence] = 00,
	[IsActive] = 0,
	[Caption] = N'AR'
WHERE [Id] = 5

UPDATE [dbo].[NoteTypes] SET
	[RowSequence] = 00,
	[IsActive] = 0,
	[Caption] = N'QR'
WHERE [Id] = 6

UPDATE [dbo].[NoteTypes] SET
	[RowSequence] = 02,
	[Caption] = N'PD', 
	[Description] =N'Note to Director'
WHERE [Id] = 7

UPDATE [dbo].[NoteTypes] SET
	[RowSequence] = 04,
	[Caption] = N'QA', 
	[Description] = N'Note to QA'
WHERE [Id] = 8

UPDATE [dbo].[NoteTypes] SET
	[RowSequence] = 00,
	[IsActive] = 0,
	[Caption] = N'SC'
WHERE [Id] = 9

UPDATE [dbo].[NoteTypes] SET
	[RowSequence] = 00,
	[IsActive] = 0,
	[Caption] = N'SY'
WHERE [Id] = 10

UPDATE [dbo].[NoteTypes] SET
	[RowSequence] = 8,
	[IsSystem] = 1,
	[Caption] = N'WF', 
	[Description] = N'Workflow'
WHERE [Id] = 11

SET IDENTITY_INSERT [dbo].[NoteTypes] ON 
INSERT [dbo].[NoteTypes]
 ([Id], [IsSystem], [IsActive], [RowSequence], [Caption], [Description]) VALUES
 (12, 0, 1, 01, N'AS', N'Note to Assessor')
,(13, 0, 1, 03, N'PM', N'Note to Policy')
,(14, 0, 1, 05, N'NR', N'Note to Reimbursement')
,(15, 1, 1, 06, N'ED', N'File Change')
,(16, 1, 1, 07, N'NT', N'Notification')
SET IDENTITY_INSERT [dbo].[NoteTypes] OFF

-- If there are no Notes of the specified NoteTypes they can be deleted.
DECLARE @Total INT
DECLARE @NoteTypeId INT

SET @NoteTypeId = 1 -- AA - Agreement or Amendment Approval
SELECT @Total= COUNT(*) FROM [dbo].[Notes] WHERE [NoteTypeId] = @NoteTypeId
IF (@Total = 0) DELETE FROM [dbo].[NoteTypes] WHERE [Id] = @NoteTypeId

SET @NoteTypeId = 2 -- CL - Claim Approval/Rejection
SELECT @Total= COUNT(*) FROM [dbo].[Notes] WHERE [NoteTypeId] = @NoteTypeId
IF (@Total = 0) DELETE FROM [dbo].[NoteTypes] WHERE [Id] = @NoteTypeId

SET @NoteTypeId = 3 -- AF - Contract Commitment Form
SELECT @Total= COUNT(*) FROM [dbo].[Notes] WHERE [NoteTypeId] = @NoteTypeId
IF (@Total = 0) DELETE FROM [dbo].[NoteTypes] WHERE [Id] = @NoteTypeId

SET @NoteTypeId = 4 -- AC - Agreement Change
SELECT @Total= COUNT(*) FROM [dbo].[Notes] WHERE [NoteTypeId] = @NoteTypeId
IF (@Total = 0) DELETE FROM [dbo].[NoteTypes] WHERE [Id] = @NoteTypeId

SET @NoteTypeId = 5 -- AR - Agreement Responsibility Change
SELECT @Total= COUNT(*) FROM [dbo].[Notes] WHERE [NoteTypeId] = @NoteTypeId
IF (@Total = 0) DELETE FROM [dbo].[NoteTypes] WHERE [Id] = @NoteTypeId

SET @NoteTypeId = 6 -- QR - Note to program manager/Qualified Receiver
SELECT @Total= COUNT(*) FROM [dbo].[Notes] WHERE [NoteTypeId] = @NoteTypeId
IF (@Total = 0) DELETE FROM [dbo].[NoteTypes] WHERE [Id] = @NoteTypeId

SET @NoteTypeId = 9 -- SC - Agreement or Amendment Approval
SELECT @Total= COUNT(*) FROM [dbo].[Notes] WHERE [NoteTypeId] = @NoteTypeId
IF (@Total = 0) DELETE FROM [dbo].[NoteTypes] WHERE [Id] = @NoteTypeId

SET @NoteTypeId = 10 -- SY - System Change
SELECT @Total= COUNT(*) FROM [dbo].[Notes] WHERE [NoteTypeId] = @NoteTypeId
IF (@Total = 0) DELETE FROM [dbo].[NoteTypes] WHERE [Id] = @NoteTypeId
