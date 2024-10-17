app.controller('ApplicationReviewTrainingCostView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadApplicationReviewTrainingCosts() {
    return $scope.load({
      url: '/Ext/Application/Review/Training/Cost/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  loadApplicationReviewTrainingCosts()
    .catch(angular.noop);
});
