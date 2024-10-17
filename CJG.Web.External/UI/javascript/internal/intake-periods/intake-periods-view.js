app.controller('IntakePeriodsView', function ($scope, $attrs, $controller, $timeout, ngDialog) {
  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  $scope.section = {
    name: 'IntakePeriodsView',
    grantProgramId: $attrs.ngGrantProgramId,
    grantStreamId: $attrs.ngGrantStreamId,
    fiscalYearId: $attrs.ngFiscalYearId,
    appDateTime: $attrs.ngAppDateTime
  };

  if ($scope.section.fiscalYearId)
    $scope.selectedFiscalYear = parseInt($scope.section.fiscalYearId);

  if ($scope.section.grantProgramId)
    $scope.selectedGrantProgram = parseInt($scope.section.grantProgramId);

  if ($scope.section.grantStreamId)
    $scope.selectedGrantStream = parseInt($scope.section.grantStreamId);

  function loadFiscalYears() {
    return $scope.load({
      url: '/Int/Admin/IntakePeriods/Fiscal/Years',
      set: 'fiscalYears'
    })
      .then(function (response) {
        if ($scope.selectedFiscalYear == null)
          $scope.selectedFiscalYear = response.data[0].Id;
        loadGrantPrograms();
      })
      .catch(angular.noop);
  }

  function loadGrantPrograms() {
    return $scope.load({
      url: '/Int/Admin/IntakePeriods/Programs',
      set: 'grantPrograms'
    })
      .then(function () {
        if (($scope.selectedGrantProgram != null || $scope.selectedGrantProgram > 0)) {
          loadGrantStreams();
        }
        $scope.onGrantProgramChange();
      })
      .catch(angular.noop);
  }

  function loadGrantStreams() {
    return $scope.load({
      url: '/Int/Admin/IntakePeriods/Streams/' + $scope.selectedGrantProgram,
      set: 'grantStreams'
    })
      .then(function () {
        $scope.selectedGrantStream = null;
      })
      .catch(angular.noop);
  }

  function loadIntakePeriods() {
    return $scope.load({
      url: '/Int/Admin/IntakePeriods/Periods/' + $scope.selectedFiscalYear + '/' + $scope.selectedGrantProgram + '/' + $scope.selectedGrantStream,
      set: 'model'
    })
      .then(function () {
        return $timeout(function () {
          $scope.showIntakePeriods = true;
        });
      })
      .catch(angular.noop);
  }

  $scope.onFiscalYearChange = function () {
    $scope.showIntakePeriods = false;
    $scope.showGrantStreamSelection = false;
    if ($scope.selectedFiscalYear != null && $scope.selectedFiscalYear > 0) {
      loadGrantPrograms();
    }
  }

  $scope.onGrantProgramChange = function () {
    $scope.showIntakePeriods = false;
    if ($scope.selectedGrantProgram != null && $scope.selectedGrantProgram > 0
      && $scope.selectedFiscalYear != null && $scope.selectedFiscalYear > 0) {
      loadGrantStreams();
      $scope.showGrantStreamSelection = true;
    }

    if ($scope.selectedGrantProgram == null || $scope.selectedGrantProgram === 0) {
      $scope.showGrantStreamSelection = false;
    }
  }

  $scope.onGrantStreamChange = function() {
    if ($scope.selectedGrantStream != null && $scope.selectedGrantStream > 0
      && $scope.selectedGrantProgram != null && $scope.selectedGrantProgram > 0
      && $scope.selectedFiscalYear != null && $scope.selectedFiscalYear > 0) {

      return loadIntakePeriods()
        .catch(angular.noop);
    }
    $scope.showIntakePeriods = false;
    return angular.noop;
  }

  $scope.refresh = function () {
    if ($scope.selectedGrantStream != null && $scope.selectedGrantStream > 0
      && $scope.selectedGrantProgram != null && $scope.selectedGrantProgram > 0
      && $scope.selectedFiscalYear != null && $scope.selectedFiscalYear > 0) {

      return loadIntakePeriods()
        .catch(angular.noop);
    }
    $scope.showIntakePeriods = false;
    return angular.noop;
  }

  $scope.openIntakeModal = function (id) {
    return showIntakeDialog({
      Id: id,
      FiscalId: $scope.selectedFiscalYear,
      GrantProgramId: $scope.selectedGrantProgram,
      GrantSteamId: $scope.selectedGrantStream
      })
      .catch(angular.noop);
  }

  function showIntakeDialog(intakePeriod) {
    if (!intakePeriod.Id)
      intakePeriod.Id = 0;

    return ngDialog.openConfirm({
      template: '/Int/Admin/IntakePeriods/IntakePeriod/View/' + intakePeriod.FiscalId + '/' + intakePeriod.GrantProgramId + '/' + intakePeriod.GrantSteamId + '/' + intakePeriod.Id,
      data: {
        intakePeriod: intakePeriod
      }
    }).then(function (updatedIntakePeriod) {
      if (updatedIntakePeriod) {
        if ($scope.updateIntakeModel(updatedIntakePeriod)) {
        } else {
          $scope.model.TrainingPeriods.push(updatedIntakePeriod);
        }
      }
    });
  }

  $scope.openIntakeToggleModal = function (id) {
    return showIntakeToggleDialog({
      Id: id,
      FiscalId: $scope.selectedFiscalYear,
      GrantProgramId: $scope.selectedGrantProgram,
      GrantSteamId: $scope.selectedGrantStream
      })
      .catch(angular.noop);
  }

  function showIntakeToggleDialog(intakePeriod) {
    if (!intakePeriod.Id)
      intakePeriod.Id = 0;

    return ngDialog.openConfirm({
      template: '/Int/Admin/IntakePeriods/IntakePeriod/CheckInflight/' + intakePeriod.Id,
      cache: false,
      data: {
        intakePeriod: intakePeriod
      }
    }).then(function (updatedIntakePeriod) {
      if (updatedIntakePeriod) {
        $scope.updateIntakeModel(updatedIntakePeriod);
      }
    });
  }

  $scope.updateIntakeModel = function (intakePeriod) {
    for (var i = 0; i < $scope.model.TrainingPeriods.length; i++) {
      if ($scope.model.TrainingPeriods[i].Id === intakePeriod.Id) {
        return $scope.model.TrainingPeriods[i] = intakePeriod;
      }
    }
  }

  loadFiscalYears()
    .catch(angular.noop);
});
