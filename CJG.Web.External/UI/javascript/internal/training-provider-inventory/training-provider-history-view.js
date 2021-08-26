app.controller('TrainingProviderHistory', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'TrainingProviderHistory',
    onRefresh: function () {
      return loadTrainingProviderHistory().catch(angular.noop);
    }
  };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
   * Make AJAX request to load training provider history data.
   * @function loadTrainingProviderHistory
   * @returns {Promise}
   **/
  function loadTrainingProviderHistory() {
    return $scope.load({
      url: '/Int/Training/Provider/History/' + $attrs.trainingProviderId,
      set: 'model'
    })
      .then(function () {
        return $timeout(function () {
          $scope.originalNotes = $scope.model.TrainingProviderNotes;
        });
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
      loadTrainingProviderHistory()
    ]);
  }

  /**
   * Make AJAX request to update the note.
   * @function updateNote
   * @returns {Promise} promise
   **/
  $scope.updateNote = function () {
    return $scope.ajax({
      url: '/Int/Training/Provider/History/Note/' + $scope.model.TrainingProviderInventoryId,
      method: 'PUT',
      data: {
        id: $scope.model.TrainingProviderInventoryId,
        notesText: $scope.model.TrainingProviderNotes,
        rowVersion: $scope.model.RowVersion
      }
    })
      .then(function (response) {
        return $timeout(function () {
          $scope.setAlert({ response: { status: 200 }, message: 'Training Provider notes have been saved successfully' });
          $scope.model.RowVersion = response.data.RowVersion;
          $scope.originalNotes = $scope.model.TrainingProviderNotes;
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
    return $scope.originalNotes === $scope.model.TrainingProviderNotes;
  };

  /**
   * Reset the notes
   * @function resetNote
   * @returns {void}
   **/
  $scope.resetNote = function () {
    return $scope.model.TrainingProviderNotes = $scope.originalNotes;
  };

  /**
   * Delete the training provider.
   * @function deleteProvider
   * @returns {Promise} promise
   **/
  $scope.deleteProvider = function () {
    return $scope.confirmDialog('Delete', 'Are you sure you want to remove this training provider from the inventory?')
      .then(function () {
        return $scope.ajax({
          url: '/Int/Training/Provider/Inventory/Delete/',
          method: 'PUT',
          data: {
            id: $scope.model.TrainingProviderInventoryId,
            rowVersion: $scope.model.RowVersion
          }
        })
          .then(function () {
            window.location = '/Int/Training/Provider/Inventory/View';
          })
          .catch(angular.noop);
      })
      .catch(angular.noop);
  };

  /**
   * Get the filtered training provider history.
   * @function getTrainingProviderHistory
   * @param {string} pageKeyword - The search filter keyword.
   * @param {int} page - The page number.
   * @param {int} quantity - The number of items in a page.
   * @returns {Promise}
   **/
  $scope.getTrainingProviderHistory = function (pageKeyword, page, quantity) {
    return $scope.ajax({
      url: '/Int/Training/Provider/History/Search/' + $scope.model.TrainingProviderInventoryId + '/' + page + '/' + quantity + (pageKeyword ? '?search=' + pageKeyword : '')
    })
      .then(function (response) {
        return Promise.resolve(response.data);
      })
      .catch(angular.noop);
  };

  init();
});
