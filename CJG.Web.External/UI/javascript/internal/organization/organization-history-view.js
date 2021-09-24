app.controller('OrganizationHistory', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'OrganizationHistory',
    onRefresh: function () {
      return loadOrganizationHistory().catch(angular.noop);
    }
  };
  $scope.grantPrograms = [];
  $scope.AllGrantProgramDescription = "All Grant Programs";
  $scope.GrantProgramDescription = $scope.AllGrantProgramDescription;
  $scope.filter = {
    grantProgramId: null,
    page: 1,
    quantity: 10
  };
  $scope.model = {};
  $scope.current = Object.assign({}, $scope.filter);
  $scope.cache = [];
  $scope.quantities = [10, 25, 50, 100];

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
   * Make AJAX request to load organization history data.
   * @function loadOrganizationHistory
   * @returns {Promise}
   **/
  function loadOrganizationHistory() {
    return $scope.load({
      url: '/Int/Organization/History/' + $attrs.orgId,
      set: 'model'
    })
      .then(function () {
        return $timeout(function () {
          $scope.originalNotes = $scope.model.Notes;
          $scope.originalRiskFlag = $scope.model.RiskFlag;
        });
      })
      .catch(angular.noop);
  }
  /**
 * Load organization YTD history for one or all program types.
 * This is called when the Select changes.
 * @function loadOrganizationHistoryYTD
 * @returns {Promise}
 **/
  function loadOrganizationHistoryYTD(grantProgramId) {
    return $scope.load({
      url: '/Int/Organization/HistoryYTD/' + $attrs.orgId + '/' + grantProgramId,
      set: 'HistoryYTD'
    })
      .then(function () {
        return $timeout(function () {
          var e = document.getElementById("grantProgramId");
          if (e != null) {
            if (e.selectedIndex === 0)
              $scope.GrantProgramDescription = $scope.AllGrantProgramDescription;
            else
              $scope.GrantProgramDescription = e.options[e.selectedIndex].text;
          }
          else {
            $scope.GrantProgramDescription = "Error displaying Grant Program";
          }
          $scope.model.YTDRequested = $scope.HistoryYTD.TotalRequested;
          $scope.model.YTDApproved = $scope.HistoryYTD.TotalApproved;
          $scope.model.YTDPaid = $scope.HistoryYTD.TotalPaid;
        });
      })
      .catch(angular.noop);
  }


  function loadProgramTypes() {
    return $scope.load({
      url: '/Int/Organization/History/Program/Types',
      set: 'grantPrograms',
      condition: !$scope.grantPrograms || !$scope.grantPrograms.length
    })
      .then(function () {
        // Remove element with Key == 1, the "Canada-BC Job Grant" for the dropdown.
        $scope.grantPrograms = $scope.grantPrograms.filter(e => e.Key !== 1);
      })
      .catch(angular.noop);
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadOrganizationHistory(),
      loadProgramTypes()
    ]);
  }

  /**
   * Make AJAX request to update the note.
   * @function updateNote
   * @returns {Promise} promise
   **/
  $scope.updateOrg = function () {
    return $scope.ajax({
      url: '/Int/Organization/History/Change/' + $scope.model.OrgId,
      method: 'PUT',
      data: {
        organizationId: $scope.model.OrgId,
        notesText: $scope.model.Notes,
        riskFlag: $scope.model.RiskFlag,
        rowVersion: $scope.model.RowVersion
      }
    })
      .then(function (response) {
        return $timeout(function () {
          $scope.setAlert({ response: { status: 200 }, message: 'Changes have been saved successfully' });
          $scope.model.RowVersion = response.data.RowVersion;
          $scope.originalNotes = $scope.model.Notes;
          $scope.originalRiskFlag = $scope.model.RiskFlag;
        });
      })
      .catch(angular.noop);
  };

  /**
 * Check if notes are different
 * @function checkNotesDiff
 * @returns {boolean}
 **/
  $scope.checkNotesDiff = function () {
    return $scope.originalNotes === $scope.model.Notes;
  };


  $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcPaidAmount= '../../../../images/icons/icon--sort.svg';
  $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

  $scope.sort = {
    column: '',
    descending: false
  };


  $scope.changeSorting = function (column) {
    var sort = $scope.sort;
    if (sort.column == column) {
      sort.descending = !sort.descending;
      if (column == 'FileNumber') {
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'CurrentStatus') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'ApplicationStream') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'ApplicantName') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'ApplicantEmail') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'TrainingProgramTitle') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'StartDate') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcStartDate = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcStartDate = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'NumberOfParticipants') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'EndDate') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcEndDate = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcEndDate = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'RequestedAmount') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'ApprovedAmount') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'PaidAmount') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcPaiddAmount = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'AverageCostPerParticipant') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort-asc.svg';
        }
      }
    } else {
      sort.column = column;
      sort.descending = false;

      if (column == 'FileNumber') {
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'CurrentStatus') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'ApplicationStream') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'ApplicantName') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'ApplicantEmail') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'TrainingProgramTitle') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'StartDate') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcStartDate = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcStartDate = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'EndDate') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcEndDate = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcEndDate = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'NumberOfParticipants') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'RequestedAmount') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'ApprovedAmount') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort-asc.svg';
        }
      }

      if (column == 'AverageCostPerParticipant') {
        $scope.imgSrcFileNumber = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicationStream = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcCurrentStatus = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantEmail = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApplicantName = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcStartDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcEndDate = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcTrainingProgramTitle = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcRequestedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcApprovedAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcPaidAmount = '../../../../images/icons/icon--sort.svg';
        $scope.imgSrcNumberOfParticipants = '../../../../images/icons/icon--sort.svg';

        if (sort.descending) {
          $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort-desc.svg';
        } else {
          $scope.imgSrcAverageCostPerParticipant = '../../../../images/icons/icon--sort-asc.svg';
        }
      }
    }
  };

  /**
   * Reset the notes
   * @function resetNote
   * @returns {void}
   **/
  $scope.resetNote = function () {
    return $scope.model.Notes = $scope.originalNotes;
  };

  /**
   * Get the filtered organization history.
   * @function getOrganizationHistory
   * @param {string} pageKeyword - The search filter keyword.
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items in a page.
   * @returns {Promise}
   **/
  $scope.getOrganizationHistory = function (pageKeyword, page, quantity) {
    return $scope.ajax({
      url: '/Int/Organization/History/Search/' + $scope.model.OrgId + '/' + page + '/' + quantity + '?grantProgramId='
        + ($scope.model.GrantProgramId ? $scope.model.GrantProgramId : 0) + '&search=' + (pageKeyword ? pageKeyword : '')
    })
      .then(function (response) {
        return Promise.resolve(response.data);
      })
      .catch(angular.noop);
  };

  /**
   * Called from grant type dropdown
   * @function getOrganizationHistoryGrant
   **/
  $scope.getOrganizationHistoryGrant = function () {
    loadOrganizationHistoryYTD($scope.model.GrantProgramId ? $scope.model.GrantProgramId : 0);
    $scope.broadcast('refreshPager');
  };

  init();
});
