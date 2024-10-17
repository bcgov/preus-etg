# Grant Opening Service
The Grant Opening Service is a Windows Console application that is run nightly by a *Windows Scheduled Task*.

The Grant Opening Service reviews all Grant Openings and determines based on their configured dates what the current state of the Grant Opening should be.

## Logging
NLog is configured to create log files for this service.

## Configuration
There is very little configuration required in the *App.config* file.

TFS build/release will overwrite the appropriate configured configuration settings for each environment.

| Configuration | Description |
|---------------|-------------|
| connectionString | The connection string to the database.  This should be automatically configured by TFS in the build/release process. |
| applicationSettings:SiteUrl | This is required to initialize the dependency injection |

## Testing
The internal STG web application has a way to manually test this service via the 'Debug' menu.
You will need to login as a System Administrator to have access to this tool