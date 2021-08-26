var utils = {}

// Taken from https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Math/floor
function decimalAdjust(type, value, exp) {
  // If the exp is undefined or zero...
  if (typeof exp === 'undefined' || +exp === 0) {
    return Math[type](value);
  }
  value = +value;
  exp = +exp;
  // If the value is not a number or the exp is not an integer...
  if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0)) {
    return NaN;
  }
  // Shift
  value = value.toString().split('e');
  value = Math[type](+(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp)));
  // Shift back
  value = value.toString().split('e');
  return +(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp));
}

// Decimal floor
if (!Math.floor10) {
  Math.floor10 = function (value, exp) {
    return decimalAdjust('floor', value, exp);
  };
}

utils.TrainingProviderStates = {
  Incomplete: 0,
  Complete: 1,
  Requested: 2,
  RequestApproved: 3,
  RequestDenied: 4
}

utils.TrainingProgramStates = {
  Incomplete: 0,
  Complete: 1
}

utils.DescriptionStates = {
  Incomplete: 0,
  Complete: 1
}

utils.GrantOpeningStates = {
  Unscheduled: 0,
  Scheduled: 1,
  Published: 2,
  Open: 3,
  Closed: 4,
  OpenForSubmit: 5
}

utils.ApplicationStateExternal = {
  NotStarted: 0,
  Incomplete: 1,
  Complete: 2,
  Submitted: 3,
  ApplicationWithdrawn: 4,
  Approved: 5,
  ApplicationDenied: 6,
  CancelledByMinistry: 7,
  CancelledByAgreementHolder: 8,
  AcceptGrantAgreement: 9,
  ChangeRequestSubmitted: 10,
  ChangeRequestApproved: 11,
  ChangeRequestDenied: 12,
  NotAccepted: 13,
  AgreementWithdrawn: 14,
  AgreementRejected: 15,
  ClaimSubmitted: 21,
  ClaimReturned: 23,
  ClaimDenied: 24,
  ClaimApproved: 25,
  AmendClaim: 26,
  ReportCompletion: 27,
  Closed: 30
}

utils.ApplicationStateInternal = {
  Draft: 0,
  New: 1,
  PendingAssessment: 2,
  UnderAssessment: 3,
  ReturnedToAssessment: 4,
  RecommendedForApproval: 5,
  RecommendedForDenial: 6,
  OfferIssued: 7,
  OfferWithdrawn: 8,
  AgreementAccepted: 9,
  Unfunded: 10,
  ApplicationDenied: 11,
  AgreementRejected: 12,
  ApplicationWithdrawn: 13,
  CancelledByMinistry: 14,
  CancelledByAgreementHolder: 15,
  ChangeRequest: 16,
  ChangeForApproval: 17,
  ChangeForDenial: 18,
  ChangeReturned: 19,
  ChangeRequestDenied: 20,
  NewClaim: 21,
  ClaimAssessEligibility:22,
  ClaimReturnedToApplicant: 23,
  ClaimDenied: 24,
  ClaimApproved: 25,
  Closed: 30,
  ClaimAssessReimbursement: 31,
  CompletionReporting: 32
}

utils.TrainingCostStates = {
  Incomplete: 0,
  Complete: 1
}

utils.ServiceTypes = {
  SkillsTraining: 1,
  EmploymentServicesAndSupports: 2,
  Administration: 3
}

utils.ExpenseTypes = {
  ParticipantAssigned : 1,
  ParticipantLimited: 2,
  NotParticipantLimited: 3,
  AutoLimitEstimatedCosts: 4
}

utils.ProgramTypes ={
  EmployerGrant: 1,
  WDAService: 2
}

utils.Attachment = {
  MaxUploadSize: 5
}

utils.ClaimTypes = {
  SingleAmendableClaim: 1,
  MultipleClaimsWithoutAmendments: 2
}

