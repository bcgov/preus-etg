app.controller('CoverLetter', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'CoverLetter',
    displayName: 'Cover Letter',
    loaded: function () {
      return $scope.model && $scope.model.Body;
    },
    onRefresh: function () {
      return loadCoverLetter().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for cover letter data.
   * @function loadCoverLetter
   * @returns {Promise}
   **/
  function loadCoverLetter() {
    return $scope.load({
      url: '/Int/Application/Agreement/Cover/Letter/' + $scope.grantAgreement.Id + ($scope.grantAgreement.Version ? '/' + $scope.grantAgreement.Version : ''),
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
      loadCoverLetter()
    ]).catch(angular.noop);
  };
});
