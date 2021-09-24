app.controller('ChangeProgramDatesView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'ChangeProgramDatesView',
    save: {
      url: '/Ext/Agreement/Change/Program/Dates',
      method: 'PUT',
      data: function () {
        return $scope.ngDialogData.model;
      }
    },
    onSave: function (event, data) {
      return $scope.confirm(data.response.data);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

});
