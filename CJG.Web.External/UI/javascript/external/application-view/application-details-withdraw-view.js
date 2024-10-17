app.controller('ApplicationDetailsWithdrawView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.model = {
    Id: $attrs.ngGrantApplicationId,
    RowVersion: $attrs.ngRowVersion,
    WithdrawReason: null
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  $scope.withdraw = function () {
    return $scope.confirmDialog('Withdraw Application', 'If you withdraw your grant application then it will be closed and will not be given further consideration by the Ministry. Are you sure you want to withdraw your application?')
      .then(function () {
        return $scope.ajax({
          url: '/Ext/Application/Withdraw',
          method: 'PUT',
          data: function () {
            return $scope.model;
          }
        })
          .then(function (response) {
            $scope.confirm();
            window.location = '/Ext/Home/Index';
          });
      })
      .catch(angular.noop);
  }
});


