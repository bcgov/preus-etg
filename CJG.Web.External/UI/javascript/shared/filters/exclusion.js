global.app.filter('filterExclusion', function () {
  return function (items, exclusions) {
    var out = [];
    if (!items) return out;
    for (var i = 0; i < items.length; i++) {
      var item = items[i];
      for (var exclusion in exclusions) {
        if (exclusions.hasOwnProperty(exclusion)) {
          if (!exclusions[exclusion].includes(item[exclusion])) {
            out.push(item);
            break;
          }
        }
      }
    }
    return out;
  }
});
