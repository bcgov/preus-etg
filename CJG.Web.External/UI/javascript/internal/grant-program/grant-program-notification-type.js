app.controller('GrantProgramNotificationType', function ($scope, $attrs, $controller, $timeout, ngDialog, Utils) {
  $scope.section = {
    name: 'GrantProgramNotificationType',
    loaded: function () {
      return $scope.ngDialogData.model.Id === $scope.$parent.model.Id && $scope.ngDialogData.model.RowVersion && $scope.ngDialogData.model.RowVersion === $scope.$parent.model.RowVersion;
    },
    onRefresh: function () {
      return loadNotification().catch(angular.noop);
    },
    editing: $scope.ngDialogData.editing
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.tinymceOptions = {
    plugins: 'link image code autoresize preview fullscreen lists advlist anchor',
    toolbar: 'undo redo | bold italic | formatselect | alignleft aligncenter alignright | outdent indent | numlist bullist | anchor | preview | fullscreen | code | variablekeywords',
    setup: function (ed) {
      ed.on('init', function (ed) {
        $('div.tox-tinymce-aux').css('z-index', '99999');
      });

      // adding variable keywords menu to toolbar
      ed.ui.registry.addMenuButton('variablekeywords', {
        type: 'menubutton',
        text: 'Variable',
        fetch: function (callback) {
          var menuItems = [];
          for (let item of $scope.ngDialogData.variableKeywords) {
            menuItems.push({
              type: 'menuitem',
              text: item.Value,
              onAction: function (_) {
                ed.insertContent('@Model.' + item.Key)
              }
            });
          }
          callback(menuItems);
        }
      });
    }
  };

  $(document).on('focusin', function (e) {
    if ($(e.target).closest(".mce-window").length)
      e.stopImmediatePropagation();
  });

  $scope.showTemplate = false;

  /**
   * Make AJAX request for the grant program notification type
   * @function loadNotificationType
   * @returns {Promise}
   **/
  function loadNotificationType() {
    return $scope.load({
      url: '/Int/Admin/Notification/Type/' + $scope.ngDialogData.model.NotificationTypeId,
      set: 'notificationType'
    });
  }

  /**
   * Make AJAX request to delete the grant program notification type
   * @function deleteGrantProgramNotificationType
   * @returns {Promise}
   **/
  $scope.deleteGrantProgramNotificationType = function () {
    $scope.ngDialogData.model.ToBeDeleted = true;
    return $scope.confirm($scope.ngDialogData.model);
  };

  /**
   * Initialize the data for the modal
   * @function init
   * @returns {void}
   **/
  function init() {
    return Promise.all([
      loadNotificationType()
    ])
      .catch(angular.noop);
  }

  /**
   * Toggle the view
   * @function toggleTemplate
   * @param {int} state - The state.
   * @returns {void}
   **/
  $scope.toggleTemplate = function (state) {
    $scope.showTemplate = state;
  };

  /**
   * Open a new tab and display the email.
   * @function preview
   * @returns {Promise}
   **/
  $scope.preview = function () {
    return $scope.ajax({
      url: '/Int/Admin/Notification/Type/Preview/View',
      method: 'POST',
      data: {
        name: $scope.ngDialogData.model.Caption,
        description: $scope.ngDialogData.model.Description,
        subject: $scope.ngDialogData.model.NotificationTemplate.EmailSubject,
        body: $scope.ngDialogData.model.NotificationTemplate.EmailBody,
        notificationTriggerId: $scope.ngDialogData.model.NotificationTriggerId
      }
    })
      .then(function (response) {
        var w = window.open('about:blank');
        w.document.open();
        w.document.write(response.data);
        w.document.close();
      })
      .catch(angular.noop);
  };

  /**
   * Send the test notification.
   * @function test
   * @returns {Promise}
   **/
  $scope.test = function () {
    return ngDialog.openConfirm({
      template: '/content/dialogs/_TestNotification.html',
      data: {
        title: 'Test Notification',
        question: 'Do you want to send a test email of the "' + $scope.ngDialogData.model.Caption + '" to "' + $scope.ngDialogData.user + '"?',
        grantPrograms: $scope.ngDialogData.grantPrograms,
        applicationStates: $scope.ngDialogData.applicationStates,
        applicationState: null,
        grantProgramKey: 'Id',
        grantProgramValue: 'Caption'
      }
    })
      .then(function (data) {
        return $scope.ajax({
          url: '/Int/Admin/Notification/Type/Test',
          method: 'POST',
          data: {
            name: $scope.ngDialogData.model.Caption,
            description: $scope.ngDialogData.model.Description,
            subject: $scope.ngDialogData.model.NotificationTemplate.EmailSubject,
            body: $scope.ngDialogData.model.NotificationTemplate.EmailBody,
            grantProgramId: data.grantProgramId,
            applicationStateInternal: data.applicationState,
            notificationTriggerId: $scope.ngDialogData.model.NotificationTriggerId
          }
        })
          .then(function () {
            return $timeout(function () {
              $scope.setAlert({ response: { status: 200 }, message: 'The Test Notification has been sent to "' + $scope.ngDialogData.user + '"' });
            });
          });
      })
      .catch(angular.noop);
  };

  init();
});
