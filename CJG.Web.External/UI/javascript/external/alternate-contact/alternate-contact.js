app.controller('AlternateContactView', function ($scope, $attrs, $controller) {
  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId,
    save: {
      url: '/Ext/Application/AlternateContact/',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      set: 'model'
    },
    onSave: function (event, data) {
      if (data.response.data.RedirectURL)
        window.location = data.response.data.RedirectURL;
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  function loadAlternateContact() {
    return $scope.load({
      url: '/Ext/Application/AlternateContact/Data/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  loadAlternateContact()
    .catch(angular.noop);
});
