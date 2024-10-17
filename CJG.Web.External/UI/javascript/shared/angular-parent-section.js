app.controller('ParentSection', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = Object.assign({
    name: 'main',
    isParent: true
  }, $scope.section);
  if (typeof ($scope.parent) === 'undefined') $scope.parent = {};
  $scope.alert = {};
  $scope.errors = {};
  $scope.children = [];

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Bubble the specified event to all children.
   * @function bubble
   * @param {any} event - The event that fired.
   * @param {any} data - The data passed to the event.
   * @param {string} data.event - The event name to broadcast to all children.
   * @returns {void}
   */
  function bubble(event, data) {
    var foptions = Object.assign({}, data);

    if (foptions.event) {
      console.log('bubble event - ' + $scope.section.name + ' broadcast: ' + data.event);
      delete data.event;
      $scope.broadcast(foptions.event, data);
    }
  }
  $scope.$on('bubble', bubble);

  /**
   * Check if all child sections has been opened.
   * @function allSectionsOpened
   * @returns {boolean} all opened
   */
  $scope.allSectionsOpened = function () {
    var result = true;
    for (var i = 0; i < $scope.children.length; i++) {
      var child = $scope.children[i].section;
      if (child.showHide && !child.excludeFromToggleAll)
        result &= child.show;
    }
    return result;
  };

  /**
   * Toggle all child sections.
   * @function toggleAllSections
   */
  $scope.toggleAllSections = function () {
    var targets = '';
    for (var i = 0; i < $scope.children.length; i++) {
      var child = $scope.children[i].section;
      if (child.showHide && !child.excludeFromToggleAll)
        targets += (i === 0 ? '' : ',') + child.name;
    }
    $scope.broadcast($scope.allSectionsOpened() ? 'hide' : 'show', { target: targets });
  };
});
