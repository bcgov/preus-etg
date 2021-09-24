var $ = require('jquery');

function CharacterCount(el) {
  var $input = $(el)
  var $status = $('<div class="char-count-status" />')
  var maxlength = $input.attr('maxlength');

  function handleChange() {
    var count = $input.val().length;

    $input.next('.char-count-status').text((maxlength - count) + ' chars left');
  }

  if(!$input.hasClass('char-count-enabled')) {
    $input.addClass('char-count-enabled');
    $status.insertAfter($input);
    $input.on('input', handleChange);
    handleChange();
  } else {
    handleChange();
  }
}

$.fn.charactercount = function() {
  return this.each(function() {
    var counter;
    if (!$(this).data('charactercount')) {
      $(this).data('charactercount', new CharacterCount(this));
    }
  });
}

module.exports = CharacterCount;
