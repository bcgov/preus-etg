app.controller('PrioritizationManagementRegions', function ($scope, $attrs, $controller) {
  $scope.section = {
    name: 'PrioritizationManagementRegions'
  };

  $scope.scores = null;
  $scope.totalScores = 0;
  $scope.upload = {
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  $scope.selectRegionFile = function () {
    var $input = angular.element('#regions-import-file');
    $input.click();
  }

  $scope.updateRegions = function ($event) {
    if ($scope.upload.file) {
      return $scope.ajax({
          url: '/Int/Admin/Prioritization/Regions',
          method: 'POST',
          dataType: 'file',
          data: {
            file: $scope.upload.file,
            fileName: $scope.upload.file.name
          },
          timeout: 10 * 60 * 1000 // Ten minutes
        })
        .then(function () {
          window.location = '/Int/Admin/Prioritization/Thresholds/View';
        })
        .catch(angular.noop);
    }
    return Promise.resolve();
  }

  function loadScores() {
    return $scope.load({
      url: '/Int/Admin/Prioritization/Regions',
      set: 'scores'
    });
  }

  $scope.getScores = function (pageKeyword, page, quantity) {
    var useUrl = '/Int/Admin/Prioritization/Regions' + '?sortby=' + $scope.sort.column + '&sortDesc=' + $scope.sort.descending;
    return $scope.ajax({
        url: useUrl
      })
      .then(function (response) {
        resetSortImage();
        $scope.totalScores = response.data.RecordsTotal;
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
      newSortImage = sortDesc;
    }

    if (column == 'Name') { $scope.imgSrcName = newSortImage; }
    if (column == 'Score') { $scope.imgSrcScore = newSortImage; }
    if (column == 'IsPriority') { $scope.imgSrcPriority = newSortImage; }
  };

  function resetSortImage() {
    $scope.imgSrcName = noSort;
    $scope.imgSrcScore = noSort;
    $scope.imgSrcPriority = noSort;
  };

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
        loadScores()
      ])
      .catch(angular.noop);
  }

  init();
});
