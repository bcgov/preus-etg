var directiveLink = require('../directive-link').default;

global.app.directive("ngPercent", function ($filter) {
  return {
    require: 'ngModel',
    link: function (scope, ele, attr, ctrl) {
      directiveLink(scope, ele, attr, ctrl, parse, format);

      function parse(viewValue) {
        var value = viewValue.replace(/,/g, '');
        var m = value.match(/^(\d+)\/(\d+)/);
        if (m != null)
          return (parseInt(m[1]) / parseInt(m[2])).toFixed(2);
        return (parseFloat(value.replace(/.*?(([0-9]*\.)?[0-9]+).*/g, "$1")) / 100).toFixed(4);
      }

      function format(modelValue) {
        return $filter('number')(parseFloat(isNaN(modelValue) ? 0 : modelValue) * 100, 2) + '%';
      }
    }
  };
});
