PRINT 'Inserting [NoteTypes]'
SET IDENTITY_INSERT [dbo].[NoteTypes] ON 
INSERT [dbo].[NoteTypes]
 ([Id], [IsSystem], [IsActive], [RowSequence], [Caption], [Description]) VALUES
 (01, 0, 1, 0, N'AA', N'Agreement or Amendment Approval')
,(02, 0, 1, 0, N'CL', N'Claim Approval/Rejection')
,(03, 0, 1, 0, N'AF', N'Contract Commitment Form')
,(04, 0, 1, 0, N'AC', N'Agreement Change')
,(05, 0, 1, 0, N'AR', N'Agreement Responsibility Change')
,(06, 0, 1, 0, N'QR', N'Note to program manager/Qualified Receiver')
,(07, 0, 1, 0, N'PD', N'Note to program director')
,(08, 0, 1, 0, N'QA', N'Note to quality assurance process')
,(09, 0, 1, 0, N'SC', N'Agreement or Amendment Approval')
,(10, 1, 1, 0, N'SY', N'System Change')
,(11, 1, 1, 0, N'WF', N'Workflow')
SET IDENTITY_INSERT [dbo].[NoteTypes] OFF