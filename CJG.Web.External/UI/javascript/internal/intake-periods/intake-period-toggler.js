app.controller('IntakePeriodToggle', function ($scope, $controller) {
  angular.extend(this, $controller('Base', { $scope: $scope }));

  Object.assign({
      name: 'IntakePeriodToggle',
      displayName: 'Intake Period Management'
    },
    $scope.section);

  $scope.saveIntakePeriodStatus = function () {
    return $scope.load({
      url: '/Int/Admin/IntakePeriods/IntakePeriod/ToggleStatus/' + $scope.ngDialogData.intakePeriod.Id,
      method: 'PUT',
      set: 'ngDialogData.intakePeriod'
    })
      .then(function (response) {
        $scope.confirm(response.data);
      })
      .catch(angular.noop);
  }

  $scope.loadIntakePeriodStatus = function (intakePeriod) {
    return $scope.load({
      url: '/Int/Admin/IntakePeriods/IntakePeriod/' + intakePeriod.FiscalId + '/' + intakePeriod.GrantProgramId + '/' + intakePeriod.GrantSteamId + '/' + intakePeriod.Id,
      set: 'ngDialogData.intakePeriod'
    });
  }

  $scope.init = function () {
    return $scope.loadIntakePeriodStatus($scope.ngDialogData.intakePeriod)
      .catch(angular.noop);
  }

  $scope.init();
});
