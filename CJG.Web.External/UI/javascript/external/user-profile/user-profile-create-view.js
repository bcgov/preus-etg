app.controller('CreateUserProfile', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  $scope.displayProgramDetails = false;

  function loadUserProfile() {
    return $scope.load({
      url: '/Ext/User/Profile/0',
      set: 'model'
    })
      .then(function (response) {
        $scope.displayProgramDetails = ($scope.model.UserId === 0);
      })
      .catch(angular.noop);
  }

  function init() {
    return loadUserProfile()
      .catch(angular.noop);
  }

  $scope.confirmDetails = function () {
    window.location = '/Ext/User/Profile/Confirm/Details/View';
  }

  $scope.save = function () {
    $scope.model.UserProfileDetails.Phone = $scope.model.UserProfileDetails.PhoneAreaCode + $scope.model.UserProfileDetails.PhoneExchange + $scope.model.UserProfileDetails.PhoneNumber;

    return $scope.load({
      url: '/Ext/User/Profile',
      method: 'POST',
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
