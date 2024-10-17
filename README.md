# Skills Training Grants

Welcome to the Skills Training Grants (STG) solution which replaces the Canada-BC Job Grant and is funded through the negotiated Workforce Development Agreement (WDA). 
[More Information](https://www.workbc.ca/Employer-Resources/Skills-Training/Skills-Training-Programs.aspx)

- B.C. Employer Training Grant (ETG). [More Information](https://www.workbc.ca/Employer-Resources/BC-Employer-Training-Grant.aspx)

- Community Workplace Response Grants (CWRG). [More Information](https://www.workbc.ca/Employment-Services/Community-Workforce-Response-Grant.aspx)

## Hosting, DevOps, Source Control
The project is hosted and managed by [Ministry of Advanced Education, Skills & Training](https://www2.gov.bc.ca/gov/content/governments/organizational-structure/ministries-organizations/ministries/advanced-education-skills-training), within [TFS](http://bonnie.idir.bcgov/tfs/Economy/CJG%20-%20Canada%20Job%20Grant) using [GIT](http://bonnie.idir.bcgov/tfs/Economy/CJG%20-%20Canada%20Job%20Grant/_git/CJG%20-%20Canada%20Job%20Grant%202018) as source control.

---
## Application Overview
The solution is a single web application and three console applications that are run as *Windows Scheduled Tasks*.
The web application is composed of three separate *Areas*, Internal (Int), External (Ext) and Participant (Part).

### Internal (Int)
The internal application is used by the ministry with IDIR accounts to manage and support the assessment and workflow of the applications.

More information [here](./Documents/Application.Internal.md)

### External (Ext)
The external application is used by BCeID user accounts to manage their grant applications for training.

More information [here](./Documents/Application.External.md)

### Participant (Part)
The participant application is an anonymous access application that allows participants to an approved training grant to submit their personal information so that they can participate in a trianing program.

More information [here](./Documents/Application.Participant.md)

---
## Console Applications - Windows Schedule Tasks
These are Windows Console application (jobs) that are run daily to support the solution.  They are run by the *Windows Scheduled Tasks*.

### Grant Opening Service
Runs nightly and updates the state of the Grant Openings based on their Publish, Opening and Closing Dates.

More information [here](./Documents/Service.GrantOpening.md)

### Notification Service
Runs nightly and sends out email notifications to BCeID applicants.  The majority of these notifictions are reminders for the Applicants to complete specific tasks related to their approved grant applications.

More information [here](./Documents/Service.Notification.md)

### Reporting Service
Runs nightly and generates a physical file that contains specific participant information.  It then copies these files to a shared network location for the ministry to download and review.

More information [here](./Documents/Service.Reporting.md)

---
## Environments
There are a number of environments hosted by the ministry to support development and maintenance of the solution.

| Environment   | Description                                               | Information                              |
|---------------|-----------------------------------------------------------|------------------------------------------|
| LOCAL         | Local developer workstation environment for development.  | [here](./Documents/Development.Guide.md) |
| DEV           | Environment for developers to test with SiteMinder.       |
| QA            | Testing environment for the current release.              |
| TEST          | UAT to test and signoff on the release.                   |
| TRAIN         | Training environment to prepare for a release.            |
| PROD          | Production environment.                                   |
| SUPPORT       | Support environment for the application support team.     |

More information [here](./Documents/Environments.md).

## Security
The application security is managed by a claim based implementation of Microsoft Identity for authorization and SiteMinder for authentication.
Authentication is performed by SiteMinder BCeID for the external site and IDIR for the internal site.

## SiteMinder Test Accounts
The environments that use SiteMinder have a number of test accounts that can be used both internally (IDIR) and externally (BCeID).
Internally it's possibly to add your own IDIR account so that you can test.

More information [here](./Documents/BCeID%20test%20accounts.md).

## Roles and Responsibilities
For successful implementation of any release it requires the roles and responsibilities described [here](./Documents/Development.Roles.Responsibilities.md)

## Development
Refer to the [Development Guide](./Documents/Development.Guide.md) to know how to setup a workstation to develop new features, or support the current application.

