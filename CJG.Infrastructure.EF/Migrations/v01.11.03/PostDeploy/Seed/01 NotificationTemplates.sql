PRINT 'Update [NotificationTemplates] - Fix invalid email'

UPDATE nt
SET nt.EmailBody = REPLACE(nt.EmailBody, ')Info@gov.bc.ca', ')@("@gov.bc.ca")')
FROM dbo.[NotificationTemplates] nt
WHERE nt.EmailBody LIKE '%)Info@%'