var utils = require('../../shared/utils');

app.controller('GrantProgramEmployerGrantConfig', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'GrantProgramEmployerGrantConfig',
    save: {
      url: '/Int/Admin/Grant/Program/Employer/Grant/Expense/Types',
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.section.isLoaded;
    },
    onSave: function () {
      $scope.emit('update', { model: $scope.model });
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.$watch('$parent.model', function (newValue, oldValue) {
    $scope.model = newValue;
  });

  /**
   * Make AJAX request for claim types
   * @function loadClaimTypes
   * @returns {Promise}
   **/
  function loadClaimTypes() {
    return $scope.load({
      url: '/Int/Admin/Claim/Types',
      set: 'claimTypes',
      condition: !$scope.claimTypes || !$scope.claimTypes.length
    });
  }

  /**
   * Make AJAX request for expense types
   * @function loadExpenseTypes
   * @returns {Promise}
   **/
  function loadExpenseTypes() {
    return $scope.load({
      url: '/Int/Admin/Expense/Types',
      set: 'expenseTypes',
      condition: !$scope.expenseTypes || !$scope.expenseTypes.length
    });
  }

  /**
   * Make AJAX request for rates
   * @function loadRates
   * @returns {Promise}
   **/
  function loadRates() {
    return $scope.load({
      url: '/Int/Admin/Rates',
      set: 'rates',
      condition: !$scope.rates || !$scope.rates.length
    });
  }

  /**
   * Initialize the data for the form
   * @function init
   * @returns {void{
   **/
  $scope.init = function () {
    return Promise.all([
      loadClaimTypes(),
      loadExpenseTypes(),
      loadRates()
    ])
      .then(function () {
        $scope.section.isLoaded = true;
      })
      .catch(angular.noop);
  }

  /**
   * Get the expense type caption that is selected.
   * @function expenseTypeCaption
   * @param {int} id - Expense type Id.
   * @returns {string}
   **/
  $scope.expenseTypeCaption = function (id) {
    var item = $scope.expenseTypes.find(function (e) { return e.Key === id; });
    return item ? item.Value : null;
  }

  /**
   * Get the claim type caption that is selected.
   * @function claimTypeCaption
   * @returns {string}
   **/
  $scope.claimTypeCaption = function () {
    var item = $scope.claimTypes.find(function (e) { return e.Key === $scope.model.ProgramConfiguration.ClaimTypeId; });
    return item ? item.Value : null;
  }

  /**
   * Get the rate caption.
   * @function rateCaption
   * @param {float} rate
   * @returns {string}
   */
  $scope.rateCaption = function (rate) {
    return $scope.rates.find(function (item) { return item.Key === rate; }).Value;
  }

  /**
   * Reorder the array by moving the item as the specified index up one.
   * @function reorder
   * @param {Array} array - The array to modify.
   * @param {int} index - The index of the item to move up one.
   * @returns {void}
   */
  $scope.reorder = function (array, index) {
    var rowSequence = array[index].RowSequence;
    array[index].RowSequence = array[index - 1].RowSequence;
    array[index - 1].RowSequence = rowSequence;
    Utils.moveItem(array, index, index - 1);
  }

  /**
   * Set the selected expense type as the active one for editing.
   * @function selectExpense
   * @param {object} item - The expense type item.
   * @returns {void}
   */
  $scope.selectExpense = function (item) {
    Utils.initValue($scope, 'section.selectedExpense.Selected', false);
    item.Selected = true;
    $scope.section.selectedExpense = item;
  }

  /**
   * Creates a new expense type object and sets it as the active one for editing.
   * @function createExpense
   * @returns {void}
   **/
  $scope.createExpense = function () {
    $scope.section.selectedExpense = {
      Id: 0,
      IsActive: true,
      Delete: false
    };
    $scope.model.ProgramConfiguration.EligibleExpenseTypes.push($scope.section.selectedExpense);
  }

  /**
   * Deletes the expense type at the specified index.
   * @function deleteExpense
   * @param {object} item - The item in the array.
   * @returns {void}
   */
  $scope.deleteExpense = function (item) {
    var index = $scope.model.ProgramConfiguration.EligibleExpenseTypes.indexOf(item);
    if (item.Id === 0) $scope.model.ProgramConfiguration.EligibleExpenseTypes.splice(index, 1);
    else {
      $scope.confirmCancel('Delete Expense Type', 'Do you want to delete this expense type?')
        .then(function () {
          item.Delete = true;
        })
        .catch(angular.noop);
    }
  }

  /**
   * Sets the expense type breakdown as the active one for editing.
   * @function selectBreakdown
   * @param {object} item - The eligible expense type breakdown item.
   * @returns {void}
   */
  $scope.selectBreakdown = function (item) {
    Utils.initValue($scope, 'section.selectedBreakdown.Selected', false);
    item.Selected = true;
    $scope.section.selectedBreakdown = item;
  }

  /**
   * Creates a new expense type breakdown object and sets it ast the ative one for editing.
   * @function createBreakdown
   * @returns {void}
   **/
  $scope.createBreakdown = function () {
    $scope.section.selectedBreakdown = {
      Id: 0,
      IsActive: true,
      Delete: false
    };
    $scope.section.selectedExpense.Breakdowns.push($scope.section.selectedBreakdown);
  }

  /**
   * Delete the expense type breakdown at the specified index.
   * @function deleteBreakdown
   * @param {int} index - The index position of the item to delete.
   * @returns {void}
   */
  $scope.deleteBreakdown = function (index) {
    var item = $scope.section.selectedExpense.Breakdowns[index];
    if (item.Id === 0) $scope.section.selectedExpense.Breakdowns.splice(index, 1);
    else {
      $scope.confirmCancel('Delete Expense Type Breakdown', 'Do you want to delete this breakdown?')
        .then(function () {
          item.Delete = true;
        })
        .catch(angular.noop);
    }
  }

  /**
   * Open a new tab and display the message.
   * @function preview
   * @param {string} title - The title of the tab.
   * @param {string} message - The message to test in the new tab.
   * @returns {Promise}
   **/
  $scope.preview = function (title, message) {
    return $scope.ajax({
      url: '/Int/Admin/Grant/Program/Preview',
      method: 'POST',
      data: {
        title: title,
        message: message
      }
    })
      .then(function (response) {
        var w = window.open('about:blank');
        w.document.open();
        w.document.write(response.data);
        w.document.close();
      })
      .catch(angular.noop);
  }

  /**
   * Remove the rate for certain expense types.
   * @function changeExpenseType
   * @returns {void}
   **/
  $scope.changeExpenseType = function () {
    switch ($scope.section.selectedExpense.ExpenseTypeId) {
      case (utils.ExpenseTypes.ParticipantAssigned):
      case (utils.ExpenseTypes.ParticipantLimited):
        $scope.section.selectedExpense.Rate = null;
        break;
      case (utils.ExpenseTypes.NotParticipantLimited):
      case (utils.ExpenseTypes.AutoLimitEstimatedCosts):
    }
  }
});
