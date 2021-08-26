var utils = require('../../shared/utils');
app.controller('ApplicationTrainingProviderView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'TrainingProvider',
    displayName: 'Training Provider',
    save: {
      url: '/Ext/Training/Provider',
      method: function () {
        return $scope.model.Id > 0 ? 'PUT' : 'POST';
      },
      dataType: 'file',
      data: function () {
        var files = [];
        if ($scope.model.PrivateSectorValidationType) {
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

        $scope.model.ContactPhone = $scope.model.ContactPhoneAreaCode +
          $scope.model.ContactPhoneExchange +
          $scope.model.ContactPhoneNumber;

        return {
          files: files,
          component: angular.toJson($scope.model)
        };
      }
    },
    onSave: function () {
      window.location = '/Ext/Application/Overview/View/' + $scope.section.grantApplicationId;
    },
    grantApplicationId: $attrs.ngGrantApplicationId,
    trainingProviderId: $attrs.ngTrainingProviderId
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

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
 * Make an AJAX request to get the training provider data
 * @function loadTrainingProvider
 * @returns {Promise}
 **/
  function loadTrainingProvider() {
    return $scope.load({
      url: '/Ext/Training/Provider/' + $scope.section.grantApplicationId + '/' + $scope.section.trainingProviderId,
      set: 'model'
    }).then(function () {
      $scope.model.MaxUploadSize = utils.Attachment.MaxUploadSize;
    }).catch(angular.noop);
  };

  /**
   * Initialize the form data by making AJAX requests.
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
   * Set the training provider type name for currently selected training provider type.
   * @function setTrainingProviderTypeName
   * @returns {void}
   **/
  $scope.setTrainingProviderTypeName = function () {
    $scope.model.TrainingProviderTypeName = $scope.ProviderTypes.find(function (element) {
      return element.Id == $scope.model.TrainingProviderTypeId;
    }).Caption;
  }

  /**
   * Determine if this training provider type requires additional attachments.
   * @function isPrivateSector
   * @param {int} trainingProviderTypeId - The training provider type id.
   * @returns {boolean}
   */
  $scope.isPrivateSector = function (trainingProviderTypeId) {
    if ($scope.ProviderTypes)
      for (let i = 0; i < $scope.ProviderTypes.length; i++) {
        let trainingProviderType = $scope.ProviderTypes[i];
        if (trainingProviderType.Id === trainingProviderTypeId && $scope.model) {
          $scope.model.PrivateSectorValidationType = trainingProviderType.PrivateSectorValidationType;
          return trainingProviderType.PrivateSectorValidationType;
        }
      }
    return false;
  };

  /**
   * Download the specified attachment.
   * @function downloadAttachment
   * @param {any} attachmentId - The attachment id.
   * @returns {void}
   */
  $scope.downloadAttachment = function (attachmentId) {
    window.open('/Ext/Training/Provider/' + $scope.model.Id + '/Attachment/Download/' + attachmentId);
  };

  /**
   * Open the modal file uploaded.
   * @function openAttachmentModal
   * @param {any} title - The title of the modal window.
   * @param {any} attachment - The attachment to update/add.
   * @returns {Promise} modal
   */
  function openAttachmentModal(title, attachment) {
    return ngDialog.openConfirm({
      template: '/content/dialogs/_TrainingProviderAttachment.html',
      data: {
        title: title,
        attachment: attachment
      },
      controller: ['$scope', function ($scope) {
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
      }]
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
  };

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
  };

  init();
});
