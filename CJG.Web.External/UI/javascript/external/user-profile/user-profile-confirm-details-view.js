app.controller('ConfirmDetails', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadConfirmDetails() {
    return $scope.load({
      url: '/Ext/User/Profile/Confirm',
      set: 'model'
    });
  }

  function init() {
    return loadConfirmDetails()
      .catch(angular.noop);
  }

  $scope.confirm = function () {
    window.location = "/Ext/User/Profile/Create/View";
  }

  init();
});
