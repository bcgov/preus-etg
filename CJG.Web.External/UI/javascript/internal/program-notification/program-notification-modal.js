app.controller('ProgramNotificationModal', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ProgramNotificationModal',
    save: {
      url: '/Int/Admin/Program/Notification',
      method: function () {
        return !$scope.model.Id ? 'POST' : 'PUT';
      },
      data: function () {
        return $scope.model;
      }
    },
    onSave: function (event, data) {
      var create = !$scope.ngDialogData.programNotificationId;
      if (create)
        $scope.ngDialogData.programNotificationId = $scope.model.Id;
      return $timeout(function () {
        $scope.setAlert({ response: { status: 200 }, message: 'Program Notification has been ' + (create ? 'created' : 'updated') + ' successfully' });
        $scope.backup();
      });
    }
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  $scope.showTemplate = false;

  $scope.tinymceOptions = {
    plugins: 'link image code autoresize preview fullscreen lists advlist anchor',
    toolbar: 'undo redo | bold italic | formatselect | alignleft aligncenter alignright | outdent indent | numlist bullist | anchor | preview | fullscreen | code',
    force_br_newlines: false,
    force_p_newlines: false,
    forced_root_block: '',
    setup: function (ed) {
      ed.on('init', function (ed) {
        $('div.tox-tinymce-aux').css('z-index', '99999');
      });
    }
  };

  /**
   * Make AJAX request to load program notification data.
   * @function loadProgramNotification
   * @returns {Promise}
   **/
  function loadProgramNotification() {
    if ($scope.ngDialogData.programNotificationId) {
      return $scope.load({
        url: '/Int/Admin/Program/Notification/' + $scope.ngDialogData.programNotificationId,
        set: 'model'
      });
    }
    else {
      return $timeout(function () {
        $scope.model = {
          Id: 0,
          SendDate: new Date(),
          Recipients: [],
          Template: {},
          VariableKeywords: []
        };
      });
    }
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadProgramNotification()
    ])
      .then(function () {
        $scope.backup();
      })
      .catch(angular.noop);
  }

  /**
   * Make AJAX request to delete the program notification.
   * @function deleteProgramNotification
   * @returns {Promise} promise
   */
  $scope.deleteProgramNotification = function () {
    return ngDialog.openConfirm({
      template: '/content/dialogs/_Confirmation.html',
      data: {
        title: 'Delete Program Notification',
        question: 'Do you want to delete this program notification?'
      }
    }).then(function () {
      return $scope.ajax({
        url: '/Int/Admin/Program/Notification/Delete',
        method: function () {
          return 'PUT';
        },
        data: $scope.model
      }) .then(function () {
        return $scope.confirm('The Program Notification has been deleted successfully');
      });
    }).catch(angular.noop);
  };

  /**
   * get program notification recipient.
   * @function getProgramNotificationRecipient
   * @param {int} grantProgramId - The grant program id.
   * @returns {object}
   **/
  $scope.getProgramNotificationRecipient = function (grantProgramId) {
    if (!$scope.model.Recipients) return {};
    for (let i = 0; i < $scope.model.Recipients.length; i++) {
      var recipient = $scope.model.Recipients[i];
      if (recipient.ProgramNotificationId === $scope.ngDialogData.programNotificationId && recipient.GrantProgramId === grantProgramId)
        return recipient;
    }
    var model = {
      ProgramNotificationId: $scope.ngDialogData.programNotificationId,
      GrantProgramId: grantProgramId,
      ApplicantOnly: false,
      SubscriberOnly: false
    };
    $scope.model.Recipients.push(model);
    return model;
  };

  /**
   * check program notification recipient.
   * @function checkProgramNotificationRecipient
   * @param {int} grantProgramId - The grant program id.
   * @param {strnig} recipientType - The type of recipient.
   * @returns {object}
   **/
  $scope.checkProgramNotificationRecipient = function (grantProgramId, recipientType) {
    var recipient = $scope.getProgramNotificationRecipient(grantProgramId);
    return recipient[recipientType];
  };

  /**
   * update program notification recipient.
   * @function updateProgramNotificationRecipient
   * @param {int} grantProgramId - The grant program id.
   * @param {strnig} recipientType - The type of recipient.
   * @param {strnig} recipientTypeOff - The type of recipient to be turn off.
   * @returns {void}
   **/
  $scope.updateProgramNotificationRecipient = function (grantProgramId, recipientType, recipientTypeOff) {
    var recipient = $scope.getProgramNotificationRecipient(grantProgramId);
    recipient[recipientType] = !recipient[recipientType];
    if (recipient[recipientType])
      recipient[recipientTypeOff] = false;
  };

  /**
   * toggle all program notification recipient.
   * @function toggleAllProgramNotificationRecipient
   * @returns {void}
   **/
  $scope.toggleAllProgramNotificationRecipient = function () {
    if ($scope.model.AllApplicants) {
      for (let i = 0; i < $scope.ngDialogData.applicants.NumberOfApplicantsPerGrantProgram.length; i++) {
        var grantProgram = $scope.ngDialogData.applicants.NumberOfApplicantsPerGrantProgram[i];
        var recipient = $scope.getProgramNotificationRecipient(grantProgram.GrantProgramId);
        recipient.SubscriberOnly = false;
        recipient.ApplicantOnly = false;
      }
    }
  };

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
      url: '/Int/Admin/Program/Notification/Preview/View',
      method: 'POST',
      data: {
        name: $scope.model.Caption,
        description: $scope.model.Description,
        subject: $scope.model.Template.EmailSubject,
        body: $scope.model.Template.EmailBody
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
   * Send the test program notification.
   * @function test
   * @returns {Promise}
   **/
  $scope.test = function () {
    return ngDialog.openConfirm({
      template: '/content/dialogs/_Confirmation.html',
      data: {
        title: 'Test Program Notification',
        question: 'Do you want to send a test email of the "' + $scope.model.Caption + '" to "' + $scope.ngDialogData.user + '"?'
      }
    })
      .then(function () {
        return $scope.ajax({
          url: '/Int/Admin/Program/Notification/Test',
          method: 'POST',
          data: {
            name: $scope.model.Caption,
            subject: $scope.model.Template.EmailSubject,
            body: $scope.model.Template.EmailBody
          }
        })
          .then(function () {
            return $timeout(function () {
              $scope.setAlert({ response: { status: 200 }, message: 'The Test Program Notification has been sent to "' + $scope.ngDialogData.user + '"' });
            });
          });
      })
      .catch(angular.noop);
  };

  /**
   * Push the program notification to the queue.
   * @function send
   * @returns {Promise}
   **/
  $scope.send = function () {
    return ngDialog.openConfirm({
      template: '/content/dialogs/_Confirmation.html',
      data: {
        title: 'Send Program Notification',
        question: 'Do you want to add the "' + $scope.model.Caption + '" to the queue? The next time the Notification Service runs it will send an email out to ' + $scope.totalApplicants() + ' applicants'
      }
    })
      .then(function () {
        return $scope.ajax({
          url: '/Int/Admin/Program/Notification/Send/' + $scope.model.Id,
          method: 'POST'
        })
          .then(function () {
            return $scope.confirm('The Program Notification has been pushed to the queue');
          });
      })
      .catch(angular.noop);
  };

  /**
   * Calculate the total number of applicants.
   * @function totalApplicants
   * @returns {int}
   **/
  $scope.totalApplicants = function () {
    if ($scope.model.AllApplicants) {
      return $scope.ngDialogData.applicants.NumberOfApplicants;
    } else {
      var total = 0;
      for (let i = 0; i < $scope.ngDialogData.applicants.NumberOfApplicantsPerGrantProgram.length; i++) {
        var grantProgram = $scope.ngDialogData.applicants.NumberOfApplicantsPerGrantProgram[i];
        var recipient = $scope.getProgramNotificationRecipient(grantProgram.GrantProgramId);
        if (recipient.SubscriberOnly) {
          total += grantProgram.NumberOfSubscribers;
        } else if (recipient.ApplicantOnly) {
          total += grantProgram.NumberOfApplicants;
        }
      }
      return total;
    }
  }

  /**
   * Check if the program notification has changed.
   * @function checkProgramNotificationChanged
   * @returns {Promise} promise
   */
  $scope.programNotificationChanged = function () {
    return $scope.section.backup && !angular.equals($scope.section.backup, $scope.model);
  };

  init();
});
