app.directive('ngNotEditing', function ($parse) {
  return {
    restrict: 'A',
    link: function ($scope, $element, $attrs) {
      if (!$attrs.ngNotEditing) $attrs.ngNotEditing = 'section.editing';
      const get = $parse($attrs.ngNotEditing);
      const localName = $element[0].localName;
      const type = $element[0].type;
      const id = $element[0].id;
      const value = $element[0].value;


      // Replace elements with condition to hide based on editing and model value.
      switch (localName) {
        case 'input':
          switch (type) {
            case 'radio':
            case 'checkbox':
              const label = $element.siblings('label[for=\'' + id + '\']');
              const _value = $element.attr('ng-value') || $element.attr('value') || value;
              if (label.length && !label.attr('ng-hide')) {
                const labelClone = label.clone();
                labelClone.attr('ng-hide', $attrs.ngNotEditing + ' && ' + $attrs.ngModel + ' != ' + _value);
                const replaceLabel = $compile(labelClone)($scope);
                label.replaceWith(replaceLabel);
              }

              if (!$element.attr('ng-hide')) {
                const elementClone = $element.clone();
                elementClone.attr('ng-hide', $attrs.ngNotEditing + ' && ' + $attrs.ngModel + ' != ' + _value);
                const replaceElement = $compile(elementClone)($scope);
                $element.replaceWith(replaceElement);
              }
          }
      }

      $scope.$watch($attrs.ngNotEditing, function () {
        switch (localName) {
          case ('option'):
          case ('select'):
          case ('input'):
          case ('textarea'):
            if (get($scope)) {
              $element.attr('disabled', true);
            } else {
              $element.removeAttr('disabled');
            }
            break;
          default:
            if (get($scope)) {
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
