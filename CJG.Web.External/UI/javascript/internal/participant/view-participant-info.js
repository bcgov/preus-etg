app.controller('ViewParticipantInfo', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ViewParticipantInfo'
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  $scope.showEmployerInfo = false;

  /**
   * Toggle the view
   * @function toggleEmployerInfo
   * @param {int} state - The state.
   * @returns {void}
   **/
  $scope.toggleEmployerInfo = function (state) {
    $scope.showEmployerInfo = state;
  };
});
