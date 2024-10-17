# Canada-BC Job Grant UI

## Requirements

- Node.js v9.11.1
- NPM v5.6.0
- Use NVM

These requirements are installed as NuGet packages, Node.js and NPM does not need to be installed as a global dependency.


### Setup

Once NuGet packages have been restored to your development environment, run `src\CJG.Web.External\UI\npm-install.cmd` and `src\CJG.Web.External\UI\gulp.cmd`. This will ensure that all of the correct packages have been installed and the UI assets have been built.


### Development

To compile UI assets, run `src\CJG.Web.External\UI\gulp.cmd`

If you'd like to run a watch task, so that modifying files will trigger a re-compile, run the following `gulp dev` command from the UI folder, in PowerShell or CMD.exe:

```
C:\Projects\CJG - Canada Job Grant\src\CJG.Web.External\UI>gulp dev
```


### Overview

The UI project consists of the following files and folders:


- `scss` - Sass files to define the "look and feel" of a website. [Gulp][3] will compile Sass files to CSS and generated files will be saved to the `CJG.Web.External\css` folder. Sass files should be written using the scss syntax.

- `javascript` - Gulp will compile JavaScript files using Webpack and generated files will be saved to the `CJG.Web.External\js` folder. JavaScript can be written using the [CommonJS][2] format, allowing JavaScript files to export modules which can then be imported and reused in other modules. Webpack can also require modules that are installed with NPM and stored in the `node_modules` directory.

- `node_modules` - This folder contains modules installed by NPM and can be ignored. This folder should not be added to source control.

- `.editorconfig` - This file helps define and maintain coding files for UI related assets. [EditorConfig][1] needs to be installed in your text editor or IDE before it can be used.

- `gulp.cmd` - This script should be used to run Gulp, it is configured to use the locally installed Node and NPM packages that were installed by NuGet.

- `npm-install.cmd` - This script should be used to install node modules, it's configured to use the locally installed Node and NPM packages that were installed by NuGet.

- `gulpfile.js` - Gulp task file.

- `webpack.config.js` - Webpack configuration file.

There are some additional files located in the `CJG.Web.External\Scripts` folder. It contains jQuery as well some dependencies required for .Net development. Webpack is configured to use jQuery as an external dependency.


### Kendo UI

This project uses [Kendo UI][4] when interactive UI components are needed. Kendo UI is located in the `CJG.Web.External\KendoUI` folder, and consists of the following directories:

- `js` - Contains the complete Kendo UI library as well as a custom build that this project currently uses.
- `styles` - Contains the required CSS files, including a "Bootstrap"-style theme.
- `sources` - Kendo UI Professional source files. We generate a custom build of Kendo UI based on the components that we need. By using a custom build, we reduce the file size by over 90% of the full library. This folder contains a README which provides more information on how to generate a custom build of Kendo UI.


### Gulp

This project uses the following Gulp tasks:

- `default` - Runs Clean, Sass, and Webpack sub-tasks.
- `dev` - Runs the default task as well as watches Sass and JavaScript files and will re-run Gulp when file changes are detected.
- `release` - Currently not used, but can run additional tasks like minification, optimization etc.


### Sass

[Sass][6] files are organized into the following files and folders:


- `styles.scss` - This is the primary entry file, partial files should be added here. This will be be compiled to `css/styles.css`.

- `mixins` - This folder contains partial files related to custom Sass Mixins and Functions.

- `partials` - This folder contains general styles such as buttons, forms, tables, layout, typography elements.

- `components` - This folder contains styles related to components such as Modals, Panel Bar, Data Tables, Header, Footer, and other re-usable elements.

- `_settings.scss` - Sass Variables should be added to this file.

- `_mixins.scss` - Additional mixin files can be added to this file.

- `_grid-settings.scss` - Grid settings and default breakpoints are defined in this file.

- `_colors.scss` - Colors are defined in this file. A function is available for use with the Semantic Colors added here. e.g. `color('primaryText')`


### JavaScript

JavaScript files are organized like how .Net MVC separates the application into Areas.

- `external` - JavaScript files related to the External, public facing site. Files defined in the entry file `index.js` will be compiled into the `js/external.js` bundle.

- `internal` - JavaScript files related to the Internal, administration site. Files defined in the entry file `index.js` will be compiled into the `js/internal.js` bundle.

- `part` - JavaScript files related to the Participant Information Collection workflow. Files defined in the entry file `index.js` will be compiled into the `js/participant-confirmation` bundle.

- `shared` - General JavaScript files that can be used in any bundle.

- `vendor` - Third party libraries that can't be installed through NPM.

JavaScript files that are used in more than one entry file are automatically bundled in `js/common.js` using the Webpack [CommonsChunkPlugin][5].




[1]: http://editorconfig.org

[2]: https://webpack.github.io/docs/commonjs.html

[3]: http://gulpjs.com

[4]: http://demos.telerik.com/kendo-ui

[5]: https://webpack.github.io/docs/list-of-plugins.html#commonschunkplugin

[6]: http://sass-lang.com/guide
