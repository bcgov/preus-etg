require('./grant-agreement-cover-letter');
require('./grant-agreement-schedule-a');
require('./grant-agreement-schedule-b');

app.controller('GrantAgreement', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'GrantAgreement',
    onRefresh: function () {
      return loadGrantAgreement().catch(angular.noop);
    },
    grantApplicationId: $attrs.ngGrantApplicationId,
    versions: []
  };

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));
  
  /**
   * Make AJAX request to load grant agreement data.
   * @function loadGrantAgreement
   * @returns {Promise}
   **/
  function loadGrantAgreement() {
    return $scope.load({
      url: '/Int/Application/Agreement/' + $scope.section.grantApplicationId,
      set: 'grantAgreement'
    })
      .then(function () {
        return $timeout(function () {
          for (var i = $scope.grantAgreement.Versions; i > 0; i--) {
            $scope.section.versions.push(i);
          }
          $scope.grantAgreement.Version = $scope.grantAgreement.Versions;
        });
      });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadGrantAgreement()
    ])
      .catch(angular.noop);
  }

  $scope.changeVersion = function () {
    $scope.broadcast('refresh', { force: true });
  }
  
  init();
});
