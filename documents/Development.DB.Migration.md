# Database Migration
The STG application uses Code-First database development with Entity Framework 6.  
Along side the features provided by this framework and implementation are custom enhancements to improve and expand upon the DB migration.
These enhancements provide a consistent and reproducable migration plan that allows developers and environments to quickly setup specific versions of the solution.

The CJG.Infrastructure.EF project contains all the DB migration related code.

Use the Package Manager console to perform the various tasks of setting up and running migration plans.  
DevOps is already configured to run these migration automatically when deploying releases.

## Package Manager Console
`update-database` - This command will execute all the current DB migrations to get the configured DB up-to-date.
`add-migration [name]` - This command will generate a new migration plan with the specified name.

## Release Process
Every release that requires any database changes, even data changes without structure changes should be contained within an EF migration plan.

Every release can only run a single EF migration plan upgrade.  
Multiple migrations during development must be merged into a single migration plan.

### Migrations 
Each migration can include SQL scripts.
These scripts will be executed in the configured sequence and allow for complex changes.

It is possible, but not required to also include rollback scripts.  This feature is not used and would require additional TFS configuration to implement.

These scripts must be placed in an appropriately named folder structure and referenced by the EF migration plan.

The scripts are executed by the `PreDeployment()`, `PrePostDeployment()`, `PostDeployment()` and `DeployScripts(...)`.

All migrations are executed within a single transaction.  Any failure will rollback any currently executed steps.

### Steps
The following steps are required to create an EF migration that will be applied by TFS during deployment of a release.

1. Create a new EF Migration
   - `add-migration v01.09.00`
2. Update the migration code to inherit from `ExtendedDbMigration`
3. Add a `DescriptionAttribute` with the specific migration version (i.e. v01.09.00)
4. Include the appropriate `PreDeployment()` and `PostDeployment()` function calls within the `Up()` function
5. Create a folder with the specified migration verions in the project */Migrations* folder
   - Add the appropriate sub folders for each type of script
6. Run the EF migration and update the database
   - `update-database`

