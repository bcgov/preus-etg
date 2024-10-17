var utils = require('../../shared/utils');
app.controller('GrantAgreementReviewView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make an AJAX request to fetch the agreement data.
   * @function loadAgreement
   * @returns {Promise}
   **/
  function loadAgreement() {
    return $scope.load({
      url: '/Ext/Agreement/Review/' + $scope.section.grantApplicationId,
      set: 'model'
    })
      .then(function () {
        return $scope.CheckAcceptAgreement();
      });
  }

  $scope.rejectAgreement = function () {
    return ngDialog.openConfirm({
      template: '/Ext/Agreement/Reject/View/' + $scope.section.grantApplicationId,
      data: {
        title: 'Reject Agreement'
      }
    })
      .then(function (reason) {
        return $scope.ajax({
          url: '/Ext/Agreement/Reject',
          method: 'PUT',
          data: function () {
            $scope.model.IncompleteReason = reason;
            return $scope.model;
          }
        })
          .then(function () {
            window.location = '/Ext/Home/Index';
          });
      })
      .catch(angular.noop);
  }

  $scope.acceptAgreement = function () {
    return $scope.ajax({
      url: '/Ext/Agreement/Accept',
      method: 'PUT',
      data: $scope.model
    })
      .then(function () {
        window.location = '/Ext/Home/Index';
      })
      .catch(angular.noop);
  }

  $scope.CheckAcceptAgreement = function () {
    var invalidStates = [
      utils.ApplicationStateInternal.ApplicationWithdrawn,
      utils.ApplicationStateInternal.CancelledByMinistry,
      utils.ApplicationStateInternal.CancelledByAgreementHolder];
    $scope.model.AllowAcceptAgreement = $scope.model.CoverLetterConfirmed
      && $scope.model.ScheduleAConfirmed
      && $scope.model.ScheduleBConfirmed
      && (invalidStates.indexOf($scope.model.ApplicationStateInternal) == -1);
  }

  loadAgreement()
    .catch(angular.noop);
});
