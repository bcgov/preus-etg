app.controller('ApplicationSkillsTrainingView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'SkillsTraining',
    displayName: 'Skills Training',
    save: {
      url: '/Ext/Skills/Training',
      method: function () {
        return $scope.model.SkillTrainingDetails.Id > 0 ? "PUT" : "POST";
      },
      dataType: 'file',
      data: function () {
        var files = [];

        if ($scope.model.SkillTrainingDetails.TrainingProvider.CourseOutline == 1) {
          if ($scope.model.SkillTrainingDetails.TrainingProvider.CourseOutlineDocument
            && $scope.model.SkillTrainingDetails.TrainingProvider.CourseOutlineDocument.File) {
            files.push($scope.model.SkillTrainingDetails.TrainingProvider.CourseOutlineDocument.File);
            $scope.model.SkillTrainingDetails.TrainingProvider.CourseOutlineDocument.Index = files.length - 1;
          }
        }

        if ($scope.model.SkillTrainingDetails.TrainingProvider.ProofOfInstructorQualifications == 1) {
          if ($scope.model.SkillTrainingDetails.TrainingProvider.ProofOfQualificationsDocument &&
            $scope.model.SkillTrainingDetails.TrainingProvider.ProofOfQualificationsDocument.File) {
            files.push($scope.model.SkillTrainingDetails.TrainingProvider.ProofOfQualificationsDocument.File);
            $scope.model.SkillTrainingDetails.TrainingProvider.ProofOfQualificationsDocument.Index = files.length - 1;
          }
        }

        if ($scope.model.SkillTrainingDetails.TrainingProvider.TrainingOutsideBC &&
          $scope.model.SkillTrainingDetails.TrainingProvider.BusinessCaseDocument &&
          $scope.model.SkillTrainingDetails.TrainingProvider.BusinessCaseDocument.File) {
            files.push($scope.model.SkillTrainingDetails.TrainingProvider.BusinessCaseDocument.File);
            $scope.model.SkillTrainingDetails.TrainingProvider.BusinessCaseDocument.Index = files.length - 1;
        }

        $scope.model.SkillTrainingDetails.TrainingProvider.ContactPhone =
          $scope.model.SkillTrainingDetails.TrainingProvider.ContactPhoneAreaCode +
          $scope.model.SkillTrainingDetails.TrainingProvider.ContactPhoneExchange +
          $scope.model.SkillTrainingDetails.TrainingProvider.ContactPhoneNumber;

        if (isNaN($scope.model.SkillTrainingDetails.TotalCost))
          $scope.model.SkillTrainingDetails.TotalCost = 0;

        if (isNaN($scope.model.SkillTrainingDetails.TotalTrainingHours))
          $scope.model.SkillTrainingDetails.TotalTrainingHours = 0;

        //Code related to Canada-Post Integration
        if (document.getElementById("AddressLine1") != null) {
          $scope.model.SkillTrainingDetails.TrainingProvider.AddressLine1 = document.getElementById("AddressLine1").value;
          $scope.model.SkillTrainingDetails.TrainingProvider.AddressLine2 = document.getElementById("AddressLine2").value;
          $scope.model.SkillTrainingDetails.TrainingProvider.City = document.getElementById("City").value;

          if ($scope.model.SkillTrainingDetails.TrainingProvider.IsCanadianAddress) {
            $scope.model.SkillTrainingDetails.TrainingProvider.PostalCode = document.getElementById("PostalCode").value;
            $scope.model.SkillTrainingDetails.TrainingProvider.RegionId = document.getElementById("RegionId").value.split(":").filter(r => r != "?").pop();
          }
          else {
            $scope.model.SkillTrainingDetails.TrainingProvider.OtherRegion = document.getElementById("OtherRegion").value;
            $scope.model.SkillTrainingDetails.TrainingProvider.OtherZipCode = document.getElementById("OtherZipCode").value;
          }
        }

        $scope.model.SkillTrainingDetails.TrainingProvider.AddressLine1TrainingProvider = document.getElementById("AddressLine1TrainingProvider").value;
        $scope.model.SkillTrainingDetails.TrainingProvider.AddressLine2TrainingProvider = document.getElementById("AddressLine2TrainingProvider").value;
        $scope.model.SkillTrainingDetails.TrainingProvider.CityTrainingProvider = document.getElementById("CityTrainingProvider").value;

        if ($scope.model.SkillTrainingDetails.TrainingProvider.IsCanadianAddressTrainingProvider) {
          $scope.model.SkillTrainingDetails.TrainingProvider.PostalCodeTrainingProvider = document.getElementById("PostalCodeTrainingProvider").value;
          $scope.model.SkillTrainingDetails.TrainingProvider.RegionIdTrainingProvider = document.getElementById("RegionIdTrainingProvider").value.split(":").filter(r => r != "?").pop();
        } else {
          $scope.model.SkillTrainingDetails.TrainingProvider.OtherRegionTrainingProvider = document.getElementById("OtherRegionTrainingProvider").value;
          $scope.model.SkillTrainingDetails.TrainingProvider.OtherZipCodeTrainingProvider = document.getElementById("OtherZipCodeTrainingProvider").value;
        }

        return {
          files: files,
          component: angular.toJson($scope.model)
        };
      }
    },
    grantApplicationId: $attrs.ngGrantApplicationId,
    eligibleExpenseTypeId: $attrs.ngEligibleExpenseTypeId,
    trainingProgramId: $attrs.ngTrainingProgramId,
    isDateDisabled: $attrs.ngDateDisabled
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.LoadDropdowns = function (url, target) {
    $scope.load({
      url: '/Ext/Training/Program' + url,
      set: target
    });
  };

  $scope.LoadEligibleTypeBreakdowns = function (id) {
    $scope.load({
      url: '/Ext/Skills/Training/Service/Lines/' + id,
      set: 'EligibleExpenseBreakdowns'
    }).then(function () {
      $scope.InitEligibleExpenseBreakdownChange();
    }).catch(angular.noop);
  };

  function waitForTrainingLocation(elementId, callBack) {
    // Now that we have ng-show rather than ng-if on SkillsTrainingView, this probably isn't needed
    window.setTimeout(function () {
        var element = document.getElementById(elementId);
        if (element) {
          fieldMappingCanadaPostTrainingLocation();
        } else {
          waitForTrainingLocation(elementId, callBack);
        }
      },
      500);
  }

  function fieldMappingCanadaPostTrainingLocation() {
    var fieldsAddress = [
      { element: "AddressLine1", field: "Line1", mode: pca.fieldMode.DEFAULT },
      { element: "AddressLine2", field: "Line2", mode: pca.fieldMode.POPULATE },
      { element: "City", field: "City", mode: pca.fieldMode.POPULATE },
      { element: "PostalCode", field: "PostalCode", mode: pca.fieldMode.POPULATE },
      { element: "Country", field: "CountryName", mode: pca.fieldMode.COUNTRY },
      { element: "OtherRegion", field: "ProvinceName", mode: pca.fieldMode.POPULATE },
      { element: "OtherZipCode", field: "PostalCode", mode: pca.fieldMode.POPULATE },
    ];

    const options = {
      key: $scope.CanadaPostKey,
      minSearch: 3,
      culture: 'en-us',
      list: {
        className: 'skill-training-view-a'
      }
    };

    trainingLocationControl = new pca.Address(fieldsAddress, options);
  }

  function fieldMappingCanadaPostTrainingProvider() {
    var fieldsTrainingAddress = [
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
      culture: 'en-us',
      list: {
        className: 'skill-training-view-b'
      }
    };

    trainingProviderControl = new pca.Address(fieldsTrainingAddress, options);
  }
  
  $scope.AddressLine1Change = function () {
    if ($scope.model.SkillTrainingDetails.TrainingProvider.IsCanadianAddress) {
      trainingLocationControl.listen("populate", function (address) {
        document.getElementById("RegionId").value = "string:" + address.ProvinceCode;
      });
    }
  }

  $scope.AddressLine1TrainingProviderChange = function () {
    if ($scope.model.SkillTrainingDetails.TrainingProvider.IsCanadianAddressTrainingProvider) {
      trainingProviderControl.listen("populate", function (address) {
        document.getElementById("RegionIdTrainingProvider").value = "string:" + address.ProvinceCode;
      });
    }
  }

  $scope.InitEligibleExpenseBreakdownChange = function () {
    if ($scope.model) {
      var selectedId = $scope.model.SkillTrainingDetails.EligibleExpenseBreakdownId
      angular.forEach($scope.EligibleExpenseBreakdowns, function (value, index) {
        if (value.Id == selectedId) {
          $scope.selectedBreakdownCaption = value.BreakdownCaption ? value.BreakdownCaption : 'Essential Skills Type';
        }
      });
    }
  };

  $scope.eligibleExpenseBreakdownChange = function (selectedId) {
    angular.forEach($scope.EligibleExpenseBreakdowns, function (value, index) {
      if (value.Id == selectedId) {
        $scope.selectedBreakdownCaption = value.BreakdownCaption ? value.BreakdownCaption : 'Essential Skills Type';
      }
    });

    $scope.model.SkillTrainingDetails.ServiceLineBreakdownId = null;
    $scope.loadServiceLineBreakdowns($scope.model.SkillTrainingDetails.EligibleExpenseBreakdownId);
  };

  $scope.countryChange = function () {
    $scope.model.SkillTrainingDetails.TrainingProvider.RegionId = null;
  };
  $scope.countryChangeTrainingProvider = function () {
    $scope.model.SkillTrainingDetails.TrainingProvider.RegionIdTrainingProvider = null;
  };

  $scope.loadServiceLineBreakdowns = function (id) {
    $scope.load({
      url: '/Ext/Skills/Training/Service/Line/Breakdowns/' + id,
      set: 'ServiceLineBreakdowns'
    });
  };

  $scope.LoadProviderTypes = function () {
    $scope.load({
      url: '/Ext/Training/Provider/Types',
      set: 'ProviderTypes'
    });
  };

  $scope.loadCountries = function () {
    $scope.load({
      url: '/Ext/Address/Countries',
      set: 'Countries'
    });
  };

  $scope.loadProvinces = function () {
    $scope.load({
      url: '/Ext/Address/Provinces',
      set: 'Provinces'
    });
  };

  // No idea why this has to be the non-$scope way.
  function loadCanadaPostKey() {
    return $scope.load({
      url: '/Ext/Address/CanadaPostKey',
      set: 'CanadaPostKey'
    });
  };
  
  $scope.LoadTrainingProgram = function () {
    $scope.load({
      url: '/Ext/Application/' + $scope.section.grantApplicationId + '/Skills/Training/' + $scope.section.eligibleExpenseTypeId + '/' + $scope.section.trainingProgramId,
      set: 'model'
    }).then(function () {
      $scope.LoadEligibleTypeBreakdowns($scope.model.EligibleExpenseTypeId);
      var ebbId = $scope.model.SkillTrainingDetails.EligibleExpenseBreakdownId;
      if (ebbId) $scope.loadServiceLineBreakdowns(ebbId);
    }).catch(angular.noop);
  };

  $scope.getDate = function (year, month, day) {
    if (year * month * day === 0) return null;
    return new Date(year + "/" + month + "/" + day);
  }

  $scope.saveTrainingProgram = function () {
    return $scope.save()
      .then(function () {
        if ((!$scope.errors || angular.equals($scope.errors, {})) && (!$scope.alert || !$scope.alert.message))
          window.location = '/Ext/Application/Overview/View/' + $scope.model.GrantApplicationId;
      })
      .catch(angular.noop);
  };

  //$scope.loadCanadaPostKey();
  $scope.LoadDropdowns('/Skill/Levels', 'SkillLevels');
  $scope.LoadDropdowns('/Expected/Qualifications', 'ExpectedQualifications');
  $scope.LoadDropdowns('/Delivery/Methods', 'DeliveryMethods');
  $scope.LoadTrainingProgram();
  $scope.LoadProviderTypes();
  $scope.loadCountries();
  $scope.loadProvinces();

  $scope.setTrainingProviderTypeDetails = function () {
    $scope.model.SkillTrainingDetails.TrainingProvider.ProofOfInstructorQualifications = $scope.ProviderTypes.find(function (element) {
      return element.Id == $scope.model.SkillTrainingDetails.TrainingProvider.TrainingProviderTypeId;
    }).ProofOfInstructorQualifications;
    $scope.model.SkillTrainingDetails.TrainingProvider.CourseOutline = $scope.ProviderTypes.find(function (element) {
      return element.Id == $scope.model.SkillTrainingDetails.TrainingProvider.TrainingProviderTypeId;
    }).CourseOutline;
  }

  $scope.isPrivateSector = function (trainingProviderTypeId) {
    if ($scope.ProviderTypes)
      for (let i = 0; i < $scope.ProviderTypes.length; i++) {
        let trainingProviderType = $scope.ProviderTypes[i];
        if (trainingProviderType.Id === trainingProviderTypeId && $scope.model.SkillTrainingDetails.TrainingProvider) {
          $scope.model.SkillTrainingDetails.TrainingProvider.PrivateSectorValidationType = trainingProviderType.PrivateSectorValidationType;
          return trainingProviderType.PrivateSectorValidationType;
        }
      }
    return false;
  };

  $scope.toggleSelection = function toggleSelection(name, _index) {
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
    window.open('/Ext/Training/Provider/' + $scope.model.SkillTrainingDetails.TrainingProvider.Id + '/Attachment/Download/' + attachmentId);
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
        $scope.model.SkillTrainingDetails.TrainingProvider[prop] = attachment;
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
    openAttachmentModal('Change Attachment', $scope.model.SkillTrainingDetails.TrainingProvider[prop])
      .then(function (attachment) {
        prop = attachment;
      })
      .catch(angular.noop);
  };

  function init() {
    return Promise.all([
        loadCanadaPostKey()
      ])
      .then(function () {
        // Suppress any auto-created addressComplete instances - might be a dev-only issue
        addressComplete.destroy();
        waitForTrainingLocation("AddressLine1",
            function() {
            }),
        fieldMappingCanadaPostTrainingProvider();
      })
      .catch(angular.noop);
  }

  init();
});
