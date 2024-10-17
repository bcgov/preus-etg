app.controller('ParticipantsHistory', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ParticipantsHistory',
    onRefresh: function () {
      return loadParticipantHistory().catch(angular.noop);
    }
  };
  $scope.filter = {
    page: 1,
    quantity: 10
  };
  $scope.model = {};
  $scope.current = Object.assign({}, $scope.filter);
  $scope.cache = [];
  $scope.quantities = [100, 50, 25];

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  function init() {
    return Promise.all([
      loadParticipantHistory()
    ]);
  }

  function loadParticipantHistory() {
    return $scope.load({
      url: '/Int/Participants/Participant/Data/' + $attrs.participantFormId,
      set: 'model'
    })
    .catch(angular.noop);
  }

  $scope.getParticipantHistory = function (pageKeyword, page, quantity) {
    var useUrl = '/Int/Participants/Participant/Search/' + $attrs.participantFormId + '/' + page + '/' + quantity
      + '?sortby=' + $scope.sort.column
      + '&sortDesc=' + $scope.sort.descending
      + '&search=' + (pageKeyword ? pageKeyword : '');

    return $scope.ajax({
        url: useUrl
      })
      .then(function (response) {
        return Promise.resolve(response.data);
      })
      .catch(angular.noop);
  };
  
  const noSort = '../../../../../images/icons/icon--sort.svg';
  const sortAsc = '../../../../../images/icons/icon--sort-asc.svg';
  const sortDesc = '../../../../../images/icons/icon--sort-desc.svg';

  resetSortImage();
  
  $scope.sort = {
    column: 'FileNumber',
    descending: true
  };

  $scope.imgSrcFileNumber = noSort;
  $scope.imgSrcCurrentStatus = noSort;
  $scope.imgSrcApplicant = noSort;
  $scope.imgSrcTrainingProgramTitle = noSort;
  $scope.imgSrcStartDate = noSort;
  $scope.imgSrcNumberOfParticipants = noSort;
  $scope.imgSrcTotalGovContribution = noSort;
  $scope.imgSrcAvgCostperPart = noSort;

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
    if (column == 'ApplicantName') { $scope.imgSrcApplicant = newSortImage; }
    if (column == 'TrainingProgramTitle') { $scope.imgSrcTrainingProgramTitle = newSortImage; }
    if (column == 'StartDate') { $scope.imgSrcStartDate = newSortImage; }
    if (column == 'NumberOfParticipants') { $scope.imgSrcNumberOfParticipants = newSortImage; }
    if (column == 'TotalGovernmentContribution') { $scope.imgSrcTotalGovContribution = newSortImage; }
    if (column == 'AverageCostPerParticipant') { $scope.imgSrcAvgCostperPart = newSortImage; }

    $scope.broadcast('refreshPager');
  };

  function resetSortImage() {
    $scope.imgSrcFileNumber = noSort;
    $scope.imgSrcCurrentStatus = noSort;
    $scope.imgSrcApplicant = noSort;
    $scope.imgSrcTrainingProgramTitle = noSort;
    $scope.imgSrcStartDate = noSort;
    $scope.imgSrcNumberOfParticipants = noSort;
    $scope.imgSrcTotalGovContribution = noSort;
    $scope.imgSrcAvgCostperPart = noSort;
  }

  init();
});
