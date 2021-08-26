app.controller('ProgramDescription', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'ProgramDescription',
    displayName: 'Program Description',
    save: {
      url: '/Int/Application/Program/Description',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion;
    },
    onSave: function () {
      $scope.section.selectedNOC = getSelectedNOC();
      $scope.section.selectedNAICS = getSelectedNAICS();
      $scope.section.selectedCommunities = getSelectedCommunities();
      $scope.emit('update', { grantFile: { RowVersion: $scope.model.RowVersion } });
      $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
    },
    onRefresh: function () {
      return loadProgramDescription().catch(angular.noop);
    },
    selectedCommunities: [],
    selectedNOC: null,
    selectedNAICS: null
  };
  if (typeof ($scope.applicantOrganizationTypes) === 'undefined') $scope.applicantOrganizationTypes = [];
  if (typeof ($scope.participantEmploymentStatuses) === 'undefined') $scope.participantEmploymentStatuses = [];
  if (typeof ($scope.underRepresentedPopulations) === 'undefined') $scope.underRepresentedPopulations = [];
  if (typeof ($scope.vulnerableGroups) === 'undefined') $scope.vulnerableGroups = [];
  if (typeof ($scope.naics1) === 'undefined') $scope.naics1 = [];
  if (typeof ($scope.nocs1) === 'undefined') $scope.nocs1 = [];
  if (typeof ($scope.communities) === 'undefined') $scope.communities = [];

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to fetch applicant organization types.
   * @function loadApplicantOrganizationTypes
   * @returns {Promise}
   **/
  function loadApplicantOrganizationTypes() {
    return $scope.load({
      url: '/Int/Application/Program/Description/Applicant/Organization/Types',
      set: 'applicantOrganizationTypes',
      condition: !$scope.applicantOrganizationTypes || !$scope.applicantOrganizationTypes.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request to fetch participant employment statuses.
   * @function loadParticipantEmploymentStatuses
   * @returns {Promise}
   **/
  function loadParticipantEmploymentStatuses() {
    return $scope.load({
      url: '/Int/Application/Program/Description/Participant/Employment/Statuses',
      set: 'participantEmploymentStatuses',
      condition: !$scope.participantEmploymentStatuses || !$scope.participantEmploymentStatuses.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request to fetch under represented populations.
   * @function loadUnderRepresentedPopulations
   * @returns {Promise}
   **/
  function loadUnderRepresentedPopulations() {
    return $scope.load({
      url: '/Int/Application/Program/Description/Under/Represented/Populations',
      set: 'underRepresentedPopulations',
      condition: !$scope.underRepresentedPopulations || !$scope.underRepresentedPopulations.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request to fetch vulnerable groups.
   * @function loadVulnerableGroups
   * @returns {Promise}
   **/
  function loadVulnerableGroups() {
    return $scope.load({
      url: '/Int/Application/Program/Description/Vulnerable/Groups',
      set: 'vulnerableGroups',
      condition: !$scope.vulnerableGroups || !$scope.vulnerableGroups.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request and load NAICS data
   * @function loadNAICS
   * @param {int} level - The NAICS level.
   * @param {int} [parentId] - The Id of the prior level.
   * @param {string} [set] - The scope level variable to set with the results.
   * @returns {Promise}
   **/
  function loadNAICS(level, parentId, set) {
    if (level > 1 && !parentId) return Promise.resolve(); // No need to fetch the data.
    if (!level) level = 1;
    if (!set) set = 'naics' + level;
    var url = '/Int/Application/Program/Description/NAICS/' + level + '/' + (parentId ? parentId : '');
    return $scope.load({
      url: url,
      set: set,
      condition: !$scope[set] || !$scope[set].length
    });
  }

  /**
   * Make AJAX request and load NOCs data
   * @function loadNOCs
   * @param {int} level - The NOC level.
   * @param {int} [parentId] - The Id of the prior level.
   * @param {string} [set] - The scope level variable to set with the results.
   * @returns {Promise}
   **/
  function loadNOCs(level, parentId, set) {
    if (level > 1 && !parentId) return Promise.resolve(); // No need to fetch the data.
    if (!level) level = 1;
    if (!set) set = 'nocs' + level;
    var url = '/Int/Application/Program/Description/NOCs/' + level + '/' + (parentId ? parentId : '');
    return $scope.load({
      url: url,
      set: set,
      condition: !$scope[set] || !$scope[set].length
    });
  }

  /**
   * Make AJAX request to fetch communities.
   * @function loadCommunities
   * @returns {Promise}
   **/
  function loadCommunities() {
    return $scope.load({
      url: '/Int/Application/Applicant/Communities',
      set: 'communities',
      condition: !$scope.communities || !$scope.communities.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request and load program description data
   * @function loadProgramDescription
   * @returns {Promise}
   **/
  function loadProgramDescription() {
    return $scope.load({
      url: '/Int/Application/Program/Description/' + $scope.parent.grantApplicationId,
      set: 'model'
    })
      .then(function (response) {
        return Promise.all([
          loadNAICS(2, $scope.model.Naics1Id, 'programNAICS2'),
          loadNAICS(3, $scope.model.Naics2Id, 'programNAICS3'),
          loadNAICS(4, $scope.model.Naics3Id, 'programNAICS4'),
          loadNAICS(5, $scope.model.Naics4Id, 'programNAICS5'),
          loadNOCs(2, $scope.model.Noc1Id, 'programNOCs2'),
          loadNOCs(3, $scope.model.Noc2Id, 'programNOCs3'),
          loadNOCs(4, $scope.model.Noc3Id, 'programNOCs4'),
        ])
          .then(function () {
            return $timeout(function () {
              $scope.section.selectedNOC = getSelectedNOC();
              $scope.section.selectedNAICS = getSelectedNAICS();
            });
          });
      });
  }

  /**
   * Initialize section data.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadApplicantOrganizationTypes(),
      loadParticipantEmploymentStatuses(),
      loadUnderRepresentedPopulations(),
      loadVulnerableGroups(),
      loadCommunities(),
      loadNAICS(),
      loadNOCs(),
      loadProgramDescription()
    ])
      .then(function () {
        return $timeout(function () {
          $scope.section.selectedCommunities = getSelectedCommunities();
        });
      })
      .catch(angular.noop);
  }

  /**
   * Make an AJAX request to fetch the next NAICS dropdown data for the selected parent NAICS.
   * @function changeNAICS
   * @param {any} level - The level to load.
   * @param {any} parentId - The parent NAICS Id.
   * @returns {Promise}
   */
  $scope.changeNAICS = function (level, parentId) {
    switch (level) {
      case (2):
        $scope.model.Naics2Id = null;
        $scope.programNAICS2 = [];
      case (3):
        $scope.model.Naics3Id = null;
        $scope.programNAICS3 = [];
      case (4):
        $scope.model.Naics4Id = null;
        $scope.programNAICS4 = [];
      case (5):
        $scope.model.Naics5Id = null;
        $scope.programNAICS5 = [];
    }
    return loadNAICS(level, parentId, 'programNAICS' + level);
  }

  /**
   * Get the currently selected NAICS code caption.
   * @function getSelectedNAICS
   * @returns {string}
   **/
  function getSelectedNAICS () {
    if ($scope.model.Naics5Id) return getCaption($scope.programNAICS5, $scope.model.Naics5Id);
    if ($scope.model.Naics4Id) return getCaption($scope.programNAICS4, $scope.model.Naics4Id);
    if ($scope.model.Naics3Id) return getCaption($scope.programNAICS3, $scope.model.Naics3Id);
    if ($scope.model.Naics2Id) return getCaption($scope.programNAICS2, $scope.model.Naics2Id);
    if ($scope.model.Naics1Id) return getCaption($scope.naics1, $scope.model.Naics1Id);
  }

  /**
   * Make an AJAX request to fetch the next NOC dropdown data for the selected parent NOC.
   * @function changeNAICS
   * @param {any} level - The level to load.
   * @param {any} parentId - The parent NOC Id.
   * @returns {Promise}
   */
  $scope.changeNOC = function (level, parentId) {
    switch (level) {
      case (2):
        $scope.model.Noc2Id = null;
        $scope.programNOCs2 = [];
      case (3):
        $scope.model.Noc3Id = null;
        $scope.programNOCs3 = [];
      case (4):
        $scope.model.Noc4Id = null;
        $scope.programNOCs4 = [];
    }
    return loadNOCs(level, parentId, 'programNOCs' + level);
  }

  /**
   * Get the currently selected NOC code caption.
   * @function getSelectedNOC
   * @returns {string}
   **/
  function getSelectedNOC () {
    if ($scope.model.Noc4Id) return getCaption($scope.programNOCs4, $scope.model.Noc4Id);
    if ($scope.model.Noc3Id) return getCaption($scope.programNOCs3, $scope.model.Noc3Id);
    if ($scope.model.Noc2Id) return getCaption($scope.programNOCs2, $scope.model.Noc2Id);
    if ($scope.model.Noc1Id) getCaption($scope.nocs1, $scope.model.Noc1Id);
  }

  /**
   * Find the item in the array and return the caption.
   * @param {Array} items - An array of items.
   * @param {int} key - The key to search for.
   * @returns {string}
   */
  function getCaption(items, key) {
    if (!items || !items.length) return '';
    var item = items.find(function (item) { return item.Key === key; });
    return item ? item.Value : '';
  }

  /**
   * Returns an array of the community names that have been selected.
   * @function getSelectedCommunities
   * @returns {Array}
   **/
  function getSelectedCommunities() {
    if (!$scope.model.SelectedCommunityIds || !$scope.model.SelectedCommunityIds.length || !$scope.communities.length) return;
    return $scope.communities.filter(function (item) {
      return $scope.model.SelectedCommunityIds.some(function (key) {
        return item.Key === key;
      });
    }).map(function (item) {
        return item.Value;
      });
  }
});

app.filter('searchCommunities', function () {
  return function (items, selectIds, searchValue) {
    var filtered = [];
    var selectIdsArray = Object.keys(selectIds).map(function (e) {
      return selectIds[e]
    })
    angular.forEach(items, function (item) {
      if (selectIdsArray.indexOf(item.Key) > -1 ||
        searchValue === undefined || item.Value.toLowerCase().includes(searchValue.toLowerCase())) {
        filtered.push(item);
      }
    });
    return filtered;
  }
});
