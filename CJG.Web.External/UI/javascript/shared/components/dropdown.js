/**
 * Creates a select element with dynamic options.
 */
app.component('dropdown', {
  transclude: true,
  require: {
    ngModelCtrl: 'ngModel'
  },
  bindings: {
    ngModel: '=',
    ngChange: '&?',
    options: '@',
    ngItems: '<',
    ngFilter: '<?',
    error: '@?',
    ngError: '<?',
    default: '@?',
    label: '@?',
    ngLabel: '<?',
    ngDisabled: '<?',
    ngEditing: '<?',
    ngText: '<?',
    key: '@?',
    value: '@?'
  },
  templateUrl: '/content/components/_AngularDropdown.html',
  controller: function ($scope, $element, $attrs, $filter, $timeout, Utils) {
    var $ctrl = this;
    var $select = $element.find('select');
    var displayText = true;

    this.$onInit = function () {
      if (!$attrs.ngItems) console.error('Component attribute "ng-items" is required.');
      if (!$attrs.options) console.error('Component attribute "options" is required.');

      if (typeof ($attrs.noText) !== 'undefined') displayText = false;
      if (typeof ($attrs.ngText) === 'undefined' && displayText) $ctrl.ngText = getText();
      if (typeof ($attrs.name) === 'undefined' && typeof ($attrs.ngModel) !== 'undefined') $ctrl.name = Utils.replaceAll($attrs.ngModel, /\./, '-');
    }

    $scope.$watch('$ctrl.ngModel', function (newValue, oldValue) {
      if (newValue !== oldValue && displayText) {
        $ctrl.ngText = getText();
      }
    });

    $scope.$watch('$ctrl.ngItems', function (newValue, oldValue) {
      if (newValue !== oldValue && displayText) {
        $ctrl.ngText = getText();
      }
    });

    function getText() {
      if (!$ctrl.ngItems) return;
      var result = $ctrl.ngItems.find(function (item) {
        return item === $ctrl.ngModel || ($ctrl.key && Utils.getValue(item, $ctrl.key) === Utils.getValue($ctrl.ngModel, $ctrl.key) || Utils.getValue(item, $ctrl.key) === $ctrl.ngModel);
      });
      return result ? result[$ctrl.value] || result : result;
    }

    this.$postLink = function () {
      Utils.copyAttributes($attrs, $select, ['options', 'default', 'label', 'error', 'key', 'value', 'no-text']);
    }

    this.editing = function () {
      return $ctrl.ngEditing || $scope.$parent.section.editing;
    }

    this.getError = function () {
      var path = $attrs.ngModel;
      if (path.startsWith('model.')) path = path.replace('model.', '');
      return $ctrl.error || $ctrl.ngError || Utils.getValue($scope.$parent.errors, path);
    }

    $scope.$watch('$ctrl.ngModel', function (newValue, oldValue) {
      if (newValue !== oldValue) {
        Utils.action($ctrl.ngChange);
      }
    });
  }
});
