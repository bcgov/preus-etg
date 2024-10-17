app.controller('GrantProgramDeliveryPartner', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'GrantProgramDeliveryPartner',
    save: {
      url: '/Int/Admin/Grant/Program/Delivery/Partners',
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
   * Add a new delivery partner to the array.
   * @function addPartner
   * @returns {void}
   **/
  $scope.addPartner = function () {
    var rowSequence = $scope.model.DeliveryPartners.length ? $scope.model.DeliveryPartners[$scope.model.DeliveryPartners.length - 1].RowSequence : 0;
    $scope.model.DeliveryPartners.push({
      Id: 0,
      Caption: '',
      IsActive: true,
      RowSequence: rowSequence + 1
    });
  }

  /**
   * Move the specified delivery partner up in RowSequence.
   * @function movePartner
   * @param {any} index
   * @returns {void}
   */
  $scope.movePartner = function (index) {
    var array = $scope.model.DeliveryPartners;
    var rowSequence = array[index].RowSequence;
    array[index].RowSequence = array[index - 1].RowSequence;
    array[index - 1].RowSequence = rowSequence;
    Utils.moveItem(array, index, index - 1);
  }

  /**
   * Mark the specified delivery partner for deletion.
   * @function deletePartner
   * @param {any} index
   * @returns {void}
   */
  $scope.deletePartner = function (index) {
    var array = $scope.model.DeliveryPartners;
    if (array[index].Id === 0) array.splice(index, 1);
    else array[index].Delete = true;
  }

  /**
   * Add a new delivery partner service to the array.
   * @function addPartner
   * @returns {void}
   **/
  $scope.addService = function () {
    var rowSequence = $scope.model.DeliveryPartnerServices.length ? $scope.model.DeliveryPartnerServices[$scope.model.DeliveryPartnerServices.length - 1].RowSequence : 0;
    $scope.model.DeliveryPartnerServices.push({
      Id: 0,
      Caption: '',
      IsActive: true,
      RowSequence: rowSequence + 1
    });
  }

  /**
   * Move the specified delivery partner service up in RowSequence.
   * @function movePartner
   * @param {any} index
   * @returns {void}
   */
  $scope.moveService = function (index) {
    var array = $scope.model.DeliveryPartnerServices;
    var rowSequence = array[index].RowSequence;
    array[index].RowSequence = array[index - 1].RowSequence;
    array[index - 1].RowSequence = rowSequence;
    Utils.moveItem(array, index, index - 1);
  }

  /**
   * Mark the specified delivery partner service for deletion.
   * @function deletePartner
   * @param {any} index
   * @returns {void}
   */
  $scope.deleteService = function (index) {
    var array = $scope.model.DeliveryPartnerServices;
    if (array[index].Id === 0) array.splice(index, 1);
    else array[index].Delete = true;
  }
});
