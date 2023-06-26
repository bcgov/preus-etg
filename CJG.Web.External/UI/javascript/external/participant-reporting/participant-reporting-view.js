app.controller('ParticipantReportingView', function($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

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
      .then(function() {
        return $timeout(function() {
          $scope.section.includeAll = $scope.model.Participants.every(function(participant) {
            return participant.IsIncludedInClaim;
          });
        });
      });
  }

  $scope.showParticipantDialog = function(invitation) {
    return ngDialog.openConfirm({
      template: '/content/dialogs/_AddParticipantInvitation.html',
      data: {
        title: 'Add Participant Invitation',
        model: invitation
      },
      controller: function($scope) {

        $scope.cancel = function() {
            $scope.ngDialogData.model.FirstName = '';
            $scope.ngDialogData.model.LastName = '';
            $scope.ngDialogData.model.EmailAddress = '';
            $scope.ngDialogData.model.ExpectedParticipantOutcome = 0;
            Utils.clearErrors($scope.ngDialogData.model);

            $scope.closeThisDialog();
          },
          $scope.save = function() {
            Utils.clearErrors($scope.ngDialogData.model);

            if ($scope.ngDialogData.model.FirstName.trim() === '')
              Utils.initValue($scope.ngDialogData.model, 'errors.FirstName', 'First name is required');

            if ($scope.ngDialogData.model.LastName.trim() === '')
              Utils.initValue($scope.ngDialogData.model, 'errors.LastName', 'Last name is required');

            let emailAddress = $scope.ngDialogData.model.EmailAddress.trim();
            if (emailAddress === '')
              Utils.initValue($scope.ngDialogData.model, 'errors.EmailAddress', 'Email address is required');

            if (emailAddress !== '' && !isValidEmail(emailAddress))
              Utils.initValue($scope.ngDialogData.model, 'errors.EmailAddress', 'Email address is not valid');

            if ($scope.ngDialogData.model.ExpectedParticipantOutcome === 0)
              Utils.initValue($scope.ngDialogData.model, 'errors.ExpectedParticipantOutcome', 'Expected participant outcome is required');

            if (JSON.stringify($scope.ngDialogData.model.errors) !== '{}')
              return angular.noop;

            return $scope.confirm($scope.ngDialogData.model);
          };
      }
    }).then(function(model) {
      return addParticipantInvite(model);
    });
  }

  function isValidEmail(email) {
    return (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(email));
  }

  function addParticipantInvite(model) {
    return $scope.load({
      url: '/Ext/Reporting/Participant/AddInvitation',
      method: 'PUT',
      
      data: {
        model: model
      },
      set: 'model'
    }).then(function (response) {
      loadParticipants();
    }).catch(angular.noop);
  }

  $scope.sendParticipantInvite = function(model) {
    return $scope.load({
      url: '/Ext/Reporting/Participant/SendInvitation',
      method: 'PUT',
      data: {
        model: model
      },
      set: 'model'
    }).then(function (response) {
      loadParticipants();
    }).catch(angular.noop);
  }

  $scope.removeParticipantInvite = function(model) {
    return $scope.load({
      url: '/Ext/Reporting/Participant/RemoveInvitation',
      method: 'PUT',
      data: {
        model: model
      },
      set: 'model'
    }).then(function (response) {
      loadParticipants();
    }).catch(angular.noop);
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

  $scope.participantOutcomesReported = function () {
    if ($scope.model.Participants === null)
      return true;

    return $scope.model.Participants.filter(p => p.ExpectedOutcome === 0).length === 0;
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
