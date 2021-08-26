app.controller('ApplicationSummary', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'ApplicationSummary',
    displayName: 'Summary',
    save: {
      url: '/Int/Application/Summary',
      method: 'PUT',
      data: function () {
        $scope.model.SelectedDeliveryPartnerServiceIds = $scope.deliveryPartnerServices
          .filter(function (item) { return item.isChecked; })
          .map(function (item) {
            return item.Key;
          });
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.grantFile && $scope.model.RowVersion === $scope.grantFile.RowVersion;
    },
    onSave: function () {
      $scope.resyncApplicationDetails();
      $scope.emit('refresh', { force: true });
    },
    onRefresh: function () {
      return loadSummary().catch(angular.noop);
    },
    maxTrainingPeriodDate: null
  };
  if (typeof ($scope.assessors) === 'undefined') $scope.assessors = [];
  if (typeof ($scope.riskClassifications) === 'undefined') $scope.riskClassifications = [];
  if (typeof ($scope.deliveryPartners) === 'undefined') $scope.deliveryPartners = [];
  if (typeof ($scope.deliveryPartnerServices) === 'undefined') $scope.deliveryPartnerServices = [];

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for assessors data
   * @function loadAssessors
   * @returns {Promise}
   **/
  function loadAssessors() {
    return $scope.load({
      url: '/Int/Application/Assessors/' + $scope.parent.grantApplicationId,
      set: 'assessors',
      condition: !$scope.assessors || !$scope.assessors.length
    });
  }

  /**
   * Make AJAX request for risk classifications
   * @function loadRiskClassifications
   * @returns {Promise}
   **/
  function loadRiskClassifications() {
    return $scope.load({
      url: '/Int/Application/Summary/Risk/Classifications',
      set: 'riskClassifications',
      condition: !$scope.riskClassifications || !$scope.riskClassifications.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request for delivery partners
   * @function loadDeliveryPartners
   * @returns {Promise}
   **/
  function loadDeliveryPartners() {
    return $scope.load({
      url: '/Int/Application/Summary/Delivery/Partners/' + $scope.grantFile.GrantProgramId,
      set: 'deliveryPartners',
      condition: !$scope.deliveryPartners || !$scope.deliveryPartners.length,
      localCache: false
    });
  }

  /**
   * Make AJAX request for delivery partner services
   * @function loadDeliveryPartnerServices
   * @returns {void}
   **/
  function loadDeliveryPartnerServices() {
    return $scope.load({
      url: '/Int/Application/Summary/Delivery/Partner/Services/' + $scope.grantFile.GrantProgramId,
      set: 'deliveryPartnerServices',
      condition: !$scope.deliveryPartnerServices || !$scope.deliveryPartnerServices.length,
      localCache: false
    });
  }

  /**
   * Make AJAX request for summary data
   * @function loadSummary
   * @returns {void}
   **/
  function loadSummary() {
    return $scope.load({
      url: '/Int/Application/Summary/' + $scope.parent.grantApplicationId,
      set: 'model'
    })
      .then(function () {
        $scope.section.maxTrainingPeriodDate = getTrainingPeriodMaxDate();
      });
  }

  /**
   * Initialize the data for the form
   * @function init
   * @returns {void{
   **/
  $scope.init = function() {
    return Promise.all([
      loadAssessors(),
      loadRiskClassifications(),
      loadDeliveryPartners(),
      loadDeliveryPartnerServices()      
    ]).then(function() {
      loadSummary();
    }).catch(angular.noop);
  }

  /**
   * Reassign the assessor for this application.
   * @function reassign
   * @returns {Promise}
   **/
  $scope.reassign = function () {
    return $scope.load({
      url: '/Int/Application/Summary/Assign',
      data: function () {
        return $scope.model;
      },
      set: 'model',
      method: 'PUT'
    })
      .then(function () {
        return $scope.section.onSave();
      })
      .catch(angular.noop)
  }

  /**
   * Get the maximum training period date.
   * @function getTrainingPeriodMaxDate
   * @return {Date}
   **/
  function getTrainingPeriodMaxDate() {
    if (Utils.isDate($scope.model.TrainingPeriodStartDate)) {
      return new Date($scope.model.TrainingPeriodStartDate.getFullYear() + 1, $scope.model.TrainingPeriodStartDate.getMonth(), $scope.model.TrainingPeriodStartDate.getDay());
    }
    return;
  }
});
