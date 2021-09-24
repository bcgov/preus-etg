app.controller('WorkQueueFilter', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'WorkQueueFilter',
    save: {
      url: '/Int/Work/Queue/Filter',
      method: function () { return $scope.model.Id === 0 ? 'POST' : 'PUT' },
      data: function () {
        initAttributes($scope.model);
        return $scope.model;
      }
    },
    onSave: function () {
      return $scope.confirm($scope.model);
    },
    onCancel: function () {
      $scope.closeThisDialog();
    }
  };

  $scope.stateGroups = [
    {
      Caption: 'Intake',
      States: [
        { Id: 1, Caption: 'New' },
        { Id: 2, Caption: 'Pending Assessment' },
        { Id: 13, Caption: 'Application Withdrawn' }
      ]
    },
    {
      Caption: 'Application Assessment',
      States: [
        { Id: 3, Caption: 'Under Assessment' },
        { Id: 4, Caption: 'Returned to Assessment' },
        { Id: 5, Caption: 'Recommended for Approval' },
        { Id: 6, Caption: 'Recommended for Denial' },
        { Id: 11, Caption: 'Application Denied' },
        { Id: 7, Caption: 'Offer Issued' },
        { Id: 9, Caption: 'Agreement Accepted' }
      ]
    },
    {
      Caption: 'Change Requests',
      States: [
        { Id: 16, Caption: 'Change Request' },
        { Id: 19, Caption: 'Change Returned' },
        { Id: 17, Caption: 'Change for Approval' },
        { Id: 18, Caption: 'Change for Denial' },
        { Id: 20, Caption: 'Change Request Denied' },
        { Id: 9, Caption: 'Change Request Approved' }
      ]
    },
    {
      Caption: 'Claim Assessment',
      States: [
        { Id: 21, Caption: 'New Claim' },
        { Id: 22, Caption: 'Claim Assess Eligibility' },
        { Id: 31, Caption: 'Claim Assess Reimbursement' },
        { Id: 23, Caption: 'Claim Returned to Applicant' },
        { Id: 24, Caption: 'Claim Denied' },
        { Id: 25, Caption: 'Claim Approved' }
      ]
    },
    {
      Caption: 'Inactive',
      States: [
        { Id: 8, Caption: 'Offer Withdrawn' },
        { Id: 12, Caption: 'Agreement Rejected' },
        { Id: 14, Caption: 'Cancelled by Ministry' },
        { Id: 15, Caption: 'Cancelled by Agreement Holder' },
        { Id: 32, Caption: 'Completion Reporting' },
        { Id: 30, Caption: 'Closed' },
        { Id: 10, Caption: 'Unfunded' },
        { Id: 28, Caption: 'Returned to Applicant Unassessed' }
      ]
    }
  ];

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for training periods data
   * @function loadTrainingPeriods
   * @param {int} fiscalYearId - The fiscal year selected.
   * @returns {Promise}
   **/
  function loadTrainingPeriods(fiscalYearId, grantStreamId) {

    if (!fiscalYearId) {
      $scope.ngDialogData.trainingPeriods = [];
      return Promise.resolve(false);
    }

    let url = '/Int/Work/Queue/Training/Periods/' + fiscalYearId;
    if (grantStreamId)
      url = url + '/' + grantStreamId;

    return $scope.load({
      url: url,
      set: 'ngDialogData.trainingPeriods',
      overwrite: true
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
      url: '/Int/Work/Queue/Grant/Streams/' + (grantProgramId || ''),
      set: 'ngDialogData.grantStreams',
      overwrite: true
    });
  }

  /**
   * Initialize the form data
   * @function init
   * @returns {void}
   **/
  function init() {
    var filter = $scope.ngDialogData.filter;
    var states = findAttribute(filter, 'States');
    if (states && states.Value) selectStates(states.Value.split(',').map(function (id) { return parseInt(id); }));

    initFilter(filter, 'AssessorId', function (attr) { return attr ? parseInt(attr.Value) || null : filter['AssessorId']; });
    initFilter(filter, 'FiscalYearId', function (attr) { return attr ? parseInt(attr.Value) || null : filter['FiscalYearId']; });
    initFilter(filter, 'TrainingPeriodCaption', function (attr) { return attr ? attr.Value || null : filter['TrainingPeriodCaption']; });
    initFilter(filter, 'GrantProgramId', function (attr) { return attr ? parseInt(attr.Value) || null : filter['GrantProgramId']; });
    initFilter(filter, 'GrantStreamId', function (attr) { return attr ? parseInt(attr.Value) || null : filter['GrantStreamId']; });
    initFilter(filter, 'Applicant');

    $scope.model = filter; // Do this so that the default section edit/cancel/save works.
    $scope.backup();
  }

  /**
   * Initialize the form filter with values from the filter attributes.
   * @function initFilter
   * @param {object} filter - The filter.
   * @param {string} key - The attribute key name.
   * @param {function} [convert] - A function to convert the attribute value.
   * @returns {object} The attribute if it was found.
   */
  function initFilter(filter, key, convert) {
    if (!convert) convert = function (attr) { return attr ? attr.Value : filter[key]; }
    var attribute = findAttribute(filter, key);
    filter[key] = convert(attribute);
    return attribute;
  }

  /**
   * Copy the form filter values into the filter attributes.
   * @function initAttributes
   * @param {any} filter - The filter.
   * @returns {void}
   */
  function initAttributes(filter) {
    var attribute = findAttribute(filter, 'States');
    if (attribute) attribute.Value = getSelectedStates().join(',');

    findAttribute(filter, 'AssessorId', true).Value = filter.AssessorId;
    findAttribute(filter, 'FiscalYearId', true).Value = filter.FiscalYearId;
    findAttribute(filter, 'TrainingPeriodCaption', true).Value = filter.TrainingPeriodCaption;
    findAttribute(filter, 'GrantProgramId', true).Value = filter.GrantProgramId;
    findAttribute(filter, 'GrantStreamId', true).Value = filter.GrantStreamId;
    var applicant = findAttribute(filter, 'Applicant', true);
    applicant.Value = filter.Applicant;
    applicant.Operator = 10;
  }

  /**
   * Find the attribute with the specified key name in the filter
   * @function findAttribute
   * @param {object} filter - The filter.
   * @param {string} key - The attribute name you are looking for.
   * @param {bool} [create=false] - Whether to create an attribute if it doesn't exist.
   * @returns {object} If the attribute was found it will be returned, otherwise returns undefined (or a new attribute if 'create=true').
   */
  function findAttribute(filter, key, create) {
    if (!filter || !Array.isArray(filter.Attributes))
      return;

    var attribute = filter.Attributes.find(function (item) {
      return item.Key === key;
    });

    if (attribute || !create)
      return attribute;

    attribute = { Id: 0, Key: key, Value: null, Operator: 0 };
    filter.Attributes.push(attribute);

    return attribute;
  }

  /**
   * Select the specified states in the filter on the page.
   * @function selectStates
   * @param {Array} states - An array of states.
   * @returns {void}
   */
  function selectStates(states) {
    $scope.stateGroups.map(function (group) {
      group.States.map(function (state) {
        state.IsChecked = states && states.some(function (item) { return item === state.Id; });
      });
    });
  }

  /**
   * Scan the selected states and return an array of their ids
   * @function getSelectedStates
   * @returns {Array}
   **/
  function getSelectedStates() {
    var states = [];
    $scope.stateGroups.map(function (group) {
      states = states.concat(group.States.filter(function (state) {
        return state.IsChecked;
      }).map(function (state) { return state.Id; }));
    });
    return states;
  }

  /**
   * Make AJAX request to delete the specified filter from the datasource.
   * @function deleteFilter
   * @param {any} filter - The filter
   * @returns {Promise}
   */
  function deleteFilter (filter) {
    return $scope.ajax({
      url: '/Int/Work/Queue/Filter/Delete',
      method: 'PUT',
      data: filter
    })
      .then(function () {
        return $scope.confirm(false);
      })
      .catch(angular.noop);
  }

  /**
   * Show/hide the state group.
   * @function toggleStateGroup
   * @param {object} $event - The angular event object
   * @returns {void}
   */
  $scope.toggleStateGroup = function ($event) {
    var element = angular.element($event.target.parentElement);

    if (element.hasClass('open'))
      element.removeClass('open');
    else
      element.addClass('open');
  }

  /**
   * Delete the current filter.
   * @function deleteFilter
   * @returns {Promise}
   **/
  $scope.deleteFilter = function () {
    return $scope.confirmDialog('Delete Filter', 'Do you want to delete this filter?')
      .then(function () {
        return deleteFilter($scope.ngDialogData.filter);
      })
      .catch(angular.noop);
  }

  /**
   * Set the default states for assessors
   * @function assessorDefaults
   * @returns {void}
   **/
  $scope.assessorDefaults = function () {
    selectStates([3, 4, 16, 19, 21, 22, 25]);
  }

  /**
   * Set the default states for directors
   * @function directorDefaults
   * @returns {void}
   **/
  $scope.directorDefaults = function () {
    selectStates([5, 6, 17, 18, 8, 28]);
  }

  /**
   * Unselect all states
   * @function clearDefaults
   * @returns {void}
   **/
  $scope.clearDefaults = function () {
    selectStates();
  }

  /**
   * Load the training periods for the currently selected fiscal year
   * @function loadTrainingPeriods
   * @returns {Promise}
   **/
  $scope.loadTrainingPeriods = function () {
    return loadTrainingPeriods($scope.model.FiscalYearId, $scope.model.GrantStreamId)
      .catch(angular.noop);
  }

  /**
   * Load the grant streams for the currently selected grant program
   * @function loadGrantStreams
   * @returns {Promise}
   **/
  $scope.loadGrantStreams = function () {
    return loadGrantStreams($scope.model.GrantProgramId)
      .catch(angular.noop);
  }

  init();
});
