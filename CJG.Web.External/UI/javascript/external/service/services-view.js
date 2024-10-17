app.controller('EmploymentServicesView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId,
    eligibleExpenseTypeId: $attrs.ngEligibleExpenseTypeId,
    eligibleCostId: $attrs.ngEligibleCostId,
    save: {
      url: '/Ext/Application/Employment/Services/Supports',
      method: 'PUT',
      data: function () {
        return $scope.model;
      }
    },
    onSave: function (event, data) {
      if (data.response.data.RedirectURL) window.location = data.response.data.RedirectURL;
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make an AJAX request to fetch the services and supports data.
   * @function loadServicesAndSupports
   * @returns {Promise}
   **/
  function loadServicesAndSupports() {
    return $scope.load({
      url: '/Ext/Application/' + $scope.section.grantApplicationId + '/Employment/Services/Supports/' + $scope.section.eligibleExpenseTypeId + '/' + $scope.section.eligibleCostId,
      set: 'model'
    });
  }

  /**
   * Initialize the form data.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return loadServicesAndSupports()
      .catch(angular.noop);
  }

  init();
});
