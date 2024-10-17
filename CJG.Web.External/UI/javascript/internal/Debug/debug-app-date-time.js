app.controller('DebugAppDateTimeController', function ($scope, $attrs, $controller, $timeout) {
  $scope.section = {
    name: 'AppDateTime',
    onRefresh: function () {
      return init().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
   * Initializes form data.
   * @function init
   * @returns {Promise}
   **/  
  function init() {
    return $scope.load({
      url: '/Int/Debug/AppDateTime',
      set: 'model'
    })
      .catch(angular.noop);
  }

  /**
   * Update the application datetime.
   * @function update
   * @returns {Promise}
   **/
  $scope.update = function () {
    return $scope.load({
      url: '/Int/Debug/AppDateTime',
      method: 'POST',
      data: $scope.model,
      set: 'model'
    })
      .then(function () {
        return $timeout(function () {
          $scope.setAlert({
            response: { status: 200 },
            message: 'Application date and time successfully updated.'
          });
        });
      })
      .catch(angular.noop);
  }

  /**
    * Reset the application datetime to the system datetime.
    * @function reset
    * @returns {Promise}
    **/
  $scope.reset = function () {
    return $scope.load({
      url: '/Int/Debug/AppDateTime/Reset',
      method: 'PUT',
      set: 'model'
    })
      .then(function (response) {
        return $timeout(function () {
          $scope.setAlert({
            response: { status: 200 },
            message: 'Application date and time successfully reset.'
          });
        });
      })
      .catch(angular.noop);
  }

  init();
});
