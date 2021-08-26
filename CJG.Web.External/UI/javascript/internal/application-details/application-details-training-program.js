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
      $scope.component.RowVersion = $scope.model.RowVersion;
      $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
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
    underRepresentedGroups: []
  };

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
      loadTrainingProgram()
    ])
      .then(function () {
        initDeliveryMethods();
        initUnderRepresentedGroups();
      })
      .catch(angular.noop);
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
