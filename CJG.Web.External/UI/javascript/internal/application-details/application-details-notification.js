app.controller('ApplicationDetailsNotification', function ($scope, $attrs, $controller, $timeout, Utils) {
  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.section = {
    name: 'ApplicationDetailsNotification',
    loaded: function () {
      return $scope.model.Id === $scope.$parent.model.Id && $scope.model.RowVersion && $scope.model.RowVersion === $scope.$parent.model.RowVersion;
    }
  };

  /**
   * Make AJAX request for the grant application notification
   * @function loadNotification
   * @param {int} notificationId - The notification id.
   * @returns {Promise}
   **/
  function loadNotification(notificationId) {
    return $scope.load({
      url: '/Int/Application/Notification/' + notificationId,
      set: 'model'
    });
  }

  /**
 * Make AJAX request to send the notification
 * @function sendNotification
 * @returns {Promise}
 **/
  $scope.sendNotification = function () {
    return $scope.ajax({
      url: '/Int/Application/Notification/Send/' + $scope.model.Id,
      method: 'PUT'
    })
      .then(function () {
        return $scope.confirm('The Notification has been sent successfully');
      })
      .catch(angular.noop);
  };

  /**
   * Initialize the data for the form
   * @function init
   * @returns {void}
   **/
  $scope.init = function () {
    return Promise.all([
      loadNotification($scope.ngDialogData.notificationId)
    ])
      .then(function () {
        $scope.section.isLoaded = true;
      })
      .catch(angular.noop);
  };

  $scope.init();
});
