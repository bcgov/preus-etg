var utils = require('../utils.js');

/**
 * Display the grant application program title label.
 */
app.component('programTitleLabel', {
  require: {
    ngModelCtrl: 'ngModel'
  },
  bindings: {
    ngModel: '<'
  },
  templateUrl: '/content/components/_AngularProgramTitleLabel.html',
  controller: function ($scope, $element, $attrs, Utils) {
    var $ctrl = this;

    this.utils = utils;

    this.$onInit = function () {
    };

    this.toPST = Utils.toPST;
  }
});
