/**
 * Creates a textarea input.
 */
app.component('textareablock', {
  require: {
    ngModelCtrl: 'ngModel'
  },
  bindings: {
    ngModel: '=',
    ngChange: '&?',
    error: '<?',
    default: '@?',
    label: '@?',
    ngLabel: '<?',
    ngDisabled: '<?',
    ngEditing: '<?'
  },
  templateUrl: '/content/components/_AngularTextArea.html',
  controller: function ($scope, $element, $attrs, $filter, $timeout, Utils) {
    var $ctrl = this;
    var $input = $element.find('textarea');

    this.$onInit = function () {
      if (typeof ($attrs.name) === 'undefined' && typeof ($attrs.ngModel) !== 'undefined') $ctrl.name = Utils.replaceAll($attrs.ngModel, /\./, '-');
    }

    this.$postLink = function () {
      Utils.copyAttributes($attrs, $input, ['default', 'label', 'error']);
    }

    this.getError = function () {
      var path = $attrs.ngModel;
      if (path.startsWith('model.')) path = path.replace('model.', '');
      return $ctrl.error || $ctrl.ngError || Utils.getValue($scope.$parent.errors, path);
    }

    this.editing = function () {
      return $ctrl.ngEditing || $scope.$parent.section.editing;
    }
  }
});
