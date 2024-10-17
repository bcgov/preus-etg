app.controller('GrantStreamPaymentRequests', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'GrantStreamPaymentRequests',
    save: {
      url: '/Int/Admin/Grant/Stream/Payment/Requests',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model.AccountCode;
    },
    onSave: function () {
      $scope.emit('refresh');
    },
    onRefresh: function () {
      return loadAccountCodes();
    },
    onCancel: function () {
      if ($scope.model.AccountCodeId === 0) {
        $scope.model.SelfAccountCode = false;
        var program = $scope.$parent.section.selectedProgram;
        $scope.model.AccountCodeId = program.AccountCodeId;
        return loadAccountCodes(program.AccountCodeId).catch(angular.noop);
      }
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.$watch('$parent.model', function (newValue, oldValue) {
    $scope.model = newValue;
  });

  /**
   * Make AJAX request for account codes.
   * @function loadAccountCodes
   * @returns {Promise}
   **/
  function loadAccountCodes() {
    return $scope.load({
      url: '/Int/Admin/Grant/Stream/Account/Code/' + $scope.model.AccountCodeId,
      set: 'model.AccountCode'
    });
  }

  /**
   * Initialize the data for the form
   * @function init
   * @returns {void}
   **/
  $scope.init = function () {
    return Promise.all([
      loadAccountCodes()
    ])
      .then(function () {
        $scope.section.isLoaded = true;
      })
      .catch(angular.noop);
  }

  /**
   * Changes the account codes to use the grant programs account codes, or its own.
   * @function changeAccountCode
   * @returns {void}
   **/
  $scope.changeAccountCode = function () {
    if ($scope.section.editing) {
      $scope.cancel();
    }

    if ($scope.model.SelfAccountCode) {
      $scope.model.AccountCodeId = 0;
      $scope.model.AccountCode = {};
      return $scope.edit();
    } else {
      var program = $scope.$parent.section.selectedProgram;
      $scope.model.AccountCodeId = program.AccountCodeId;
      return loadAccountCodes(program.AccountCodeId)
        .then(function () {
          return $scope.save();
        }).catch (angular.noop);
    }
  }
});
