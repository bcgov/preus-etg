app.controller('ReportingGrantFileView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  }

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadGrantFile() {
    return $scope.load({
      url: '/Ext/Reporting/Grant/File/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }


  loadGrantFile()
    .catch(angular.noop);
});


