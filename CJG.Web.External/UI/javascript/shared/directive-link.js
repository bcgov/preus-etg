function directiveLink(scope, ele, attr, ctrl, parser, formatter) {
  ctrl.$parsers.unshift(parser);
  ctrl.$formatters.unshift(formatter);
  ele.on("blur", function () {
    ctrl.$commitViewValue();
    reformatViewValue(ctrl);
  });
}

function reformatViewValue(ctrl) {
  var formatters = ctrl.$formatters,
    idx = formatters.length;

  var viewValue = ctrl.$$rawModelValue;
  while (idx--) {
    viewValue = formatters[idx](viewValue);
  }

  ctrl.$setViewValue(viewValue);
  ctrl.$render();
}

export default directiveLink;
