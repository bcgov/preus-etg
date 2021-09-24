app.controller('ProgramCosts', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'ProgramCosts',
    displayName: 'Program Costs',
    editingAgreedCost: 0.0,
    save: {
      url: '/Int/Application/Training/Cost',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      // TODO: Fix
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.grantFile.RowVersion;
    },
    onSave: function () {
      $scope.resyncApplicationDetails();
      $scope.emit('refresh', { target: ['ApplicationNotes', 'ApplicationSummary', 'ProgramDescription', /SkillsTrainingProgram\-.*/], force: true });
    },
    onRefresh: function () {
      return loadProgramCost().catch(angular.noop);
    },
    showEditEligibleCost: false
  };
  if (typeof ($scope.eligibleExpenseTypes) === 'undefined') $scope.eligibleExpenseTypes = [];

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make an AJAX request for the eligible expense types
   * @function loadEligibleExpenseTypes
   * @returns {void}
   **/
  function loadEligibleExpenseTypes() {
    return $scope.load({
      url: '/Int/Application/Training/Cost/Eligible/Expense/Types/' + $scope.grantFile.GrantStreamId,
      set: 'eligibleExpenseTypes',
      condition: !$scope.eligibleExpenseTypes || !$scope.eligibleExpenseTypes.length
    });
  }

  /**
   * Make an AJAX request for the program cost data.
   * @function loadProgramCost
   * @returns {void}
   **/
  function loadProgramCost() {
    return $scope.load({
      url: '/Int/Application/Training/Cost/' + $scope.parent.grantApplicationId,
      set: 'model'
    });
  }

  /**
   * Initialize the data for the form.
   * @function init
   * @returns {void}
   **/
  $scope.init = function () {
    return Promise.all([
      loadEligibleExpenseTypes(),
      loadProgramCost()
    ]).catch(angular.noop);
  }

  /**
   * Update all eligible costs with expense type "NotParticipantLimited"
   * @function syncNumberOfParticipants
   * @returns {void}
   **/
  $scope.syncNumberOfParticipants = function () {
    $scope.model.TrainingCost.EligibleCosts.forEach(function (cost) {
      if (cost.EligibleExpenseType.ExpenseTypeId === 3) { // TODO: Fix
        cost.AgreedMaxParticipants = $scope.model.TrainingCost.AgreedParticipants;
      }
    });
    recalculate();
  }

  /**
   * Recalculate the totals on the form.
   * @function recalculateTotals
   * @returns {void}
   **/
  function recalculateTotals() {
    $scope.model.TrainingCost.TotalAgreedCost = $scope.model.TrainingCost.EligibleCosts.map(function (ec) { return parseFloat(ec.AgreedCost); }).reduce(function (a, b) { return a + b; }, 0);
    $scope.model.TrainingCost.TotalAgreedEmployer = $scope.model.TrainingCost.EligibleCosts.map(function (ec) { return parseFloat(ec.AgreedEmployerContribution); }).reduce(function (a, b) { return a + b; }, 0);
    $scope.model.TrainingCost.TotalAgreedReimbursement = $scope.model.TrainingCost.EligibleCosts.map(function (ec) { return parseFloat(ec.AgreedMaxReimbursement); }).reduce(function (a, b) { return a + b; }, 0);
    if ($scope.model.TrainingCost.ProgramType == 1) {
          let ratedAmount = Math.round($scope.model.TrainingCost.TotalAgreedCost * $scope.model.TrainingCost.ReimbursementRate * 100) / 100;
          ratedAmount = ratedAmount >= $scope.model.TrainingCost.MaxReimbursementAmt * $scope.model.TrainingCost.AgreedParticipants ? $scope.model.TrainingCost.MaxReimbursementAmt * $scope.model.TrainingCost.AgreedParticipants : ratedAmount;
          $scope.model.TrainingCost.TotalAgreedEmployer = $scope.model.TrainingCost.TotalAgreedCost - ratedAmount;
          $scope.model.TrainingCost.TotalAgreedReimbursement = ratedAmount;
      }
    }

  /**
   * Recalculate the average costs.
   * @function recalculateAverage
   * @returns {void}
   **/
  function recalculateAverages() {
    $scope.model.TrainingCost.ESSAgreedAverage = 0;
    $scope.model.TrainingCost.EligibleCosts.forEach(function (cost) {
      if (cost.ServiceType === 2) { // TODO: Fix
        $scope.model.TrainingCost.ESSAgreedAverage += parseFloat(cost.AgreedMaxParticipantCost);
      }
    });
  }

  /**
   * Recalculate the whole form and all costs.
   * @function recalculate
   * @returns {void}
   **/
  function recalculate() {
    $scope.model.TrainingCost.EligibleCosts.forEach(function (ec) {
      calculateEligibleCost(ec);
    });
    recalculateAverages();
    recalculateTotals();
  }
  $scope.recalculate = recalculate;

  /**
   * Calculate the per-participant cost for the expense.
   * @function calculateParticipantCost
   * @param {any} participants - The number of participants.
   * @param {any} cost - The cost of the expense.
   * @returns {float} The per-participant cost.
   */
  function calculateParticipantCost(participants, cost) {
    if (!participants || participants < 1) return 0;
    return Math.floor(parseFloat(cost) / parseInt(participants) * 100) / 100;
  }

  /**
   * Calculate the reimbursement amount.
   * @function calculateReimbursement
   * @param {any} cost - The cost of the expense.
   * @param {any} rate - The rate for the expense type.
   * @returns {float} - The reimbursement amount.
   */
  function calculateReimbursement(cost, rate) {
    return Math.round(parseFloat(cost) * parseFloat(rate) * 100) / 100;
  }

  /**
   * Calculate the eligible cost that are currently being added/updated.
   * @function calculateEligibleCost
   * @param {any} eligibleCost
   * @returns {void}
   */
  function calculateEligibleCost(eligibleCost) {
    // If there are no participants the eligible cost will be $0.
    eligibleCost.AgreedMaxParticipants = parseInt(eligibleCost.AgreedMaxParticipants);
    if (eligibleCost.AgreedMaxParticipants <= 0) {
      eligibleCost.AgreedMaxParticipantCost = 0;
      eligibleCost.AgreedMaxReimbursement = 0;
      eligibleCost.AgreedEmployerContribution = 0;
      eligibleCost.AgreedCost = 0;
      if (eligibleCost.Breakdowns)
        eligibleCost.Breakdowns.forEach(function (breakdown) {
          breakdown.AssessedCost = 0;
        });
    } else {
      // Skills Training eligible costs are the sum of the breakdowns.
      if (eligibleCost.EligibleExpenseType.ServiceType === 1) {
        eligibleCost.AgreedCost = eligibleCost.Breakdowns.map(function (b) { return parseFloat(b.AssessedCost); }).reduce(function (a, b) { return a + b; });
      }
      eligibleCost.AgreedMaxParticipantCost = calculateParticipantCost(eligibleCost.AgreedMaxParticipants, eligibleCost.AgreedCost);
      eligibleCost.AgreedMaxReimbursement = calculateReimbursement(eligibleCost.AgreedCost, $scope.model.TrainingCost.ReimbursementRate);
      eligibleCost.AgreedEmployerContribution = parseFloat(eligibleCost.AgreedCost) - eligibleCost.AgreedMaxReimbursement;

      if ($scope.model.TrainingCost.ProgramType == 1) {
          let rate = eligibleCost.EligibleExpenseType.Rate == null ? $scope.model.TrainingCost.ReimbursementRate : eligibleCost.EligibleExpenseType.Rate;
          let reimbursementAmount = Math.round(eligibleCost.AgreedCost * rate * 100) / 100;
          reimbursementAmount = reimbursementAmount >= $scope.model.TrainingCost.MaxReimbursementAmt ? $scope.model.TrainingCost.MaxReimbursementAmt : reimbursementAmount;
          eligibleCost.AgreedMaxReimbursement = reimbursementAmount;
          eligibleCost.AgreedEmployerContribution = parseFloat(eligibleCost.AgreedCost) - eligibleCost.AgreedMaxReimbursement;
      }
    }
  }

  /**
   * Calculate the eligible cost that are currently being added/updated.
   * Also recalculate the totals on the form.
   * @function calculate
   * @returns {void}
   **/
  function calculate() {
    if ($scope.section.eligibleCost.AgreedMaxParticipants > $scope.model.TrainingCost.AgreedParticipants) {
      Utils.initValue($scope, 'errors.eligibleCost.AgreedMaxParticipants', 'The number of participants is greater than the agreed maximum.');
    } else {
      Utils.initValue($scope, 'errors.eligibleCost.AgreedMaxParticipants', null);
    }
    calculateEligibleCost($scope.section.eligibleCost);
    recalculateAverages();
    recalculateTotals();

    if ($scope.model.TrainingCost.ProgramType === 1) {
      $scope.model.TrainingCost.TotalAgreedCost += parseFloat($scope.section.eligibleCost.AgreedCost - $scope.section.editingAgreedCost);

      let ratedAmount = Math.round($scope.model.TrainingCost.TotalAgreedCost * $scope.model.TrainingCost.ReimbursementRate * 100) / 100;
      ratedAmount = ratedAmount >= $scope.model.TrainingCost.MaxReimbursementAmt * $scope.model.TrainingCost.AgreedParticipants ? $scope.model.TrainingCost.MaxReimbursementAmt * $scope.model.TrainingCost.AgreedParticipants : ratedAmount;

      $scope.model.TrainingCost.TotalAgreedEmployer = $scope.model.TrainingCost.TotalAgreedCost - ratedAmount;
      $scope.model.TrainingCost.TotalAgreedReimbursement = ratedAmount;
    } else {
      $scope.model.TrainingCost.TotalAgreedCost += parseFloat($scope.section.eligibleCost.AgreedCost);
      $scope.model.TrainingCost.TotalAgreedEmployer += parseFloat($scope.section.eligibleCost.AgreedEmployerContribution);
      $scope.model.TrainingCost.TotalAgreedReimbursement += parseFloat($scope.section.eligibleCost.AgreedMaxReimbursement);
    }
  }

  $scope.calculate = calculate;

  /**
   * Show/Hide the requested rows.
   * @function toggleRequestRows
   * @returns {void}
   **/
  $scope.toggleRequestRows = function () {
    $scope.section.showRequestRows = !$scope.section.showRequestRows;
  }

  /**
   * Show the edit form and populate it with the specified eligible cost.
   * @function editEligibleCost
   * @param {any} index - The index position of the eligible cost.
   * @returns {void}
   */
  $scope.editEligibleCost = function (index) {
    $scope.section.eligibleCost = Object.assign({ index: index }, $scope.model.TrainingCost.EligibleCosts[index]);
    $scope.section.showEditEligibleCost = true;
    $scope.section.addNewEligibleCost = false;
    $scope.section.editingAgreedCost = $scope.section.eligibleCost.AgreedCost;
  }

  /**
   * Delete the eligible cost from the array.
   * @function deleteEligibleCost
   * @param {any} index - The index position of the eligible cost.
   * @returns {void}
   */
  $scope.deleteEligibleCost = function (index) {
    var caption = $scope.model.TrainingCost.EligibleCosts[index].EligibleExpenseType.Caption;
    $scope.confirmDialog('Delete', 'Do you want to remove this eligible cost "' + caption + '".')
      .then(function () {
        $scope.model.TrainingCost.EligibleCosts.splice(index, 1);
        recalculate();
      })
      .catch(angular.noop);
  }

  /**
   * Show the edit eligible cost form and populate it with a new cost.
   * @function createEligibleCost
   * @returns {void}
   **/
  $scope.createEligibleCost = function () {
    $scope.section.eligibleCost = {
      Id: 0,
      AddedByAssessor: true,
      EstimatedMaxParticipants: 0,
      EstimatedCost: 0,
      EstimatedMaxParticipantCost: 0,
      EstimatedEmployerContribution: 0,
      EstimatedMaxReimbursement: 0,
      AgreedMaxParticipants: 0,
      AgreedCost: 0,
      AgreedMaxParticipantCost: 0,
      AgreedEmployerContribution: 0,
      AgreedMaxReimbursement: 0
    };
    $scope.section.showEditEligibleCost = true;
    $scope.section.addNewEligibleCost = true;
  }

  /**
   * Cancel the add or edit of the eligible cost and hide the section.
   * @function cancelEligibleCost
   * @returns {void}
   **/
  $scope.cancelEligibleCost = function () {
    recalculate();
    $scope.section.showEditEligibleCost = false;
    $scope.section.addNewEligibleCost = false;
  }

  /**
   * Update the form with the new/updated eligible cost.
   * @function updateEligibleCost
   * @returns {void}
   **/
  $scope.updateEligibleCost = function () {
    if ($scope.section.addNewEligibleCost) {
      $scope.model.TrainingCost.EligibleCosts.push($scope.section.eligibleCost);
    } else {
      $scope.model.TrainingCost.EligibleCosts[$scope.section.eligibleCost.index] = $scope.section.eligibleCost;
    }
    recalculate();
    $scope.section.showEditEligibleCost = false;
    $scope.section.addNewEligibleCost = false;
  }
});
