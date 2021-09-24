app.controller('ClaimAttachmentsView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'ClaimAttachmentsView',
    displayName: 'Claim Attachments',
    loaded: function () {
      return $scope.model && $scope.model.RowVersion;
    },
    preSave: function () {
      // Only save if there are new attachments.
      return $scope.section.attachments.length;
    },
    save: {
      url: '/Ext/Claim/Attachments/' + $scope.section.claimId + '/' + $scope.section.claimVersion,
      method: 'PUT',
      dataType: 'file',
      data: function () {
        var model = {
          files: $scope.section.attachments.filter(function (attachment) {
            return !attachment.Delete && typeof (attachment.File) !== 'undefined';
          }).map(function (attachment, index) {
            attachment.Index = index; // Map object to file array.
            var file = attachment.File; // Add file to array.
            return file;
          }),
          attachments: JSON.stringify($scope.section.attachments)
        };

        return model;
      },
      backup: true
    },
    onSave: function () {
      $scope.section.attachments = [];
      return $scope.resyncClaim($scope.model.RowVersion);
    },
    onRefresh: function () {
      $scope.section.attachments = [];
      return loadAttachments().catch(angular.noop);
    },
    onCancel: function () {
      $scope.section.attachments = [];
    },
    claimId: $attrs.ngClaimId,
    claimVersion: $attrs.ngClaimVersion,
    attachments: []
  };
  
  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to fetch attachment data
   * @function loadAttachments
   * @returns {Promise}
   **/
  function loadAttachments() {
    return $scope.load({
      url: '/Ext/Claim/Attachments/' + $scope.section.claimId + '/' + $scope.section.claimVersion,
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
          $scope.displayErrors();
        }
      }).catch(angular.noop);
  };

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
      File: {}
    })
      .then(function (attachment) {
        $scope.model.Attachments.push(attachment);
        $scope.section.attachments.push(attachment);
        $scope.displayErrors();
      })
      .catch(angular.noop);
  };

  /**
   * Open modal file uploader popup and allow user to updte the attachment and/or file.
   * @function changeAttachment
   * @param {any} attachment - The attachment to update.
   * @returns {void}
   */
  $scope.changeAttachment = function (attachment) {
    $scope.section.attachment = attachment;
    return $scope.attachmentDialog('Update Attachment', attachment)
      .then(function (attachment) {
        if ($scope.section.attachments.indexOf(attachment) === -1) {
          $scope.section.attachments.push(attachment);
        }
        $scope.displayErrors();
      })
      .catch(angular.noop);
  };
  
  init();
});
