app.factory('Utils', function ($http, $sce, $loadingOverlay, $timeout) {
  const loadingQueue = [];

  /**
   * Checks if the value is a function.  If it is, it will be called to return a value.
   * @function action
   * @param {any} value - The value to test if it's a function and then call.
   * @param {any} [...data] - The arguments you want to pass to the function.
   * @returns {any} - The value returned by the function, or the value it self.
   */
  function action(value) {
    var args = Array.prototype.slice.call(arguments).slice(1);
    return typeof (value) === 'function' ? value.apply(this, args) : value;
  }

  /**
   * Clean the source object by deleting the specified properties.
   * @function clean
   * @param {object} source - The source object.
   * @param {object} options - The configuration options.
   * @param {array} [options.delete] - An array of properties to delete.
   * @returns {object} The mutated source.
   */
  function clean(source, options) {
    var foptions = Object.assign({ delete: [] }, options);
    if (typeof (source) === 'undefined') return;

    for (var prop in source) {
      if (source.hasOwnProperty(prop)) {
        if (foptions.delete.indexOf(prop) >= 0) delete source[prop];
        else if (Array.isArray(source[prop])) source[prop].forEach(function (item) { clean(item, foptions); });
        else if (typeof (source[prop]) === 'object') clean(source[prop], foptions);
      }
    }

    return source;
  }

  /**
   * Convert the object into a Blob with a type of 'application/json'.
   * @function toBlob
   * @param {object} data - The object to convert.
   * @returns {Blob} A new Blob object containing your object.
   */
  function toBlob(data) {
    var json = JSON.stringify(data);
    return new Blob([json], {
      type: 'application/json'
    });
  }

  /**
   * Make an AJAX request with the specified options.
   * @function ajax
   * @param {any} options - Configuration options for making the AJAX request
   * @param {string|function} options.url - The URL to the resource, or function to generate one.
   * @param {string} [options.method='GET'] - The HTTP method to use [GET|POST|PUT|DELETE|HEAD|OPTIONS] (default: GET).
   * @param {any} [options.data] - The data to send in the AJAX request, or a function to retreive the data.
   * @param {object} [options.headers] - An object containing HTTP headers to include in the request (default includes; RequestVerificationToken, X-Requested-With, Cache-Control, Pragma, If-Modified-Since).
   * @param {string} [options.contentType] - The Content-Type header value.  This will override the 'dataType' parameter.
   * @param {string} [options.dataType] - The Content-Type being included [json|form|text|html|xml].
   * @param {bool} [options.backup=false] - Whether to backup the model (default: false).
   * @param {callback} [options.success] - A callback if the request was successful.
   * @param {bool|callback} [options.error=true] - Whether to throw the error, or make a callback (default: true). Set this to true if you want to handle the error, or don't want your 'then()' statements to be called.
   * @param {bool} [options.withCredentials=false] - Wether to include session credentials.
   * @param {function} [options.transformRequest] - A function to transform the request.
   * @param {function} [options.transformResponse] - A function to transform the response.
   * @param {bool} [options.ajaxCache=true] - Whether to use AJAX caching.
   * @returns {Promise} A promise containing the response.
   */
  function ajax(options) {
    if (!options.url) throw new Error('Argument "options.url" is required.');

    var foptions = Object.assign({
      method: 'GET',
      backup: false,
      url: url,
      data: data,
      error: true,
      headers: {
        'RequestVerificationToken': $("input[name='__AjaxRequestVerificationToken']").val(),
        'X-Requested-With': 'XMLHttpRequest'
      },
      withCredentials: false,
      transformRequest: $http.defaults.transformRequest,
      transformResponse: $http.defaults.transformResponse,
      ajaxCache: true,
      timeout: 2 * 60 * 1000
    }, options);

    // Control AJAX caching.
    if (foptions.ajaxCache) {
      foptions.headers = Object.assign({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
        'If-Modified-Since': '0'
      }, foptions.headers);
    }

    try {
      var data = clean(action(foptions.data), { delete: ['ValidationErrors', 'HasError', '$$hashKey'] });
      var url = action(foptions.url);
      var method = action(foptions.method);

      switch (foptions.dataType) {
        case 'text':
          foptions.header['Content-Type'] = 'text/plain';
          if (typeof (data) === 'object') data = angular.toJson(data);
          break;
        case 'html':
          foptions.headers['Content-Type'] = 'text/html';
          if (typeof (data) === 'object') data = angular.toJson(data);
          break;
        case 'xml':
          foptions.headers['Content-Type'] = 'application/xml';
          break;
        case 'form':
          foptions.headers['Content-Type'] = 'application/x-www-form-urlencoded; charset=utf-8';
          var _data = data;
          var data = new FormData();
          if (Array.isArray(_data)) {
            data.append('value', item);
          } else if (typeof (_data) === 'object') {
            for (let prop in _data) {
              if (_data.hasOwnProperty(prop)) {
                if (Array.isArray(_data[prop])) {
                  _data[prop].forEach(function (item) { data.append(prop, item); });
                } else {
                  data.append(prop, _data[prop]);
                }
              }
            }
          } else {
            data.append('values', _data);
          }
          break;
        case 'file':
          foptions.transformRequest = angular.identity;
          foptions.headers['Content-Type'] = undefined;
          var _data = data;
          var data = new FormData();
          if (Array.isArray(_data)) {
            _data.forEach(function (item) {
              data.append('files', item);
            });
          } else if (typeof (_data) === 'object') {
            for (let prop in _data) {
              if (_data.hasOwnProperty(prop) && typeof (_data[prop]) !== 'undefined' && _data[prop] !== null) {
                if (Array.isArray(_data[prop])) {
                  _data[prop].forEach(function (item) { data.append(prop, item); });
                } else if (_data[prop] instanceof File) {
                  data.append(prop, _data[prop]);
                } else if (typeof (_data[prop]) === 'object') {
                  data.append(prop, _data[prop]);
                } else {
                  data.append(prop, _data[prop]);
                }
              }
            }
          } else {
            data.append('file', _data);
          }
          break;
        case 'json':
        default:
          foptions.headers['Content-Type'] = 'application/json';
          if (typeof (data) === 'object') data = angular.toJson(data);
          break;
      }
      if (foptions.contentType) foptions.headers['Content-Type'] = foptions.contentType;

      return $timeout(function () {
        pushToQueue(url);
      })
        .then(function () {
          return new Promise(function (resolve, reject) {
            $http({
              withCredentials: foptions.withCredentials,
              method: method,
              url: url,
              data: data,
              headers: foptions.headers,
              transformRequest: foptions.transformRequest,
              transformResponse: foptions.transformResponse,
              timeout: foptions.timeout
            })
              .then(function (response) {
                console.log('request successful - ' + url);
                convertDateStringsToDates(response.data);
                action(foptions.success, response);
                resolve(response);
              })
              .catch(function (response) {
                console.log('request failed - ' + url);
                // By default 'error=true', which will return a rejected promise.  If 'error=function' then it's up to that function whether the promise will be rejected.
                if (foptions.error === true) reject(response);
                else return action(foptions.error, response);
              })
              .finally(function () {
                popOutQueue();
              });
          });
        })
    } catch (ex) {
      return Promise.reject(ex);
    }
  }

  /**
   * Show the loading overlay.
   * Add request to queue.
   * @function pushToQueue
   * @param {any} url
   * @returns {void}
   */
  function pushToQueue(url) {
    if (loadingQueue && loadingQueue.length === 0) {
      $loadingOverlay.show();
    }
    loadingQueue.push(url);
  };

  /**
   * Hide the loading overlay.
   * Remove the request from the queue.
   * @function popOutQueue
   * @returns {void}
   */
  function popOutQueue() {
    loadingQueue.pop();
    if (loadingQueue && loadingQueue.length === 0) {
      $loadingOverlay.hide();
    }
  };

  /**
   * Set the value of the specified path.
   * This will initialize missing segments of the path.
   * //// EXAMPLE
   * initValue($scope, 'some.path.to.var', value);
   * $scope.some.path.to.var = value;
   * @function initValue
   * @param {object} root - The object that will be updated with the value.
   * @param {string} path - The path on the root that will be set with the value (i.e. 'some.path.value').
   * @param {any} value - The value that will be set on the root path.
   * @returns {any} The variable path value.
   */
  function initValue(root, path, value) {
    if (!root) throw new Error('Argument "root" is required.');
    if (!path || typeof (path) !== 'string') return;
    var segments = path.split('.');

    if (segments.length == 1) {
      root[path] = value;
      return root[path];
    } else {
      if (!root[segments[0]]) root[segments[0]] = {};
      return initValue(root[segments[0]], segments.slice(1).join('.'), value);
    }
  }

  /**
   * Get the value for the specified path expression.
   * @function getValue
   * @param {any} root - The root of the path.
   * @param {any} path - The string path to the value being requested.
   * @returns {any} The value of the path, or 'undefined' if it doesn't exist.
   */
  function getValue(root, path) {
    if (!root) return;
    if (!path) return;
    if (typeof (path) !== 'string') path = String(path);

    var i = path.indexOf('.');
    if (i < 0) return root[path];
    var key = path.substring(0, i);
    if (typeof (root[key]) === 'undefined') return;
    return getValue(root[key], path.substring(i + 1));
  }

  /**
   * Verify the value has a matching value in the specified 'test'.
   * If the specified args are equal it will return true.
   * If the test is an array all values in the array will be verified to find a match.
   * If the test is an object the property with the value name will be determined if it exists.
   * If the test is a function it will be called and passed the value for verification.
   * If the test is a comma-separated string it will split and verify all values.
   * If the test is a regex expression it will be test the expression with the value.
   * @function verifyMatch
   * @param {any} value - The value to look for.
   * @param {any} test - The value to compare with.
   * @returns {bool} True if the value matches the test, otherwise false.
   */
  function verifyMatch(value, test) {
    if (test === value) return true;

    if (Array.isArray(test)) {
      for (let i = 0; i < test.length; i++) {
        if (test[i] instanceof RegExp && test[i].test(value)) return true;
        else if (test[i] === value) return true;
      }
    } else if (typeof (test) === 'string') {
      var segments = test.split(',').map(function (item) {
        return item.trim();
      });
      return verifyMatch(value, segments);
    } else if (test instanceof RegExp) {
      return test.test(value);
    } else if (typeof (test) === 'object') {
      return typeof (test[value]) !== 'undefined';
    } else if (typeof (test) === 'function') {
      return test(value);
    }

    return false;
  }

  /**
   * Copy the source object into the destination object, but only where the destination matches.
   * If you provide a property that has a value of '__delete', it will delete the destination property with the same name.
   * @function copyWhere
   * @param {any} source - The source to copy into the destination.
   * @param {any} dest - The destination that will be updated.
   * @param {object} [options] - The configuration options.
   * @param {bool|function} [options.match] - Whether the properties need to match in destination to be copied {callback: match(source, dest, prop)}
   * @param {bool} [options.match.name=true] - Whether the properties need to match the destination name to be copied (default: true).
   * @param {bool} [options.match.type=false] - Whether the properties need to match the destination type to be copied (default: false).
   * @param {any} [options.ignore] - A way to list properties that should not be copied.
   * @returns {void}
   */
  function copyWhere(source, dest, options) {
    var foptions = Object.assign({ match: { name: true, type: false, ignore: false } }, options);

    if (typeof (source) !== 'object' && typeof (dest) !== 'object') {
      dest = source;
      return;
    }

    // Need this if the original source is an array.
    if (Array.isArray(source)) {
      if (Array.isArray(dest)) {
        if (source.length) {
          if (source.length < dest.length) dest.splice(source.length - 1);
          source.forEach(function (item, index) {
            if (dest.length > index) copyWhere(item, dest[index], foptions);
            else dest.push(item); // Add the new item.
          });
        } else dest.length = 0;
      } else dest = source; // Probably should copy an array into an unknown dest.
    }

    for (let prop in source) {
      if (source.hasOwnProperty(prop)
        && (!foptions.ignore || !verifyMatch(prop, foptions.ignore)) // Only pass if we not ignoring this property.
        && (
          action(foptions.match, source, dest, prop) // Fire callback to determine.
          || (
            (!foptions.match.name || dest.hasOwnProperty(prop)) // Property names must match.
            && (!foptions.match.type || source[prop] === '__delete' || typeof (source[prop]) === typeof (dest[prop])) // Property types must match.
          )
        )) {
        if (source[prop] === '__delete') delete dest[prop];
        else if (source[prop] && typeof (source[prop]) === 'object' && !Array.isArray(source[prop]) && !objectIsEmpty(dest[prop])) copyWhere(source[prop], dest[prop], foptions); // Recursive copy.
        else if (Array.isArray(source[prop]) && Array.isArray(dest[prop])) {
          if (source[prop].length) {
            if (source[prop].length < dest[prop].length) dest[prop].splice(source[prop].length - 1); // Remove any extra items.

            source[prop].forEach(function (item, index) {
              if (dest[prop].length > index) copyWhere(item, dest[prop][index], foptions); // Recursive copy.
              else dest[prop].push(item); // Just add the extra to the array.
            });
          } else dest[prop].length = 0;
        }
        else dest[prop] = source[prop]; // Copy the property.
      }
    }
  }

  /**
   * Sync the destination and source values.
   * Only update where there is a change.
   * This mutates the dest.
   * @function sync
   * @param {any} source - The source data you want to copy from.
   * @param {any} dest - The destination to copy the source into.
   * @returns {void}
   **/
  function sync(source, dest) {
    copyWhere(source, dest, {
      match: function (source, dest, prop) {
        if (typeof (source[prop]) === 'undefined') return true;
        else if (Array.isArray(dest[prop]) && (Array.isArray(dest[prop]) || typeof (dest[prop]) === 'undefined')) return true;
        return source[prop] !== dest[prop]; // Only copy if the value has changed.
      }
    });
  }

  /**
   * Check if the object is empty (i.e. {}).
   * @function objectIsEmpty
   * @param {any} obj - The object test.
   * @returns {boolean} Whether the object is empty (true).
   */
  function objectIsEmpty(obj) {
    for (let prop in obj) {
      if (obj.hasOwnProperty(prop)) return false;
    }
    return true;
  }

  /**
   * Handle AJAX response errors.
   * Calls setAlert if there is a generic error message, or a summary error.
   * Copys 'response.data.ValidationErrors' to $scope.errors[key].
   * @function handleAjaxFailure
   * @param {any} options - Configuration options to handle AJAX errors.
   * @param {object} options.$scope - Angular $scope.
   * @param {object} [options.$sce] - Angular $sce.
   * @param {object} options.response - AJAX response.
   * @param {object} [options.copy] - Configuration options on how to copy errors to $scope.
   * @param {string} [options.copy.path] - The path to copy the error to in $scope (default 'model.').
   * @param {function} [options.copy.format] - The name formater (default: replace path '.' with '_' and appends 'ErrorMessage').
   * @param {bool|string} [options.clear] - Whether to clear errors and alerts.  By default it will clear when a request is a 'POST', 'PUT' or 'DELETE'.
   * @returns {void}
   */
  function handleAjaxFailure(options) {
    if (!options.$scope) throw new Error("Argument 'options.$scope' required.");
    var $scope = options.$scope;
    var $sce = options.$sce;
    var foptions = Object.assign({
      response: {},
      copy: {
        path: 'model.',
        format: function (key) {
          return key.split('.').join('_') + 'ErrorMessage';
        }
      }
    }, options);

    if (typeof (foptions.clear) === 'undefined') foptions.clear = 'POST|PUT|DELETE';
    if (typeof ($scope.errors) === 'undefined') $scope.errors = {};
    if (foptions.response.config && foptions.clear.includes(foptions.response.config.method)) $scope.errors = {};

    var message = '';
    if (foptions.response instanceof Error) {
      message = foptions.response.message;
    } else if (typeof (foptions.response) === 'object') {
      var errors = foptions.response.data ? foptions.response.data.ValidationErrors || [] : [];
      if (errors && errors.length > 0) {
        for (let i = 0; i < errors.length; i++) {
          var error = errors[i];
          if (error.Key === 'Summary') {
            setAlert({ response: foptions.response, message: error.Value, $scope: $scope, $sce: $sce });
          } else if (error.Key === 'ExessiveRequests') {
            setAlert({ response: foptions.response, message: error.Value, $scope: $scope, $sce: $sce });
          } else if (error.Key && error.Key.length) {
            var path = foptions.copy.path + foptions.copy.format(error.Key);
            initValue($scope, path, error.Value);
          } else {
            message += '<p>' + error.Value + '</p>';
          }
        }
        return; // No need to process further, error messages should bind.
      } else {
        message = foptions.response.statusText;
      }
    } else if (typeof (foptions.response) === 'string') {
      message = foptions.response;
    } else {
      message = 'An unexpected error has occured.';
    }

    if (message.length) {
      console.log('error - ' + message);
      setAlert({ response: foptions.response, message: message, $scope: $scope, $sce: $sce });
    }
  }

  /**
  * Update the alert message in the scope.
  * @function setAlert
  * @param {object} options - Configuration options for the alert.
  * @param {object} options.$scope - Angular scope.
  * @param {object} [options.response] - The AJAX response or error details.
  * @param {string} [options.message] - The alert message to display.
  * @param {object} [options.$sce] - Angular service provider.
  * @param {bool} [options.clear=false] - Whether to clear the alert before adding to it (default: false).
  * @returns {void}
  */
  function setAlert(options) {
    if (!options.$scope) throw new Error("Argument 'options.$scope' required.");
    var $scope = options.$scope;
    var foptions = Object.assign({ clear: false }, options);

    if (typeof ($scope.alert) === 'undefined') $scope.alert = { message: '' };

    if (foptions.clear || !$scope.alert.message) $scope.alert.message = '';
    $scope.alert.message += '<p>' + (foptions.$sce ? foptions.$sce.trustAsHtml(foptions.message) : foptions.message) + '</p>';

    if (typeof (foptions.response) === 'object') {
      switch (foptions.response.status) {
        case 200:
        case 201:
          $scope.alert.type = 'success';
          break;
        case 204:
        case 400:
        case 409:
        case 500:
        default:
          $scope.alert.type = 'error';
          break;
      }
    } else {
      $scope.alert.type = 'error';
    }
    window.scrollTo(0, 0);
  }

  /**
   * Clear the summary messages.
   * @function clearAlert
   * @param {object} $scope - Angular $scope.
   * @returns {void}
   */
  function clearAlert($scope) {
    if ($scope.$parent && typeof ($scope.$parent.alert) !== 'undefined') $scope.$parent.alert.message = null;
    if (typeof ($scope.alert) !== 'undefined') $scope.alert.message = null;
  }

  /**
   * Clear the error messages.
   * @function clearErrors
   * @param {any} $scope
   * @returns {void}
   */
  function clearErrors($scope) {
    $scope.errors = {};
  }

  /**
   * Parses the object and converts all date strings into data objects.
   * @param {any} obj - The object to parse.
   * @returns {void}
   */
  function convertDateStringsToDates(obj) {
    if (typeof (obj) === 'undefined') return;

    if (typeof obj !== "object") {
      if (obj != null && jQuery.type(obj) == "string" && obj.match(global.regexIso8601)) {
        return new Date(parseInt(obj.substr(6)));
      }
      return obj;
    }

    for (var key in obj) {
      if (!obj.hasOwnProperty(key)) continue;

      var value = obj[key];
      var match;
      // Check for string properties which look like dates.
      if (jQuery.type(value) == "string" && value.match(global.regexIso8601)) {
        obj[key] = new Date(parseInt(value.substr(6)));
      } else if (jQuery.type(value) == "object") {
        // Recurse into object
        convertDateStringsToDates(value);
      } else if (jQuery.type(value) == "array") {
        // Recurse into object
        for (var i = 0; i < value.length; i++) {
          convertDateStringsToDates(value[i]);
        }
      }
    }
  }

  /**
   * Copy the attributes into the specified destination element (excluding the specified attributes).
   * @function copyAttributes
   * @param {object} $attrs - Angular $attrs object.
   * @param {object} $dest - HTML element to copy the attributes to.
   * @param {Array} [exclude] - A list of attributes that should not be copied.
   * @returns {void}
   */
  function copyAttributes($attrs, $dest, exclude) {
    if (typeof ($attrs) !== 'object') throw new Error('Argument "$attrs" is required.');
    if (typeof ($dest) !== 'object') throw new Error('Argument "$dest" is required.');

    angular.forEach($attrs, function (value, key) {
      if (key[0] === '$' || key.startsWith('ng') || (exclude && exclude.includes(key))) return;
      switch (key) {
        case 'class':
        case 'style':
          $dest.attr(key, $dest.attr(key) + ' ' + value);
          break;
        default:
          $dest.attr(key, value);
          break;
      }
    });
  }

  /**
   * Add the specified attributes into the specified destination element.
   * @function addAttributes
   * @param {object} $attrs - Angular $attrs object.
   * @param {object} $dest - HTML element to copy the attributes to.
   * @param {Array} include - A list of attributes that will be moved.
   * @returns {void}
   */
  function addAttributes($attrs, $dest, include) {
    if (typeof ($attrs) !== 'object') throw new Error('Argument "$attrs" is required.');
    if (typeof ($dest) !== 'object') throw new Error('Argument "$dest" is required.');
    if (typeof (include) === 'undefined' || !Array.isArray(include)) throw new Error('Argument "include" is required.');

    angular.forEach($attrs, function (value, key) {
      if (!include.includes(key)) return;
      if (key.startsWith('ng')) key = 'ng-' + key.substring(2);
      $dest.attr(key, value);
    });
  }

  /**
   * Get the actual name of a variable expression.
   * @function nameof
   * @param {any} exp - An expression (i.e. window.path.path.value ).
   * @returns {string} The name of the expression (i.e. window.path.path.value ).
   */
  function nameof(exp) {
    return exp.toString().match(/(?=[^.]*$)(\w+)/g)[0];
  }

  /**
   * Determine if the value passed is a Date object with a valid date.
   * @function isDate
   * @param {any} value - The value to check
   * @returns {bool} Whether it is a valid date.
   */
  function isDate(value) {
    if (Object.prototype.toString.call(value) === '[object Date]') {
      if (isNaN(value.getTime())) return false;
      else return true;
    } else return false;
  }

  /**
   * Replace all found occurances with the specified replace value.
   * @function replaceAll
   * @param {any} text - The text to search.
   * @param {any} search - This is a regular expression.
   * @param {any} replace - The text to replace with.
   * @returns {string} The updated new value.
   */
  function replaceAll(text, search, replace) {
    if (typeof (text) !== 'string') return text;
    return text.replace(new RegExp(search, 'g'), replace);
  }

  /**
   * Moves the specified array item to the new position.
   * @function moveItem
   * @param {Array} arr - The array to manipulate.
   * @param {int} old_index - The original index position.
   * @param {int} new_index - The new index position the item will be moved to.
   */
  function moveItem(arr, old_index, new_index) {
    if (new_index >= arr.length) {
      var k = new_index - arr.length + 1;
      while (k--) {
        arr.push(undefined);
      }
    }
    arr.splice(new_index, 0, arr.splice(old_index, 1)[0]);
  };

  /**
   * Creates and configures an object containing pager information.
   * This will provide a simple way to show/hide/enable/disable previous and next buttons.
   * This will provide a simple way to list available pages.
   * This will provide a simple way to tally the number of items on each page.
   * @function setupPager
   * @param {int} page - The current page numnber.
   * @param {int} quantity - The number of items on the page.
   * @param {int} total - The total number of items in the collection.
   * @param {Array} [items] - The items on the page.
   * @param {object} [options] - Configuration options for the pager.
   * @param {int} [options.pages] - The number of pages to display in the pager.
   * @returns {object}
   */
  function setupPager(page, quantity, total, items, options) {
    if (!page) page = 1;
    if (!quantity) quantity = 10;
    if (!total) total = 0;
    var foptions = Object.assign({ pages: 5 }, options);

    var enablePrevious = page > 1;
    var pageCount = Math.ceil(total / quantity);
    var enableNext = page < pageCount;
    var pages = [];
    var halfPages = Math.floor(foptions.pages / 2); // The number of pages to show before the current page.
    var start = page - halfPages > 0 ? page - halfPages : 1;
    var end = start + foptions.pages - 1;
    for (let i = start; i <= end && i <= pageCount; i++) {
      pages.push(i);
    }

    var showFirstPage = !pages.length || pages[0] === 1 ? 0 : 1;
    var showLastPage = pages[pages.length - 1] === pageCount ? 0 : pageCount;

    return {
      enablePrevious: enablePrevious,
      enableNext: enableNext,
      page: page,
      quantity: quantity,
      pages: pages,
      pageCount: pageCount,
      showFirstPage: showFirstPage,
      showLastPage: showLastPage,
      items: {
        first: (page - 1) * quantity + 1,
        last: Array.isArray(items) ? (page - 1) * quantity + items.length : page * quantity,
        total: total
      }
    }
  }

  /**
   * Check if localStorage is available.
   * @function localStorageEnabled
   * @returns {bool}
   **/
  function localStorageEnabled() {
    var test = 'test';
    try {
      localStorage.setItem(test, test);
      localStorage.removeItem(test);
      return true;
    } catch (e) {
      return false;
    }
  }

  /**
   * Delay calling the action.
   * @function delay
   * @param {function} action - The action to perform, which must return a promise.
   * @param {number} time - Number of milliseconds.
   * @returns {Promise}
   */
  function delay(action, time) {
    if (!time) time = 2000;

    return new Promise(function (resolve, reject) {
      setTimeout(function () {
        action()
          .then(function (response) {
            resolve(response);
          })
          .catch(function (error) {
            reject(error);
          });
      }, time);
    });
  }

  /**
   * Converts the date to PST timezone.
   * @function toPST
   * @param {any} date - A date value to convert.
   * @param {string='YYYY-MM-DD'} format - The format to return the date (default)
   * @return {string} The converted and formatted date value.
   */
  function toPST(date, format) {
    if (typeof (date) === 'undefined')
      return null;

    if (typeof (date) === 'string') {
      try {
        date = new Date(date);
      } catch (error) {
        return date; // Ignore error and return the string as it was.
      }
    }

    if (typeof (format) === 'undefined')
      format = 'YYYY-MM-DD';

    return moment(date).tz('America/Vancouver').format(format);
  }

  return {
    ajax: ajax,
    action: action,
    pushToQueue: pushToQueue,
    popOutQueue: popOutQueue,
    initValue: initValue,
    getValue: getValue,
    verifyMatch: verifyMatch,
    copyWhere: copyWhere,
    sync: sync,
    objectIsEmpty: objectIsEmpty,
    handleAjaxFailure: handleAjaxFailure,
    setAlert: setAlert,
    clearAlert: clearAlert,
    clearErrors: clearErrors,
    convertDateStringsToDates: convertDateStringsToDates,
    copyAttributes: copyAttributes,
    addAttributes: addAttributes,
    nameof: nameof,
    isDate: isDate,
    replaceAll: replaceAll,
    toBlob: toBlob,
    clean: clean,
    moveItem: moveItem,
    setupPager: setupPager,
    localStorageEnabled: localStorageEnabled,
    delay: delay,
    toPST: toPST
  };
});
