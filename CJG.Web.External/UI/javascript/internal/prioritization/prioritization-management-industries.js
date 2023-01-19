app.controller('PrioritizationManagementIndustries', function ($scope, $attrs, $controller) {
  $scope.section = {
    name: 'PrioritizationManagementIndustries'
  };

  $scope.scores = null;

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  function loadScores() {
    return $scope.load({
      url: '/Int/Admin/Prioritization/Scores',
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
