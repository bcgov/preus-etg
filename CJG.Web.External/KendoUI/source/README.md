# Kendo UI

The contents of this directory is part of the Telerik Kendo UI Professional source code. It is not open source and should not be distributed or used in other projects.

The source files are available to download by signing in to the Telerik account which is associated with the above license.

## Custom build

To generate a custom build of Kendo UI containing only the components you need, following these instructions:

* Download and install Node.js and NPM
* Open a command prompt and navigate to the `KendoUI/source/src` folder (the folder that contains the `package.json` file)
* run the command `npm install`
* run the command `npm run build:js` to generate a custom JavaScript bundle.
* TODO: run the command `npm run build:css` to generate a custom CSS bundle.

If you need edit the components that are added to the custom JavaScript bundle, edit the `package.json` file and file the `build:js` command.