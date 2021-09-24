var $ = require('jquery');
var $root = $('.js-completion-report');
var postObject;
var $continueBtn = $('button[value=Continue]');
var $cancelBtn = $('button[value=Cancel]');
var $textareas = $('.table--completion-report textarea,.completion-report--step4 textarea');
var $selectAll = $('#selectAllParticipants');
var $checkboxes = $root.find('.table--completion-report input[type=checkbox]');
var $allParticipantsCompletedTrainingNo = $('#allParticipantsCompletedTrainingNo,#allparticipantsEmployedNo');
var $allParticipantsCompletedTrainingYes = $('#allParticipantsCompletedTrainingYes,#allparticipantsEmployedYes');
var $allParticipantsMostImportantOutcome = $('#mostImportantOutcomeForAllParticipants');
var $warningModal = $('#warningMostImportantOutcomeForAllParticipants');
var $cancelWarningModal = $('#cancelCompletionReportWarning');
var $backWarningModal = $('#backCompletionReportWarning');
var $participantsTableRows;
var newValueAllParticipantsMostImportantOutcome;
var $participantRowsMostImportantDropdownAnyVal;
var $backBtn = $root.find('.btn--back');

function bindEvents() {
  $root.find('.table--completion-report.four-col').DataTable({
    // scrollY: 410,
    "language": {
      "lengthMenu": "Show <div class='selectmenu'>_MENU_</div> entries"
    },
    "aoColumns": [{"bSearchable": true}, {"bSearchable": false}, {"bSearchable": false}, {"bSearchable": false}]
  });
  $root.find('.table--completion-report.three-col').DataTable({
    // scrollY: 410,
    "language": {
      "lengthMenu": "Show <div class='selectmenu'>_MENU_</div> entries"
    },
    "aoColumns": [{"bSearchable": false}, {"bSearchable": true}, {"bSearchable": false}]
  });

  $(function() {
    $warningModal.kendoWindow({
      modal: true,
      visible: false,
      minWidth: 300,
      resizable: false,
      open: function() {
        var $closeModal = $('.btn--cancel-change-outcome-for-all-participants');
        var $yesChangeAll = $('.btn--change-outcome-for-all-participants');
        $participantsTableRows = $('.table--completion-report tbody tr');

        $closeModal.click(function() {
          closeModal();
        });

        $yesChangeAll.click(function() {
          changeAllParticipantsMostImportantOutcome($participantsTableRows, newValueAllParticipantsMostImportantOutcome);
          $participantRowsMostImportantDropdownAnyVal = $('.table--completion-report tbody tr [data-question-id="3"] .dropdown--most-important select').val();
          stepThreeDetermineIfContinueBtnIsActive($participantRowsMostImportantDropdownAnyVal);
          closeModal();
        });

        centerModal();
      },
      close: function() {
        //console.log('close modal');
        //window.location = window.location.href;
      }
    });
  });

  checkboxLogic();
  textareaLogic($textareas);
  continueLogic();
  stepOneTwoLogic();
  stepFourLogic();

  $cancelBtn.click(function () {
    cancelCompletionReport();
  });

  checkForTableValuesOnLoad();

  $selectAll.change(function() {
    var $this = $(this);
    var $selects = $root.find('.table--completion-report .selectmenu');
    if ($this[0].checked == true) {
      $checkboxes.each(function() {
        $(this).prop("checked", true);
        $selects.removeClass('disabled');
        $selects.find('select').removeAttr('disabled');
        if (!$(this).closest('tr').hasClass('active-row')) {
          $(this).closest('tr').addClass('active-row');
        }
      });
    } else {
      $checkboxes.each(function() {
        $(this).prop("checked", false);
        $selects.addClass('disabled');
        $selects.find('select').attr('disabled', 'disabled').val(0);
        if ($(this).closest('tr').hasClass('active-row')) {
          $(this).closest('tr').removeClass('active-row');
        }
      });
    }
  });

  if ($allParticipantsMostImportantOutcome.length > 0 && $warningModal.length > 0) {
    stepThreeLogic($allParticipantsMostImportantOutcome);
  }
}

function goBackStep() {
  var currentUrl = window.location.href;
  var currentStep = $('#CompletionReportStep').val();
  var previousStep = parseInt(currentStep) - 1;
  var newUrl = currentUrl.substr(0, currentUrl.length - 1) + previousStep;

  if (previousStep > 0) {
    window.location = newUrl;
  }
}

