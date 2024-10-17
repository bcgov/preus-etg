app.controller('PrioritizationManagement', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'PrioritizationManagement',
    save: {
      url: '/Int/Admin/Prioritization/Thresholds',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    onSave: function () {
      window.location = '/Int/Admin/Prioritization/Thresholds/View';
    }
  };

  angular.extend(this, $controller('ParentSection', { $scope, $attrs }));

  function loadThresholds() {
    return $scope.load({
      url: '/Int/Admin/Prioritization/Thresholds',
      set: 'model'
    });
  }

  function init() {
    return Promise.all([
        loadThresholds()
      ])
      .catch(angular.noop);
  }

  $scope.recalculatePrioritization = function () {
    return $scope.confirmDialog('Recalculate Intake Queue Prioritization', 'Are you sure you wish to recalculate the Intake Queue Prioritization scores?')
      .then(function () {
        return $scope.ajax({
          url: '/Int/Admin/Prioritization/Recalculate',
          method: 'PUT'
        });
      })
      .then(function (response) {
        if (response.data.RedirectUrl)
          window.location = response.data.RedirectUrl;
      })
      .catch(angular.noop);
  }

  //$scope.$on('ngDialog.closing', function () {
  //  $scope.broadcast('refreshPager');
  //});

  init();
});
