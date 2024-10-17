app.controller('ApplicationReviewProgramView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  }

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadApplicationReviewProgram() {
    return $scope.load({
      url: '/Ext/Application/Review/Program/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  loadApplicationReviewProgram()
    .catch(angular.noop);
});


