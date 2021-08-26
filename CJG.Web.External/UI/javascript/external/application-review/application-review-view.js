app.controller('ApplicationReviewView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadApplicationReview() {
    return $scope.load({
      url: '/Ext/Application/Review/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  loadApplicationReview().catch(angular.noop);
});


