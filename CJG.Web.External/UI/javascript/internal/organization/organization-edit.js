app.controller('OrganizationEdit', function ($scope, $controller, $timeout, Utils) {

  angular.extend(this, $controller('Base', { $scope }));

  /**
 * Make AJAX request to save the organization
 * @function saveOrganization
 * @returns {Promise}
 **/
  $scope.saveOrganization = function () {
    var organization = $scope.ngDialogData.model.org;
    return $scope.ajax({
      url: '/Int/Organization/Upsert',
      method: function () {
        return organization.OrgId ? 'PUT' : 'POST';
      },
      data: organization
    })
      .then(function () {
        return $scope.confirm(organization);
      })
      .catch(angular.noop);
  };

  /**
* Make AJAX request for organization types data
* @function loadOrganizationTypes
* @returns {Promise}
**/
  function loadOrganizationTypes() {
    return $scope.load({
      url: '/Int/Organization/Types',
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
      url: '/Int/Organization/Legal/Structures',
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
      url: '/Int/Organization/Provinces',
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
      url: '/Int/Organization/NAICS/' + level + '/' + (parentId ? parentId : ''),
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
      $scope.ngDialogData.model.org["Naics" + x + "Id"] = null;
      $scope['naics' + x] = [];
    }
    loadNAICS(level, level > 1 ? $scope.ngDialogData.model.org["Naics" + (level - 1) + "Id"] : 0);
  };

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    if ($scope.ngDialogData.model.org.OrgId == 0) {
      $scope.ngDialogData.model.org.HeadOfficeAddress = {
        RegionId: 'BC'
      };
    }
    return Promise.all([
      loadOrganizationTypes(),
      loadLegalStructures(),
      loadProvinces()
        .then(function () {
          for (let x = 1; x <= 5; x++) {
            loadNAICS(x, x > 1 ? $scope.ngDialogData.model.org["Naics" + (x - 1) + "Id"] : 0);
          }
        }).catch(angular.noop)
    ])
      .catch(angular.noop);
  }


  /**
   * Cancel the organization popup.
   * @function cancelOrganization
   * @returns {Promise}
   **/
  $scope.cancelOrganization = function () {
    return $scope.confirm();
  };

  init();
});
