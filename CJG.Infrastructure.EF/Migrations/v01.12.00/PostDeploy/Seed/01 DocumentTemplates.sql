PRINT 'Updating [DocumentTemplates] - Fix email'

UPDATE dt
SET dt.Body = REPLACE(dt.Body, ')Info@gov.bc.ca', ')@("@gov.bc.ca")')
FROM dbo.[DocumentTemplates] dt
WHERE dt.Body LIKE '%)Info@%'
