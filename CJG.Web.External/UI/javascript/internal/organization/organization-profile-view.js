app.controller('OrganizationProfile', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'OrganizationProfile',
    save: {
      url: '/Int/Admin/Organization',
      method: 'PUT',
      data: function () {
        var user = $scope.getOrganizationProfileAdmin();
        if (user) {
          $scope.model.SelectedUserId = user.Id;
          $scope.model.RowVersion = user.RowVersion;
          return $scope.model;
        }
        return null;
      },
      backup: false
    },
    onSave: function () {
      if (!Utils.getValue($scope, 'alert.message'))
        return $scope.loadUsers($scope.selectedOrganizationId).then(function () {
          return $timeout(function () {
            $scope.setAlert({ response: { status: 200 }, message: 'Organization profile owner successfully updated' });
          });
        });
    }
  };

  $scope.organizations = [];
  $scope.keyword = '';
  $scope.organizationId;

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load organization profile data.
   * @function loadOrganizationProfile
   * @returns {Promise}
   **/
  function loadOrganizationProfile() {
    return $scope.load({
      url: '/Int/Admin/Organization',
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
      loadOrganizationProfile()
    ])
      .catch(angular.noop);
  }

  /**
   * Make AJAX request for organizations data
   * @function loadOrganizations
   * @returns {Promise}
   **/
  $scope.loadOrganizations = function () {
    return $scope.load({
      url: '/Int/Admin/Organization/Organizations?search=' + encodeURIComponent($scope.keyword),
      set: 'organizations'
    }).catch(angular.noop);
  };

  /**
   * Make AJAX request for users data
   * @function loadUsers
   * @param {int} organizationId - The organization id.
   * @returns {Promise}
   **/
  $scope.loadUsers = function (organizationId) {
    $scope.selectedOrganizationId = organizationId || $scope.organizationId;
    return $scope.load({
      url: '/Int/Admin/Organization/Users/' + $scope.selectedOrganizationId,
      set: 'profile'
    }).catch(angular.noop);
  };

  /**
   * set the organization profile administrator
   * @function setOrganizationProfileAdmin
   * @param {object} user - The selected user.
   * @returns {void}
   **/
  $scope.setOrganizationProfileAdmin = function (user) {
    user.IsOrganizationProfileAdministrator = true;
    for (let i = 0; i < $scope.profile.Users.length; i++) {
      var _user = $scope.profile.Users[i];
      if (_user.Id !== user.Id)
        _user.IsOrganizationProfileAdministrator = false;
    }
  };

  /**
   * get the organization profile administrator
   * @function getOrganizationProfileAdmin
   * @returns {boolean}
   **/
  $scope.getOrganizationProfileAdmin = function () {
    if (Utils.getValue($scope, 'profile.Users'))
      for (let i = 0; i < $scope.profile.Users.length; i++) {
        var user = $scope.profile.Users[i];
        if (user.IsOrganizationProfileAdministrator)
          return user;
      }
    return null;
  };

  init();
});
