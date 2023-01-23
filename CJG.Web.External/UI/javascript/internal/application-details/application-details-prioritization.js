app.controller('Prioritization', function ($scope, $attrs, $controller, $element, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'Prioritization',
    displayName: 'Prioritization',
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.grantFile.RowVersion;
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load the application prioritization data.
   * @function loadApplicantContact
   * @returns {Promise}
   **/
  function loadPrioritizationInfo() {
    return $scope.load({
      url: '/Int/Application/Prioritization/' + $scope.parent.grantApplicationId,
      set: 'model'
    });
  }

  /**
   * Initialize the section and load the data.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadPrioritizationInfo()
    ]).catch(angular.noop);
  }
});
