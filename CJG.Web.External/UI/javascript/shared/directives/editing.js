const findOptions = /([^\s]+)( as ([^\s]+)? for ([^\s]+))? in ([^\s]+).*/;

app.directive('ngEditing', function ($parse, $compile, Utils) {
  return {
    restrict: 'A',
    link: function ($scope, $element, $attrs) {
      if (!$attrs.ngEditing) $attrs.ngEditing = 'section.editing';
      const getEditing = $parse($attrs.ngEditing);
      const getSelectedValue = $parse($attrs.ngModel);
      const localName = $element[0].localName;
      const type = $element[0].type;
      const id = $element[0].id;
      const value = $element[0].value;

      /**
       * Extract the parts of the options statement
       **/
      const getOptionParts = function () {
        const match = findOptions.exec($attrs.ngOptions);
        const itemPath = match[4] || match[1];
        const keyPath = typeof (match[1]) === 'string' ? match[1].replace(itemPath + '.', '') : null;
        const valuePath = typeof (match[3]) === 'string' ? match[3].replace(itemPath + '.', '') : null;
        const optionsPath = match && match.length >= 6 ? match[5] : null;
        return {
          itemPath: itemPath,
          keyPath: keyPath,
          valuePath: valuePath,
          optionsPath: optionsPath
        };
      }

      /**
       * Determine the text value of the selected option.
       **/
      const getSelectText = function () {
        const getSelectedText = $parse($attrs.ngModel.replace('model.', '') + '_text');
        var parts = getOptionParts();
        const options = Utils.getValue($scope, parts.optionsPath);
        const value = getSelectedValue($scope);
        if (Array.isArray(options)) {
          var item = options.find(function (o) { return Utils.getValue(o, parts.keyPath) === value; });
          getSelectedText.assign($scope, Utils.getValue(item, parts.valuePath));
        }
      }

      // Replace elements with condition to hide based on editing and model value.
      switch (localName) {
        case 'input':
          switch (type) {
            case 'radio': {
              const label = $element.siblings('label[for=\'' + id + '\']');
              const value = typeof ($attrs.ngValue) !== 'undefined' ? $parse($attrs.ngValue)($scope) : $element.attr('value');
              if (label.length && !label.attr('ng-hide')) {
                const labelClone = label.clone();
                labelClone.attr('ng-hide', '!' + $attrs.ngEditing + ' && ' + $attrs.ngModel + '!==' + value);
                var replaceLabel = $compile(labelClone)($scope);
                label.replaceWith(replaceLabel);
              }

              if (!$element.attr('ng-hide')) {
                const elementClone = $element.clone();
                elementClone.attr('ng-hide', '!' + $attrs.ngEditing + ' && ' + $attrs.ngModel + '!==' + value);
                var replaceElement = $compile(elementClone)($scope);
                $element.replaceWith(replaceElement);
              }
              break;
            }
            case 'checkbox': {
              const label = $element.siblings('label[for=\'' + id + '\']');
              //const value = typeof ($attrs.ngValue) !== 'undefined' ? $parse($attrs.ngValue)($scope) : $element.attr('value');
              if (label.length && !label.attr('ng-hide')) {
                const labelClone = label.clone();
                labelClone.attr('ng-hide', '!' + $attrs.ngEditing + ' && !' + $attrs.ngModel);
                var replaceLabel = $compile(labelClone)($scope);
                label.replaceWith(replaceLabel);
              }

              if (!$element.attr('ng-hide')) {
                const elementClone = $element.clone();
                elementClone.attr('ng-hide', '!' + $attrs.ngEditing + ' && !' + $attrs.ngModel);
                var replaceElement = $compile(elementClone)($scope);
                $element.replaceWith(replaceElement);
              }
              break;
            }
            case 'text':
            default:
              var ngCurrency = $attrs.ngCurrency === '' ? true : false;
              if (!$element.attr('ng-hide')) {
                const elementClone = $element.clone();
                elementClone.attr('ng-hide', '!' + $attrs.ngEditing);
                var replaceElement = $compile(elementClone)($scope);
                $element.replaceWith(replaceElement);
                var text = $compile('<text ng-show="!' + $attrs.ngEditing + '">{{' + $attrs.ngModel + (ngCurrency ? ' | currency' : '') + '}}</text>')($scope);
                text.insertAfter(replaceElement);
              } else {
                var text = $compile('<text ng-show="!' + $attrs.ngEditing + '">{{' + $attrs.ngModel + (ngCurrency ? ' | currency' : '') + '}}</text>')($scope);
                text.insertAfter($element);
              }
              break;
          }
          break;
        case 'textarea':
          if (!$element.attr('ng-hide')) {
            const elementClone = $element.clone();
            elementClone.attr('ng-hide', '!' + $attrs.ngEditing);
            var replaceElement = $compile(elementClone)($scope);
            $element.replaceWith(replaceElement);

            var text = $compile('<text ng-show="!' + $attrs.ngEditing + '">{{' + $attrs.ngModel + '}}</text>')($scope);
            text.insertAfter(replaceElement);
          }
          break;
        case 'select':
          if (!$element.attr('ng-hide')) {
            const elementClone = $element.clone();
            elementClone.attr('ng-hide', '!' + $attrs.ngEditing);
            var replaceElement = $compile(elementClone)($scope);
            $element.replaceWith(replaceElement);

            var text = $compile('<text ng-show="!' + $attrs.ngEditing + '">{{' + $attrs.ngModel.replace('model.', '') + '_text}}</text>')($scope);
            text.insertAfter(replaceElement);
          }

          var parts = getOptionParts();
          $scope.$watch(parts.optionsPath, function () {
            getSelectText();
          });

          $scope.$watch($attrs.ngModel, function () {
            getSelectText();
          });
          break;
      }

      $scope.$watch($attrs.ngEditing, function () {
        var editing = getEditing($scope);
        switch (localName) {
          case ('select'):
            if (editing && !$element.attr('multiple')) {
              $element.parent().addClass('selectmenu');
            } else {
              $element.parent().removeClass('selectmenu');
            }
          case ('option'):
          case ('input'):
          case ('textarea'):
            if (editing) {
              $element.removeAttr('disabled');
            } else {
              $element.attr('disabled', true);
            }
            break;
          default:
            if (editing) {
              $element.removeClass('ng-hide');
            } else {
              $element.addClass('ng-hide');
            }
            break;
        }
      })
    }
  }
});
