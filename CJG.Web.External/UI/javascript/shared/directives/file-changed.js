/**
 * Fires a change event when the file is selected
 **/
app.directive("ngFileChanged", function ($timeout) {
  return {
    scope: {
      ngFileChanged: '&',
    },
    link: function (scope, element, attributes) {
      element.bind("change", function (changeEvent) {
        return $timeout(function() {
          scope.ngFileChanged({ $files: changeEvent.target.files });
        });
      });
    }
  }
});
