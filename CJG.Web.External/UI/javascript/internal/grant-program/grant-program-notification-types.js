app.controller('GrantProgramNotificationTypes', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'GrantProgramNotificationTypes',
    save: {
      url: '/Int/Admin/Grant/Program/Notification/Types/',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model.Id === $scope.$parent.model.Id && $scope.model.RowVersion && $scope.model.RowVersion === $scope.$parent.model.RowVersion;
    },
    onSave: function () {
      $scope.emit('update', { model: $scope.model });
      loadGrantProgramNotificationTypes();
    },
    onRefresh: function () {
      return loadGrantProgramNotificationTypes().then(function () {
        return $scope.broadcast('refreshPager');
      }).catch(angular.noop);
    }
  };

  $scope.notificationTrigger = null;
  $scope.notificationTypeList = [];

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for grant program notifications types
   * @function loadGrantProgramNotificationTypes
   * @returns {Promise}
   **/
  function loadGrantProgramNotificationTypes() {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Notification/Types/' + $scope.$parent.model.Id,
      set: 'model'
    });
  }

  /**
   * Make AJAX request for notifications types
   * @function loadNotificationTypes
   * @returns {Promise}
   **/
  function loadNotificationTypes() {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Notification/Types/',
      set: 'notificationTypes'
    });
  }

  /**
   * Make AJAX request to load all triggers.
   * @function loadTriggers
   * @returns {Promise}
   **/
  function loadTriggers() {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Notification/Triggers',
      set: 'triggers'
    });
  }

  /**
   * Make AJAX request to load all application states.
   * @function loadApplicationStates
   * @returns {Promise}
   **/
  function loadApplicationStates() {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Notification/Application/States',
      set: 'applicationStates'
    });
  }
  
  /**
   * Make AJAX request for user email data
   * @function loadUserEmail
   * @returns {Promise}
   **/
  function loadUserEmail() {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Notification/User/Email',
      set: 'user'
    });
  }

  /**
   * Get the notification types.
   * @function getNotificationTypes
   * @returns {object}
   **/
  $scope.getNotificationTypes = function () {
    var items = $scope.model.GrantProgramNotificationTypes;
    if (!$scope.notificationTrigger)
      return items;
    var result = [];
    for (let i = 0; i < items.length; i++) {
      var item = items[i];
      if (item.TriggerType && item.TriggerType === $scope.notificationTrigger.Caption || item.NotificationTriggerId === $scope.notificationTrigger.Id)
        result.push(item);
    }
    return result;
  };

  /**
   * Update the notification trigger filter.
   * @function updateTriggerFilter
   * @param {string} trigger - The search filter notification trigger.
   * @returns {void}
   **/
  $scope.updateTriggerFilter = function (trigger) {
    $scope.notificationTrigger = trigger;
    $scope.updateNotificationTypeList();
  };

  /**
   * Update notification types to be add to grant program
   * @function updateNotificationTypeList
   * @returns {void}
   **/
  $scope.updateNotificationTypeList = function () {
    $scope.notificationTypeList = [];
    $scope.notificationTypes.NotificationTypes.map(function (item) {
      if ((!$scope.notificationTrigger || item.NotificationTriggerId === $scope.notificationTrigger.Id)
        && !$scope.model.GrantProgramNotificationTypes.some(function (selected) {
          return selected.Id ? selected.Id === item.Id : selected.NotificationTypeId === item.Id;
        })) {
        $scope.notificationTypeList.push(item);
      }
    });
    $scope.broadcast('refreshPager');
  };

  /**
   * Add notification type to grant program
   * @function addNotificationType
   * @param {int} notificationTypeId - The notification type id.
   * @returns {Promise}
   **/
  $scope.addNotificationType = function (notificationTypeId) {
    if (notificationTypeId > 0) {
      return $scope.load({
        url: '/Int/Admin/Notification/Type/' + notificationTypeId,
        set: 'data'
      })
        .then(function () {
          return $timeout(function () {
            var deepCopy = {};
            angular.copy($scope.data, deepCopy);
            $scope.model.GrantProgramNotificationTypes.unshift(deepCopy);
            $scope.updateNotificationTypeList();
          });
        });
    }
  };

  /**
   * Cancel notification type to grant program
   * @function cancelNotificationType
   * @param {object} notificationType - The notification type.
   * @returns {void}
   **/
  $scope.cancelNotificationType = function (notificationType) {
    var index = $scope.model.GrantProgramNotificationTypes.indexOf(notificationType);
    $scope.model.GrantProgramNotificationTypes.splice(index, 1);
    $scope.updateNotificationTypeList();
  };

  /**
  * Open up modal
  * @function openNotificationTypeModal
   * @param {int} grantProgramNotificationType - The notification type.
  * @returns {Promise}
  **/
  $scope.openNotificationTypeModal = function (grantProgramNotificationType) {
    return ngDialog.openConfirm({
      template: '/Int/Admin/Grant/Program/Notification/Type/View/' + grantProgramNotificationType.Id,
      data: {
        model: angular.copy(grantProgramNotificationType),
        editing: $scope.section.editing,
        grantPrograms: $scope.$parent.grantPrograms,
        applicationStates: $scope.applicationStates,
        user: $scope.user,
        variableKeywords: $scope.model.VariableKeywords
      }
    }).then(function (data) {
      if (data) {
        grantProgramNotificationType.IsActive = data.IsActive;
        grantProgramNotificationType.ToBeDeleted = data.ToBeDeleted;
        grantProgramNotificationType.NotificationTemplate.EmailSubject = data.NotificationTemplate.EmailSubject;
        grantProgramNotificationType.NotificationTemplate.EmailBody = data.NotificationTemplate.EmailBody;
      }
    }).catch(angular.noop);
  };

  /**
   * Initialize the data for the form
   * @function init
   * @returns {void}
   **/
  $scope.init = function () {
    return Promise.all([
      loadGrantProgramNotificationTypes(),
      loadNotificationTypes(),
      loadTriggers(),
      loadApplicationStates(),
      loadUserEmail()
    ])
      .then(function () {
        $scope.updateNotificationTypeList();
      })
      .catch(angular.noop);
  };
});
