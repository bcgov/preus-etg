app.controller('ScheduleA', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'ScheduleA',
    displayName: 'Schedule A',
    loaded: function () {
      return $scope.model && $scope.model.Body;
    },
    onRefresh: function () {
      return loadScheduleA().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for schedule A data.
   * @function loadScheduleA
   * @returns {Promise}
   **/
  function loadScheduleA() {
    return $scope.load({
      url: '/Int/Application/Agreement/ScheduleA/' + $scope.grantAgreement.Id + ($scope.grantAgreement.Version ? '/' + $scope.grantAgreement.Version : ''),
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
      loadScheduleA()
    ]).catch(angular.noop);
  };
});
