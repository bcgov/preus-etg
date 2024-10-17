var utils = require('../../shared/utils');

app.controller('GrantOpeningView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'GrantOpeningView',
    edit: {
      dates: false,
      budget: false
    },
    save: {
      url: function () {
        return ($scope.ngDialogData.model.Id == 0 ? '/Int/Admin/Grant/Opening' : '/Int/Admin/Grant/Opening');
      },
      method: function () {
        return $scope.ngDialogData.model.Id == 0 ? 'POST' : 'PUT'
      },
      data: function () {
        return $scope.ngDialogData.model;
      }
    },
    onSave: function (event, data) {
      return $timeout(function () {
        $scope.ngDialogData.model = data.response.data;
        backup();
        return $scope.toggle();
      });
    },
    minDate: new Date(new Date($scope.ngDialogData.model.TrainingPeriodStartDate).setFullYear($scope.ngDialogData.model.TrainingPeriodStartDate.getFullYear() - 1))
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  function backup() {
    $scope.ngDialogData.backup = angular.copy($scope.ngDialogData.model);
  }

  function restore() {
    $scope.sync($scope.ngDialogData.backup, $scope.ngDialogData.model);
  }

  $scope.deleteGrantOpening = function () {
    return $scope.confirmDialog('Delete Grant Opening', 'Are you sure you want to delete this grant opening?')
      .then(function () {
        return $scope.ajax({
          url: '/Int/Admin/Grant/Opening/Delete',
          method: 'PUT',
          data: function () {
            return $scope.ngDialogData.model;
          }
        })
          .then(function () {
            return $scope.confirm();
          });
      })
      .catch(angular.noop);
  }

  $scope.changeGrantStatus = function ($event, stateName) {
    if (!$event.target.hasAttribute('disabled')) {
      $event.target.setAttribute("disabled", true);
      return $scope.load({
        url: '/Int/Admin/Grant/Opening/' + stateName + '/' + $scope.ngDialogData.model.Id,
        method: 'PUT',
        data: function () {
          return $scope.ngDialogData.model;
        },
        set: 'ngDialogData.model'
      }).then(function () {
        $timeout(function() {
          backup();
          $scope.setAlert({ 
            response: { status: 200 }, 
            message: 'Grant Opening has been ' + $scope.getOpeningStateLabel($scope.ngDialogData.model.State).toLowerCase() + ' successfully.' });
            $event.target.setAttribute("disabled", false);
        })
      }).catch(angular.noop);
    }
  }

  $scope.returnFundingApplications = function ($event) {
    if (!$event.target.hasAttribute('disabled')) {
      $event.target.setAttribute("disabled", true);
      return $scope.load({
        url: '/Int/Admin/Grant/Opening/ReturnUnfundedApplications/' + $scope.ngDialogData.model.Id,
        method: 'PUT',
        set: 'ngDialogData.model',
        timeout: 5 * 60 * 1000
      })
        .then(function () {
          return $timeout(function () {
            backup();
            $scope.IsEditing = false;
            $scope.setAlert({ 
              response: { status: 200 }, 
              message: 'All unfunded applications successfully returned.' });
            $event.target.setAttribute("disabled", false);
          });
        })
        .catch(angular.noop);
    }
  }

  $scope.getOpeningStateLabel = function (state) {
    if (state == 0) {
      return 'Unscheduled';
    }
    else if (state == 1) {
      return 'Scheduled';
    }
    else if (state == 2) {
      return 'Published';
    }
    else if (state == 3) {
      return 'Open';
    }
    else if (state == 4) {
      return 'Closed';
    }
    else if (state == 5) {
      return 'Open for Submit';
    }
    else {
      return 'Not Started';
    }
  }

  $scope.getOpeningStateCss = function (state) {
    if (state == 0 || state == 5) {
      return 'label--unpublished';
    }
    else if (state == 1) {
      return 'label--scheduled';
    }
    else if (state == 2) {
      return 'label--published';
    }
    else if (state == 3) {
      return 'label--open';
    }
    else if (state == 4) {
      return 'label--closed';
    }
    else {
      return 'label--notstarted';
    }
  }

  $scope.toggle = function (section) {
    return $timeout(function () {
      var editing = $scope.section.edit[section]
      if (editing) {
        restore(); // The Cancel button was pressed.
      }

      section = section || ($scope.section.edit.dates ? 'dates' : 'budget');
      editing = typeof (editing) === 'undefined' ? false : !editing;
      $scope.section.edit[section] = editing;
      $scope.IsEditing = editing;
      $scope.errors = {};
    });
  }

  $scope.validateAvailability = function () {
    var isValid = true;
    var askForConfirmation = true;

    function now() {
      var appDateTimeNow = $scope.ngDialogData.appDateTime;
      var now = appDateTimeNow && appDateTimeNow.length ? new Date(Date.parse(appDateTimeNow.val())) : new Date();
      now.setHours(0);
      now.setMinutes(0);
      now.setMilliseconds(0);
      now.setSeconds(0);
      return now;
    }

    var validatePublishDate = $scope.ngDialogData.model.State < utils.GrantOpeningStates.Published;
    var validateOpeningDate = $scope.ngDialogData.model.State < utils.GrantOpeningStates.Open;
    var validateClosingDate = true;

    if (validatePublishDate && $scope.ngDialogData.model.PublishDate < now()) {
      $scope.errors.PublishDate = 'Publish date can not be set to a date in the past.';
      isValid = false;
      askForConfirmation = false;
    } else {
      delete $scope.errors.PublishDate;
    }

    if (validateOpeningDate && $scope.ngDialogData.model.OpeningDate < now()) {
      $scope.errors.OpeningDate = 'Opening date can not be set to a date in the past.';
      isValid = false;
      askForConfirmation = false;
    } else {
      delete $scope.errors.OpeningDate;
    }

    if (validateClosingDate && $scope.ngDialogData.model.ClosingDate < now()) {
      $scope.errors.ClosingDate = 'Closing date can not be set to a date in the past.';
      isValid = false;
      askForConfirmation = false;
    } else {
      delete $scope.errors.ClosingDate;
    }

    if (askForConfirmation
      && validateClosingDate
      && ($scope.ngDialogData.model.State === 1 || $scope.ngDialogData.model.State === 2 || $scope.ngDialogData.model.State === 3)
      && $scope.ngDialogData.model.ClosingDate.getTime() === now().getTime()) {
      askForConfirmation = false;
      if (!confirm('The dates you have entered will make the grant opening CLOSED today. Are you sure you want to save these dates?')) {
        isValid = false;
      }
    }

    if (askForConfirmation
      && validateOpeningDate
      && ($scope.ngDialogData.model.State === 1 || $scope.ngDialogData.model.State === 2)
      && $scope.ngDialogData.model.OpeningDate.getTime() === now().getTime()) {
      askForConfirmation = false;
      if (!confirm('The dates you have entered will make the grant opening OPEN today. Are you sure you want to save these dates?')) {
        isValid = false;
      }
    }

    if (askForConfirmation
      && validatePublishDate
      && ($scope.ngDialogData.model.State === 1)
      && $scope.ngDialogData.model.PublishDate.getTime() === now().getTime()) {
      askForConfirmation = false;
      if (!confirm('The dates you have entered will make the grant opening PUBLISHED today. Are you sure you want to save these dates?')) {
        isValid = false;
      }
    }
    return isValid;
  }

  $scope.recalculatedAmt = function () {
    $scope.ngDialogData.model.DeniedAmt = ($scope.ngDialogData.model.BudgetAllocationAmt == null ? 0 : $scope.ngDialogData.model.BudgetAllocationAmt) *
      ($scope.ngDialogData.model.PlanDeniedRate == null ? 0 : $scope.ngDialogData.model.PlanDeniedRate)
    $scope.ngDialogData.model.WithdrawnAmt = ($scope.ngDialogData.model.BudgetAllocationAmt == null ? 0 : $scope.ngDialogData.model.BudgetAllocationAmt) *
      ($scope.ngDialogData.model.PlanWithdrawnRate == null ? 0 : $scope.ngDialogData.model.PlanWithdrawnRate);
    $scope.ngDialogData.model.ReductionAmt = ($scope.ngDialogData.model.BudgetAllocationAmt == null ? 0 : $scope.ngDialogData.model.BudgetAllocationAmt) *
      ($scope.ngDialogData.model.PlanReductionRate == null ? 0 : $scope.ngDialogData.model.PlanReductionRate);
    $scope.ngDialogData.model.SlippageAmt = ($scope.ngDialogData.model.BudgetAllocationAmt == null ? 0 : $scope.ngDialogData.model.BudgetAllocationAmt) *
      ($scope.ngDialogData.model.PlanSlippageRate == null ? 0 : $scope.ngDialogData.model.PlanSlippageRate);
    $scope.ngDialogData.model.CancellationAmt = ($scope.ngDialogData.model.BudgetAllocationAmt == null ? 0 : $scope.ngDialogData.model.BudgetAllocationAmt) *
      ($scope.ngDialogData.model.PlanCancellationRate == null ? 0 : $scope.ngDialogData.model.PlanCancellationRate);
    $scope.ngDialogData.model.IntakeTargetAmt = parseFloat($scope.ngDialogData.model.BudgetAllocationAmt) + $scope.ngDialogData.model.DeniedAmt + $scope.ngDialogData.model.WithdrawnAmt + $scope.ngDialogData.model.ReductionAmt + $scope.ngDialogData.model.SlippageAmt + $scope.ngDialogData.model.CancellationAmt;
    $scope.ngDialogData.model.IntakeTargetRate = parseFloat($scope.ngDialogData.model.PlanDeniedRate) + parseFloat($scope.ngDialogData.model.PlanWithdrawnRate) + parseFloat($scope.ngDialogData.model.PlanReductionRate) + parseFloat($scope.ngDialogData.model.PlanSlippageRate) + parseFloat($scope.ngDialogData.model.PlanCancellationRate);
  }

  $scope.safeApply = function (fn) {
    var phase = this.$root.$$phase;
    if (phase == '$apply' || phase == '$digest') {
      if (fn && (typeof (fn) === 'function')) {
        fn();
      }
    } else {
      this.$apply(fn);
    }
  };

  $scope.handleCheckboxToggle = function (dialog) {
    var $dialog
    var $container;
    if (dialog) {
      var $dialog = $("#" + dialog);
      $dialog.toggleClass('open');
      $container = $dialog.find('.block--contact-us__accordion--body').first();
    }
    else {
      $container = $("#return-unfunded-applications");
    }

    var $cbx = $container.find('input[type=checkbox]');
    $cbx.click(function () {
      $scope.safeApply(function () {
        if (dialog)
          $scope.disableCloseToggleButton = $cbx.is(':checked');
        else
          $scope.disableReturnToggleButton = $cbx.is(':checked');
      });
    });
    $container.toggleClass('is-hidden');

    if ($container.is(':visible')) {
      $scope.disableCloseToggleButton = false;
      $scope.disableReturnToggleButton = false;
    }

    if (!$container.is(':visible') && $cbx.is(':checked')) {
      $cbx.trigger('click');
      $scope.disableCloseToggleButton = false;
      $scope.disableReturnToggleButton = false;
    }
  }

  function init() {
    backup();
  }

  init();
});