// both sanitize functions assumes string to be passed as argument.
// decimal place is optional, defaults to 2 decimal points
utils.sanitizeFloat = function (str, dec) {
  // debugger;
  if (typeof dec === 'undefined' || typeof dec !== 'number') {
    // type checking
    dec = 2;
  }
  // remove all characters except 0-9 and .
  if (typeof str !== 'undefined' && str.length || typeof str === 'number') {
    typeof str !== 'string' ? str = str.toString() : "";
    return parseFloat(str.replace(/[^0-9.]/gmi, '')).toFixed(dec) || 0;
  } else {
    return 0;
  }
}

utils.truncateDecimals = function (number, digits) {
  const re = new RegExp("(\\d+\\.\\d{" + digits + "})(\\d)");
  const m = number.toString().match(re);
  return m ? parseFloat(m[1]) : number.valueOf();
}

// Always returns whole number.
// Any number after decimal points are rounded DOWN.
utils.sanitizeInteger = function (str) {
  // remove all characters except 0-9
  if (typeof str !== 'undefined' && str.length) {
    return Math.floor(parseFloat(str.replace(/[^0-9.]/gmi, ''), 10)) || 0;
  } else {
    return 0;
  }
}

utils.formatCurrency = function (num, fractionDigits) {
  if (fractionDigits === undefined) fractionDigits = 2;

  // debugger;
  if (typeof num === 'number') {
    return '$' + num.toFixed(fractionDigits).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  } else {
    num = parseFloat(utils.sanitizeFloat(num), 10);
    if (num === 0) {
      console.error('value provided in argument was not number');
      console.error('Variable entered was typeof ' + typeof num);
    } else {
      return '$' + num.toFixed(fractionDigits).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    }
  }
}

// We need to validate that AJAX responses are valid by checking that the returned
// response is not a Login page, due a to a session time out. This could be changed
// in the future to check response headers, or a heartbeat function. Ideally a
// session timeout would return with a different status code like 401, but authentication
// is handled by a 3rd party and is not accessible to the Application.
utils.handleAjaxSuccess = function (data, textStatus, xhr) {
  var valid = true;
  var $form;
  var formAction;
  var message = 'Your session timed out, please log in again.';
  var isJson;

  try {
    isJson = !!JSON.parse(data);
  } catch (err) {
    isJson = false;
  }

  utils.hideAlerts();

  if ($('.prm-overlay').length > 0) {
    $('.prm-overlay').remove();
  }

  // Check if a form action contains any strings that would identify
  // it as a login page or redirected location (AJAX can't handle 302 redirects)
  function invalidAction(action) {
    var valid = true;
    var keywords = [
      'preLogon',
      'Auth/LogIn',
    ];
    $(keywords).each(function (i, item) {
      if (action.indexOf(item) !== -1) return valid = false;
    });
    return valid;
  }

  function backToIndex() {
    window.location = '/';
  }
  // Test the AJAX response for a redirected location
  if (typeof data === 'string' && !isJson) {
    $form = $(data).find('form:first');
    if ($form.length > 0) {
      formAction = $form.attr('action');
      valid = invalidAction(formAction);
      if (!valid) {
        AlertPopup(message, backToIndex);
      }
    }
  }
  return valid;
}

utils.hideLoadingPanel = function () {
  kendo.ui.progress($("html"), false);
}

utils.showLoadingPanel = function () {
  kendo.ui.progress($("html"), true);
}

// Handle Errors by displaying a popup, and output error in JavaScript console
utils.handleAjaxError = function (xhr, textStatus, errorThrown) {
  utils.hideLoadingPanel();
  var message = 'An unexpected error occured, please try again later or contact support.';
  var consoleError = errorThrown || 'An unknown error occured.';
  var alertType = "alert--default";
  if (xhr.statusText != "") {
    if (xhr.status === 500) {

      var errorType = xhr.getResponseHeader("Error-Type");

      if (errorType && errorType === 'HttpRequestValidationException') {
        message = 'Invalid character detected in entered text';
      } else {
        message = 'The grant file referenced by your action is no longer in a state or available for the action to occur. <br />' +
          'Please use Grant Files to locate and select the grant file again.';
      }

    } else {
      message = xhr.statusText;
    }
  }
  if (xhr.getResponseHeader("Alert-Type") !== null) {
    alertType = xhr.getResponseHeader("Alert-Type");
  }

  utils.displayAlert(message, alertType);
  console.error('Error: ' + consoleError + ' (' + this.url + ')');
}

