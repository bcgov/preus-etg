app.controller('PaymentReconciliation', function ($scope, $controller, $timeout, Utils, ngDialog) {
  $scope.filter = {
    orderBy: 'DateAdded',
    page: 1,
    quantity: 10,
    search: null,
    activePayment: $scope.ngDialogData.selectedPayment || $scope.ngDialogData.model
  };
  $scope.model = {};
  $scope.current = Object.assign({}, $scope.filter);
  $scope.cache = [];
  $scope.selectedPaymentRequestIndex = null;
  $scope.action = "Reconcile";
  $scope.allowReconcile = false;

  $scope.ngDialogData.selectedPayment = $scope.ngDialogData.model.Payments ? null : $scope.ngDialogData.model;

  $scope.$watch('ngDialogData.selectedPayment', function (newValue, oldValue) {
    $scope.filter.activePayment = $scope.ngDialogData.selectedPayment || $scope.ngDialogData.model;
    $scope.selectedPaymentRequestIndex = getSelectedPaymentRequestRadioValue();
  });

  angular.extend(this, $controller('Base', { $scope: $scope }));

  /**
   * Make AJAX request to load payment requests.
   * @function loadPaymentRequests
   * @returns {Promise}
   **/
  function loadPaymentRequests() {
    return $scope.load({
      url: '/Int/Payment/Reconciliation/Payment/Requests?reconciliationPaymentId=' + $scope.filter.activePayment.Id + '&page=' + $scope.filter.page + '&quantity=' + $scope.filter.quantity + '&sort=' + $scope.filter.orderBy + '&search=' + ($scope.filter.search || ''),
      set: 'model',
      condition: $scope.cache.length < $scope.filter.page || !$scope.cache[$scope.filter.page - 1] || filterChanged() // The the quantity size changes, or the page isn't cached yet.
    })
      .then(function () {
        return $timeout(function () {
          setupPage($scope.filter.page, $scope.filter.quantity);
          $scope.selectedPaymentRequestIndex = getSelectedPaymentRequestRadioValue();
          $scope.action = getAction();
        });
      });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    $scope.filter.search = $scope.ngDialogData.model.DocumentNumber;
    return Promise.all([
      loadPaymentRequests()
    ]).catch(angular.noop);
  }

  /**
   * Get the sorting order of the specified property.
   * @function sortDirection
   * @param {any} propertyName - The property name to order by.
   * @returns {string}
   */
  $scope.sortDirection = function (propertyName) {
    if (!isOrderedBy(propertyName)) return 'sorting';
    return isAscending(propertyName) ? 'sorting_asc' : 'sorting_desc';
  }

  /**
   * Check if the filter is currently ordered by the specified property name.
   * @function isOrderedBy
   * @param {string} propertyName - The property name to order by.
   * @returns {bool}
   */
  function isOrderedBy(propertyName) {
    return $scope.filter.orderBy.startsWith(propertyName) ? true : false;
  }

  /**
   * Check if the filter is currently be ordered in ascending order by the specified property name.
   * Use this to determine if the order by is ascending or descending.
   * @function isAscending
   * @param {string} propertyName - The property name to order by.
   * @returns {bool}
   */
  function isAscending(propertyName) {
    var found = isOrderedBy(propertyName);
    if (!found) return true;
    return $scope.filter.orderBy.endsWith('desc') ? false : true;
  }

  /**
   * Order the applications by the specified property name.
   * @function sort
   * @param {string} propertyName - The property name to order by.
   * @returns {Promise}
   */
  $scope.sort = function (propertyName) {
    $scope.filter.orderBy = !isOrderedBy(propertyName) || !isAscending(propertyName) ? propertyName : propertyName + ' desc';
    return $scope.applyFilter();
  }

  /**
   * Refresh the page of payment requests with the specified sort.
   * @function applyFilter
   * @param {int} [page] - The page number.
   * @param {int} [quantity] - The number of items per page.
   * @param {bool} [force=false] - Whether to force a refresh.
   * @returns {Promise}
   */
  $scope.applyFilter = function (page, quantity, force) {
    if (!page) page = $scope.filter.page;
    if (!quantity) quantity = $scope.filter.quantity;
    if (typeof (force) === 'undefined') force = false;

    $scope.filter.page = page;
    $scope.filter.quantity = quantity;

    if (force || filterChanged()) $scope.cache = [];

    return loadPaymentRequests()
      .then(function () {
        $scope.current = {
          page: $scope.filter.page,
          quantity: $scope.filter.quantity,
          orderBy: $scope.filter.orderBy,
          search: $scope.filter.search
        }
      }).catch(angular.noop);
  }

  /**
   * Check if the filter has been changed.
   * @function filterChanged
   * @returns {bool}
   **/
  function filterChanged() {
    if ($scope.filter.page !== $scope.current.Page
      || $scope.filter.quantity !== $scope.current.quantity
      || $scope.filter.orderBy !== $scope.current.orderBy
      || $scope.filter.search !== $scope.current.search) return true;

    return false;
  }

  /**
   * Update the page values with page numbers.
   * Cache the page.
   * Load the page from cache if required.
   * @function setupPage
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items on the page.
   * @returns {void}
   */
  function setupPage(page, quantity) {
    // If the page isn't cached then set it up, otherwise just pull from cache.
    if ($scope.cache.length < page || !$scope.cache[page - 1] || $scope.cache[page - 1].model.Quantity !== quantity) {
      $scope.pager = Utils.setupPager(page, quantity, $scope.model.Total, $scope.model.Items);

      if ($scope.cache.length != $scope.pager.pageCount) $scope.cache = new Array($scope.pager.pageCount); // Reset cache.
      $scope.cache[page - 1] = {
        model: angular.copy($scope.model), // cache page.
        pager: angular.copy($scope.pager)
      }
    } else {
      $scope.model = $scope.cache[page - 1].model;
      $scope.pager = $scope.cache[page - 1].pager;
    }
  }

  /**
   * Make an AJAX request to search for payment requests that match the filter.
   * @function search
   * @returns {Promise}
   **/
  $scope.search = function () {
    return $scope.applyFilter();
  }

  /**
   * Provide a way to unselect a radio button.
   * @function changePaymentRequest
   * @param {any} $event
   * @returns {void}
   */
  $scope.changePaymentRequest = function ($event) {
    if ($scope.selectedPaymentRequestIndex === $event.target.value) {
      $scope.selectedPaymentRequestIndex = null;
      $event.target.checked = false;
      $scope.action = getAction();
    } else {
      $scope.selectedPaymentRequestIndex = $event.target.value;
      $scope.action = getAction();
    }
  }

  /**
   * Determine what action can be performed.
   * @function getAction
   * @returns {bool}
   **/
  function getAction() {
    $scope.allowReconcile = true;
    var reconciliationPayment = $scope.filter.activePayment;
    var paymentRequest = $scope.selectedPaymentRequestIndex ? $scope.model.Items[parseInt($scope.selectedPaymentRequestIndex.replace('i', ''))] : null;
    if (!paymentRequest) {
      if (reconciliationPayment.ClaimId != null) return "Unbind Payment";
      $scope.allowReconcile = false;
      return "Reconcile";
    }


    // If this is a single CAS Payment then determine whether the Payment Request amount matches.
    if (!$scope.ngDialogData.model.Payments && paymentRequest.PaymentAmount != reconciliationPayment.Amount) return "Bind Payment";

    $scope.allowReconcile = $scope.filter.activePayment.State !== 1;
    return "Reconcile";
  }

  /**
   * Find the radio button value of the payment request for the currently selected reconciliation payment.
   * @function getSelectedPaymentRequestRadioValue
   * @return {string} - The string value that represents the index position.
   **/
  function getSelectedPaymentRequestRadioValue() {
    if (!Array.isArray($scope.model.Items) || !$scope.ngDialogData.selectedPayment) return null;
    var index = $scope.model.Items.findIndex(function (item) {
      return item.PaymentRequestBatchId === $scope.ngDialogData.selectedPayment.PaymentRequestBatchId
        && item.ClaimId === $scope.ngDialogData.selectedPayment.ClaimId
        && item.ClaimVersion === $scope.ngDialogData.selectedPayment.ClaimVersion;
    });

    return index > -1 ? 'i' + index : null;
  }

  /**
   * Make an AJAX request reconcile the current reconciliation payment with the selected payment request.
   * @function reconcile
   * @returns {Promise}
   */
  $scope.reconcile = function () {
    var reconciliationPayment = $scope.filter.activePayment;
    var paymentRequest = $scope.selectedPaymentRequestIndex ? $scope.model.Items[parseInt($scope.selectedPaymentRequestIndex.replace('i',''))] : null;

    var type = reconciliationPayment.Amount >= 0 ? 'Payment Request' : 'Amount Owing';
    var warningMessage = null;
    if (!paymentRequest) {
      warningMessage = 'This action will result in removing the linked Payment Request and will not be reconciled.';
    }
    else if (paymentRequest.IsReconciled && paymentRequest.ReconciliationPaymentId !== reconciliationPayment.Id) warningMessage = 'The selected ' + type + ' is already reconciled.  Do you want to bind this payment?';
    else if (paymentRequest.DocumentNumber != reconciliationPayment.DocumentNumber) warningMessage = 'The Document Number "' + reconciliationPayment.DocumentNumber + '" does not match the selected ' + type + ' "' + paymentRequest.DocumentNumber + '".  Do you want to reconcile?';
    else if (paymentRequest.SupplierName != reconciliationPayment.SupplierName) warningMessage = 'The Supplier Name "' + reconciliationPayment.SupplierName + '" does not match the selected ' + type + ' "' + paymentRequest.SupplierName + '".  Do you want to reconcile?';
    else if (paymentRequest.SupplierNumber != reconciliationPayment.SupplierNumber) warningMessage = 'The Supplier Number "' + reconciliationPayment.SupplierNumber + '" does not match the selected ' + type + ' "' + (paymentRequest.SupplierNumber || "") + '".  Do you want to reconcile?';
    else if (paymentRequest.PaymentAmount != reconciliationPayment.Amount) warningMessage = 'The Amount "' + reconciliationPayment.Amount + '" does not match the selected ' + type + ' "' + paymentRequest.PaymentAmount + '".  Do you want to try to reconcile?';

    if (warningMessage) {
      return $scope.confirmDialog('Reconcile Payment Request', warningMessage)
        .then(function () {
          return reconcile(reconciliationPayment, paymentRequest);
        });
    }
    return reconcile(reconciliationPayment, paymentRequest);
  }

  /**
   * Reconcile the selected reconciliation payment with the selected payment request.
   * @function reconcile
   * @param {any} reconciliationPayment - The reconciliation payment object.
   * @param {object} paymentRequest - The payment request object.
   * @returns {Promise}
   */
  function reconcile(reconciliationPayment, paymentRequest) {
    var clonePayment = angular.copy(reconciliationPayment);

    if (paymentRequest) {
      clonePayment.PaymentRequestBatchId = paymentRequest.PaymentRequestBatchId;
      clonePayment.ClaimId = paymentRequest.ClaimId;
      clonePayment.ClaimVersion = paymentRequest.ClaimVersion;
      clonePayment.GrantApplicationId = paymentRequest.GrantApplicationId;
    } else {
      clonePayment.PaymentRequestBatchId = null;
      clonePayment.ClaimId = null;
      clonePayment.ClaimVersion = null;
      clonePayment.GrantApplicationId = null;
    }

    return $scope.ajax({
      url: '/Int/Payment/Reconciliation/Reconcile/Payment',
      method: 'PUT',
      data: clonePayment
    })
      .then(function (response) {
        return $scope.confirm(response.data);
      })
      .catch(angular.noop);
  }

  init();
});
