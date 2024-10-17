app.controller('ScheduleB', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'ScheduleB',
    displayName: 'Schedule B',
    loaded: function () {
      return $scope.model && $scope.model.Body;
    },
    onRefresh: function () {
      return loadScheduleB().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for schedule B data.
   * @function loadScheduleB
   * @returns {Promise}
   **/
  function loadScheduleB() {
    return $scope.load({
      url: '/Int/Application/Agreement/ScheduleB/' + $scope.grantAgreement.Id + ($scope.grantAgreement.Version ? '/' + $scope.grantAgreement.Version : ''),
      set: 'model'
    });
  }

  /**
   * Initialize section data.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadScheduleB()
    ]).catch(angular.noop);
  };
});
