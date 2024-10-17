# Internal Application
The internal application is used by the ministry with IDIR accounts to manage and support the assessment and workflow of the applications.

The internal STG application has the following features;
- [Intake Queue](#intake-queue)
- [Work Queue](#work-queue)
- [Expense Approval](#expense-approval)
- [Training Provider Inventory](#training-provider-inventory)
- [Payment Requests](#payment-request)
  - [Issue Payments](#issue-payments)
  - [Reconcile CAS](#reconcile-cas)
- [Management](#management)
  - [Intake](#intake)
  - [Claims](#claims)
  - [Grant Programs](#grant-programs)
  - [Grant Streams](#grant-streams)
  - [Grant Openings](#grant-openings)
  - [Service Descriptions](#service-descriptions)
  - [Notification Types](#notification-types)
  - [Program Notifications](#program-notifications)
  - [Communities](#communities)
  - [Organization Profile Owners](#organization-profile-owners)
  - [Users](#users)

---
## Application Workflow
The application workflow provides a consistent secure process for managing grant applications.

1. Configure
    - Grant Program
    - Grant Streams
    - Grant Openings
1. *External*: Applicant Submits Grant Application
1. Application Assessment
   - Select for Assessment
   - Begin Assessment
   - Validate Training Provider(s)
   - Recommend for Approval/Denial
     - Deny Application
     - Issue Offer
     - Return to Assessment
1. *External*: Applicant Accepts/Rejects Agreement
1. *External*: Reports Participants
1. *External*: Submits Change Request
1. Change Request Assessment 
   - Validate Training Provider(s)
   - Recommend for Approval/Denial
   - Approve/Deny
1. *External*: Reports Claim
1. Claim Assessment 
   - Select for Assessment
   - Assess Eligibility
   - Assess Reimbursement
   - Approve/Deny
1. Issue Payment Request/Amount Owing
1. *External*: Completion Reporting
1. Reconcile Payment Requests with CAS

---
## STG Features
The following features compose all functionality within the internal application.

The internal application requires a valid IDIR account, and in environments where SiteMinder is used it may require a valid TEST IDIR account.

### Intake Queue
The Intake Queue provides a quick way to view a list of all grant applications that are in the *New* or *Selectd for Assessment* states.
This provides a way to begin assessment and assigning assessors to grant applications.

### Work Queue
The Work Queue provides a way to filter and view a list of grant applications based upon various properties (i.e. state, grant program, grant stream, grant opening, assessor, fiscal period).
This provides a way to view applications, assess applications, assess change requests, assess claims, and view completion reports.

### Expense Approval
The Director Expense Approval dashboard provides a way to Issue Offers to multiple grant applications that are Recommended for Approval.
This provides an efficient way to mass approve grant applications within a specific filter.

### Training Provider Inventory
The Training Provider Inventory management tool provides a way to identify and document both approved and unapproved training providers.
These training providers are used when validation grant application training providers.
These training providers also provide a way to report on their historys within grant applications.

### Payment Requests
The Payment Requests section is two-fold.  First, for issuing Payment Requests for approved Claims.
Second, to Reconcile with CAS to ensure the correct amount was paid or received.

#### Issue Payments
The Issue Payments dashboard provides a way to generate formal Payment Requests (PDF) that will be sent to another ministry to issue a cheque or an amount owing request.
Additionally all past payment requests can be viewed and duplicate Payment Requests can be generated.

#### Reconcile CAS
The Reconcile CAS dashboard provides a way to import CAS Payment Reports (Journal entries) and to compare them to STG Payment Requests.
This then generates a report that will provide additional information in relation to what is reconciled and what requires additional actions in-order to reconcile.

### Management
The management section of the application provides a way for the Director or System Administrator to configure various aspects.

#### Intake
The Intake Dashboard provides a way to report on the current state of the budget for any given Grant Opening.
This will allow the Director to make decisions about funding and the state of the current opportunities related to submitted applications.

#### Claims
The Claims Dashboard provides a way to report on the current state of the budget for any given Grant Opening.
This will allow the Director to make decisions about funding and the state of the current opportunities related to approved applications.

#### Grant Programs
The Grant Program management dashboard provides a way for the Director to configure and manage Grant Programs.
Grant Programs are used to configure a group of related Grant Streams.
Grant Programs provide application workflow, notification and document configuration.

There are two current Grant Programs; Employer Training Grants (ETG) and Community Workforce Response Grants (CWRG).
There are two current types of Programs; Employer Grants and WDA Services.
These two types of programs use significantly different configurations, although the backend framework that support both is similar.

Presently creating new Grant Programs through this dashboard is not fully supported, although it is possible with some manual configuration.

#### Grant Streams
The Grant Stream management dashboard provides a way for the Director to configure and manage Grant Streams
Grant Streams are used to configure a group of Grant Openings.
Grant Streams provide budget limits and application workflow configuration.

#### Grant Openings
The Grant Opening management dashboard provides a way for the Director to configure available openings and when they are published and closed.
Grant Openings provide a way to manage budgets for a given training period.
There are three training periods schedule within a given fiscal year.

These openings appear to external applicants when they are in a Published state.
The applicants are able to submit their applications when the opening is in an Open state.

Grant Openings state is automatically managed by their configure dates.  A daily job is run to maintain their state.

#### Service Descriptions
The Service Descriptions management dashboard provides a way for the Director to configure application workflow for WDA Services.
These Services provide the eligible expense types that an applicant can apply for grants.

It should be understood that this dashboard provides a master list of Service Categories that may or may not directly affect current Grant Programs and Grant Streams (based on their current state).

Ideally Service Descriptions are configured prior to Implementing a new Grant Program or Grant Stream.  As editing these configuration details during live Grant Openings is generally limited.

#### Notification Types
The notification Types management dashboard provides a way for the Director to create and configure notifications that are sent to applicants during the lifecycle of their grant applications.

There are currently two types of notifications; workflow and scheduled.  
Workflow notifications are automatically sent during the application workflow lifecycle when the application state changes.
Scheduled notifications are automatically sent by the daily Notification Service Job when specific milestones and details of their respective grant application are match.

All notifications must be linked to a Grant Program before they are sent to applicants.

#### Program Notifications
The Program Notifications management dashboard provides a way for the Director to send out mass emails to BCeID users who are registered as applicants within the solution.

This type of notification is primarily used for announcements that are not related to specific grant applications.

#### Communities
The Communities management dashboard provides a way for the Director to edit the list of active communities used in a WDA Services Grant Program.

#### Organization Profile Owners
The Organization Profile Owners management dashboard provides a way for an Assessor to assign the current Owner (administrator) of a specific Organization.
This is used to allow only specific Applicants to edit/manage their Organization Profile.

#### Users
The Users management dashboard provides a way for the System Administrator and Director to add/update internal IDIR account users.