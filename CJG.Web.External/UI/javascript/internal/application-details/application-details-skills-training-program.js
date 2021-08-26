app.controller('SkillsTrainingProgram', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'SkillsTrainingProgram-' + $scope.program.Id,
    displayName: 'Skills Training Program',
    save: {
      url: '/Int/Application/Skills/Training/Program',
      method: 'PUT',
      data: function () {
        $scope.model.SelectedDeliveryMethodIds = $scope.section.deliveryMethods
          .filter(function (item) { return item.Selected; })
          .map(function (item) {
            return item.Key;
          });

        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.program.RowVersion;
    },
    onSave: function () {
      setServiceLineBreakdownCaption();
      $scope.resyncApplicationDetails();
      $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
    },
    onRefresh: function () {
      return loadTrainingProgram()
        .then(function () {
          setServiceLineBreakdownCaption();
          initDeliveryMethods();
        })
        .catch(angular.noop);
    },
    serviceLines: [],
    serviceLineBreakdowns: [],
    deliveryMethods: []
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
   * Make AJAX request for service lines for the specified expense type.
   * @function loadServiceLines
   * @param {int} eligibleExpenseTypeId - The eligible expense type id.
   * @returns {Promise}
   **/
  function loadServiceLines(eligibleExpenseTypeId) {
    return $scope.load({
      url: '/Int/Application/Skills/Training/Service/Lines/' + eligibleExpenseTypeId,
      set: 'section.serviceLines'
    });
  }

  /**
   * Make AJAX request for service line breakdowns for the specified service line.
   * @function loadServiceLineBreakdowns
   * @param {int} serviceLineId - The service line id.
   * @returns {Promise}
   **/
  function loadServiceLineBreakdowns(serviceLineId) {
    return $scope.load({
      url: '/Int/Application/Skills/Training/Service/Line/Breakdowns/' + serviceLineId,
      set: 'section.serviceLineBreakdowns'
    });
  }

  /**
   * Make AJAX request for training program data
   * @function loadTrainingProgram
   * @returns {Promise}
   **/
  function loadTrainingProgram() {
    return $scope.load({
      url: '/Int/Application/Skills/Training/Program/' + $scope.program.Id,
      set: 'model'
    })
      .then(function () {
        return $scope.loadServiceLineBreakdowns();
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
      loadTrainingLevels(),
      loadExpectedQualifications(),
      loadServiceLines($scope.component.EligibleExpenseTypeId),
      loadTrainingProgram()
    ])
      .then(function () {
        setServiceLineBreakdownCaption();
        initDeliveryMethods();
      })
      .catch(angular.noop);
  }

  /**
   * Based on the selected service line, set the service line breakdown caption.
   * @function setServiceLineBreakdownCaption
   * @returns {void}
   **/
  function setServiceLineBreakdownCaption() {
    var serviceLine = $scope.section.serviceLines.find(function (item) { return item.Id === $scope.model.ServiceLineId; });
    if (serviceLine) $scope.model.ServiceLineBreakdownCaption = serviceLine.BreakdownCaption;
  }

  /**
   * When the service line is changed load the child service line breakdowns
   * @function loadServiceLineBreakdowns
   * @returns {Promise}
   **/
  $scope.loadServiceLineBreakdowns = function () {
    setServiceLineBreakdownCaption();
    if ($scope.model.ServiceLineId) {
      return loadServiceLineBreakdowns($scope.model.ServiceLineId).catch(angular.noop);
    }

    return Promise.resolve();
  }

  /**
   * Make an AJAX request and update the program's eligibility.
   * @function changeEligibility
   * @param {any} $event - The angular event.
   * @returns {Promise}
   */
  $scope.changeEligiblity = function ($event) {
    $event.stopPropagation();
    return $scope.load({
      url: '/Int/Application/Skills/Training/Eligibility/' + $scope.program.Id + '?rowVersion=' + encodeURIComponent($scope.model.EligibleCostBreakdownRowVersion),
      method: 'PUT',
      set: 'model'
    })
      .then(function () {
        $scope.section.onSave();
        $scope.emit('refresh', { target: 'ProgramCosts', force: true });
      })
      .catch(function () {
        return $timeout(function () {
          $scope.program.IsEligible = !$scope.program.IsEligible; // Undo because the update failed.
        });
      });
  }

  /**
   * Initialize which delivery methods are selected.
   * @function initDeliveryMethods
   * @returns {void}
   **/
  function initDeliveryMethods() {
    $scope.section.deliveryMethods.forEach(function (item) {
      item.Selected = $scope.model.SelectedDeliveryMethodIds.some(function (id) { return id === item.Key });
    });
  }

  /**
   * Make AJAX request and delete the specified skills training component.
   * @function removeComponent
   * @returns {Promise}
   **/
  $scope.removeComponent = function ($event) {
    $event.preventDefault();
    $event.stopPropagation();
    return $scope.confirmDialog('Remove Component', 'Do you want to remove the "' + $scope.model.CourseTitle + '" skills training component?')
      .then(function () {
        return $scope.ajax({
          url: '/Int/Application/Skills/Training/Component/Delete/' + $scope.model.Id + '?rowVersion=' + encodeURIComponent($scope.model.RowVersion),
          method: 'PUT'
        })
          .then(function () {
            $scope.resyncApplicationDetails();
            $scope.emit('refresh', { target: 'ApplicationNotes,ProgramCosts', force: true });
          })
      })
      .catch(angular.noop);
  }
});
