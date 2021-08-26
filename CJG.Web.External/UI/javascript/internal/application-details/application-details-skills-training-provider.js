app.controller('SkillsTrainingProvider', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'SkillsTrainingProvider-' + $scope.provider.Id,
    displayName: 'Skills Training Provider',
    save: {
      url: '/Int/Application/Skills/Training/Provider',
      method: 'PUT',
      dataType: 'file',
      data: function () {
        var files = [];
        if ($scope.model.PrivateSectorValidationType) {
          if ($scope.model.CourseOutlineDocument && $scope.model.CourseOutlineDocument.File) {
            files.push($scope.model.CourseOutlineDocument.File);
            $scope.model.CourseOutlineDocument.Index = files.length - 1;
          }
          if ($scope.model.ProofOfQualificationsDocument && $scope.model.ProofOfQualificationsDocument.File) {
            files.push($scope.model.ProofOfQualificationsDocument.File);
            $scope.model.ProofOfQualificationsDocument.Index = files.length - 1;
          }
        }
        if ($scope.model.TrainingOutsideBC && $scope.model.BusinessCaseDocument && $scope.model.BusinessCaseDocument.File) {
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
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.provider.RowVersion;
    },
    onSave: function () {
      $scope.resyncApplicationDetails();
      $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
    },
    onValidate: function () {
      $scope.section.onSave();
      $scope.section.onRefresh();
    },
    onRefresh: function () {
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
      url: '/Int/Application/Skills/Training/Provider/' + $scope.provider.Id,
      set: 'model'
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
      $scope.loadTrainingProviderTypes(),
      loadTrainingProvider()
    ]).catch(angular.noop);
  }

  /**
   * Make AJAX request to approve the training provider
   * @function approveProvider
   * @returns {Promise}
   **/
  $scope.approveProvider = function () {
    return $scope.load({
      url: '/Int/Application/Skills/Training/Provider/Approve',
      method: 'PUT',
      data: $scope.model,
      set: 'model'
    })
      .then(function () {
        $scope.resyncApplicationDetails();
        $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
      })
      .catch(angular.noop);
  }

  /**
   * Make AJAX request to deny the training provider
   * @function denyProvider
   * @returns {Promise}
   **/
  $scope.denyProvider = function () {
    return $scope.load({
      url: '/Int/Application/Skills/Training/Provider/Deny',
      method: 'PUT',
      data: $scope.model,
      set: 'model'
    })
      .then(function () {
        $scope.resyncApplicationDetails();
        $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
      })
      .catch(angular.noop);
  }

  /**
  * Change training provider private sector Type
  * @function trainingProviderTypeChange
  * @param {any} trainingProviderTypeId - The trainingProviderType id.
  * @returns {void}
  */
  $scope.trainingProviderTypeChange = function (trainingProviderTypeId) {
    var trainingProviderSelected = $scope.getTrainingProviderSelected(trainingProviderTypeId);
    if (trainingProviderSelected === null) {
      return;
    }
    $scope.model.PrivateSectorValidationType = trainingProviderSelected.Parent;
  }
});
