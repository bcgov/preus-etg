app.controller('GrantSelectionView', function ($scope, $attrs, $controller, $sce, $timeout, Utils, ngDialog) {
  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId,
    grantProgramId: $attrs.ngGrantProgramId,
    seedGrantApplicationId: $attrs.ngSeedGrantApplicationId,
    save: {
      url: '/Ext/Application',
      method: function () {
        return $scope.model.GrantApplicationId ? 'PUT' : 'POST';
      },
      data: function () {
        return $scope.model;
      },
      set: 'model'
    },
    onSave: function (event, data) {
      if (data.response.data.RedirectURL) window.location = data.response.data.RedirectURL;
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to fetch the grant openings.
   * @function loadGrantSelections
   * @returns {Promise}
   **/
  function loadGrantSelections() {
    return $scope.load({
      url: '/Ext/Application/Grant/Selection/' + $scope.section.grantApplicationId + '/' + $scope.section.grantProgramId + '/' + $scope.section.seedGrantApplicationId,
      set: 'model'
    });
  }

  /**
   * Iniitalize the form data.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return loadGrantSelections()
      .then(function (response) {
        return $timeout(function () {
          for (var i = 0; i < $scope.model.TrainingPeriods.length; i++) {
            var period = $scope.model.TrainingPeriods[i];
            for (var j = 0; j < period.GrantOpenings.length; j++) {
              var item = period.GrantOpenings[j];
              item.GrantStream.ObjectiveHTML = $sce.trustAsHtml(item.GrantStream.Objective);
            }
          }
          $scope.showApplicationForm = $scope.model.TrainingPeriods && $scope.model.TrainingPeriods.length > 0;
          $scope.isNewApplication = ($scope.model.GrantApplicationId === 0);
          $scope.isDuplication = ($scope.model.SeedGrantApplicationId > 0);
        })
      })
      .catch(angular.noop);
  }

  /**
   * Find the currently selected grant opening in the array.
   * @function getCurrentTrainingPeriod
   * @returns {object}
   **/
  function getCurrentTrainingPeriod() {
    for (var i = 0; i < $scope.model.TrainingPeriods.length; i++) {
      var period = $scope.model.TrainingPeriods[i];
      for (var j = 0; j < period.GrantOpenings.length; j++) {
        var item = period.GrantOpenings[j];
        if ($scope.model.GrantOpeningId === item.Id) {
          return period;
        }
      }
    }
  }

  /**
   * Return the training period name.
   * @function getDescription
   * @returns {string}
   **/
  $scope.getDescription = function () {
    if ($scope.model.GrantOpeningId && ($scope.currentTrainingPeriod || ($scope.currentTrainingPeriod = getCurrentTrainingPeriod())))
      return $scope.currentTrainingPeriod.Description;
    return null;
  }

  /**
   * Select the specified grant openings.
   * Make an AJAX request to fetch the grant stream requirements.
   * @function getRequirements
   * @param {any} id - The grant opening id.
   * @returns {Promise}
   */
  $scope.getRequirements = function (id) {
    $scope.model.GrantOpeningId = id;
    $scope.currentTrainingPeriod = getCurrentTrainingPeriod();
    $scope.model.GrantStream = null;  // Required: If model not cleared, the <validation> element for eligibility questions keeps the old ng-model ID and the errors do not display
    return $scope.load({
      url: '/Ext/Application/Grant/Stream/Eligibility/Requirements/' + $scope.model.GrantOpeningId,
      set: 'model.GrantStream'
    })
      .catch(angular.noop);
  }
});
