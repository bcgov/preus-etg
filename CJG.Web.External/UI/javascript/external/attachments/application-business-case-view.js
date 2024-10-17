app.controller('ApplicationBusinessCaseView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ApplicationBusinessCaseView',
    displayName: 'Business Case',
    save: {
      url: function () {
        return '/Ext/Application/Business/Case/' + $scope.section.grantApplicationId;
      },
      method: 'PUT',
      dataType: 'file',
      data: function () {
          var model = {
            file: $scope.model.BusinessCaseDocument.File,
            description: $scope.model.BusinessCaseDocument.Description
          };
        return model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.grantFile.RowVersion;
    },
    onSave: function () {
      $scope.model.BusinessCaseDocument = {};
      window.location = $scope.section.redirectUrl;
    },
    onRefresh: function () {
      $scope.model.BusinessCaseDocument = {};
      return loadAttachments().catch(angular.noop);
    },
    onCancel: function () {
      $scope.model.BusinessCaseDocument = {};
    },
    grantApplicationId: $attrs.ngGrantApplicationId,
    redirectUrl: $attrs.ngRedirectUrl,
  };

  $scope.grantFile = {
    Id: $attrs.grantApplicationId
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to fetch attachment data
   * @function loadAttachment
   * @returns {Promise}
   **/
  function loadAttachment() {
    return $scope.load({
      url: '/Ext/Application/Business/Case/' + $scope.section.grantApplicationId,
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
      loadAttachment()
    ]).catch(angular.noop);
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
    })
      .then(function (attachment) {
        $scope.model.BusinessCaseDocument = attachment;
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
        $scope.model.BusinessCaseDocument = attachment;
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
