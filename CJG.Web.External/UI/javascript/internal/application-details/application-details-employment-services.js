app.controller('EmploymentServices', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'EmploymentServices-' + $scope.component.Id,
    displayName: 'Employment Services',
    save: {
      url: '/Int/Application/ESS/Services',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.component.RowVersion;
    },
    onSave: function () {
      $scope.resyncApplicationDetails();
      $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
    },
    onRefresh: function () {
      return loadServices()
        .catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load all service lines for the specified eligible expense type.
   * @function loadServiceLines
   * @returns {Promise}
   **/
  function loadServiceLines() {
    return $scope.load({
      url: '/Int/Application/ESS/Service/Lines/' + $scope.component.EligibleExpenseTypeId,
      set: 'section.serviceLines'
    });
  }

  /**
   * Make AJAX request to load services data
   * @function loadServices
   * @returns {Promise}
   **/
  function loadServices() {
    return $scope.load({
      url: '/Int/Application/ESS/Services/' + $scope.component.Id,
      set: 'model'
    });
  }

  /**
   * Initialize the form data
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadServiceLines(),
      loadServices()
    ]).catch (angular.noop);
  }

  /**
   * Add a new provider to the form so that it can be edited and saved.
   * @function addProvider
   * @returns {Promise}
   **/
  $scope.addProvider = function () {
    return $timeout(function () {
      $scope.component.Providers.push({
        Id: 0,
        RowVersion: null,
        Caption: null,
        CanEdit: true,
        CanRemove: true,
        CanValidate: false,
        IsValidated: true,
        State: 0,
        AddedByAssessor: true
      });
      return Promise.resolve();
    })
      .then(function () {
        $scope.component.CanAdd = false;
        $scope.emit('bubble', { event: 'edit', target: 'EmploymentProvider-0', mode: { Id: 0, CountryId: 'CA' } });
      })
      .catch(angular.noop);
  }
});