function cancelCompletionReport() {
  var currentUrl = window.location.href;
  var grantApplicationId = $('#grantApplicationId').val();
  window.location = "/Ext/Reporting/Grant/File/View/" + grantApplicationId;
}

function openModal(content) {
  $warningModal.html(content);
  $warningModal.data('kendoWindow').center().open();
}

function closeModal() {
  $allParticipantsMostImportantOutcome.val('0');
  $warningModal.data('kendoWindow').close();
}

function centerModal() {
  $warningModal.data('kendoWindow').center();
}

function closeWarningModal(warningType) {
  if (warningType == "cancel") {
    $cancelWarningModal.data('kendoWindow').close();
  }
  else {
    $backWarningModal.data('kendoWindow').close();
  }
}

function centerWarningModal(warningType) {
  if (warningType == "cancel") {
    $cancelWarningModal.data('kendoWindow').center();
  }
  else {
    $backWarningModal.data('kendoWindow').center();
  }
}

function stepThreeLogic($allParticipantsDropdown) {
  var $root = $('.completion-report--step3');
  var $requiredDropdowns = $root.find('select');
  var $mostImportantReason = $root.find('[id*="question-3-participant-"]');
  var $secondImportantReason = $root.find('[id*="question-4-participant-"]');
  var $thirdImportantReason = $root.find('[id*="question-5-participant-"]');
  var requiredDropdownsEntered = 0;
  var modalContent = '<div class="well modal--warning"><h2>Warning</h2><p>This will change the most important outcome for all participants in the table below.</p><button class="btn btn--primary btn--cancel-change-outcome-for-all-participants pull-left" type="button">Cancel</button><button class="btn btn--primary btn--change-outcome-for-all-participants pull-right" type="button">Yes, change all</button></div>';
  $participantRowsMostImportantDropdown = $('.table--completion-report tbody tr [data-question-id="3"] .dropdown--most-important select');
  $participantRowsMostImportantDropdownAnyVal = $participantRowsMostImportantDropdown.val();
  disableContinue = true;

  $mostImportantReason.on('change', function() {
    var $this = $(this);
    var $thisRow = $this.parents().closest('tr');
    $thisRow.find('[id*="question-4-participant-"]').val(0);
    $thisRow.find('[id*="question-5-participant-"]').val(0);

    $thisRow.find('[id*="question-4-participant-"] option').prop('disabled', false);
    $thisRow.find('[id*="question-5-participant-"] option').prop('disabled', false);

    $thisRow.find('[id*="question-4-participant-"] option[value=0]').prop('disabled', false);
    $thisRow.find('[id*="question-4-participant-"] option[value=' + $this.val() + ']').prop('disabled', true);

    $thisRow.find('[id*="question-5-participant-"] option[value=0]').prop('disabled', false);
    $thisRow.find('[id*="question-5-participant-"] option[value=' + $this.val() + ']').prop('disabled', true);

    if($this.val() != '0' || $this.val() != null) {
      $thisRow.find('[id*="question-4-participant-"]').removeAttr('disabled').parent().removeClass('disabled');
    } else {
      $thisRow.find('[id*="question-4-participant-"]').attr('disabled', 'disabled').parent().addClass('disabled');
    }

    $thisRow.find('[id*="question-5-participant-"]').attr('disabled', 'disabled').parent().addClass('disabled');
  });

  $secondImportantReason.each(function() {
    var $this = $(this);
    var $thisRow = $this.parents().closest('tr');
    var $selectBefore = $thisRow.find('[id*="question-3-participant-"]').val();

    if($selectBefore == '0' || $selectBefore == null) {
      // $this.attr('disabled', 'disabled').parent().addClass('disabled');
    } else {
      $thisRow.find('[id*="question-4-participant-"] option[value=' + $selectBefore + ']').prop('disabled', true);
      $thisRow.find('[id*="question-4-participant-"] option[value=0]').prop('disabled', false);
      $thisRow.find('[id*="question-5-participant-"] option[value=' + $selectBefore + ']').prop('disabled', true);
      $thisRow.find('[id*="question-5-participant-"] option[value=0]').prop('disabled', false);
    }
  }).on('change', function() {
    var $this = $(this);
    var $thisRow = $this.parents().closest('tr');
    var hiddenOption = $thisRow.find('[id*="question-3-participant-"]').val();

    $thisRow.find('[id*="question-5-participant-"]').val(0);
    $thisRow.find('[id*="question-5-participant-"] option').prop('disabled', false);
    $thisRow.find('[id*="question-5-participant-"] option[value=0]').prop('disabled', false);
    $thisRow.find('[id*="question-5-participant-"] option[value=' + $this.val() + ']').prop('disabled', true);
    $thisRow.find('[id*="question-5-participant-"] option[value=' + hiddenOption + ']').prop('disabled', true);

    if($this.val() != '0' && $this.val() != null) {
      $thisRow.find('[id*="question-5-participant-"]').removeAttr('disabled').parent().removeClass('disabled');
    } else {
      $thisRow.find('[id*="question-5-participant-"]').attr('disabled', 'disabled').parent().addClass('disabled');
    }
  });

  $thirdImportantReason.each(function() {
    var $this = $(this);
    var $thisRow = $this.parents().closest('tr');
    var $selectBefore = $thisRow.find('[id*="question-4-participant-"]').val();

    if($selectBefore == '0' || $selectBefore == null) {
      $this.attr('disabled', 'disabled').parent().addClass('disabled');
    } else {
      $thisRow.find('[id*="question-5-participant-"] option[value=' + $selectBefore + ']').prop('disabled', true);
    }
  });

  // When required field changes, check value/error state and remove error if necessary
  $requiredDropdowns.each(function() {
    var $this = $(this);
    $this.change(function() {
      if ($this.val() && $this.hasClass('k-invalid')) {
        $(this).removeClass('k-invalid');
      }
    });
  });

  $allParticipantsDropdown.change(function() {
    var $this = $(this);
    newValueAllParticipantsMostImportantOutcome = $this.val();

    var $participantDropDowns = $('[id*="question-3-participant-"]');
    var previouslySelectedValue = false;

    $.each($participantDropDowns, function () {
      if ($(this).val() != null) {
        previouslySelectedValue = true;
        return false;
      }
    });

    if (previouslySelectedValue) {
      openModal(modalContent);
    }
    else {
      $participantsTableRows = $('.table--completion-report tbody tr');
      changeAllParticipantsMostImportantOutcome($participantsTableRows, newValueAllParticipantsMostImportantOutcome);
      $participantRowsMostImportantDropdownAnyVal = $('.table--completion-report tbody tr [data-question-id="3"] .dropdown--most-important select').val();
      stepThreeDetermineIfContinueBtnIsActive($participantRowsMostImportantDropdownAnyVal);
      $allParticipantsMostImportantOutcome.val('0');
    }
  });

  $participantRowsMostImportantDropdown.each(function() {
    var $this = $(this);
    var currentVal = $this.val();

    $this.change(function() {
      currentVal = $this.val();
      $participantRowsMostImportantDropdowns = $('.table--completion-report tbody tr [data-question-id="3"] .dropdown--most-important select');
      $participantRowsMostImportantDropdownAnyVal = $participantRowsMostImportantDropdowns.val();
      stepThreeDetermineIfContinueBtnIsActive($participantRowsMostImportantDropdownAnyVal);
    });
  });

  stepThreeDetermineIfContinueBtnIsActive($participantRowsMostImportantDropdownAnyVal);
}

