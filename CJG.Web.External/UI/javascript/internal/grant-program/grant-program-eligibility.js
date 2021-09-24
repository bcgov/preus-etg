app.controller('GrantProgramEligibility', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'GrantProgramEligibility',
    save: {
      url: '/Int/Admin/Grant/Program/Eligibility',
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
      $scope.emit('update', { model: $scope.model });
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
      url: '/Int/Admin/Grant/Program/Preview',
      method: 'POST',
      data: {
        title: $scope.model.Name,
        message: $scope.model.EligibilityDescription
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
});
