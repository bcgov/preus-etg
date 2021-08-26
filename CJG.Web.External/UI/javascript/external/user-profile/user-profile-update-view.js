app.controller('EditUserProfile', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadUserProfile() {
    return $scope.load({
      url: '/Ext/User/Profile/' + angular.element("#currentUserId").val(),
      set: 'model'
    });
  }

  function init() {
    return loadUserProfile()
      .catch(angular.noop);
  }

  $scope.save = function () {
    $scope.model.UserProfileDetails.Phone = $scope.model.UserProfileDetails.PhoneAreaCode + $scope.model.UserProfileDetails.PhoneExchange + $scope.model.UserProfileDetails.PhoneNumber;

    return $scope.load({
      url: '/Ext/User/Profile',
      method: 'PUT',
      data: $scope.model,
      set: 'model'
    })
      .then(function (response) {
        window.location = '/Ext/Home';
      })
      .catch(angular.noop);
  }

  init();
});
