var MathFunction = require("../../shared/math-functions");
var utils = require('../../shared/utils');
var errors = [];

app.controller('ClaimReportingView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'ClaimReportingView',
    displayName: 'Claim Reporting',
    save: {
      url: '/Ext/Claim/Cost',
      method: 'PUT',
      data: function () {
        $scope.EligibleCostSuccessMessage = '';
        $scope.EligibleCostSummaryMessage = '';

        var model = {
          participantsPaidForExpenses: $scope.participantsPaidForExpenses,
          participantsHaveBeenReimbursed: $scope.participantsHaveBeenReimbursed,
          EligibleCosts: $scope.model.Claim.EligibleCosts
        };
        return model;
      },
      backup: true
    },
    onSave: function (event, data) {
      if ($scope.redirectToReview) {
        let claimReviewUrl = '/Ext/Claim/Reporting/Review/View/' + $scope.section.claimId + '/' + $scope.section.claimVersion;
        window.location = claimReviewUrl;
        return;
      }

      $scope.IsValid = false;

      $scope.EligibleCostSuccessMessage = "Claimed costs saved successfully.";
      angular.element("html, body").animate({ scrollTop: $('h2').offset().top }, 300);

    },
    grantApplicationId: $attrs.ngGrantApplicationId,
    claimId: $attrs.ngClaimId,
    claimVersion: $attrs.ngClaimVersion
  };

  $scope.$on('updateAttachments', function (event, data) {
    $scope.totalAttachments = data;
  });

  $scope.redirectToReview = false;
  $scope.participantsPaidForExpenses = null;
  $scope.participantsHaveBeenReimbursed = null;

  $scope.allowSubmitButton = false;

  $scope.totalAttachments = 0;

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make an AJAX request to fetch the claim data.
   * @function loadClaim
   * @returns {Promise}
   **/
  function loadClaim() {
    return $scope.load({
      url: '/Ext/Claim/Report/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  /**
   * Initialize the form data.
   * @function loadClaim
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadClaim(),
      loadGrantFile()
    ])
      .catch(angular.noop);
  }

  /**
   * Refresh the claim data.
   * @function loadClaim
   * @returns {Promise}
   **/
  $scope.loadClaim = function () {
    return loadClaim()
      .catch(angular.noop);
  }

  function loadGrantFile() {
    return $scope.load({
      url: '/Ext/Reporting/Grant/File/' + $scope.section.grantApplicationId,
      set: 'grantFile'
    });
  }

  /**
   * Resync the claim row version in each eligible cost item.
   * @function resyncClaim
   * @param {any} rowVersion - The latest row version.
   * @returns {Promise}
   */
  $scope.resyncClaim = function (rowVersion) {
    return $timeout(function () {
      for (let i = 0; i < $scope.model.Claim.EligibleCosts.length; i++) {
        let item = $scope.model.Claim.EligibleCosts[i];
        item.ClaimRowVersion = rowVersion;
      }
    });
  }

  $scope.toggle = function (claimEligibleCost, attendanceCompleted) {
    if (!attendanceCompleted)
      return;

    var el = angular.element("#claim-eligible-cost-" + claimEligibleCost.Id);
    if (el.is(":hidden")) {
      el.show();

      var t = angular.element("#panel-header-eligible-cost-" + claimEligibleCost.Id).find("span.k-icon");
      t.removeClass("k-i-arrow-s");
      t.removeClass("k-panelbar-expand");
      t.addClass("k-i-arrow-n");
      t.addClass("k-panelbar-collapse");

      var l = angular.element("#panel-header-toggle-eligible-cost-icon-" + claimEligibleCost.Id); //.find("span.k-icon");
      l.removeClass("down-arrow");
      l.addClass("up-arrow");

    } else {
      var t = angular.element("#panel-header-eligible-cost-" + claimEligibleCost.Id).find("span.k-icon");
      t.removeClass("k-i-arrow-n");
      t.removeClass("k-panelbar-collapse");
      t.addClass("k-i-arrow-s");
      t.addClass("k-panelbar-expand");
      el.hide();

      var l = angular.element("#panel-header-toggle-eligible-cost-icon-" + claimEligibleCost.Id); //.find("span.k-icon");
      l.removeClass("up-arrow");
      l.addClass("down-arrow");
    }
  }

  $scope.recalculateTrainingCost = function (claimEligibleCost) {
    var claimParticipants;
    var claimCost;

    return $timeout(function () {
      //console.group("recalculateTrainingCost");

      if (claimEligibleCost.ServiceType == utils.ServiceTypes.SkillsTraining ||
        claimEligibleCost.ServiceType == utils.ServiceTypes.EmploymentServicesAndSupports ||
        claimEligibleCost.ServiceType == utils.ServiceTypes.Administration) {

        if (claimEligibleCost.ServiceType == utils.ServiceTypes.SkillsTraining || claimEligibleCost.ServiceType == utils.ServiceTypes.EmploymentServicesAndSupports) {
          //claimParticipants = claimEligibleCost.CountAttended;
          claimParticipants = claimEligibleCost.EligibleCosts.AgreedMaxParticipants;

          if (claimEligibleCost.Breakdowns != null && claimEligibleCost.Breakdowns.length > 0) {
            claimEligibleCost.ClaimCost = 0.0;
            for (var i = 0; i < claimEligibleCost.Breakdowns.length; i++) {
              claimEligibleCost.ClaimCost = claimEligibleCost.ClaimCost + (claimEligibleCost.Breakdowns[i].ClaimCost / 1);
            }
          }

          claimCost = claimEligibleCost.ClaimCost == null ? 0 : claimEligibleCost.ClaimCost;
          claimEligibleCost.ClaimMaxParticipantCost = claimParticipants == 0 ? 0 : MathFunction.truncate(claimCost / claimParticipants * 100);

          if (claimEligibleCost.ClaimMaxParticipantCost > claimEligibleCost.AgreedMaxParticipantCost)
            claimEligibleCost.ClaimMaxParticipantCost = claimEligibleCost.AgreedMaxParticipantCost;

          claimEligibleCost.ClaimMaxParticipantReimbursementCost = MathFunction.truncate(claimEligibleCost.ClaimMaxParticipantCost * claimEligibleCost.AgreedReimbursementRate * 100);
          claimEligibleCost.ClaimParticipantEmployerContribution = claimEligibleCost.ClaimMaxParticipantCost - claimEligibleCost.ClaimMaxParticipantReimbursementCost;
          claimEligibleCost.ClaimMaxReimbursement = MathFunction.truncate(claimCost * claimEligibleCost.AgreedReimbursementRate * 100);
          claimEligibleCost.ClaimEmployerContribution = claimEligibleCost.ClaimCost - claimEligibleCost.ClaimMaxReimbursement;
        }
        else {
          claimCost = claimEligibleCost.ClaimCost == null ? 0 : claimEligibleCost.ClaimCost;
          claimEligibleCost.ClaimMaxParticipantReimbursementCost = MathFunction.truncate(claimCost * claimEligibleCost.AgreedReimbursementRate * 100);
          claimEligibleCost.ClaimParticipantEmployerContribution = claimCost - claimEligibleCost.ClaimMaxParticipantReimbursementCost;
          claimEligibleCost.ClaimMaxReimbursement = MathFunction.truncate(claimCost * claimEligibleCost.AgreedReimbursementRate * 100);
          claimEligibleCost.ClaimEmployerContribution = claimEligibleCost.ClaimCost - claimEligibleCost.ClaimMaxReimbursement;
        }

        claimEligibleCost.ClaimTotalPaid = Math.min(
          MathFunction.round((claimEligibleCost.ClaimCost * claimEligibleCost.AgreedReimbursementRate) * 100) / 100,
          MathFunction.round(((claimEligibleCost.AgreedMaxCost * claimEligibleCost.AgreedReimbursementRate) - claimEligibleCost.TotalPaidClaimedToDate) * 100) / 100);

        $scope.calculateGrantTotal();
        $scope.validateActualTraining(claimEligibleCost);
      }
      else {
        if (!$.isNumeric(claimEligibleCost.ClaimCost)) {
          claimEligibleCost.ClaimCost = 0;
        }

        claimCost = claimEligibleCost.ClaimCost == null ? 0 : claimEligibleCost.ClaimCost;
        claimParticipants = claimEligibleCost.ParticipantCosts.filter((participantCost) => participantCost.ClaimParticipantCost > 0).length;

        var sumOfParticipantCosts = claimEligibleCost.ParticipantCosts.reduce(function (total, pc) {
          return total + MathFunction.truncate(pc.ClaimParticipantCost * 100);
        }, 0);

        claimEligibleCost.SumOfParticipantCostUnitsUnassigned = claimEligibleCost.ClaimCost - sumOfParticipantCosts;

        //if ($scope.model.ProgramType === 1) {
        //claimEligibleCost.ClaimMaxParticipantCost = ($scope.model.Claim.CountParticipants === 0) ? 0 : MathFunction.truncate(claimCost / $scope.model.Claim.CountParticipants * 100);
        claimEligibleCost.ClaimMaxParticipantCost = (claimEligibleCost.AgreedMaxParticipants === 0) ? 0 : MathFunction.truncate(claimCost / claimEligibleCost.AgreedMaxParticipants * 100);

        let maxCostPerParticipant = claimEligibleCost.AgreedMaxParticipantCost * claimEligibleCost.AgreedReimbursementRate;

        let participantsAttended = $scope.model.Claim.Attended.length;

        let maxTotalCost = maxCostPerParticipant * participantsAttended;
        //console.log("Max per " + maxCostPerParticipant);
        //console.log("Max total " + maxTotalCost);

        var calculatedPer = claimCost * claimEligibleCost.AgreedReimbursementRate;
        //console.log("Calc per " + calculatedPer);
        //console.log("Calc Max: " + Math.min(calculatedPer, maxCostPerParticipant));
        

        let result = Math.min(
          claimCost === 0 ? 0 : MathFunction.truncate($scope.model.Claim.TotalApprovedAmount / claimEligibleCost.AgreedMaxParticipants * 100),
          claimCost === 0 ? 0 : MathFunction.truncate(claimCost / claimEligibleCost.AgreedMaxParticipants * claimEligibleCost.AgreedReimbursementRate * 100),
          claimEligibleCost.AgreedMaxParticipantCost);

        //result = claimEligibleCost.AgreedMaxParticipantCost * claimEligibleCost.AgreedReimbursementRate;
        //console.group('Claim values');
        //console.log('A: ' + MathFunction.truncate($scope.model.Claim.TotalApprovedAmount / claimEligibleCost.AgreedMaxParticipants * 100));
        //console.log('B: ' + MathFunction.truncate(claimCost / claimEligibleCost.AgreedMaxParticipants * claimEligibleCost.AgreedReimbursementRate * 100));

        //console.log('Claim Participants: ' + claimEligibleCost.ClaimParticipants);

        //console.log('Total Approved: ' + $scope.model.Claim.TotalApprovedAmount);
        //console.log('Agreed Max: ' + claimEligibleCost.AgreedMaxParticipants);
        //console.log('Agreed Rate: ' + claimEligibleCost.AgreedReimbursementRate);
        //console.log('Agreed Max Participant Cost: ' + claimEligibleCost.AgreedMaxParticipantCost);
        //console.log('Claim Cost: ' + claimCost);
        //console.log('Max Part Cost: ' + claimEligibleCost.ClaimMaxParticipantCost);
        //console.log('Max Part Reimb Cost: ' + claimEligibleCost.ClaimMaxParticipantReimbursementCost);
        //
        //console.log('Calculated Max Reimb: ' + result);
        //
        //console.groupEnd('Claim values');

        claimEligibleCost.ClaimMaxParticipantReimbursementCost = result;  // This is the per cost, "Maximum government contribution per participant"
        claimEligibleCost.ClaimParticipantEmployerContribution = claimEligibleCost.ClaimMaxParticipantCost - claimEligibleCost.ClaimMaxParticipantReimbursementCost;

        //$scope.model.Claim.EligibleCosts.forEach($scope.recalculateParticipantCosts);

        //}
        //else {
        //  claimEligibleCost.ClaimMaxParticipantCost = claimParticipants == 0 ? 0 : MathFunction.truncate(claimCost / claimParticipants * 100);

        //  if (claimEligibleCost.ClaimMaxParticipantCost > claimEligibleCost.AgreedMaxParticipantCost)
        //    claimEligibleCost.ClaimMaxParticipantCost = claimEligibleCost.AgreedMaxParticipantCost;

        //  claimEligibleCost.ClaimMaxParticipantReimbursementCost = MathFunction.truncate(claimEligibleCost.ClaimMaxParticipantCost * claimEligibleCost.AgreedReimbursementRate * 100);
        //  claimEligibleCost.ClaimParticipantEmployerContribution = claimEligibleCost.ClaimMaxParticipantCost - claimEligibleCost.ClaimMaxParticipantReimbursementCost;
        //}

        $scope.calculateGrantTotal();
        $scope.validateActualTraining(claimEligibleCost);
      }
      console.groupEnd();
    });
  }

  $scope.recalculateParticipantCost = function (participantCost, claimEligibleCost) {
    //console.group("recalculateParticipantCost");

    if (!$.isNumeric(participantCost.ClaimParticipantCost)) {
      participantCost.ClaimParticipantCost = 0;
    }
    var claimParticipantCost = participantCost.ClaimParticipantCost == null ? 0 : participantCost.ClaimParticipantCost;
    if ($scope.model.ProgramType !== 1) {
      if (claimParticipantCost > claimEligibleCost.AgreedMaxParticipantCost)
        claimParticipantCost = claimEligibleCost.AgreedMaxParticipantCost;
      participantCost.ClaimReimbursement = MathFunction.truncate(claimParticipantCost * participantCost.Rate * 100);
      participantCost.ClaimEmployerContribution = claimParticipantCost - participantCost.ClaimReimbursement;
    }

    var sumOfParticipantCosts = claimEligibleCost.ParticipantCosts.reduce(function (total, pc) {
      return total + MathFunction.truncate(pc.ClaimParticipantCost * 100);
    }, 0);
    claimEligibleCost.SumOfParticipantCostUnitsUnassigned = claimEligibleCost.ClaimCost - sumOfParticipantCosts;

    let maxPerParticipantCost = ($scope.model.Claim.TotalApprovedAmount / claimEligibleCost.AgreedMaxParticipants);
    let rule0 = (claimParticipantCost === 0) ? 0 : claimParticipantCost;
    let rule1 = (claimEligibleCost.ClaimCost === 0) ? 0 : MathFunction.truncate(claimEligibleCost.ClaimCost / claimEligibleCost.AgreedMaxParticipants * 100);
    let rule2 = (claimEligibleCost.ClaimCost === 0) ? 0 : MathFunction.truncate($scope.model.Claim.TotalApprovedAmount / claimEligibleCost.AgreedMaxParticipants * 100);
    let rule3 = (claimEligibleCost.ClaimCost === 0) ? 0 : MathFunction.truncate(((maxPerParticipantCost * $scope.model.Claim.CountParticipants) + participantCost.ClaimReimbursement - $scope.model.Claim.TotalClaimReimbursement) * 100);
    let rule4 = (claimParticipantCost === 0) ? 0 : MathFunction.truncate(claimParticipantCost * participantCost.Rate * 100);

    let result = Math.min(rule0, rule1, rule2, rule3, rule4, claimEligibleCost.AgreedMaxParticipantCost, claimEligibleCost.ClaimMaxParticipantReimbursementCost);

    //console.log('Claim Cost: ' + claimParticipantCost);
    //console.log('Claim REimb: ' + participantCost.ClaimReimbursement);
    //console.log('Rule 0: ' + rule0);
    //console.log('Rule 1: ' + rule1);
    //console.log('Rule 2: ' + rule2);
    //console.log('Rule 3: ' + rule3);
    //console.log('Rule 4: ' + rule4);
    //console.log('Result: ' + result);
    //
    //console.groupEnd();

    participantCost.ClaimReimbursement = result;  // This is the 'per participant/per cost' "Government contribution per participant"
    participantCost.ClaimEmployerContribution = claimParticipantCost - participantCost.ClaimReimbursement;

    $scope.calculateGrantTotal();

    //if ($scope.model.ProgramType === 1) {
    //Recalculate other participant costs
    //let otherParticipantCosts = claimEligibleCost.ParticipantCosts.filter((pc) => pc.Id !== participantCost.Id);
    //for (var i = 0; i < otherParticipantCosts.length; i++) {
    //  let participantCost = otherParticipantCosts[i];
    //  let claimParticipantCost = participantCost.ClaimParticipantCost == null ? 0 : participantCost.ClaimParticipantCost;
    //  let maxPerParticipantCost = ($scope.model.Claim.TotalApprovedAmount / $scope.model.Claim.MaximumParticipants);

    //  let ruleX = (claimParticipantCost === 0) ? 0 : claimParticipantCost;
    //  let ruleA = (claimEligibleCost.ClaimCost === 0) ? 0 : MathFunction.truncate(claimEligibleCost.ClaimCost / $scope.model.Claim.CountParticipants * 100);
    //  let ruleB = (claimEligibleCost.ClaimCost === 0) ? 0 : MathFunction.truncate($scope.model.Claim.TotalApprovedAmount / $scope.model.Claim.MaximumParticipants * 100);
    //  let ruleC = (claimEligibleCost.ClaimCost === 0) ? 0 : MathFunction.truncate(((maxPerParticipantCost * $scope.model.Claim.CountParticipants) + participantCost.ClaimReimbursement - $scope.model.Claim.TotalClaimReimbursement) * 100);
    //  let ruleD = (claimParticipantCost === 0) ? 0 : MathFunction.truncate(claimParticipantCost * participantCost.Rate * 100);

    //  let result = Math.min(ruleA, ruleB, ruleC, ruleD, ruleX, claimEligibleCost.AgreedMaxParticipantCost, claimEligibleCost.ClaimMaxParticipantReimbursementCost);

    //  participantCost.ClaimReimbursement = result;
    //  participantCost.ClaimEmployerContribution = claimParticipantCost - participantCost.ClaimReimbursement;

    //  $scope.calculateGrantTotal();
    //}
    //}

    $scope.validateActualTraining(claimEligibleCost);

    //Recalculate other eligible costs
    //let otherEligibleCosts = $scope.model.Claim.EligibleCosts.filter((ec) => ec.Id !== claimEligibleCost.Id);
    //otherEligibleCosts.forEach($scope.recalculateParticipantCosts);
  }

  $scope.recalculateParticipantCosts = function (claimEligibleCost) {
    //console.group("recalculateParticipantCosts");
    //for (var i = 0; i < claimEligibleCost.ParticipantCosts.length; i++) {
    //  let participantCost = claimEligibleCost.ParticipantCosts[i];
    //  let claimParticipantCost = participantCost.ClaimParticipantCost == null ? 0 : participantCost.ClaimParticipantCost;
    //  let participantsAttended = $scope.model.Claim.Attended.length;

    //  let maxPerParticipantCost = ($scope.model.Claim.TotalApprovedAmount / claimEligibleCost.AgreedMaxParticipants);

    //  let rule0 = (claimParticipantCost === 0) ? 0 : claimParticipantCost;
    //  let rule1 = (claimEligibleCost.ClaimCost === 0) ? 0 : MathFunction.truncate(claimEligibleCost.ClaimCost / claimEligibleCost.AgreedMaxParticipants * 100);
    //  let rule2 = (claimEligibleCost.ClaimCost === 0) ? 0 : MathFunction.truncate($scope.model.Claim.TotalApprovedAmount / claimEligibleCost.AgreedMaxParticipants * 100);
    //  let rule3 = (claimEligibleCost.ClaimCost === 0) ? 0 : MathFunction.truncate(((maxPerParticipantCost * claimEligibleCost.AgreedMaxParticipants) + participantCost.ClaimReimbursement - $scope.model.Claim.TotalClaimReimbursement) * 100);
    //  let rule4 = (claimParticipantCost === 0) ? 0 : MathFunction.truncate(claimParticipantCost * participantCost.Rate * 100);

    //  participantCost.ClaimReimbursement = Math.min(rule0, rule1, rule2, rule3, rule4, claimEligibleCost.AgreedMaxParticipantCost, claimEligibleCost.ClaimMaxParticipantReimbursementCost);
    //  participantCost.ClaimEmployerContribution = claimParticipantCost - participantCost.ClaimReimbursement;


    //  console.log('Claim Cost: ' + claimParticipantCost);
    //  console.log('Claim REimb: ' + participantCost.ClaimReimbursement);
    //  console.log('Rule 0: ' + rule0);
    //  console.log('Rule 1: ' + rule1);
    //  console.log('Rule 2: ' + rule2);
    //  console.log('Rule 3: ' + rule3);
    //  console.log('Rule 4: ' + rule4);

    //  $scope.calculateGrantTotal();
    //}
    $scope.calculateGrantTotal();
    //console.groupEnd();
  }

  $scope.calculateGrantTotal = function () {
    //console.group("calculateGrantTotal");

    if ($scope.model.Claim.EligibleCosts != null && $scope.model.Claim.EligibleCosts.length > 0) {
      $scope.model.Claim.TotalClaimReimbursement = 0;

      for (var i = 0; i < $scope.model.Claim.EligibleCosts.length; i++) {
        $scope.model.Claim.EligibleCosts[i].TotalClaimedReimbursement = 0;

        if ($scope.model.Claim.EligibleCosts[i].ServiceType == utils.ServiceTypes.SkillsTraining ||
          $scope.model.Claim.EligibleCosts[i].ServiceType == utils.ServiceTypes.EmploymentServicesAndSupports ||
          $scope.model.Claim.EligibleCosts[i].ServiceType == utils.ServiceTypes.Administration) {
          $scope.model.Claim.EligibleCosts[i].TotalClaimedReimbursement = $scope.model.Claim.EligibleCosts[i].ClaimTotalPaid;
        } else {
          for (var j = 0; j < $scope.model.Claim.EligibleCosts[i].ParticipantCosts.length; j++) {
            $scope.model.Claim.EligibleCosts[i].TotalClaimedReimbursement = $scope.model.Claim.EligibleCosts[i].TotalClaimedReimbursement + $scope.model.Claim.EligibleCosts[i].ParticipantCosts[j].ClaimReimbursement;
          }
        }
        $scope.model.Claim.TotalClaimReimbursement = $scope.model.Claim.TotalClaimReimbursement + $scope.model.Claim.EligibleCosts[i].TotalClaimedReimbursement;
      }
    }
    console.groupEnd();
  }

  $scope.validateActualTraining = function (claimEligibleCost) {
    errors = [];
    if (!claimEligibleCost.ClaimCost && claimEligibleCost.ClaimCost != 0) {
      errors.push('Training Cost must have a value.');
      return;
    }

    if (claimEligibleCost.ServiceType == utils.ServiceTypes.SkillsTraining ||
      claimEligibleCost.ServiceType == utils.ServiceTypes.EmploymentServicesAndSupports ||
      claimEligibleCost.ServiceType == utils.ServiceTypes.Administration) {

      var actualTrainingCost = claimEligibleCost.ClaimCost;
      var actualParticipant = claimEligibleCost.ClaimParticipants;
      var actualParticipantCost = claimEligibleCost.ClaimMaxParticipantCost;
      var actualGovContribution = claimEligibleCost.ClaimMaxReimbursement;

      var remainingTrainingCost = claimEligibleCost.RemainingToClaimed;
      var remainingParticipantCost = claimEligibleCost.ParticipantCostRemainingToClaimed;
      var remainingGovContribution = claimEligibleCost.MaxReimbursementRemainingToClaimed;

      var agreedMaxCost = claimEligibleCost.AgreedMaxCost;
      var agreedMaxPart = claimEligibleCost.AgreedMaxParticipants;

      if (claimEligibleCost.ExpenseType == utils.ExpenseTypes.ParticipantAssigned || claimEligibleCost.ExpenseType == utils.ExpenseTypes.NotParticipantLimited) {
        if (actualTrainingCost >= 0 && actualTrainingCost > remainingTrainingCost) {
          errors.push('The new claim Total Cost cannot exceed the remaining to be claimed amount of $' + remainingTrainingCost + ' for ' + claimEligibleCost.EligibleExpenseTypeCaption + '.');
        }

        if (actualTrainingCost < 0 && remainingTrainingCost - actualTrainingCost > agreedMaxCost) {
          errors.push('The negative new claim Total Cost cannot exceed the amount owing $' + (agreedMaxCost - remainingTrainingCost) + ' for remaining to be claimed in ' + claimEligibleCost.EligibleExpenseTypeCaption + '.');
        }

        if (actualParticipant > agreedMaxPart) {
          errors.push('Number of Participants cannot exceed Agreement Number of Participants.');
        }

        if ((claimEligibleCost.ExpenseType == utils.ExpenseTypes.ParticipantAssigned) && (actualParticipantCost > remainingParticipantCost)) {
          errors.push('The new claim Maximum Cost per Participant cannot exceed the remaining to be claimed amount for ' + claimEligibleCost.EligibleExpenseTypeCaption + '.');
        }

        if (actualGovContribution > remainingGovContribution) {
          errors.push('The Maximum Government Contribution cannot exceed the remaining to be claimed amount for ' + claimEligibleCost.EligibleExpenseTypeCaption + '.');
        }

        errors.length == 0 ? angular.element("#claim-cost-" + claimEligibleCost.Id).removeClass('validation-error') : angular.element("#claim-cost-" + claimEligibleCost.Id).addClass('validation-error');
      }
    }
    else {
      if (claimEligibleCost.ClaimParticipants == null) {
        errors.push('The Number of Participants must be provided.');
        angular.element("#claim-participant-" + claimEligibleCost.Id).addClass('validation-error');
        return;
      }
      else {
        if (errors.length == 0) {
          angular.element("#claim-participant-" + claimEligibleCost.Id).removeClass('validation-error');
        }
      }

      var actualTrainingCost = claimEligibleCost.ClaimCost;
      var actualParticipant = claimEligibleCost.ParticipantCosts.filter((participantCost) => participantCost.ClaimParticipantCost > 0).length;
      var maxPerParticipant = claimEligibleCost.ClaimMaxParticipantCost;
      var agreedMaxPart = claimEligibleCost.AgreedMaxParticipants;
      var agreedMaxPerParticipant = claimEligibleCost.AgreedMaxParticipantCost;

      if (actualParticipant > agreedMaxPart) {
        errors.push(`Number of Participants exceed Agreement limit for expense type '${claimEligibleCost.EligibleExpenseTypeCaption}'.`);
        angular.element("#claim-participant-" + claimEligibleCost.Id).addClass('validation-error');
      }
      else {
        angular.element("#claim-participant-" + claimEligibleCost.Id).removeClass('validation-error');
      }

      if ($scope.model.ProgramType !== 1 && maxPerParticipant > agreedMaxPerParticipant) {
        errors.push('Maximum Cost per Participant exceeds the Agreement Maximum Cost per Participant.');
        angular.element("#claim-max-participant-cost-" + claimEligibleCost.Id).addClass('validation-error');
      }
      else {
        angular.element("#claim-max-participant-cost-" + claimEligibleCost.Id).removeClass('validation-error');
      }

      if ($scope.model.ProgramType === 1 && claimEligibleCost.SumOfParticipantCostUnitsUnassigned < 0) {
        errors.push(`The sum of all participant costs exceeds the Paid Amount for expense type '${claimEligibleCost.EligibleExpenseTypeCaption}'.`);
        angular.element("#claim-cost-" + claimEligibleCost.Id).addClass('validation-error');
      }
      else {
        angular.element("#claim-cost-" + claimEligibleCost.Id).removeClass('validation-error');
      }

      $scope.validateParticipants(claimEligibleCost.ParticipantCosts, maxPerParticipant, actualParticipant);
    }
    $scope.displayErrors(claimEligibleCost);
  }

  $scope.validateParticipants = function (participantCosts, maxPerParticipant, actualParticipant) {
    var participantTotal = 0;
    var numParticipants = 0;

    for (var i = 0; i < participantCosts.length; i++) {
      var partVal = participantCosts[i].ClaimParticipantCost == null ? 0 : participantCosts[i].ClaimParticipantCost;
      participantTotal += partVal;
      if (partVal > 0) {
        numParticipants += 1;
      }
    }

    if (numParticipants > actualParticipant) {
      errors.push('Number of participants with assigned cost exceeds Number of Participants Approved.');
    }
  }

  $scope.toggleParticipants = function () {
    var el = angular.element("#participantAttendance");

    if (el.is(":hidden")) {
      el.show();
      var t = angular.element("#panel-header-participantAttendance").find("span.k-icon");
      t.removeClass("k-i-arrow-s");
      t.removeClass("k-panelbar-expand");
      t.addClass("k-i-arrow-n");
      t.addClass("k-panelbar-collapse");
      var l = angular.element("#panel-header-toggle-icon-participantAttendance"); //.find("span.k-icon");
      l.removeClass("down-arrow");
      l.addClass("up-arrow");

    } else {
      var t = angular.element("#panel-header-participantAttendance").find("span.k-icon");
      t.removeClass("k-i-arrow-n");
      t.removeClass("k-panelbar-collapse");
      t.addClass("k-i-arrow-s");
      t.addClass("k-panelbar-expand");
      el.hide();
      var l = angular.element("#panel-header-toggle-icon-participantAttendance"); //.find("span.k-icon");
      l.removeClass("up-arrow");
      l.addClass("down-arrow");
    }
  }

  $scope.saveAndReview = function () {
    $scope.redirectToReview = true;
    $scope.save();
  }

  $scope.saveAttendance = function () {
    return $scope.ajax({
      url: '/Ext/Claim/Attendance',
      method: 'PUT',
      data: function () {
        return $scope.model.Claim;
      },
    }).catch(angular.noop)
      .then(function () {
        $scope.loadClaim();
      });
  }

  $scope.displayErrors = function () {
    $scope.EligibleCostSuccessMessage = '';
    $scope.EligibleCostSummaryMessage = '';
    if (errors.length > 0) {
      var errorSummary;
      for (var i = 0; i < errors.length; i++) {
        if (errorSummary) {
          errorSummary = errorSummary + '<br/>' + errors[i];
        }
        else {
          errorSummary = errors[i];
        }
      }
      $scope.EligibleCostSummaryMessage = errorSummary;
      $scope.IsValid = false;
    } else {
      $scope.IsValid = true;
    }
  }

  init();
});
