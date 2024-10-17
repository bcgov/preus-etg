/**
 * Creates a list of options with dynamic options.
 */
app.component('options', {
  transclude: true,
  require: {
    ngModelCtrl: 'ngModel'
  },
  bindings: {
    ngModel: '=?',
    ngItems: '<',
    options: '@?',
    ngClick: '&?',
    error: '@?',
    ngError: '<?',
    label: '@?',
    ngLabel: '<?',
    ngDisabled: '<?',
    ngEditing: '<?',
    name: '@?',
    key: '@',
    value: '@'
  },
  templateUrl: '/content/components/_AngularOptions.html',
  controller: function ($scope, $element, $attrs, Utils) {
    var $ctrl = this;
    var $input = $element.find('input[type=\'radio\']');

    this.$onInit = function () {
      if (!$attrs.ngItems) console.error('Component attribute "ng-items" is required.');
      if (!$attrs.key) console.error('Component attribute "key" is required.');
      if (!$attrs.value) console.error('Component attribute "value" is required.');
      if (typeof ($attrs.ngModel) === 'undefined' && typeof ($attrs.name) === 'undefined') console.error('Component attribute "ng-name" or "ng-model" is required.');

      if (typeof ($attrs.name) === 'undefined' && typeof ($attrs.ngModel) !== 'undefined') $ctrl.name = Utils.replaceAll($attrs.ngModel, /\./, '-');
    }

    this.$postLink = function () {
      Utils.copyAttributes($attrs, $input, ['options', 'label', 'error', 'name', 'key', 'value']);
    }

    this.getError = function () {
      var path = $attrs.ngModel;
      if (path.startsWith('model.')) path = path.replace('model.', '');
      return $ctrl.error || $ctrl.ngError || Utils.getValue($scope.$parent.errors, path);
    }

    this.editing = function () {
      return $ctrl.ngEditing || $scope.$parent.section.editing;
    }

    this.onClick = function ($event, item) {
      if ($ctrl.ngModel != null && typeof ($ctrl.ngModel) === typeof (item)) $ctrl.ngModel = item;
      else if ($ctrl.key) $ctrl.ngModel = item[$ctrl.key];
      if (typeof ($ctrl.ngClick) === 'function') $ctrl.ngClick($event, item);
    }
  }
});
