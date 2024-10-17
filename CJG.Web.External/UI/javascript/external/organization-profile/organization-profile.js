app.filter('unsafe', function ($sce) { return $sce.trustAsHtml; });

app.controller('OrganizationProfile', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    name: 'OrganizationProfile',
    save: {
      url: '/Ext/Organization/Profile',
      method: function () {
        return $scope.model.Id ? 'PUT' : 'POST';
      },
      dataType: 'file',
      data: function () {
        var files = [];
        var attachments = $scope.section.attachments.filter(function (attachment) {
          if (typeof (attachment.File) !== 'undefined') {
            attachment.Index = files.length;
            files.push(attachment.File);
          }
          return attachment;
        });

        $scope.model.HeadOfficeAddressBlob = angular.toJson($scope.model.HeadOfficeAddress);
        $scope.model.files = files;
        $scope.model.attachments = JSON.stringify(attachments);

        return $scope.model;
      },
      backup: false
    },
    onSave: function (event, data) {
      if (data.response.data.RedirectURL)
        window.location = data.response.data.RedirectURL;
    },
    //onRefresh: function () {
    //  $scope.section.attachments = [];
    //  return loadAttachments().catch(angular.noop);
    //},
    onCancel: function () {
      $scope.section.attachments = [];
    },
    attachments: []
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
   * Make AJAX request to load organization profile data.
   * @function loadOrganizationProfile
   * @returns {Promise}
   **/
  function loadOrganizationProfile() {
    return $scope.load({
      url: '/Ext/Organization/Profile',
      set: 'model'
    });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadOrganizationTypes(),
      loadLegalStructures(),
      loadProvinces(),
      loadOrganizationProfile()
        .then(function () {
          for (let x = 1; x <= 5; x++)
            loadNAICS(x, x > 1 ? $scope.model["Naics" + (x - 1) + "Id"] : 0);
        }).catch(angular.noop)
    ])
      .catch(angular.noop);
  }

  /**
   * Make AJAX request for organization types data
   * @function loadOrganizationTypes
   * @returns {Promise}
   **/
  function loadOrganizationTypes() {
    return $scope.load({
      url: '/Ext/Organization/Types',
      set: 'organizationTypes',
      condition: !$scope.organizationTypes || !$scope.organizationTypes.length
    });
  }

  /**
   * Make AJAX request for legal structures data
   * @function loadLegalStructures
   * @returns {Promise}
   **/
  function loadLegalStructures() {
    return $scope.load({
      url: '/Ext/Organization/Legal/Structures',
      set: 'legalStructures',
      condition: !$scope.legalStructures || !$scope.legalStructures.length
    });
  }

  /**
   * Make AJAX request for provinces data
   * @function loadProvinces
   * @returns {Promise}
   **/
  function loadProvinces() {
    return $scope.load({
      url: '/Ext/Organization/Provinces',
      set: 'provinces',
      condition: !$scope.provinces || !$scope.provinces.length
    });
  }

  /**
   * Make AJAX request and load NAICS data
   * @function loadNAICS
   * @param {int} level - The NAICS level.
   * @param {int} [parentId] - The Id of the prior level.
   * @returns {Promise}
   **/
  function loadNAICS(level, parentId) {
    if (!level) level = 1;
    return $scope.load({
      url: '/Ext/Organization/NAICS/' + level + '/' + (parentId ? parentId : ''),
      set: function (response) {
        $scope['naics' + level] = response.data;
      },
      condition: level === 1 || parentId
    });
  }

  /**
 * Open modal file uploader popup and then add the new file to the model.
 * @function addAttachment
 * @returns {void}
 **/
  $scope.addAttachment = function () {
    return $scope.attachmentDialog('Add Business License Document', {
        Id: 0,
        FileName: '',
        Description: '',
        File: {},
        AttachmentType: 0
      })
      .then(function (attachment) {
        $scope.model.BusinessLicenseDocumentAttachments.push(attachment);
        $scope.section.attachments.push(attachment);
      })
      .catch(angular.noop);
  };

  $scope.removeAttachment = function (index) {
    var attachment = $scope.model.BusinessLicenseDocumentAttachments[index];
    return $scope.confirmDialog('Remove Business License Document', 'Do you want to delete this document "' + attachment.FileName + '"?')
      .then(function (response) {
        if (response === true) {
          var attachment = $scope.model.BusinessLicenseDocumentAttachments.splice(index, 1)[0];
          attachment.Delete = true;
          var i = $scope.section.attachments.indexOf(attachment);
          if (i === -1) {
            $scope.section.attachments.push(attachment);
          } else if (attachment.Id === 0) {
            $scope.section.attachments.splice(i, 1);
          }
        }
      }).catch(angular.noop);
  };

  $scope.changeAttachment = function (attachment) {
    $scope.section.attachment = attachment;
    return $scope.attachmentDialog('Update Business License Document', attachment, false)
      .then(function (attachment) {
        if ($scope.section.attachments.indexOf(attachment) === -1) {
          $scope.section.attachments.push(attachment);
        }
      })
      .catch(angular.noop);
  };

  /**
   * Make an AJAX request to fetch the next NAICS dropdown data for the selected parent NAICS.
   * @function changeNAICS
   * @param {int} level - The level to load.
   */
  $scope.changeNAICS = function (level) {
    for (let x = level; x <= 5; x++) {
      $scope.model["Naics" + x + "Id"] = null;
      $scope['naics' + x] = [];
    }
    loadNAICS(level, level > 1 ? $scope.model["Naics" + (level - 1) + "Id"] : 0);
  };
  
  /**
   * Get the name for the organization type.
   * @function getOrganizationType
   * @returns {string}
   **/
  $scope.getOrganizationType = function () {
    if ($scope.organizationTypes)
      for (let i = 0; i < $scope.organizationTypes.length; i++) {
        var type = $scope.organizationTypes[i];
        if (type.Key === $scope.model.OrganizationTypeId)
          return type.Value;
      }
    return null;
  };

  $scope.tinymceOptions = {
    plugins: 'link code autoresize preview fullscreen lists advlist anchor paste',
    toolbar: 'undo redo | bold italic | formatselect | alignleft aligncenter alignright | outdent indent | numlist bullist | anchor | preview | fullscreen | code ',
    forced_root_block: 'p',
    browser_spellcheck: true,
    contextmenu: false,
    setup: function (ed) {
      ed.on('init', function (ed) {
        $('div.tox-tinymce-aux').css('z-index', '999999');
        $('.tox.tox-tinymce').css('min-height', '250px');
      });
    }
  };

  $(document).on('focusin', function (e) {
    if ($(e.target).closest(".mce-window").length)
      e.stopImmediatePropagation();
  });

  init();
});
