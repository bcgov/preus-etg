require('./grant-program-definition');
require('./grant-program-home-page-message');
require('./grant-program-eligibility');
require('./grant-program-user-guidance');
require('./grant-program-delivery-partners');
require('./grant-program-payment-requests');
require('./grant-program-employer-grant-expense-types');
require('./grant-program-wda-services-expense-types');
require('./grant-program-notification');
require('./grant-program-notifications');
require('./grant-program-document-templates');
require('./grant-program-notification-type');
require('./grant-program-notification-types');
require('./grant-program-denial-reasons');

app.controller('GrantProgramManagement', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'GrantProgramManagement',
    onRefresh: function () {
      return loadGrantPrograms().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load grant program management data.
   * @function loadGrantPrograms
   * @returns {Promise}
   **/
  function loadGrantPrograms() {
    return $scope.load({
      url: '/Int/Admin/Grant/Programs',
      set: 'grantPrograms'
    });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadGrantPrograms()
    ]).catch(angular.noop);
  }

  /**
   * Make AJAX request to get the grantprogram data if a program was selected,
   * otherwise set the model to null.
   * @function loadProgram
   * @returns {Promise}
   **/
  $scope.loadProgram = function () {
    if ($scope.section.selectedProgram) {
      return $scope.load({
        url: '/Int/Admin/Grant/Program/' + $scope.section.selectedProgram.Id,
        set: 'model',
        overwrite: true
      })
        .then(function () {
          $scope.broadcast('show', { target: 'GrantProgramDefinition' });
          $scope.broadcast('refresh', { force: true });
        }).catch(angular.noop);
    }

    $scope.model = null;
    return Promise.resolve();
  }

  /**
   * Make AJAX request to terminate the program
   * @function terminate
   * @returns {Promise}
   **/
  $scope.terminate = function () {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Terminate/' + $scope.model.Id + '?rowVersion=' + encodeURIComponent($scope.model.RowVersion),
      method: 'PUT',
      set: 'model'
    }).catch(angular.noop);
  }

  /**
   * Make AJAX request to implement the program
   * @function implement
   * @returns {Promise}
   **/
  $scope.implement = function () {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Implement/' + $scope.model.Id + '?rowVersion=' + encodeURIComponent($scope.model.RowVersion),
      method: 'PUT',
      set: 'model'
    }).catch(angular.noop);
  }

  /**
   * Make AJAX request to delete the program.
   * @function deleteProgram
   * @returns {Promise}
   **/
  $scope.deleteProgram = function () {
    return $scope.confirmDialog('Delete Grant Program', 'Do you want to delete this grant program?')
      .then(function () {
        return $scope.ajax({
          url: '/Int/Admin/Grant/Program/Delete/' + $scope.model.Id + '?rowVersion=' + encodeURIComponent($scope.model.RowVersion),
          method: 'PUT'
        })
          .then(function () {
            // Delete program
            var index = $scope.grantPrograms.findIndex(function (item) { return item == $scope.section.selectedProgram; });
            $scope.grantPrograms.splice(index, 1);
            $scope.section.selectedProgram = null;
            $scope.model = null;
          });
      })
      .catch(angular.noop);
  }

  /**
   * Create a new program object and open the program definition section for edit.
   * @function createProgram
   * @returns {void}
   * */
  $scope.createProgram = function () {
    $scope.confirmCancel('Create Grant Program', 'Please be aware that this feature is for testing purposes only and will not work for production.')
      .then(function () {
        $scope.model = {
          Id: 0,
          State: 0
        }
        $scope.broadcast('edit', { target: 'GrantProgramDefinition' });
      });
  }

  init();
});
