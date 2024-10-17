app.controller('ClaimWithdrawView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  };

  $scope.model = {
    ClaimId: $attrs.ngClaimId,
    ClaimVersion: $attrs.ngClaimVersion,
    RowVersion: $attrs.ngRowVersion,
    WithdrawReason: null
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  /**
   * Ask user confirmation then send AJAX request to withdraw the claim.
   * @function withdraw
   * @returns {Promise}
   **/
  $scope.withdraw = function () {
    return $scope.confirmDialog('Withdraw Claim', 'Are you certain you want to withdraw your claim?')
      .then(function () {
        return $scope.ajax({
          url: '/Ext/Claim/Withdraw',
          method: 'PUT',
          data: $scope.model
        })
          .then(function (response) {
            $scope.confirm();
            window.location = response.data.RedirectUrl || '/Ext/Claim/Report/View/' + $scope.section.grantApplicationId;
          });
      })
      .catch(angular.noop);
  }
});


