app.filter('maintainLineBreaks', function() {
  return function (value) {
    if (value == undefined || value == '')
      return '';

    return value.replaceAll("\n", "<br/>");
  };
});
