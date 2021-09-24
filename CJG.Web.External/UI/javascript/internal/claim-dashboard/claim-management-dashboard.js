app.controller('ClaimManagementDashboard', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ClaimManagementDashboard',
    save: {
      url: '/Int/Admin/Claim',
      method: 'POST',
      data: function () {
        return $scope.model;
      }
    },
    onRefresh: function () {
      return loadClaimManagementDashboard().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
 * Make AJAX request to save overpayments
 * @function saveOverpayments
 * @returns {Promise}
 **/
  $scope.saveOverpayments = function () {
    return $scope.ajax({
      url: '/Int/Admin/Claim/Overpayments',
      method: 'POST',
      data: $scope.model
    }).then(function (response) {
        return $timeout(function () {
          $scope.model = response.data;
        });
    }).catch(angular.noop);
  }


  /**
   * Make AJAX request for fiscal years data
   * @function loadFiscalYears
   * @returns {Promise}
   **/
  function loadFiscalYears() {
    return $scope.load({
      url: '/Int/Admin/Claim/Fiscal/Years/',
      set: 'fiscalYears',
      condition: !$scope.fiscalYears || !$scope.fiscalYears.length
    });
  }

  /**
   * Make AJAX request for grant programs data
   * @function loadGrantPrograms
   * @returns {Promise}
   **/
  $scope.loadGrantPrograms = function () {
    $scope.model.SelectedGrantProgramId = null;
    $scope.model.SelectedGrantStreamId = null;
    $scope.model.DataColumns = [];
    return $scope.load({
      url: '/Int/Admin/Claim/Grant/Programs/' + ($scope.model.SelectedFiscalYearId || ''),
      set: 'grantPrograms'
    }).then(function () {
      return $scope.loadGrantStreams();
    })
      .catch(angular.noop);
  };

  /**
   * Make AJAX request for grant streams data
   * @function loadGrantStreams
   * @returns {Promise}
   **/
  $scope.loadGrantStreams = function () {
    $scope.model.SelectedGrantStreamId = null;
    $scope.model.DataColumns = [];
    return $scope.load({
      url: '/Int/Admin/Claim/Grant/Streams/' + ($scope.model.SelectedFiscalYearId || '') + '/' + ($scope.model.SelectedGrantProgramId || ''),
      set: 'grantStreams'
    });
  };

  /**
   * Make AJAX request to load claim management dashboard data.
   * @function loadClaimManagementDashboard
   * @returns {Promise}
   **/
  function loadClaimManagementDashboard() {
    return $scope.load({
      url: '/Int/Admin/Claim/',
      set: 'model'
    });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadFiscalYears(),
      loadClaimManagementDashboard()
    ])
      .then(function () {
        return $scope.loadGrantPrograms();
      })
      .catch(angular.noop);
  }

  /**
   * Make AJAX request to refresh data
   * @function refresh
   * @returns {Promise}
   **/
  $scope.refresh = function () {
    return $scope.ajax({
      url: '/Int/Admin/Claim/Refresh',
      method: 'POST',
      data: $scope.model
    })
      .then(function (response) {
        return $timeout(function () {
          $scope.model = response.data;
        });
      })
      .catch(angular.noop);
  };

  /**
   * Make AJAX request to calculate data
   * @function calculate
   * @returns {Promise}
   **/
  $scope.calculate = function () {
    return $scope.ajax({
      url: '/Int/Admin/Claim/Calculate',
      method: 'POST',
      data: $scope.model
    })
      .then(function (response) {
        return $timeout(function () {
          $scope.model = response.data;
        });
      })
      .catch(angular.noop);
  };

  /**
  * Do a 'soft' calculation of the data
  * @function calculateAfterKeypress
  * @returns {void}
  **/
  $scope.calculateAfterKeypress = function () {
    // non-fiscal calculation
    var trainingPeriodData = $scope.model.DataColumns.filter(item => !item.Name.includes("Fiscal"));
    trainingPeriodData.forEach(function (columnViewModel) {
      // claimed and unclaimed calculation
      columnViewModel.UnclaimedSlipageAmount = -1 * columnViewModel.UnclaimedCommitments.Amount * ($scope.model.UnclaimedSlippageRate / 100);
      columnViewModel.UnclaimedCancellationAmount = -1 * columnViewModel.UnclaimedCommitments.Amount * ($scope.model.UnclaimedCancellationRate / 100);
      columnViewModel.ClaimsSlippageAmount = -1 * columnViewModel.ClaimsReceivedAmount.Amount * ($scope.model.ClaimedSlippageRate / 100);

      // projection of performance to budget calculation
      columnViewModel.ProjectionOfPerformanceAmount = columnViewModel.UnclaimedCommitments.Amount +
        columnViewModel.UnclaimedSlipageAmount + columnViewModel.UnclaimedCancellationAmount +
        columnViewModel.ClaimsReceivedAmount.Amount + columnViewModel.ClaimsSlippageAmount +
        columnViewModel.PaymentRequests.Amount;

      // over/under budget calculation
      columnViewModel.OverUnderBudget.Amount = columnViewModel.ProjectionOfPerformanceAmount - columnViewModel.BudgetAllocationAmount;
      columnViewModel.OverUnderBudget.Percent = columnViewModel.BudgetAllocationAmount != 0 ? columnViewModel.OverUnderBudget.Amount / columnViewModel.BudgetAllocationAmount : 0;
    });

    // fiscal calculation
    var fiscalYearData = $scope.model.DataColumns.find(function (item) { return item.Name.includes("Fiscal"); });
    calculateFiscalYearColumnData(fiscalYearData, trainingPeriodData);
  };

  /**
   * Calculates the fiscal year column data
   * @function calculateFiscalYearColumnData
   * @param {object} target
   * @param {object} source
   * @returns {void}
   **/
  function calculateFiscalYearColumnData(target, source) {
    target.TotalAgreementsCount = 0;
    target.BudgetAllocationAmount = 0;
    target.UnclaimedCancellationAmount = 0;
    target.UnclaimedSlipageAmount = 0;
    target.ClaimsSlippageAmount = 0;
    target.ProjectionOfPerformanceAmount = 0;
    target.UnclaimedCommitments = { Percent: 0, Amount: 0, Count: 0 };
    target.ClaimsReceivedAmount = { Percent: 0, Amount: 0, Count: 0 }
    target.PaymentRequests = { Percent: 0, Amount: 0, Count: 0 }
    target.OverUnderBudget = { Percent: 0, Amount: 0, Count: 0 }

    source.forEach(function (sourceDataColumn) {        
      target.TotalAgreementsCount += sourceDataColumn.TotalAgreementsCount;
      incrementClaimStats(target.UnclaimedCommitments, sourceDataColumn.UnclaimedCommitments);
      target.BudgetAllocationAmount += sourceDataColumn.BudgetAllocationAmount;
      target.UnclaimedCancellationAmount += sourceDataColumn.UnclaimedCancellationAmount;
      target.UnclaimedSlipageAmount += sourceDataColumn.UnclaimedSlipageAmount;
      target.ClaimsSlippageAmount += sourceDataColumn.ClaimsSlippageAmount;
      target.ProjectionOfPerformanceAmount += sourceDataColumn.ProjectionOfPerformanceAmount;
      incrementClaimStats(target.ClaimsReceivedAmount, sourceDataColumn.ClaimsReceivedAmount);
      incrementClaimStats(target.PaymentRequests, sourceDataColumn.PaymentRequests);
      incrementClaimStats(target.OverUnderBudget, sourceDataColumn.OverUnderBudget);
    });

    if (target.TotalAgreementsCount > 0) {
      target.UnclaimedCommitments.Percent = target.UnclaimedCommitments.Count / target.TotalAgreementsCount;
      target.ClaimsReceivedAmount.Percent = target.ClaimsReceivedAmount.Count / target.TotalAgreementsCount;
      target.PaymentRequests.Percent = target.PaymentRequests.Count / target.TotalAgreementsCount;
    } else {
      target.UnclaimedCommitments.Percent = 0;
      target.ClaimsReceivedAmount.Percent = 0;
      target.PaymentRequests.Percent = 0;
    }

    target.OverUnderBudget.Percent = target.BudgetAllocationAmount != 0 ? target.OverUnderBudget.Amount / target.BudgetAllocationAmount : 0;
  }

  /**
  * Helper function that adds source values to the target
  * @function incrementClaimStats
  * @param {object} target
  * @param {object} source
  * @returns {void}
  **/
  function incrementClaimStats(target, source) {
    target.Amount += source.Amount;
    target.Count += source.Count;
  }

  init();
});
