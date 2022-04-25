app.controller('ApplicationDetailsView', function($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  function loadApplicationDetails() {
    return $scope.load({
      url: '/Ext/Application/Details/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  function loadAlternateUsers() {
    return $scope.load({
      url: '/Ext/Application/AlternateUsers/' + $scope.section.grantApplicationId,
      set: 'alternateUsers'
    });
  }

  $scope.showComponent = function($event) {
    var section = $event.currentTarget.nextElementSibling;
    var icon = $event.currentTarget.getElementsByClassName("icon")[0];
    if (section.classList.contains('ng-hide')) {
      section.classList.remove('ng-hide');
      section.classList.add('ng-show');
      icon.classList.remove('down-arrow');
      icon.classList.add('up-arrow');
    } else {
      section.classList.add('ng-hide');
      section.classList.remove('ng-show');
      icon.classList.add('down-arrow');
      icon.classList.remove('up-arrow');
    }
  }

  $scope.changeApplicationContact = function () {
    const selectedNewUserId = $scope.model.SelectedNewUser;
    const selectedUser = $scope.alternateUsers.filter(a => a.Key === selectedNewUserId).pop().Value;

    if (selectedNewUserId === 0)
      return angular.noop;

    return $scope.confirmDialog('Change Application Contact', '<p>Please confirm that you\'d like to assign this application to <strong>' + selectedUser + '</strong>.</p>')
      .then(function () {
        return $scope.ajax({
          url: '/Ext/Application/AlternateUsers/ChangeUser/',
          method: 'PUT',
          data: {
            Id: $scope.section.grantApplicationId,
            RowVersion: $scope.model.RowVersion,
            ApplicantContactId: selectedNewUserId
          }
        });
      })
      .then(function (response) {
        if (response.data.RedirectURL)
          window.location = response.data.RedirectURL;
      })
      .catch(angular.noop);
  }

  function init() {
    return Promise.all([
      loadApplicationDetails(),
      loadAlternateUsers()
    ]).catch(angular.noop);
  }

  init();
});
