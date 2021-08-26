app.controller('ApplicationDetailsNotifications', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ApplicationDetailsNotifications',
    displayName: 'Notifications',
    loaded: function () {
      return $scope.model.Id === $scope.$parent.model.Id && $scope.model.RowVersion && $scope.model.RowVersion === $scope.$parent.model.RowVersion;
    },
    onRefresh: function () {
      return loadScheduledNotifications().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.filter = {
    triggerType: null,
    orderBy: "Id"
  };

  /**
   * Make AJAX request for grant application notifications
   * @function loadScheduledNotifications
   * @returns {Promise}
   **/
  function loadScheduledNotifications() {
    return $scope.load({
      url: '/Int/Application/Details/Scheduled/Notifications/' + $scope.parent.grantApplicationId,
      set: 'model'
    });
  }

  /**
  * Make AJAX request to load all triggers.
  * @function loadTriggers
  * @returns {Promise}
  **/
  function loadTriggers() {
    return $scope.load({
      url: '/Int/Admin/Notification/Trigger/Types',
      set: 'triggers'
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
      url: '/Int/Application/Notifications/' + $scope.parent.grantApplicationId + '/' + page + '/' + quantity + (pageKeyword ? '?search=' + pageKeyword : ''),
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
  }

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
      template: '/Int/Application/Notifications/View/' + notificationId,
      data: {
        notificationId: notificationId
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
   * Turns notifications on or off for the grant application (with dialog)
   * @function toggleNotifications
   * @returns {Promise}
   **/
  $scope.toggleNotificationsDialog = function () {
    if ($scope.model.ScheduledNotificationsEnabled) {
      return $scope.confirmDialog('Disable Scheduled Notifications', 'Do you want to disable scheduled notifications for this grant application?')
        .then(function () {
          return $scope.toggleNotifications();
        })
        .catch(angular.noop);
    } else {
      return $scope.toggleNotifications();
    }
  };

  /**
   * Turns scheduled notifications on or off for the grant application
   * @function toggleNotifications
   * @returns {Promise}
   **/
  $scope.toggleNotifications = function () {
    $scope.model.ScheduledNotificationsEnabled = !$scope.model.ScheduledNotificationsEnabled;

    return $scope.load({
      url: '/Int/Application/Details/Enable/Scheduled/Notifications',
      set: 'model',
      method: 'PUT',
      data: $scope.model
    })
      .then(function (response) {
        $scope.emit('update', {
          grantFile: {
            RowVersion: response.data.RowVersion,
            ScheduledNotificationsEnabled: response.data.ScheduledNotificationsEnabled
          }
        });
        $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
      });
  };

  /**
   * Initialize the data for the form
   * @function init
   * @returns {promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadScheduledNotifications(),
      loadTriggers()
    ])
      .then(function () {
        $scope.section.isLoaded = true;
      })
      .catch(angular.noop);
  };
});
