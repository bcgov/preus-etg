app.directive('ngValidation', function ($parse, $compile) {
  return {
    restrict: 'A',
    link: function ($scope, $element, $attrs) {
      if (!$attrs.ngValidation) $attrs.ngValidation = typeof($attrs.ngModel) === 'string' ? $attrs.ngModel.replace('model.', 'errors.') : '';
      const get = $parse($attrs.ngValidation);
      $scope.$watch($attrs.ngValidation, function () {
        if (get($scope)) {
          $element.addClass('has-error');
          if (angular.element(".has-error")[0] === $element[0]) {
            window.scrollTo(0, angular.element(".has-error")[0].offsetTop);
          }
        } else {
          $element.removeClass('has-error');
        }
      });
      var showError = typeof ($attrs.ngShowError) === 'undefined' || $attrs.ngShowError === 'true' ? true : false;
      if (showError) {
        var validation = $compile('<validation ng-model="' + $attrs.ngValidation + '"></validation>')($scope);
        validation.insertAfter($element);
      }
    }
  };
});
