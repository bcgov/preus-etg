app.controller('ParticipantHistory', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ParticipantHistory'
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
 * Get the filtered training provider history.
 * @function getTrainingProviderHistory
 * @param {int} page - The page number.
 * @param {int} quantity - The number of items in a page.
 * @returns {Promise}
 **/
  $scope.getTrainingHistory = function (pageKeyword, page, quantity) {
    var useUrl = '/Int/Application/Participant/TrainingHistory/' + $attrs.participantId + '/' + page + '/' + quantity + '?sortby=' + $scope.sort.column + '&sortDesc=' + $scope.sort.descending + (pageKeyword ? '&search=' + pageKeyword : '');
    return $scope.ajax({
      url: useUrl
    })
      .then(function (response) {
        resetSortImage();
        return Promise.resolve(response.data);
      })
      .catch(angular.noop);
  };  

  const noSort = '../../../../../images/icons/icon--sort.svg';
  const sortAsc = '../../../../../images/icons/icon--sort-asc.svg';
  const sortDesc = '../../../../../images/icons/icon--sort-desc.svg';

  $scope.sort = {
    column: '',
    descending: false
  };

  $scope.changeSorting = function (column) {    
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
      newSortImage = sortDesc
    }
    if (column == 'FileNumber') { $scope.imgSrcFileNumber = newSortImage; }
    if (column == 'TrainingStartDate') { $scope.imgSrcTrainingStartDate = newSortImage; }
    if (column == 'TrainingEndDate') { $scope.imgSrcTrainingEndDate = newSortImage; }
    if (column == 'TrainingStream') { $scope.imgSrcTrainingStream = newSortImage; }
    if (column == 'ApplicationStatus') { $scope.imgSrcApplicationStatus = newSortImage; }
    if (column == 'TrainingProvider') { $scope.imgSrcTrainingProvider = newSortImage; }
    if (column == 'TrainingCourse') { $scope.imgSrcTrainingCourse = newSortImage; }
    if (column == 'ApprovedGovtContribution') { $scope.imgSrcApprovedGovtContribution = newSortImage; }
    if (column == 'AmountPaid') { $scope.imgSrcAmountPaid = newSortImage; }

    $scope.broadcast('refreshPager');
  };

  function resetSortImage() {
    $scope.imgSrcFileNumber = noSort;
    $scope.imgSrcTrainingStartDate = noSort;
    $scope.imgSrcTrainingEndDate = noSort;
    $scope.imgSrcTrainingStream = noSort;
    $scope.imgSrcApplicationStatus = noSort;
    $scope.imgSrcTrainingProvider = noSort;
    $scope.imgSrcTrainingCourse = noSort;
    $scope.imgSrcApprovedGovtContribution = noSort;
    $scope.imgSrcAmountPaid = noSort;
  };

});
