/**
 * Adds the Edit button.
 * When the Edit button is clicked it will then be replaced with Cancel and Save buttons.
 * By default these buttons call the parent $scope.edit(), $scope.cancel() and $scope.save() functions.
 */
app.component('sectionEditButtons', {
  require: {
    ngModelCtrl: 'ngModel'
  },
  bindings: {
    ngModel: '<',
    ngEdit: '&?',
    ngCancel: '&?',
    ngSave: '&?',
    ngClass: '@',
    ngCanSave: '<'
  },
  templateUrl: '/content/components/_AngularSectionEditButtons.html',
  controller: function ($scope, $element, $attrs) {
    var $ctrl = this;
    this.removeSave = false;  // Add ng-Removesave="true" to <section-edit-buttons> to remove the save button from view
    if (typeof ($attrs.ngRemovesave) != 'undefined' && $attrs.ngRemovesave === 'true') this.removeSave = true;

    this.$onInit = function () {
      if (typeof ($ctrl.ngCanSave) === 'undefined') $ctrl.ngCanSave = true;
    }

    this.edit = function ($event) {
      if (typeof ($ctrl.ngEdit) === 'function') $ctrl.ngEdit($event);
      else if (typeof ($scope.$parent.edit) === 'function') $scope.$parent.edit($event);
    }

    this.cancel = function ($event) {
      if (typeof ($ctrl.ngCancel) === 'function') $ctrl.ngCancel($event);
      if (typeof ($scope.$parent.cancel) === 'function') $scope.$parent.cancel($event);
    }

    this.save = function ($event) {
      if (typeof ($ctrl.ngSave) === 'function') $ctrl.ngSave($event);
      if (typeof ($scope.$parent.save) === 'function') $scope.$parent.save($event);
    }
  }
});
