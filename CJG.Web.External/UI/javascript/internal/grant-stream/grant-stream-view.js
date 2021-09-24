require('./grant-stream-definition');
require('./grant-stream-application-attachments');
require('./grant-stream-application-business-case');
require('./grant-stream-eligibility');
require('./grant-stream-reporting');
require('./grant-stream-payment-requests');
require('./grant-stream-employer-grant-expense-types');
require('./grant-stream-wda-services-expense-types');

app.controller('GrantStreamManagement', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'GrantStreamManagement',
    onRefresh: function () {
      return loadGrantPrograms().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to fetch a list of grant programs.
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
   * Make AJAX request to fetch a list of grant streams for the speicifed grant program.
   * @function loadStreams
   * @returns {Promise}
   **/
  $scope.loadStreams = function () {
    if ($scope.section.selectedProgram) {
      $scope.model = {};
      return $scope.load({
        url: '/Int/Admin/Grant/Streams/' + $scope.section.selectedProgram.Id,
        set: 'grantStreams'
      });
    } else {
      return $timeout(function () {
        $scope.model = null;
        $scope.grantStreams = [];
      });
    }
  }

  /**
   * Make AJAX request to get the grant stream data if a stream was selected, otherwise set the model to null.
   * @function loadStream
   * @param {object} stream - The stream to load.
   * @returns {Promise}
   **/
  $scope.loadStream = function (stream) {
    return $scope.load({
      url: '/Int/Admin/Grant/Stream/' + stream.Id,
      set: 'model',
      overwrite: true
    })
      .then(function () {
        $scope.broadcast('show', { target: 'GrantStreamDefinition' });
        $scope.broadcast('refresh', { force: true });
      }).catch(angular.noop);
  }

  /**
   * Make AJAX request to delete the stream.
   * @function deleteStream
   * @returns {Promise}
   **/
  $scope.deleteStream = function () {
    return $scope.confirmDialog('Delete Grant Stream', 'Do you want to delete the "' + $scope.model.Name + '" grant stream?')
      .then(function () {
        return $scope.ajax({
          url: '/Int/Admin/Grant/Stream/Delete',
          method: 'PUT',
          data: $scope.model
        })
          .then(function () {
            $scope.loadStreams();
          });
      })
      .catch(angular.noop);
  }

  /**
   * Create a new stream object and open the stream definition section for edit.
   * @function createStream
   * @returns {void}
   * */
  $scope.createStream = function () {
    $scope.model = {
      Id: 0,
      State: 0,
      GrantProgramState: $scope.section.selectedProgram.State,
      GrantProgramId: $scope.section.selectedProgram.Id,
      ProgramConfigurationId: $scope.section.selectedProgram.ProgramConfigurationId,
      AccountCodeId: $scope.section.selectedProgram.AccountCodeId
    }
    $scope.broadcast('edit', { target: 'GrantStreamDefinition' });
  }

  init();
});
