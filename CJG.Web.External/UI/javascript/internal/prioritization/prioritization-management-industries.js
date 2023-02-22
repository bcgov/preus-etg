app.controller('PrioritizationManagementIndustries', function ($scope, $attrs, $controller) {
  $scope.section = {
    name: 'PrioritizationManagementIndustries'
  };

  $scope.scores = null;
  $scope.upload = {
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));
  
  $scope.selectFile = function () {
    var $input = angular.element('#industry-import-file');
    $input.click();
  }

  $scope.updateIndustries = function ($event) {
    if ($scope.upload.file) {
      return $scope.ajax({
        url: '/Int/Admin/Prioritization/Industries',
          method: 'POST',
          dataType: 'file',
          data: {
            file: $scope.upload.file,
            fileName: $scope.upload.file.name
          },
          timeout: 10 * 60 * 1000 // Ten minutes
        })
        .then(function() {
          window.location = '/Int/Admin/Prioritization/Thresholds/View';
        })
        .catch(angular.noop);
    }
    return Promise.resolve();
  }

  function loadScores() {
    return $scope.load({
      url: '/Int/Admin/Prioritization/Industries',
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
