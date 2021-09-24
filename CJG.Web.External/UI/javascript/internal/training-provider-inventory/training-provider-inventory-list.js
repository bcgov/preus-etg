require('./training-provider-inventory-edit');

app.controller('TrainingProviderInventoryList', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'TrainingProviderInventory',
    onRefresh: function () {
      return loadTrainingProviderInventory().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
   * Make AJAX request to load training provider inventory data.
   * @function loadTrainingProviderInventory
   * @returns {Promise}
   **/
  function loadTrainingProviderInventory() {
    return $scope.load({
      url: '/Int/Training/Provider/Inventory',
      set: 'model'
    });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadTrainingProviderInventory()
    ]).catch(angular.noop);
  }

  /**
   * Get the filtered providers.
   * @function getProviders
   * @param {string} pageKeyword - The search filter keyword.
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items in a page.
   * @returns {Promise}
   **/
  $scope.getProviders = function (pageKeyword, page, quantity) {
    return $scope.ajax({
      url: '/Int/Training/Provider/Inventory/Search/' + page + '/' + quantity + (pageKeyword ? '?search=' + pageKeyword : '')
    })
      .then(function (response) {
        return Promise.resolve(response.data);
      })
      .catch(angular.noop);
  };

  /**
   * Show the add training provider modal popup.
   * @function addProvider
   * @returns {Promise}
   **/
  $scope.addProvider = function () {
    return ngDialog.openConfirm({
      template: '/Int/Training/Provider/Inventory/Form/View',
      data: {
        title: 'Add Training Provider',
        model: {
          provider: {
            Id: 0,
            IsActive: true,
            IsEligible: true
          },
          admin: $scope.model.AdminUser
        }
      },
      controller: 'TrainingProviderInventoryEdit'
    }).then(function (response) {
      if (response)
        $scope.broadcast('refreshPager');
    }).catch(angular.noop);
  };

  /**
   * Show the edit training provider modal popup.
   * @function editProvider
   * @param {object} provider - The training provider.
   * @returns {Promise}
   **/
  $scope.editProvider = function (provider) {
    return ngDialog.openConfirm({
      template: '/Int/Training/Provider/Inventory/Form/View',
      data: {
        title: 'Training Provider Details',
        model: {
          provider: angular.copy(provider),
          admin: $scope.model.AdminUser
        }
      },
      controller: 'TrainingProviderInventoryEdit'
    }).then(function (response) {
      if (response)
        $scope.broadcast('refreshPager');
    }).catch(angular.noop);
  };

  init();
});
