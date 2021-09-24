app.controller('GrantOpeningsView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'GrantOpeningsView',
    grantProgramId: $attrs.ngGrantProgramId,
    fiscalYearId: $attrs.ngFiscalYearId,
    appDateTime: $attrs.ngAppDateTime
  };

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  if ($scope.section.fiscalYearId) $scope.selectedFiscalYear = parseInt($scope.section.fiscalYearId);
  if ($scope.section.grantProgramid) $scope.selectedGrantProgram = parseInt($scope.section.grantProgramid);

  function loadFiscalYears() {
    return $scope.load({
      url: '/Int/Admin/Grant/Opening/Fiscal/Years',
      set: 'fiscalYears'
    })
      .then(function (response) {
        if ($scope.selectedFiscalYear == null) $scope.selectedFiscalYear = response.data[0].Id;
        loadGrantPrograms();
      })
      .catch(angular.noop);
  }

  function loadGrantPrograms() {
    return $scope.load({
      url: '/Int/Admin/Grant/Opening/Programs',
      set: 'grantPrograms'
    })
      .then(function (response) {
        if (($scope.selectedGrantProgram == null || $scope.selectedGrantProgram == 0) && $scope.grantPrograms.length > 1)
          $scope.selectedGrantProgram = response.data[1].Id;
        $scope.onGrantProgramChange();
      })
      .catch(angular.noop);
  }

  function loadGrantStreamInformation() {
    var url = '/Int/Admin/Grant/Opening/Fiscal';
    if ($scope.selectedFiscalYear != null) {
      url = url + "/" + $scope.selectedFiscalYear;
      if ($scope.selectedGrantProgram != null) {
        url = url + "/" + $scope.selectedGrantProgram;
      }
    }
    return $scope.load({
      url: url,
      set: 'model'
    })
      .then(function () {
        return $timeout(function () {
          $scope.showGrantStreamInformation = true;
        });
      })
      .catch(angular.noop);
  }

  $scope.onFiscalYearChange = function () {
    if ($scope.selectedFiscalYear != null &&
      $scope.selectedFiscalYear > 0) {
      loadGrantPrograms();
    }
  }

  $scope.onGrantProgramChange = function () {
    if ($scope.selectedGrantProgram != null && $scope.selectedGrantProgram > 0 && $scope.selectedFiscalYear != null && $scope.selectedFiscalYear > 0) {
      return loadGrantStreamInformation()
        .catch(angular.noop);
    }

    if ($scope.selectedGrantProgram == null || $scope.selectedGrantProgram == 0) {
      $scope.showGrantStreamInformation = false;
    }
  }

  $scope.loadModalView = function () {
    return ngDialog.openConfirm({
      template: '/Int/Admin/Grant/Opening/View',
      data: {
        title: 'Grant Opening',
        model: $scope.grantOpening,
        appDateTime: $scope.section.appDateTime
      }
    })
      .then(function () {
        return loadGrantStreamInformation();
      })
      .catch(function () {
        return loadGrantStreamInformation();
      });
  }

  $scope.initCreateOpeningGrantModal = function (trainingPeriodId, grantStreamId) {
    return $scope.load({
      url: '/Int/Admin/Grant/Opening/' + trainingPeriodId + "/" + grantStreamId,
      set: 'grantOpening'
    })
      .then(function () {
        return $scope.loadModalView();
      })
      .catch(angular.noop);
  }

  $scope.initSelectOpeningGrantModal = function (grantOpeningId) {
    return $scope.load({
      url: '/Int/Admin/Grant/Opening/' + grantOpeningId,
      set: 'grantOpening'
    })
      .then(function () {
        return $scope.loadModalView();
      })
      .catch(angular.noop);
  }

  $scope.getOpeningStateLabel = function (state) {
    if (state == 0) {
      return 'Unscheduled';
    }
    else if (state == 1) {
      return 'Scheduled';
    }
    else if (state == 2) {
      return 'Published';
    }
    else if (state == 3) {
      return 'Open';
    }
    else if (state == 4) {
      return 'Closed';
    }
    else if (state == 5) {
      return 'Open for Submit';
    }
    else {
      return 'Not Started';
    }
  }

  $scope.getOpeningStateCss = function (state) {
    if (state == 0 || state == 5) {
      return 'label--unpublished';
    }
    else if (state == 1) {
      return 'label--scheduled';
    }
    else if (state == 2) {
      return 'label--published';
    }
    else if (state == 3) {
      return 'label--open';
    }
    else if (state == 4) {
      return 'label--closed';
    }
    else {
      return 'label--notstarted';
    }
  }

  loadFiscalYears()
    .catch(angular.noop);
});