function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};

function stepFourLogic() {
  var $root = $('.completion-report--step4');
  var $requiredDropdowns = $root.find('select');
  var $requiredTextareas = $root.find('textarea');
  var requiredDropdownsEntered = 0;
  var requiredTextareasEntered = 0;
  var thisParticipantFormId = $('#participant-form-id').attr('value');
  var grantApplicationId = parseInt($('#grantApplicationId').attr('value'));
  var $currentStep = $('div[class*="completion-report--step"]');
  var currentStepClass = $currentStep[0].className;
  var currentStep = parseInt(currentStepClass.substr(currentStepClass.length - 1));
  var currentAffirmativeAnswerId = parseInt($('#AffirmativeAnswerId').attr('value'));

  if (currentAffirmativeAnswerId == 0) {
    currentAffirmativeAnswerId = null;
  }

  if ($root.length > 0) {
    if (!thisParticipantFormId) {
      thisParticipantFormId = 3;
    }

    $('textarea[maxlength]').charactercount();

    // When required field changes, check value/error state and remove error if necessary
    $requiredDropdowns.each(function() {
      var $this = $(this);
      $this.change(function() {
        if ($this.val() && $this.hasClass('k-invalid')) {
          $(this).removeClass('k-invalid');
        }
      });
    });

    $requiredTextareas.each(function() {
      var $this = $(this);
      $this.change(function() {
        if ($this.val() > 0 && $this.hasClass('k-invalid')) {
          $(this).removeClass('k-invalid');
        }
      });
    });

    // Disabling Save on clicking Back button.
    // $('body').on('click', 'button[value=Continue], .btn--back', function(e) {
    $('body').on('click', 'button[value=Continue]', function(e) {
      if(e.currentTarget.value === "Back") {
        postObject.GoBack = true;
      }
      if ($requiredDropdowns.length > 0) {
        $requiredDropdowns.each(function() {
          var $this = $(this);
          var thisSelectedIndex = $this[0].options.selectedIndex;
          thisQuestionId = $this.attr('data-question-id');
          thisAnswerId = $this.val();

          if ($this[0].selectedIndex !== 0) {
            requiredDropdownsEntered = requiredDropdownsEntered + 1;
          }

          if (thisSelectedIndex != 0) {
            rowObject = { "ParticipantFormId": parseInt(thisParticipantFormId), "QuestionId": parseInt(thisQuestionId), "AnswerId": parseInt(thisAnswerId), "OtherAnswer":"" };
                postObject.EmployerAnswers.push(rowObject);
          }
        });
      }
      if ($requiredTextareas.length > 0) {
        $requiredTextareas.each(function() {
          var $this = $(this);
          var thisEnteredValue = $this[0].value;
          thisQuestionId = $this.attr('data-question-id');

          if (thisEnteredValue.length > 0) {
            requiredTextareasEntered = requiredTextareasEntered + 1;
            rowObject = { "ParticipantFormId": parseInt(thisParticipantFormId), "QuestionId": parseInt(thisQuestionId), "AnswerId": null, "OtherAnswer":thisEnteredValue };
                postObject.EmployerAnswers.push(rowObject);
          }
        });
      }

      if ($requiredDropdowns.length === requiredDropdownsEntered && $requiredTextareas.length === requiredTextareasEntered) {
        // console.log('all required fields are entered');
        requiredDropdownsEntered = 0;
        requiredTextareasEntered = 0;
        postObject.AffirmativeAnswerId = currentAffirmativeAnswerId;
        postData(postObject);
        postObject = { grantApplicationId: grantApplicationId, CompletionReportStep: currentStep, ParticipantAnswers: [], EmployerAnswers: [], AffirmativeAnswerId: currentAffirmativeAnswerId, RowVersion: "" };
      } else {
        // console.log('all required fields are NOT entered');
        requiredDropdownsEntered = 0;
        requiredTextareasEntered = 0;
        // console.log('postObject:');
        // console.log(postObject);
        // postData(postObject);

        // Highlight fields that are missing values
        $requiredDropdowns.each(function() {
          var $this = $(this);
          if (!$this.val() && !$this.hasClass('k-invalid')) {
            $(this).addClass('k-invalid');
          }
        });

        $requiredTextareas.each(function() {
          var $this = $(this);
          if (!$this.val() && !$this.hasClass('k-invalid')) {
            $(this).addClass('k-invalid');
          }
        });

        $('html, body').animate({
          scrollTop: $('.k-invalid').offset().top - 24
        }, 300);

        postObject = { grantApplicationId: grantApplicationId, CompletionReportStep: currentStep, ParticipantAnswers: [], EmployerAnswers: [], AffirmativeAnswerId: currentAffirmativeAnswerId, RowVersion: "" };
      }
    });
  }
}

