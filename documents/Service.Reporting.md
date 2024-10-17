# Reporting Service
The Reporting Service is a Windows Console application that is run nightly by a *Windows Scheduled Task*.

The Reporting Service generates a specific report for participants that have submitted their PIF and are unemployeed and receiving Employment Insurance.

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
| applicationSettings:CsvAddReportHeader | Whether to include the header names for each column in the CSV. [True-False], Default: True |
| applicationSettings:NumDaysBefore | The number of days before the run date that participants will be included in the report.  Generally the report only includes participants reported 'today'. [0-*], Default: 0 |
| applicationSettings:MaxParticipants | The maximum number of participants to include in the report.  [0-*], Default: 1000 |
| applicationSettings:CsvFilePathTemplate | The name of the CSV file that will be generated. Default: `report_output\participant-report-{0:yyyy-MM-dd}.csv` |
| applicationSettings:HtmlFilePathTemplate | The name of the HTML file that will be generated.  Default: `report_output\participant-report-{0:yyyy-MM-dd}-::ParticipantFormId::.html` |

## Testing
The internal STG web application has a way to manually test this service via the 'Debug' menu.
You will need to login as a System Administrator to have access to this tool