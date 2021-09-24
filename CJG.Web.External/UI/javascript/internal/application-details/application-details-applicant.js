app.controller('Applicant', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'Applicant',
    displayName: 'Applicant',
    save: {
      url: '/Int/Application/Applicant',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.grantFile.RowVersion;
    },
    onSave: function () {
      $scope.emit('update', { grantFile: { RowVersion: $scope.model.RowVersion } });
      $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
      $scope.emit('refresh', { target: 'ApplicationSummary', force: true });
    },
    onRefresh: function () {
      return loadApplicant().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for country data.
   * @function loadCountries
   * @returns {Promise}
   **/
  function loadCountries() {
    return $scope.load({
      url: '/Int/Address/Countries',
      set: 'countries',
      condition: !$scope.countries || !$scope.countries.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request for province data.
   * @function loadProvinces
   * @returns {Promise}
   **/
  function loadProvinces() {
    return $scope.load({
      url: '/Int/Address/Provinces',
      set: 'provinces',
      condition: !$scope.provinces || !$scope.provinces.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request for organization types
   * @function loadOrganizationTypes
   * @returns {Promise}
   **/
  function loadOrganizationTypes() {
    return $scope.load({
      url: '/Int/Application/Applicant/Organization/Types',
      set: 'organizationTypes',
      condition: !$scope.organizationTypes || !$scope.organizationTypes.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request for legal structures.
   * @function loadLegalStructures
   * @returns {Promise}
   **/
  function loadLegalStructures() {
    return $scope.load({
      url: '/Int/Application/Applicant/Legal/Structures',
      set: 'legalStructures',
      condition: !$scope.legalStructures || !$scope.legalStructures.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request for applicant data
   * @function loadApplicant
   * @returns {Promise}
   **/
  function loadApplicant() {
    return $scope.load({
      url: '/Int/Application/Applicant/' + $scope.parent.grantApplicationId,
      set: 'model'
    }).then(function () {
      return Promise.all([
        $scope.load({
          url: '/Int/Application/Applicant/Naics/1',
          set: 'applicantNAICS1',
          condition: !$scope.applicantNAICS1 || !$scope.applicantNAICS1.length
        }),
        $scope.load({
          url: '/Int/Application/Applicant/Naics/2/' + $scope.model.NAICSLevel1Id,
          set: 'applicantNAICS2'
        }),
        $scope.load({
          url: '/Int/Application/Applicant/Naics/3/' + $scope.model.NAICSLevel2Id,
          set: 'applicantNAICS3'
        }),
        $scope.load({
          url: '/Int/Application/Applicant/Naics/4/' + $scope.model.NAICSLevel3Id,
          set: 'applicantNAICS4'
        }),
        $scope.load({
          url: '/Int/Application/Applicant/Naics/5/' + $scope.model.NAICSLevel4Id,
          set: 'applicantNAICS5'
        })
      ]);
    })
  }

  /**
   * Initialize the data for the form.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadCountries(),
      loadProvinces(),
      loadOrganizationTypes(),
      loadLegalStructures(),
      loadApplicant()
    ]).catch(angular.noop);
  }

  /**
   * Make AJAX request to fetch the NAICS data for the specified level.
   * @function getNAICS
   * @param {any} level - The level of the NAICS data.
   * @returns {Promise}
   */
  $scope.getNAICS = function (level) {
    return $scope.load({
      url: '/Int/Application/Applicant/Naics/' + level + '/' + $scope.model['NAICSLevel' + (level - 1) + 'Id'],
      set: 'applicantNAICS' + level
    }).then(function () {
      return $timeout(function () {
        $scope.model['NAICSLevel' + (level) + 'Id'] = null;
        $scope.model['NAICSLevel' + (level + 1) + 'Id'] = null;
        $scope['applicantNAICS' + (level + 1)] = [];
      });
    }).catch (angular.noop);
  }

  /**
   * Get the NAICS label
   * @function naics
   * @returns {void}
   **/
  $scope.naics = function () {
    if ($scope.model.NAICSLevel5Id) {
      var result = $scope.applicantNAICS5.find(function (item) {
        return item.Key === $scope.model.NAICSLevel5Id;
      });
      return result ? result.Code + ' | ' + result.Value : '';
    } else if ($scope.model.NAICSLevel4Id) {
      var result = $scope.applicantNAICS4.find(function (item) {
        return item.Key === $scope.model.NAICSLevel4Id;
      });
      return result ? result.Code + ' | ' + result.Value : '';
    } else if ($scope.model.NAICSLevel3Id) {
      var result = $scope.applicantNAICS3.find(function (item) {
        return item.Key === $scope.model.NAICSLevel3Id;
      });
      return result ? result.Code + ' | ' + result.Value : '';
    } else if ($scope.model.NAICSLevel2Id) {
      var result = $scope.applicantNAICS2.find(function (item) {
        return item.Key === $scope.model.NAICSLevel2Id;
      });
      return result ? result.Code + ' | ' + result.Value : '';
    } else if ($scope.model.NAICSLevel1Id) {
      var result = $scope.applicantNAICS1.find(function (item) {
        return item.Key === $scope.model.NAICSLevel1Id;
      });
      return result ? result.Code + ' | ' + result.Value : '';
    }
  }
});
