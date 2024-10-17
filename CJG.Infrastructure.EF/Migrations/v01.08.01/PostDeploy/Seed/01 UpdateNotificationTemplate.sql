PRINT 'UPDATE NOTIFICATION TEMPLATE'
UPDATE NotificationTemplates
SET EmailBody = REPLACE(EmailBody, 'ETGInfo', '@(Model.ProgramCode)Info');

UPDATE NotificationTemplates
SET EmailBody = REPLACE(EmailBody, 'reinvested', 'reinvest')
Where ID in (10, 11);
