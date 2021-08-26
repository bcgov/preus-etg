app.controller('ApplicationDetailsView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadApplicationDetails() {
    return $scope.load({
      url: '/Ext/Application/Details/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  $scope.showComponent = function ($event) {
    var section = $event.currentTarget.nextElementSibling;
    var icon = $event.currentTarget.getElementsByClassName("icon")[0];
    if (section.classList.contains('ng-hide')) {
      section.classList.remove('ng-hide');
      section.classList.add('ng-show');
      icon.classList.remove('down-arrow');
      icon.classList.add('up-arrow');
    } else {
      section.classList.add('ng-hide');
      section.classList.remove('ng-show');
      icon.classList.add('down-arrow');
      icon.classList.remove('up-arrow');
    }
  }

  loadApplicationDetails()
      .catch(angular.noop);
});


