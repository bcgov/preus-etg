var $ = require('jquery');

var months = [
  'January',
  'February',
  'March',
  'April',
  'May',
  'June',
  'July',
  'August',
  'September',
  'October',
  'November',
  'December'
];

var defaultDayLabel = 'Day';
var defaultMonthLabel = 'Month';
var defaultYearLabel = 'Year';
var validClassName = 'k-valid';
var invalidClassName = 'k-invalid';

function DateField(el, showDefaultPlaceHolder) {
  var $root = $(el);
  var api = {};
  var enabled = true;
  var _date;
  var $month;
  var $day;
  var $year;
  var $ddlMonth;
  var $ddlDay;
  var $ddlYear;
  var $status;
  var showDefaultPlaceHolder;

  var hasDateRange;
  var isStartDate;
  var isEndDate;
  var $startDate;
  var $endDate;
  var $error;

  function init() {
    $month = $root.find('input[type=hidden][id*=Month]');
    $day = $root.find('input[type=hidden][id*=Day]');
    $year = $root.find('input[type=hidden][id*=Year]');
    $ddlMonth = $root.find('select[data-bind=' + $month.attr('id') + ']');
    $ddlDay = $root.find('select[data-bind=' + $day.attr('id') + ']');
    $ddlYear = $root.find('select[data-bind=' + $year.attr('id') + ']');
    $error = $root.next('.block__form--message-wrapper');
    $status = $('<div class="field__date--status field-validation-error is-hidden" />').appendTo($error);
    $startDate = $(document.getElementById($root.data('start-date-id')));
    $endDate = $(document.getElementById($root.data('end-date-id')));
    hasDateRange = ($startDate.length || $endDate.length) ? true : false;
    isStartDate = $endDate.length > 0;
    isEndDate = $startDate.length > 0;
    if (showDefaultPlaceHolder == null) {
      showDefaultPlaceHolder = true; 
    }

    if ($root.data('disabled')) {
      enabled = false;
      $ddlMonth.attr('disabled', 'disabled');
      $ddlDay.attr('disabled', 'disabled');
      $ddlYear.attr('disabled', 'disabled');
    }

    readDateFromHiddenFields();
    populateFields();
    setFields();
    bindEvents();
    watchForChange();
  }

  function bindEvents() {
    $ddlYear.on('change', handleYearMonthChange);
    $ddlMonth.on('change', handleYearMonthChange);
    $ddlDay.on('change', handleDateChange);
  }

  function populateFields() {
    populateDays(31);
    populateMonths();
    populateYears();
  }

  function populateDays(days) {
    if ($ddlDay.children('option').length === days + (showDefaultPlaceHolder ? 1 : 0)) return;

    $ddlDay.html('');
    var defaultLabel = defaultDayLabel;
    var s = '';
    if (showDefaultPlaceHolder) s += createDropdownOption(defaultLabel).outerHTML;
    for (var i = 1; i <= days; i++) {
      s += createDropdownOption(i, i).outerHTML;
    }
    $ddlDay.append(s);
  }

  function populateMonths() {
    $ddlMonth.html('');
    var defaultLabel = defaultMonthLabel;
    var s = '';
    if (showDefaultPlaceHolder) s += createDropdownOption(defaultLabel).outerHTML;
    for (var i = 0, l = months.length; i < l; i++) {
      s += createDropdownOption(months[i], (i + 1)).outerHTML;
    }
    $ddlMonth.append(s);
  }

  function populateYears() {
    $ddlYear.html('');
    var defaultLabel = defaultYearLabel;
    var now = new Date();
    var startYear = parseInt($ddlYear.attr("data-start")) || new Date().getFullYear() - 90;
    var endYear = parseInt($ddlYear.attr("data-end")) || new Date().getFullYear();
    var s = '';

    if (showDefaultPlaceHolder) s += createDropdownOption(defaultLabel).outerHTML;
    for (var i = startYear; i <= endYear; i++) {
      s += createDropdownOption(i, i).outerHTML;
    }
    $ddlYear.append(s);
  }

  function createDropdownOption(label, value) {
    var option = document.createElement('option');
    option.innerHTML = label;
    option.setAttribute('value', value === undefined ? 0 : value);
    return option;
  }

  function setFields() {
    var d;

    if (isValidDate() === true) {
      d = getDate();
      var year = d.getFullYear();
      var month = d.getMonth() + 1;
      $ddlYear.find('option[value=' + year + ']').attr('selected', true);
      $ddlMonth.find('option[value=' + month + ']').attr('selected', true);
      populateDays(getDaysForMonthYear(year, month));
      $ddlDay.find('option[value=' + d.getDate() + ']').attr('selected', true);
    }
    else {
      // Reset date fields
      $year.val(0);
      $month.val(0);
      $day.val(0);
    }
  }

  // Retrieve the date from the hidden fields and save it as a Date object
  function readDateFromHiddenFields() {
    var y = parseInt($year.val(), 10);
    var m = parseInt($month.val(), 10);
    var d = parseInt($day.val(), 10);
    _date = getDateString(y, m, d);
  }

  function readDateFromSelectFields() {
    var y = parseInt($ddlYear.val(), 10);
    var m = parseInt($ddlMonth.val(), 10);
    var d = parseInt($ddlDay.val(), 10);
    _date = getDateString(y, m, d);
  }

  // Update the hidden fields from the internal Date object
  function writeDateToHiddenFields() {
    var y = parseInt($ddlYear.val(), 10);
    var m = parseInt($ddlMonth.val(), 10);
    var d = parseInt($ddlDay.val(), 10);
    $year.val(y);
    $month.val(m);
    $day.val(d);
  }

  function setValidationMessage(msg) {
    $status.removeClass('is-hidden').text(msg);
  }

  function clearValidationMessage() {
    $status.addClass('is-hidden').empty();
  }

  function getDate() {
    var parts = _date.split('-');
    var d = new Date(parts[0], parts[1] - 1, parts[2]);
    d.setHours(0);
    d.setMinutes(0);
    d.setMilliseconds(0);
    d.setSeconds(0);
    return d;
  }

  // Return a date string in this format: YYYY-MM-DD
  // Add padding where necessary
  function getDateString(year, month, day) {
    var y = parseInt(year, 10);
    var m = parseInt(month, 10);
    var d = parseInt(day, 10);
    function pad(num) {
      if (num < 10) {
        return '0' + num;
      }
      return num;
    }
    return y + '-' + pad(m) + '-' + pad(d);
  }

  function isValidDate() {
    var parsed = Date.parse(_date);
    var parsedDate = new Date(parsed);
    var parsedStr = getDateString(parsedDate.getUTCFullYear(), (parsedDate.getUTCMonth() + 1), parsedDate.getUTCDate());
    return parsed !== null && isNaN(parsed) !== true && _date === parsedStr;
  }

  function isValidDateRange() {
    if (!hasDateRange) return true;

    var isValid = true, startDate, endDate, start, end;

    if ($startDate.length) {
      startDate = $startDate.data('datefield').getDate();
      endDate = getDate();
      if (startDate.getTime() > endDate.getTime()) {
        isValid = false;
      }
    } else if ($endDate.length) {
      startDate = getDate();
      endDate = $endDate.data('datefield').getDate();
      if (startDate.getTime() > endDate.getTime()) {
        isValid = false;
      }
    }
    return isValid;
  }

  function getDaysForMonthYear(year, month) {
    return new Date(year, month, 0).getDate();
  }

  function handleYearMonthChange() {
    populateDays(getDaysForMonthYear(parseInt($ddlYear.val(), 10), parseInt($ddlMonth.val(), 10)));
    handleDateChange();
  }

  function handleDateChange() {
    // Client-side validation doesn't stop user sending request, so let the server handle it
    writeDateToHiddenFields();
  }

  function validate() {

    readDateFromSelectFields();
    clearValidationMessage();

    if (isNaN(getDate().getTime())) return false;

    if (isValidDate() === true) {
      validateFields(true);
      if (isValidDateRange() === true) {
        validateFields(true);
        writeDateToHiddenFields();
      } else {
        if (isEndDate === true) {
          writeDateToHiddenFields();
          setErrorMessage('Training end date cannot be before the start date');
        }
      }
    } else {
      setErrorMessage('Select a valid date');
    }
  }

  // Hide additional message when there's multiple messages
  function watchForChange() {
    var $wrapper = $error;

    if ($wrapper.length === 0) return false;

    var observer = new MutationObserver(function (mutations) {
      mutations.forEach(function (mutation) {
        if ($wrapper.find('.field-validation-error').length > 1) {
          $wrapper.find('.field__date--status').hide();
        } else {
          $wrapper.find('.field__date--status').show();
        }
      });
    });

    var config = { childList: true };
    observer.observe($wrapper[0], config);
  }

  function validateFields(isValid) {
    var className = isValid === true ? validClassName : invalidClassName;
    var elements = [$ddlMonth.get(0), $ddlDay.get(0), $ddlYear.get(0)];
    $(elements).removeClass([validClassName, invalidClassName].join(' ')).addClass(className);
  }

  function setErrorMessage(msg) {
    validateFields(false);
    setValidationMessage(msg);
  }

  function clearErrorMessage() {
    validate();
  }

  function isEnabled() {
    return enabled;
  }

  api = {
    getDate: getDate,
    validate: validate,
    setErrorMessage: setErrorMessage,
    clearErrorMessage: clearErrorMessage,
    isEnabled: isEnabled,
  };

  init();

  // Public properties and methods
  $root.data('datefield', api);
  return api;
}

$('.datefield').each(function () {
  if (!$(this).hasClass('ignore-parent'))
    var df = new DateField(this);
});

module.exports = DateField;
