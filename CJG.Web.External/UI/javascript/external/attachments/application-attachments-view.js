app.controller('ApplicationAttachmentsView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ApplicationAttachmentsView',
    displayName: 'Application Attachments',
    save: {
      url: function () {
        return '/Ext/Application/Attachments/' + $scope.section.grantApplicationId;
      },
      method: 'PUT',
      dataType: 'file',
      data: function () {
        var files = [];
        var attachments = $scope.section.attachments.filter(function (attachment) {
          if (typeof (attachment.File) !== 'undefined') {
            attachment.Index = files.length;
            files.push(attachment.File);
          }
          return attachment;
        });
        var model = {
          files: files,
          attachments: JSON.stringify(attachments)
        };

        return model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.grantFile.RowVersion;
    },
    onSave: function () {
      $scope.section.attachments = [];
      window.location = $scope.section.redirectUrl;
    },
    onRefresh: function () {
      $scope.section.attachments = [];
      return loadAttachments().catch(angular.noop);
    },
    onCancel: function () {
      $scope.section.attachments = [];
    },
    grantApplicationId: $attrs.ngGrantApplicationId,
    redirectUrl: $attrs.ngRedirectUrl,
    attachments: []
  };

  $scope.grantFile = {
    Id: $attrs.grantApplicationId
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to fetch attachment data
   * @function loadAttachments
   * @returns {Promise}
   **/
  function loadAttachments() {
    return $scope.load({
      url: '/Ext/Application/Attachments/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

  /**
   * Initialize form data.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadAttachments()
    ]).catch(angular.noop);
  }

  /**
   * Mark the attachment for deletion.
   * @function removeAttachment
   * @param {any} index index
   * @returns {Promise} dialog
   */
  $scope.removeAttachment = function (index) {
    var attachment = $scope.model.Attachments[index];
    return $scope.confirmDialog('Remove Attachment', 'Do you want to delete this attachment "' + attachment.FileName + '"?')
      .then(function (response) {
        if (response === true) {
          var attachment = $scope.model.Attachments.splice(index, 1)[0];
          attachment.Delete = true;
          var i = $scope.section.attachments.indexOf(attachment);
          if (i === -1) {
            $scope.section.attachments.push(attachment);
          } else if (attachment.Id === 0) {
            $scope.section.attachments.splice(i, 1);
          }
        }
      }).catch(angular.noop);
  };

  // Count the number of attachments that are 'Applicant Attachments'
  $scope.getTotals = function () {
    var attachments = $scope.model.Attachments;
    if (attachments === undefined || attachments == null)
      return 0;

    var attachmentTotal = attachments.filter(d => d.AttachmentType === 0).length;
    return attachmentTotal;
  }

  /**
   * Open modal file uploader popup and then add the new file to the model.
   * @function addAttachment
   * @returns {void}
   **/
  $scope.addAttachment = function () {
    return $scope.attachmentDialog('Add Attachment', {
      Id: 0,
      FileName: '',
      Description: '',
      File: {},
      AttachmentType: 0
    })
      .then(function (attachment) {
        $scope.model.Attachments.push(attachment);
        $scope.section.attachments.push(attachment);
      })
      .catch(angular.noop);
  };

  /**
   * Open modal file uploader popup and allow user to update the attachment and/or file.
   * @function changeAttachment
   * @param {any} attachment - The attachment to update.
   * @returns {void}
   */
  $scope.changeAttachment = function (attachment) {
    $scope.section.attachment = attachment;
    return $scope.attachmentDialog('Update Attachment', attachment, false)
      .then(function (attachment) {
        if ($scope.section.attachments.indexOf(attachment) === -1) {
          $scope.section.attachments.push(attachment);
        }
      })
      .catch(angular.noop);
  };

  /**
   * Cancel the changes to attachments and redirect to specified URL.
   * @function cancel
   * @returns {void}
   **/
  $scope.cancel = function () {
    window.location = $scope.section.redirectUrl;
  }

  init();
});
