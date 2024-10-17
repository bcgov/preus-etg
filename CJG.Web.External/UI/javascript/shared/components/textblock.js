/**
 * Creates a textbox input.
 */
app.component('textblock', {
  require: {
    ngModelCtrl: 'ngModel'
  },
  bindings: {
    ngModel: '=',
    ngChange: '&?',
    error: '@?',
    ngError: '<?',
    default: '@?',
    label: '@?',
    ngLabel: '<?',
    ngDisabled: '<?',
    ngEditing: '<?',
    ngClass: '<?',
    ngRequired: '<?',
    ngMinlength: '<?',
    ngMaxlength: '<?',
    ngPattern: '<?',
    ngTrim: '<?',
    name: '@?'
  },
  templateUrl: '/content/components/_AngularText.html',
  controller: function ($scope, $element, $attrs, $filter, $timeout, Utils) {
    var $ctrl = this;
    var $input = $element.find('input[type=\'text\']');

    this.$onInit = function () {
      if (typeof ($attrs.name) === 'undefined' && typeof ($attrs.ngModel) !== 'undefined') $ctrl.name = Utils.replaceAll($attrs.ngModel, /\./, '-');
      if ($ctrl.default && !$ctrl.ngModel) $ctrl.ngModel = $ctrl.default;
    }

    this.$postLink = function () {
      Utils.copyAttributes($attrs, $input, ['default', 'label', 'error', 'name']);
      Utils.addAttributes($attrs, $input, ['ngNumber', 'ngPostal']);
    }

    this.editing = function () {
      return $ctrl.ngEditing || $scope.$parent.section.editing;
    }

    this.getError = function () {
      var path = $attrs.ngModel;
      if (path.startsWith('model.')) path = path.replace('model.', '');
      return $ctrl.error || $ctrl.ngError || Utils.getValue($scope.$parent.errors, path);
    }
  }
});
