/**
 * Display the grant application internal state as a label.
 */
app.component('internalStateLabel', {
  require: {
    ngModelCtrl: 'ngModel'
  },
  bindings: {
    ngModel: '<',
    ngLabel: '<?',
    label: '@?'
  },
  templateUrl: '/content/components/_AngularInternalStateLabel.html',
  controller: function ($scope, $element, $attrs) {
    var $ctrl = this;

    this.$onInit = function () {
    }
  }
});
