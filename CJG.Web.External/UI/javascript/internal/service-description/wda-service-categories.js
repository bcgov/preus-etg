var uniqid = require('uniqid');
var utils = require('../../shared/utils');

app.controller('WDAServiceCategories', function ($scope, $attrs, $controller, Utils) {
  $scope.section = {
    name: 'WDAServiceCategories',
    displayName: 'Service Descriptions',
    save: {
      url: '/Int/WDA/Services',
      method: 'PUT',
      data: function () {
        $scope.section.expand = angular.copy($scope.model);
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.ServiceCategories;
    },
    onSave: function () {
      $scope.model.ServiceCategories.map(function (item, index) {
        item.Selected = $scope.section.expand.ServiceCategories[index].Selected;
        var serviceLines = $scope.section.expand.ServiceCategories[index].ServiceLines;
        item.ServiceLines.map(function (item, index) {
          item.Selected = serviceLines[index].Selected;
        });
      });
      $scope.section.expand = null;
    },
    onRefresh: function () {
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  if (typeof ($scope.serviceTypes) === 'undefined') $scope.serviceTypes = [];

  /**
   * Make AJAX request to load wda services data.
   * @function loadWDAServices
   * @returns {Promise}
   **/
  function loadWDAServices() {
    return $scope.load({
      url: '/Int/WDA/Services',
      set: 'model'
    });
  }

  /**
   * Initialize by loading the WDA services.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadWDAServices()
    ])
      .catch(angular.noop);
  }

  /**
   * Load the WDA Services preview page and open it in a new tab
   * @function preview
   * @returns {Promise}
   **/
  $scope.preview = function () {
    return $scope.ajax({
      url: '/Int/WDA/Service/Preview',
      method: 'POST',
      data: $scope.model
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
   * Add a new service category item.
   * @function addServiceCategory
   * @returns {void}
   */
  $scope.addServiceCategory = function () {
    var items = $scope.model.ServiceCategories;
    var item = {
      uid: uniqid(),
      Id: 0,
      RowSequence: 0,
      ServiceTypeId: utils.ServiceTypes.EmploymentServicesAndSupports,
      Caption: '',
      Description: '',
      IsActive: false,
      ServiceLines: [],
      Deleted: false,
      AutoInclude: true,
      AllowMultiple: false,
      CompletionReport: false,
      AllowDelete: true,
      MinProviders: 0,
      MaxProviders: 5,
      MinPrograms: 0,
      MaxPrograms: 5
    };
    if (items.length > 0) {
      var lastItem = items[items.length - 1];
      item.RowSequence = lastItem.RowSequence + 1;
    }
    items.push(item);
  }

  /**
   * Add a new service line item.
   * @function addServiceLine
   * @param {object} category - The service category.
   * @returns {void}
   */
  $scope.addServiceLine = function (category) {
    var items = category.ServiceLines;
    var item = {
      uid: uniqid(),
      Id: 0,
      RowSequence: 0,
      Caption: '',
      Description: '',
      IsActive: false,
      ServiceLineBreakdowns: [],
      Deleted: false,
      EnableCost: category.ServiceTypeId === utils.ServiceTypes.SkillsTraining,
      AllowDelete: true
    };
    if (items.length > 0) {
      var lastItem = items[items.length - 1];
      item.RowSequence = lastItem.RowSequence + 1;
    }
    items.push(item);
  }

  /**
   * Add a new service line breakdown item.
   * @function addServiceLineBreakdown
   * @param {object} serviceLine - The service line.
   * @returns {void}
   */
  $scope.addServiceLineBreakdown = function (serviceLine) {
    var items = serviceLine.ServiceLineBreakdowns;
    var item = {
      uid: uniqid(),
      Id: 0,
      RowSequence: 0,
      Caption: '',
      Description: '',
      IsActive: false,
      AllowDelete: true
    };
    if (items.length > 0) {
      var lastItem = items[items.length - 1];
      item.RowSequence = lastItem.RowSequence + 1;
    }
    items.push(item);
  }

  /**
  * Delete the item from the array.
  * @function deleteItem
  * @param {Array} items - An array to delete from.
  * @param {int} index - The index in the array to delete from.
  * @returns {void}
  */
  $scope.deleteItem = function (items, index) {
    var item = items[index];
    $scope.confirmDialog('Delete', 'Are you sure you want to delete "' + item.Caption + '"?')
      .then(function () {
        if (item.Id === 0) {
          items.splice(index, 1);
        } else {
          items[index].Deleted = true;
        }
      })
      .catch(angular.noop);
  }

  /**
   * Change the order of the items in the array.
   * @function changeOrder
   * @param {Array} items - The array to reorder.
   * @param {int} index - The index position to change.
   * @returns {void}
   */
  $scope.changeOrder = function (items, index) {
    if (items.length === 0 || index === 0) return;

    items.map(function (item, i) { item.RowSequence = i; });

    var item = items[index];
    var previousItem = items[index - 1];

    var currentRowSequence = item.RowSequence;
    item.RowSequence = previousItem.RowSequence;
    previousItem.RowSequence = currentRowSequence;

    items.sort(function (a, b) {
      return a.RowSequence < b.RowSequence ? -1 : a.RowSequence > b.RowSequence ? 1 : 0;
    })
  }

  /**
   * Update the configuration properties for the service category and service lines based on the service type.
   * @function changeServiceType
   * @param {any} serviceCategory
   * @returns {void}
   */
  $scope.changeServiceType = function (serviceCategory) {
    switch (serviceCategory.ServiceTypeId) {
      case (utils.ServiceTypes.EmploymentServicesAndSupports):
        serviceCategory.CompletionReport = true;
        serviceCategory.MinPrograms = 0;
        serviceCategory.MaxPrograms = 0;
        serviceCategory.MinProviders = 0;
        serviceCategory.MaxProviders = 5;

        serviceCategory.ServiceLines.map(function (serviceLine) {
          serviceLine.EnableCost = false;
        });
        break;
      case (utils.ServiceTypes.Administration):
        serviceCategory.CompletionReport = false;
        serviceCategory.MinPrograms = 0;
        serviceCategory.MaxPrograms = 0;
        serviceCategory.MinProviders = 0;
        serviceCategory.MaxProviders = 0;

        serviceCategory.ServiceLines.map(function (serviceLine) {
          serviceLine.EnableCost = false;
        });
        break;
      case (utils.ServiceTypes.SkillsTraining):
      default:
        serviceCategory.CompletionReport = false;
        serviceCategory.MinPrograms = 0;
        serviceCategory.MaxPrograms = 5;
        serviceCategory.MinProviders = 0;
        serviceCategory.MaxProviders = 0;

        serviceCategory.ServiceLines.map(function (serviceLine) {
          serviceLine.EnableCost = true;
        });
        break;
    }
  }
});
