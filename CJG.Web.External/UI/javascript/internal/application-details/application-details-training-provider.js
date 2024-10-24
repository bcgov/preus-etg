app.controller('TrainingProvider', function($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: $attrs.name || 'TrainingProvider',
    displayName: 'Training Provider',
    save: {
      url: '/Int/Application/Training/Provider',
      method: 'PUT',
      dataType: 'file',
      data: function() {
        var files = [];
        //if ($scope.model.TrainingProviderType.PrivateSectorValidationType) {
        if ($scope.model.CourseOutlineDocument && $scope.model.CourseOutlineDocument.File) {
          files.push($scope.model.CourseOutlineDocument.File);
          $scope.model.CourseOutlineDocument.Index = files.length - 1;
        }
        if ($scope.model.ProofOfQualificationsDocument && $scope.model.ProofOfQualificationsDocument.File) {
          files.push($scope.model.ProofOfQualificationsDocument.File);
          $scope.model.ProofOfQualificationsDocument.Index = files.length - 1;
        }
        //}
        if ($scope.model.TrainingOutsideBcListViewModel.TrainingOutsideBC &&
          $scope.model.BusinessCaseDocument &&
          $scope.model.BusinessCaseDocument.File) {
          files.push($scope.model.BusinessCaseDocument.File);
          $scope.model.BusinessCaseDocument.Index = files.length - 1;
        }

        return {
          files: files,
          provider: angular.toJson($scope.model)
        };
      },
      backup: true
    },
    loaded: function() {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.provider.RowVersion;
    },
    onSave: function() {
      $scope.resyncApplicationDetails();
      $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
    },
    onValidate: function(response) {
      return $timeout(function() {
        $scope.model = response.data;
        $scope.section.onSave();
      });
    },
    onRefresh: function() {
      return loadTrainingProvider().catch(angular.noop);
    }
};

  angular.extend(this, $controller('TrainingProviderBase', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load training provider data
   * @function loadTrainingProvider
   * @returns {Promise}
   **/
  function loadTrainingProvider() {
    return $scope.load({
      url: '/Int/Application/Training/Provider/' + $scope.provider.Id,
      set: 'model'
    });
  }

  /**
   * Make AJAX request to load training provider extra info data
   * @function loadTrainingProviderExtraInfo
   * @returns {Promise}
   **/
  function loadTrainingProviderExtraInfo() {
    return $scope.load({
      url: '/Int/Application/Training/Provider/ExtraInfo/' + $scope.provider.Id,
      set: 'extraInfo'
    });
  }

  function loadTrainingProviderTypesWithInactive() {
    return $scope.load({
      url: '/Int/Application/Training/Provider/TypesAll/' + $scope.provider.Id,
      set: 'trainingProviderTypes',
      condition: !$scope.trainingProviderTypes || !$scope.trainingProviderTypes.length,
      overwrite: true,
      localCache: false
    });
  }

  function loadProviderTypeDetailsWithInactiveOptions() {
    return $scope.load({
      url: '/Int/Application/Training/Provider/Types/Details/' + $scope.provider.Id,
      set: 'ProviderTypesDetails',
      condition: !$scope.trainingProviderTypes || !$scope.trainingProviderTypes.length,
      overwrite: true,
      localCache: false
    });
  }

  /**
   * Initialize the form data
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      $scope.loadCountries(),
      $scope.loadProvinces(),
      //$scope.loadProviderTypesDetails(),
      loadTrainingProviderTypesWithInactive(),
      loadProviderTypeDetailsWithInactiveOptions(),
      loadTrainingProviderExtraInfo(),
      loadTrainingProvider()
    ]).catch(angular.noop);
  }

  /**
  * Change training provider private sector Type
  * @function trainingProviderTypeChange
  * @param {any} trainingProviderTypeId - The trainingProviderType id.
  * @returns {void}
  */
  $scope.setTrainingProviderTypeDetails = function () {
    $scope.model.TrainingProviderType.ProofOfInstructorQualifications = $scope.ProviderTypesDetails.find(function (element) {
      return element.Id == $scope.model.TrainingProviderType.Id;
    }).ProofOfInstructorQualifications;
    $scope.model.TrainingProviderType.CourseOutline = $scope.ProviderTypesDetails.find(function (element) {
      return element.Id == $scope.model.TrainingProviderType.Id;
    }).CourseOutline;
    $scope.model.TrainingProviderType.PrivateSectorValidationType = $scope.ProviderTypesDetails.find(function (element) {
      return element.Id == $scope.model.TrainingProviderType.Id;
    }).PrivateSectorValidationType;
  }

  $scope.previewTrainingOptions = function() {
    return ngDialog.open({
      template: '/content/dialogs/_FullContent.html',
      closeByDocument: true,
        data: {
          title: 'Alternative Training Options',
          content: $scope.extraInfo.AlternativeTrainingOptions
        }
      }
    );
  }

  $scope.previewChoiceOfTrainer = function () {
    return ngDialog.open({
      template: '/content/dialogs/_FullContent.html',
      closeByDocument: true,
        data: {
          title: 'Choice of Training Provider',
          content: $scope.extraInfo.ChoiceOfTrainerOrProgram
        }
      }
    );
  }
});