utils.displayAlert = function (message, type, doScrollToTop, $contextual) {

  if (doScrollToTop === undefined) {
    doScrollToTop = true;
  }

  function toggleMessage($el) {
    $el.removeClass(function (index, className) {
      return (className.match(/(^|\s)alert--\S+/g) || []).join(' ');
    })
      .addClass('alert--ajax')
      .addClass(type)
      .html(message);
  }

  toggleMessage($('.row--alert .alert'));

  if ($contextual !== undefined) {
    toggleMessage($contextual.find('.row.alert--contextual div'));
  }

  $('.row--alert').removeClass('is-hidden');

  if (!utils.isInView($('.row--alert')) && doScrollToTop) {
    utils.scrollIntoView($('.row--alert'));
  }
}

utils.hideAlerts = function () {
  var $alert = $('.row--alert .alert--ajax');

  if ($alert.length > 0) {
    $('.row--alert').addClass('is-hidden');

    $alert
      .removeClass(function (index, className) {
        return (className.match(/(^|\s)alert--\S+/g) || []).join(' ');
      })
      .empty();
  }
}

utils.isInView = function (el) {
  var $el = $(el).first();
  var inView = false;
  if ($el.length > 0) {
    var top = $(document).scrollTop();
    var bottom = top + $(document).height();
    var elementTop = $el.position().top;
    inView = elementTop >= top && elementTop <= bottom;
  }
  return inView;
}

utils.scrollIntoView = function (el) {
  var $el = $(el).first();
  var props;
  var settings;
  var top = $el.position().top - 20;

  if ($('.application-header').length > 0) {
    top = $el.position().top - 80;
  }

  if ($el.length > 0) {
    props = {
      scrollTop: top
    };
    settings = {
      speed: 500
    };
    $('body, html').animate(props, settings);
  }
}

utils.grantApplicationMatchExternalState = function (state, id, callback) {
  $.ajax({
    type: "GET",
    cache: false,
    url: "/Ext/Application/State/" + id + "?state=" + state,
    success: function (data, textStatus, xhr) {
      if (!utils.handleAjaxSuccess(data, textStatus, xhr)) {
        callback(false, data.state)
        return false;
      }
      callback(data.success, data.state);
    },
    error: utils.handleAjaxError,
  });
}

utils.displayStateConflict = function (newState, conflictState) {
  if (conflictState !== undefined) {
    utils.displayAlert(
      '<b>"' + newState + '"</b> action is no longer valid ' +
      'for the current grant file state of <b>"' + conflictState + '"</b>. ' +
      'The state may have been changed by actions of another user.',
      'alert--error');
  } else {
    utils.displayAlert(
      'The grant file referenced by your action is no longer in a state or available for the action to occur. <br />' +
      'Please use Grant Files to locate and select the grant file again.'
    );
  }

}

utils.parseCookie = function (cname) {
  var name = cname + "=";
  var decodedCookie = decodeURIComponent(document.cookie);
  var ca = decodedCookie.split(';');
  for (var i = 0; i < ca.length; i++) {
    var c = ca[i];
    while (c.charAt(0) == ' ') {
      c = c.substring(1);
    }
    if (c.indexOf(name) == 0) {
      return c.substring(name.length, c.length);
    }
  }
  return "";
}

// Window resize event　with delay
utils.delayAction = function (action, cb, delay) {
  var resizeTimer = 0;
  var w = $(window).width();

  $(window).on(action, function () {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(function () {
      switch (action) {
        case 'resize':
          if (w !== $(window).width()) {
            cb();
            w = $(window).width();
          }

        case 'keydown':
          cb();
      }

    }, delay);
  });
}

utils.disableButtonOnSubmit = require('./form-disable-submit');

module.exports = utils;
