require('./organization-edit');

app.controller('OrganizationList', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'Organization',
    onRefresh: function () {
      return loadOrganization().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
   * Make AJAX request to load organization data.
   * @function loadOrganization
   * @returns {Promise}
   **/
  function loadOrganization() {
    return $scope.load({
      url: '/Int/Organization/List',
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
      loadOrganization()
    ]).catch(angular.noop);
  }

  /**
   * Get the filtered organizations.
   * @function getOrganizations
   * @param {string} pageKeyword - The search filter keyword.
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items in a page.
   * @returns {Promise}
   **/
  $scope.getOrganizations = function (pageKeyword, page, quantity) {
    return $scope.ajax({
      url: '/Int/Organization/Search/' + page + '/' + quantity + (pageKeyword ? '?search=' + pageKeyword : '')
    })
      .then(function (response) {
        return Promise.resolve(response.data);
      })
      .catch(angular.noop);
  };

  /**
   * Show the add Organization modal popup.
   * @function addOrganization
   * @returns {Promise}
   **/
  $scope.addOrganization = function () {
    return ngDialog.openConfirm({
      template: '/Int/Organization/Form/View',
      data: {
        title: 'Add Organization',
        model: {
          org: {
            OrgId: 0
          },
          admin: $scope.model.AdminUser
        }
      },
      controller: 'OrganizationEdit'
    }).then(function (response) {
      if (response)
        $scope.broadcast('refreshPager');
    }).catch(angular.noop);
  };

  /**
   * Show the edit Organization modal popup.
   * @function editOrganization
   * @param {object} organization - The Organization.
   * @returns {Promise}
   **/
  $scope.editOrganization = function (org) {
    return ngDialog.openConfirm({
      template: '/Int/Organization/Form/View',
      data: {
        title: 'Organization Details',
        model: {
          org: angular.copy(org),
          admin: $scope.model.AdminUser
        }
      },
      controller: 'OrganizationEdit'
    }).then(function (response) {
      if (response)
        $scope.broadcast('refreshPager');
    }).catch(angular.noop);
  };

  init();
});
