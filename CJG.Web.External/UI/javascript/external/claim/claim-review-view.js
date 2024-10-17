app.controller('ClaimReviewView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId,
    claimId: $attrs.ngClaimId,
    claimVersion: $attrs.ngClaimVersion
  };

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  $scope.LoadReviewClaim = function () {
    return $scope.load({
      url: '/Ext/Claim/Reporting/Review/' + $scope.section.claimId + '/' + $scope.section.claimVersion,
      set: 'model'
    })
      .catch(angular.noop);
  }
  $scope.LoadReviewClaim();

  $scope.SubmitClaim = function () {
    return $scope.ajax({
      url: '/Ext/Claim/Submit',
      method: 'PUT',
      data: $scope.model
    })
      .then(function (response) {
        window.location = response.data.RedirectURL;
      })
      .catch(angular.noop);
  }

  $scope.toggle = function (claimEligibleCost) {
    var el = angular.element("#claim-eligible-cost-" + claimEligibleCost.Id);

    if (el.is(":hidden")) {
      el.show();
      var t = $("#panel-header-eligible-cost-" + claimEligibleCost.Id).find("span.k-icon");
      t.removeClass("k-i-arrow-s");
      t.removeClass("k-panelbar-expand");
      t.addClass("k-i-arrow-n");
      t.addClass("k-panelbar-collapse");
      var l = angular.element("#panel-header-toggle-eligible-cost-icon-" + claimEligibleCost.Id); //.find("span.k-icon");
      l.removeClass("down-arrow");
      l.addClass("up-arrow");
    }
    else {
      var t = angular.element("#panel-header-eligible-cost-" + claimEligibleCost.Id).find("span.k-icon");
      t.removeClass("k-i-arrow-n");
      t.removeClass("k-panelbar-collapse");
      t.addClass("k-i-arrow-s");
      t.addClass("k-panelbar-expand");
      el.hide();
      var l = angular.element("#panel-header-toggle-eligible-cost-icon-" + claimEligibleCost.Id); //.find("span.k-icon");
      l.removeClass("up-arrow");
      l.addClass("down-arrow");
    }
  }

  $scope.toggleParticipants = function () {
    var el = angular.element("#participantAttendance");

    if (el.is(":hidden")) {
      el.show();
      var t = angular.element("#panel-header-participantAttendance").find("span.k-icon");
      t.removeClass("k-i-arrow-s");
      t.removeClass("k-panelbar-expand");
      t.addClass("k-i-arrow-n");
      t.addClass("k-panelbar-collapse");
      var l = angular.element("#panel-header-toggle-icon-participantAttendance"); //.find("span.k-icon");
      l.removeClass("down-arrow");
      l.addClass("up-arrow");

    } else {
      var t = angular.element("#panel-header-participantAttendance").find("span.k-icon");
      t.removeClass("k-i-arrow-n");
      t.removeClass("k-panelbar-collapse");
      t.addClass("k-i-arrow-s");
      t.addClass("k-panelbar-expand");
      el.hide();
      var l = angular.element("#panel-header-toggle-icon-participantAttendance"); //.find("span.k-icon");
      l.removeClass("up-arrow");
      l.addClass("down-arrow");
    }
  }
});


