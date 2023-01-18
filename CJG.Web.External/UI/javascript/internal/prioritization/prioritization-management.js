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
        loadThresholds()
      ])
      .catch(angular.noop);
  }
  
  /**
   * The Program Notification Modal
   * @function update
   * @param {int} programNotificationId - The Program Notification Id.
   * @returns {Promise}
   **/
  $scope.update = function (programNotificationId) {
    return ngDialog.openConfirm({
      template: '/Int/Admin/Program/Modal/View',
      data: {
        programNotificationId: programNotificationId,
        applicants: $scope.applicants,
        user: $scope.user
      }
    })
      .then(function (response) {
        return $timeout(function () {
          if (response)
            $scope.setAlert({ response: { status: 200 }, message: response });
        });
      }).catch(angular.noop);
  };

  $scope.$on('ngDialog.closing', function () {
    $scope.broadcast('refreshPager');
  });

  init();
});
