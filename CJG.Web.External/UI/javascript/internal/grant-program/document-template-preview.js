app.controller('GrantProgramDocumentTemplatePreview', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'GrantProgramDocumentTemplatePreview'
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
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    $scope.model = angular.fromJson($attrs.ngDocumentTemplate);
    $scope.template = {
      body: $scope.model.Body
    };
    return Promise.all([
      loadApplicationStates()
    ])
      .then(function () {
        if ($scope.applicationStates.InternalStates.length > 0)
          $scope.model.ApplicationStateInternal = $scope.applicationStates.InternalStates[0].Id;
        $scope.loadDocumentTemplatePreview();
      })
      .catch(angular.noop);
  }

  /**
   * Load the Document Template Preview
   * @function loadDocumentTemplatePreview
   * @returns {Promise}
   **/
  $scope.loadDocumentTemplatePreview = function () {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Document/Template/Preview',
      set: 'model',
      method: 'POST',
      data: function () {
        $scope.model.Body = $scope.template.body;
        return $scope.model;
      },
      condition: $scope.model.GrantProgramId && $scope.model.ApplicationStateInternal !== null
    });
  };

  init();
});
