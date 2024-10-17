app.controller('Sidebar', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId,
    claimId: $attrs.ngClaimId,
    claimVersion: $attrs.ngClaimVersion
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  /**
   * Show modal popup to withdraw application.
   * @function withdrawApplication
   * @returns {Promise}
   **/
  $scope.withdrawApplication = function () {
    return ngDialog.openConfirm({
      template: '/Ext/Application/Withdraw/View/' + $scope.section.grantApplicationId,
      data: {
        title: 'Withdraw Application'
      }
    })
      .catch(angular.noop);
  }

/**
 * Show modal popup to withdraw claim.
 * @function withdrawClaim
 * @returns {Promise}
 **/
  $scope.withdrawClaim = function () {
    return ngDialog.openConfirm({
      template: '/Ext/Claim/Withdraw/View/' + $scope.section.claimId + '/' + $scope.section.claimVersion,
      data: {
        title: 'Withdraw Claim'
      }
    })
      .catch(angular.noop);
  }
});


