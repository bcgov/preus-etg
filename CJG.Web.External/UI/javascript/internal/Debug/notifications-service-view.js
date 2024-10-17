app.controller('NotificationsServiceView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
  };

  $scope.quantities = [10, 25, 50, 100];
  $scope.filter = {
    Page: 1,
    Quantity: $scope.quantities[0],
    Search: null,
    Status: 'Queued,Failed',
    OrderBy: []
  };
  $scope.current = {
  }
  $scope.cache = [];

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make an AJAX request to fetch the notification queue.
   * @function loadNotificationQueue
   * @param {any} page
   * @param {any} quantity
   * @returns {Promise}
   */
  function loadNotificationQueue(page, quantity) {
    if (!page) page = 1;
    if (!quantity) quantity = $scope.quantities[0];
    return $scope.load({
      url: '/Int/Debug/Notifications/Service/Queue',
      method: 'POST',
      data: function () {
        var filter = angular.copy($scope.filter);
        filter.OrderBy = filter.OrderBy.join(',');
        return filter;
      },
      set: 'model'
    })
      .then(function () {
        return $timeout(function () {
          setupPage(page, quantity);
        });
      });
  }

  /**
   * Initialize the form data.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return loadNotificationQueue()
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
    if ($scope.cache.length < page || !$scope.cache[page - 1] || $scope.cache[page - 1].model.Queue.length !== quantity) {
      $scope.pager = Utils.setupPager(page, quantity, $scope.model.NotificationsInQueue, $scope.model.Queue);

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
   * Check if the filter has been changed.
   * @function filterChanged
   * @returns {bool}
   **/
  function filterChanged() {
    if ($scope.filter.Quantity !== $scope.current.Quantity
      || $scope.filter.Search !== $scope.current.Search) return true;

    return false;
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
    if (!page) page = $scope.filter.Page;
    if (!quantity) quantity = $scope.filter.Quantity;
    if (typeof (force) === 'undefined') force = false;

    $scope.filter.Page = page;
    $scope.filter.Quantity = quantity;

    if (force || filterChanged()) {
      $scope.cache = [];
    }

    return loadNotificationQueue(page, quantity)
      .then(function () {
        $scope.current = {
          Quantity: $scope.filter.Quantity,
          Search: $scope.filter.Search,
          OrderBy: $scope.filter.OrderBy
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

  /**
   * Make an AJAX request with the filter including the search terms.
   * @function search
   * @param {any} $event - The angular event.
   * @returns {Promise}
   */
  $scope.search = function ($event) {
    if ($event.keyCode === 13) return $scope.applyFilter();
    return Promise.resolve();
  }

  /**
   * Make AJAX request to add scheduled notifications to the queue.
   * @function queueScheduledNotifications
   * @returns {Promise}
   **/
  $scope.queueScheduledNotifications = function () {
    $scope.filter.Status = 'Queued,Failed';
    return $scope.load({
      url: '/Int/Debug/Notifications/Service/Queue/Scheduled/Notifications',
      method: 'POST',
      data: $scope.model,
      set: 'model'
    })
      .then(function () {
        $scope.section.sent = true;
      })
      .catch(angular.noop);
  }
  
  /**
   * Make AJAX request to send the notifications in the queue.
   * @function sendNotifications
   * @returns {Promise}
   **/
  $scope.sendNotifications = function () {
    $scope.filter.Status = null;
    return $scope.load({
      url: '/Int/Debug/Notifications/Service/Send/Notifications',
      method: 'POST',
      data: $scope.model,
      set: 'model'
    })
      .then(function () {
        return $timeout(function () {
          $scope.section.sent = true;
          $scope.cache = [];
          setupPage($scope.filter.Page, $scope.filter.Quantity);
        });
      })
      .catch(angular.noop);
  }

  /**
   * Show the notification email body message.
   * @function viewNotification
   * @param {object} notification - The notification to view
   * @returns {Promise}
   */
  $scope.viewNotification = function (notification) {
    return $scope.messageDialog('Notification', notification.EmailBody);
  }

  init();
});
