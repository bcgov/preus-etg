/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
			   SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
PRINT 'UPDATE NOTIFICATION TEMPLATE'
UPDATE NotificationTemplates
SET EmailBody = REPLACE(REPLACE(REPLACE(EmailBody, 'log in', 'login'),
								'Login to the system',
								'Login'),
						'login to the system',
						'login');

PRINT 'UPDATE NOTIFICATION TEMPLATE'
DECLARE @TemplateCursor CURSOR;
BEGIN
	DECLARE @counter tinyint;
	DECLARE @oddText nvarchar(10) = '@Model.';
	DECLARE @evenText nvarchar(max) = '';
	DECLARE @textToReplace nvarchar(10) = '::';
	DECLARE @currentTemplateId int;
	DECLARE @text nvarchar(max);

	SET @TemplateCursor = CURSOR FOR
	SELECT Id FROM NotificationTemplates 

	OPEN @TemplateCursor 
	FETCH NEXT FROM @TemplateCursor 
	INTO @currentTemplateId

	WHILE @@FETCH_STATUS = 0
	BEGIN
		PRINT 'Update Notification Template Id : ' + CAST(@currentTemplateId AS VARCHAR(16));

		-- Update the AlertCaption -- Old Keywords
		SET @counter = 1;
		SELECT @text = AlertCaption FROM NotificationTemplates WHERE Id = @currentTemplateId;

		WHILE CHARINDEX(@textToReplace, @text, 1) > 0
		BEGIN
			SELECT @text = STUFF(@text, 
						CHARINDEX(@textToReplace, @text, 1), 
						LEN(@textToReplace), 
						IIF(@counter%2=0, @evenText, @oddText)),
					@counter = @counter + 1
		END
		
		SET @text = REPLACE(@text, 'AAFirstName', 'RecipientFirstName')
		SET @text = REPLACE(@text, 'AALastName', 'RecipientLastName')
		SET @text = REPLACE(@text, 'AAEmail', 'ReceipientEmail')
		SET @text = REPLACE(@text, 'EAFirstName', 'RecipientFirstName')
		SET @text = REPLACE(@text, 'EALastName', 'RecipientLastName')
		SET @text = REPLACE(@text, 'EAEmail', 'ReceipientEmail')
		SET @text = REPLACE(@text, 'StreamName', 'FullStreamName')
		SET @text = REPLACE(@text, 'skillstraininggrants.gov.bc.ca', '@Model.BaseURL')
		SET @text = REPLACE(@text, 'Canada-BC Job Grant', '@Model.ProgramName')
		SET @text = REPLACE(@text, 'CJG ', '@Model.ProgramAbbreviation ')
		SET @text = REPLACE(@text, 'CJGInfo', 'ETGInfo')
		SET @text = REPLACE(@text, 'cjgreimbursement@gov.bc.ca', 'ETGInfo@gov.bc.ca')

		UPDATE NotificationTemplates
		SET 
			AlertCaption = @text
		FROM NotificationTemplates
		WHERE Id = @currentTemplateId;

		-- Update the EmailSubject -- Old Keywords
		SET @counter = 1;
		SELECT @text = EmailSubject FROM NotificationTemplates WHERE Id = @currentTemplateId;

		WHILE CHARINDEX(@textToReplace, @text, 1) > 0
		BEGIN
			SELECT @text = STUFF(@text, 
						CHARINDEX(@textToReplace, @text, 1), 
						LEN(@textToReplace), 
						IIF(@counter%2=0, @evenText, @oddText)),
					@counter = @counter + 1
		END

		SET @text = REPLACE(@text, 'AAFirstName', 'RecipientFirstName')
		SET @text = REPLACE(@text, 'AALastName', 'RecipientLastName')
		SET @text = REPLACE(@text, 'AAEmail', 'ReceipientEmail')
		SET @text = REPLACE(@text, 'EAFirstName', 'RecipientFirstName')
		SET @text = REPLACE(@text, 'EALastName', 'RecipientLastName')
		SET @text = REPLACE(@text, 'EAEmail', 'ReceipientEmail')
		SET @text = REPLACE(@text, 'StreamName', 'FullStreamName')
		SET @text = REPLACE(@text, 'skillstraininggrants.gov.bc.ca', '@Model.BaseURL')
		SET @text = REPLACE(@text, 'Canada-BC Job Grant', '@Model.ProgramName')
		SET @text = REPLACE(@text, 'CJG ', '@Model.ProgramAbbreviation ')
		SET @text = REPLACE(@text, 'CJGInfo', 'ETGInfo')
		SET @text = REPLACE(@text, 'cjgreimbursement@gov.bc.ca', 'ETGInfo@gov.bc.ca')

		UPDATE NotificationTemplates
		SET 
			EmailSubject = @text
		FROM NotificationTemplates
		WHERE Id = @currentTemplateId;
		
		-- Update the EmailBody -- Old Keywords
		SET @counter = 1;
		SELECT @text = EmailBody FROM NotificationTemplates WHERE Id = @currentTemplateId;

		WHILE CHARINDEX(@textToReplace, @text, 1) > 0
		BEGIN
			SELECT @text = STUFF(@text, 
						CHARINDEX(@textToReplace, @text, 1), 
						LEN(@textToReplace), 
						IIF(@counter%2=0, @evenText, @oddText)),
					@counter = @counter + 1
		END

		SET @text = REPLACE(@text, 'AAFirstName', 'RecipientFirstName')
		SET @text = REPLACE(@text, 'AALastName', 'RecipientLastName')
		SET @text = REPLACE(@text, 'AAEmail', 'ReceipientEmail')
		SET @text = REPLACE(@text, 'EAFirstName', 'RecipientFirstName')
		SET @text = REPLACE(@text, 'EALastName', 'RecipientLastName')
		SET @text = REPLACE(@text, 'EAEmail', 'ReceipientEmail')
		SET @text = REPLACE(@text, 'StreamName', 'FullStreamName')
		SET @text = REPLACE(@text, 'skillstraininggrants.gov.bc.ca', '@Model.BaseURL')
		SET @text = REPLACE(@text, 'Canada-BC Job Grant', '@Model.ProgramName')
		SET @text = REPLACE(@text, 'CJG ', '@Model.ProgramAbbreviation ')
		SET @text = REPLACE(@text, 'CJGInfo', 'ETGInfo')
		SET @text = REPLACE(@text, 'cjgreimbursement@gov.bc.ca', 'ETGInfo@gov.bc.ca')

		UPDATE NotificationTemplates
		SET 
			EmailBody = @text
		FROM NotificationTemplates
		WHERE Id = @currentTemplateId;

		FETCH NEXT FROM @TemplateCursor 
		INTO @currentTemplateId
	END; 

	CLOSE @TemplateCursor ;
	DEALLOCATE @TemplateCursor;
END

UPDATE NotificationTemplates
SET EmailBody = REPLACE(EmailBody, '@Model.ApplicationAdministratorAddress', '@Raw(Model.ApplicationAdministratorAddress)');

UPDATE NotificationTemplates
SET EmailBody = REPLACE(EmailBody, 'float: left;', '');
