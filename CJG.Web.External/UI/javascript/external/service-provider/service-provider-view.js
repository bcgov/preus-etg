app.controller('ServiceProviderView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId,
    trainingProviderId: $attrs.ngTrainingProviderId,
    eligibleExpenseTypeId: $attrs.ngEligibleExpenseTypeId,
    save: {
      url: '/Ext/Service/Provider',
      method: function () { 
        return $scope.section.trainingProviderId > 0 ? 'PUT' : 'POST' 
      },
      dataType: 'file',
      data: function () {
        var files = [];
        if ($scope.model.ServiceProvider.CourseOutlineDocument && $scope.model.ServiceProvider.CourseOutlineDocument.File) {
          files.push($scope.model.ServiceProvider.CourseOutlineDocument.File);
          $scope.model.ServiceProvider.CourseOutlineDocument.Index = files.length - 1;
        }
        if ($scope.model.ServiceProvider.ProofOfQualificationsDocument && $scope.model.ServiceProvider.ProofOfQualificationsDocument.File) {
          files.push($scope.model.ServiceProvider.ProofOfQualificationsDocument.File);
          $scope.model.ServiceProvider.ProofOfQualificationsDocument.Index = files.length - 1;
        }
        if ($scope.model.ServiceProvider.BusinessCaseDocument && $scope.model.ServiceProvider.BusinessCaseDocument.File) {
          files.push($scope.model.ServiceProvider.BusinessCaseDocument.File);
          $scope.model.ServiceProvider.BusinessCaseDocument.Index = files.length - 1;
        }

        $scope.model.ServiceProvider.ContactPhone = $scope.model.ServiceProvider.ContactPhoneAreaCode +
          $scope.model.ServiceProvider.ContactPhoneExchange +
          $scope.model.ServiceProvider.ContactPhoneNumber;

        return {
          files: files,
          component: angular.toJson($scope.model)
        };
      }
    },
    onSave: function () {
      window.location = '/Ext/Application/Overview/View/' + $scope.section.grantApplicationId;
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make an AJAX request to fetch the service provider data.
   * @function loadServiceProvider
   * @returns {Promise}
   **/
  function loadServiceProvider() {
    return $scope.load({
      url: '/Ext/Service/Provider/' + $scope.section.trainingProviderId + '/' + $scope.section.grantApplicationId + '/' + $scope.section.eligibleExpenseTypeId,
      set: 'model'
    });
  }

  /**
   * Make an AJAX request to fetch an array of provinces.
   * @function loadServiceProvider
   * @returns {Promise}
   **/
  function loadProvinces() {
    return $scope.load({
      url: '/Ext/Address/Provinces',
      set: 'Provinces'
    });
  }

  /**
   * Make an AJAX request to fetch an array of provider type.
   * @function loadProviderTypes
   * @returns {Promise}
   **/
  function loadProviderTypes() {
    $scope.load({
      url: '/ext/training/provider/types',
      set: 'ProviderTypes'
    });
  };

  /**
   * Initialize the form data.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadProvinces(),
      loadProviderTypes(),
      loadServiceProvider()
    ])
      .catch(angular.noop);
  }

  // $scope.saveServiceProvider = function () {
  //   return $scope.save()
  //     .then(function () {
  //       if ((!$scope.errors || angular.equals($scope.errors, {})) && (!$scope.alert || !$scope.alert.message))
  //         window.location = '/Ext/Application/Overview/View/' + $scope.model.GrantApplicationId;
  //     })
  //     .catch(angular.noop);
  // };

  $scope.isPrivateSector = function (trainingProviderTypeId) {
    if ($scope.ProviderTypes)
      for (let i = 0; i < $scope.ProviderTypes.length; i++) {
        let trainingProviderType = $scope.ProviderTypes[i];
        if (trainingProviderType.Id === trainingProviderTypeId && $scope.model.ServiceProvider) {
          $scope.model.ServiceProvider.PrivateSectorValidationType = trainingProviderType.PrivateSectorValidationType;
          return trainingProviderType.PrivateSectorValidationType;
        }
      }
    return false;
  };


  $scope.toggleSelection = function(name, _index) {
    if ($scope.model.SkillTrainingDetails[name] == null) {
      $scope.model.SkillTrainingDetails[name] = [];
    }

    var idx = $scope.model.SkillTrainingDetails[name].indexOf(_index);

    if (idx > -1) {
      $scope.model.SkillTrainingDetails[name].splice(idx, 1);
    }
    else {
      $scope.model.SkillTrainingDetails[name].push(_index);
    }
  }

  /**
   * Download the specified attachment.
   * @function downloadAttachment
   * @param {any} attachmentId - The attachment id.
   * @returns {void}
   */
  $scope.downloadAttachment = function (attachmentId) {
    window.open('/Ext/Training/Provider/' + $scope.model.ServiceProvider.Id + '/Attachment/Download/' + attachmentId);
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
        $scope.model.ServiceProvider[prop] = attachment;
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
    openAttachmentModal('Change Attachment', $scope.model.ServiceProvider[prop])
      .then(function (attachment) {
        prop = attachment;
      })
      .catch(angular.noop);
  };

  init();
});
