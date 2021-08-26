app.filter('searchCommunities', function () {
  return function (items, selectIds, searchValue) {
    if (!items) return null;
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

app.controller('ProgramDescriptionView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId,
    save: {
      url: '/Ext/Application/Program/Description',
      method: 'POST',
      data: function () {
        $scope.model.SelectedUnderRepresentedPopulationIds = $scope.model.UnderRepresentedPopulations.filter(function (item) { return item.Selected; }).map(function (item) { return item.Key; });
        $scope.model.SelectedVulnerableGroupIds = $scope.model.VulnerableGroups.filter(function (item) { return item.Selected; }).map(function (item) { return item.Key; });
        return $scope.model;
      }
    },
    onSave: function (event, data) {
      if (data.response.data.RedirectURL) window.location = data.response.data.RedirectURL;
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to fetch program description data.
   * @function loadProgramDescription
   * @returns {Promise}
   **/
  function loadProgramDescription() {
    return $scope.load({
      url: '/Ext/Application/Program/Description/' + $scope.section.grantApplicationId,
      set: 'model'
    })
      .then(function (response) {
        return $timeout(function () {
          $scope.OriginalNumberOfParticipants = response.data.NumberOfParticipants;
          $scope.model.UnderRepresentedPopulations.forEach(function (item) {
            item.Selected = $scope.model.SelectedUnderRepresentedPopulationIds.some(function (id) { return item.Key === id; });
          });
          $scope.model.VulnerableGroups.forEach(function (item) {
            item.Selected = $scope.model.SelectedVulnerableGroupIds.some(function (id) { return item.Key === id; });
          });
          $scope.modelLoaded = true;
        });
      })
      .catch(angular.noop);
  }

  /**
   * Check if the participant cost limits have been exceed.
   * Make AJAX request if required to test.
   * @function checkLimit
   * @returns {Promise}
   **/
   function checkLimit() {
    return $scope.load({
      url: '/Ext/Application/Program/Description/Training/Max/Cost/' + $scope.model.Id + '/' + $scope.model.NumberOfParticipants,
      set: 'model.costCheck'
    })
      .then(function (response) {
        if ($scope.model.costCheck.CostExceeded == true) {
          return $scope.confirmDialog('Program Description Confirmation', '<p>Lowering the number of participants results in exceeding the per participant cost limits.  If you want to proceed with a lower number of participants, your costs will be reset.</p>')
            .then(function () {
              return $scope.save();
            });
        } else {
          return $scope.save();
        }
      })
      .catch(angular.noop);
  }

  /**
   * Check if the participant cost limits have been exceed and save the program descriptions.
   * @function submit
   * @returns {Promise}
   **/
  $scope.submit = function () {
    if ($scope.model.NumberOfParticipants && $scope.model.NumberOfParticipants < $scope.OriginalNumberOfParticipants) {
      return checkLimit();
    } else {
      return $scope.save();
    }
  }

  /**
   * Initialize the data for this form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return loadProgramDescription()
      .catch(angular.noop);
  }

  init();
});
