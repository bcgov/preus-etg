app.controller('IntakePeriodView', function ($scope, $controller, ngDialog) {
  angular.extend(this, $controller('Base', { $scope: $scope }));

  Object.assign({
      name: 'IntakePeriodView',
      displayName: 'Intake Period Management'
    },
    $scope.section);

  $scope.saveIntakePeriod = function () {
    return $scope.load({
      url: '/Int/Admin/IntakePeriods/IntakePeriod/Save',
      set: 'ngDialogData.intakePeriod',
      method: $scope.ngDialogData.intakePeriod.Id === 0 ? 'POST' : 'PUT',
      data: $scope.ngDialogData.intakePeriod,
      condition: $scope.ngDialogData.intakePeriod
    })
      .then(function (response) {
        $scope.confirm(response.data);
      })
      .catch(angular.noop);
  }

  $scope.loadIntakePeriod = function (intakePeriod) {
    return $scope.load({
      url: '/Int/Admin/IntakePeriods/IntakePeriod/' + intakePeriod.FiscalId + '/' + intakePeriod.GrantProgramId + '/' + intakePeriod.GrantSteamId + '/' + intakePeriod.Id,
      set: 'ngDialogData.intakePeriod'
    });
  }

  $scope.init = function () {
    return $scope.loadIntakePeriod($scope.ngDialogData.intakePeriod)
      .catch(angular.noop);
  }

  $scope.init();
});
