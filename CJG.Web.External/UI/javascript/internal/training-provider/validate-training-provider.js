app.controller('ValidateTrainingProvider', function ($scope, $controller, $timeout, Utils) {
  angular.extend(this, $controller('Base', { $scope: $scope }));

  /**
   * Get the filtered providers.
   * @function getProviders
   * @param {string} search - The search filter keyword.
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items in a page.
   * @returns {Promise}
   **/
  $scope.getProviders = function (search, page, quantity) {
    return $scope.ajax({
      url: '/Int/Application/Training/Provider/Inventory/' + page + '/' + quantity + (search ? '?search=' + search : '')
    }).then(function (response) {
        return Promise.resolve(response.data);
    }).catch(angular.noop);
  };

  $scope.selectProvider = function (provider) {
    $scope.confirm(provider);
  }

  $scope.addProvider = function () {
    var provider = $scope.ngDialogData.model;
    $scope.ajax({
      url: '/Int/Application/Training/Provider/Add/To/Inventory?name=' + encodeURIComponent(provider.Name),
      method: 'POST'
    }).then(function () {
      $scope.broadcast('refreshPager');
      $scope.setAlert({ response: { status: 200 }, message: 'Training Provider ' + provider.Name +' has been added successfully.' });
    }).catch(angular.noop);
  }
});
