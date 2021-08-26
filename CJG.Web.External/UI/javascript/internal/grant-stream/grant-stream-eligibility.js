app.controller('GrantStreamEligibility', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'GrantStreamEligibility',
    save: {
      url: '/Int/Admin/Grant/Stream/Eligibility',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.section.isLoaded;
    },
    onSave: function () {
      $scope.emit('refresh');
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.$watch('$parent.model', function (newValue, oldValue) {
    $scope.model = newValue;
  });

  /**
   * Open a new tab and display the message.
   * @function preview
   * @returns {Promise}
   **/
  $scope.preview = function () {
    return $scope.ajax({
      url: '/Int/Admin/Grant/Stream/Preview',
      method: 'POST',
      data: {
        title: $scope.model.Name,
        message: $scope.model.EligibilityRequirements
      }
    })
      .then(function (response) {
        var w = window.open('about:blank');
        w.document.open();
        w.document.write(response.data);
        w.document.close();
      })
      .catch(angular.noop);
  }

  /**
   * Enable or Disable the eligiblity requirements.
   * Change the section to edit mode.
   * @function changeEligibilityEnabled
   * @returns {Promise}
   **/
  $scope.changeEligibilityEnabled = function () {
    if (!$scope.section.editing) {
      return $scope.edit()
        .then(function () {
          $scope.section.backup.EligibilityEnabled = !$scope.section.backup.EligibilityEnabled;
        });
    }
  }
});
