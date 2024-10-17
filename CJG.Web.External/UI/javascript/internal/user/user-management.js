// User Management modal
app.controller('UserManagement', function ($scope, $controller) {
  angular.extend(this, $controller('Base', { $scope: $scope }));

  Object.assign({
    name: 'UserManagement',
    displayName: 'User Management'
  }, $scope.section)

  $scope.activeOptions = [
    { Key: true, Value: "True" },
    { Key: false, Value: "False" }
  ];

  /**
  * Ajax call to Add or Update user.
  * @function saveUser
  * @returns {Promise}
  * */
  $scope.saveUser = function () {
    return $scope.load({
      url: '/Int/Admin/User/',
      set: 'ngDialogData.user',
      method: $scope.ngDialogData.user.ApplicationUserId ? 'PUT' : 'POST',
      data: $scope.ngDialogData.user,
      condition: $scope.ngDialogData.user
    })
      .then(function (response) {
        $scope.confirm(response.data);
      })
      .catch(angular.noop);
  }

  /**
  * Load internal user for the table.
  * @function loadUser
  * @param {int} id
  * @returns {Promise}
  * */
  function loadUser(id) {
    return $scope.load({
      url: '/Int/Admin/User/' + id,
      set: 'ngDialogData.user'
    })
      .then(function () {
        console.debug("Success");
      })
      .catch(angular.noop);
  }

  /**
   * Initialize the data for the form.
   * @function init
   * @returns {void}
   **/
  $scope.init = function() {
    $scope.roles = $scope.ngDialogData.user.Roles;

    if ($scope.ngDialogData.user.Id) {
      return loadUser($scope.ngDialogData.user.Id)
        .catch(angular.noop);
    }
  }

  $scope.init();
});
