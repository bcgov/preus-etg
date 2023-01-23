app.controller('PrioritizationManagement', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'PrioritizationManagement',
    save: {
      url: '/Int/Admin/Prioritization/Thresholds',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    onSave: function () {
      window.location = '/Int/Admin/Prioritization/Thresholds/View';
    }
  };

  angular.extend(this, $controller('ParentSection', { $scope, $attrs }));

  /**
   * Make AJAX request for grant program applicants data
   * @function loadGrantProgramApplicants
   * @returns {Promise}
   **/
  function loadThresholds() {
    return $scope.load({
      url: '/Int/Admin/Prioritization/Thresholds',
      set: 'model'
    });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
        loadThresholds(),
      ])
      .catch(angular.noop);
  }
  
  $scope.$on('ngDialog.closing', function () {
    $scope.broadcast('refreshPager');
  });

  init();
});
