app.controller('SyncBCeIDAccountsController', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'SyncBCeIDAccounts'
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
   * Make AJAX request to sync BCeID accounts data.
   * @function sync
   * @returns {Promise}
   **/
  $scope.sync = function () {
    return $scope.ajax({
      url: '/Int/Debug/GetAllBCeIDAccounts',
      method: 'GET'
    })
      .then(function (response) {
        $scope.executeQueue(response.data);
      })
      .catch(angular.noop);
  };

  $scope.executeQueue = function (user) {
    return $scope.ajax({
      url: '/Int/Debug/UpdateAllBCeIDAccounts',
      method: 'POST',
      data: user
    })
      .then(function (response) {
        var data = response.data;
        if (data.IsCompleted) {
          var message = "There are " + data.NumberOfUpdatedAccounts + " Out of " + data.NumberOfAccounts + " BCeID Account Upated."
          if (data.Failed.length > 0) {
            message = message + "<br/>" + "Failed BCeID as following:";
            for (var i = 0; i < data.Failed.length; i++) {
              message = message + "<br/>" +
                " BCeID : " + data.Failed[i].BCeID +
                ", First Name : " + data.Failed[i].FirstName +
                ", Last Name : " + data.Failed[i].LastName +
                ", Reason : " + data.Failed[i].Reason;
            }
          }

          if (data.Failed.length > 0 && data.Failed.length === data.NumberOfAccounts) {
            $scope.setAlert({ response: { status: 400 }, message: message });
          }
          else {
            $scope.setAlert({ response: { status: 200 }, message: message });
          }
        }
        else {
          data.Skip = data.Skip + 1;
          $scope.executeQueue(data);
        }
      })
      .catch(angular.noop);
  };
});
