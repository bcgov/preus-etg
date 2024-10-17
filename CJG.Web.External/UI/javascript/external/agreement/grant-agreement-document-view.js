app.controller('AgreementAcceptDocumentView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId,
    agreementDocument: $attrs.ngAgreementDocument
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make an AJAX request to fetch the agreement document data.
   * @function loadAgreementDocument
   * @returns {Promise}
   **/
  function loadAgreementDocument() {
    return $scope.load({
      url: '/Ext/Agreement/' + $scope.section.agreementDocument + '/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  /**
   * Make an AJAX request to accept the agreement document.
   * @function accept
   * @returns {Promise}
   **/
  $scope.accept = function () {
    return $scope.ajax({
      url: '/Ext/Agreement/Accept/' + $scope.section.agreementDocument,
      method: 'PUT',
      data: $scope.model
    })
      .then(function () {
        window.location = '/Ext/Agreement/Review/View/' + $scope.section.grantApplicationId;
      })
      .catch(angular.noop);
  }

  loadAgreementDocument()
    .catch(angular.noop);
});
