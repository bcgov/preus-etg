app.controller('ApplicantContact', function ($scope, $attrs, $controller, $element, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ApplicantContact',
    displayName: 'Applicant Contact',
    save: {
      url: '/Int/Application/ApplicantContact',
      method: 'PUT',
      data: function () {
        if ($scope.model.MailingAddressSameAsPhysical) {
          delete $scope.model.MailingAddress;
        }
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.grantFile.RowVersion;
    },
    onSave: function () {
      $scope.emit('update', { grantFile: { RowVersion: $scope.model.RowVersion } });
      $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
    },
    onShow: function (event, data) {
    },
    onRefresh: function () {
      return loadApplicantContact().catch(angular.noop);
    }
  };
  if (typeof ($scope.provinces) === 'undefined') $scope.provinces = [];
  if (typeof ($scope.countries) === 'undefined') $scope.countries = [];
  if (typeof ($scope.applicantContacts) === 'undefined') $scope.applicantContacts = [];

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to load the provinces
   * @function loadProvinces
   * @returns {Promise}
   **/
  function loadProvinces() {
    return $scope.load({
      url: '/Int/Address/Provinces',
      set: 'provinces',
      condition: !$scope.provinces || !$scope.provinces.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request to load the countries
   * @function loadCountries
   * @returns {Promise}
   **/
  function loadCountries() {
    return $scope.load({
      url: '/Int/Address/Countries',
      set: 'countries',
      condition: !$scope.countries || !$scope.countries.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request to load the applicant contacts.
   * @function loadApplicantContacts
   * @returns {Promise}
   **/
  function loadApplicantContacts() {
    return $scope.load({
      url: '/Int/Application/Applicant/Contacts/' + $scope.parent.grantApplicationId,
      set: 'applicantContacts',
      condition: !$scope.applicantContacts || !$scope.applicantContacts.length || !$scope.useCatch
    }).then(function() {
      $scope.useCatch = true;
    });
  }

  /**
   * Make AJAX request to load the alternate contacts.
   * @function loadAlternateContact
   * @returns {Promise}
   **/
  function loadAlternateContact() {
    return $scope.load({
      url: '/Int/Application/ApplicantContact/' + $scope.parent.grantApplicationId,
      set: 'alternateContactModel',
      condition: !$scope.alternateContactModel
    }).then(function() {
      $scope.useCatch = true;
    });
  }

  /**
   * Make AJAX request to load the applicant contact data.
   * @function loadApplicantContact
   * @returns {Promise}
   **/
  function loadApplicantContact() {
    return $scope.load({
      url: '/Int/Application/ApplicantContact/' + $scope.parent.grantApplicationId,
      set: 'model'
    });
  }

  /**
   * Initialize the section and load the data.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadProvinces(),
      loadCountries(),
      loadApplicantContact()
    ]).catch(angular.noop);
  }

  /**
   * Make AJAX request and update the applicant contact for the application.
   * @function changeApplicantContact
   * @param {int} applicantId - The applicant contact id selected.
   * @returns {Promise}
   **/
  function changeApplicantContact(applicantId) {
    return $scope.load({
      url: '/Int/Application/Applicant/Contact/Change',
      method: 'PUT',
      data: {
        Id: $scope.model.Id,
        RowVersion: $scope.model.RowVersion,
        ApplicantContactId: applicantId
      },
      set: 'model'
    }).then(function(response) {
      $scope.useCatch = false;
      $scope.resyncApplicationDetails(); 
      $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
    }).catch(angular.noop);
  }

  /**
   * Make AJAX request and update the applicant contact for the application.
   * @function changeApplicantContact
   * @param {ApplicantContactViewModel} model - The alternate contact info entered.
   * @returns {Promise}
   **/
  function changeAlternateContact(model) {
    return $scope.load({
      url: '/Int/Application/Alternate/Contact/Change',
      method: 'PUT',
      data: {
        Id: $scope.alternateContactModel.Id,
        RowVersion: $scope.alternateContactModel.RowVersion,
        Model: $scope.alternateContactModel
      },
      set: 'model'
    }).then(function(response) {
      $scope.useCatch = false;
      $scope.resyncApplicationDetails();
      $scope.alternateContactModel = null;
      $scope.emit('refresh', { target: 'ApplicationNotes', force: true });
    }).catch(angular.noop);
  }

  /**
   * Show the dialog to change the applicant contact.
   * @function showDialog
   * @returns {Promise}
   **/
  function showDialog() {
    return ngDialog.openConfirm({
      template: '/content/dialogs/_ChangeApplicant.html',
      data: {
        title: 'Change Applicant Contact',
        applicantContacts: $scope.applicantContacts
      }
    }).then(function (applicantId) {
      return changeApplicantContact(applicantId);
    });
  }

  /**
   * Show the dialog to change the alternate contact.
   * @function showDialog
   * @returns {Promise}
   **/
  function showAlternateContactDialog() {
    return ngDialog.openConfirm({
      template: '/content/dialogs/_ChangeAlternateContact.html',
      data: {
        title: 'Change Alternate Contact',
        model: $scope.alternateContactModel
      }
    }).then(function (model) {
      return changeAlternateContact(model);
    });
  }

  /**
   * Show the change applicant contact popup form.
   * @function showApplicantContacts
   * @returns {Promise}
   **/
  $scope.showApplicantContacts = function () {
    return loadApplicantContacts()
      .then(function () {
        return showDialog();
      })
      .catch(angular.noop);
  };

  /**
   * Show the change alternate contact popup form.
   * @function showApplicantContacts
   * @returns {Promise}
   **/
  $scope.showAlternateContact = function () {
    return loadAlternateContact()
      .then(function () {
        return showAlternateContactDialog();
      })
      .catch(angular.noop);
  };
});
