app.controller('GrantProgramWDAServicesConfig', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'GrantProgramWDAServicesConfig',
    save: {
      url: '/Int/Admin/Grant/Program/WDA/Service/Expense/Types',
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
   * Make AJAX request for claim types
   * @function loadClaimTypes
   * @returns {Promise}
   **/
  function loadClaimTypes() {
    return $scope.load({
      url: '/Int/Admin/Claim/Types',
      set: 'claimTypes',
      condition: !$scope.claimTypes || !$scope.claimTypes.length
    });
  }

  /**
   * Make AJAX request for expense types
   * @function loadExpenseTypes
   * @returns {Promise}
   **/
  function loadExpenseTypes() {
    return $scope.load({
      url: '/Int/Admin/Expense/Types',
      set: 'expenseTypes',
      condition: !$scope.expenseTypes || !$scope.expenseTypes.length
    });
  }

  /**
   * Make AJAX request for rates
   * @function loadRates
   * @returns {Promise}
   **/
  function loadRates() {
    return $scope.load({
      url: '/Int/Admin/Rates',
      set: 'rates',
      condition: !$scope.rates || !$scope.rates.length
    });
  }

  /**
   * Initialize the data for the form
   * @function init
   * @returns {void{
   **/
  $scope.init = function () {
    return Promise.all([
      loadClaimTypes(),
      loadExpenseTypes(),
      loadRates()
    ])
      .then(function () {
        $scope.section.isLoaded = true;
      })
      .catch(angular.noop);
  }

  /**
   * Get the claim type caption that is selected.
   * @function claimTypeCaption
   * @returns {string}
   **/
  $scope.claimTypeCaption = function () {
    return $scope.claimTypes.find(function (item) {
      return item.Key === $scope.model.ProgramConfiguration.ClaimTypeId;
    }).Value;
  }

  /**
   * Get the rate caption.
   * @function rateCaption
   * @param {float} rate
   * @returns {string}
   */
  $scope.rateCaption = function (rate) {
    var item = $scope.rates.find(function (item) { return item.Key === rate; });
    return item ? item.Value : 'N/A';
  }

  $scope.STPCaption = function (min, max) {
    return min + ' to ' + max;
  }

  $scope.ESSProvidersCaption = function (min, max) {
    return !max ? 'None' : (min + ' to ' + max);
  }

  /**
   * Reorder the array by moving the item as the specified index up one.
   * @function reorder
   * @param {Array} array - The array to modify.
   * @param {int} index - The index of the item to move up one.
   * @returns {void}
   */
  $scope.reorder = function (array, index) {
    var rowSequence = array[index].RowSequence;
    array[index].RowSequence = array[index - 1].RowSequence;
    array[index - 1].RowSequence = rowSequence;
    Utils.moveItem(array, index, index - 1);
  }

  /**
   * Open a new tab and display the message.
   * @function preview
   * @param {string} title - The title of the tab.
   * @param {string} message - The message to test in the new tab.
   * @returns {Promise}
   **/
  $scope.preview = function (title, message) {
    return $scope.ajax({
      url: '/Int/Admin/Grant/Program/Preview',
      method: 'POST',
      data: {
        title: title,
        message: message
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
   * Confirm the user wants to perform the action.
   * Send AJAX request to synchronize with the master WDA Service Descriptions.
   * @function syncWDAServices
   * @returns {Promise}
   **/
  $scope.syncWDAService = function () {
    return $scope.confirmDialog('Sync WDA Service', '<p>Do you want to synchronize this programs WDA Services with the master Service Descriptions?</p><p>This could result in additional service categories and service lines.</p>')
      .then(function () {
        return $scope.load({
          url: '/Int/Admin/Grant/Program/WDA/Service/Sync',
          method: 'PUT',
          data: $scope.model,
          set: 'model'
        })
          .then(function () {
            $scope.emit('update', { model: $scope.model });
          })
      })
      .catch(angular.noop);
  }
});
