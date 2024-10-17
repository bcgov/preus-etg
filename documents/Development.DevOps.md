# Development DevOps
Development is supported by TFS and GIT.  These tools provide a consistent way to define requirements, assign tasks, review code changes, automate testing, and automate deployment.

## TFS Configuration
TFS is currently configured to automatically build, test and deploy the application to the configured environments based upon merging code to specific branches within GIT.

All deployments require approval by an appropriate resource.

During each planned release it is important to setup/configure GIT branches and QA environments.

## Builds
Due to the original design and configuration of the solution, every environment also has its own build artifacts.
This was done to support various compile time differences.  It would be ideal in the future to correct this design implementation to reduce the number of different artifacts.

TFS is configured to build the appropriate artifact for each environment.

TFS is configured to automatically generate builds when specific GIT branches are updated.

```
** It is important to update the assembly versions of the various projects within the solution (specifically the web application).
This is because this version number is used to maintain local browser cache settings.
```

## Releases
TFS is configured to automatically generate a release when a bulid is completed successfully.  
These releases require the Release Managers approval before they are deployed to the environment.

Each release has configured *actions* that provide a quick way to backup their database, refresh their databasse, restore their database.
This was done to speed up deployments and assist testing.
While they are effective, this specific configuration uses TFS *environments* incorrectly and as such causes some annoying headaches when managing TFS and drive space.

Each release is configured to first backup the current DB so that if a failure occurs it can automatically rollback to the prior version.
This is especially important and useful in the Production environment.

## Branch Workflow
There are primarily three active branches; `master`, `release` and `feature`.  
The `master` branch is the **production** branch and the source of all development.
The `release` branch is used to deploy to **QA** and provides the source for the current iteration.  
The `feature` branches are used by developers and used to develop and test an assigned task/story before submitting a Pull Request (PR) to merge to the `release` branch.

Each new iteration will clone the `master` branch and create a named development branch (i.e. `v01.09.00-develop`).
Then all developers will clone this new branch to create their `feature` branches for their user stories in the iteration.
Developer branches should contain a unique name that identifies what the branch is for (i.e. their name and/or the user story it is related to. `devname/v01.09.00-develop-12345`).

Once a developer completes their `feature` they will create a **Pull Request** (PR) from their `feature` branch to the `development` branch.
They will also associate the appropriate tickets to this PR.
Once it is successfully merged the *Team Lead* will create a new PR from the `development` branch to the `release` branch.
This is done to deploy the latest code to QA.
The reason `feature` branches are first merged to the `development` branch is to limit the number of daily releases to the QA environments.  Ideally there will only be one release per day.

After QA signs off on an iteration, the *Team Lead* will create a PR from the `release` branch to the `master` branch.
One finally review will occur to ensure the code is ready for deployment.
Once the PR is approved and the build is successful the code can be released to the **TEST** environment for User Acceptance Testing (UAT).
The `master` branch is then used to deploy to the following environments; TEST, TRAINING, PRODUCTION and SUPPORT.

![branch workflow diagram](./Documents/BranchWorkflow.PNG)

## Release Cleanup
Once a release has been deployed to production it is important to do the following clean up activities;
- Delete any branches that are no longer needed
- Update any active branches with the `master` branch
- Tag the `master` branch with the released *version* number (i.e. v01.14.00).  This is important if you ever need to test a specific version.
- Close any remaining TFS Epics, Features, User Stories, Tasks, Bugs.
- Confirm that the three services are running successfully (next business day);
  - Grant Opening Service
  - Notification Service
  - Reporting Service