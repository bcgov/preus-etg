app.controller('PaymentRequestManagement', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'PaymentRequestManagement'
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
   * Make AJAX request for grant programs data
   * @function loadGrantPrograms
   * @returns {Promise}
   **/
  function loadGrantPrograms() {
    return $scope.load({
      url: '/Int/Payment/Request/Programs',
      set: 'grantPrograms'
    });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadGrantPrograms()
    ]);
  }

  /**
   * Make AJAX request to refresh data
   * @function refresh
   * @returns {Promise}
   **/
  $scope.refresh = function () {
    if ($scope.selectGrantProgramId) {
      $scope.broadcast('refreshPager');
      if ($scope.paymentRequest)
        return $scope.getPaymentRequestClaims();
      if ($scope.amountOwing)
        return $scope.getAmountOwingClaims();
      if ($scope.paymentRequestHold)
        return $scope.getPaymentRequestOnHoldClaims();
    }
    return Promise.resolve();
  };

  /**
   * Get the filtered payment request batches.
   * @function getPaymentRequestBatches
   * @param {string} pageKeyword - The search filter keyword.
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items in a page.
   * @returns {Promise}
   **/
  $scope.getPaymentRequestBatches = function (pageKeyword, page, quantity) {
    return $scope.ajax({
      url: '/Int/Payment/Request/Batch/Search/' + $scope.selectGrantProgramId + '/' + page + '/' + quantity + (pageKeyword ? '?search=' + pageKeyword : '')
    })
      .then(function (response) {
        return Promise.resolve(response.data);
      })
      .catch(angular.noop);
  };

  /**
   * The Request Batch Modal
   * @function showRequestBatch
   * @param {int} requestBatchId - The Request Batch Id.
   * @returns {Promise}
   **/
  $scope.showRequestBatch = function (requestBatchId) {
    return ngDialog.openConfirm({
      template: '/Int/Payment/Request/Batch/' + requestBatchId
    });
  };
  
  /**
   * Print the payment request batch
   * @function printRequestBatch
   * @param {string} url - The payment request batch url.
   * @param {string} title - The payment request batch title.
   * @returns {void}
   **/
  $scope.printRequestBatch = function (url, title) {
    window.open(url, title, 'directories=0,titlebar=0,toolbar=0,location=0,status=0,menubar=0');
  };

  /**
   * All Requests Reconciled
   * @function allRequestsReconciled
   * @returns {void}
   **/
  $scope.allRequestsReconciled = function () {
    ngDialog.openConfirm({
      template: '/content/dialogs/_Message.html',
      data: {
        title: 'All Requests Reconciled',
        message: 'All requests in the batch have been reconciled; no duplicate requests can be generated.'
      }
    });
  };

  /**
   * Make AJAX request to get the payment request claims.
   * @function getPaymentRequestClaims
   * @returns {Promise} promise
   */
  $scope.getPaymentRequestClaims = function () {
    return $scope.load({
      url: '/Int/Payment/Request/Payment/Request/' + $scope.selectGrantProgramId,
      set: 'paymentRequest'
    })
      .then(function () {
        return $timeout(function () {
          $scope.amountOwing = null;
          $scope.paymentRequestHold = null;
        });
      })
      .catch(angular.noop);
  };

  /**
   * Make AJAX request to get the amount owing claims.
   * @function getAmountOwingClaims
   * @returns {Promise} promise
   */
  $scope.getAmountOwingClaims = function () {
    return $scope.load({
      url: '/Int/Payment/Request/Amount/Owing/' + $scope.selectGrantProgramId,
      set: 'amountOwing'
    })
      .then(function () {
        return $timeout(function () {
          $scope.paymentRequest = null;
          $scope.paymentRequestHold = null;
        });
      })
      .catch(angular.noop);
  };

  /**
   * Make AJAX request to get the payment request on hold claims.
   * @function getPaymentRequestOnHoldClaims
   * @returns {Promise} promise
   */
  $scope.getPaymentRequestOnHoldClaims = function () {
    return $scope.load({
      url: '/Int/Payment/Request/On/Hold/' + $scope.selectGrantProgramId,
      set: 'paymentRequestHold'
    })
      .then(function () {
        return $timeout(function () {
          $scope.paymentRequest = null;
          $scope.amountOwing = null;
        });
      })
      .catch(angular.noop);
  };

  /**
   * The Payment Request Batch Modal
   * @function generatePaymentRequest
   * @param {int} requestBatchId - The Request Batch Id.
   * @returns {Promise}
   **/
  $scope.generatePaymentRequest = function () {
    return $scope.ajax({
      url: '/Int/Payment/Request/Generate/Payment/Request/' + $scope.selectGrantProgramId,
      method: 'POST'
    })
      .then(function (response) {
        if (response.data.result === 'empty') {
          ngDialog.openConfirm({
            template: '/content/dialogs/_Message.html',
            data: {
              title: 'Payment Request',
              message: 'No Claims are ready for Payment Request generation, another user may have initiated generation. Please refresh the list.'
            }
          });
        }
        else if (response.data.result === 'success') {
          ngDialog.openConfirm({
            template: '/content/dialogs/_Confirmation.html',
            data: {
              title: 'Payment Request Batch',
              question: 'The Payment Request Batch (#' + response.data.number + ') has been generated successfully, please click [OK] to print. Payment Requests are generated for a LANDSCAPE page orientation. Make sure you set your page orientation correctly.',
              cancel: 'Cancel',
              confirm: 'OK'
            }
          })
            .then(function () {
              return $timeout(function () {
                $scope.printRequestBatch('/Int/Payment/Request/Print/Payment/Request/Batch/' + response.data.id, 'Payment Request Batch');
              });
            })
            .catch(angular.noop);
        }
        return $scope.refresh();
      })
      .catch(angular.noop);
  };

  /**
   * The Amount Owing Batch Modal
   * @function generateAmountOwing
   * @param {int} requestBatchId - The Request Batch Id.
   * @returns {Promise}
   **/
  $scope.generateAmountOwing = function () {
    return $scope.ajax({
      url: '/Int/Payment/Request/Generate/Amount/Owing/' + $scope.selectGrantProgramId,
      method: 'POST'
    })
      .then(function (response) {
        if (response.data.result === 'empty') {
          ngDialog.openConfirm({
            template: '/content/dialogs/_Message.html',
            data: {
              title: 'Amount Owing',
              message: 'No Claims are ready for Amount Owing generation, another user may have initiated generation. Please refresh the list.'
            }
          });
        }
        else if (response.data.result === 'success') {
          ngDialog.openConfirm({
            template: '/content/dialogs/_Confirmation.html',
            data: {
              title: 'Amount Owing Batch',
              question: 'The Amount Owing Batch (#' + response.data.number + ') has been generated successfully, please click [OK] to print. Amount Owing requests are generated for a PORTRAIT page orientation. Make sure you set your page orientation correctly.',
              cancel: 'Cancel',
              confirm: 'OK'
            }
          })
            .then(function () {
              return $timeout(function () {
                $scope.printRequestBatch('/Int/Payment/Request/Print/Amount/Owing/Batch/' + response.data.id, 'Amount Owing Batch');
              });
            })
            .catch(angular.noop);
        }
        return $scope.refresh();
      })
      .catch(angular.noop);
  };

  init();
});
