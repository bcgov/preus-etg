app.controller('TrainingProviderBase', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Register the shared resources.
   **/
  $scope.registerSharedResources({
    loadTrainingProviderTypes: loadTrainingProviderTypes,
    loadCountries: loadCountries,
    loadProvinces: loadProvinces,
   loadProviderTypesDetails: loadProviderTypesDetails,
    trainingProviderTypes: [],
    countries: [],
    provinces: [],
    ProviderTypesDetails:[]
  });

  /**
  * Make AJAX request to load countries
  * @function loadCountries
  * @returns {Promise}
  **/
  function loadCountries() {
    return $scope.load({
      url: '/Int/Address/Countries',
      set: 'countries',
      condition: !$scope.countries || !$scope.countries.length,
      overwrite: false,
      localCache: true
    });
  }

  /**
   * Make AJAX request to load provinces
   * @function loadProvinces
   * @returns {Promise}
   **/
  function loadProvinces() {
    return $scope.load({
      url: '/Int/Address/Provinces',
      set: 'provinces',
      condition: !$scope.provinces || !$scope.provinces.length,
      overwrite: false,
      localCache: true
    });
  }

  function loadProviderTypesDetails() {
    return $scope.load({
      url: '/Int/Application/Training/Provider/Types/Details',
      set: 'ProviderTypesDetails',
      condition: !$scope.ProviderTypesDetails || !$scope.ProviderTypesDetails.length
      //overwrite: false,
      //localCache: true
    });
  }

  /**
   * Make AJAX request to load training provider types
   * @function loadTrainingProviderTypes
   * @returns {Promise}
   **/
  function loadTrainingProviderTypes() {
    return $scope.load({
      url: '/Int/Application/Training/Provider/Types',
      set: 'trainingProviderTypes',
      condition: !$scope.trainingProviderTypes || !$scope.trainingProviderTypes.length,
      overwrite: false,
      localCache: true
    });
  }

  /**
   * Download the specified attachment.
   * @function downloadAttachment
   * @param {any} attachmentId - The attachment id.
   * @returns {void}
   */
  $scope.downloadAttachment = function (attachmentId) {
    window.open('/Int/Application/Training/Provider/' + $scope.model.Id + '/Download/Attachment/' + attachmentId);
  }

  /**
  * Get Selected Training Provider
  * @function getTrainingProviderSelected
  * @param {any} trainingProviderTypeId - The trainingProviderType id.
  * @returns {void}
  */
  $scope.getTrainingProviderSelected = function (trainingProviderTypeId) {
    return $scope.trainingProviderTypes.find(function(trainingProviderType) {
      return trainingProviderType.Key === trainingProviderTypeId
    });
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
   * Show the training provider inventory validation modal popup.
   * @function showValidateView
   * @returns {Promise}
   **/
  $scope.showValidateView = function () {
    return ngDialog.openConfirm({
      template: '/Int/Application/Training/Provider/Validate/View/' + $scope.model.Id,
      data: {
        title: 'Validate Training Provider',
        model: $scope.model
      },
      controller: 'ValidateTrainingProvider'
    }).then(function (provider) {
      return validate(provider);
    }).catch(angular.noop);
  }

  /**
   * Validate the training provider name with the inventory.
   * @function valdiate
   * @param {any} provider - The training provider from inventory.
   * @returns {Promise}
   */
  function validate(provider) {
    return $scope.ajax({
      url: '/Int/Application/Training/Provider/Validate?trainingProviderId=' + $scope.model.Id + '&trainingProviderInventoryId=' + provider.Id + '&rowVersion=' + encodeURIComponent($scope.model.RowVersion),
      method: 'PUT'
    })
      .then(function (response) {
        Utils.action($scope.section.onValidate, response);
      })
      .catch(angular.noop);
  }
});
