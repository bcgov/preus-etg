PRINT 'Updating Fiscal Years'

UPDATE dbo.FiscalYears
SET Caption = 'FY' + Caption
WHERE Caption not like 'FY%'