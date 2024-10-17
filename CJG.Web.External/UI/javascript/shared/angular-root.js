global.app = angular.module('grantApp', ['ngSanitize', 'ngDialog', 'angularMoment', 'ngLoadingOverlay', 'ui.tinymce']);
global.app.constant('moment', require('moment-timezone'));
global.app.constant('angularMomentConfig', {
  timezone: 'America/Vancouver'
});
global.regexIso8601 = /^\/Date\(-?(\d+)(?:-(\d+))?\)\/$/;

app.config(['ngDialogProvider', function (ngDialogProvider) {
  ngDialogProvider.setDefaults({
    className: 'ngdialog-theme-default',
    appendTo: 'body',
    closeByDocument: false
  });
}]);

app.config(['$loadingOverlayConfigProvider', function ($loadingOverlayConfigProvider) {
  $loadingOverlayConfigProvider.defaultConfig('<b>Loading...</b>');
  var defaults = $loadingOverlayConfigProvider.$get().get();
}]);

require('./filters');
require('./directives');
require('./components');

