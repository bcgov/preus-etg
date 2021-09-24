app.controller('IntakeQueue', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'IntakeQueue',
    onRefresh: function () {
      return loadApplications().catch(angular.noop);
    }
  };

  $scope.quantities = [10, 25, 50, 100];
  $scope.assessors = [];
  $scope.fiscalYears = [];
  $scope.trainingPeriods = [];
  $scope.grantPrograms = [];
  $scope.grantStreams = [];
  $scope.filter = {
    Page: 1,
    Quantity: $scope.quantities[0],
    OrderBy: []
  };
  $scope.current = {
    FiscalYearId: 0,
    GrantProgramId: 0
  }
  $scope.cache = [];

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for assessors data
   * @function loadAssessors
   * @returns {Promise}
   **/
  function loadAssessors() {
    return $scope.load({
      url: '/Int/Intake/Queue/Assessors',
      set: 'assessors',
      condition: !$scope.assessors || !$scope.assessors.length
    });
  }

  /**
   * Make AJAX request for fiscal years data
   * @function loadFiscalYears
   * @returns {Promise}
   **/
  function loadFiscalYears() {
    return $scope.load({
      url: '/Int/Intake/Queue/Fiscal/Years',
      set: 'fiscalYears',
      condition: !$scope.fiscalYears || !$scope.fiscalYears.length
    })
      .then(function (response) {
        return loadTrainingPeriods($scope.filter.FiscalYearId, $scope.filter.GrantStreamId);
      });
  }

  /**
   * Make AJAX request for training periods data
   * @function loadTrainingPeriods
   * @param {int} fiscalYearId - The fiscal year selected.
   * @param {int} grantStreamId - The grant stream selected.
   * @returns {Promise}
   **/
  function loadTrainingPeriods(fiscalYearId, grantStreamId) {
    if (fiscalYearId) {
      $scope.trainingPeriods = [];  // ensures model is updated to dropdown
      let url = '/Int/Intake/Queue/Training/Periods/' + fiscalYearId;
      if (grantStreamId)
        url = url + '/' + grantStreamId;

      return $scope.load({
        url: url,
        set: 'trainingPeriods',
        condition: !$scope.trainingPeriods || !$scope.trainingPeriods.length || fiscalYearId !== $scope.current.FiscalYearId || grantStreamId !== $scope.current.GrantStreamId
      })
        .then(function () {
          $scope.filter.FiscalYearId = fiscalYearId;
          $scope.filter.GrantStreamId = grantStreamId;
        });
    } else {
      $scope.current.TrainingPeriodId = null;
      $scope.current.TrainingPeriodCaption = null;
      $scope.trainingPeriods = [];
      return Promise.resolve();
    }
  }

  /**
   * Make AJAX request for grant programs data
   * @function loadGrantPrograms
   * @returns {Promise}
   **/
  function loadGrantPrograms() {
    return $scope.load({
      url: '/Int/Intake/Queue/Grant/Programs',
      set: 'grantPrograms',
      condition: !$scope.grantPrograms || !$scope.grantPrograms.length
    });
  }

  /**
   * Make AJAX request for grant streams data
   * @function loadGrantStreams
   * @param {int} [grantProgramId] - The grant programId.
   * @returns {Promise}
   **/
  function loadGrantStreams(grantProgramId) {
    return $scope.load({
      url: '/Int/Intake/Queue/Grant/Streams/' + (grantProgramId || ''),
      set: 'grantStreams',
      condition: !$scope.grantStreams || !$scope.grantStreams.length || grantProgramId !== $scope.current.GrantProgramId
    })
      .then(function () {
        $scope.filter.GrantProgramId = grantProgramId;
      });
  }

  /**
   * Make AJAX request to load applications.
   * @function loadApplications
   * @returns {Promise}
   **/
  function loadApplications(page, quantity) {
    if (!page) page = 1;
    if (!quantity) quantity = $scope.quantities[0];
    return $scope.load({
      url: '/Int/Intake/Queue?page=' + page + '&quantity=' + quantity,
      method: 'POST',
      data: $scope.filter,
      set: 'model',
      condition: quantity != $scope.model.Quantity || $scope.cache.length < page || !$scope.cache[page - 1] || filterChanged() // The the quantity size changes, or the page isn't cached yet.
    })
      .then(function () {
        return $timeout(function () {
          setupPage(page, quantity);
        });
      });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadAssessors(),
      loadFiscalYears(),
      loadGrantPrograms(),
      loadGrantStreams(),
      loadApplications()
    ])
      .catch(angular.noop);
  }

  /**
   * Update the page values with page numbers.
   * Cache the page.
   * Load the page from cache if required.
   * @function setupPage
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items on the page.
   * @returns {void}
   */
  function setupPage(page, quantity) {
    // If the page isn't cached then set it up, otherwise just pull from cache.
    if ($scope.cache.length < page || !$scope.cache[page - 1] || $scope.cache[page - 1].model.Quantity !== quantity) {
      $scope.pager = Utils.setupPager(page, quantity, $scope.model.Total, $scope.model.Items);

      if ($scope.cache.length != $scope.pager.pageCount) $scope.cache = new Array($scope.pager.pageCount); // Reset cache.
      $scope.cache[page - 1] = {
        model: angular.copy($scope.model), // cache page.
        pager: angular.copy($scope.pager)
      }

      if ($scope.filter.SelectAll) $scope.toggleAll(); // Default them all selected based on the filter.
    } else {
      $scope.model = $scope.cache[page - 1].model;
      $scope.pager = $scope.cache[page - 1].pager;
    }
  }

  /**
   * Load the training periods for the filtered fiscal year.
   * @function loadTrainingPeriods
   * @returns {void}
   **/
  $scope.loadTrainingPeriods = function () {
    return loadTrainingPeriods($scope.filter.FiscalYearId, $scope.filter.GrantStreamId).catch(angular.noop);
  }

  /**
   * Load the grant streams for the filtered grant program.
   * @function loadGrantStreams
   * @returns {void}
   **/
  $scope.loadGrantStreams = function () {
    return loadGrantStreams($scope.filter.GrantProgramId).catch(angular.noop);
  }

  /**
   * Check if the filter has been changed.
   * @function filterChanged
   * @returns {void}
   **/
  function filterChanged() {
    if ($scope.filter.FiscalYearId !== $scope.current.FiscalYearId
      || $scope.filter.TrainingPeriodCaption !== $scope.current.TrainingPeriodCaption
      || $scope.filter.GrantProgramId !== $scope.current.GrantProgramId
      || $scope.filter.GrantStreamId !== $scope.current.GrantStreamId
      || $scope.filter.Quantity !== $scope.current.Quantity) return true;

    return false;
  }

  /**
   * Clear the filter.
   * @function clearFilter
   * @returns {void}
   */
  $scope.clearFilter = function() {
    if ($scope.filter == null) {
      return;
    }
    $scope.filter.FiscalYearId = null;
    $scope.filter.GrantProgramId = null;
    $scope.filter.GrantStreamId = null;
    $scope.filter.TrainingPeriodCaption = null;
  }

  /**
   * Apply the filter and load the applications.
   * @function applyFilter
   * @param {int} [page] - The page number.
   * @param {int} [quantity] - The number of items per page.
   * @param {bool} [force=false] - Whether to force a refresh.
   * @returns {Promise}
   */
  $scope.applyFilter = function (page, quantity, force) {
    if (!page) page = 1;
    if (!quantity) quantity = $scope.filter.Quantity;
    if (typeof (force) === 'undefined') force = false;

    if (force || filterChanged()) $scope.cache = [];

    return loadApplications(page, quantity)
      .then(function () {
        $scope.current = {
          FiscalYearId: $scope.filter.FiscalYearId,
          TrainingPeriodCaption: $scope.filter.TrainingPeriodCaption,
          GrantProgramId: $scope.filter.GrantProgramId,
          GrantStreamId: $scope.filter.GrantStreamId,
          Quantity: $scope.filter.Quantity
        }
      })
      .catch(angular.noop);
  }

  /**
   * Get the sorting order of the specified property.
   * @function sortDirection
   * @param {any} propertyName - The property name to order by.
   * @returns {string}
   */
  $scope.sortDirection = function (propertyName) {
    if (!isOrderedBy(propertyName)) return 'sorting';
    return isAscending(propertyName) ? 'sorting_asc' : 'sorting_desc';
  }

  /**
   * Check if the filter is currently ordered by the specified property name.
   * @function isOrderedBy
   * @param {string} propertyName - The property name to order by.
   * @returns {bool}
   */
  function isOrderedBy(propertyName) {
    return $scope.filter.OrderBy.find(function (prop) { return prop.startsWith(propertyName); }) ? true : false;
  }

  /**
   * Check if the filter is currently be ordered in ascending order by the specified property name.
   * Use this to determine if the order by is ascending or descending.
   * @function isAscending
   * @param {string} propertyName - The property name to order by.
   * @returns {bool}
   */
  function isAscending(propertyName) {
    var found = $scope.filter.OrderBy.find(function (prop) { return prop.startsWith(propertyName); });
    if (!found) return true;
    return found.endsWith('desc') ? false : true;
  }

  /**
   * Order the applications by the specified property name.
   * @function sort
   * @param {string} propertyName - The property name to order by.
   * @returns {Promise}
   */
  $scope.sort = function (propertyName) {
    $scope.filter.OrderBy = [!isOrderedBy(propertyName) || !isAscending(propertyName) ? propertyName : propertyName + ' desc'];
    $scope.cache = [];
    return $scope.applyFilter();
  }

  /**
   * Change the status of the selected grant application to pending assessment.
   * @function selectForAssessment
   * @param {object} application - The application.
   * @returns {Promise}
   */
  $scope.selectForAssessment = function (application) {
    return $scope.ajax({
      url: '/Int/Intake/Queue/Select/For/Assessment/' + application.Id + '?rowVersion=' + encodeURIComponent(application.RowVersion),
      method: 'PUT'
    })
      .then(function (response) {
        return $scope.sync(response.data, application);
      })
      .catch(angular.noop);
  }

  /**
   * Change the status of the selected grant application to begin assessment.
   * @function beginAssessment
   * @param {object} application - The application.
   * @returns {Promise}
   */
  $scope.beginAssessment = function (application) {
    return $scope.ajax({
      url: '/Int/Intake/Queue/Begin/Assessment/' + application.Id + '/' + application.AssessorId + '?rowVersion=' + encodeURIComponent(application.RowVersion),
      method: 'PUT'
    })
      .then(function () {
        return $scope.applyFilter(1, $scope.filter.Quantity, true);
      })
      .catch(angular.noop);
  }

  init();
});
