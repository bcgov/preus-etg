PRINT 'UPDATE NOTIFICATION TEMPLATE'
UPDATE NotificationTemplates
SET EmailBody = REPLACE(EmailBody, 'CJGInfo', '@(Model.ProgramCode)Info');

UPDATE NotificationTemplates
SET EmailBody = REPLACE(EmailBody, 'cjgreimbursement', '@(Model.ProgramCode)Info');