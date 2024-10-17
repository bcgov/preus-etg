app.controller('TrainingProviderInventoryEdit', function ($scope, $controller, $timeout, Utils) {

  angular.extend(this, $controller('Base', { $scope }));

  /**
   * Make AJAX request to save the training provider.
   * @function saveProvider
   * @returns {Promise}
   **/
  $scope.saveProvider = function () {
    var provider = $scope.ngDialogData.model.provider;
    return $scope.ajax({
      url: '/Int/Training/Provider/Inventory',
      method: function () {
        return provider.Id ? 'PUT' : 'POST';
      },
      data: provider
    })
      .then(function () {
        return $scope.confirm(provider);
      })
      .catch(angular.noop);
  };

  /**
   * Make AJAX request to delete the training provider.
   * @function deleteProvider
   * @returns {Promise}
   **/
  $scope.deleteProvider = function () {
    var provider = $scope.ngDialogData.model.provider;;
    return $scope.confirmDialog('Delete Training Provider', 'Do you want to delete this training provider?')
      .then(function () {
        return $scope.ajax({
          url: '/Int/Training/Provider/Inventory/Delete',
          method: 'PUT',
          data: provider
        })
          .then(function () {
            return $scope.confirm(provider);
          });
      })
      .catch(angular.noop);
  };

  /**
   * Cancel the training provider popup.
   * @function cancelProvider
   * @returns {Promise}
   **/
  $scope.cancelProvider = function () {
    return $scope.confirm();
  };
});
