PRINT 'Updating [NotificationTemplates]'

--4375
UPDATE [dbo].[NotificationTemplates]
SET [EmailBody] = N'<!DOCTYPE html><html><head/><body><p>Hello ::AAFirstName::,</p><p>Your Canada-BC Job Grant application for "::TrainingProgramTitle::" has been received and assigned the file number ::FileNumber::. Future emails regarding the application will have the file number in the subject line and it is your reference to the file in the <a href="::BaseURL::">skillstraininggrants.gov.bc.ca</a>.</p><p> PLEASE NOTE: You will be informed of decisions within 60 days of submitting an application. If training proceeds before receiving a decision from the Ministry, the employer will be liable for all incurred costs should the application be denied.</p><p><b>IMPORTANT!</b> Participants who are currently Employment Insurance or Income Assistance Clients must have approval prior to the start of training if they wish to maintain their financial supports. Refer to Appendix A of the CJG General Criteria for details.  EI or IA clients who do not obtain pre-approval before participating in training may become ineligible for continued financial supports under EI or IA. All Participant Information Forms are due no less than 5 business days prior to the start of training.  However, if you are applying for Participants who may currently be Employment Insurance or Income Assistance Clients, please send notification to the <a href="mailto:CJGInfo@gov.bc.ca">CJGInfo@gov.bc.ca</a>.</p><p>Canada-BC Job Grant Team</p></body></html>' 
WHERE [Id] = 01

--4269
UPDATE dbo.NotificationTemplates
SET 
	AlertCaption = REPLACE(AlertCaption,'RE: ',''),
	EmailSubject = REPLACE(EmailSubject,'RE: ','')
FROM dbo.NotificationTemplates