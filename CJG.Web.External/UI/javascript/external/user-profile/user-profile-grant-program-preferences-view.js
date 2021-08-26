app.controller('ManageGrantProgramReferences', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadGrantProgramPreferences() {
    return $scope.load({
      url: '/Ext/User/Profile/Program/Preferences',
      set: 'model'
    });
  }

  $scope.save = function () {
    return $scope.load({
      url: '/Ext/User/Profile/Program/Preferences',
      method: 'PUT',
      data: $scope.model,
      set: 'model'
    })
      .then(function (response) {
        if (response.data.ReturnURL) window.location = response.data.ReturnURL;
      })
      .catch(angular.noop);
  }


  function init() {
    return loadGrantProgramPreferences()
      .catch(angular.noop);
  }

  init();

});
