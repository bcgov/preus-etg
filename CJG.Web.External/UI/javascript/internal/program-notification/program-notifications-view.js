require('./program-notification-modal');

app.controller('ProgramNotificationManagement', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ProgramNotificationManagement'
  };

  angular.extend(this, $controller('ParentSection', { $scope, $attrs }));

  /**
   * Make AJAX request for grant program applicants data
   * @function loadGrantProgramApplicants
   * @returns {Promise}
   **/
  function loadGrantProgramApplicants() {
    return $scope.load({
      url: '/Int/Admin/Program/Notification/Applicants',
      set: 'applicants'
    });
  }

  /**
   * Make AJAX request for user email data
   * @function loadUserEmail
   * @returns {Promise}
   **/
  function loadUserEmail() {
    return $scope.load({
      url: '/Int/Admin/Program/Notification/User/Email',
      set: 'user'
    });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadGrantProgramApplicants(),
      loadUserEmail()
    ])
      .catch(angular.noop);
  }

  /**
   * Get the filtered program notifications.
   * @function getProgramNotifications
   * @param {string} pageKeyword - The search filter keyword.
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items in a page.
   * @returns {Promise}
   **/
  $scope.getProgramNotifications = function (pageKeyword, page, quantity) {
    return $scope.ajax({
      url: '/Int/Admin/Program/Notifications/Search/' + page + '/' + quantity + (pageKeyword ? '?search=' + pageKeyword : '')
    })
      .then(function (response) {
        return response.data;
      });
  };

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
