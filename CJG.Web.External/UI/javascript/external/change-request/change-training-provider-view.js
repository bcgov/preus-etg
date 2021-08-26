app.controller('ChangeTrainingProviderView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'ChangeTrainingProviderView',
    save: {
      url: '/Ext/Agreement/Change/Training/Provider',
      method: function () {
        return $scope.model.Id ? 'PUT' : 'POST';
      },
      dataType: 'file',
      data: function () {
        var files = [];
        if ($scope.isPrivateSector($scope.model.TrainingProviderTypeId)) {
          if ($scope.model.CourseOutlineDocument && $scope.model.CourseOutlineDocument.File) {
            files.push($scope.model.CourseOutlineDocument.File);
            $scope.model.CourseOutlineDocument.Index = files.length - 1;
          }
          if ($scope.model.ProofOfQualificationsDocument && $scope.model.ProofOfQualificationsDocument.File) {
            files.push($scope.model.ProofOfQualificationsDocument.File);
            $scope.model.ProofOfQualificationsDocument.Index = files.length - 1;
          }
        }
        if ($scope.model.TrainingOutsideBC && $scope.model.BusinessCaseDocument && $scope.model.BusinessCaseDocument.File) {
          files.push($scope.model.BusinessCaseDocument.File);
          $scope.model.BusinessCaseDocument.Index = files.length - 1;
        }

        return {
          files: files,
          provider: angular.toJson($scope.model)
        };
      }
    },
    onSave: function (event, data) {
      return $scope.confirm($scope.model);
    },
    originalTrainingProviderId: parseInt($attrs.ngOriginalTrainingProviderId),
    loadUrl: $scope.ngDialogData.loadUrl || '/Ext/Agreement/Change/Training/Provider/'
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make an AJAX request to get the training provider.
   * @function loadProviderTypes
   * @returns {Promise}
   **/
  function loadTrainingProvider() {
    return $scope.load({
      url: $scope.section.loadUrl + $scope.section.originalTrainingProviderId,
      set: 'model'
    });
  }

  /**
   * Make an AJAX request to get an array of provider types
   * @function loadProviderTypes
   * @returns {Promise}
   **/
  function loadProviderTypes() {
    return $scope.load({
      url: '/Ext/Training/Provider/Types',
      set: 'ProviderTypes'
    });
  };

  /**
   * Make an AJAX request to get an array of countries
   * @function loadCountries
   * @returns {Promise}
   **/
  function loadCountries() {
    return $scope.load({
      url: '/Ext/Address/Countries',
      set: 'Countries'
    });
  };

  /**
   * Make an AJAX request to get an array of provinces
   * @function loadProvinces
   * @returns {Promise}
   **/
  function loadProvinces() {
    return $scope.load({
      url: '/Ext/Address/Provinces',
      set: 'Provinces'
    });
  };

  /**
   * Initialize the form data.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadProviderTypes(),
      loadCountries(),
      loadProvinces(),
      loadTrainingProvider()
    ])
      .catch(angular.noop);
  }

  /**
   * Determine if this training provider type requires attachments.
   * @function isPrivateSector
   * @param {int} trainingProviderTypeId
   * @returns {boolean}
   */
  $scope.isPrivateSector = function (trainingProviderTypeId) {
    if (!$scope.ProviderTypes) return false;

    var type = $scope.ProviderTypes.find(function (item) {
      return item.Id === trainingProviderTypeId;
    });

    if (type) {
      switch (type.PrivateSectorValidationType) {
        case (0): // No additional documents required.
          return false;
        case (1): // Additional documents required.
        case (2): // TODO: Additional documents required if past a setting date.
        default:
          return true;
      }
    }
    return false;
  }

  /**
   * Download the specified attachment.
   * @function downloadAttachment
   * @param {any} attachmentId - The attachment id.
   * @returns {void}
   */
  $scope.downloadAttachment = function (attachmentId) {
    window.open('/Ext/Training/Provider/' + $scope.model.Id + '/Attachment/Download/' + attachmentId);
  }

  /**
   * Open the modal file uploaded.
   * @function openAttachmentModal
   * @param {any} title - The title of the modal window.
   * @param {any} attachment - The attachment to update/add.
   * @returns {Promise}
   */
  function openAttachmentModal(title, attachment) {
    return ngDialog.openConfirm({
      template: '/content/dialogs/_TrainingProviderAttachment.html',
      data: {
        title: title,
        attachment: attachment
      },
      controller: function ($scope) {
        /**
         * Return the selected file in the promise.
         * @function save
         * @returns {Promise}
         **/
        $scope.save = function () {
          $scope.confirm($scope.ngDialogData.attachment);
        };

        /**
         * Manually call the file select.
         * @function chooseFile
         * @returns {void}
         **/
        $scope.chooseFile = function () {
          var $input = angular.element('#training-provider-upload');
          $input.trigger('click');
        }

        /**
         * Set the selected file as the active attachment.
         * @function fileChanged
         * @param {any} $files
         * @returns {void}
         */
        $scope.fileChanged = function ($files) {
          if ($files.length) {
            $scope.ngDialogData.attachment.File = $files[0];
            $scope.ngDialogData.attachment.FileName = $scope.ngDialogData.attachment.File.name;
          }
        }
      }
    });
  }

  /**
   * Open modal file uploader popup and then add the new file to the model.
   * @function addAttachment
   * @param {string} prop - The name of the property for this attachment.
   * @returns {void}
   **/
  $scope.addAttachment = function (prop) {
    openAttachmentModal('Add Attachment', {
      Id: 0,
      FileName: '',
      Description: '',
      File: {}
    })
      .then(function (attachment) {
        $scope.model[prop] = attachment;
      })
      .catch(angular.noop);
  }

  /**
   * Open modal file uploader popup and allow user to updte the attachment and/or file.
   * @function changeAttachment
   * @param {string} prop - The name of the property for this attachment.
   * @returns {void}
   */
  $scope.changeAttachment = function (prop) {
    openAttachmentModal('Change Attachment', $scope.model[prop])
      .then(function (attachment) {
        prop = attachment;
      })
      .catch(angular.noop);
  }

  /**
   * Confirm the user wants to delete the training provider.
   * Make an AJAX request to delete the training provider.
   * @function removeTrainingProvider
   * @returns {Promise}
   **/
  $scope.removeTrainingProvider = function () {
    return $scope.confirmDialog('Remove Training Provider', 'Do you want to remove this training provider from the change request?')
      .then(function () {
        return $scope.ajax({
          url: '/Ext/Agreement/Change/Training/Provider/Delete',
          method: 'PUT',
          data: $scope.model
        })
          .then(function (response) {
            return $scope.confirm(response.data);
          });
      })
      .catch(angular.noop);
  }

  $scope.disableRemovedTrainingRequest = function() {
    if ($scope.model.Id && $scope.model.Id !== 0) {
      return false;
    }
    return true;
  }
  init();
});
