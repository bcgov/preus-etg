require('./wda-service-categories');

app.controller('WDAServicesManagementView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'WDAServices'
  };
  $scope.serviceTypes = [];

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load service types data.
   * @function loadServiceTypes
   * @returns {Promise}
   **/
  function loadServiceTypes() {
    return $scope.load({
      url: '/Int/WDA/Service/Types',
      set: 'serviceTypes',
      condition: !$scope.serviceTypes || !$scope.serviceTypes.length,
      localCache: true
    });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadServiceTypes()
    ])
      .then(function () {
        $scope.broadcast('show', { target: 'WDAServiceCategories' });
      })
      .catch(angular.noop);
  }

  init();
});