function stepThreeDetermineIfContinueBtnIsActive($mostImportantDropdownAnyVal) {
  var $participantRowsMostImportantDropdown = $('.table--completion-report tbody tr [data-question-id="3"] .dropdown--most-important select');
  var notNullValues = 0;

  $participantRowsMostImportantDropdown.each(function() {
    var $this = $(this);

    if ($this.val() !== null) {
      notNullValues = notNullValues + 1;
    }
  });

  if (notNullValues === $participantRowsMostImportantDropdown.length) {
    disableContinue = false;
  }

  if (disableContinue === false) {
    if ($continueBtn.hasClass('disabled')) {
      $continueBtn.removeClass('disabled');
    }
  } else if (disableContinue === true) {
    if (!$continueBtn.hasClass('disabled')) {
      $continueBtn.addClass('disabled');
    }
  }
}

function changeAllParticipantsMostImportantOutcome($participantRows, newValue) {
  $participantRows.each(function() {
    var $this = $(this);
    var $mostImportantReason = $('[data-question-id="3"] .dropdown--most-important select');
    var $secondImportantReason = $('[data-question-id="4"] .dropdown--most-important select');
    var $thirdImportantReason = $('[data-question-id="5"] .dropdown--most-important select');
    //var eachMostImportantOutcomeVal = $mostImportantReason.val();

    //if (eachMostImportantOutcomeVal !== newValue) {
    //  console.log('new value, update this dropdown to:', newValue)
    //}
    //else {
    //  console.log('this dropdown already is set to the new value so no need to do anything');
    //}

    $mostImportantReason.val(newValue);

    $secondImportantReason.val(0);
    $secondImportantReason.each(function () {
      $(this).removeAttr('disabled').parent().removeClass('disabled');
    });

    $thirdImportantReason.val(0);
    $thirdImportantReason.each(function () {
      $(this).attr('disabled', 'disabled').parent().addClass('disabled');
    });
  });
}

