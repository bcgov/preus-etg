app.controller('OrganizationProfile', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'OrganizationProfile',
    save: {
      url: '/Ext/Organization/Profile',
      method: function () {
        return $scope.model.Id ? 'PUT' : 'POST';
      },
      data: function () {
        return $scope.model;
      },
      backup: false
    },
    onSave: function () {
      if (!Utils.getValue($scope, 'alert.message'))
        return $timeout(function () {
          $scope.setAlert({ response: { status: 200 }, message: 'Organization Profile has been updated successfully.' });
        });
    }
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
   * Make AJAX request to load organization profile data.
   * @function loadOrganizationProfile
   * @returns {Promise}
   **/
  function loadOrganizationProfile() {
    return $scope.load({
      url: '/Ext/Organization/Profile',
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
      loadOrganizationTypes(),
      loadLegalStructures(),
      loadProvinces(),
      loadOrganizationProfile()
        .then(function () {
          for (let x = 1; x <= 5; x++)
            loadNAICS(x, x > 1 ? $scope.model["Naics" + (x - 1) + "Id"] : 0);
        }).catch(angular.noop)
    ])
      .catch(angular.noop);
  }

  /**
   * Make AJAX request for organization types data
   * @function loadOrganizationTypes
   * @returns {Promise}
   **/
  function loadOrganizationTypes() {
    return $scope.load({
      url: '/Ext/Organization/Types',
      set: 'organizationTypes',
      condition: !$scope.organizationTypes || !$scope.organizationTypes.length
    });
  }

  /**
   * Make AJAX request for legal structures data
   * @function loadLegalStructures
   * @returns {Promise}
   **/
  function loadLegalStructures() {
    return $scope.load({
      url: '/Ext/Organization/Legal/Structures',
      set: 'legalStructures',
      condition: !$scope.legalStructures || !$scope.legalStructures.length
    });
  }

  /**
   * Make AJAX request for provinces data
   * @function loadProvinces
   * @returns {Promise}
   **/
  function loadProvinces() {
    return $scope.load({
      url: '/Ext/Organization/Provinces',
      set: 'provinces',
      condition: !$scope.provinces || !$scope.provinces.length
    });
  }

  /**
   * Make AJAX request and load NAICS data
   * @function loadNAICS
   * @param {int} level - The NAICS level.
   * @param {int} [parentId] - The Id of the prior level.
   * @returns {Promise}
   **/
  function loadNAICS(level, parentId) {
    if (!level) level = 1;
    return $scope.load({
      url: '/Ext/Organization/NAICS/' + level + '/' + (parentId ? parentId : ''),
      set: function (response) {
        $scope['naics' + level] = response.data;
      },
      condition: level === 1 || parentId
    });
  }

  /**
   * Make an AJAX request to fetch the next NAICS dropdown data for the selected parent NAICS.
   * @function changeNAICS
   * @param {int} level - The level to load.
   */
  $scope.changeNAICS = function (level) {
    for (let x = level; x <= 5; x++) {
      $scope.model["Naics" + x + "Id"] = null;
      $scope['naics' + x] = [];
    }
    loadNAICS(level, level > 1 ? $scope.model["Naics" + (level - 1) + "Id"] : 0);
  };

  /**
   * Get the name for the organization type.
   * @function getOrganizationType
   * @returns {string}
   **/
  $scope.getOrganizationType = function () {
    if ($scope.organizationTypes)
      for (let i = 0; i < $scope.organizationTypes.length; i++) {
        var type = $scope.organizationTypes[i];
        if (type.Key === $scope.model.OrganizationTypeId)
          return type.Value;
      }
    return null;
  };

  init();
});
