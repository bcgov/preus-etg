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
        if ($scope.model.CourseOutline == 1) {
          if ($scope.model.CourseOutlineDocument && $scope.model.CourseOutlineDocument.File) {
            files.push($scope.model.CourseOutlineDocument.File);
            $scope.model.CourseOutlineDocument.Index = files.length - 1;
          }
        }
        if ($scope.model.ProofOfInstructorQualifications == 1) {
          if ($scope.model.ProofOfQualificationsDocument && $scope.model.ProofOfQualificationsDocument.File) {
            files.push($scope.model.ProofOfQualificationsDocument.File);
            $scope.model.ProofOfQualificationsDocument.Index = files.length - 1;
          }
        }
        if ($scope.model.TrainingOutsideBC && $scope.model.BusinessCaseDocument && $scope.model.BusinessCaseDocument.File) {
          files.push($scope.model.BusinessCaseDocument.File);
          $scope.model.BusinessCaseDocument.Index = files.length - 1;
        }

        //Code related to Canada-Post Integration
        if (document.getElementById("AddressLine1") != null) {
          $scope.model.TrainingAddress.AddressLine1 = document.getElementById("AddressLine1").value;
          $scope.model.TrainingAddress.AddressLine2 = document.getElementById("AddressLine2").value;
          $scope.model.TrainingAddress.City = document.getElementById("City").value;
          if ($scope.model.TrainingAddress.IsCanadianAddress) {
            $scope.model.TrainingAddress.PostalCode = document.getElementById("PostalCode").value;
            $scope.model.TrainingAddress.RegionId = document.getElementById("RegionId").value.split(":").filter(r => r != "?").pop();
            $scope.model.TrainingAddress.OtherRegion = '';
            $scope.model.TrainingAddress.OtherZipCode = '';
            $scope.model.TrainingAddress.CountryId = 'CA';
          }
          else {
            $scope.model.TrainingAddress.PostalCode = '';
            $scope.model.TrainingAddress.RegionId = '';
            $scope.model.TrainingAddress.OtherRegion = document.getElementById("OtherRegion").value;
            $scope.model.TrainingAddress.OtherZipCode = document.getElementById("OtherZipCode").value;
          }
        }

        $scope.model.TrainingProviderAddress.AddressLine1 = document.getElementById("AddressLine1TrainingProvider").value;
        $scope.model.TrainingProviderAddress.AddressLine2 = document.getElementById("AddressLine2TrainingProvider").value;
        $scope.model.TrainingProviderAddress.City = document.getElementById("CityTrainingProvider").value;
        if ($scope.model.TrainingProviderAddress.IsCanadianAddress) {
          $scope.model.TrainingProviderAddress.PostalCode = document.getElementById("PostalCodeTrainingProvider").value;
          $scope.model.TrainingProviderAddress.RegionId = document.getElementById("RegionIdTrainingProvider").value.split(":").filter(r => r != "?").pop();
          $scope.model.TrainingProviderAddress.CountryId = 'CA';
        }
        else {
          $scope.model.TrainingProviderAddress.OtherRegion = document.getElementById("OtherRegionTrainingProvider").value;
          $scope.model.TrainingProviderAddress.OtherZipCode = document.getElementById("OtherZipCodeTrainingProvider").value;
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

  function fieldMappingCanadaPost() {
    var fields1 = [
      { element: "AddressLine1", field: "Line1", mode: pca.fieldMode.DEFAULT },
      { element: "AddressLine2", field: "Line2", mode: pca.fieldMode.POPULATE },
      { element: "City", field: "City", mode: pca.fieldMode.POPULATE },
      { element: "PostalCode", field: "PostalCode", mode: pca.fieldMode.POPULATE },
      { element: "Country", field: "CountryName", mode: pca.fieldMode.COUNTRY },
      { element: "OtherRegion", field: "ProvinceName", mode: pca.fieldMode.POPULATE },
      { element: "OtherZipCode", field: "PostalCode", mode: pca.fieldMode.POPULATE },
    ];

    var fields2 = [
      { element: "AddressLine1TrainingProvider", field: "Line1", mode: pca.fieldMode.DEFAULT },
      { element: "AddressLine2TrainingProvider", field: "Line2", mode: pca.fieldMode.POPULATE },
      { element: "CityTrainingProvider", field: "City", mode: pca.fieldMode.POPULATE },
      { element: "PostalCodeTrainingProvider", field: "PostalCode", mode: pca.fieldMode.POPULATE },
      { element: "CountryTrainingProvider", field: "CountryName", mode: pca.fieldMode.COUNTRY },
      { element: "OtherRegionTrainingProvider", field: "ProvinceName", mode: pca.fieldMode.POPULATE },
      { element: "OtherZipCodeTrainingProvider", field: "PostalCode", mode: pca.fieldMode.POPULATE }
    ];
    
    const options = {
      key: $scope.CanadaPostKey,
      minSearch: 3,
      culture: "en-us",
      list: {
        className: 'change-training-provider-view'
      }
    };

    // Unbind previous registered address completed - testing to resolve issues found in dev
    addressComplete.destroy();

    window.trainingLocationControl = new pca.Address(fields1, options);
    window.trainingProviderControl = new pca.Address(fields2, options);
  }

  $scope.AddressLine1Change = function () {
    //let countryPickerHeight = $(".country-picker-traininglocation").height();
    //let offsetInModal = countryPickerHeight + ($('#AddressLine1').position().top + $('#AddressLine1').offset().top) + 'px';
    //$(".change-popup-address").css('top', offsetInModal);

    if ($scope.model.TrainingAddress.IsCanadianAddress) {
      window.trainingLocationControl.listen("populate", function (address) {
        document.getElementById("RegionId").value = "string:" + address.ProvinceCode;
      });
    }
  }

  $scope.AddressLine1TrainingProviderChange = function () {
    //let countryPickerHeight = $(".country-picker-trainingprovider").height();
    //let offsetInModal = countryPickerHeight + ($('#AddressLine1TrainingProvider').position().top + $('#AddressLine1TrainingProvider').offset().top) + 'px';
    //$(".change-popup-address").css('top', offsetInModal);

    if ($scope.model.TrainingProviderAddress.IsCanadianAddress) {
      window.trainingProviderControl.listen("populate", function (address) {
        document.getElementById("RegionIdTrainingProvider").value = "string:" + address.ProvinceCode;
      });
    }
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
   * Make an AJAX request to get a key for the Canada Post API
   * @function loadCanadaPostKey
   * @returns {Promise}
   **/
  function loadCanadaPostKey() {
    return $scope.load({
      url: '/Ext/Address/CanadaPostKey',
      set: 'CanadaPostKey'
    });
  };

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
* Set the training provider type name and upload details for currently selected training provider type.
* @function setTrainingProviderTypeDetails
* @returns {void}
**/
  $scope.setTrainingProviderTypeDetails = function () {
    $scope.model.ProofOfInstructorQualifications = $scope.ProviderTypes.find(function (element) {
      return element.Id == $scope.model.TrainingProviderTypeId;
    }).ProofOfInstructorQualifications;
    $scope.model.CourseOutline = $scope.ProviderTypes.find(function (element) {
      return element.Id == $scope.model.TrainingProviderTypeId;
    }).CourseOutline;
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
  
  /**
   * Initialize the form data.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
        loadCanadaPostKey(),
        loadProviderTypes(),
        loadCountries(),
        loadProvinces(),
        loadTrainingProvider()
      ])
      .then(function () {
        fieldMappingCanadaPost();
      })
      .catch(angular.noop);
  }

  init();
});
