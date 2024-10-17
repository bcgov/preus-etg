app.controller('GrantProgramNotifications', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'GrantProgramNotifications',
    onRefresh: function () {
      return loadFilters().then(function () {
        return $scope.broadcast('refreshPager');
      }).catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.filter = {
    orderBy: 'Id',
    organization: null,
    notificationType: null,
    status: null
  };
  $scope.statuses = ['Queued', 'Sent', 'Failed'];

  /**
   * Make AJAX request for grant program filters data
   * @function loadFilters
   * @returns {Promise}
   **/
  function loadFilters() {
    return $scope.ajax({
      url: '/Int/Admin/Grant/Program/Notification/Filters/' + $scope.$parent.model.Id
    })
      .then(function (response) {
        return $timeout(function () {
          $scope.organizations = response.data.Organizations;
          $scope.notificationTypes = response.data.NotificationTypes;
        });
      });
  }

  /**
   * Get the filtered notifications.
   * @function getNotifications
   * @param {string} pageKeyword - The search filter keyword.
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items in a page.
   * @returns {Promise}
   **/
  $scope.getNotifications = function (pageKeyword, page, quantity) {
    return $scope.ajax({
      url: '/Int/Admin/Grant/Program/Notifications/' + $scope.$parent.model.Id + '/' + page + '/' + quantity,
      method: 'POST',
      data: $scope.filter
    })
      .then(function (response) {
        return response.data;
      });
  };

  /**
   * Get the sorting order of the specified property.
   * @function sortDirection
   * @param {any} propertyName - The property name to order by.
   * @returns {string} string
   */
  $scope.sortDirection = function (propertyName) {
    if (!isOrderedBy(propertyName)) return 'sorting';
    return isAscending(propertyName) ? 'sorting_asc' : 'sorting_desc';
  };

  /**
   * Check if the filter is currently be ordered in ascending order by the specified property name.
   * Use this to determine if the order by is ascending or descending.
   * @function isAscending
   * @param {string} propertyName - The property name to order by.
   * @returns {bool} boolean
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
   * @returns {Promise} promise
   */
  $scope.sort = function (propertyName) {
    $scope.filter.orderBy = !isOrderedBy(propertyName) || !isAscending(propertyName) ? propertyName : propertyName + ' desc';
    return $scope.applyFilter();
  };

  /**
   * Refresh the page of notification types with the specified sort.
   * @function applyFilter
   * @returns {Promise} promise
   */
  $scope.applyFilter = function () {
    return $scope.broadcast('refreshPager');
  };

  /**
   * Check if the filter is currently ordered by the specified property name.
   * @function isOrderedBy
   * @param {string} propertyName - The property name to order by.
   * @returns {bool} boolean
   */
  function isOrderedBy(propertyName) {
    return $scope.filter.orderBy.startsWith(propertyName) ? true : false;
  }

  /**
   * Open up modal
   * @function openNotificationModal
   * @param {int} notificationId - The notification id.
   * @returns {Promise}
   **/
  $scope.openNotificationModal = function (notificationId) {
    return showDialog(notificationId)
      .catch(angular.noop);
  };

  /**
   * Make AJAX request for modal
   * @function showDialog
   * @param {int} notificationId - The notification id.
   * @returns {Promise}
   **/
  function showDialog(notificationId) {
    return ngDialog.openConfirm({
      template: '/Int/Admin/Grant/Program/Notification/View/' + notificationId,
      data: {
        notificationId: notificationId,
        grantProgramId: $scope.$parent.model.Id
      }
    })
      .then(function (response) {
        return $timeout(function () {
          if (response) {
            $scope.setAlert({ response: { status: 200 }, message: response });
            $scope.broadcast('refreshPager');
          }
        });
      }).catch(angular.noop);
  }

  /**
   * Initialize the data for the form
   * @function init
   * @returns {promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadFilters()
    ])
      .then(function () {
        $scope.section.isLoaded = true;
      })
      .catch(angular.noop);
  };
});
