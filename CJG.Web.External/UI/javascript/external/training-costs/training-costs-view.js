var utils = require('../../shared/utils');
app.controller('TrainingCostsView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'TrainingCostsView',
    displayName: 'Training Costs',
    save: {
      url: '/Ext/Application/Training/Cost',
      method: 'PUT',
      data: function () {
        return $scope.model
      }
    },
    onSave: function () {
      window.location = '/Ext/Application/Overview/View/' + $scope.section.grantApplicationId;
    },
    grantApplicationId: $attrs.ngGrantApplicationId
  }

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  function loadDropdowns(url, target) {
    return $scope.load({
      url: '/Ext/Application/Training' + url,
      set: target
    });
  }

  function loadTraningCost() {
    return $scope.load({
      url: '/Ext/Application/Training/Cost/' + $scope.section.grantApplicationId,
      set: 'model'
    })
      .then(function (response) {
        calculateESSTotal();
      });
  }

  function init() {
    return Promise.all([
      loadDropdowns('/Eligible/Expense/Types/' + $scope.section.grantApplicationId, 'EligibleExpenseTypes'),
      loadTraningCost()
    ])
      .catch(angular.noop);
  }

  $scope.changeEligibleCostExpenseType = function () {
    $scope.recalculate();
  }

  $scope.recalculate = function () {
    $scope.model.SummaryMessage = null;
    if ($scope.EligibleCost.EligibleExpenseType) {

      if ($scope.EligibleCost.EligibleExpenseType.ExpenseTypeId == utils.ExpenseTypes.ParticipantAssigned) {
        if ($scope.EligibleCost.EstimatedParticipants == 0 || $scope.EligibleCost.EstimatedParticipants == null) {
          $scope.EligibleCost.EstimatedParticipantCost = 0;
          $scope.EligibleCost.EstimatedEmployerContribution = 0;
          $scope.EligibleCost.EstimatedReimbursement = 0;
        }
        else {
          if ($scope.EligibleCost.EstimatedCost == null) {
            $scope.EligibleCost.EstimatedCost = 0;
          }
          $scope.EligibleCost.EstimatedParticipantCost = calculateCost($scope.EligibleCost.EstimatedCost, $scope.EligibleCost.EstimatedParticipants);
        }
      }
      else if ($scope.EligibleCost.EligibleExpenseType.ExpenseTypeId > utils.ExpenseTypes.ParticipantAssigned) {
        $scope.EligibleCost.EstimatedParticipants = $scope.model.EstimatedParticipants;

        if ($scope.EligibleCost.EligibleExpenseType.ServiceType != 2) {
          if ($scope.EligibleCost.EstimatedCost == null) {
            $scope.EligibleCost.EstimatedCost = 0;
          }

          if ($scope.EligibleCost.Breakdowns.length > 0) {
            $scope.EligibleCost.EstimatedCost = 0;

            angular.forEach($scope.EligibleCost.Breakdowns, function (value, key) {

              if (value.EstimatedCost != null) {
                $scope.EligibleCost.EstimatedCost += parseFloat(value.EstimatedCost);
              }
            });
          }
        }

        $scope.EligibleCost.EstimatedParticipantCost = calculateCost($scope.EligibleCost.EstimatedCost, $scope.EligibleCost.EstimatedParticipants);
      }

      $scope.EligibleCost.EstimatedReimbursement = calculateReimbursement($scope.EligibleCost.EstimatedCost, $scope.EligibleCost.EstimatedParticipants);
      $scope.EligibleCost.EstimatedEmployerContribution = $scope.EligibleCost.EstimatedCost - $scope.EligibleCost.EstimatedReimbursement;
    }
    else {
      $scope.EligibleCost.EstimatedParticipants = 0;
      $scope.EligibleCost.EstimatedParticipantCost = 0;
      $scope.EligibleCost.EstimatedCost = 0;
      $scope.EligibleCost.EstimatedEmployerContribution = 0;
      $scope.EligibleCost.EstimatedReimbursement = 0;
    }
  }

  function calculateCost(cost, participant) {
    return Math.round(cost / participant * 100) / 100;
  }

  function calculateReimbursement(cost, partticipant) {
    let rate = $scope.EligibleCost.EligibleExpenseType.Rate == null ? $scope.model.ReimbursementRate : $scope.EligibleCost.EligibleExpenseType.Rate;

    if ($scope.model.ProgramType == utils.ProgramTypes.EmployerGrant) {
      let reimbursementAmount = Math.round(cost * rate * 100) / 100;
      reimbursementAmount = reimbursementAmount >= $scope.model.MaxReimbursementAmt ? $scope.model.MaxReimbursementAmt : reimbursementAmount;

      return reimbursementAmount * partticipant;
    }

     return Math.round(cost * rate * 100) / 100;
  }

  $scope.editEligibleCost = function (index) {
    $scope.EnableEdit = true;
    $scope.EnableEditBreakdown = false;
    $scope.BackupEligibleCose = angular.copy($scope.model.EligibleCosts[index]);
    $scope.EligibleCost = $scope.model.EligibleCosts[index];
    $scope.EligibleCostIndex = index;
    clearValidationErrors();
    calculateTotals();
    setTimeout(function () {
      angular.element('#EligibleCost').focus();
      angular.element("html, body").animate({ scrollTop: angular.element(document).height() }, "fast");
    });
  }

  $scope.deleteEligibleCost = function (index) {
    return $scope.confirmDialog('Delete Eligible Cost', 'Are you sure you want to delete this eligible cost?')
      .then(function () {
        $scope.model.EligibleCosts.splice(index, 1);
        calculateTotals();
      })
      .catch(angular.noop); x
  }

  $scope.cancelEligibleCost = function () {
    $scope.EnableEdit = false;
    if ($scope.EligibleCostIndex >= 0) {
      $scope.model.EligibleCosts[$scope.EligibleCostIndex] = $scope.BackupEligibleCose;
    }

    $scope.EligibleCostIndex = null;
    $scope.EnableEdit = false;
  }

  $scope.updateEligibleCost = function () {
    if (validateEligibleCost()) {
      if ($scope.EligibleCostIndex == null)
        $scope.model.EligibleCosts.push($scope.EligibleCost);
      else {
        $scope.model.EligibleCosts[$scope.EligibleCostIndex] = $scope.EligibleCost;
        $scope.EligibleCostIndex = null;
      }
      calculateTotals();
      $scope.EnableEdit = false;
    }
  }

  function calculateTotals() {
    var trainingCostTotal = 0;
    var employerTotal = 0;
    var reimbursementTotal = 0;
    $scope.model.EligibleCosts.forEach(function (cost) {
      trainingCostTotal += parseFloat(cost.EstimatedCost);
      employerTotal += parseFloat(cost.EstimatedEmployerContribution);
      reimbursementTotal += parseFloat(cost.EstimatedReimbursement);
    });

    var rate = $scope.EligibleCost.EligibleExpenseType.Rate == null ? $scope.model.ReimbursementRate : $scope.EligibleCost.EligibleExpenseType.Rate;
    var ratedAmount = Math.round(trainingCostTotal * rate * 100) / 100;
    ratedAmount = ratedAmount >= $scope.model.MaxReimbursementAmt * $scope.model.EstimatedParticipants ? $scope.model.MaxReimbursementAmt * $scope.model.EstimatedParticipants : ratedAmount;

    $scope.model.TotalEstimatedCost = trainingCostTotal;
    $scope.model.TotalEmployer = trainingCostTotal - ratedAmount;
    $scope.model.TotalRequest = ratedAmount;
    calculateESSTotal();
  }

  function calculateESSTotal() {
    $scope.ESSTotalAverage = 0;
    $scope.model.EligibleCosts.forEach(function (cost) {
      if (cost.EligibleExpenseType.ServiceType == utils.ServiceTypes.EmploymentServicesAndSupports) {
        $scope.ESSTotalAverage += parseFloat(cost.EstimatedParticipantCost);
      }
    });
  }

  $scope.createEligibleCost = function () {
    $scope.EligibleCost = {
      Id: 0,
      EstimatedParticipants: 0,
      EstimatedParticipantCost: 0,
      EstimatedCost: 0,
      EstimatedEmployerContribution: 0,
      EstimatedReimbursement: 0,
      ServiceType: null,
      ShowBreakdowns: false,
      AddedByAssessor: false,
      Breakdowns: []
    };
    $scope.EnableEdit = true;
    $scope.EligibleCostIndex = null;
    clearValidationErrors();
  }

  function validateEligibleCost() {
    clearValidationErrors();
    var validate = true;
    if ($scope.EligibleCost.EligibleExpenseType == null) {
      $scope.EligibleCostExpenseTypeIdError = true;
      $scope.EligibleCostSummaryMessage = "Eligible expense type is required.";
    } else {
      var matches = $.grep($scope.model.EligibleCosts, function (item) {
        return (item.EligibleExpenseType.Id === $scope.EligibleCost.EligibleExpenseType.Id && !item.EligibleExpenseType.AllowMultiple);
      });


      if (matches.length > 0 && $scope.EligibleCostIndex == null ||
        matches.length > 1 && $scope.EligibleCostIndex >= 0) {
        $scope.EligibleCostExpenseTypeIdError = true;
        $scope.EligibleCostSummaryMessage = "<p>Cannot add multiple expenses of type '" + $scope.EligibleCost.EligibleExpenseType.Caption + "'.</p>";
        validate = false;
      }
      if (!Number($scope.EligibleCost.EstimatedCost) > 0 && $scope.EligibleCost.EligibleExpenseType.ExpenseTypeId != utils.ExpenseTypes.NotParticipantLimited && $scope.EligibleCost.EligibleExpenseType.ExpenseTypeId != utils.ExpenseTypes.AutoLimitEstimatedCosts) {
        $scope.EligibleCostEstimatedCostError = true;
        $scope.EligibleCostSummaryMessage += "<p>The total expense cost of type '" + $scope.EligibleCost.EligibleExpenseType.Caption + "' must be greater than 0.</p>";
        validate = false;
      }
    }

    if (Number($scope.EligibleCost.EstimatedParticipants) <= 0) {
      $scope.EligibleCostEstimatedParticipantsError = true;
      $scope.EligibleCostSummaryMessage += "<p>The number of participants for expense type '" + $scope.EligibleCost.EligibleExpenseType.Caption + "' must be greater than 0</p>";
      validate = false;
    }

    if (Number($scope.EligibleCost.EstimatedParticipants) > Number($scope.model.EstimatedParticipants)) {
      $scope.EligibleCostEstimatedParticipantsError = true;
      $scope.EligibleCostSummaryMessage += "<p>The number of participants for expense type '" + $scope.EligibleCost.EligibleExpenseType.Caption + "' cannot exceed the number of participants you entered in part 1, which was '" + $scope.model.EstimatedParticipants + "'.</p>";
      validate = false;
    }

    return validate;
  }

  function clearValidationErrors() {
    $scope.EligibleCostExpenseTypeIdError = false;
    $scope.EligibleCostEstimatedParticipantsError = false;
    $scope.EligibleCostEstimatedCostError = false;
    $scope.EligibleCostSummaryMessage = "";
  }

  $scope.editEligibleCostBreakdown = function (index, array, parentIndex) {
    $scope.EnableEdit = false;
    $scope.EnableEditBreakdown = true;
    $scope.BackupEligibleCostBreakdown = angular.copy(array[index]);
    $scope.EligibleCost = $scope.model.EligibleCosts[parentIndex];
    $scope.EligibleCostBreakdown = array[index];
    $scope.EligibleCostBreakdownIndex = index;
    $scope.EligibleCostIndex = parentIndex;
    clearValidationErrors();
    calculateTotals();
    setTimeout(function () {
      angular.element('#EligibleBreakdownCost').focus();
      angular.element("html, body").animate({ scrollTop: angular.element(document).height() }, "fast");
    });
  }

  $scope.deleteEligibleCostBreakdown = function (index, array, parentIndex) {
    var eligibleCostBreakdown = array[index];
    $scope.EligibleCost = $scope.model.EligibleCosts[parentIndex];
    $scope.EligibleCostBreakdown = array[index];
    $scope.EligibleCostBreakdownIndex = index;
    $scope.EligibleCostIndex = parentIndex;

    return $scope.confirmDialog('Delete Eligible Cost Breakdown', 'Are you sure you want to delete the ' + eligibleCostBreakdown.EligibleExpenseBreakdown.Caption + ' component for ' + eligibleCostBreakdown.TrainingProgramTitle + '?')
      .then(function () {
        array.splice(index, 1);
        $scope.EligibleCost = $scope.model.EligibleCosts[parentIndex];
        $scope.recalculate();
        calculateTotals();
      })
      .catch(angular.noop);
  }

  $scope.cancelEligibleCostBreakdown = function () {
    $scope.model.EligibleCosts[$scope.EligibleCostIndex].Breakdowns[$scope.EligibleCostBreakdownIndex] = $scope.BackupEligibleCostBreakdown;
    $scope.EligibleCostBreakdownIndex = null;
    $scope.EligibleCostIndex = null;
    $scope.EnableEditBreakdown = false;
    $scope.EligibleCost = null;
    $scope.EligibleCostBreakdown = null;
  }

  $scope.updateEligibleCostBreakdown = function () {
    $scope.recalculate();
    calculateTotals();
    $scope.EnableEditBreakdown = false;
    $scope.EligibleCost = null;
    $scope.EligibleCostBreakdown = null;
  }

  /**
   * Recalculate the estimated participant costs for each eligible cost.
   * @function recalculateParticipantCosts
   * @returns {void}
   **/
  function recalculateParticipantCosts() {
    angular.forEach($scope.model.EligibleCosts, function (value, index) {
      if (value.EstimatedParticipants > 0) {
        value.EstimatedParticipantCost = value.EstimatedCost / value.EstimatedParticipants;
      }
    });
  }

  /**
   * Some eligible costs default to the overal estimated participants.
   * @function syncNumberOfParticipants
   * @returns {void}
   **/
  $scope.syncNumberOfParticipants = function () {
    angular.forEach($scope.model.EligibleCosts, function (value, index) {
      if (value.EligibleExpenseType.AutoInclude) { // TODO: This is supposed to be based on the expense type, not the auto include.
        value.EstimatedParticipants = $scope.model.EstimatedParticipants;
      }
    });
    //If WDA
    if ($scope.model.ProgramType == utils.ProgramTypes.WDAService) {
      recalculateParticipantCosts();
    }
  }

  init();
});






