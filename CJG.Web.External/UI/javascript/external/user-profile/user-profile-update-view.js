app.controller('EditUserProfile', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    postalCodeNotInBC: null
  }

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadUserProfile() {
    return $scope.load({
      url: '/Ext/User/Profile/' + angular.element("#currentUserId").val(),
      set: 'model'
    });
  }

  function init() {
    return Promise.all([
        loadUserProfile()
      ])
      .then(function () {
        return $timeout(function () {
          $scope.checkPostalCode();
        });
      }).catch(angular.noop);
  }

  $scope.checkPostalCode = function () {
    let postalCode = $scope.model.UserProfileDetails.PhysicalPostalCode.toUpperCase().trim();
    if (postalCode === undefined || postalCode === '') {
      $scope.section.postalCodeNotInBC = false;
      return;
    }

    $scope.section.postalCodeNotInBC = postalCode.charAt(0) !== "V";
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
