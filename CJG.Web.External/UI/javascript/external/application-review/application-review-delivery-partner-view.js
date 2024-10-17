app.controller('ApplicationReviewDeliveryPartnerView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadApplicationReviewDeliveryPartner() {
    return $scope.load({
      url: '/Ext/Application/Review/Delivery/Partner/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  loadApplicationReviewDeliveryPartner()
    .catch(angular.noop);

  $scope.submit = function () {
    return $scope.ajax({
      url: '/Ext/Application/Review/Delivery/Partner',
      method: 'PUT',
      data: $scope.model
    })
      .then(function (response) {
        window.location = '/Ext/Application/Review/Applicant/Declaration/View/' + $scope.section.grantApplicationId;
      })
      .catch(angular.noop);
  }

  $scope.toggleSelection = function toggleSelection(name, _index) {
    if ($scope.model[name] == null) {
      $scope.model[name] = [];
    }

    var idx = $scope.model[name].indexOf(_index);

    if (idx > -1) {
      $scope.model[name].splice(idx, 1);
    }
    else {
      $scope.model[name].push(_index);
    }
  }
});
