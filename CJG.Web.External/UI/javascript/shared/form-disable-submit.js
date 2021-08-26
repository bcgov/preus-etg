// See utils.js for usage

// 2652 - Update forms to disable submit buttons after clicked
//
// Current issues:
//
// Can't add `disabled` attribute to a button on click, because Controller logic depends
// on Submit button being posted as form data. Disabled elements do not get POSTed.
//
// We could disable a button based on which element has focus (which element was clicked)
// when the form is submitted. However this does not work cross browser, issues with Safari.
//
// Safari won't redraw UI when form is submitting, causing the `disabled` styles to not
// apply. This could be problematic on iOS (iPhone, iPad) as well.


var lastButtonClicked = $();
var formSubmitted = false;
var timerId;
var timeout = 400;

function DisableButtonOnSubmit($el) {
  this.$root = $el || $('body');

  this.$root.on('click.disablesubmit', 'a.btn:not([data-disableonsubmit="false"]), button[type=submit]:not([data-disableonsubmit="false"])', function(e) {
    lastButtonClicked = $(this);
    if ( $(this).is('a') === true) {
      disableLastButtonClicked();
    }
  });

  this.$root.find('form').on('submit.disablesubmit', function(e) {
    var $form = $(this);
    if (formSubmitted) return false;
    formSubmitted = true;
    disableLastButtonClicked();
  });

  return this;
}

// Not implemented or tested yet
// e.g.
// var disableButtons = new utils.disableButtonOnSubmit($modalContainer);
// disableButtons.destroy();
DisableButtonOnSubmit.prototype.destroy = function() {
  // this.$root.find('a.btn, button[type=submit]').off('click.disablesubmit');
  // this.$root.find('form').off('submit.disablesubmit');
}


function setTimer() {
  timerId = setTimeout(function() {
    enableLastButtonClicked();
  }, timeout);
}

function disableLastButtonClicked() {
  if (lastButtonClicked.length > 0) {
    if (lastButtonClicked.is('button')) {
      lastButtonClicked.addClass('disabled');
    } else {
      lastButtonClicked.attr('disabled', 'disabled');
    }
    setTimer();
  }
}

function enableLastButtonClicked() {
  clearInterval(timerId);
  lastButtonClicked.attr('disabled', false).removeClass('disabled');
  lastButtonClicked = $();
}

$(document).ajaxError(function(event, jqxhr, settings, thrownError) {
  enableLastButtonClicked();
});

module.exports = DisableButtonOnSubmit;
