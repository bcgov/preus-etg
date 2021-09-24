/**
 * Creates a date dropdown input.
 */
app.component('datedropdown', {
  require: {
    ngModelCtrl: 'ngModel'
  },
  bindings: {
    ngModel: '=',
    ngYear: '=?',
    ngMonth: '=?',
    ngDay: '=?',
    ngChange: '&?',
    error: '@?',
    ngError: '<?',
    label: '@?',
    ngLabel: '<?',
    ngDisabled: '<?',
    ngEditing: '<?',
    ngRequired: '<?',
    name: '@?',
    ngText: '<?',
    ngMinDate: '<?',
    ngMaxDate: '<?',
    ngDefaultMonth: '<?',
    ngDefaultDay: '<?',
    ngDefaultYear: '<?'
  },
  templateUrl: '/content/components/_AngularDateDropdown.html',
  controller: function ($scope, $element, $attrs, $filter, $timeout, Utils, moment) {
    var $ctrl = this;
    var $input = $element.find('select');
    var monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']

    this.$onInit = function () {
      if (typeof ($attrs.name) === 'undefined' && typeof ($attrs.ngModel) !== 'undefined') $ctrl.name = Utils.replaceAll($attrs.ngModel, /\./, '-');
      if (!$ctrl.ngMinDate) $ctrl.ngMinDate = new Date();
      if (!$ctrl.ngMaxDate) $ctrl.ngMaxDate = new Date($ctrl.ngMinDate.getFullYear() + 1, $ctrl.ngMinDate.getMonth(), $ctrl.ngMinDate.getDay());
      if (!$ctrl.ngTextFormat) $ctrl.ngTextFormat = 'YYYY-MM-DD';
      initDate();
    }

    this.$postLink = function () {
      Utils.copyAttributes($attrs, $input, ['default', 'label', 'error']);
    }

    this.getError = function () {
      var path = $attrs.ngModel;
      if (!path) {
        return;
      }
      if (path.startsWith('model.')) path = path.replace('model.', '');
      return $ctrl.error || $ctrl.ngError || Utils.getValue($scope.$parent.errors, path);
    }

    this.editing = function () {
      return $ctrl.ngEditing || $scope.$parent.section.editing;
    }

    function getTime(date) {
      if (date.getTime) return date.getTime();
      else if (date.valueOf) return date.valueOf();
    }

    $scope.$watch('$ctrl.ngModel', function (newValue, oldValue) {
      if (typeof (newValue) !== 'undefined' && newValue !== null) {
        if (typeof (oldValue) === 'undefined'
          || oldValue === null
          || ((typeof (oldValue) !== 'undefined' && oldValue !== null)
            && (getTime(newValue) !== getTime(oldValue))
            && ($ctrl.date && getTime(newValue) !== getTime($ctrl.date)))) {
          initDate();
        }
      }
    });

    $scope.$watch('$ctrl.ngMinDate', function (newValue, oldValue) {
      if (newValue != oldValue) {
        $ctrl.years = genYears();
      }
    });

    $scope.$watch('$ctrl.ngMaxDate', function (newValue, oldValue) {
      if (newValue != oldValue) {
        $ctrl.years = genYears();
      }
    });

    $scope.setDateToToday = function ($event) {
      var select = angular.element($event.currentTarget).find('select');
      if ($event.ctrlKey && select && !select[0].getAttribute('disabled')) {
        var date = new Date();
        $ctrl.year = date.getFullYear();
        $ctrl.month = date.getMonth();
        $ctrl.days = genDays($ctrl.year, $ctrl.month);
        $ctrl.day = date.getDate();
        setDate();
      }
    }

    $scope.toPST = Utils.toPST;

    function initDate() {
      // initialize the available dates.
      $ctrl.years = genYears();
      $ctrl.months = genMonths();
      $ctrl.days = genDays(getYear(), getMonth());
      $ctrl.year = $ctrl.ngYear || getYear();
      $ctrl.month = typeof($ctrl.ngMonth) !== 'undefined' && $ctrl.ngMonth > 0 ? $ctrl.ngMonth - 1 : getMonth(); // The dropdown is zero-indexed, but we expect a valid month number in ngMonth.
      $ctrl.day = $ctrl.ngDay || getDay();
      $ctrl.date = getDate();
    }

    function getDate() {
      try {
        if (!$ctrl.ngModel) return null;
        return moment($ctrl.ngModel).tz('America/Vancouver');
      } catch (e) {
        return null;
      }
    }

    function getYear() {
      var date = getDate();
      if (date) return date.year();
      return date;
    }

    function getMonth() {
      var date = getDate();
      if (date) return date.month();
      return date;
    }

    function getDay() {
      var date = getDate();
      if (date) return date.date();
      return date;
    }

    function genYears() {
      var years = [];
      var start = $ctrl.ngMinDate ? $ctrl.ngMinDate.getFullYear() : new Date().getFullYear();
      var end = $ctrl.ngMaxDate ? $ctrl.ngMaxDate.getFullYear() : new Date().getFullYear();
      for (let i = start; i <= end; i++) {
        years.push(i);
      }
      return years;
    }

    function genMonths() {
      var months = [];
      for (let i = 0; i < 12; i++) {
        months.push({ key: i, value: monthNames[i] });
      }
      return months;
    }

    function genDays(year, month) {
      days = [];
      var today = moment();
      var gYear = year || today.year();
      var gMonth = (typeof (month) === 'undefined' || month == null ? today.month() : month);
      var max = moment([gYear, gMonth, 1]).daysInMonth();
      for (let i = 1; i <= max; i++) {
        days.push(i);
      }
      return days;
    }

    function setDate() {
      if ($ctrl.year == null || $ctrl.month == null || $ctrl.day == null) {
        $ctrl.ngModel = null;
        return; // If any part of the date isn't selected return undefined.
      }
      var date = new Date($ctrl.year, $ctrl.month, $ctrl.day);
      var value = moment( [$ctrl.year, $ctrl.month, $ctrl.day, 12]);
      if (Utils.isDate(date)) {
        $ctrl.date = value;
        $ctrl.ngModel = value;
        if (typeof ($ctrl.ngYear) !== 'undefined') $ctrl.ngYear = $ctrl.year;
        if (typeof ($ctrl.ngMonth) !== 'undefined') $ctrl.ngMonth = $ctrl.month + 1; // javascript uses zero-indexed months.
        if (typeof ($ctrl.ngDay) !== 'undefined') $ctrl.ngDay = $ctrl.day;
      }
    }

    this.changeMonth = function () {
      // update the number of days.
      var days = genDays($ctrl.year, $ctrl.month);
      if (days.length !== $ctrl.days.length) {
        $ctrl.days = days;
      }
      setDate();
    }

    this.changeDay = function () {
      setDate();
    }

    this.changeYear = function () {
      // update the number of days if February.
      if ($ctrl.month === 1) {
        $ctrl.days = genDays($ctrl.year, $ctrl.month);
      }
      setDate();
    }
  }
});
