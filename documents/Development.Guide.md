# Development Guide
The purpose of the development guide is for supporting the existing application and building new enhancements.

## Technology and Framework
The solution is a Visual Studio ASP.NET 4.5 Web Application, using Entity Framework 6 and MVC 5, with SQL Server 2016.

There is a mix of jQuery, AngularJs and Razor code to support the front-end

The solution is generally architected in a basic n-tier structure; Repository, Services, Workflow Engine, Controllers, API Endpoints.

## Solution Structure
The solution is currently organized into a few different folders and projects (i.e. Application, Core, Infrastructure, Testing, Web).
This was done at the beginning of the development as a way to organize everything and currently has less significance or reason.

## Unit Testing
Currently the coverage of unit test generally is focused on the Services layer.
There is some test coverage for the controllers.

Building new unit tests to expand coverage should continue to use the custom implementation that can be reviewed in the existing unit tests.
This implementation takes advantage of numerous helper methods that simplify the initialization (*ARRANGE*) step.
Allowing the development of additional unit tests without having tons of boilerplate configuration for each service or controller.

## AngularJS
The majority of the solution has been updated to use AngularJs version 1.7.8.

The UI uses the standard MVC structure provided by AngularJs.

There is a custom framework structure layer beneath AngularJs developed for this solution that enforces efficient, consistent and maintainable UI code.
This structure provides a number of basic features like AJAX, error handling, validation, modal popups, confirmation dialogs and CRUD operations.

## Security
Authentication is performed by SiteMinder (or a simulator for testing), which sets session variables.  
These session variables are then used in conjunction with a custom Microsoft Identity to set the authenticated user.

Authorization is performed by a claim based implementation of Microsoft Identity.  This allows validation of any given action against a users role and privileges.

All access to the datasource, and most actions require authorization validation.  This is almost exclusively performed by the extension method `User.CanPerformAction([object], [trigger])`.
It is important to maintain this design structure to ensure security is implemented consistently throughout the whole solution.

## Services
The Services layer of the solution provides access to the datasource and ensures all actions performed are authorized and executed in a consistent way.

All access to data and workflow requirements must be implemented in the Service layer.

## API Endpoints
The web application UI is controlled through the API endpoints.  These endpoints provide Views and JSON data for the various features.

All endpoints are secured through configured authentication and authorization.

All access to data and workflow is provided through the Services.

All endpoints handle exceptions in a consistent way.  Views will redirect to generic error pages.  JSON data will include appropriate error information.

## Logging
NLog is configured to report all logging information to log files and the DB Logging table.  This configuration can be updated and managed different for each environment.

## Development Lifecycle
Refer to the [DevOps documentation](./Development.DevOps.md) for further information.

## Database Migration
Refer to the [Database Migration documentation](./Development.DB.Migration.md)

## Setup Developer Local Environment
A developer will need the following to be able to run and work on the application within their local workstation.

1) An IDIR account
1) VPN access to the ministries network
1) Access to TFS.  Added to one of the following groups; 
    - AEST_W_TFS_Stakeholders
    - AEST_W_TFS_Basic
    - AEST_W_TFS_VSEnterprise
1) Access to the 'CJG - Canada Job Grant' collection
    - Added to one of the Teams
1) Visual Studio 2015-2019
    - Install Node -v10.16.0
    - Recommended to install NVM
    - Install NPM - 6.9.2
1) Microsoft SQL Server 2012-2017 (optional)
1) Clone the GIT repository that you will be working on
    - You can use Visual Studio to connect to the GIT repository by using the **Team Explorer** -> *Manage Connections* -> *Connect to a Project*
    - Or command line `git clone http://bonnie.idir.bcgov/tfs/Economy/CJG%20-%20Canada%20Job%20Grant/_git/CJG%20-%20Canada%20Job%20Grant%202018`
1) Update the Web.config connection string to point to your local SQL Server database (SQL Express can be used)
   - This is not required if you are using the default configuration options
   - If you do this, do not commit this change to the GIT repo, or you will annoy all other developers
1) Build the solution
1) Create the database by using the **Package Manager Console**
    - Set the *Default Project* to *CJG.Infrastructure.EF*
    - Run the command `update-database`
1) Generate the transpiled static resources (js and css files)
    - This is not required if the solution is configured correctly, as it will perform this task automaticlaly in the Task Runner
    - Open a command prompt or powershell in the **CJG.Web.External** folder
    - Run the command `npm run dev-build`
1) Run the application (press **F5**)
    - By default it should load a testers login page, which allows you to select an account to login as.
    - Internal - http://localhost:[port]/int/auth/login
    - External - http://localhost:[port]/ext/auth/login
