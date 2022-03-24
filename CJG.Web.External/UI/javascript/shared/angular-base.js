var controllerCounter = 1;

require('./directives/editing');
require('./directives/not-editing');
require('./directives/validation');
require('./directives/file-read');
require('./directives/file-selected');
require('./directives/files-selected');
require('./directives/selected');

app.controller('Base', function ($scope, $sce, $timeout, Utils, ngDialog) {
  $scope.section = Object.assign({
    name: 'Controller' + controllerCounter++
  }, $scope.section);

  /**
   * Converts the date to PST timezone.
   * @function toPST
   * @param {any} date - A date value to convert.
   * @param {string='YYYY-MM-DD'} format - The format to return the date (default)
   * @return {string} The converted and formatted date value.
   */
  $scope.toPST = Utils.toPST;

  /**
   * Backup the model.
   * Copy the $scope.model into $scope.state.backup
   * @function backup
   * @return {void}
   **/
  function backup() {
    if ($scope.model) {
      console.log('backing up - ' + $scope.section.name);
      $scope.section.backup = angular.copy($scope.model);
    }
  }
  $scope.backup = backup;

  /**
   * Restore the back of the model.
   * Copy the $scope.state.backup into $scope.model.
   * @function restore
   * @returns {void}
   **/
  function restore() {
    if ($scope.section.backup) {
      console.log('restoring - ' + $scope.section.name);
      //Utils.initValue($scope, 'model', $scope.section.backup);
      Utils.sync($scope.section.backup, $scope.model);
      //$scope.model = angular.copy($scope.section.backup);
    }
  }
  $scope.restore = restore;

  /**
   * Clear the errors from scope.  Deletes $scope.errors property.
   * @function clearErrors
   * @returns {void}
   **/
  $scope.clearErrors = function () { Utils.clearErrors($scope); }

  /**
   * Clear the alert messages.  Sets $scope.alert.message property to null.
   * @function clearAlert
   * @returns {void}
   **/
  $scope.clearAlert = function () { Utils.clearAlert($scope); }

  /**
   * Set an alert message based on the options provied.
   * @function setAlert
   * @param {any} options
   */
  $scope.setAlert = function (options) { Utils.setAlert(Object.assign({ $scope: $scope, $sce: $sce }, options)); }

  /**
   * Handle an AJAX failure.
   * If the response is a standardized view model contains error information, it will use this to populate the $scope.errors object and the $scope.alert object.
   * @function handleAjaxFailure
   * @param {any} options
   * @returns {void}
   */
  $scope.handleAjaxFailure = function (options) {
    Utils.handleAjaxFailure(Object.assign({
      $scope: $scope,
      $sce: $sce,
      copy: {
        path: 'errors.',
        format: function (key) {
          return key;
        }
      }
    }, options));
  }

  /**
   * Sync the destination and source values.
   * Only update where there is a change.
   * This mutates the dest.
   * @function sync
   * @param {any} source - The source data you want to copy from.
   * @param {any} dest - The destination to copy the source into.
   * @returns {Promise}
   **/
  $scope.sync = function (source, dest) {
    return $timeout(function () {
      Utils.sync(source, dest);
    });
  }

  /**
   * Make an AJAX request with the specified options.
   * @param {any} options - Configuration options for making the AJAX request
   * @param {string|function} options.url - The URL to the resource, or function to generate one.
   * @param {string} [options.method] - The HTTP method to use [GET|POST|PUT|DELETE|HEAD|OPTIONS] (default: GET).
   * @param {any} [options.data] - The data to send in the AJAX request, or a function to retreive the data.
   * @param {object} [options.headers] - An object containing HTTP headers to include in the request (default includes; RequestVerificationToken, X-Requested-With, Cache-Control, Pragma, If-Modified-Since).
   * @param {bool} [options.backup] - Whether to backup the model (default: false).
   * @returns {Promise} A promise containing the response.
   */
  function ajax(options) {
    var foptions = Object.assign({}, options);
    return new Promise(function (resolve, reject) {
      $scope.clearAlert();
      $scope.clearErrors();
      Utils.ajax(foptions)
        .then(function (response) {
          if (foptions.backup) $scope.backup();
          resolve(response);
        })
        .catch(function (response) {
          failure = true;
          $timeout(function () {
            $scope.handleAjaxFailure({ response: response });
          }).then(function () {
            reject(response);
          });
        });
    });
  }
  $scope.ajax = ajax;

  /**
   * Emits the event to the parent controller.
   * @function emit
   * @param {any} event - The event name.
   * @param {any} data - The data to include in the event.  This will be copied into the destination $scope if there are matches.
   * @returns {void}
   */
  function emit(event, data) {
    console.log('emiting "' + event + '" - ' + $scope.section.name);
    $scope.$emit(event, Object.assign({}, data, { source: $scope.section.name }));
  }
  $scope.emit = emit;

  /**
   * Broadcast the specified event and data to child controllers.
   * @function broadcast
   * @param {any} event - The event name.
   * @param {any} data - The data to send in the event.  This will be copied into the destination $scope if there are matches.
   * @returns {void}
   */
  function broadcast(event, data) {
    console.log('broadcasting "' + event + '" - ' + $scope.section.name);
    $scope.$broadcast(event, Object.assign({}, data, { source: $scope.section.name }));
  }
  $scope.broadcast = broadcast;

  /**
  * Make an AJAX request with the specified options and sets the value into the specified $scope location.
  * @param {any} options - Configuration options for making the AJAX request
  * @param {string|function} options.url - The URL to the resource, or function to generate one.
  * @param {string|function} options.set - The location to save the response.data (i.e. path.to.value), or a function to pas the response to.
  * @param {any} options.condition - The condition to check to determine if an AJAX request is required. If it returns true the AJAX request will be made.
  * @param {string} [options.method='GET'] - The HTTP method to use [GET|POST|PUT|DELETE|HEAD|OPTIONS] (default: GET).
  * @param {any} [options.data] - The data to send in the AJAX request, or a function to retreive the data.
  * @param {object} [options.headers] - An object containing HTTP headers to include in the request (default includes; RequestVerificationToken, X-Requested-With, Cache-Control, Pragma, If-Modified-Since).
  * @param {bool} [options.backup=false] - Whether to backup the model (default: false).
  * @param {bool} [options.overwrite=false] - Whether to overwrite the 'set' variable.  When 'true' it will, when 'false' it will sync instead (only update differences) (default: false).
  * @param {bool} [options.localCache=false] - Whether to check local storage cache for the data first.
  * @returns {Promise} A promise containing the response.
  */
  function load(options) {
    if (typeof (options.set) === 'undefined') throw new Error('Argument "options.set" is required.');
    var foptions = Object.assign({ condition: true, overwrite: false }, options);

    if (Utils.action(foptions.condition)) {
      var localStorageEnabled = Utils.localStorageEnabled();
      var version = localStorageEnabled ? window.localStorage.getItem('appVersion') : null;

      /**
       * Set the scope variable to the response data.
       * @param {any} response
       */
      function initLoadData(response) {
        return $timeout(function () {
          if (typeof (foptions.set) === 'function') foptions.set(response);
          else if (typeof (foptions.set) !== 'undefined') {
            var value = Utils.getValue($scope, foptions.set);
            // Overwrite if the object is empty.
            if (foptions.overwrite || value == null || typeof (value) === 'undefined' || (typeof (value) === "object" && Object.entries(value).length === 0 && !Array.isArray(value))) {
              Utils.initValue($scope, foptions.set, response.data);
            } else {
              Utils.sync(response.data, value);
            }
          }

          return response;
        });
      }

      // Use localStorage if it's enabled.
      if (foptions.localCache && localStorageEnabled) {
        if (!window.version || version === window.version) {
          var data = window.localStorage.getItem(foptions.url);
          if (data)
            return initLoadData({ status: 200, data: angular.fromJson(data) });
        } else {
          window.localStorage.setItem('appVersion', window.version);
        }
      }

      return $scope.ajax(foptions).then(function (response) {
        if (foptions.localCache && localStorageEnabled) {
          window.localStorage.setItem(foptions.url, angular.toJson(response.data));
        }
        return initLoadData(response);
      });
    }

    return Promise.resolve();
  }
  $scope.load = load;

  /**
   * Register shared resources to the parent scope.
   * The parent scope will be the top level controller which extended the Base controller (this file).
   * It will only register if the $scope[prop] === 'undefined'.  It will not overwrite.
   * @function registerSharedResources
   * @param {object} options - A collection of shared resource properties.  Each property will be saved to scope with the value.
   * @returns {object} This $scope object.
   */
  function registerSharedResources(options) {
    var foptions = Object.assign({}, options);

    for (let prop in foptions) {
      if (foptions.hasOwnProperty(prop) && typeof ($scope[prop]) === 'undefined') {
        Utils.initValue($scope, prop, foptions[prop]);
      }
    }

    return $scope;
  }
  if (!$scope.registerSharedResources) $scope.registerSharedResources = registerSharedResources;

  /**
   * Open a message dialog and display the specified message.
   * @function messageDialog
   * @param {string} title - The dialog title.
   * @param {string} message - The message on the dialog.
   * @returns {void}
   **/
  $scope.messageDialog = function (title, message) {
    if (!title) title = 'Message';
    ngDialog.open({
      template: '/content/dialogs/_Message.html',

      data: {
        title: title,
        message: message
      }
    });
  }

  /**
   * Open a confirmation dialog and ask the specified question.
   * @function confirmDialog
   * @param {string} title - The dialog title.
   * @param {string} question - The question on the dialog.
   * @returns {Promise}
   **/
  $scope.confirmDialog = function (title, question) {
    if (!title) title = 'Confirmation';
    if (!question) console.error('The confirmDialog argument "question" should be defined.');
    return ngDialog.openConfirm({
      template: '/content/dialogs/_Confirmation.html',
      data: {
        title: title,
        question: question
      }
    });
  }

  /**
   * Open the modal file uploaded dialog.
   * @function attachmentDialog
   * @param {any} title - The title of the modal window.
   * @param {any} attachment - The attachment to update/add.
   * @returns {Promise}
   */
  $scope.attachmentDialog = function (title, attachment, showAttachmentType = false) {
    if (!title)
      title = 'Attachment';

    if (!attachment) {
      attachment = Object.assign({
          Id: 0
        },
        attachment);
    }

    return ngDialog.openConfirm({
      template: '/content/dialogs/_ApplicationAttachments.html',
      data: {
        title: title,
        attachment: attachment,
        showAttachmentType: showAttachmentType
      },
      controller: function ($scope, Utils) {
        /**
         * Return the selected file in the promise.
         * @function save
         * @returns {Promise}
         **/
        $scope.save = function () {
          if ($scope.ngDialogData.attachment.File.name) {
            return $scope.confirm($scope.ngDialogData.attachment);
          } else {
            Utils.initValue($scope, 'errors.File', 'A file is required.');
          }
        };

        /**
         * Manually call the file select.
         * @function chooseFile
         * @returns {void}
         **/
        $scope.chooseFile = function () {
          var $input = angular.element('#grant-application-upload');
          $input.trigger('click');
        }

        /**
         * Set the selected file as the active attachment.
         * @function fileChanged
         * @param {any} $files
         * @returns {void}
         */
        $scope.fileChanged = function ($files) {
          if ($files.length) {
            $scope.ngDialogData.attachment.File = $files[0];
            $scope.ngDialogData.attachment.FileName = $scope.ngDialogData.attachment.File.name;
          }
        }
      }
    });
  }
});
