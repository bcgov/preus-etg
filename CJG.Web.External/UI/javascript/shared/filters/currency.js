app.filter('currencyParentheses', function ($filter) {
  return function (amount, symbol, fractionSize) {
    var value = $filter('currency')(amount, symbol, fractionSize)
    if (value < 0) {
      return value.replace("-", "(") + ')';
    }
    return value;
  };
});
