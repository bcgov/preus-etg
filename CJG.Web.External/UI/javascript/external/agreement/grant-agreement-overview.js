app.controller('AgreementOverviewView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  };

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  function loadAgreement() {
    return $scope.load({
      url: '/Ext/Agreement/Overview/' + $scope.section.grantApplicationId,
      set: 'model'
    })
      .then(function () {
        $scope.broadcast('update', { model: $scope.model });
      });
  }

  function loadProviderTypes() {
    return $scope.load({
      url: '/Ext/Training/Provider/Types',
      set: 'ProviderTypes'
    });
  }

  function loadCountries() {
    return $scope.load({
      url: '/Ext/Address/Countries',
      set: 'Countries'
    });
  }

  function loadProvinces() {
    return $scope.load({
      url: '/Ext/Address/Provinces',
      set: 'Provinces'
    });
  }

  function init() {
    return Promise.all([
      loadAgreement(),
      loadProviderTypes(),
      loadCountries(),
      loadProvinces()
    ])
      .catch(angular.noop);
  }

  $scope.toggleDocument = function ($event) {
    if ($event.currentTarget.nextElementSibling.classList.contains('ng-hide')) {
      $event.currentTarget.nextElementSibling.classList.remove('ng-hide');
      $event.currentTarget.nextElementSibling.classList.add('ng-show');
    } else {
      $event.currentTarget.nextElementSibling.classList.remove('ng-show');
      $event.currentTarget.nextElementSibling.classList.add('ng-hide');
    }
  }

  $scope.cancelAgreement = function () {
    return ngDialog.openConfirm({
      template: '/Ext/Agreement/Cancel/View/' + $scope.section.grantApplicationId,
      data: {
        title: 'Cancel Agreement'
      }
    })
      .then(function (reason) {
        return $scope.ajax({
          url: '/Ext/Agreement/Cancel',
          method: 'PUT',
          data: function () {
            return {
              Id: $scope.model.Id,
              RowVersion: $scope.model.RowVersion,
              CancelReason: reason
            };
          }
        })
          .then(function () {
            window.location = '/Ext/Home/Index';
          });
      })
      .catch(angular.noop);
  }

  init();
});
