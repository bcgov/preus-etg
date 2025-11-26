/**
 * Fires a change event when many files are selected
 **/
app.directive("ngFilesSelected", function ($timeout) {
  return {
    scope: {
      ngFilesSelected: "="
    },
    link: function (scope, element, attributes) {
      element.on("change", function (changeEvent) {
        return $timeout(function () {
          scope.ngFilesSelected = changeEvent.target.files;
        });
      });
    }
  }
});
