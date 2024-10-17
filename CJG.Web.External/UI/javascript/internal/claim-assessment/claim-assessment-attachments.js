app.controller('ClaimAttachments', function($scope, $attrs, $controller, $timeout, $compile) {
  $scope.section = {
    name: 'ClaimAttachments',
    displayName: 'Claim Attachments',
    onRefresh: function () {
      return loadAttachments().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to fetch attachment data
   * @function loadAttachments
   * @returns {Promise}
   **/
  function loadAttachments() {
    return $scope.load({
      url: '/Int/Claim/Attachments/' + $scope.parent.claimId + '/' + $scope.parent.claimVersion,
      set: 'model'
    });
  }

  /**
   * Initialize section data.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function() {
    return Promise.all([
      loadAttachments()
    ]).catch(angular.noop);
  }

  /**
   * Mark the attachment for deletion.
   * @function removeAttachment
   * @param {int} index
   * @returns {Promise}
   */
  $scope.removeAttachment = function (index) {
    var attachment = $scope.model.ClaimAttachments[index];
    return $scope.confirmDialog('Remove Attachment', 'Do you want to delete this attachment "' + attachment.FileName + '"?')
      .then(function (response) {
        if (response !== true) {
          return;
        }
        return $scope.load({
          url: '/Int/Claim/Attachment/Delete/' + $scope.parent.claimId + '/' + $scope.parent.claimVersion + '/' + attachment.Id,
          method: 'PUT',
          set: 'model'
        }).then(function() {
          $scope.resyncClaimDetails();
        }).catch(angular.noop);
      }).catch(angular.noop);
  }

  /**
   * Mark the attachment for post or put.
   * @function uploadAttachment
   * @param {any} attachment
   * @returns {Promise}
   */
  function uploadAttachment(attachment) {
    return $scope.load({
      url: '/Int/Claim/Attachment',
      method: attachment.Id != 0 ? 'PUT' : 'POST',
      set: 'model',
      dataType: 'file',
      data: function () {
        var model = {
          claimId: $scope.parent.claimId,
          claimVersion: $scope.parent.claimVersion,
          file: attachment.File,
          attachments: JSON.stringify(attachment)
        };
        return model;
      }
    }).then(function() {
      $scope.resyncClaimDetails();
    }).catch(angular.noop);
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
      File: {}
    }).then(function (attachment) {
      uploadAttachment(attachment);
    }).catch(angular.noop);
  }
  
  /**
   * Open modal file uploader popup and allow user to updte the attachment and/or file.
   * @function changeAttachment
   * @param {any} attachment - The attachment to update.
   * @returns {void}
   */
  $scope.changeAttachment = function (attachment) {
    return $scope.attachmentDialog('Update Attachment', attachment)
      .then(function (attachment) {
        uploadAttachment(attachment);
      }).catch(angular.noop);
  }
});
