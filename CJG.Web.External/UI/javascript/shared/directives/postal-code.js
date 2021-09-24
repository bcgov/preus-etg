require('../directive-link');

global.app.directive("ngPostalCode", function ($filter) {
  return {
    require: 'ngModel',
    link: function (scope, ele, attr, ctrl) {
      directiveLink(scope, ele, attr, ctrl,
        function (viewValue) {
          var value = viewValue ? viewValue.replace(/(-|\s)/g, '') : '';
          return value;
        },
        function (modelValue) {
          return modelValue ? modelValue.toUpperCase() : '';
        }
      );
    }
  };
});
