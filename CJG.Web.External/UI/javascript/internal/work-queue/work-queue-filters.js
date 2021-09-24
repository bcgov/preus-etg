require('./work-queue-filter');

app.controller('WorkQueueFilters', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'WorkQueueFilters',
    onRefresh: function () {
      return loadFilters().catch(angular.noop);
    }
  };

  $scope.filters = [];

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for work queue filters.
   * @function loadFilters
   * @returns {Promise}
   **/
  function loadFilters() {
    return $scope.load({
      url: '/Int/Work/Queue/Filters',
      set: 'model'
    })
      .then(function (response) {
        return $timeout(function () {
          $scope.filters = response.data.Filters;
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
      loadFilters()
    ])
      .catch(angular.noop);
  }

  /**
   * Find the attribute with the specified key name in the filter
   * @function findAttribute
   * @param {object} filter - The filter.
   * @param {string} key - The attribute name you are looking for.
   * @returns {object} If the attribute was found it will be returned, otherwise returns a new attribute {}.
   */
  function findAttribute(filter, key) {
    if (!filter || !Array.isArray(filter.Attributes)) return;
    return filter.Attributes.find(function (item) {
      return item.Key === key;
    }) || { Id: 0, Key: key, Value: null, Operator: 0 };
  }

  /**
   * Open a modal dialog to add/edit/delete the specified filter.
   * @function openFilterDialog
   * @param {object} filter - The selected filter.
   * @returns {Promise}
   */
  function openFilterDialog(filter) {
    var _filter = filter;
    return ngDialog.openConfirm({
      template: '/Int/Work/Queue/Filter/' + filter.Id,
      data: {
        title: 'Work Queue Filter',
        filter: filter,
        quantities: $scope.quantities,
        assessors: $scope.assessors,
        fiscalYears: $scope.fiscalYears,
        trainingPeriods: $scope.trainingPeriods,
        grantPrograms: $scope.grantPrograms,
        grantStreams: $scope.grantStreams
      }
    })
      .then(function (filter) {
        return $timeout(function () {
          if (filter) {
            if (_filter.Id === 0) {
              $scope.filters.push(filter);
            } else {
              $scope.sync(filter, _filter);
              if ($scope.filters.indexOf(filter) === -1) {
                $scope.filters.push(filter);
              }
            }
            $scope.applyFilter(filter);
          } else {
            var index = $scope.filters.indexOf(_filter);
            $scope.filters.splice(index, 1);
            if ($scope.filters.length)
              $scope.applyFilter($scope.filters[0]);
            else
              $scope.applyFilter({ Attributes: [] });
          }
        });
      })
      .catch(angular.noop);
  }

  /**
   * Open the filter modal dialog to add a new filter.
   * Prepopulate the filter with the currently selected filter on the page.
   * @function addFilter
   * @returns {Promise}
   **/
  $scope.addFilter = function () {
    var filter = Object.assign({ Attributes: [{ Key: 'States', Value: '3,4,5,6', Operator: 6 }] }, $scope.filter, { Id: 0 });
    return openFilterDialog(filter);
  }

  /**
   * Open the filter modal dialog to edit the selected filter.
   * @function editFilter
   * @param {object} filter - The filter to edit.
   * @returns {Promise}
   */
  $scope.editFilter = function (filter) {
    return openFilterDialog(filter);
  }

  /**
   * Apply the selected filter and fetch the applications.
   * @function applyFilter
   * @param {any} filter - The filter.
   * @returns {Promise}
   */
  $scope.applyFilter = function (filter) {
    var element = angular.element('#filter_' + filter.Id);
    if (!element.hasClass('selected')) {
      deselectFilters(filter);
      element.addClass('selected');
      $scope.filter.Id = filter.Id;
      $scope.filter.Applicant = findAttribute(filter, 'Applicant').Value;
      $scope.filter.AssessorId = parseInt(findAttribute(filter, 'AssessorId').Value) || null;
      $scope.filter.FiscalYearId = parseInt(findAttribute(filter, 'FiscalYearId').Value) || null;
      $scope.filter.TrainingPeriodCaption = findAttribute(filter, 'TrainingPeriodCaption').Value || null;
      $scope.filter.GrantProgramId = parseInt(findAttribute(filter, 'GrantProgramId').Value) || null;
      $scope.filter.GrantStreamId = parseInt(findAttribute(filter, 'GrantStreamId').Value) || null;
      var states = findAttribute(filter, 'States').Value;
      $scope.filter.States = states ? states.split(',').map(function (id) { return parseInt(id); }) : states;
    } else {
      element.removeClass('selected');
      $scope.clearFilter();
    }
    return $scope.$parent.applyFilter(1);
  }

  /**
   * Remove the 'selected' class from the filters
   * @function deselectFilters
   * @returns {object} The DOM object array containing filters.
   **/
  function deselectFilters () {
    var filterElements = angular.element('.filter-btn-group');
    angular.forEach(filterElements, function (value, key) {
      var element = angular.element(value);
      element.removeClass('selected');
    });
    return filterElements;
  }

  init();
});
