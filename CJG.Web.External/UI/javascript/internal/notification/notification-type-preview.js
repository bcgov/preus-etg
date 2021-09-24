app.controller('NotificationTypePreview', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'NotificationTypePreview'
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

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
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    $scope.model = angular.fromJson($attrs.ngNotificationTemplate);
    $scope.template = {
      subject: $scope.model.Subject,
      body: $scope.model.Body
    };
    return Promise.all([
      loadApplicationStates(),
      loadGrantPrograms()
    ])
      .then(function () {
        if ($scope.applicationStates.InternalStates.length > 0)
          $scope.model.ApplicationStateInternal = $scope.applicationStates.InternalStates[0].Id;
        if ($scope.grantPrograms.length > 0)
          $scope.model.GrantProgramId = $scope.grantPrograms[0].Key;
        $scope.loadNotificationTypePreview();
      })
      .catch(angular.noop);
  }

  /**
   * Load the Notification Preview
   * @function loadNotificationTypePreview
   * @returns {Promise}
   **/
  $scope.loadNotificationTypePreview = function () {
    return $scope.load({
      url: '/Int/Admin/Notification/Type/Preview',
      set: 'model',
      method: 'POST',
      data: function () {
        $scope.model.Subject = $scope.template.subject;
        $scope.model.Body = $scope.template.body;
        return $scope.model;
      },
      condition: $scope.model.GrantProgramId && $scope.model.ApplicationStateInternal !== null
    });
  };

  init();
});
