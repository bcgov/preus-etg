
app.controller('GrantProgramDenialReasons', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'GrantProgramDenialReasons',
    save: {
      url: '/Int/Admin/Grant/Program/Denial/Reasons/',
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

  /**
   * Make AJAX request for grant program Denial Reasons
   * @function loadGrantProgramDenialReasons
   * @returns {Promise}
   **/
  function loadGrantProgramDenialReasons() {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Denial/Reason/' + $scope.$parent.model.Id,
      set: 'model'
    });
  }

  $scope.getDenialReasons = function () {
    var items = $scope.model.GrantProgramDenialReasons;
    var result = [];
    for (let i = 0; i < items.length; i++) {
      var item = items[i];
      result.push(item);
    }
    return result;
  };

  $scope.createReason = function () {
    var items = $scope.model.GrantProgramDenialReasons;
    for (let i = 0; i < items.length; i++) {
      var item = items[i];
      if (item.Caption === "") {
        $scope.alert.type = 'error';
        $scope.clearAlert();
        return $scope.setAlert({ response: { status: 400 }, message: 'Complete all required fields on the first empty row on the denial reasons grid before adding a new reason.' });
      }
    }

    $scope.section.selectedReason = {
      Id: 0,
      IsActive: true,
      Caption: ""
    };
    $scope.model.GrantProgramDenialReasons.splice(0, 0, $scope.section.selectedReason);
    $scope.broadcast('refreshPager');
  }

  $scope.init = function () {
    return Promise.all([
      loadGrantProgramDenialReasons()

    ])
      .then(function () {
      })
      .catch(angular.noop);
  }
});
