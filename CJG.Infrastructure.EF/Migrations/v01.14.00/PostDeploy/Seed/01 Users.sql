PRINT 'Updating [Users]'

-- Test01
UPDATE dbo.[Users]
SET [EmailAddress] = 'Max.Sudik@avocette.com'
WHERE [BCeIDGuid] = 'B0D099E1-FDFD-4D5F-9286-9E228F948903'

-- Test02
UPDATE dbo.[Users]
SET [EmailAddress] = 'Max.Sudik@avocette.com'
WHERE [BCeIDGuid] = '221371BB-030C-4163-8C58-15768336DA55'

-- Test03
UPDATE dbo.[Users]
SET [EmailAddress] = 'Max.Sudik@avocette.com'
WHERE [BCeIDGuid] = '305494E4-8680-4880-9C6C-F743B04FC8F6'

-- Test04
UPDATE dbo.[Users]
SET [EmailAddress] = 'Max.Sudik@avocette.com'
WHERE [BCeIDGuid] = '729E2A4D-FB99-4D4D-B34E-A08097E37486'

-- Test05
UPDATE dbo.[Users]
SET [EmailAddress] = 'Max.Sudik@avocette.com'
WHERE [BCeIDGuid] = 'BE03CBFA-A00F-478B-894E-61841FF70EFB'

-- Test06
UPDATE dbo.[Users]
SET [EmailAddress] = 'Max.Sudik@avocette.com'
WHERE [BCeIDGuid] = '2B4BD3A4-E75A-4B40-9327-F8347450C757'

-- All Fake Accounts
UPDATE dbo.[Users]
SET [EmailAddress] = 'Max.Sudik@avocette.com'
WHERE [AccountType] = 2

-- Change Sushil's account to a test account
UPDATE dbo.[Users]
SET [AccountType] = 2
WHERE [BCeIDGuid] = 'DCFF0F71-1430-4554-8500-7397C38A1E4F'


CHECKPOINT