require('./claim-assessment-attachments');
require('./claim-assessment-details');

app.controller('ClaimAssessmentView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ClaimAssessmentView',
    displayName: 'Claim Assessment',
    notesUpdated: false
};
  $scope.parent = {
    grantApplicationId: $attrs.grantApplicationId,
    claimId: $attrs.claimId,
    claimVersion: $attrs.claimVersion
  };

  if (typeof ($scope.assessors) === 'undefined')
    $scope.assessors = [];

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

   /**
   * Fetch all the data for the claim.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadAssessors(),
      loadExpenseTypes(),
      loadClaim()
    ]).then(function () {
      $scope.broadcast('show', { target: 'ApplicationNotes' });
    })
    .catch(angular.noop);;
  }

  /**
   * Make AJAX request for claim
   * @function loadClaim
   * @param {bool} [set=true] - Whether to set a scope level variable.
   * @returns {Promise}
   **/
  function loadClaim(set) {
    if (typeof (set) === 'undefined') set = true;
    return $scope.load({
      url: '/Int/Claim/Assessment/' + $scope.parent.claimId + '/' + $scope.parent.claimVersion,
      set: set ? 'claim' : null
    }).catch(angular.noop);
  }

  /**
   * Make AJAX request for assessors data
   * @function loadAssessors
   * @returns {Promise}
   **/
  function loadAssessors() {
    return $scope.load({
      url: '/Int/Claim/Assessors',
      set: 'assessors',
      condition: !$scope.assessors || !$scope.assessors.length
    });
  }

  /**
   * Make AJAX request for expense types
   * @function loadExpenseTypes
   * @returns {Promise}
   **/
  function loadExpenseTypes() {
    return $scope.load({
      url: '/Int/Claim/Expense/Types/' + $scope.parent.grantApplicationId,
      set: 'expenseTypes',
      condition: !$scope.expenseTypes || !$scope.expenseTypes.length
    })
  }

  /**
   * Resync the claim details view data.
   * Make AJAX request for new data.
   * Update model with only the parts that changed.
   * @function resyncApplicaiontDetails
   * @returns {Promise}
   **/
  function resyncClaimDetails() {
    $scope.broadcast('refresh', { force: true });
    return loadClaim(false)
      .then(function (response) {
        return $scope.sync(response.data, $scope.claim);
      })
      .catch(angular.noop);
  }
  $scope.resyncClaimDetails = resyncClaimDetails;

  /**
   * If a section is currently being edited it will prompt the user to either finish or cancel the edit.
   * @function confirmAction
   * @returns {Promise}
   **/
  function confirmAction() {
    if ($scope.parent.editing) {
      return ngDialog.openConfirm({
        template: '/content/dialogs/_FinishEditing.html',
        data: {
          title: 'Unsaved Changes',
          question: 'Finish editing before attempting to perform a workflow action. Clicking "Cancel" will undo any edits you have currently made.'
        }
      }).catch(function (data) {
        $scope.parent.cancel(event, data);
        return Promise.reject();
      });
    }
    return Promise.resolve();
  }

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
        return $scope.ajax({
          url: url,
          method: 'PUT',
          data: {
            ClaimWorkflowViewModel: {
              Id: $scope.claim.Id,
              ClaimVersion: $scope.claim.Version,
              RowVersion: $scope.claim.RowVersion
            }
          }
        });
      }).then(function () {
        return $scope.resyncClaimDetails();
      }).catch(angular.noop);
  }

  /**
   * Make AJAX request for reassign
   * @function reassign
   * @returns {Promise}
   **/
  $scope.reassign = function () {
    return confirmAction()
      .then(function () {
        return $scope.load({
          url: "/Int/Claim/Reassign",
          method: 'PUT',
          set: 'claim',
          data: $scope.claim
        });
      }).then(function () {
        $scope.broadcast('refresh', { force: true });
      }).catch (angular.noop);
  }

  /**
   * Make AJAX request for save note
   * @function saveNotes
   * @returns {Promise}
   **/
  $scope.saveNotes = function () {
    return $scope.load({
      url: '/Int/Claim/Notes',
      method: 'PUT',
      set: 'claim',
      data: $scope.claim
    }).then(function () {
      $scope.broadcast('refresh', { force: true });
      $scope.section.notesUpdated = false;
      $scope.$broadcast('notesUpdated', {
        updated: false
      });

      return $timeout(function () {
        $scope.alert.message = "Claim has has been updated successfully.";
        $scope.alert.type = 'success';
      });
    });
  }

  $scope.flagNotesUpdated = function() {
    $scope.section.notesUpdated = true;
    $scope.$broadcast('notesUpdated', {
      updated: true
    });
  }

  init();
});
