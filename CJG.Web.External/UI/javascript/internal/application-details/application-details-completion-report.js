app.controller('CompletionReport', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'CompletionReport',
    displayName: 'Completion Report',
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.grantFile.RowVersion;
    },
    onRefresh: function () {
      return loadCompletionReport().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for completion report data.
   * @function loadCompletionReport
   * @returns {Promise}
   * */
  function loadCompletionReport() {
    return $scope.load({
      url: '/Int/Application/Completion/Report/Summary/' + $scope.parent.grantApplicationId,
      set: 'model'
    });
  }

  /**
   * Initialize section data.
   * @function init
   * @returns {Promise}
   * */
  $scope.init = function () {
    return Promise.all([
      loadCompletionReport()
    ]).catch(angular.noop);
  }
});
