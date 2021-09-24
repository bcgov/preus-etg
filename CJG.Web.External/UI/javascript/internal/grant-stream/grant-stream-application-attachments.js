app.controller('GrantStreamApplicationAttachments', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'GrantStreamApplicationAttachments',
    save: {
      url: '/Int/Admin/Grant/Stream/Application/Attachments',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.section.isLoaded;
    },
    onSave: function () {
      $scope.emit('refresh');
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.$watch('$parent.model', function (newValue, oldValue) {
    $scope.model = newValue;
  });

  /**
   * Open a new tab and display the message.
   * @function preview
   * @returns {Promise}
   **/
  $scope.preview = function () {
    return $scope.ajax({
      url: '/Int/Admin/Grant/Stream/Preview',
      method: 'POST',
      data: {
        title: $scope.model.Name,
        message: $scope.model.AttachmentsUserGuidance
      }
    })
      .then(function (response) {
        var w = window.open('about:blank');
        w.document.open();
        w.document.write(response.data);
        w.document.close();
      })
      .catch(angular.noop);
  }

  /**
   * Enable or Disable the application attachments.
   * Change the section to edit mode.
   * @function changeAttachmentsEnabled
   * @returns {Promise}
   **/
  $scope.changeAttachmentsEnabled = function () {
    if (!$scope.section.editing) {
      return $scope.edit()
        .then(function () {
          $scope.section.backup.AttachmentsIsEnabled = !$scope.section.backup.AttachmentsIsEnabled;
        });
    }
  }
});
