app.controller('GrantStreamDefinition', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'GrantStreamDefinition',
    save: {
      url: function () {
        return $scope.model.Id === 0 ? '/Int/Admin/Grant/Stream' : '/Int/Admin/Grant/Stream/Definition';
      },
      method: function () { return $scope.model.Id === 0 ? 'POST' : 'PUT' },
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.section.isLoaded;
    },
    onSave: function (event, data) {
      $scope.emit('refresh');
      if (data.response.config.method === 'POST') {
        var stream = {
          Id: $scope.model.Id,
          Caption: $scope.model.Name,
          DateFirstUsed: 'N/A',
          CanDelete: true
        };
        $scope.grantStreams.push(stream);
      }
    },
    onCancel: function () {
      if ($scope.model.Id === 0) {
        return $scope.loadStream();
      }
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.$watch('$parent.model', function (newValue, oldValue) {
    $scope.model = newValue;
  });

  /**
   * Make AJAX request for rates.
   * @function loadRates
   * @returns {Promise}
   **/
  function loadRates() {
    return $scope.load({
      url: '/Int/Admin/Rates',
      set: 'reimbursementRates',
      condition: !$scope.reimbursementRates || !$scope.reimbursementRates.length
    });
  }
  
  /**
   * Initialize the data for the form
   * @function init
   * @returns {void}
   **/
  $scope.init = function () {
    return Promise.all([
      loadRates()
    ])
      .then(function () {
        $scope.section.isLoaded = true;
      })
      .catch(angular.noop);
  }

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
        message: $scope.model.Objective
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
