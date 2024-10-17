app.controller('EmploymentProvider', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'EmploymentProvider-' + $scope.provider.Id,
    displayName: 'Employment Provider',
    ProviderTypesDetails: [],
    save: {
      url: '/Int/Application/ESS/Provider',
      method: function () {
        return $scope.model.Id ? 'PUT' : 'POST';
      },
      dataType: 'file',
      data: function () {
        $scope.model.CountryId = 'CA';
        $scope.model.Region = '';
        $scope.model.GrantApplicationId = $scope.parent.grantApplicationId;
        $scope.model.EligibleCostId = $scope.component.Id;
        $scope.model.IsCanadianAddress = true;
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
      return loadProvider().catch(angular.noop);
    },
    onCancel: function () {
      if (!$scope.model || Utils.objectIsEmpty($scope.model) || !$scope.model.Id) {
        return $scope.removeProvider();
      }
    }
  };

  angular.extend(this, $controller('TrainingProviderBase', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load provider data
   * @function loadProvider
   * @returns {Promise}
   **/
  function loadProvider() {
    if ($scope.provider.Id === 0) return Promise.resolve();
    return $scope.load({
      url: '/Int/Application/ESS/Provider/' + $scope.provider.Id,
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
      $scope.loadProviderTypesDetails(),
      loadProvider()
    ]).catch(angular.noop);
  }

  /**
   * Confirm whether the provider should be removed.
   * Make AJAX request to remove provider.
   * @function removeProvider
   * @returns {Promise}
   **/
  $scope.removeProvider = function () {
    if (!$scope.model.Id) {
      $scope.component.Providers.splice($attrs.providerIndex, 1); // Delete this provider from the page.
      $scope.component.CanAdd = true;
      return Promise.resolve();
    }

    return ngDialog.openConfirm({
      template: '/content/dialogs/_Confirmation.html',
      data: {
        title: 'Remove Provider',
        question: 'Do you want to remove this provider "' + $scope.model.Name + '" from the grant file?'
      }
    }).then(function () {
      return $scope.ajax({
        url: '/Int/Application/ESS/Provider/Delete/' + $scope.model.Id + '?rowVersion=' + encodeURIComponent($scope.model.RowVersion),
        method: 'PUT'
      }).then(function () {
        return $timeout(function () {
          $scope.component.Providers.splice($attrs.providerIndex, 1); // Delete this provider from the page.
        });
      })
    }).catch(angular.noop);
  }

  /**
   * Make AJAX request to approve the training provider
   * @function approveProvider
   * @returns {Promise}
   **/
  $scope.approveProvider = function () {
    return $scope.load({
      url: '/Int/Application/ESS/Provider/Approve',
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
      url: '/Int/Application/ESS/Provider/Deny',
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

  $scope.loadProviderTypesDetails = function (loadProviderTypesDetails) {
    return $scope.load({
      url: '/Int/Application/Training/Provider/Types/Details',
      set: 'ProviderTypesDetails',
      condition: !$scope.ProviderTypesDetails || !$scope.ProviderTypesDetails.length,
      overwrite: false,
      localCache: true
    });
  }

  /**
* Change training provider private sector Type
* @function trainingProviderTypeChange
* @param {any} trainingProviderTypeId - The trainingProviderType id.
* @returns {void}
*/
  $scope.setTrainingProviderTypeDetails = function () {
    $scope.model.CourseOutline = $scope.ProviderTypesDetails.find(function (element) {
      return element.Id == $scope.model.TrainingProviderTypeId;
    }).CourseOutline;

    $scope.model.ProofOfInstructorQualifications = $scope.ProviderTypesDetails.find(function (element) {
      return element.Id == $scope.model.TrainingProviderTypeId;
    }).ProofOfInstructorQualifications;

    $scope.model.PrivateSectorValidationType = $scope.ProviderTypesDetails.find(function (element) {
      return element.Id == $scope.model.TrainingProviderTypeId;
    }).PrivateSectorValidationType;
  }
});
