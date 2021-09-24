app.controller('ChangeRequestView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'ChangeRequestView',
    grantApplicationId: $attrs.ngGrantApplicationId
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.ToggleChangeServiceProvider = function (id, trainingProviderId, eligibleExpenseTypeId) {
    return $scope.load({
      url: '/Ext/Agreement/Service/Provider/' + id + '/' + trainingProviderId + '/' + $scope.section.grantApplicationId + "/" + eligibleExpenseTypeId,
      set: 'TrainingProvider'
    })
      .then(function () {
        return $scope.ajax({
          url: '/Ext/Agreement/Change/Service/Provider/View'
        })
          .then(function (response) {
            // TODO: Replace Kendo
            $trainingProviderModal.html($compile(response.data)($scope));
            $trainingProviderModal.data('kendoWindow').center().open();
          })
      })
      .catch(angular.noop);
  }

  $scope.removeChangeTrainingProvider = function () {
    var url = '/Ext/Agreement/Provider/Delete';
    var verb = 'PUT';
    if ($scope.TrainingProvider.TrainingProvider != null) {
      url = url + "?trainingProviderId=" + $scope.TrainingProvider.TrainingProvider.Id + "&rowVersion=" + encodeURIComponent($scope.TrainingProvider.TrainingProvider.RowVersion);
    }
    if ($scope.TrainingProvider.ServiceProvider != null) {
      url = url + "?trainingProviderId=" + $scope.TrainingProvider.ServiceProvider.Id + "&rowVersion=" + encodeURIComponent($scope.TrainingProvider.ServiceProvider.RowVersion);
    }

    $scope.ajax({
      url: url
    })
      .then(function (response) {
        // TODO: Replace Kendo
        $trainingProviderModal.data('kendoWindow').close();
        $scope.loadAgreement();
      })
      .catch(angular.noop);
  }

  $scope.ChangeTrainingProvider = function () {
    angular.element.each(errorPropertyList, function (index, value) {
      $scope[value] = '';
    });

    var url = '/Ext/agreement/training/provider';
    var verb = 'PUT';
    if ($scope.TrainingProvider.TrainingProvider != null) {
      if ($scope.TrainingProvider.TrainingProvider.IsSkillProvider) {
        url = '/Ext/agreement/skills/training/provider';
      }
      $scope.TrainingProvider.TrainingProvider.ContactPhone = $scope.TrainingProvider.TrainingProvider.ContactPhoneAreaCode +
        $scope.TrainingProvider.TrainingProvider.ContactPhoneExchange +
        $scope.TrainingProvider.TrainingProvider.ContactPhoneNumber;

      verb = $scope.TrainingProvider.TrainingProvider.Id == 0 ? 'POST' : 'PUT';
    }
    if ($scope.TrainingProvider.ServiceProvider != null) {
      url = '/Ext/agreement/service/provider';
      $scope.TrainingProvider.ServiceProvider.ContactPhone = $scope.TrainingProvider.ServiceProvider.ContactPhoneAreaCode +
        $scope.TrainingProvider.ServiceProvider.ContactPhoneExchange +
        $scope.TrainingProvider.ServiceProvider.ContactPhoneNumber;
      verb = $scope.TrainingProvider.ServiceProvider.Id == 0 ? 'POST' : 'PUT';
    }

    return $scope.ajax({
      url: url,
      method: verb,
      data: $scope.TrainingProvider
    })
      .then(function (response) {
        // TODO: Replace Kendo
        $trainingProviderModal.data('kendoWindow').close();
      })
      .catch(angular.noop);
  }

  /**
   * Determine if the selected training provider type requires additional attachments.
   * @function isPrivateSector
   * @param {any} trainingProviderTypeId
   * @returns {boolean}
   */
  $scope.isPrivateSector = function (trainingProviderTypeId) {
    var type = $scope.ProviderTypes.find(function (item) {
      return item.Id === trainingProviderTypeId;
    });

    if (type) {
      switch (type.PrivateSectorValidationType) {
        case (0): // No additional documents required.
          return false;
        case (1): // Additional documents required.
        case (2): // TODO: Additional documents required if past a setting date.
        default:
          return true;
      }
    }
    return false;
  };

  /**
   * Show the modal popup to change the delivery dates.
   * @function showChangeDeliveryDates
   * @returns {Promise}
   **/
  $scope.showChangeDeliveryDates = function () {
    return ngDialog.openConfirm({
      template: '/Ext/Agreement/Change/Delivery/Dates/View',
      data: {
        title: 'Change Delivery Dates',
        model: {
          Id: $scope.model.Id,
          RowVersion: $scope.model.RowVersion,
          GrantOpeningTrainingPeriodStartDate: $scope.model.ScheduleA.GrantOpeningTrainingPeriodStartDate,
          GrantOpeningTrainingPeriodEndDate: $scope.model.ScheduleA.GrantOpeningTrainingPeriodEndDate,
          StartDate: $scope.model.ScheduleA.DeliveryDate.StartDate,
          EndDate: $scope.model.ScheduleA.DeliveryDate.EndDate,
          TermStartDate: $scope.model.ScheduleA.DeliveryDate.TermStartDate,
          TermEndDate: $scope.model.ScheduleA.DeliveryDate.TermEndDate,
          MaxEndDate: new Date($scope.model.ScheduleA.GrantOpeningTrainingPeriodEndDate.getFullYear() + 1, $scope.model.ScheduleA.GrantOpeningTrainingPeriodEndDate.getMonth(), $scope.model.ScheduleA.GrantOpeningTrainingPeriodEndDate.getDay())
        }
      }
    })
      .then(function (model) {
        $scope.sync(model, $scope.model);
      })
      .catch(angular.noop);
  }

  /**
   * Show the modal popup to change the program dates.
   * @function showChangeProgramDates
   * @returns {Promise}
   **/
  $scope.showChangeProgramDates = function (trainingProgram) {
    return ngDialog.openConfirm({
      template: '/Ext/Agreement/Change/Program/Dates/View',
      data: {
        title: 'Change Program Dates',
        model: trainingProgram,
        DeliveryStartDate: $scope.model.ScheduleA.DeliveryDate.StartDate,
        DeliveryEndDate: $scope.model.ScheduleA.DeliveryDate.EndDate

      }
    })
      .then(function (model) {
        $scope.sync(model, $scope.model);
      })
      .catch(angular.noop);
  }

  /**
   * Show the modal popup to change the selected training provider.
   * @function showChangeTrainingProvider
   * @param {int} trainingProviderId - The original training provider Id.
   * @returns {Promise}
   */
  $scope.showChangeTrainingProvider = function (trainingProviderId) {
    return ngDialog.openConfirm({
      template: '/Ext/Agreement/Change/Training/Provider/View/' + trainingProviderId,
      data: {
        title: 'Change Training Provider'
      }
    })
      .then(function (model) {
        if (model) $scope.sync(model, $scope.model);
        else $scope.loadAgreement();
      })
      .catch(angular.noop);
  };

  /**
   * Show the modal popup to change the selected service provider.
   * @function showChangeServiceProvider
   * @param {int} trainingProviderId - The original service provider Id.
   * @returns {Promise}
   */
  $scope.showChangeServiceProvider = function (trainingProviderId) {
    return ngDialog.openConfirm({
      template: '/Ext/Agreement/Change/Service/Provider/View/' + trainingProviderId,
      data: {
        title: 'Change Service Provider',
        loadUrl: '/Ext/Agreement/Change/Service/Provider/'
      }
    }).then(function (model) {
      if (model) $scope.sync(model, $scope.model);
      else $scope.loadAgreement();
    }).catch(angular.noop);
  };

  /**
   * Cancel the current change request and delete any change request details.
   * Make AJAX request to save to the datasource.s
   * @function cancelChangeRequest
   * @returns {Promise}
   **/
  $scope.cancelChangeRequest = function () {
    return $scope.ajax({
      url: '/Ext/Agreement/Cancel/Change/Request',
      method: 'PUT',
      data: $scope.model
    })
      .then(function (response) {
        return $scope.sync(response.data, $scope.model);
      })
      .catch(angular.noop);
  }

  /**
   * Submit the current change request.
   * Make an AJAX request to save to the datasource.
   * @function submitChangeRequest
   * @returns {Promise}
   **/
  $scope.submitChangeRequest = function () {
    return $scope.confirmDialog('Submit Request', 'Have you entered all the provider changes that you need in this request?')
      .then(function () {
        return $scope.ajax({
          url: '/Ext/Agreement/Submit/Change/Request',
          method: 'PUT',
          data: $scope.model
        })
          .then(function (response) {
            window.location = '/Ext/Reporting/Grant/File/View/' + $scope.section.grantApplicationId;
          });
      })
      .catch(angular.noop);
  }

  /**
   * Get the maximum training period date.
   * @function getTrainingPeriodMaxDate
   * @return {Date}
   **/
  $scope.getTrainingPeriodMaxDate= function() {
    if (Utils.isDate($scope.model.GrantOpeningTrainingPeriodStartDate)) {
      return new Date($scope.model.GrantOpeningTrainingPeriodStartDate.getFullYear() + 1, $scope.model.GrantOpeningTrainingPeriodStartDate.getMonth(), $scope.model.GrantOpeningTrainingPeriodStartDate.getDay());
    }
    return;
  }
});
