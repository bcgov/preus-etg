app.controller('ApplicationReviewApplicantDeclarationView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadApplicationReviewApplicantDeclaration() {
    return $scope.load({
      url: '/Ext/Application/Review/Applicant/Declaration/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  loadApplicationReviewApplicantDeclaration()
    .catch(angular.noop);

  $scope.submit = function () {
    return $scope.ajax({
      url: '/Ext/Application/Review/Submit',
      method: 'PUT',
      data: $scope.model
    })
      .then(function (response) {
        if (response.data.RedirectURL) window.location = response.data.RedirectURL;
      })
      .catch(angular.noop);
  }
});
