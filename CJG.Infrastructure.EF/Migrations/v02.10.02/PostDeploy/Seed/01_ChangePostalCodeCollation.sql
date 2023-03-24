PRINT 'Fix Postal Code Collation'

-- Altering ApplicationAddress table since it appears to be different across db instances
ALTER TABLE dbo.ApplicationAddresses
ALTER COLUMN PostalCode NVARCHAR(10) COLLATE Latin1_General_CI_AS NOT NULL

-- Alter Prioritization to match application address collation
ALTER TABLE dbo.PrioritizationPostalCodes
ALTER COLUMN PostalCode NVARCHAR(10) COLLATE Latin1_General_CI_AS NULL
