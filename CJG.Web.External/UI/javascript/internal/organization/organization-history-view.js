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

  const noSort = '../../../../../images/icons/icon--sort.svg';
  const sortAsc = '../../../../../images/icons/icon--sort-asc.svg';
  const sortDesc = '../../../../../images/icons/icon--sort-desc.svg';

  resetSortImage();

  $scope.sort = {
    column: '',
    descending: false
  };

  $scope.changeSorting = function (column) {
    resetSortImage();
    var sort = $scope.sort;
    var newSortImage = sortAsc;

    if (sort.column == column) {
      sort.descending = !sort.descending;
    }
    else {
      sort.column = column;
      sort.descending = false;
    }
    if (sort.descending) {
      newSortImage = sortDesc;
    }

    if (column == 'FileNumber') { $scope.imgSrcFileNumber = newSortImage; }
    if (column == 'CurrentStatus') { $scope.imgSrcCurrentStatus = newSortImage; }
    if (column == 'ApplicationStream') { $scope.imgSrcApplicationStream = newSortImage; }
    if (column == 'ApplicantName') { $scope.imgSrcApplicantName = newSortImage; }
    if (column == 'ApplicantEmail') { $scope.imgSrcApplicantEmail = newSortImage; }
    if (column == 'TrainingProgramTitle') { $scope.imgSrcTrainingProgramTitle = newSortImage; }
    if (column == 'StartDate') { $scope.imgSrcStartDate = newSortImage; }
    if (column == 'EndDate') { $scope.imgSrcEndDate = newSortImage; }
    if (column == 'NumberOfParticipants') { $scope.imgSrcNumberOfParticipants = newSortImage; }
    if (column == 'RequestedAmount') { $scope.imgSrcRequestedAmount = newSortImage; }
    if (column == 'ApprovedAmount') { $scope.imgSrcApprovedAmount = newSortImage; }
    if (column == 'AverageCostPerParticipant') { $scope.imgSrcAverageCostPerParticipant = newSortImage; }

    $scope.broadcast('refreshPager');
  };

  function resetSortImage() {
    $scope.imgSrcFileNumber = noSort;
    $scope.imgSrcCurrentStatus = noSort;
    $scope.imgSrcApplicationStream = noSort;
    $scope.imgSrcApplicantName = noSort;
    $scope.imgSrcApplicantEmail = noSort;
    $scope.imgSrcTrainingProgramTitle = noSort;
    $scope.imgSrcStartDate = noSort;
    $scope.imgSrcEndDate = noSort;
    $scope.imgSrcNumberOfParticipants = noSort;
    $scope.imgSrcRequestedAmount = noSort;
    $scope.imgSrcApprovedAmount = noSort;
    $scope.imgSrcPaidAmount = noSort;
    $scope.imgSrcAverageCostPerParticipant = noSort;
  }

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
    var useUrl = '/Int/Organization/History/Search/' + $scope.model.OrgId + '/' + page + '/' + quantity
      + '?grantProgramId=' + ($scope.model.GrantProgramId ? $scope.model.GrantProgramId : 0)
      + '&sortby=' + $scope.sort.column
      + '&sortDesc=' + $scope.sort.descending
      + '&search=' + (pageKeyword ? pageKeyword : '')

    return $scope.ajax({
      url: useUrl
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
