app.controller('TrainingProgramView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'TrainingProgramView',
    displayName: 'Training Program',
    save: {
      url: '/Ext/Training/Program',
      method: function () {
        return $scope.model.Id > 0 ? 'PUT' : 'POST';
      },
      data: function () {
        $scope.model.SelectedUnderRepresentedGroupIds = $scope.UnderRepresentedGroups.filter(function (item) { return item.IsSelected; }).map(function (item) { return item.Id; });
        $scope.model.SelectedDeliveryMethodIds = $scope.DeliveryMethods.filter(function (item) { return item.IsSelected; }).map(function (item) { return item.Id; });

        return $scope.model;
      }
    },
    onSave: function () {
      window.location = '/Ext/Application/Overview/View/' + $scope.model.GrantApplicationId;
    },
    grantApplicationId: $attrs.ngGrantApplicationId,
    trainingProgramId: $attrs.ngTrainingProgramId
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Initizalize the specified model property with the specified items.
   * @function initSelectedValues
   * @param {any} model - The model property to initialize.
   * @param {any} items - The items that were selected.
   * @returns {void}
   */
  function initSelectedValues(model, items) {
    if (!Array.isArray(model) || !Array.isArray(items)) return;
    items.forEach(function (item) {
      item.IsSelected = model.some(function (id) { return item.Id === id; });
    });
  }

  /**
   * Make an AJAX request to fetch the training program data.
   * @function loadTrainingProgram
   * @returns {Promise}
   **/
  function loadTrainingProgram() {
    return $scope.load({
      url: '/Ext/Training/Program/' + $scope.section.grantApplicationId + '/' + $scope.section.trainingProgramId,
      set: 'model'
    })
      .then(function () {
        return $timeout(function () {
          initSelectedValues($scope.model.SelectedUnderRepresentedGroupIds, $scope.UnderRepresentedGroups);
          initSelectedValues($scope.model.SelectedDeliveryMethodIds, $scope.DeliveryMethods);
        });
      });
  }

  /**
   * Make AJAX request to fetch dropdown list data.
   * @function loadDropdown
   * @param {string} url - The url path to the list data.
   * @param {string} target - The scope[target] property name.
   * @returns {Promise}
   */
  function loadDropdown(url, target) {
    return $scope.load({
      url: '/Ext/Training/Program' + url,
      set: target
    });
  }

  /**
   * Initialize the form data by making AJAX requests.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadDropdown('/Skill/Levels', 'SkillLevels'),
      loadDropdown('/Skills/Focuses', 'SkillFocus'),
      loadDropdown('/Expected/Qualifications', 'ExpectedQualifications'),
      loadDropdown('/Indemand/Occupations', 'InDemandOccupations'),
      loadDropdown('/Training/Levels', 'TrainingLevels'),
      loadDropdown('/Underrepresented/Groups', 'UnderRepresentedGroups'),
      loadDropdown('/Delivery/Methods', 'DeliveryMethods')
    ])
      .then(function () {
        return loadTrainingProgram();
      })
      .catch(angular.noop);
  }

  init();
});
