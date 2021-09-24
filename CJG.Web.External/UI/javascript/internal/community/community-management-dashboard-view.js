// Community Management View
app.controller('CommunityManagementDashboard', function ($scope, $attrs, $controller, ngDialog) {
  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));
  
  /**
  * Opens the Edit/Add community modal.
  * @function openCommunityModal
  * @param {int} id
  * @returns {Promise}
  * */
  $scope.openCommunityModal = function (id) {
    return showDialog({
        Id: id
      })
      .catch(angular.noop);
  }

  /**
  * Ajax call for Community modal.
  * @function showDialog
  * @param {object} community
  * @returns {Promise}
  * */
  function showDialog(community) {
    if (!community.Id) community.Id = 0;
    return ngDialog.openConfirm({
      template: '/Int/Admin/Community/View/' + community.Id,
      data: {
        community: community
      }
    }).then(function (updatedCommunity) {
      if (updatedCommunity) {
        if ($scope.updateCommunityModel(updatedCommunity)) {
          // Edit
          formHelper.SetAlert($scope, status, ('Community Updated'));
        } else {
          // Add
          $scope.model.Communities.push(updatedCommunity);
          formHelper.SetAlert($scope, status, ('Community Added'));
        }
      }
    });
  }

  /**
  * Updates the row with the just changed value.
  * @function updateCommunityModel
  * @param {object} community
  * @returns {Promise}
  * */
  $scope.updateCommunityModel = function (community) {
    for (var i = 0; i < $scope.model.Communities.length; i++) {
      if ($scope.model.Communities[i].Id == community.Id) {
        return $scope.model.Communities[i] = community;
      }
    }
  }

  /**
  * Search for communities that match the filter.
  * @function search
  * @param {any} $event - The angular event.  *
  * @returns {Promise}
  **/
  $scope.search = function ($event) {
    if ($event.keyCode === 13 || $event.type === 'click') $scope.CommunitySearch = $scope.searchCriteria; 
    return Promise.resolve();
  }

  /**
  * Initializes the page with Community data.
  * @function init
  * @returns {Promise}
  * */
  $scope.init = function () {
    return $scope.load({
      url: '/Int/Admin/Communities',
      set: 'model'
    });
  }

  $scope.init();
});
