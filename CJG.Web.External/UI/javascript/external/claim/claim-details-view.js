app.controller('ClaimDetailsView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId,
    claimId: $attrs.ngClaimId,
    claimVersion: $attrs.ngClaimVersion
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request to fetch claim data.
   * @function loadClaim
   * @returns {Promise}
   **/
  function loadClaim() {
    return $scope.load({
      url: '/Ext/Claim/Details/' + $scope.section.claimId + '/' + $scope.section.claimVersion,
      set: 'model'
    });
  }
  
  /**
   * Initialize form data.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadClaim()
    ])
      .catch(angular.noop);
  }

  $scope.toggle = function (claimEligibleCost) {
    var el = angular.element("#claim-eligible-cost-" + claimEligibleCost.Id);

    if (el.is(":hidden")) {
      el.show();
      var t = angular.element("#panel-header-eligible-cost-" + claimEligibleCost.Id).find("span.k-icon");
      t.removeClass("k-i-arrow-s");
      t.removeClass("k-panelbar-expand");
      t.addClass("k-i-arrow-n");
      t.addClass("k-panelbar-collapse");
    }
    else {
      var t = angular.element("#panel-header-eligible-cost-" + claimEligibleCost.Id).find("span.k-icon");
      t.removeClass("k-i-arrow-n");
      t.removeClass("k-panelbar-collapse");
      t.addClass("k-i-arrow-s");
      t.addClass("k-panelbar-expand");
      el.hide();
    }
  }

  $scope.expandComponents = function () {
    for (var i = 0; i < $scope.model.Claim.EligibleCosts.length; i++) {
      $scope.toggle($scope.model.Claim.EligibleCosts[i]);
    }
  }

  init();
});


