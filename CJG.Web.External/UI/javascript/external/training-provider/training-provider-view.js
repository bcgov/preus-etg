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
      data: function() {
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
        if ($scope.model.TrainingOutsideBC &&
          $scope.model.BusinessCaseDocument &&
          $scope.model.BusinessCaseDocument.File) {
          files.push($scope.model.BusinessCaseDocument.File);
          $scope.model.BusinessCaseDocument.Index = files.length - 1;
        }

        $scope.model.ContactPhone = $scope.model.ContactPhoneAreaCode +
          $scope.model.ContactPhoneExchange +
          $scope.model.ContactPhoneNumber;

        //Code related to Canada-Post Integration
        if (document.getElementById("AddressLine1") != null) {
          $scope.model.AddressLine1 = document.getElementById("AddressLine1").value;
          $scope.model.AddressLine2 = document.getElementById("AddressLine2").value;
          $scope.model.City = document.getElementById("City").value;
          if ($scope.model.IsCanadianAddress) {
            $scope.model.PostalCode = document.getElementById("PostalCode").value;
            $scope.model.RegionId = document.getElementById("RegionId").value.split(":").filter(r => r != "?").pop();
          } else {
            $scope.model.OtherRegion = document.getElementById("OtherRegion").value;
            $scope.model.OtherZipCode = document.getElementById("OtherZipCode").value;
          }
        }
        if ($scope.model.IsCanadianAddressTrainingProvider) {
          $scope.model.PostalCodeTrainingProvider = document.getElementById("PostalCodeTrainingProvider").value;
          $scope.model.RegionIdTrainingProvider = document.getElementById("RegionIdTrainingProvider").value.split(":")
            .filter(r => r != "?").pop();
        } else {
          $scope.model.OtherRegionTrainingProvider = document.getElementById("OtherRegionTrainingProvider").value;
          $scope.model.OtherZipCodeTrainingProvider = document.getElementById("OtherZipCodeTrainingProvider").value;
        }
        $scope.model.AddressLine1TrainingProvider = document.getElementById("AddressLine1TrainingProvider").value;
        $scope.model.AddressLine2TrainingProvider = document.getElementById("AddressLine2TrainingProvider").value;
        $scope.model.CityTrainingProvider = document.getElementById("CityTrainingProvider").value;
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

  function fieldMappingCanadaPost() {
    var trainingLocationAddressFields = [
      { element: "AddressLine1", field: "Line1", mode: pca.fieldMode.DEFAULT },
      { element: "AddressLine2", field: "Line2", mode: pca.fieldMode.POPULATE },
      { element: "City", field: "City", mode: pca.fieldMode.POPULATE },
      { element: "PostalCode", field: "PostalCode", mode: pca.fieldMode.POPULATE },
      { element: "Country", field: "CountryName", mode: pca.fieldMode.COUNTRY },
      { element: "OtherRegion", field: "ProvinceName", mode: pca.fieldMode.POPULATE },
      { element: "OtherZipCode", field: "PostalCode", mode: pca.fieldMode.POPULATE },
    ];

    var trainingProviderAddressFields = [
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
        className: 'training-provider-view'
      }
    };

    trainingLocationControl = new pca.Address(trainingLocationAddressFields, options);
    trainingProviderControl = new pca.Address(trainingProviderAddressFields, options);
  }

  $scope.AddressLine1Change = function () {
    if ($scope.model.IsCanadianAddress) {
      trainingLocationControl.listen("populate", function(address) {
          document.getElementById("RegionId").value = "string:" + address.ProvinceCode;
      });
    }
  }

  $scope.AddressLine1TrainingProviderChange = function () {
    if ($scope.model.IsCanadianAddressTrainingProvider) {
      trainingProviderControl.listen("populate", function (address) {
        document.getElementById("RegionIdTrainingProvider").value = "string:" + address.ProvinceCode;
      });
    }
  }
  
  $scope.filterItems = function (item) {
    return item.Key !== 'CA';
  }

  $scope.countryChange = function () {
    if ($scope.model.IsCanadianAddress) {
      $scope.model.CountryId = 'CA';
      $scope.model.Country = 'Canada';
      $scope.model.RegionId = null;
    } else {
      $scope.model.CountryId = null;
    }    
  }

  $scope.countryChangeTrainingProvider = function () {
    if ($scope.model.IsCanadianAddressTrainingProvider) {
      $scope.model.CountryIdTrainingProvider = 'CA';
      $scope.model.CountryTrainingProvider = 'Canada';
      $scope.model.RegionIdTrainingProvider = null;
    } else {
      $scope.model.CountryIdTrainingProvider = null;
    }
  }

  /**
   * Set the training provider type name and upload details for currently selected training provider type.
   * @function setTrainingProviderTypeName
   * @returns {void}
   **/
  $scope.setTrainingProviderTypeName = function () {
    $scope.model.TrainingProviderTypeName = $scope.ProviderTypes.find(function (element) {
      return element.Id == $scope.model.TrainingProviderTypeId;
    }).Caption;
    $scope.model.ProofOfInstructorQualifications = $scope.ProviderTypes.find(function (element) {
      return element.Id == $scope.model.TrainingProviderTypeId;
    }).ProofOfInstructorQualifications;
    $scope.model.CourseOutline = $scope.ProviderTypes.find(function (element) {
      return element.Id == $scope.model.TrainingProviderTypeId;
    }).CourseOutline;
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

  /**
   * Initialize the form data by making AJAX requests.
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
        // Suppress any auto-created addressComplete instances - might be a dev-only issue
        addressComplete.destroy();
        fieldMappingCanadaPost();
      })
      .catch(angular.noop);
  }

  init();
});
