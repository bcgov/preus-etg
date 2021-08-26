app.controller('Claims', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'Claims',
    displayName: 'Claims',
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.grantFile.RowVersion && $scope.model.ClaimViewModels;
    },
    onRefresh: function () {
      return loadClaims().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for claim data
   * @function loadClaims
   * @returns {Promise}
   **/
  function loadClaims() {
    return $scope.load({
      url: '/Int/Application/Claims/' + $scope.parent.grantApplicationId,
      set: 'model'
    });
  }
    /**
   * Initialize section data.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadClaims()
    ]).catch(angular.noop);
  }

  /**
   * Make AJAX request and enable/disable the payment requests.
   * @function togglePaymentRequests
   * @param {any} $event - Angular event.
   * @returns {void}
   */
  $scope.togglePaymentRequests = function ($event) {
    if (!$scope.model.HoldPaymentRequests) {
      confirmation();
    } else {
      updatePaymentRequests();
    }
  }

  /**
   * Make AJAX request to update the application to enable or disable payment requests
   * @function updatePaymentRequests
   * @returns {Promise}
   **/
  function updatePaymentRequests() {
    if (!$scope.model.HoldPaymentRequests) {
      $scope.model.HoldPaymentRequests = $scope.grantFile.HoldPaymentRequests;
    }
    return $scope.ajax({
      url: '/Int/Application/Claim/Hold/Payment/Requests/' + $scope.parent.grantApplicationId + '?rowVersion=' + encodeURIComponent($scope.grantFile.RowVersion) + '&hold=' + !$scope.model.HoldPaymentRequests,
      method: 'PUT'
    })
      .then(function (response) {
        $scope.resyncApplicationDetails();
        $scope.emit('refresh', { force: true });
        return $timeout(function () {
          $scope.model.HoldPaymentRequests = response.data.HoldPaymentRequests;
          $scope.model.RowVersion = response.data.RowVersion;
        });
      })
      .catch(angular.noop);
  }

  /**
   * Show confirmation popup dialog
   * @function confirmation
   * @returns {void}
   **/
  function confirmation() {
    $scope.confirmDialog('Hold Payment Requests', 'This will stop payment requests from being issued to pay approved claims until you enable payment requests again.  Are you sure you want to hold payment requests for this grant file?')
      .then(function () {
        return updatePaymentRequests();
      })
      .catch(angular.noop);
  }

  /**
   * Change the state of the selected claim to selected for assessment.
   * Make AJAX request to update claim.
   * @function selectForAssessment
   * @param {any} claim - The claim to update.
   * @returns {Promise}
   */
  $scope.selectForAssessment = function (claim) {
    return $scope.ajax({
      url: '/Int/Workflow/Select/Claim/For/Assessment',
      method: 'PUT',
      data: {
        ApplicationWorkflowViewModel: {},
        ClaimWorkflowViewModel: claim
      }
    })
      .then(function () {
        return $scope.resyncApplicationDetails()
          .then(function () {
            $scope.emit('refresh', { force: true });
            return loadClaims();
          });
      })
      .catch(angular.noop);
  }
});
