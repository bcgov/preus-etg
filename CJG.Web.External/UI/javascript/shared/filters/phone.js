global.app.filter('phone', function () {
  return function (phone) {
    if (typeof (phone) !== 'string' || phone == null)
      return '';
    return phone.replace(/[^0-9]/g, '');
  }
});
