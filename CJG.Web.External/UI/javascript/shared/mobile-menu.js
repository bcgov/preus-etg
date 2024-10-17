var $ = require('jquery');
var $menuTrigger = $('.mobile-menu--trigger');
var $menu = $('.mobile-menu');
var $menuList = $('.mobile-menu--list');

if ($menuTrigger && $menu) {
	bindMenuEvent();
}

function bindMenuEvent() {
	$menuTrigger.click(function(e) {
		var menuStatus = $menu.attr('menu-status');

		if (menuStatus == "opened") {
			$menuList.slideToggle("fast", function() {
				$menu.attr('menu-status','closed');
			});
		} else {
			$menuList.slideToggle("fast", function() {
				$menu.attr('menu-status','opened');
			});
		}
	});
}