PRINT 'Inserting [ApplicationUsers]'
INSERT [dbo].[ApplicationUsers]
 ([InternalUserId], [ApplicationUserId], [Email], [SecurityStamp], [UserName], [Active]) VALUES

-- Internal Business Users
 (1, N'c3d93d3e-f6c4-47ff-bacd-0e672fceaccf', N'sushil.bhojwani@gov.bc.ca',          N'8ec83866-76bd-4d28-b9fd-ec5ebe3189c6', N'sbhojwan', 1)
 
-- Internal Team Users
,(50, N'6512b19d-af09-416d-b9cc-5369ee055cb5', N'ian.caesar@ccal.ca',                 N'804685e0-b495-4b7a-b5f5-9d39f09fa506', N'icaesar',  1)
