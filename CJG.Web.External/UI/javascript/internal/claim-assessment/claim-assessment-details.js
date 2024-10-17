var MathFunction = require("../../shared/math-functions");
var utils = require('../../shared/utils');
app.controller('ClaimAssessmentDetails',
  function($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
    $scope.section = {
      name: 'ClaimAssessmentDetails',
      displayName: 'Claim Assessment Details',
      save: {
        url: "/Int/Claim",
        method: 'PUT',
        data: function() {
          return $scope.model;
        },
        backup: true
      },
      onSave: function () {
        $scope.section.notesExternallyUpdated = false;
        $scope.resyncClaimDetails();
      },
      loaded: function() {
        return $scope.model &&
          $scope.model.RowVersion &&
          $scope.claim &&
          $scope.model.RowVersion === $scope.claim.RowVersion;
      },
      onRefresh: function() {
        return loadClaim().catch(angular.noop);
      },
      canSave: true,
      notesExternallyUpdated: false
  };

    $scope.$on('notesUpdated', function (event, data) {
      $scope.section.notesExternallyUpdated = data.updated;
    });

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
  * Initialize section data.
  * @function init
  * @returns {Promise}
  **/
  $scope.init = function () {
    return Promise.all([
      loadClaim()
    ]).catch(angular.noop);
  }

  /**
   * Make AJAX request for claim
   * @function loadClaim
   * @returns {Promise}
   **/
  function loadClaim() {
    return $scope.load({
      url: '/Int/Claim/Assessment/Details/' + $scope.parent.claimId + '/' + $scope.parent.claimVersion,
      set: 'model'
    }).catch(angular.noop);
  }

  /**
   * Switch the form to edit mode.
   * Open the first claim eligible cost if none are opened already.
   * @function editClaim
   * @param {any} $event - The event.
   * @returns {void}
   */
    $scope.editClaim = function ($event) {
      if (!$scope.section.notesExternallyUpdated) {
        var toggle = $scope.model.EligibleCosts.find(function(ec) {
          return ec.show;
        });

        if (!toggle)
          $scope.model.EligibleCosts[0].show = true;
        $scope.edit($event);
      } else {
        return $scope.confirmDialog('Claim notes updated', 'Claim notes have been modified. Finish editing before attempting to edit Eligible Expense Types, otherwise you may lose your changes.<br />Click "Yes" to edit Expenses and ignore updated notes changes.')
          .then(function () {
            var toggle = $scope.model.EligibleCosts.find(function (ec) {
              return ec.show;
            });

            if (!toggle)
              $scope.model.EligibleCosts[0].show = true;
            $scope.edit($event);
          }).catch(angular.noop);
      }
  }

  /**
   * Show or hide the eligible cost.
   * @function toggle
   * @param {object} claimEligibleCost - The eligible cost to show or hide.
   * @returns {void}
   */
  $scope.toggle = function (claimEligibleCost) {
    var show = typeof (claimEligibleCost.show) === 'undefined' ? true : !claimEligibleCost.show;
    $scope.model.EligibleCosts.map(function (eligibleCost) {
      eligibleCost.show = false;
    });
    claimEligibleCost.show = show;
  }

  /**
   * Show hide the participant costs based on the selected filter.
   * @function filterParticipants
   * @param {any} claimEligibleCost - The eligible cost.
   * @returns {void}
   */
  $scope.filterParticipants = function (claimEligibleCost) {
    claimEligibleCost.ParticipantCosts.map(function (participantCost) {
      participantCost.filter = claimEligibleCost.filter == 'claimed' && participantCost.ClaimParticipantCost === 0 ? true : false;
    });
  }

  /**
   * Clear Assessed Value
   * @function clearAssessedValue
   * @param {object} claimEligibleCost - Claim Eligible Cost.
   * @returns {void}
   */
  $scope.clearAssessedValue = function (claimEligibleCost) {
    claimEligibleCost.AssessedCost = 0;
    claimEligibleCost.AssessedParticipants = 0;

    for (let i = 0; i < claimEligibleCost.ParticipantCosts.length; i++) {
      $scope.clearAssessedParticipantCost(claimEligibleCost.ParticipantCosts[i], claimEligibleCost);
    }
    $scope.recalculateTrainingCost(claimEligibleCost);
  }

  /**
   * clear Assessed Participant Cost
   * @function clearAssessedParticipantCost
   * @param {object} participantCost - Participant Cost.
   * @param {object} claimEligibleCost - Claim Eligible Cost.
   * @returns {void}
   */
  $scope.clearAssessedParticipantCost = function (participantCost, claimEligibleCost) {
    participantCost.AssessedParticipantCost = 0;
    $scope.recalculateParticipantCost(participantCost, claimEligibleCost);
  }

  /**
   * Recalculate Training Cost
   * @function recalculateTrainingCost
   * @param {object} claimEligibleCost - Claim Eligible Cost.
   * @param {bool} override - Whether to allow overriding the reimbursement amounts.
   * @returns {void}
   */
  $scope.recalculateTrainingCost = function (claimEligibleCost, override) {
    $scope.errors = {};

    if (!$.isNumeric(claimEligibleCost.AssessedCost)) {
      claimEligibleCost.AssessedCost = 0;
    }
    var assessedCost = claimEligibleCost.AssessedCost;
    var claimParticipants = claimEligibleCost.AssessedParticipants == null ? 0 : claimEligibleCost.AssessedParticipants;

    claimEligibleCost.AssessedMaxParticipantCost = (assessedCost === 0 || claimEligibleCost.AssessedParticipants === 0)
      ? 0
      : MathFunction.truncate(assessedCost / claimEligibleCost.AgreedMaxParticipants * 100);
    // CJG-1183
          claimEligibleCost.AssessedMaxParticipantCost = claimEligibleCost.AgreedMaxParticipantCost;

    claimEligibleCost.AssessedMaxParticipantReimbursementCost = Math.min(
      (assessedCost === 0) ? 0 : MathFunction.truncate(assessedCost / claimParticipants * 100),
      (assessedCost === 0) ? 0 : MathFunction.truncate($scope.model.AgreedMaxCommittment / $scope.model.MaximumParticipants * 100),
      (assessedCost === 0) ? 0 : MathFunction.truncate((assessedCost / claimParticipants) * claimEligibleCost.ReimbursementRate * 100),
      claimEligibleCost.AgreedMaxParticipantCost);

    claimEligibleCost.AssessedParticipantEmployerContribution = (assessedCost === 0)
      ? 0
      : MathFunction.truncate((claimEligibleCost.AssessedMaxParticipantCost - claimEligibleCost.AssessedMaxParticipantReimbursementCost) * 100);

    if (claimEligibleCost.ParticipantCosts != null) {
      claimEligibleCost.TotalAssessedParticipantReimbursement = claimEligibleCost.ParticipantCosts.reduce(function (a, b) {
        return a + b.AssessedReimbursement;
      }, 0);
    }

    // Clear participant costs
    claimEligibleCost.ParticipantCosts.map(function (participantCost) {
      participantCost.AssessedParticipantCost = 0;
      participantCost.AssessedReimbursement = 0;
      participantCost.AssessedEmployerContribution = 0;
    });

    validateClaimEligibleCost(claimEligibleCost);
    calculateGrantTotal();
  }

  /**
   * Recalculate Participant Cost
   * @function recalculateParticipantCost
   * @param {object} participantCost - Participant Cost.
   * @param {object} claimEligibleCost - Claim Eligible Cost.
   * @returns {void}
   */
  $scope.recalculateParticipantCost = function (participantCost, claimEligibleCost) {
    $scope.errors = {};
    if (!$.isNumeric(participantCost.AssessedParticipantCost)) {
      participantCost.AssessedParticipantCost = 0;
    }

    var claimParticipantCost = MathFunction.truncate(participantCost.AssessedParticipantCost * 100);
    let maxPerParticipantCost = ($scope.model.AgreedMaxCommittment / $scope.model.MaximumParticipants);

    let rule0 = (claimParticipantCost === 0) ? 0 : claimParticipantCost;
    let rule1 = (claimEligibleCost.AssessedCost === 0) ? 0 : MathFunction.truncate(claimEligibleCost.AssessedCost / claimEligibleCost.AssessedParticipants * 100);
    let rule2 = (claimEligibleCost.AssessedCost === 0) ? 0 : MathFunction.truncate($scope.claim.AgreedMaxCommittment / claimEligibleCost.AgreedMaxParticipants * 100);
    let rule3 = (claimEligibleCost.AssessedCost === 0) ? 0 : MathFunction.truncate(((maxPerParticipantCost * claimEligibleCost.AssessedParticipants) + participantCost.AssessedReimbursement - $scope.claim.TotalAssessedReimbursement) * 100);
    let rule4 = (claimParticipantCost === 0) ? 0 : MathFunction.truncate((claimParticipantCost * claimEligibleCost.OverrideRate) * 100);

    participantCost.AssessedReimbursement = Math.min(rule0, rule1, rule2, rule3, rule4, claimEligibleCost.AgreedMaxParticipantCost, claimEligibleCost.AssessedMaxParticipantReimbursementCost);
    participantCost.AssessedEmployerContribution = claimParticipantCost - participantCost.AssessedReimbursement;

    validateParticipantCost(claimEligibleCost, participantCost);
    calculateGrantTotal();
  }

    $scope.showClaimWarning = function (participants, participantFormId) {
      if (participants == null || participants.length <= 0)
        return false;

      if (participantFormId <= 0)
        return false;

      let participant = participants.filter(p => p.ParticipantFormId === participantFormId);
      if (participant.length <= 0)
        return false;

      return participant[0].HasClaimWarnings === true;
    }

  /**
   * Calculate Grant Total
   * @function calculateGrantTotal
   * @returns {void}
   */
  function calculateGrantTotal() {
    var eligibleCosts = $scope.model.EligibleCosts;
    if (eligibleCosts == null || eligibleCosts.length <= 0) {
      return;
    }
    $scope.claim.TotalAssessedReimbursement = 0;
    eligibleCosts.map(function (claimEligibleCost) {
      claimEligibleCost.TotalAssessedParticipantReimbursement = 0;

      if (claimEligibleCost.ParticipantCosts.length > 0) {
        claimEligibleCost.ParticipantCosts.map(function (participantCost) {
          claimEligibleCost.TotalAssessedParticipantReimbursement += participantCost.AssessedReimbursement;
        });
        $scope.claim.TotalAssessedReimbursement += claimEligibleCost.TotalAssessedParticipantReimbursement * 1;
      } else {
        $scope.claim.TotalAssessedReimbursement += claimEligibleCost.AssessedReimbursementCost * 1;
        if ($scope.model.AgreedMaxCommittment < $scope.claim.TotalAssessedReimbursement) {
          Utils.initValue($scope, 'errors.TotalAssessedReimbursement', 'Total government contribution cannot exceed the agreed committment.');
        }

        $scope.section.canSave = Utils.objectIsEmpty($scope.errors);
      }
    });
  }

  /**
   * Copy Object Value
   * @function copyValue
   * @param {object} object - The object.
   * @param {string} from - The object key name.
   * @param {string} to - The object key name.
   * @returns {void}
   */
  function copyValue(object, from, to) {
    object[to] = object[from];
  }

  /**
   * Validate the claim eligible cost.
   * @function validateClaimEligibleCost
   * @param {object} claimEligibleCost - A ClaimEligibleCost object.
   * @returns {void}
   */
  function validateClaimEligibleCost(claimEligibleCost) {
    var add = function (name, error) {
      Utils.initValue($scope, 'errors.EligibleCost_' + claimEligibleCost.Id + '_' + name, error);
    };

    // AssessedParticipants
    if (claimEligibleCost.AssessedParticipants === '' || claimEligibleCost.AssessedParticipants === null) {
      add('AssessedParticipants', 'The number of participants must be provided.');
    }
    if (claimEligibleCost.AgreedMaxParticipants < claimEligibleCost.AssessedParticipants) {
      add('AssessedParticipants', 'The number of participants cannot be greater than the agreed limit.');
    }

    // AssessedCost
    if (!claimEligibleCost.AssessedCost && claimEligibleCost.AssessedCost != 0) {
      add('AssessedCost', 'Training cost must be provided.');
    }
    if ($scope.model.ProgramType !== 1 && claimEligibleCost.AgreedMaxCost < claimEligibleCost.AssessedCost) {
      add('AssessedCost', 'Training cost cannot be greater than the agreed max cost.');
    }

    // AssessedReimbursementCost
    if ($scope.model.ProgramType !== 1 && claimEligibleCost.AgreedMaxReimbursement < claimEligibleCost.AssessedReimbursementCost) {
      add('AssessedReimbursementCost', 'Government contribution cannot exceed the agreed maximum reimbursement.');
    }

    // ParticipantCosts
    if (claimEligibleCost.ExpenseType === utils.ExpenseTypes.ParticipantAssigned || claimEligibleCost.ExpenseType === utils.ExpenseTypes.ParticipantLimited) {
      // AssessedMaxParticipantReimbursementCost
      if ($scope.model.ProgramType !== 1 && claimEligibleCost.AgreedMaxParticipantReimbursement < claimEligibleCost.AssessedMaxParticipantReimbursementCost) {
        add('AssessedMaxParticipantReimbursementCost', 'Government contribution per participant cannot exceed the agreed maximum reimbursement.');
      }

      // AssessedMaxParticipantCost
      if ($scope.model.ProgramType !== 1 && claimEligibleCost.AgreedMaxParticipantCost < claimEligibleCost.AssessedMaxParticipantCost) {
        add('AssessedMaxParticipantCost', 'Participant cost cannot exceed the agreed maximum.');
      }
    }

    // ParticipantCosts
    if (Array.isArray(claimEligibleCost.ParticipantCosts)) {
      claimEligibleCost.ParticipantCosts.map(function (pc) {
        validateParticipantCost(claimEligibleCost, pc);
      });
    }

    // Breakdowns
    if (Array.isArray(claimEligibleCost.Breakdowns)) {
      var breakdownCount = claimEligibleCost.Breakdowns.length;
      var breakdownSum = claimEligibleCost.Breakdowns.reduce(function (a, b) { return a + parseFloat(b.AssessedCost); }, 0);
      if (claimEligibleCost.ServiceType === utils.ServiceTypes.SkillsTraining) {
        // AssessedCost
        if (claimEligibleCost.AssessedCost != breakdownSum) {
          add('AssessedCost', 'Training cost must be the sum of the breakdown costs.');
        }
      } else if (breakdownCount > 0) {
        // AssessedCost
        if (claimEligibleCost.AssessedCost < breakdownSum) {
          add('AssessedCost', 'Training cost breakdown cannot exceed the assessed cost.');
        }
      }

      claimEligibleCost.Breakdowns.map(function (b) {
        validateBreakdown(claimEligibleCost, b);
      });
    }

    // Prior Claims
    if ($scope.model.ClaimType === utils.ClaimTypes.MultipleClaimsWithoutAmendments) {
      // AssessedCost
      if (claimEligibleCost.RemainingToBeClaimed < claimEligibleCost.AssessedCost) {
        add('AssessedCost', 'The assessed total cost cannot exceed the remaining to be claimed amount of $' + claimEligibleCost.RemainingToBeClaimed);
      }

      // AssessedMaxParticipantCost
      if (claimEligibleCost.RemainingToBeClaimedParticipantCost < claimEligibleCost.AssessedMaxParticipantCost) {
        add('AssessedMaxParticipantCost', 'The assessed maximum cost per participant cannot exceed the remaining to be claimed amount of $' + claimEligibleCost.RemainingToBeClaimedParticipantCost);
      }

      // AssessedReimbursementCost
      if (claimEligibleCost.RemainingToBeClaimedMaxReimbursement < claimEligibleCost.AssessedReimbursementCost) {
        add('AssessedReimbursementCost', 'The assessed maximum government contribution cannot exceed the remaining to be claimed amount of $' + claimEligibleCost.RemainingToBeClaimedMaxReimbursement);
      }

      // AssessedCost
      if (claimEligibleCost.AssessedCost < 0 && claimEligibleCost.RemainingToBeClaimed - claimEligibleCost.AssessedCost > claimEligibleCost.AgreedMaxCost) {
        add('AssessedCost', 'The negative assessed total cost cannot exceed the amount owing of $' + (claimEligibleCost.AgreedMaxCost - claimEligibleCost.RemainingToBeClaimed));
      }
    }

    $scope.section.canSave = Utils.objectIsEmpty($scope.errors);
  }

  /**
   * Validate participant cost.
   * @function validateParticipantCost
   * @param {object} claimEligibleCost - ClaimEligibleCost object.
   * @param {object} participantCost - ParticipantCost object.
   * @returns {void}
   */
  function validateParticipantCost(claimEligibleCost, participantCost) {
    var add = function (name, error) {
      Utils.initValue($scope, 'errors.ParticipantCost_' + participantCost.Id + '_' + name, error);
    };

    // ParticipantCosts
    if (Array.isArray(claimEligibleCost.ParticipantCosts)) {
      var numParticipantsWithCosts = 0;
      var sumParticipantCosts = 0;
      claimEligibleCost.ParticipantCosts.map(function (pc) {
        sumParticipantCosts += parseFloat(pc.AssessedParticipantCost);
        numParticipantsWithCosts += (parseFloat(pc.AssessedParticipantCost) > 0 ? 1 : 0);

        if (claimEligibleCost.AssessedParticipants < numParticipantsWithCosts) {
          Utils.initValue($scope, 'errors.ParticipantCost_' + pc.Id + '_AssessedParticipantCost', ' ');
        }

        if (claimEligibleCost.AssessedCost < sumParticipantCosts) {
          Utils.initValue($scope, 'errors.ParticipantCost_' + pc.Id + '_AssessedParticipantCost', ' ');
        }
      });

      if (participantCost.AssessedParticipantCost > 0 && claimEligibleCost.AssessedParticipants < numParticipantsWithCosts) {
        add('AssessedParticipantCost', 'Too many');
      }

      // AssessedParticipants
      if (claimEligibleCost.AssessedParticipants < numParticipantsWithCosts) {
        Utils.initValue($scope, 'errors.EligibleCost_' + claimEligibleCost.Id + '_AssessedParticipants', 'Number of participants with assigned cost exceeds maximum number of participants.');
      }

      // AssessedCost
      if (claimEligibleCost.AssessedCost < sumParticipantCosts) {
        Utils.initValue($scope, 'errors.EligibleCost_' + claimEligibleCost.Id + '_AssessedCost', 'Sum of participant costs cannot exceed the maximum assessed cost.');
      }
    }

    if (participantCost.AssessedParticipantCost > 0) {

      // AssessedParticipantCost
      if ($scope.model.ProgramType !== 1 && claimEligibleCost.AssessedMaxParticipantCost < participantCost.AssessedParticipantCost) {
        add('AssessedParticipantCost', 'Training cost per participant cannot exceed the assessed maximum.');
      }

      // AssessedReimbursement
      if (claimEligibleCost.AssessedMaxParticipantReimbursementCost < participantCost.AssessedReimbursement) {
        add('AssessedReimbursement', 'Government contribution per participant cannot exceed the assessed maximum.');
      }
    } else if (participantCost.AssessedParticipantCost < 0) {
      // AssessedParticipantCost
      if (claimEligibleCost.AssessedMaxParticipantCost > participantCost.AssessedParticipantCost) {
        add('AssessedParticipantCost', 'Training cost per participant cannot exceed the assessed maximum.');
      }

      // AssessedReimbursement
      if (claimEligibleCost.AssessedMaxParticipantReimbursementCost > participantCost.AssessedReimbursement) {
        add('AssessedReimbursement', 'Government contribution per participant cannot exceed the assessed maximum.');
      }
    }

    $scope.section.canSave = Utils.objectIsEmpty($scope.errors);
  }

  /**
   * Validate the claim breakdown cost.
   * @function validateBreakdown
   * @param {any} claimEligibleCost
   * @param {any} claimBreakdownCost
   * @returns {void}
   */
  function validateBreakdown(claimEligibleCost, claimBreakdownCost) {
    var add = function (name, error) {
      Utils.initValue($scope, 'errors.Breakdown_' + claimBreakdownCost.Id + '_' + name, error);
    };

    // AssessedCost
    if (claimEligibleCost.AssessedCost < claimBreakdownCost.AssessedCost) {
      add('AssessedCost', 'Training cost breakdown cannot exceed the assessed maximum.');
    }

    // AssessedCost
    if (claimEligibleCost.RemainingToBeClaimed < claimBreakdownCost.AssessedCost) {
      add('AssessedCost', 'Training cost breakdown cannot exceed the remaining to be claimed amount of $' + claimEligibleCost.RemainingToBeClaimed + '.');
    }

    $scope.section.canSave = Utils.objectIsEmpty($scope.errors);
  }

  /**
   * Recalculate Training Cost
   * @function copyValueAndRecalculateTrainingCost
   * @param {object} claimEligibleCost - Claim Eligible Cost.
   * @param {object} object - The object.
   * @param {string} from - The object key name.
   * @param {string} to - The object key name.
   * @returns {void}
   */
  $scope.copyValueAndRecalculateTrainingCost = function (claimEligibleCost, object, from, to) {
    if (object[to] != object[from]) {
      object[to] = object[from];
      $scope.recalculateTrainingCost(claimEligibleCost);
    }
  }

  /**
   * Copy Single Claim Value
   * @function copySingleClaimValue
   * @param {object} claimEligibleCost - Claim Eligible Cost.
   * @returns {void}
   */
  $scope.copySingleClaimValue = function (claimEligibleCost) {
    copyValue(claimEligibleCost, 'ClaimParticipants', 'AssessedParticipants');
    copyValue(claimEligibleCost, 'ClaimCost', 'AssessedCost');
    copyValue(claimEligibleCost, 'ClaimMaxParticipantReimbursementCost', 'AssessedMaxParticipantReimbursementCost');

    for (let i = 0; i < claimEligibleCost.ParticipantCosts.length; i++) {
      $scope.clearAssessedParticipantCost(claimEligibleCost.ParticipantCosts[i], claimEligibleCost);
    }

    $scope.recalculateTrainingCost(claimEligibleCost);
  }

  /**
   * Copy SParticipantValue
   * @function copyParticipantValue
   * @param {object} participantCost - Participant Cost.
   * @param {object} claimEligibleCost - Claim Eligible Cost.
   * @returns {void}
   */
  $scope.copyParticipantValue = function (participantCost, claimEligibleCost) {
    copyValue(participantCost, 'ClaimParticipantCost', 'AssessedParticipantCost');
    $scope.recalculateParticipantCost(participantCost, claimEligibleCost);
  }

  /**
   * Remove Override
   * @function removeOverride
   * @param {object} claimEligibleCost - Claim Eligible Cost.
   * @returns {Promise}
   **/
  $scope.toggleLock = function (claimEligibleCost) {
    claimEligibleCost.IsUnlocked = !claimEligibleCost.IsUnlocked;
    if (!claimEligibleCost.IsUnlocked) {
      claimEligibleCost.RemoveOverride = false;
    }
  }

  /**
   * Remove Override
   * @function removeOverride
   * @param {object} claimEligibleCost - Claim Eligible Cost.
   * @returns {Promise}
   **/
  $scope.removeOverride = function (claimEligibleCost) {
    claimEligibleCost.OverrideRate = claimEligibleCost.ReimbursementRate;
    copyValue(claimEligibleCost, 'ClaimParticipants', 'AssessedParticipants');
    copyValue(claimEligibleCost, 'ClaimCost', 'AssessedCost');
    copyValue(claimEligibleCost, 'ClaimMaxParticipantReimbursementCost', 'AssessedMaxParticipantReimbursementCost');

    $scope.recalculateTrainingCost(claimEligibleCost, true);
  }

  /**
   * Make AJAX request for delete claim eligible cost
   * @function deleteClaimEligibleCost
   * @param {object} eligibleCost - claim eligible cost.
   * @returns {Promise}
   **/
  $scope.deleteClaimEligibleCost = function (eligibleCost) {
    return $scope.confirmDialog('Delete Eligible Cost', 'Do you want to delete "' + eligibleCost.Caption + '"?')
      .then(function () {
        $scope.EligibleCostSummaryMessage = '';
        var i = $scope.model.EligibleCosts.indexOf(eligibleCost);
        $scope.model.EligibleCosts.splice(i, 1);
        calculateGrantTotal();
      })
      .catch(angular.noop);
  }

  /**
   * Add the selected expense type to the claim.
   * @function addExpenseType
   * @param {int} selectedExpenseType - expense type to add.
   * @returns {void}
   **/
  $scope.addExpenseType = function (selectedExpenseType) {
    var rate = selectedExpenseType.Rate || $scope.claim.ReimbursementRate;
    var maxParticipantCost = MathFunction.truncate($scope.model.AgreedMaxCost / $scope.model.MaximumParticipants * 100);
    var eligibleCost = {
      Id: '_' + $scope.model.EligibleCosts.length,
      EligibleExpenseTypeId: selectedExpenseType.Id,
      Caption: selectedExpenseType.Caption,
      Description: selectedExpenseType.Description,
      ReimbursementRate: rate,
      OverrideRate: rate,
      ExpenseType: selectedExpenseType.ExpenseType,
      ServiceType: selectedExpenseType.ServiceType,
      AddedByAssessor: true,
      ParticipantCosts: selectedExpenseType.ExpenseType == utils.ExpenseTypes.ParticipantAssigned ? $scope.claim.Participants.map(function (participant) {
        return {
          Id: '_' + $scope.model.EligibleCosts.length + '_' + participant.ParticipantFormId,
          Participant: {
            ParticipantFormId: participant.ParticipantFormId,
            Name: participant.Name,
            Email: participant.Email,
            PhoneNumber: participant.PhoneNumber
          }
        };
      }) : [],
      Breakdowns: [],
      TotalAssessedParticipantReimbursement: 0,
      AgreedMaxParticipants: $scope.model.MaximumParticipants,
      AgreedMaxCost: $scope.model.AgreedMaxCost,
      AgreedMaxReimbursement: $scope.model.AgreedMaxCommittment,
      AgreedMaxParticipantCost: maxParticipantCost,
      AgreedMaxParticipantReimbursement: MathFunction.truncate(maxParticipantCost * rate * 100) / 100
    };

    $scope.EligibleCostSummaryMessage = '';
    $scope.model.EligibleCosts.push(eligibleCost);
    $scope.toggle(eligibleCost);
  }
});