function stepOneTwoLogic() {
  // Deactivate table by default
  // Have all participants completed the training?
  // If so, then allow table to be selected
  var $root = $('.completion-report--step1,.completion-report--step2');
  var $tableCompletionReport = $root.find('.table--completion-report');
  var $allParticipantsCompletedTrainingInputs = $('input[type=radio][name="haveAllParticipantsCompletedTraining"],input[type=radio][name="wereAllParticipantsEmployed"]');
  var $currentStep = $('div[class*="completion-report--step"]');
  var currentStepClass = $currentStep[0].className;
  var currentStep = parseInt(currentStepClass.substr(currentStepClass.length - 1));

  if ($root.length > 0) {
    if (currentStep < 3 && !$allParticipantsCompletedTrainingNo.prop('checked')) {
      $allParticipantsCompletedTrainingYes.attr('checked','true');
    }
    participantsLogic();

    function participantsLogic() {
      var selectedOption;
      var currentStepClass = $currentStep[0].className;
      var currentStep = parseInt(currentStepClass.substr(currentStepClass.length - 1));
      var grantApplicationId = parseInt($('#grantApplicationId').attr('value'));
      var currentAffirmativeAnswerId = parseInt($('#AffirmativeAnswerId').attr('value'));

      if (currentAffirmativeAnswerId == 0) {
        currentAffirmativeAnswerId = null;
      }

      if ($allParticipantsCompletedTrainingNo[0].checked == true) {
        selectedOption = $tableCompletionReport.find('select')[0].options.selectedIndex;
        if($tableCompletionReport.hasClass('disabled')) {
          $allParticipantsCompletedTrainingYes.removeAttr('checked');
          $tableCompletionReport.removeClass('disabled');

          if(!$allParticipantsCompletedTrainingNo.prop('checked')) {
            $allParticipantsCompletedTrainingYes.attr('checked','true');
          }
        }

        if (selectedOption != 0) {
          if ($continueBtn.hasClass('disabled')) {
            $continueBtn.removeClass('disabled');
          }
        } else {
          if (!$continueBtn.hasClass('disabled')) {
            $continueBtn.addClass('disabled');
          }
        }

        $tableCompletionReport.find('tr').each(function() {
          var $tr = $(this);
          if($tr.find('select').val() == '8' || $tr.find('select').val() == '18') {
            $tr.find('.is-hidden').removeClass('is-hidden');
          }
          if(!$tr.find('input[type=checkbox]').prop('checked')) {
            $tr.find('.selectmenu').addClass('disabled')
               .find('select').attr('disabled', 'disabled');
          }
        });

      } else if ($allParticipantsCompletedTrainingYes[0].checked == true && !$tableCompletionReport.hasClass('disabled')) {
        // $allParticipantsCompletedTrainingNo.removeAttr('checked');
        // $allParticipantsCompletedTrainingYes.attr('checked','true');
        $tableCompletionReport.addClass('disabled');
        $tableCompletionReport.find('tr').each(function() {
          var $tr = $(this);
          $tr.removeClass('active-row');
          $tr.find('input[type=checkbox]').prop('checked', false);
          $tr.find('textarea').val('').parent().addClass('is-hidden');
          $tr.find('select').addClass('disabled').attr('disabled', 'disabled').val(0);
        });

        if ($continueBtn.hasClass('disabled')) {
          $continueBtn.removeClass('disabled');
        }

        postObject = { grantApplicationId: grantApplicationId, CompletionReportStep: currentStep, ParticipantAnswers: [], EmployerAnswers: [], AffirmativeAnswerId: currentAffirmativeAnswerId, RowVersion: "" };
      }
    }


    $allParticipantsCompletedTrainingInputs.click(function(e) {
      valueChanged = true;
      participantsLogic();
    });
  }
}

