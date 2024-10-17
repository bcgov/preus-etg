# Notification Service
The Notification Service is a Windows Console application that is run nightly by a *Windows Scheduled Task*.

The Notification Service reviews the Notification Queue and for each notification that is queued or failed it will review whether it should be resent.
If it should be resent it will email the notification to the configured receipient(s).

If the Notification Service does not run all Scheduled Notifications and Program Notifications will not be sent.

The Notification Service uses SMTP to send emails via the ministries email service.

## Logging
NLog is configured to create log files for this service.

## Configuration
The following configuration settings are required to run this application.
Configure these in the `App.config` file.

TFS build/release will overwrite the appropriate configured configuration settings for each environment.

| Configuration | Description |
|---------------|-------------|
| connectionString | The connection string to the database.  This should be automatically configured by TFS in the build/release process. |
| applicationSettings:SiteUrl | This is required to initialize the dependency injection |
| applicationSettings:EmailSenderTimeout | The amount of time before an attempt to send an email should fail. [Timespan], Default: 00:00:05 |
| applicationSettings:MaxEmailSendAttempts | The maximum number of attempts after a failure.  [0-*], Default: 5 |
| applicationSettings:SmtpServer | The domain name of the SMTP server.  Default: apps.smtp.gov.bc.ca |
| applicationSettings:DefaultSenderAddress | The default email address of the 'FROM'.  Default: noreply.skillstraininggrants@gov.bc.ca |
| applicationSettings:DefaultSenderName | The default name of the sender of the email.  Default: Skills Training Grants Support |

## Testing
The internal STG web application has a way to manually test this service via the 'Debug' menu.
You will need to login as a System Administrator to have access to this tool