app.controller('NotificationTypes', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'NotificationTypes',
    onRefresh: function () {
      return loadNotifications().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  $scope.filter = {
    caption: null,
    orderBy: 'DateAdded',
    notificationTriggerId: null,
    page: 1,
    quantity: 10
  };
  $scope.model = {};
  $scope.current = Object.assign({}, $scope.filter);
  $scope.cache = [];
  $scope.quantities = [10, 25, 50, 100];

  /**
   * Make AJAX request to load all notifications.
   * @function loadNotifications
   * @param {int} page
   * @param {int} quantity
   * @returns {Promise}
   **/
  function loadNotificationTypes() {
    return $scope.load({
      url: '/Int/Admin/Notification/Types?page=' + $scope.filter.page + '&quantity=' + $scope.filter.quantity,
      method: 'POST',
      data: $scope.filter,
      set: 'data',
      condition: $scope.cache.length < $scope.filter.page || !$scope.cache[$scope.filter.page - 1] || filterChanged() // The the quantity size changes, or the page isn't cached yet.
    })
      .then(function () {
        $scope.model = $scope.data.NotificationTypes;
        $scope.enableNotifications = $scope.data.EnableNotifications;
        $scope.resendRules = $scope.data.ResendRules;
        $scope.approvalRules = $scope.data.ApprovalRules;
        $scope.participantReportRules = $scope.data.ParticipantReportRules;
        $scope.claimReportRules = $scope.data.ClaimReportRules;
        $scope.completionReportRules = $scope.data.CompletionReportRules;
        $scope.recipientRules = $scope.data.RecipientRules;
        $scope.variableKeywords = $scope.data.VariableKeywords;

        return $timeout(function () {
          setupPage($scope.filter.page, $scope.filter.quantity);
        })
      })
      .catch(angular.noop);
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
   * Make AJAX request to load all milestone dates.
   * @function loadMilestoneDates
   * @returns {Promise}
   **/
  function loadMilestoneDates() {
    return $scope.load({
      url: '/Int/Admin/Notification/Milestone/Dates',
      set: 'milestoneDates'
    });
  }

  /**
   * Make AJAX request to load all application states.
   * @function loadApplicationStates
   * @returns {Promise}
   **/
  function loadApplicationStates() {
    return $scope.load({
      url: '/Int/Admin/Notification/Application/States',
      set: 'applicationStates'
    });
  }

  /**
   * Make AJAX request to load all grant programs.
   * @function loadGrantPrograms
   * @returns {Promise}
   **/
  function loadGrantPrograms() {
    return $scope.load({
      url: '/Int/Admin/Notification/Type/Grant/Programs',
      set: 'grantPrograms'
    });
  }

  /**
   * Make AJAX request for user email data
   * @function loadUserEmail
   * @returns {Promise}
   **/
  function loadUserEmail() {
    return $scope.load({
      url: '/Int/Admin/Notification/Type/User/Email',
      set: 'user'
    });
  }

  /**
   * Turns notifications on or off for the grant program (with dialog if disabled)
   * @function toggleNotificationsDialog
   * @returns {Promise}
   **/
  $scope.toggleNotificationsDialog = function () {
    if ($scope.data.EnableNotifications) {
      return $scope.confirmDialog('Disable Notifications', 'Do you want to disable notifications?')
        .then(function () {
          return $scope.toggleNotifications();
        })
        .catch(angular.noop);
    } else {
      return $scope.toggleNotifications();
    }
  }

  /**
   * Turns notifications on or off for the grant program
   * @function toggleNotifications
   * @returns {Promise}
   **/
  $scope.toggleNotifications = function () {
    return $scope.ajax({
      url: '/Int/Admin/Notification/Enable/?enable=' + !$scope.data.EnableNotifications,
      method: 'PUT'
    }).then(function (response) {
      $timeout(function () {
        $scope.data.EnableNotifications = response.data;
      })
    });
  }

  /**
   * Open up modal
   * @function openNotificationTypeModal
   * @returns {Promise}
   **/
  $scope.openNotificationTypeModal = function (notificationType) {
    if (typeof (notificationType) === 'undefined') {
      notificationType = {
        Id: 0,
        NotificationTemplate: {
          Caption: "",
          EmailSubject: "",
          EmailBody: ""
        }
      };
    }

    return showDialog(notificationType)
      .catch(angular.noop);
  }

  /**
   * Make AJAX request for modal
   * @function showDialog
   * @returns {Promise}
   **/
  function showDialog(notificationType) {
    var data = {
      Triggers: $scope.triggers,
      MilestoneDates: $scope.milestoneDates,
      ApplicationStates: $scope.applicationStates,
      ResendRules: $scope.resendRules,
      ApprovalRules: $scope.approvalRules,
      ParticipantReportRules: $scope.participantReportRules,
      ClaimReportRules: $scope.claimReportRules,
      CompletionReportRules: $scope.completionReportRules,
      RecipientRules: $scope.recipientRules,
      NotificationType: notificationType,
      grantPrograms: $scope.grantPrograms,
      user: $scope.user,
      VariableKeywords: $scope.variableKeywords
    };

    return ngDialog.openConfirm({
      template: '/Int/Admin/Notification/Types/View/' + notificationType.Id,
      data: data
    }).then(function (notificationType) {
      if (!notificationType.ValidationErrors) {
        return $timeout(function () {
          if (notificationType.IsDeleted) {
            for (let i = 0; i < $scope.model.Items.length; i++) {
              if ($scope.model.Items[i].Id === notificationType.Id) {
                $scope.model.Items.splice(i, 1);
                break;
              }
            }

            $scope.alert.message = 'Notification type successfully deleted.';
          } else {
            var found = $scope.model.Items.find(function (item) {
              return item.Id === notificationType.Id;
            });

            if (found) {
              found = notificationType;
              $scope.alert.message = 'Notification type successfully updated.';
            } else {
              $scope.model.Items.push(notificationType);
              $scope.pager.items.total++;
              $scope.pager.items.last++;

              $scope.alert.message = 'Notification type successfully added.';
            }
          }
        });
      }
    });
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
   * Refresh the page of notification types with the specified sort.
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

    return loadNotificationTypes(page, quantity)
      .then(function () {
        $scope.current = {
          page: $scope.filter.page,
          quantity: $scope.filter.quantity,
          orderBy: $scope.filter.orderBy,
          caption: $scope.filter.caption,
          notificationTriggerId: $scope.filter.notificationTriggerId
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
      || $scope.filter.caption !== $scope.current.caption
      || $scope.filter.notificationTriggerId !== $scope.current.notificationTriggerId) return true;

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
    if ($scope.cache.length < page || !$scope.cache[page - 1] || $scope.cache[page - 1].model.quantity !== quantity) {
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
   * Check if the filter is currently ordered by the specified property name.
   * @function isOrderedBy
   * @param {string} propertyName - The property name to order by.
   * @returns {bool}
   */
  function isOrderedBy(propertyName) {
    return $scope.filter.orderBy.startsWith(propertyName) ? true : false;
  }

  /**
   * Make an AJAX request to search for notification types that match the filter.
   * @function search
   * @param {any} $event - The angular event.
   * @returns {Promise}
   **/
  $scope.search = function ($event) {
    if ($event.keyCode === 13 || $event.type === 'click') return $scope.applyFilter();
    return Promise.resolve();
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadNotificationTypes(),
      loadTriggers(),
      loadMilestoneDates(),
      loadApplicationStates(),
      loadGrantPrograms(),
      loadUserEmail()
    ]).catch(angular.noop);
  }

  init();
});