function checkForTableValuesOnLoad() {
  var $currentStep = $('div[class*="completion-report--step"]');
  var currentStepClass = $currentStep[0].className;
  var currentStep = parseInt(currentStepClass.substr(currentStepClass.length - 1));
  var $table = $('.table--completion-report');
  var $dropdowns = $table.find('select');

  $dropdowns.each(function() {
    var $this = $(this);
    var currentValue = $this[0].value;
    var $closestRow = $this.closest('tr');
    var $closestCheckbox = $closestRow.find('input[type="checkbox"]');

    if (currentValue > 0 && currentStep != '3') {
      if (!$closestRow.hasClass('active-row')) {
        $closestRow.addClass('active-row');
      }
      $closestCheckbox.prop("checked", true).trigger('change');
    }
  });

  if ($('.table--completion-report .active-row').length > 0 && $allParticipantsCompletedTrainingNo.length > 0) {
    if ($allParticipantsCompletedTrainingNo[0].checked !== true) {
      $allParticipantsCompletedTrainingYes.removeAttr('checked');
      $allParticipantsCompletedTrainingNo.prop("checked", true).trigger('change');
    }

    if ($table.hasClass('disabled')) {
      $table.removeClass('disabled');
    }
  }
}

function checkboxLogic() {
  var $activeRows;

  $checkboxes.each(function() {
    var $this = $(this);
    var $completionReportDropdowns;
    var $closestRow;
    var $closestSelectWrapDisabled;
    var $closestSelectWrap;
    var $closestSelect;

    $this.change(function() {
      $closestSelect = $this.closest('tr').find('select');
      $continueBtnNotDisabled = $('button[value=Continue]:not(.disabled)');

      if ($closestSelect.val() < 1) {
        if ($continueBtnNotDisabled.length > 0) {
          $continueBtn.addClass('disabled');
        }
      }

      if ($this[0].checked == true) {
        $closestRow = $this.closest('tr');
        $closestSelectWrapDisabled = $closestRow.find('.selectmenu.disabled');

        $closestRow.addClass('active-row');
        $closestSelect.removeAttr('disabled');

        if ($closestSelectWrapDisabled.length > 0) {
          $closestSelectWrapDisabled.each(function() {
            $(this).find('select').removeAttr('disabled');
            $(this).removeClass('disabled');
          });
        }

        $completionReportDropdowns = $('.table--completion-report select:not([disabled])');
        if ($completionReportDropdowns.length > 0) {
          selectLogic($completionReportDropdowns);
        }
      } else {
        $closestRow = $this.closest('tr');
        $closestSelectWrap = $closestRow.find('.selectmenu:not(.disabled)');
        if ($closestRow.hasClass('active-row')) {
          $closestRow.removeClass('active-row');
          $closestSelectWrap.each(function() {
            $(this).find('select').attr('disabled','true');
            $(this).addClass('disabled');
          });
          $selectAll.prop("checked", false);
          if ($selectAll.closest('tr').hasClass('active-row')) {
            $selectAll.closest('tr').removeClass('active-row');
          }
        }

        $completionReportDropdowns = $('.table--completion-report select:not([disabled])');
        $closestRow.find('select').val(0);
        $closestRow.find('textarea').parent().addClass('is-hidden');
        if ($completionReportDropdowns.length > 0) {
          selectLogic($completionReportDropdowns);
        }
      }

      setTimeout(function(){
        $activeRows =  $root.find('.table--completion-report .active-row');

        if ($activeRows.length < 1) {
          if (!$continueBtn.hasClass('disabled')) {
            $continueBtn.addClass('disabled');
          }
        } else {
          if ($continueBtn.hasClass('disabled')) {
            $continueBtn.removeClass('disabled');
          }
        }
      }, 100);
    });
  });
}

