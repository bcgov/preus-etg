app.controller('NotificationType', function ($scope, $attrs, $controller, uiTinymceConfig, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'NotificationType',
    save: {
      url: '/Int/Admin/Notification/Type',
      set: 'ngDialogData.NotificationType',
      method: $scope.ngDialogData.NotificationType.Id ? 'PUT' : 'POST',
      data: function () {
        if (!$scope.ngDialogData.NotificationType.NotificationTemplate) {
          $scope.ngDialogData.NotificationType.NotificationTemplate = {
            Caption: "",
            EmailSubject: "",
            EmailBody: ""
          };
        } else {
          $scope.ngDialogData.NotificationType.NotificationTemplate.Caption = $scope.ngDialogData.NotificationType.Caption;
        }

        return $scope.ngDialogData.NotificationType;
      }
    },
    onSave: function (event, data) {
      $scope.ngDialogData.NotificationType = data.response.data;
      return $scope.confirm($scope.ngDialogData.NotificationType);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.tinymceOptions = {
    plugins: 'link image code autoresize preview fullscreen lists advlist anchor',
    toolbar: 'undo redo | bold italic | formatselect | alignleft aligncenter alignright | outdent indent | numlist bullist | anchor | preview | fullscreen | code | variablekeywords',
    force_br_newlines: false,
    force_p_newlines: false,
    forced_root_block: '',
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
          for (let item of $scope.VariableKeywords) {
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

  var widthOfModal = "70%";

  /**
   * Initialize the data for the form
   * @function loadNotificationType
   * @param {int} id
   * @returns {void}
   **/
  function loadNotificationType(id) {
    return $scope.load({
      url: '/Int/Admin/Notification/Type/' + id,
      set: 'ngDialogData.NotificationType',
      condition: $scope.ngDialogData.NotificationType.Id && $scope.ngDialogData.NotificationType.Id != 0
    })
      .then(function () {
        console.debug("Success: NotificationType");
        // open rules tab by default
        $scope.openTab('rules-block');
        document.getElementById("rules-tab").className += " active";
      })
      .catch(angular.noop);
  }

  /**
   * Opens tab for viewing 
   * @function openTab
   * @param {string} tabName
   * @param {action} event
   * @returns {void}
   **/
  $scope.openTab = function (tabName, event) {
    var i, tabcontent, tablinks;

    // close tab content/block
    tabcontent = document.getElementsByClassName("tab-content");
    for (i = 0; i < tabcontent.length; i++) {
      tabcontent[i].style.display = "none";
    }

    // change color of all other tabs
    tablinks = document.getElementsByClassName("tab-links");
    for (i = 0; i < tablinks.length; i++) {
      tablinks[i].className = tablinks[i].className.replace(" active", "");
    }

    // open content/block of current tab
    document.getElementById(tabName).style.display = "block";

    // highlight clicked tab
    if (event) event.currentTarget.className += " active";
  }

  /**
   * Initialize the data for the form
   * @function init
   * @returns {void}
   **/
  $scope.init = function () {
    $scope.Triggers = $scope.ngDialogData.Triggers;
    $scope.ApplicationStates = $scope.ngDialogData.ApplicationStates;
    $scope.ResendRules = $scope.ngDialogData.ResendRules;
    $scope.ApprovalRules = $scope.ngDialogData.ApprovalRules;
    $scope.ParticipantReportRules = $scope.ngDialogData.ParticipantReportRules;
    $scope.ClaimReportRules = $scope.ngDialogData.ClaimReportRules;
    $scope.CompletionReportRules = $scope.ngDialogData.CompletionReportRules;
    $scope.RecipientRules = $scope.ngDialogData.RecipientRules;
    $scope.MilestoneDates = $scope.ngDialogData.MilestoneDates;
    $scope.grantPrograms = $scope.ngDialogData.grantPrograms;
    $scope.VariableKeywords = $scope.ngDialogData.VariableKeywords;

    return loadNotificationType($scope.ngDialogData.NotificationType.Id)
      .then(function () {
        angular.element(".ngdialog-content").css("width", widthOfModal);
      })
      .catch(angular.noop);
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
        name: $scope.ngDialogData.NotificationType.Caption,
        description: $scope.ngDialogData.NotificationType.Description,
        subject: $scope.ngDialogData.NotificationType.NotificationTemplate.EmailSubject,
        body: $scope.ngDialogData.NotificationType.NotificationTemplate.EmailBody,
        notificationTriggerId: $scope.ngDialogData.NotificationType.NotificationTriggerId
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
        question: 'Do you want to send a test email of the "' + $scope.ngDialogData.NotificationType.Caption + '" to "' + $scope.ngDialogData.user + '"?',
        grantPrograms: $scope.grantPrograms,
        applicationStates: $scope.ApplicationStates.InternalStates,
        applicationState: null
      }
    })
      .then(function (data) {
        return $scope.ajax({
          url: '/Int/Admin/Notification/Type/Test',
          method: 'POST',
          data: {
            name: $scope.ngDialogData.NotificationType.Caption,
            description: $scope.ngDialogData.NotificationType.Description,
            subject: $scope.ngDialogData.NotificationType.NotificationTemplate.EmailSubject,
            body: $scope.ngDialogData.NotificationType.NotificationTemplate.EmailBody,
            grantProgramId: data.grantProgramId,
            applicationStateInternal: data.applicationState,
            notificationTriggerId: $scope.ngDialogData.NotificationType.NotificationTriggerId
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

  /**
   * Make AJAX request to delete the notification type from the datasource.
   * @function deleteNotification
   * @returns {Promise}
   **/
  $scope.deleteNotification = function () {
    return $scope.confirmDialog('Delete Notification', 'Do you want to delete this notification?')
      .then(function () {
        return $scope.ajax({
          url: '/Int/Admin/Notification/Type/Delete',
          method: 'PUT',
          data: $scope.ngDialogData.NotificationType
        })
          .then(function () {
            $scope.ngDialogData.NotificationType.IsDeleted = true;
            return $scope.confirm($scope.ngDialogData.NotificationType);
          });
      }).catch(angular.noop);
  };

  $scope.init();
});
