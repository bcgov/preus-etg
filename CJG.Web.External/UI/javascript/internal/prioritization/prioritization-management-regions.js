app.controller('PrioritizationManagementRegions', function ($scope, $attrs, $controller) {
  $scope.section = {
    name: 'PrioritizationManagementRegions'
  };

  $scope.scores = null;
  $scope.upload = {
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  $scope.selectRegionFile = function () {
    var $input = angular.element('#regions-import-file');
    $input.click();
  }

  $scope.updateRegions = function ($event) {
    if ($scope.upload.file) {
      return $scope.ajax({
          url: '/Int/Admin/Prioritization/Regions',
          method: 'POST',
          dataType: 'file',
          data: {
            file: $scope.upload.file,
            fileName: $scope.upload.file.name
          },
          timeout: 10 * 60 * 1000 // Ten minutes
        })
        .then(function () {
          window.location = '/Int/Admin/Prioritization/Thresholds/View';
        })
        .catch(angular.noop);
    }
    return Promise.resolve();
  }

  function loadScores() {
    return $scope.load({
      url: '/Int/Admin/Prioritization/Regions',
      set: 'scores',
      condition: !$scope.scores
    });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
        loadScores()
      ])
      .catch(angular.noop);
  }

  init();
});
