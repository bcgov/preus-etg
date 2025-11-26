/**
 * Fires a change event when the file is uploaded
 **/
app.directive("ngFileRead", function ($timeout) {
  return {
    scope: {
      ngFileRead: "="
    },
    link: function (scope, element, attributes) {
      element.on("change", function (changeEvent) {
        var reader = new FileReader();
        reader.onload = function (loadEvent) {
          return $timeout(function () {
            scope.ngFileRead = loadEvent.target.result;
          });
        }
        reader.readAsDataURL(changeEvent.target.files[0]);
      });
    }
  }
});
