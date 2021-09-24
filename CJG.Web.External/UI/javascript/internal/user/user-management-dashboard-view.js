// User Management dashboard
app.controller('UserManagementDashboard', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  angular.extend(this, $controller('ParentSection', { $scope: $scope, $attrs: $attrs }));

  $scope.section = {
    name: 'UserManagementDashboard',
    onRefresh: function () {
      return loadUsers().catch(angular.noop);
    },
    userId: parseInt($attrs.userId)
  };
  $scope.quantities = [10, 25, 50, 100];
  $scope.filter = {
    Page: 1,
    Quantity: $scope.quantities[0],
    OrderBy: []
  };
  $scope.current = {
    FirstName: "",
    LastName: ""
  }
  $scope.cache = [];

  /**
   * Load internal users for the table.
   * @function loadUsers
   * @param {int} page
   * @param {int} quantity
   * @returns {Promise}
   * */
  function loadUsers(page, quantity) {
    if (!page) page = 1;
    if (!quantity) quantity = $scope.quantities[0];
    return $scope.load({
      url: '/Int/Admin/Users?page=' + page + '&quantity=' + quantity,
      method: 'POST',
      data: $scope.filter,
      set: 'data',
      condition: quantity != $scope.model.Quantity || $scope.cache.length < page || !$scope.cache[page - 1] || searchCriteriaChanged()
    })
      .then(function () {
        $scope.model = $scope.data.Users;
        return $timeout(function () {
          setupPage(page, quantity);
        })
      })
      .catch(angular.noop);
  }

  /**
  * Load internal roles.
  * @function loadRoles
  * @returns {Promise}
  * */
  function loadRoles() {
    return $scope.load({
      url: '/Int/Admin/Application/Roles/',
      set: 'roles',
      condition: !$scope.roles || !$scope.roles.length,
    })
      .catch(angular.noop);
  }

  /**
  * Opens the Edit/Add user modal.
  * @function openUserModal
  * @param {int} id
  * @returns {Promise}
  * */
  $scope.openUserModal = function (id) {
    return showDialog({
      Id: id,
      Roles: $scope.roles
    })
      .catch(angular.noop);
  }

  /**
  * Ajax call for User modal.
  * @function showDialog
  * @param {object} user
  * @returns {Promise}
  * */
  function showDialog(user) {
    return ngDialog.openConfirm({
      template: '/Int/Admin/User/View/' + user.Id,
      data: {
        user: user
      }
    }).then(function (updatedUser) {
      if (updatedUser.ApplicationUserId) {
        // Edit
        $scope.updateUserModel(updatedUser);
      } else {
        // Add
        $scope.model.Items.push(updatedUser);
        $scope.pager.items.total++;
      }
    });
  }

  /**
  * Updates the row with the just changed value.
  * @function updateUserModel
  * @param {object} user
  * @returns {Promise}
  * */
  $scope.updateUserModel = function (user) {
    for (var i = 0; i < $scope.model.Items.length; i++) {
      if ($scope.model.Items[i].ApplicationUserId == user.ApplicationUserId) {
        return $scope.model.Items[i] = user;
      }
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

    if (force || searchCriteriaChanged()) $scope.cache = [];

    return loadUsers(page, quantity)
      .then(function () {
        $scope.current = {
          SearchCriteria: $scope.filter.SearchCriteria,
          OrderBy: $scope.filter.OrderBy
        }
      })
      .catch(angular.noop);
  }

  /**
   * Check if the filter has been changed.
   * @function searchCriteriaChanged
   * @returns {bool}
   **/
  function searchCriteriaChanged() {
    if ($scope.filter.SearchCriteria !== $scope.current.SearchCriteria) return true;

    return false;
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
        model: angular.copy($scope.model), // Cache page.
        pager: angular.copy($scope.pager) 
      }
    } else {
      $scope.model = $scope.cache[page - 1].model;
      $scope.pager = $scope.cache[page - 1].pager;
    }
  }

  /**
   * Make an AJAX request with the filter including the search terms.
   * @function search
   * @param {any} $event - The angular event.
   * @returns {Promise}
   */
  $scope.search = function ($event) {
    if ($event.keyCode === 13 || $event.type === 'click') return $scope.applyFilter();
    return Promise.resolve();
  }

  /**
   * Initialize the data for the form.
   * @function init
   * @returns {void}
   **/
  $scope.init = function() {
    loadUsers($scope.filter.Page, $scope.filter.Quantity);
    loadRoles();
  };

  $scope.init();
});
