app.directive('ngSelected', function ($parse, Utils) {
  return {
    restrict: 'A',
    controller: function ($scope, $element) {

    },
    link: function ($scope, $element, $attrs) {
      if (!$attrs.ngSelected) $attrs.ngSelected = 'section.editing';
      var selected = $parse($attrs.ngSelected)($scope);
      const getModel = $parse($attrs.ngModel);
      const getValue = $parse($attrs.ngValue);
      const localName = $element[0].localName;
      const type = $element[0].type;

      if (!Array.isArray(selected)) {
        selected = Utils.initValue($scope, $attrs.ngSelected, []);
      }

      /**
       * Change which values are selected in the specified model.
       * @function change
       * @param {any} event - The event that was fired.
       * @returns {void}
       */
      const change = function (event) {
        var selected = $parse($attrs.ngSelected)($scope);
        const checked = event.target.checked;
        const value = getValue($scope);

        const index = selected.indexOf(value);
        if (checked && index === -1) {
          selected.push(value);
        } else if (!checked && index >= 0) {
          selected.splice(index, 1);
        }

      }

      const unwatchSelected = $scope.$watchCollection($attrs.ngSelected, function (newValue, oldValue) {
        if (newValue && Array.isArray(newValue)) {
          const value = getValue($scope);
          getModel.assign($scope, newValue.indexOf(value) >= 0)
        }
      });

      // Replace elements with condition to hide based on editing and model value.
      switch (localName) {
        case 'input':
          switch (type) {
            case 'radio':
            case 'checkbox':
              $element.on('click', change);
          }
      }

    }

  }
});
