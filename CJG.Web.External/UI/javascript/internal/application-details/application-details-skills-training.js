app.controller('SkillsTraining', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'SkillsTraining',
    displayName: 'Skills Training',
    save: {
      url: '/Int/Application/Skills/Training',
      method: 'POST',
      dataType: 'file',
      data: function () {
        $scope.GrantApplication = $scope.parent.grantApplicationId;
        $scope.EligibleCostId = $scope.component.Id;

        var files = [];
        //if ($scope.model.TrainingProvider.PrivateSectorValidationType) {
          if ($scope.model.TrainingProvider.CourseOutlineDocument && $scope.model.TrainingProvider.CourseOutlineDocument.File) {
            files.push($scope.model.TrainingProvider.CourseOutlineDocument.File);
            $scope.model.TrainingProvider.CourseOutlineDocument.Index = files.length - 1;
          }
          if ($scope.model.TrainingProvider.ProofOfQualificationsDocument && $scope.model.TrainingProvider.ProofOfQualificationsDocument.File) {
            files.push($scope.model.TrainingProvider.ProofOfQualificationsDocument.File);
            $scope.model.TrainingProvider.ProofOfQualificationsDocument.Index = files.length - 1;
          }
        //}
        if ($scope.model.TrainingProvider.TrainingOutsideBC
          && $scope.model.TrainingProvider.BusinessCaseDocument
          && $scope.model.TrainingProvider.BusinessCaseDocument.File) {
          files.push($scope.model.TrainingProvider.BusinessCaseDocument.File);
          $scope.model.TrainingProvider.BusinessCaseDocument.Index = files.length - 1;
        }

        return {
          files: files,
          component: angular.toJson($scope.model)
        };
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.program.RowVersion;
    },
    onSave: function () {
      $scope.resyncApplicationDetails();
      $scope.hide();
      $scope.model = {};
      $scope.emit('refresh', { target: 'ApplicationSummary,ApplicationNotes,ProgramCosts', force: true });
    },
    onCancel: function () {
      // Remove the new component.
      $scope.model = {};
      $scope.hide();
    }
  };

  angular.extend(this, $controller('TrainingProviderBase', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for delivery methods
   * @function loadDeliveryMethods
   * @returns {Promise}
   **/
  function loadDeliveryMethods() {
    return $scope.load({
      url: '/Int/Application/Training/Program/Delivery/Methods',
      set: 'deliveryMethods',
      condition: !$scope.deliveryMethods || !$scope.deliveryMethods.length,
      localCache: true
    })
      .then(function () {
        $scope.section.deliveryMethods = angular.copy($scope.deliveryMethods); // Copy to local.
      });
  }

  /**
   * Make AJAX request for skill levels
   * @function loadSkillLevels
   * @returns {Promise}
   **/
  function loadSkillLevels() {
    return $scope.load({
      url: '/Int/Application/Training/Program/Skill/Levels',
      set: 'skillLevels',
      condition: !$scope.skillLevels || !$scope.skillLevels.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request for training levels
   * @function loadTrainingLevels
   * @returns {Promise}
   * */
  function loadTrainingLevels() {
    return $scope.load({
      url: '/Int/Application/Training/Program/Training/Levels',
      set: 'trainingLevels',
      condition: !$scope.trainingLevels || !$scope.trainingLevels.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request for expected qualifications
   * @function loadExpectedQualifications
   * @returns {Promise}
   **/
  function loadExpectedQualifications() {
    return $scope.load({
      url: '/Int/Application/Training/Program/Expected/Qualifications',
      set: 'expectedQualifications',
      condition: !$scope.expectedQualifications || !$scope.expectedQualifications.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request for service lines for the specified expense type.
   * @function loadServiceLines
   * @param {int} eligibleExpenseTypeId - The eligible expense type id.
   * @returns {Promise}
   **/
  function loadServiceLines(eligibleExpenseTypeId) {
    return $scope.load({
      url: '/Int/Application/Skills/Training/Service/Lines/' + eligibleExpenseTypeId,
      set: 'section.serviceLines'
    });
  }

  /**
   * Make AJAX request for service line breakdowns for the specified service line.
   * @function loadServiceLineBreakdowns
   * @param {int} serviceLineId - The service line id.
   * @returns {Promise}
   **/
  function loadServiceLineBreakdowns(serviceLineId) {
    return $scope.load({
      url: '/Int/Application/Skills/Training/Service/Line/Breakdowns/' + serviceLineId,
      set: 'section.serviceLineBreakdowns'
    });
  }

  function loadProviderTypesDetails() {
    return $scope.load({
      url: '/Int/Application/Skills/Training/Provider/Types',
      set: 'ProviderTypesDetails'
    });
  }

  /**
   * Initialize the form data
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadDeliveryMethods(),
      loadSkillLevels(),
      loadTrainingLevels(),
      loadExpectedQualifications(),
      loadServiceLines($scope.component.EligibleExpenseTypeId),
      $scope.loadCountries(),
      $scope.loadProvinces(),
      loadProviderTypesDetails(),
      $scope.loadTrainingProviderTypes()
    ])
      .then(function () {
        setServiceLineBreakdownCaption();
        initDeliveryMethods();
      })
      .catch(angular.noop);
  }

  /**
   * Show the form to create a new skills training component.
   * @param {any} $event
   */
  $scope.addComponent = function ($event) {
    $scope.model = {
      Id: 0,
      GrantApplicationId: $scope.parent.grantApplicationId,
      EligibleCostId: $scope.component.Id,
      TrainingProvider: {
        RegionId: 'BC',
        CountryId: 'CA',
        RegionIdTrainingProvider: 'BC',
        CountryIdTrainingProvider: 'CA',
        TrainingProviderTypeId: null,
        PrivateSectorValidationType: null,
        TrainingOutsideBC: false,
        ProofOfInstructorQualifications: null,
        CourseOutline: null
      }
    }
    $scope.edit($event);
  }



  /**
   * Based on the selected service line, set the service line breakdown caption.
   * @function setServiceLineBreakdownCaption
   * @returns {void}
   **/
  function setServiceLineBreakdownCaption() {
    var serviceLine = $scope.section.serviceLines.find(function (item) { return item.Id === $scope.model.ServiceLineId; });
    if (serviceLine) $scope.model.ServiceLineBreakdownCaption = serviceLine.BreakdownCaption;
  }

  /**
   * When the service line is changed load the child service line breakdowns
   * @function loadServiceLineBreakdowns
   * @returns {Promise}
   **/
  $scope.loadServiceLineBreakdowns = function () {
    setServiceLineBreakdownCaption();
    return loadServiceLineBreakdowns($scope.model.ServiceLineId).catch(angular.noop);
  }

  /**
   * Initialize which delivery methods are selected.
   * @function initDeliveryMethods
   * @returns {void}
   **/
  function initDeliveryMethods() {
    $scope.section.deliveryMethods.forEach(function (item) {
      item.Selected = $scope.model.SelectedDeliveryMethodIds.some(function (id) { return id === item.Key });
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
        $scope.model.TrainingProvider[prop] = attachment;
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
    openAttachmentModal('Change Attachment', $scope.model.TrainingProvider[prop])
      .then(function (attachment) {
        prop = attachment;
      })
      .catch(angular.noop);
  }

  /**
  * Change training provider private sector Type
  * @function trainingProviderTypeChange
  * @param {any} trainingProviderTypeId - The trainingProviderType id.
  * @returns {void}
  */
  $scope.setTrainingProviderTypeDetails = function () {
    $scope.model.TrainingProvider.ProofOfInstructorQualifications = $scope.ProviderTypesDetails.find(function (element) {
      return element.Id == $scope.model.TrainingProvider.TrainingProviderTypeId;
    }).ProofOfInstructorQualifications;
    $scope.model.TrainingProvider.CourseOutline = $scope.ProviderTypesDetails.find(function (element) {
      return element.Id == $scope.model.TrainingProvider.TrainingProviderTypeId;
    }).CourseOutline;
    $scope.model.TrainingProvider.PrivateSectorValidationType = $scope.ProviderTypesDetails.find(function (element) {
      return element.Id == $scope.model.TrainingProvider.TrainingProviderTypeId;
    }).PrivateSectorValidationType;
  }
});
