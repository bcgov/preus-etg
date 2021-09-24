app.controller('Participants', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'Participants',
    displayName: 'Participants',
    save: {
      url: '/Int/Application/Participants',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion && $scope.model.RowVersion === $scope.grantFile.RowVersion && $scope.model.ParticipantInfo;
    },
    onSave: function () {
      $scope.resyncApplicationDetails();
      $scope.emit('refresh', { force: true });
    },
    onRefresh: function () {
      return loadParticipants().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for participants data
   * @function loadParticipants
   * @returns {Promise}
   **/
  function loadParticipants() {
    return $scope.load({
      url: '/Int/Application/Participants/' + $scope.parent.grantApplicationId,
      set: 'model'
    });
  }

  /**
   * Initialize section data.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadParticipants()
    ]).catch(angular.noop);
  }

  $scope.imgSrcLastName = "../../../../images/icons/icon--sort.svg";
  $scope.imgSrcReportedOn = "../../../../images/icons/icon--sort.svg";

  $scope.sort = {
    column: '',
    descending: false
  };

  $scope.changeSorting = function (column) {
    var sort = $scope.sort;
    if (sort.column == column) {
      sort.descending = !sort.descending;
      if (column == 'LastName') {
        $scope.imgSrcReportedOn = '../../../../images/icons/icon--sort.svg';
        if (sort.descending) {
          $scope.imgSrcLastName = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcLastName = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'ReportedOn') {
        $scope.imgSrcLastName = '../../../../images/icons/icon--sort.svg';
        if (sort.descending) {
          $scope.imgSrcReportedOn = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcReportedOn = '../../../../images/icons/icon--sort-asc.svg';
        }
      }
  } else {
      sort.column = column;
      sort.descending = false;
      if (column == 'LastName') {
        $scope.imgSrcReportedOn = '../../../../images/icons/icon--sort.svg';
        if (sort.descending) {
          $scope.imgSrcLastName = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcLastName = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'ReportedOn') {
        $scope.imgSrimgSrcReportedOncLastName = '../../../../images/icons/icon--sort.svg';
        if (sort.descending) {
          $scope.imgSrcReportedOn = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcReportedOn = '../../../../images/icons/icon--sort-asc.svg';
        }
      }
    }
  };

  /**
   * Open a popup window to view the participant information.
   * @function popUpWindow
   * @param {any} participantId - The participant Id.
   * @returns {void}
   */
  $scope.popUpWindow = function (participantId) {
    window.open('/Int/Application/Participant/Info/View/' + participantId, 'popUpWindow', "height=920, width=990, toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=auto, resizable=yes, copyhistory=no");
  };

  /**
   * Enable or Disable the participant reporting for this grant file.
   * @function toggleParticpantReporting
   * @returns {void}
   **/
  $scope.toggleApplicantParticipantReporting = function () {
    if ($scope.grantFile.CanApplicantReportParticipants) {
      updateParticipantReporting();
    } else {
      confirmation();
    }
  }

  /**
   * Enable or Disable the participant reporting for this grant file.
   * @function toggleParticipantReporting
   * @returns {void}
   **/
  $scope.toggleParticipantReporting = function () {
    return $scope.ajax({
      url: '/Int/Application/Participant/Reporting/' + $scope.parent.grantApplicationId + '?rowVersion=' + encodeURIComponent($scope.grantFile.RowVersion) + '&enable=' + !$scope.grantFile.CanReportParticipants,
      method: 'PUT'
    }).then(function (response) {
      $scope.emit('update',
        {
          grantFile: {
            RowVersion: response.data.RowVersion,
            CanReportParticipants: response.data.CanReportParticipants,
            ShowApplicantReportingOfParticipantsButton: response.data.ShowApplicantReportingOfParticipantsButton
          }
        });
      return $timeout(function () {
        $scope.model.RowVersion = response.data.RowVersion;
      });
    }).catch(angular.noop);
  }

  /**
   * Make AJAX request to update the application.
   * @function updateParticipantReporting
   * @returns {Promise}
   **/
  function updateParticipantReporting() {
    return $scope.ajax({
      url: '/Int/Application/Applicant/Reporting/Participants/' + $scope.parent.grantApplicationId + '?rowVersion=' + encodeURIComponent($scope.grantFile.RowVersion) + '&enable=' + !$scope.grantFile.CanApplicantReportParticipants,
      method: 'PUT'
    })
      .then(function (response) {
        $scope.emit('update', { grantFile: { RowVersion: response.data.RowVersion, CanApplicantReportParticipants: response.data.CanApplicantReportParticipants } });
        return $timeout(function () {
          $scope.model.RowVersion = response.data.RowVersion;
        });
      })
      .catch(angular.noop);
  }

  /**
   * Show a popup confirmation dialog.
   * @function confirmation
   * @returns {void}
   **/
  function confirmation() {
    $scope.confirmDialog('Confirmation Required',
      'Are you sure you want to enable applicant reporting of participants for this file? This should only be required when an applicant has participants who are unable to report themselves.')
      .then(function () {
        updateParticipantReporting();
      });
  }
});
