app.controller('ExternalHomeController', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for grant programs data
   * @function loadGrantPrograms
   * @returns {Promise}
   **/
  function loadGrantPrograms() {
    return $scope.load({
      url: '/Ext/Home/Grant/Programs',
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
    ])
      .catch(angular.noop);
  }

  /**
   * Get the filtered applications.
   * @function getApplications
   * @param {string} pageKeyword - The search filter keyword.
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items in a page.
   * @returns {Promise}
   **/
  $scope.getApplications = function (pageKeyword, page, quantity) {
    return $scope.ajax({
      url: '/Ext/Home/Grant/Applications/' + page + '/' + quantity
    })
      .then(function (response) {
        return response.data;
      });
  };

  $scope.deleteApplication = function (grantApplicationId, rowVersion) {
    return $scope.confirmDialog('Delete Application', 'Are you sure you want to delete this application?')
      .then(function () {
        return $scope.ajax({
          url: '/Ext/Application/Delete/' + grantApplicationId + '?rowVersion=' + encodeURI(rowVersion),
          method: 'PUT'
        })
          .then(function () {
            angular.element('div[data-id=\'' + grantApplicationId + '\']').remove();
          })
          .catch(angular.noop);
      })
      .catch(angular.noop);
  };

  init();
});
