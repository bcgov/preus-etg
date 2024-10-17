app.controller('ReconciliationReports', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ReconciliationReports',
    onRefresh: function () {
      return loadReconciliationReports().catch(angular.noop);
    },
  };
  $scope.quantities = [10, 25, 50, 100];
  $scope.filter = {
    Page: 1,
    Quantity: $scope.quantities[0],
    OrderBy: ['DateAdded desc']
  };
  $scope.current = Object.assign({}, $scope.filter);
  $scope.cache = [];
  $scope.upload = {
    updateExisting: true
  };

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load reconciliation reports.
   * @function loadReconciliationReports
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items per page.
   * @param {string} sort - The property to sort on.
   * @returns {Promise}
   **/
  function loadReconciliationReports(page, quantity, sort) {
    if (!page) page = $scope.filter.Page;
    if (!quantity) quantity = $scope.filter.Quantity;
    if (!sort) sort = $scope.filter.OrderBy;
    return $scope.load({
      url: '/Int/Payment/Reconciliation/Reports?page=' + page + '&quantity=' + quantity + '&sort=' + sort,
      set: 'model',
      condition: $scope.cache.length < page || !$scope.cache[page - 1] || filterChanged() // The the quantity size changes, or the page isn't cached yet.
    })
      .then(function () {
        return $timeout(function () {
          setupPage(page, quantity);
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
      loadReconciliationReports()
    ]).catch(angular.noop);
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
    return $scope.filter.OrderBy.find(function (prop) { return prop.startsWith(propertyName); }) ? true : false;
  }

  /**
   * Check if the filter is currently be ordered in ascending order by the specified property name.
   * Use this to determine if the order by is ascending or descending.
   * @function isAscending
   * @param {string} propertyName - The property name to order by.
   * @returns {bool}
   */
  function isAscending(propertyName) {
    var found = $scope.filter.OrderBy.find(function (prop) { return prop.startsWith(propertyName); });
    if (!found) return true;
    return found.endsWith('desc') ? false : true;
  }

  /**
   * Order the applications by the specified property name.
   * @function sort
   * @param {string} propertyName - The property name to order by.
   * @returns {Promise}
   */
  $scope.sort = function (propertyName) {
    $scope.filter.OrderBy = [!isOrderedBy(propertyName) || !isAscending(propertyName) ? propertyName : propertyName + ' desc'];
    $scope.cache = [];
    return $scope.applyFilter();
  }

  /**
   * Apply the filter and load the reconcilation reports.
   * @function applyFilter
   * @param {int} [page] - The page number.
   * @param {int} [quantity] - The number of items per page.
   * @param {bool} [force=false] - Whether to force a refresh.
   * @returns {Promise}
   */
  $scope.applyFilter = function (page, quantity, force) {
    if (!page) page = $scope.filter.Page;
    if (!quantity) quantity = $scope.filter.Quantity;
    if (typeof (force) === 'undefined') force = false;

    if (force || filterChanged()) $scope.cache = [];

    return loadReconciliationReports(page, quantity)
      .then(function () {
        $scope.current = {
          Page: $scope.filter.Page,
          Quantity: $scope.filter.Quantity,
          OrderBy: $scope.filter.OrderBy
        }
      }).catch (angular.noop);
  }

  /**
   * Check if the filter has been changed.
   * @function filterChanged
   * @returns {bool}
   **/
  function filterChanged() {
    if ($scope.filter.Page !== $scope.current.Page
      || $scope.filter.Quantity !== $scope.current.Quantity
      || $scope.filter.OrderBy !== $scope.current.OrderBy) return true;

    return false;
  }

  /**
   * Open modal file uploader popup and then add the new file to the model.
   * @function selectFile
   * @returns {void}
   **/
  $scope.selectFile = function () {
    var $input = angular.element('#payment-reconciliation-report-file');
    $input.click();
  }

  /**
   * Upload the selected file and create a reconcilation report.
   * @function reconcile
   * @param {object} $event - The AngularJs event.
   * @returns {Promise}
   **/
  $scope.reconcile = function ($event) {
    if ($scope.upload.file) {
      if ($event.ctrlKey) {
        // Switch to create a new report.
        $scope.upload.updateExisting = false;
      }
      return $scope.ajax({
        url: '/Int/Payment/Reconciliation/Reconcile',
        method: 'POST',
        dataType: 'file',
        data: {
          file: $scope.upload.file,
          fileName: $scope.upload.file.name,
          createNew: !$scope.upload.updateExisting
        },
        timeout: 10 * 60 * 1000 // Ten minutes
      })
        .then(function (response) {
          return $timeout(function () {
            window.location = '/Int/Payment/Reconciliation/Report/View/' + response.data.Id;
          });
        })
        .catch(angular.noop);
    }
    return Promise.resolve();
  }

  /**
   * Handle when the report row is clicked.
   * @function selectReport
   * @param {object} $event - The AngularJs event.
   * @param {object} report - The selected reconciliation report.
   * @returns {void}
   */
  $scope.clickReport = function ($event, report) {
    if ($event.ctrlKey) return $scope.deleteReport(report, $event);
  }

  /**
   * Handle when the report row is double clicked.
   * @function selectReport
   * @param {object} $event - The AngularJs event.
   * @param {object} report - The selected reconciliation report.
   * @returns {void}
   */
  $scope.viewReport = function (report) {
    window.location = '/Int/Payment/Reconciliation/Report/View/' + report.Id;
  }

  /**
   * Delete the specified report from the datasource.
   * @function deleteReport
   * @param {object} report - The report to delete.
   * @param {object} $event - The angular event.
   * @returns {Promise}
   */
  $scope.deleteReport = function (report, $event) {
    if ($event) {
      $event.stopPropagation();
      $event.preventDefault();
    }
    return $scope.confirmDialog('Delete Reconciliation Report', 'Do you want to delete this report?')
      .then(function () {
        return $scope.ajax({
          url: '/Int/Payment/Reconciliation/Report/Delete',
          method: 'PUT',
          data: report,
          timeout: 10 * 60 * 1000 // Ten minutes
        })
          .then(function (response) {
            $scope.model.Items.splice($scope.model.Items.indexOf(report), 1);
          });
      })
      .catch(angular.noop);
  }

  init();
});
