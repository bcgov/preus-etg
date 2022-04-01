app.controller('ParticipantReportingView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'ParticipantReportingView',
    grantApplicationId: $attrs.ngGrantApplicationId,
    showInvitation: false,
    includeAll: false,
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
   * Make AJAX request to load participant reporting data.
   * @function loadParticipants
   * @returns {Promise}
   **/
  function loadParticipants() {
    return $scope.load({
      url: '/Ext/Reporting/Participants/' + $scope.section.grantApplicationId,
      set: 'model'
    })
      .then(function () {
        return $timeout(function () {
          $scope.section.includeAll = $scope.model.Participants.every(function (participant) {
            return participant.IsIncludedInClaim;
          });
        });
      });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadParticipants()
    ])
      .catch(angular.noop);
  }

  /**
   * Make an AJAX request to include/exclude the participant from the current claim.
   * @function toggleParticipants
   * @param {array} participantFormIds - Any array of ids.
   * @param {bool} include - Whether to include or exclude the participants.
   * @returns {Promise}
   */
  function toggleParticipants(participantFormIds, include) {
    return $scope.load({
      url: '/Ext/Reporting/Participant/Toggle',
      method: 'PUT',
      data: function () {
        return {
          GrantApplicationId: $scope.model.GrantApplicationId,
          ClaimRowVersion: $scope.model.ClaimRowVersion,
          Include: include || false,
          ParticipantFormIds: participantFormIds
        }
      },
      set: 'model'
    })
      .catch(function () {
        return $timeout(function () {
          // Undo any checkbox changes if there is a failure.
          $scope.model.Participants.map(function (participant) {
            if (participantFormIds.some(function (id) { return id === participant.Id; })) {
              participant.IsIncludedInClaim = !participant.IsIncludedInClaim;
            }
          });

          $scope.section.includeAll = $scope.model.Participants.every(function (participant) {
            return participant.IsIncludedInClaim;
          });
        });
      });
  }

  /**
   * Auto include all or exclude all participants from the current claim.
   * @function includeAll
   * @returns {Promise}
   **/
  $scope.includeAll = function () {
    var participantFormIds = [];

    $scope.model.Participants.map(function (participant) {
      if (participant.ClaimReported) {
        participantFormIds.push(participant.Id);
      }
    });

    return toggleParticipants(participantFormIds, $scope.section.includeAll);
  }

  /**
   * Include or exclude the specified participant from the current claim.
   * @function toggleParticipant
   * @param {object} participant - The participant.
   * @returns {Promise}
   */
  $scope.toggleParticipant = function (participant) {
    if (!participant.IsIncludedInClaim) {
      $scope.section.includeAll = false;
    }
    return toggleParticipants([ participant.Id ], participant.IsIncludedInClaim);
  }

  /**
   * Shows confirmation prompt and deletes participant form.
   * @function removeParticipant
   * @param {object} participant - The participant to remove.
   * @returns {Promise}
   */
  $scope.removeParticipant = function (participant) {
    return $scope.confirmDialog('Remove Participant', '<p>Remove ' + participant.FirstName + ' ' + participant.LastName + ' from this application?</p>  <p>Removing a participant deletes their information. You will only be reimbursed for participants who completed training and submit their PIF.</p>')
      .then(function () {
        return $scope.load({
          url: '/Ext/Reporting/Participant/Delete',
          method: 'PUT',
          data: participant,
          set: 'model'
        });
      })
      .catch(angular.noop);
  }

  $scope.setExpectedTrainingOutcome = function (participant, outcome, oldValue) {
    let selectedOutcome = $scope.model.ExpectedOutcomes.filter(a => a.Key === participant.ExpectedOutcome).pop().Value;
    return $scope.confirmDialog('Set Training Outcome', '<p>Set training outcome for ' + participant.FirstName + ' ' + participant.LastName + ' to <strong>' + selectedOutcome + '</strong>?</p>')
      .then(function () {
        return $scope.load({
          url: '/Ext/Reporting/Participant/SetOutcome',
          method: 'PUT',
          data: participant,
          set: 'model'
        });
      })
      .catch(function () {
        participant.ExpectedOutcome = parseInt(oldValue);
      });
  }

  init();
});
