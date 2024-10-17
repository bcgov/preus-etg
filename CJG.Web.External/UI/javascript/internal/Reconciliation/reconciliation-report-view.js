require('./reconcile-payment-view')

app.controller('ReconciliationReport', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ReconciliationReport',
    onRefresh: function () {
      return loadReconciliationReport().catch(angular.noop);
    },
    reconciliationReportId: $attrs.reconciliationReportId,
    orderBy: 'DateCreated',
    showAll: false
  };

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load reconciliation report.
   * @function loadReconciliationReport
   * @returns {Promise}
   **/
  function loadReconciliationReport() {
    return $scope.load({
      url: '/Int/Payment/Reconciliation/Report/' + $scope.section.reconciliationReportId,
      set: 'model'
    }).then(function () {
      $scope.section.showAll = $scope.model.IsReconciled;
      applyFilter();
    });
  }

  /**
   * Get the filtered notifications.
   * @function getReconciliationReport
   * @returns {Array}
   **/
  $scope.getReconciliationReport = function () {
    if ($scope.section.showAll) {
      return $scope.model.Payments;
    }
    var result = [];
    for (let i = 0; i < $scope.model.Payments.length; i++) {
      var payment = $scope.model.Payments[i];
      if (payment.State !== 1) {
        result.push(payment);
      }
    }
    return result;
  };

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadReconciliationReport()
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
    return $scope.section.orderBy.startsWith(propertyName) ? true : false;
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
    return $scope.section.orderBy.endsWith('desc') ? false : true;
  }

  /**
   * Order the applications by the specified property name.
   * @function sort
   * @param {string} propertyName - The property name to order by.
   * @returns {Promise}
   */
  $scope.sort = function (propertyName) {
    $scope.section.orderBy = !isOrderedBy(propertyName) || !isAscending(propertyName) ? propertyName : propertyName + ' desc';
    return applyFilter();
  }

  /**
   * Refresh the page of payments with the specified sort.
   * @function applyFilter
   * @returns {void}
   **/
  function applyFilter() {
    return $timeout(function () {
      var propertyName = $scope.section.orderBy.replace(' asc', '').replace(' desc', '');
      var asc = isAscending(propertyName);
      $scope.model.Payments.sort(function (a, b) {
        return compare(a[propertyName], b[propertyName], asc);
      });
      $scope.broadcast('refreshPager');
    });
  }

  /**
   * Compares two values for sorting an array.
   * @function compare
   * @param {any} a - First value
   * @param {any} b - Second value
   * @param {any} ascending - Whether the sort is ascending or descending
   * @returns {int}
   */
  function compare(a, b, ascending) {
    var result = 0;
    if (typeof (a) === 'string')
      result = a.localeCompare(b);
    else
      result = a < b ? -1 : a > b ? 1 : 0;

    return ascending ? result : result * -1;
  }

  /**
   * Provides a hover label for the payment to describe its state.
   * @function status
   * @param {object} payment - The payment
   * @returns {string}
   */
  $scope.status = function (payment) {
    switch (payment.State) {
      case (0): // NotReconciled
        return 'This payment has not been reconciled';
      case (1): // Reconciled
        return null;
      case (2): // NoMatch
        return payment.ClaimId ? 'This payment does not exist in CAS' : 'This payment does not exist in STG';
      case (3): // Duplicate
        return 'This payment is a duplicate';
      case (4): // InvalidAmount
        return 'This payment amount does not match';
      case (5): // InvalidDocumentNumber
        return 'This document number is incorrect';
      case (6): // InvalidSupplierName
        return 'This supplier name does not match';
      case (7): // InvalidSupplierNumber
        return 'This supplier number does not match';
    }
  }

  /**
   * Open a modal popup dialog to display the selected payment.
   * @function reconcile
   * @param {object} payment - The payment to view
   * @returns {Promise}
   */
  $scope.reconcile = function (payment) {
    if (payment.FromCAS) {
      return ngDialog.openConfirm({
        template: '/Int/Payment/Reconcile/Payment/Request/View/' + payment.Id,
        data: {
          title: 'Reconcile Payment Request',
          model: payment
        },
        controller: 'PaymentReconciliation'
      })
        .then(function (result) {
          return loadReconciliationReport();
        })
        .catch(angular.noop);
    } else return Promise.resolve();
  }

   /**
   * Show All Check Box change
   * @function changeShowAll
   * @returns {void}
   */
  $scope.changeShowAll = function () {
    $scope.broadcast('refreshPager');
  }
  init();
});
