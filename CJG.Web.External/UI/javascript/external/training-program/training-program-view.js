var utils = require('../../shared/utils');
app.controller('TrainingProgramView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'TrainingProgramView',
    displayName: 'Training Program',
    save: {
      url: '/Ext/Training/Program',
      method: function () {
        return $scope.model.Id > 0 ? 'PUT' : 'POST';
      },
      dataType: 'file',
      data: function () {
        var files = [];
        $scope.model.SelectedUnderRepresentedGroupIds = $scope.UnderRepresentedGroups.filter(function (item) { return item.IsSelected; }).map(function (item) { return item.Id; });
        $scope.model.SelectedDeliveryMethodIds = $scope.DeliveryMethods.filter(function (item) { return item.IsSelected; }).map(function (item) { return item.Id; });

        if ($scope.model.CourseOutlineDocument && $scope.model.CourseOutlineDocument.File) {
          files.push($scope.model.CourseOutlineDocument.File);
          $scope.model.CourseOutlineDocument.Index = files.length - 1;
        }

        return {
          files: files,
          component: angular.toJson($scope.model)
        };
      }
    },
    onSave: function () {
      window.location = '/Ext/Application/Overview/View/' + $scope.model.GrantApplicationId;
    },
    grantApplicationId: $attrs.ngGrantApplicationId,
    trainingProgramId: $attrs.ngTrainingProgramId
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.tinymceOptions = {
    plugins: 'link code autoresize preview fullscreen lists advlist anchor paste',
    toolbar: 'undo redo | bold italic | formatselect | alignleft aligncenter alignright | outdent indent | numlist bullist | anchor | preview | fullscreen | code ',
    forced_root_block: 'p',
    browser_spellcheck: true,
    contextmenu: false,
    setup: function (ed) {
      ed.on('init', function (ed) {
        $('div.tox-tinymce-aux').css('z-index', '999999');
        $('.tox.tox-tinymce').css('min-height', '300px');
      });
    }
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

  $scope.downloadAttachment = function (attachmentId) {
    window.open('/Ext/Training/Program/' + $scope.section.trainingProgramId + '/Attachment/Download/' + attachmentId);
  };

  /**
   * Initizalize the specified model property with the specified items.
   * @function initSelectedValues
   * @param {any} model - The model property to initialize.
   * @param {any} items - The items that were selected.
   * @returns {void}
   */
  function initSelectedValues(model, items) {
    if (!Array.isArray(model) || !Array.isArray(items)) return;
    items.forEach(function (item) {
      item.IsSelected = model.some(function (id) { return item.Id === id; });
    });
  }

  /**
   * Make an AJAX request to fetch the training program data.
   * @function loadTrainingProgram
   * @returns {Promise}
   **/
  function loadTrainingProgram() {
    return $scope.load({
      url: '/Ext/Training/Program/' + $scope.section.grantApplicationId + '/' + $scope.section.trainingProgramId,
      set: 'model'
    })
      .then(function () {
        $scope.model.MaxUploadSize = utils.Attachment.MaxUploadSize;

        return $timeout(function () {
          initSelectedValues($scope.model.SelectedUnderRepresentedGroupIds, $scope.UnderRepresentedGroups);
          initSelectedValues($scope.model.SelectedDeliveryMethodIds, $scope.DeliveryMethods);
        });
      }).catch(angular.noop);
  }

  /**
   * Make AJAX request to fetch dropdown list data.
   * @function loadDropdown
   * @param {string} url - The url path to the list data.
   * @param {string} target - The scope[target] property name.
   * @returns {Promise}
   */
  function loadDropdown(url, target) {
    return $scope.load({
      url: '/Ext/Training/Program' + url,
      set: target
    });
  }

  /**
   * Initialize the form data by making AJAX requests.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadDropdown('/Skill/Levels', 'SkillLevels'),
      loadDropdown('/Skills/Focuses', 'SkillFocus'),
      loadDropdown('/Expected/Qualifications', 'ExpectedQualifications'),
      loadDropdown('/Indemand/Occupations', 'InDemandOccupations'),
      loadDropdown('/Training/Levels', 'TrainingLevels'),
      loadDropdown('/Underrepresented/Groups', 'UnderRepresentedGroups'),
      loadDropdown('/Delivery/Methods', 'DeliveryMethods')
    ])
      .then(function () {
        return loadTrainingProgram();
      })
      .catch(angular.noop);
  }

  init();
});
