var sectionCounter = 1;

app.controller('Section', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = Object.assign({
    name: $attrs.name || 'Section' + sectionCounter++,
    show: typeof ($attrs.show) === 'undefined' ? false : String($attrs.show).toLowerCase() === 'true',
    showHide: typeof ($attrs.showHide) === 'undefined' ? true : String($attrs.showHide).toLowerCase() === 'true',
    editing: false,
    save: {},
    loaded: false
  }, $scope.section);
  $scope.model = {}; // Required to stop prototypal inheritance.
  $scope.errors = Object.assign({}, $scope.errors);
  if (!$scope.section.isParent && $scope.children)
    $scope.children.push($scope);
  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  /**
   * Initialize the section and populate the $scope.model.
   * This event will only act if the target matches the $scope.section.name or the force parameter is set to true.
   * This event will call the $scope.init, and expects the model to be returned.
   * @function init
   * @param {object} event - The event that fired.
   * @param {any} data - The data passed in the event.
   * @param {string} [data.target] - The section name to initialize.
   * @returns {Promise}
   */
  function init(event, data) {
    event = Object.assign({}, event);
    // Need to initize the section if it isn't loaded.
    if (typeof ($scope.init) === 'function' && (!Utils.action($scope.section.loaded) || (event.ctrlKey))) {
      return $scope.init().then(function (response) {
        if (typeof ($scope.section.loaded) !== 'function') $scope.section.loaded = true;
        return response;
      });
    }

    return Promise.resolve();
  }
  $scope.$on('init', init);

  /**
   * Buble the event to all children if this is the parent.
   * @function bubble
   * @param {any} event - The event name.
   * @param {any} data - The data to pass in the event.
   * @returns {void}
   */
  function bubble(event, data) {
    if (!event) console.warn('The argument "event" is required to bubble.');
    var foptions = Object.assign({}, data);
    if (!Utils.verifyMatch($scope.section.name, foptions.source) && foptions.bubble && $scope.section.isParent) {
      foptions.bubble = false;
      $scope.broadcast(event, foptions); // Only bubble from the parent.
    }
  }

  /**
   * Update section event.
   * Update the section with the passed data.
   * This event will call the $scope.update if there is not target, or if the target matches the $scope.section.name.
   * @function updateSection
   * @param {object} event - The event that fired.
   * @param {any} data - The data passed to the event.
   * @param {string} [data.target] - The section name to update.  If none provided, update all sections.
   * @param {bool} [data.bubble=false] - Whether to bubble the event to children.
   * @returns {void}
   **/
  function update(event, data) {
    event = Object.assign({}, event);
    var foptions = Object.assign({ bubble: false }, data);
    // Only update if the section name matches or if there is no target.
    if ((!foptions.target || Utils.verifyMatch($scope.section.name, foptions.target))
      && !Utils.verifyMatch($scope.section.name, foptions.source)) {
      console.log('update event - ' + $scope.section.name);
      return $timeout(function () {
        // Copy the data into scope.
        Utils.copyWhere(foptions, $scope, { ignore: 'source,target' });
      }).then(function () {
        bubble('update', foptions);
        if (typeof ($scope.section.onUpdate) === 'function') return $scope.section.onUpdate(event, foptions);
      });
    }
    bubble('update', foptions);
  }
  $scope.$on('update', update);

  /**
   * Refresh the section event.
   * This event will only act if the target matches the $scope.section.name and the source of the event didn't originate from this section.
   * This event will call the $scope.refresh if the section has already been loaded, or it will fire the 'initSection' event if the section isn't loaded.
   * @function refresh
   * @param {object} event - The event that fired.
   * @param {any} data - The data passed to the event.
   * @param {string} [data.target] - The section name to refresh.
   * @param {bool} [data.force=false] - Whether to force a refresh even if the section doesn't need it.
   * @param {bool} [data.bubble=true] - Whether to bubble the event to children.
   * @returns {Promise}
   **/
  function refresh(event, data) {
    event = Object.assign({}, event);
    var foptions = Object.assign({ force: false, bubble: true }, data);
    // Only refresh if the section name matches or if there is no target.
    if ((!foptions.target || Utils.verifyMatch($scope.section.name, foptions.target))
      && !Utils.verifyMatch($scope.section.name, foptions.source)
      && $scope.section.show
      && (!Utils.action($scope.section.loaded) || (event.ctrlKey) || data.force)) {
      console.log('refresh event - ' + $scope.section.name);
      // Copy the data into scope.
      Utils.copyWhere(foptions, $scope, { ignore: 'source,target' });
      bubble('refresh', foptions);
      if (typeof ($scope.section.onRefresh) === 'function') return $scope.section.onRefresh(event, data);
    }
    bubble('refresh', foptions);
    return Promise.resolve();
  }
  $scope.$on('refresh', refresh);

  /**
   * Show the section event.
   * This event will call the $scope.refresh if the section has already been loaded, or it will fire the 'initSection' event if the section isn't loaded.
   * @function show
   * @param {object} event - The event that fired.
   * @param {any} data - The data passed in the event.
   * @param {string} [data.target] - The section name to refresh.
   * @param {bool} [data.bubble=true] - Whether to bubble the event to children.
   * @returns {Promise}
   */
  function show(event, data) {
    event = Object.assign({}, event);
    var foptions = Object.assign({ bubble: true }, data);
    // Only show if the section name matches or if there is no target.
    if ((!foptions.target || Utils.verifyMatch($scope.section.name, foptions.target))
      && !Utils.verifyMatch($scope.section.name, foptions.source)) {
      console.log('show event - ' + $scope.section.name);
      // Copy the data into scope.
      Utils.copyWhere(foptions, $scope, { ignore: 'source,target' });
      return init(event, data)
        .then(function () {
          return $timeout(function () {
            $scope.section.show = true;
            Utils.initValue($scope, 'parent.showing.' + $scope.section.name, true);
            bubble('show', foptions);
            if (typeof ($scope.section.onShow) === 'function') return $scope.section.onShow(event, data);
          });
        });
    }
    Utils.action(event.preventDefault);
    Utils.action(event.stopPropagation);
    bubble('show', foptions);
  }
  $scope.$on('show', show);
  $scope.show = show;

  /**
   * Hide the section event.
   * @function hideSection
   * @param {object} event - The event that fired.
   * @param {any} data - The data passed in the event.
   * @param {string} [data.target] - The section name to refresh.
   * @param {bool} [data.bubble=true] - Whether to bubble the event to children.
   * @returns {Promise}
   */
  function hide(event, data) {
    event = Object.assign({}, event);
    var foptions = Object.assign({ bubble: true }, data);
    // Only hide if the section name matches or if there is no target.
    if ((!foptions.target || Utils.verifyMatch($scope.section.name, foptions.target))
      && !Utils.verifyMatch($scope.section.name, foptions.source)) {
      console.log('hide event - ' + $scope.section.name);
      if ($scope.section.showHide) {
        $scope.section.show = false;
        Utils.initValue($scope, 'parent.showing.' + $scope.section.name, false);
        $scope.clearAlert();
        $scope.clearErrors();
        bubble('hide', foptions);
        if (typeof ($scope.section.onHide) === 'function') return $scope.section.onHide(event, data);
      }
    }
    Utils.action(event.preventDefault);
    Utils.action(event.stopPropagation);
    bubble('hide', foptions);
  }
  $scope.$on('hide', hide);
  $scope.hide = hide;

  /**
   * Toggle the section event.
   * @function toggleSection
   * @param {object} event - The event that fired.
   * @param {any} data - The data passed in the event.
   * @returns {Promise}
   */
  function toggle(event, data) {
    event = Object.assign({}, event);
    var foptions = Object.assign({}, data);
    // Only toggle if the section name matches or if there is no target.
    if ((!foptions.target || Utils.verifyMatch($scope.section.name, foptions.target))
      && !Utils.verifyMatch($scope.section.name, foptions.source)) {
      console.log('toggle event - ' + $scope.section.name);
      if (!$scope.section.show || (event && event.ctrlKey)) return show(event, data);
      else if ($scope.section.showHide) return hide(event, data);
    }
    Utils.action(event.preventDefault);
    Utils.action(event.stopPropagation);
  }
  $scope.toggle = toggle;

  /**
   * Open a confirmation dialog if there is a section being currently edited.
   * @function confirmCancel
   * @returns {Promise}
   **/
  $scope.confirmCancel = function (title, question) {
    if (!title) title = 'Finish Editing';
    if (!question) question = 'Finish editing before attempting to edit another section. Clicking "Cancel" will undo any edits you have currently made.';
    return ngDialog.openConfirm({
      template: '/content/dialogs/_FinishEditing.html',
      data: {
        title: title,
        question: question
      }
    }).catch(function (data) {
      $scope.parent.cancel(event, data);
      return Promise.reject();
    });
  }

  /**
   * Change the section to edit mode.
   * @function edit
   * @param {object} event - The event that fired.
   * @param {any} data - The data passed in the event.
   * @param {string} [data.target] - The section name to refresh.
   * @param {bool} [data.bubble=true] - Whether to bubble the event to children.
   * @returns {Promise}
   **/
  function edit(event, data) {
    event = Object.assign({}, event);
    var foptions = Object.assign({ bubble: true }, data);
    // Only edit if the section name matches or if there is no target.
    if ((!foptions.target || Utils.verifyMatch($scope.section.name, foptions.target))
      && !Utils.verifyMatch($scope.section.name, foptions.source)) {
      console.log('edit event - ' + $scope.section.name);

      var _show = function (event, data) {
        return show(event, data)
          .then(function () {
            $scope.backup();
            return $timeout(function () {
              // Copy the data into scope.
              Utils.copyWhere(foptions, $scope, { ignore: 'source,target' });
              $scope.section.editing = true;
              Utils.initValue($scope, 'parent.editing', $scope.section.name);
              Utils.initValue($scope, 'parent.cancel', $scope.cancel);
              bubble('edit', foptions);
              if (typeof ($scope.section.onEdit) === 'function') return $scope.section.onEdit(event, data);
            });
          });
      }

      if (!Utils.getValue($scope, 'parent.editing')) {
        return _show(event, data);
      } else {
        return $scope.confirmCancel()
          .catch(function () {
            return _show(event, data);
          });
      }
    }
    Utils.action(event.preventDefault);
    Utils.action(event.stopPropagation);
    bubble('edit', foptions);
  }
  $scope.$on('edit', edit);
  $scope.edit = edit;

  /**
   * Cancel editing and undo any changes to the section.
   * @function cancel
   * @param {object} event - The event that fired.
   * @param {any} data - The data passed in the event.
   * @param {string} [data.target] - The section name to refresh.
   * @param {bool} [data.bubble=true] - Whether to bubble the event to children.
   * @returns {void}
   **/
  function cancel(event, data) {
    event = Object.assign({}, event);
    var foptions = Object.assign({ bubble: true }, data);
    // Only cancel if the section name matches or if there is no target.
    if ((!foptions.target || Utils.verifyMatch($scope.section.name, foptions.target))
      && !Utils.verifyMatch($scope.section.name, foptions.source)) {
      console.log('cancel event - ' + $scope.section.name);
      $scope.section.editing = false;
      Utils.initValue($scope, 'parent.editing', null);
      Utils.initValue($scope, 'parent.cancel', null);
      $scope.restore();
      $scope.clearAlert();
      $scope.clearErrors();
      bubble('cancel', foptions);
      if (typeof ($scope.section.onCancel) === 'function') return $scope.section.onCancel(event, data);
    }
    Utils.action(event.preventDefault);
    Utils.action(event.stopPropagation);
    bubble('cancel', foptions);
  }
  $scope.$on('cancel', cancel);
  $scope.cancel = cancel;


  /**
   * Save the section data.  Make AJAX request to endpoint.
   * @function save
   * @param {object} event - The event that fired.
   * @param {any} data - The data passed in the event.
   * @param {string} [data.target] - The section name to refresh.
   * @param {bool} [data.bubble=true] - Whether to bubble the event to children.
   * @returns {Promise}
   */
  function save(event, data) {
    event = Object.assign({}, event);
    var foptions = Object.assign({ bubble: true }, data);
    // Only save if the section name matches or if there is no target.
    if ((!foptions.target || Utils.verifyMatch($scope.section.name, foptions.target))
      && !Utils.verifyMatch($scope.section.name, foptions.source)) {
      if (typeof ($scope.section.preSave) === 'function' ? $scope.section.preSave(event, data) : true) {
        console.log('save event - ' + $scope.section.name);
        $scope.clearErrors();
        return $scope.ajax($scope.section.save)
          .then(function (response) {
            return $timeout(function () {
              $scope.section.editing = false;
              Utils.initValue($scope, 'parent.editing', null);
              return $scope.sync(response.data, $scope.model)
                .then(function () {
                  return response;
                });
            }).then(function (response) {
              bubble('save', foptions); // Tell child controllers to save.
              if (typeof ($scope.section.onSave) === 'function') return $scope.section.onSave(event, { response: response, data: data });
              return response;
            });
          })
          .catch(angular.noop);
      }
    }
    Utils.action(event.preventDefault);
    Utils.action(event.stopPropagation);

    bubble('save', foptions); // Tell child controllers to save.
    return Promise.resolve();
  }
  $scope.$on('save', save);
  $scope.save = save;
});
