app.controller('Prioritization', function ($scope, $attrs, $controller, $element, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'Prioritization',
    displayName: 'Prioritization',
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.grantFile.RowVersion;
    },
    onRefresh: function () {
      return loadPrioritizationInfo().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load the application prioritization data.
   * @function loadApplicantContact
   * @returns {Promise}
   **/
  function loadPrioritizationInfo() {
    return $scope.load({
      url: '/Int/Application/Prioritization/' + $scope.parent.grantApplicationId,
      set: 'model'
    });
  }
  
  $scope.recalculatePrioritization = function () {
    return $scope.confirmDialog('Recalculate Application Prioritization Score', 'Are you sure you wish to recalculate the Prioritization score for this application?')
      .then(function () {
        return $scope.ajax({
          url: '/Int/Application/Prioritization/Recalculate/' + $scope.parent.grantApplicationId,
          method: 'PUT'
        });
      })
      .then(function (response) {
        window.location.reload();
      //  if (response.data.RedirectUrl)
      //    window.location = response.data.RedirectUrl;
      })
      .catch(angular.noop);
  }

  /**
   * Initialize the section and load the data.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadPrioritizationInfo()
    ]).catch(angular.noop);
  }
});
