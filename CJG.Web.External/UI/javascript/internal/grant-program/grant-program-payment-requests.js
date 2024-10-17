app.controller('GrantProgramPaymentRequests', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'GrantProgramPaymentRequests',
    save: {
      url: '/Int/Admin/Grant/Program/Payment/Requests',
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
   * Make AJAX request for expense authorities
   * @function loadExpenseAuthorities
   * @returns {Promise}
   **/
  function loadExpenseAuthorities() {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Expense/Authorities',
      set: 'expenseAuthorities',
      condition: !$scope.expenseAuthorities || !$scope.expenseAuthorities.length
    });
  }
  /**
   * Initialize the data for the form
   * @function init
   * @returns {void{
   **/
  $scope.init = function () {
    return Promise.all([
      loadExpenseAuthorities()
    ])
      .then(function () {
        $scope.section.isLoaded = true;
      })
      .catch(angular.noop);
  }
});
