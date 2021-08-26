/**
 * Creates a list of checkboxes with dynamic options.
 */
app.component('checkboxes', {
  transclude: true,
  bindings: {
    ngModel: '=?',
    ngItems: '<',
    options: '@?',
    ngChange: '&?',
    ngClick: '&?',
    ngChecked: '<?',
    error: '@?',
    ngError: '<?',
    label: '@?',
    ngLabel: '<?',
    ngDisabled: '<?',
    ngEditing: '<?',
    name: '@?',
    key: '@',
    value: '@',
    flag: '@?'
  },
  templateUrl: '/content/components/_AngularCheckboxes.html',
  controller: function ($scope, $element, $attrs, Utils) {
    var $ctrl = this;
    var $checkboxes = $element.find('input[type=\'checkbox\']');

    this.$onInit = function () {
      if (!$attrs.ngItems) console.error('Component attribute "ng-items" is required.');
      if (!$attrs.key) console.error('Component attribute "key" is required.');
      if (!$attrs.value) console.error('Component attribute "value" is required.');
      if (typeof ($attrs.ngModel) === 'undefined' && typeof ($attrs.name) === 'undefined') console.error('Component attribute "ng-name" or "ng-model" is required.');

      if (typeof ($attrs.name) === 'undefined' && typeof ($attrs.ngModel) !== 'undefined') $ctrl.name = Utils.replaceAll($attrs.ngModel, /\./, '-');
      if (typeof ($attrs.flag) === 'undefined') $ctrl.flag = 'isChecked';
    }

    this.$postLink = function () {
      Utils.copyAttributes($attrs, $checkboxes, ['options', 'label', 'error', 'name', 'key', 'value']);
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
      if (typeof ($ctrl.ngClick) === 'function') $ctrl.ngClick($event, item);
    }

    this.onChange = function (item) {
      if (typeof ($ctrl.ngChange) === 'function') $ctrl.ngChange(item);
    }
  }
});
