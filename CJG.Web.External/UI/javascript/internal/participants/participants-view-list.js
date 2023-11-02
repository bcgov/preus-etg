app.controller('ParticipantsView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ParticipantsView',
    onRefresh: function () {
      return loadApplications().catch(angular.noop);
    }
  };

  $scope.quantities = [100, 50, 25];
  $scope.filter = {
    Page: 1,
    Quantity: $scope.quantities[0],
    OrderBy: []
  };
  $scope.current = {
    Participant: '',
    FileNumber: ''
  }
  $scope.cache = [];

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load applications.
   * @function loadApplications
   * @returns {Promise}
   **/
  function loadApplications(page, quantity) {
    if (!page)
      page = 1;

    if (!quantity)
      quantity = $scope.quantities[0];

    return $scope.load({
      url: '/Int/Participants/Search?page=' + page + '&quantity=' + quantity,
      method: 'POST',
      data: $scope.filter,
      set: 'model',
      condition: quantity != $scope.model.Quantity || $scope.cache.length < page || !$scope.cache[page - 1] || filterChanged() // The the quantity size changes, or the page isn't cached yet.
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
      loadApplications()
    ])
      .catch(angular.noop);
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

      if ($scope.filter.SelectAll) $scope.toggleAll(); // Default them all selected based on the filter.
    } else {
      $scope.model = $scope.cache[page - 1].model;
      $scope.pager = $scope.cache[page - 1].pager;
    }
  }

  /**
   * Check if the filter has been changed.
   * @function filterChanged
   * @returns {void}
   **/
  function filterChanged() {
    if ($scope.filter.Participant !== $scope.current.Participant
      || $scope.filter.FileNumber !== $scope.current.FileNumber)
      return true;

    return false;
  }

  /**
   * Clear the filter.
   * @function clearFilter
   * @returns {void}
   */
  $scope.clearFilter = function () {
    if ($scope.filter == null)
      return angular.noop;

    $scope.filter.FileNumber = '';
    $scope.filter.Participant = '';

    return $scope.applyFilter();
  }

  /**
   * Apply the filter and load the applications.
   * @function applyFilter
   * @param {int} [page] - The page number.
   * @param {int} [quantity] - The number of items per page.
   * @param {bool} [force=false] - Whether to force a refresh.
   * @returns {Promise}
   */
  $scope.applyFilter = function (page, quantity, force) {
    if (!page)
      page = 1;

    if (!quantity)
      quantity = $scope.filter.Quantity;

    if (typeof(force) === 'undefined')
      force = false;

    if (force || filterChanged()) $scope.cache = [];

    return loadApplications(page, quantity)
      .then(function () {
        $scope.current = {
          FileNumber: $scope.filter.FileNumber,
          Participant: $scope.filter.Participant,
          Quantity: $scope.filter.Quantity
        }
      })
      .catch(angular.noop);
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

  $scope.search = function ($event) {
    if ($event.keyCode === 13) return $scope.applyFilter();
    return Promise.resolve();
  }

  init();
});