function selectLogic($selects) {
  var $selectMenus = $selects;
  // var $disabledContinueBtn;

  $selectMenus.each(function() {
    var $this = $(this);
    var $textareaCell;

    $this.change(function() {
      $textareaCell = $this.closest('td').find('.form-item + .form-item');
      // $disabledContinueBtn = $('button.disabled[name="btnContinue"]');

      if ($this.val()) {
        $(this).removeClass('k-invalid');
      }

      // if ($this.val.length > 0 && $disabledContinueBtn.length > 0) {
      //   $disabledContinueBtn.removeClass('disabled');
      // } else {
      //   $continueBtn.addClass('disabled');
      // }

      if ($this.val() == 8 || $this.val() == 18) {
        if ($textareaCell.hasClass('is-hidden')) {
          $textareaCell.removeClass('is-hidden');
          $textareaCell.removeAttr('aria-hidden');
        }
      } else {
        $textareaCell.addClass('is-hidden');
        $textareaCell.attr('aria-hidden','true');
      }
    });
  });
}

function textareaLogic($textareas) {
  $textareas.each(function() {
    var $thisTextarea = $(this);

    $thisTextarea.on('keyup', function () {
      if ($thisTextarea.val().length > 0) {
        if ($thisTextarea.hasClass('k-invalid')) {
          $thisTextarea.removeClass('k-invalid');
        }
      }
    });
  })
}

function post(path, params, method) {
    method = method || "post"; // Set method to post by default if not specified.

    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);

    for(var key in params) {
        if(params.hasOwnProperty(key)) {
          var hiddenField = document.createElement("input");
          hiddenField.setAttribute("type", "hidden");
          hiddenField.setAttribute("name", key);
          hiddenField.setAttribute("value", params[key]);
          if (params[key] !== null && params[key].constructor === Array) {
            for (var i = 0; i < params[key].length; i++) {
              var answer = params[key][i];
              for (var propertyKey in answer) {
                var nestedHiddenField = document.createElement("input");
                nestedHiddenField.setAttribute("type", "hidden");
                nestedHiddenField.setAttribute("name", key + "[" + i + "]." + propertyKey);
                nestedHiddenField.setAttribute("value", answer[propertyKey]);
                form.appendChild(nestedHiddenField);
              }
            }
          }
          else {
            hiddenField.setAttribute("value", params[key]);
            form.appendChild(hiddenField);
          }
        }
    }

    document.body.appendChild(form);
    form.submit();
}

function postData(object) {
  // var currentHref = window.location.href;
  // var currentOrigin = window.location.origin;
  // var currentPath = currentHref.replace(window.location.origin, "");
  var currentPath = $('#completion-report-form').val();
  var $rowErrors ;

  setTimeout(function(){
    $rowErrors = $('div[class*="completion-report--step"]').find('k-invalid');
    if ($rowErrors.length < 1) {
      // console.log('object:');
      // console.log(JSON.stringify(object));
      post(currentPath, object);
    }
  }, 100);
}

