// Mini helper for header nav
// main purpose is to hide vertical bar when either user name or org name wraps into 2 lines
var $header = angular.element('.header');
var doit;

function init() {
  checkHeightToggleClass();
  bindEvent();
}

function bindEvent() {
  window.onresize = function(){
    clearTimeout(doit);
    doit = setTimeout(checkHeightToggleClass, 100);
  };
}

function checkHeightToggleClass() {
  if($header.find('.menu--item__names').height() > 25) {
    $header.find('.hide-on-wrap').addClass('is-hidden');
  } else {
    $header.find('.hide-on-wrap').removeClass('is-hidden');
  }
}

(function() {
  init();
})();
