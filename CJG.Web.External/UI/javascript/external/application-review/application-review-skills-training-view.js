app.controller('ApplicationReviewSkillsTrainingView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId,
    eligibleExpenseTypeId: $attrs.ngEligibleExpenseTypeId
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadApplicationReviewSkillsTraining() {
    return $scope.load({
      url: '/Ext/Application/Review/Skills/Training/' + $scope.section.grantApplicationId + '/' + $scope.section.eligibleExpenseTypeId,
      set: 'model'
    });
  }

  loadApplicationReviewSkillsTraining()
    .catch(angular.noop);
});


