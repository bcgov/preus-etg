/**
 * Fires a change event when the file is selected
 **/
app.directive("ngFileSelected", function ($timeout) {
  return {
    scope: {
      ngFileSelected: '=',
    },
    link: function (scope, element, attributes) {
      element.on("change", function (changeEvent) {
        return $timeout(function () {
          scope.ngFileSelected = changeEvent.target.files[0];
        });
      });
    }
  }
});
