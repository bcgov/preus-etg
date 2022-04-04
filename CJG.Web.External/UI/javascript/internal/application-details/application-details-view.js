var utils = require('../../shared/utils.js');
require('../training-provider');
require('./application-details-summary');
require('./application-details-applicant-contact');
require('./application-details-applicant');
require('./application-details-program-description');
require('./application-details-attachments');
require('./application-details-training-provider');
require('./application-details-training-program');
require('./application-details-skills-training');
require('./application-details-skills-training-program');
require('./application-details-skills-training-provider');
require('./application-details-employment-provider');
require('./application-details-employment-services');
require('./application-details-program-costs');
require('./application-details-participants');
require('./application-details-claims');
require('./application-details-completion-report');
require('./application-details-notification');
require('./application-details-notifications');

app.filter('unsafe', function ($sce) { return $sce.trustAsHtml; });

app.controller('ApplicationDetails', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ApplicationDetails',
    onRefresh: function () {
      return loadApplicationDetails().catch(angular.noop);
    }
  };

  $scope.denialReasonsSelection = [];

  $scope.assessors = [];

  $scope.parent = {
    grantApplicationId: $attrs.grantApplicationId,
    editing: null
  };

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));


  /**
   * Make AJAX request for assessors data
   * @function loadAssessors
   * @returns {Promise}
   **/
  function loadAssessors() {
    return $scope.load({
      url: '/Int/Application/Assessors/' + $scope.parent.grantApplicationId,
      set: 'assessors',
      condition: !$scope.assessors || !$scope.assessors.length
    });
  }

  /**
   * Make AJAX request to load application details data.
   * @function loadApplicationDetails
   * @param {bool} [set=true] - Whether to set a scope level variable.
   * @returns {Promise}
   **/
  function loadApplicationDetails(set) {
    if (typeof (set) === 'undefined') set = true;
    return $scope.load({
      url: '/Int/Application/Details/' + $attrs.grantApplicationId,
      set: set ? 'grantFile' : null
    });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadAssessors(),
      loadApplicationDetails()
    ])
      .then(function () {
        $scope.broadcast('show', { target: 'ApplicationSummary,ApplicationNotes' });
      })
      .catch(angular.noop);
  }

  /**
   * Checks to see if training providers still require validation.
   * @function requiresTrainingProviderValidation
   * @returns {boolean}
   **/
  $scope.requiresTrainingProviderValidation = function () {
    var requiresValidation = $scope.grantFile.Components.some(function (component) {
      // If any programs have providers that are not validated.
      if (Array.isArray(component.Programs) && component.Programs.length
        && component.Programs.some(function (program) {
          return program.Provider && !program.Provider.IsValidated;
        })) return true;

      // If any providers are not validated.
      if (Array.isArray(component.Providers) && component.Providers.length
        && component.Providers.some(function (provider) {
          return !provider.IsValidated;
        })) return true;

      return false;
    });

    $scope.grantFile.RequiresTrainingProviderValidation = requiresValidation;
    return requiresValidation;
  }

  /**
   * Resync the application details view data.
   * Make AJAX request for new data.
   * Update model with only the parts that changed.
   * @function resyncApplicaiontDetails
   * @returns {Promise}
   **/
  function resyncApplicationDetails() {
    return loadApplicationDetails(false)
      .then(function (response) {
        return $scope.sync(response.data, $scope.grantFile);
      })
      .catch(angular.noop);
  }
  $scope.resyncApplicationDetails = resyncApplicationDetails;

  /**
   * If a section is currently being edited it will prompt the user to either finish or cancel the edit.
   * @function confirmAction
   * @returns {Promise}
   **/
  function confirmAction() {
    if ($scope.parent.editing) {
      return new Promise(function (resolve, reject) {
        $scope.confirmCancel('Unsaved Changes', 'Finish editing before attempting to perform a workflow action. Clicking "Cancel" will undo any edits you have currently made.')
          .then(function () {
            reject();
          })
          .catch(function () {
            $scope.parent.cancel();
            resolve();
          });
      });
    }
    return Promise.resolve();
  }

  /**
   * Open a confirmation dialog to ask for the reason for the specified trigger.
   * @function getReason
   * @param {any} url - The URL for the trigger action.
   * @param {any} trigger - The tigger being performed.
   * @returns {Promise}
   */
  function getReason(url, trigger) {
    var title = '';
    var text = '';
    var denialReasonList = '';
    var template = '/content/dialogs/_Reason.html';
    switch (trigger) {
      case ('RecommendForDenial'):
        title = 'Deny Application';
        text = 'Your application has been denied for the following reason:';
        template = '/content/dialogs/_DenialReason.html';
        denialReasonList = $scope.grantFile.GrantProgramDenialReasons;
        break;
      case ('WithdrawOffer'):
        title = 'Withdraw Offer';
        text = 'Your application offer has been withdrawn for the following reason:';
        break;
      case ('CancelAgreementMinistry'):
        title = 'Cancel Agreement';
        text = 'Your agreement has been cancelled for the following reason:';
        break;
      case ('RecommendChangeForDenial'):
        title = 'Recommend To Deny Change Request';
        text = 'Your change request has been denied for the following reason:';
        break;
      case ('ReturnToAssessment'):
        title = 'Return To Assessment';
        template = '/content/dialogs/_ReturnToAssessmentReason.html';
        break;
    }
    return ngDialog.openConfirm({
      template: template,
      data: {
        title: title,
        trigger: title,
        text: text,
        tinymceOptions: {
          plugins: 'link code autoresize preview fullscreen lists advlist anchor',
          toolbar: 'undo redo | bold italic | formatselect | alignleft aligncenter alignright | outdent indent | numlist bullist | anchor | preview | fullscreen | code ',
          forced_root_blocks: true
        },
        denialReasonList: denialReasonList,
        denialReasonsSelection: $scope.denialReasonsSelection,
        denialEditorHasContent: function () {
          var content = tinymce.activeEditor.getContent({ format: "text" });
          return content != null && content.length > 0;
        },
        toggleDenialSelection: function (reason) {
          var idx = $scope.denialReasonsSelection.indexOf(reason);
          // Is currently selected
          if (idx > -1) {
            $scope.denialReasonsSelection.splice(idx, 1);
          }
          else {
            $scope.denialReasonsSelection.push(reason);
          }
        },
        getDenialReasonsSelection: function (selection, reason) {
          var deniedReason = tinymce.activeEditor.getContent();

          return JSON.stringify({
            selectedReasons: selection,
            deniedReason: deniedReason
          });
        },

      }
    }).then(function (reason) {
      return doWorkflowTrigger(url, reason);
    });
  }

  /**
   * Make AJAX request to update the application.
   * @function doWorkflowTrigger
   * @param {any} url - The workflow URL.
   * @param {any} reason - The reason for the action.
   * @returns {Promise}
   */
  function doWorkflowTrigger(url, reason) {
    return $scope.ajax({
      url: url,
      method: 'PUT',
      data: {
        ApplicationWorkflowViewModel: {
          Id: $scope.grantFile.Id,
          RowVersion: $scope.grantFile.RowVersion,
          AssessorId: $scope.grantFile.AssessorId || $scope.grantFile.WorkflowViewModel.ApplicationWorkflowViewModel.AssessorId,
          ReasonToDeny: reason,
          ReasonToCancel: reason,
          ReasonToWithdraw: reason,
          ReasonToDenyChangeRequest: reason,
          ReasonToReassess: reason
        }
      }
    });
  }

  $scope.tinymceOptions = {
    plugins: 'link code autoresize preview fullscreen lists advlist anchor',
    toolbar: 'undo redo | bold italic | formatselect | alignleft aligncenter alignright | outdent indent | numlist bullist | anchor | preview | fullscreen | code ',
    forced_root_blocks: true,
    setup: function (ed) {
      ed.on('init', function (ed) {
        $('div.tox-tinymce-aux').css('z-index', '999999');
      });
    }
  };

  $(document).on('focusin', function (e) {
    if ($(e.target).closest(".mce-window").length)
      e.stopImmediatePropagation();
  });

  /**
   * An application workflow action has been triggered.
   * @function handleWorkflow
   * @param {any} url - The workflow URL.
   * @param {any} trigger - The workflow trigger action.
   * @returns {Promise}
   */
  $scope.handleWorkflow = function (url, trigger) {
    return confirmAction()
      .then(function () {
        switch (trigger) {
          case ('RecommendForDenial'):
          case ('WithdrawOffer'):
          case ('CancelAgreementMinistry'):
          case ('RecommendChangeForDenial'):
            return getReason(url, trigger);
          case ('ReturnToAssessment'):
            if ($scope.grantFile.WorkflowViewModel.ApplicationWorkflowViewModel.ApplicationStateInternal === utils.ApplicationStateInternal.ApplicationDenied)
              return $scope.confirmDialog('Warning', 'You have indicated that you want to re-assess this previously denied application.\nDo you want to continue?').then(function () {
                return getReason(url, trigger);
              });
            return doWorkflowTrigger(url);
          case ('ReturnUnassessed'):
            return $scope.confirmDialog('Warning', '<p>Do you want to return this application without being assessed?</p>' + '<p>This application will be removed from the queue and applicant will receive the "Returned to Applicant Unassessed" notification. \n Application details will remain in the system but will not affect the budget.</p>').then(function () {
              return doWorkflowTrigger(url);
            });
          case ('ReturnUnderAssessmentToDraft'):
          case ('ReturnOfferToAssessment'):
          case ('ReturnWithdrawnOfferToAssessment'):
          case ('ReturnUnassessedToNew'):
          case ('ReverseAgreementCancelledByMinistry'):
            return $scope.confirmDialog('Warning', 'Are you sure you want to reverse this decision?').then(function () {
              return doWorkflowTrigger(url);
            });
          default:
            return doWorkflowTrigger(url);
        }
      })
      .then(function () {
        return resyncApplicationDetails();
      })
      .then(function () {
        $scope.broadcast('refresh', { force: true });
      })
      .catch(angular.noop);
  }

  init();
});
