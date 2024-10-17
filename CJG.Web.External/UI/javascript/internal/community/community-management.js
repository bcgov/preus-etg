// Community Management Popup Form
app.controller('CommunityManagement', function ($scope, $controller) {
  angular.extend(this, $controller('Base', { $scope: $scope }));

  Object.assign({
    name: 'CommunityManagement',
    displayName: 'Community Management'
  }, $scope.section)

  $scope.activeOptions = [
    { Key: true, Value: "True" },
    { Key: false, Value: "False" }
  ];

  /**
  * Ajax call to Add or Update Community.
  * @function saveCommunity
  * @returns {Promise}
  * */
  $scope.saveCommunity = function () {
    return $scope.load({
      url: '/Int/Admin/Community/',
      set: 'ngDialogData.community',
      method: $scope.ngDialogData.community.Id === 0 ? 'POST' : 'PUT',
      data: $scope.ngDialogData.community,
      condition: $scope.ngDialogData.community
    })
      .then(function (response) {
        $scope.confirm(response.data);
      })
      .catch(angular.noop);
  }

  /**
  * Ajax call to get community 'id'.
  * @function loadCommunity
   * @param {int} id
  * @returns {Promise}
  * */
  $scope.loadCommunity = function (id) {
    return $scope.load({
      url: '/Int/Admin/Community/' + id,
      set: 'ngDialogData.community'
    });
  }

  /**
   * Initialize the data for the form.
   * @function init
   * @returns {void}
   **/
  $scope.init = function () {
    if (!$scope.ngDialogData.community.Id) {
      $scope.ngDialogData.community.Active = true;
    } else {
      return $scope.loadCommunity($scope.ngDialogData.community.Id)
        .catch(angular.noop);
    }
  }

  $scope.init();
});
