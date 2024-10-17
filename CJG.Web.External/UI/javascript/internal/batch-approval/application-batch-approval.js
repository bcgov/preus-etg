app.controller('ApplicationBatchApproval', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ApplicationBatchApproval',
    fiscalYearId: $attrs.ngFiscalYearId,
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
    OrderBy: [],
    SelectAll: false
  };

  $scope.current = {
    FiscalYearId: 0,
    GrantProgramId: 0
  }

  if ($scope.section.fiscalYearId)
    $scope.filter.FiscalYearId = parseInt($scope.section.fiscalYearId);

  $scope.cache = [];

  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for assessors data
   * @function loadAssessors
   * @returns {Promise}
   **/
  function loadAssessors() {
    return $scope.load({
      url: '/Int/Application/Batch/Approval/Assessors',
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
        url: '/Int/Application/Batch/Approval/Fiscal/Years',
        set: 'fiscalYears',
        condition: !$scope.fiscalYears || !$scope.fiscalYears.length
      })
      .then(function(response) {
        if ($scope.filter.FiscalYearId == null) {
          $scope.filter.FiscalYearId = response.data[0].Key;
        }
        return loadTrainingPeriods($scope.filter.FiscalYearId, $scope.filter.GrantStreamId);
      });
  }

  /**
   * Make AJAX request for training periods data
   * @function loadTrainingPeriods
   * @param {int} fiscalYearId - The fiscal year selected.
   * @returns {Promise}
   **/
  function loadTrainingPeriods(fiscalYearId, grantStreamId) {
    if (fiscalYearId) {
      let url = '/Int/Application/Batch/Approval/Training/Periods/' + fiscalYearId;

      if (grantStreamId)
        url = url + '/' + grantStreamId;

      return $scope.load({
          url: url,
          set: function(response) {
            $scope.trainingPeriods = response.data.TrainingPeriods;

            if (response.data.CurrentPeriod == null || response.data.CurrentPeriod === '')
              $scope.filter.TrainingPeriodCaption = null;
            else
              $scope.filter.TrainingPeriodCaption = response.data.CurrentPeriod;
          },
          condition: !$scope.trainingPeriods || !$scope.trainingPeriods.length || fiscalYearId !== $scope.current.FiscalYearId || grantStreamId !== $scope.current.GrantStreamId
        })
        .then(function(response) {
          $scope.filter.FiscalYearId = fiscalYearId;
          $scope.filter.GrantStreamId = grantStreamId;
        });
    } else {
      $scope.filter.TrainingPeriodCaption = null;
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
      url: '/Int/Application/Batch/Approval/Grant/Programs',
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
      url: '/Int/Application/Batch/Approval/Grant/Streams/' + (grantProgramId || ''),
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
      url: '/Int/Application/Batch/Approval?page=' + page + '&quantity=' + quantity,
      method: 'POST',
      data: $scope.filter,
      set: 'model',
      condition: quantity != $scope.model.Quantity || $scope.cache.length < page || !$scope.cache[page - 1] // The the quantity size changes, or the page isn't cached yet.
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
    $scope.$evalAsync(function () {
      $scope.broadcast('show', { target: 'ApplicationNotes', section: { showAdd: true } });
    });

    return Promise.all([
      loadAssessors(),
      loadFiscalYears(),
      loadGrantPrograms(),
      loadGrantStreams()
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
   * Check if the filter is valid before allowing requests for applications.
   * @function validFilter
   * @returns {bool}
   **/
  $scope.validFilter = function () {
    return $scope.filter.TrainingPeriodCaption && $scope.filter.GrantProgramId && $scope.filter.GrantStreamId;
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
    $scope.filter.AssessorId = null;
    $scope.filter.FiscalYearId = 1;
    $scope.filter.GrantProgramId = null;
    $scope.filter.GrantStreamId = null; 
    $scope.filter.TrainingPeriodCaption = null;
  }

  /**
   * Apply the filter and load the applications.
   * @function applyFilter
   * @param {any} [page] - The page number.
   * @param {any} [quantity] - The number of items per page.
   * @returns {Promise}
   */
  $scope.applyFilter = function (page, quantity) {
    if ($scope.validFilter()) {
      if (!page)
        page = 1;
      if (!quantity)
        quantity = $scope.filter.Quantity;

      $scope.broadcast('update', { target: 'ApplicationNotes', model: { Notes: [] } });
      $scope.cache = [];

      return loadApplications(page, quantity)
        .then(function () {
          $scope.current = {
            AssessorId: $scope.filter.AssessorId,
            FiscalYearId: $scope.filter.FiscalYearId,
            TrainingPeriodCaption: $scope.filter.TrainingPeriodCaption,
            GrantProgramId: $scope.filter.GrantProgramId,
            GrantStreamId: $scope.filter.GrantStreamId,
            Quantity: $scope.filter.Quantity
          }
        })
        .catch(angular.noop);
    } else {
      return Promise.resolve();
    }
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
   * Clean the check boxes.
   * @function cleanCheckbox
   * @returns {void}
   */
  function cleanCheckbox(model) {
    if (model == null || model.Items == null) {
      return;
    }
    for (var i = 0; i < model.Items.length; i++) {
      model.Items[i].IsChecked = false;
    }
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
   * Show the notes for the specified grant application.
   * @function showNotes
   * @param {object} application - The application to show notes for.
   * @returns {void}
   */
  $scope.showNotes = function (application) {
    $scope.broadcast('refresh', { target: 'ApplicationNotes', section: { grantApplicationId: application.Id }, grantFile: { RowVersion: '' } });
  }

  /**
   * Map model to cache
   * @function mapModelToCache
   * @returns {void}
   */
  function mapModelToCache() {
    $scope.cache.forEach(function(cache) {
      if (cache.model.Page == $scope.model.Page) {
        cache.model = $scope.model;
      }  
    });
  }

  /**
   * Map cache to model
   * @function mapModelToCache
   * @returns {void}
   */
  function mapCacheToModel() {
    $scope.cache.forEach(function(cache) {
      if (cache.model.Page == $scope.model.Page) {
        $scope.model = cache.model;
      }  
    });
  }
  
  /**
   * Check if an offer can be issued.
   * @function canIssueOffer
   * @returns {bool}
   **/
  $scope.canIssueOffer = function () {
    if ($scope.cache.length == 0) {
      return false;
    }
    return $scope.cache.some(function (cache) {
      return cache.model.Items.some(function (application) {
        if (application.IsChecked) return true;
      });
    });
  }

  /**
   * Toggle all of the filtered applications to either selected or not selected.
   * @function toggleAll
   * @param {object} $event - The angularjs event.
   * @returns {void}
   **/
  $scope.toggleAll = function ($event) {
    $scope.cache.map(function (cache) {
      cache.model.Items.map(function (item) {
        item.IsChecked = $scope.filter.SelectAll;
      });
    });
    mapCacheToModel();
  };

  /**
   * Toggle select all checkbox.
   * @function toggleSelectAll
   * @param {object} application - Application object.
   * @returns {void}
   **/
  $scope.toggleSelectAll = function (application) {
    mapModelToCache();
    if ($scope.filter.SelectAll)
      $scope.filter.SelectAll = application.IsChecked;
  };
  
  /**
   * Show the results of the Issue Offer in a popup modal dialog.
   * @function showResults
   * @param {object} response - Http response object.
   * @returns {Promise}
   */
  function showResults(response) {
    var errors = [];
    var success = [];

    response.data.GrantApplications.map(function (application) {
      if (application.ErrorMessage) errors.push('<p>"#' + application.FileNumber + '" : ' + application.ErrorMessage + '</p>');
      else success.push(application.FileNumber);
    });

    var message = '';
    if (errors.length) message = '<em>Offers were NOT successfully issued for the following applications: </em><div>' + errors.join('') + '</div>';
    if (success.length) message = message + '<em>Offers were successfully issued for the following applications:</em><div>' + success.join(', ') + '</div>';

    return $scope.messageDialog('Batch Approval Result', message);
  }

  /**
   * Make an AJAX request to issue an offer to all selected grant applications.
   * @function issueOffers
   * @returns {Promise}
   **/
  $scope.issueOffers = function () {
    var applications = [];

    $scope.cache.map(function (page) {
      page.model.Items.map(function (application) {
        if (application.IsChecked) applications.push(application);
      });
    });

    return $scope.confirmDialog('Issue Offer', 'Do you want to issue offers to ' + ($scope.filter.SelectAll ? $scope.model.Total : applications.length) + ' applications?')
      .then(function () {
        return $scope.ajax({
          url: '/Int/Application/Batch/Approval/Issue/Offers',
          method: 'PUT',
          data: function () {

            return Object.assign({
              GrantApplications: applications,
              Total: $scope.model.Total
            }, $scope.filter);
          }
        })
          .then(function (response) {
            cleanCheckbox($scope.model);
            // Display what was done.
            showResults(response);
            $scope.cache = [];
            return $scope.applyFilter(1);
          });
      })
      .catch(angular.noop);
  }

  init();
});
