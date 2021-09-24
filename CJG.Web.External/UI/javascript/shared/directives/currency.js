var directiveLink = require('../directive-link').default;

global.app.directive("ngCurrency", function ($filter) {
  return {
    require: 'ngModel',
    link: function (scope, ele, attr, ctrl) {
      var maxLength = parseInt(attr.ngCurrency) || 9;
      var template = "99999999999999999999.99";
      var maxValue = parseFloat(template.slice(template.length - (maxLength + 1)));

      directiveLink(scope, ele, attr, ctrl,
        function (viewValue) {
          if (typeof (viewValue) !== 'string') return null;
          var value = viewValue.replace(/,/g, '').replace(/[^\d.-]/g, '');
          var decimalNumber = attr.ngDecimal ? attr.ngDecimal : 2;
          var floatNumber = parseFloat(value).toFixed(decimalNumber);
          var isNegative = !isNaN(floatNumber) && floatNumber < 0;
          floatNumber = Math.abs(floatNumber).toFixed(decimalNumber);
          if (attr.ngCurrency != null && attr.ngCurrency !== '' && floatNumber.length > attr.ngCurrency) {
            floatNumber = floatNumber.substring(floatNumber.length - attr.ngCurrency);
          }
          return isNegative ? '-' + floatNumber : floatNumber;
        },
        function (modelValue) {
          if (typeof (modelValue) !== 'string') return null;
          var value = modelValue.replace(/,/g, '').replace(/[^\d.-]/g, '');
          value = isNaN(value) ? 0 : value;
          var decimalNumber = attr.ngDecimal ? attr.ngDecimal : 2;
          var floatNumber = parseFloat(value).toFixed(decimalNumber);
          var isNegative = !isNaN(floatNumber) && floatNumber < 0;
          floatNumber = Math.abs(floatNumber).toFixed(decimalNumber);
          if (attr.ngCurrency != null && attr.ngCurrency !== '' && floatNumber.length > attr.ngCurrency) {
            floatNumber = floatNumber.substring(floatNumber.length - attr.ngCurrency);
          }
          return (isNegative ? '-' : '') + '$' + $filter('number')(floatNumber, decimalNumber);
        }
      );

      var original = {};

      ele.on('change', function () {
        format();
      });

      ele.on("keydown", function (event) {
        var viewValue = ctrl.$viewValue;
        if (viewValue) {
          var value = Number(viewValue.replace(/[^0-9.-]+/g, ""));
          var key = String.fromCharCode(event.keyCode);
          var isNumber = event.keyCode !== 9 && (!isNaN(key) || key === '.');
          if (isNumber && value > maxValue) {
            return false;
          }
        }
      });

      function format() {
        var formatters = ctrl.$formatters,
          idx = formatters.length;

        var viewValue = ctrl.$$rawModelValue;
        while (idx--) {
          viewValue = formatters[idx](viewValue);
        }

        ctrl.$setViewValue(viewValue);
        ctrl.$render();
      }
    }
  };
});