function continueLogic() {
  var $currentStep = $('div[class*="completion-report--step"]');
  var currentStepClass = $currentStep[0].className;
  var currentStep = parseInt(currentStepClass.substr(currentStepClass.length - 1));
  var $activeRows;
  var $invalidDropdowns;
  var grantApplicationId = parseInt($('#grantApplicationId').attr('value'));
  var currentAffirmativeAnswerId = parseInt($('#AffirmativeAnswerId').attr('value'));

  postObject = { grantApplicationId: grantApplicationId, CompletionReportStep: currentStep, ParticipantAnswers: [], EmployerAnswers: [], AffirmativeAnswerId: currentAffirmativeAnswerId, RowVersion: "" };

  $backBtn.on('click', goBackStep);

  // Disabling Save on clicking Back button.
  // $('body').on('click', 'button[value=Continue], .btn--back', function(e) {
  $('body').on('click', 'button[value=Continue]', function(e) {
    var thisParticipantFormId;
    var thisQuestionId;
    var thisAnswerId;
    var $thisDropdown;
    var $thisTextarea;
    var otherAnswer;
    var rowObject;

    if(e.currentTarget.value === "Back" && currentStep !== 4) {
      postObject.GoBack = true;
    }

    if (currentStep == 3) {
      $activeRows =  $root.find('.table--completion-report tr');
      $activeRows.each(function(i) {
        var $this = $(this);
        $thisDropdown = $this.find('select');
        thisParticipantFormId = $this.find('td[data-participant-form-id]').attr('data-participant-form-id');

        $thisDropdown.each(function() {
          var $this = $(this);
          var thisSelectedIndex = $this[0].options.selectedIndex;
          thisQuestionId = $this.closest('td[data-question-id]').attr('data-question-id');
          thisAnswerId = $this.val();

          if (thisSelectedIndex != 0) {
            rowObject = { "ParticipantFormId": parseInt(thisParticipantFormId), "QuestionId": parseInt(thisQuestionId), "AnswerId": parseInt(thisAnswerId), "OtherAnswer":"" };
                postObject.ParticipantAnswers.push(rowObject);
          }
        });
      });
      setTimeout(function(){
        // only send post if there are no invalid dropdowns
        $invalidDropdowns = $activeRows.find('.k-invalid');
        if ($invalidDropdowns.length < 1) {
          postData(postObject);
        }
      }, 100);
    } else if (currentStep == 1||2) {
      $activeRows =  $root.find('.table--completion-report .active-row');

      if ($activeRows.length > 0) {
        $activeRows.each(function(i) {
          var $this = $(this);
          thisParticipantFormId = $this.find('td[data-participant-form-id]').attr('data-participant-form-id');
          thisQuestionId = $this.find('td[data-question-id]').attr('data-question-id');
          $thisDropdown = $this.find('select');
          thisAnswerId = $thisDropdown.val() !== null ? parseInt($thisDropdown.val()) : null;
          $thisTextarea = $this.find('textarea');

          if (thisAnswerId === null) {
            // console.log('warning: please select a reason');
            $thisDropdown.addClass('k-invalid');
          } else if (thisAnswerId < 1) {
            $thisDropdown.addClass('k-invalid');
          }

          if (thisAnswerId !== null) {
            if (thisAnswerId > 0) {
              if (thisAnswerId == 8 || thisAnswerId == 18) {
                if ($thisTextarea.val().length < 1) {
                  $thisTextarea.addClass('k-invalid');
                } else {
                  otherAnswer = $thisTextarea.val();
                  rowObject = { "ParticipantFormId": parseInt(thisParticipantFormId), "QuestionId": parseInt(thisQuestionId), "AnswerId": parseInt(thisAnswerId), "OtherAnswer": otherAnswer };
                  postObject.ParticipantAnswers.push(rowObject);
                  // postData(postObject);
                }
              }

              if (thisAnswerId != 8 && thisAnswerId != 18) {
                rowObject = { "ParticipantFormId": parseInt(thisParticipantFormId), "QuestionId": parseInt(thisQuestionId), "AnswerId": parseInt(thisAnswerId), "OtherAnswer":"" };
                postObject.ParticipantAnswers.push(rowObject);
                // postData(postObject);
              }
            }
          }
        });

        setTimeout(function(){
          // only send post if there are no invalid dropdowns
          $invalidDropdowns = $activeRows.find('.k-invalid');
          if ($invalidDropdowns.length < 1) {
            postData(postObject);
          } else if ($invalidDropdowns.length > 0) {
            $('html, body').animate({
              scrollTop: $invalidDropdowns.offset().top - 24
            }, 300);
          }
        }, 100);
      } else if ($allParticipantsCompletedTrainingYes.length > 0) {
        if ($allParticipantsCompletedTrainingYes[0].checked = true) {
          // All participants completed training, send post
          postData(postObject);
        }
      }
      // else {
      //   console.log('no active rows');
      // }
    }

    $activeRows =  $root.find('.table--completion-report .active-row');

    if (currentAffirmativeAnswerId == 0) {
      currentAffirmativeAnswerId = null;
    }
  });
}

if ($root.length > 0) {
  bindEvents();
}
