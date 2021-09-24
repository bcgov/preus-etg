// ETG, Training program
app.controller('TrainingProgram', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'TrainingProgram',
    displayName: 'Training Program',
    save: {
      url: '/Int/Application/Training/Program',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.component.RowVersion;
    },
    onSave: function () {
      // Cannot refresh ETG training provider without learning the name(s) of the controller. 
      // Second best way to refresh: refresh screen. Commented code could be used if we want to go the other way.
      //$scope.resyncApplicationDetails();
      //$scope.component.RowVersion = $scope.model.RowVersion;
      //$scope.section.selectedCipsCode = getSelectedCipsCode();
      // Refresh ETG training provider control
      //$scope.emit('refresh', { target: ['ApplicationNotes','TrainingProvider'], force: true });
      window.location.reload();
    },
    onRefresh: function () {
      return loadTrainingProgram()
        .then(function () {
          initDeliveryMethods();
          initUnderRepresentedGroups();
        })
        .catch(angular.noop);
    },
    deliveryMethods: [],
    underRepresentedGroups: [],
    selectedCipsCode: null
  };
  if (typeof ($scope.CipsCode1) === 'undefined') $scope.CipsCode1 = [];

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for delivery methods
   * @function loadDeliveryMethods
   * @returns {Promise}
   **/
  function loadDeliveryMethods() {
    return $scope.load({
      url: '/Int/Application/Training/Program/Delivery/Methods',
      set: 'deliveryMethods',
      condition: !$scope.deliveryMethods || !$scope.deliveryMethods.length,
      localCache: true
    })
      .then(function () {
        $scope.section.deliveryMethods = angular.copy($scope.deliveryMethods); // Copy to local.
      });
  }

  /**
   * Make AJAX request for skill levels
   * @function loadSkillLevels
   * @returns {Promise}
   **/
  function loadSkillLevels() {
    return $scope.load({
      url: '/Int/Application/Training/Program/Skill/Levels',
      set: 'skillLevels',
      condition: !$scope.skillLevels || !$scope.skillLevels.length,
      localCache: true
    });
  }

  /**
   * Make AJAX requst for skill focuses
   * @function loadSkillFocuses
   * @returns {Promise}
   **/
  function loadSkillFocuses() {
    return $scope.load({
      url: '/Int/Application/Training/Program/Skill/Focuses',
      set: 'skillFocuses',
      condition: !$scope.skillFocuses || !$scope.skillFocuses.length,
      localCache: true
    })
  }

  /**
   * Make AJAX request for in-demand occupations
   * @function loadInDemandOccupations
   * @returns {Promise}
   **/
  function loadInDemandOccupations() {
    return $scope.load({
      url: '/Int/Application/Training/Program/In/Demand/Occupations',
      set: 'inDemandOccupations',
      condition: !$scope.inDemandOccupations || !$scope.inDemandOccupations.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request for training levels
   * @function loadTrainingLevels
   * @returns {Promise}
   * */
  function loadTrainingLevels() {
    return $scope.load({
      url: '/Int/Application/Training/Program/Training/Levels',
      set: 'trainingLevels',
      condition: !$scope.trainingLevels || !$scope.trainingLevels.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request for represented groups
   * @fucntion loadUnderRepresentedGroups
   * @returns {Promise}
   **/
  function loadUnderRepresentedGroups() {
    return $scope.load({
      url: '/Int/Application/Training/Program/Under/Represented/Groups',
      set: 'underRepresentedGroups',
      condition: !$scope.underRepresentedGroups || !$scope.underRepresentedGroups.length,
      localCache: true
    })
      .then(function () {
        $scope.section.underRepresentedGroups = angular.copy($scope.underRepresentedGroups); // Copy to local.
      });
  }

  /**
   * Make AJAX request for expected qualifications
   * @function loadExpectedQualifications
   * @returns {Promise}
   **/
  function loadExpectedQualifications() {
    return $scope.load({
      url: '/Int/Application/Training/Program/Expected/Qualifications',
      set: 'expectedQualifications',
      condition: !$scope.expectedQualifications || !$scope.expectedQualifications.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request for training program data
   * @function loadTrainingProgram
   * @returns {Promise}
   **/
  function loadTrainingProgram() {
    return $scope.load({
      url: '/Int/Application/Training/Program/' + $scope.component.Id,
      set: 'model'
    })
      .then(function (response) {
        return Promise.all([
          loadCipsCode(2, $scope.model.CipsCode1Id, 'programCipsCode2'),
          loadCipsCode(3, $scope.model.CipsCode2Id, 'programCipsCode3'),
        ])
          .then(function () {
            return $timeout(function () {
              $scope.section.selectedCipsCode = getSelectedCipsCode();
            });
          });
      });
  }

  /**
  * Make AJAX request and load CIPS Code data
  * @function loadCipsCode
  * @param {int} level - The CIPS Code level.
  * @param {int} [parentId] - The Id of the prior level.
  * @param {string} [set] - The scope level variable to set with the results.
  * @returns {Promise}
  **/
  function loadCipsCode(level, parentId, set) {
    if (level > 1 && !parentId) return Promise.resolve(); // No need to fetch the data.
    if (!level) level = 1;
    if (!set) set = 'CipsCode' + level;
    var url = '/Int/Application/Training/Program/CipsCodes/' + level + '/' + (parentId ? parentId : '');
    return $scope.load({
      url: url,
      set: set,
      condition: !$scope[set] || !$scope[set].length
    });
  }

  /**
   * Initialize the section data.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadDeliveryMethods(),
      loadSkillLevels(),
      loadSkillFocuses(),
      loadInDemandOccupations(),
      loadTrainingLevels(),
      loadUnderRepresentedGroups(),
      loadExpectedQualifications(),
      loadCipsCode(),
      loadTrainingProgram()
    ])
      .then(function () {
        initDeliveryMethods();
        initUnderRepresentedGroups();
      })
      .catch(angular.noop);
  }

  /**
 * Make an AJAX request to fetch the next Cips Code dropdown data for the selected parent Cips.
 * @function changeCipsCode
 * @param {any} level - The level to load.
 * @param {any} parentId - The parent CIPS Code Id.
 * @returns {Promise}
 */
  $scope.changeCipsCode = function (level, parentId) {
    switch (level) {
      case (2):
        $scope.model.CipsCode2Id = null;
        $scope.programCipsCode2 = [];
      case (3):
        $scope.model.CipsCode3Id = null;
        $scope.programCipsCode3 = [];
    }
    return loadCipsCode(level, parentId, 'programCipsCode' + level);
  }

  /**
   * Get the currently selected Cips code description.
   * @function getSelectedCipsCode
   * @returns {string}
   **/
  function getSelectedCipsCode() {
    if ($scope.model.CipsCode3Id) return getCaption($scope.programCipsCode3, $scope.model.CipsCode3Id);
    if ($scope.model.CipsCode2Id) return getCaption($scope.programCipsCode2, $scope.model.CipsCode2Id);
    if ($scope.model.CipsCode1Id) return getCaption($scope.CipsCode1, $scope.model.CipsCode1Id);
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
   * Initialize which delivery methods are selected.
   * @function initDeliveryMethods
   * @returns {void}
   **/
  function initDeliveryMethods() {
    $scope.section.deliveryMethods.forEach(function (item) {
      item.isChecked = $scope.model.SelectedDeliveryMethodIds.some(function (id) { return id === item.Key });
    });
  }

  /**
   * Initialize which under represented groups are selected.
   * @function initUnderRepresentedGroups
   * @returns {void}
   **/
  function initUnderRepresentedGroups() {
    $scope.section.underRepresentedGroups.forEach(function (item) {
      item.isChecked = $scope.model.SelectedUnderRepresentedGroupIds.some(function (id) { return id === item.Key });
    });
  }
});
