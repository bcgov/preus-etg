app.controller('AutomationManagement', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'AutomationManagement',
    save: {
      url: '/Int/Admin/Automation/Settings',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    onSave: function () {
      window.location = '/Int/Admin/Automation/View';
    }
  };

  angular.extend(this, $controller('ParentSection', { $scope, $attrs }));

  function loadSettings() {
    return $scope.load({
      url: '/Int/Admin/Automation/Settings',
      set: 'model'
    });
  }

  function init() {
    return Promise.all([
        loadSettings()
      ])
      .catch(angular.noop);
  }

  init();
});
