app.controller('ChangeDeliveryDatesView', function ($scope, $attrs, $controller, Utils) {

  $scope.section = {
    name: 'ChangeDeliveryDatesView',
    save: {
      url: '/Ext/Agreement/Change/Delivery/Dates',
      method: 'PUT',
      data: function () {
        return $scope.ngDialogData.model;
      }
    },
    onSave: function (event, data) {
      return $scope.confirm(data.response.data);
    },
    endDateLoaded: false
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.$watch('ngDialogData.model.StartDate', function (newValue, oldValue) {
    if (!$scope.section.endDateLoaded) {
      let maxEndDate = $scope.getTrainingPeriodMaxDate();
      $scope.ngDialogData.model.MaxEndDate = maxEndDate;
      $scope.section.endDateLoaded = true;
      return;
    }

    if (newValue === oldValue)
      return;

    let maxEndDate = $scope.getTrainingPeriodMaxDate();
    $scope.ngDialogData.model.MaxEndDate = maxEndDate;
  });

  $scope.getTrainingPeriodMaxDate = function() {
    if (moment.isMoment($scope.ngDialogData.model.StartDate) && $scope.ngDialogData.model.StartDate.isValid()) {
      const startDate = $scope.ngDialogData.model.StartDate;
      return new Date(startDate.year() + 1, startDate.month(), startDate.day());
    }

    if (Utils.isDate($scope.model.StartDate)) {
      const startDate = $scope.ngDialogData.model.StartDate;
      return new Date(startDate.getFullYear() + 1, startDate.getMonth(), startDate.getDay());
    }

    if (Utils.isDate($scope.ngDialogData.model.GrantOpeningTrainingPeriodStartDate)) {
      const startDate = $scope.ngDialogData.model.GrantOpeningTrainingPeriodStartDate;
      return new Date(startDate.getFullYear() + 1, startDate.getMonth(), startDate.getDay());
    }

    return null;
  }
});
