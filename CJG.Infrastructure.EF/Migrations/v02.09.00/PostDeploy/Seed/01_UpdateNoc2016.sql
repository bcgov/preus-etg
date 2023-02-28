PRINT 'Start Update existing NOC Codes to 2016'

UPDATE NationalOccupationalClassifications
SET NOCVersion = '2016',
    DateUpdated = GETUTCDATE()

PRINT 'End Update existing NOC Codes to 2016'
