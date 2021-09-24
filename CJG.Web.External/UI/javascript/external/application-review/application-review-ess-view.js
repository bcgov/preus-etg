app.controller('ApplicationReviewESSView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadApplicationReviewESS() {
    return $scope.load({
      url: '/Ext/Application/Review/ESS/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  loadApplicationReviewESS()
    .catch(angular.noop);
});
