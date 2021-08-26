PRINT 'Inserting [ApplicationUserRoles]'

INSERT [dbo].[ApplicationUserRoles]
 ([UserId], [RoleId], [Discriminator]) VALUES

-- Internal Business Users
 (N'c3d93d3e-f6c4-47ff-bacd-0e672fceaccf', N'5310a4ac-5eed-4dfa-8603-f7c616bce47a', N'IdentityUserRole')

-- Internal Team Users
,(N'6512b19d-af09-416d-b9cc-5369ee055cb5', N'5310a4ac-5eed-4dfa-8603-f7c616bce47a', N'IdentityUserRole')
