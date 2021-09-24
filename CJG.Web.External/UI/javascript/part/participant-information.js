var DateField = require('../shared/datefield');
var participantSessionIdleTime = 0;
var participantSessionNotification = 5;
var participantSessionWarning = false;

app.controller('ParticipantInformationView', function ($scope, $controller, $attrs, $timeout, Utils, ngDialog, $http, $sce) {

  $scope.section = {
    sessionDuration: $attrs.ngSessionDuration || 0,
    validRecaptcha: false
  };

  angular.extend(this, $controller('Base', { $scope: $scope }));

  function initializeTimer() {
    if ($scope.section.sessionDuration) {
      //Increment the idle time counter every minute.
      setInterval(timerIncrement, 60000);

      //Zero the idle timer on mouse movement.
      angular.element(this).mousedown(function (e) {
        participantSessionReset();
      });
      angular.element(this).keypress(function (e) {
        participantSessionReset();
      });

      if ($scope.section.sessionDuration <= 5)
        participantSessionNotification = 1;

      timerIncrement();
    }
  }

  function timerIncrement() {
    if (++participantSessionIdleTime > $scope.section.sessionDuration) {
      participantSessionTimeout();
    } else if (participantSessionIdleTime == $scope.section.sessionDuration - participantSessionNotification + 1) {
      sessionTimeout();
    }
  }

  function sessionTimeout() {
    var message = "Your session will be timeout in "
      + participantSessionNotification + " minute"
      + (participantSessionNotification > 1 ? "s" : "")
      + ", and your data will be lost. Please click [OK] if you would like to stay on the page.";

    participantSessionWarning = true;

    return $scope.messageDialog('Session Timeout', message)
      .then(function () {
        participantSessionWarning = false;
        participantSessionReset();
      })
      .catch(angular.noop);
  }

  function participantSessionReset() {
    if (!participantSessionWarning) {
      participantSessionIdleTime = 0;
    }
  }

  function participantSessionTimeout() {
    window.location = '/Part/Information/Timeout';
  }

  initializeTimer();

  /**
   * Setup and initialize the Recaptcha form.
   * @function setupRecaptcha
   * @returns {void}
   **/
  function setupRecaptcha() {
    var el = document.getElementById('recaptcha');
    var key = el && el.getAttribute('data-site-key');
    if (el && key) {
      try {
        grecaptcha.render(el, {
          'sitekey': key,
          'callback': function (token) {
            return $timeout(function () {
              $scope.section.validRecaptcha = true;
            });
          },
          'expired-callback': function (token) {
            return $timeout(function () {
              $scope.section.validRecaptcha = false;
            });
          }
        });
      } catch (ex) {
        return $scope.messageDialog('ReCaptcha', '<p>Google Recaptcha failed to load, please try again later or contact support.</p><p>'  + ex.message + '</p>')
          .catch(angular.noop);
      }
    } else {
      return $scope.messageDialog('ReCaptcha', 'Google Recaptcha failed to load, please try again later or contact support.')
        .catch(angular.noop);
    }
  }

  /**
   * If this is step one of the PIF, setup the Recaptcha.
   * @function onRecaptchaLoad
   * @returns {void}
   **/
  window.onRecaptchaLoad = function () {
    if (angular.element('#step1-participant-info').length) {
      setupRecaptcha();
    }
    angular.element('#ng-loading-overlay').addClass('ng-hide');
  }

  var location = window.location.href.toLowerCase();
  if (location.indexOf("/part/") > -1) {
    // need to remove the menu if logged in
    // find the url and set to lower case
    // hide the menus
    angular.element('.desktop-menu, .mobile-menu').hide();
  }

  /**
   * Initialize the PIF form.
   * @function init
   * @returns {void}
   **/
  $scope.init = function () {
    $scope.model = getViewModel();
    Utils.convertDateStringsToDates($scope.model);

    setTimeout(function () {
      angular.element('.datefield').each(function () {
        var df = new DateField(this);
      });
    });
  }

  $scope.previous = function () {
    $scope.model.SummaryMessage = null;
    $scope.goto(-1);
    window.scrollTo(0, 0);
  }

  $scope.getDate = function (year, month, day) {
    if (year * month * day === 0) return null;
    return new Date(year + "/" + month + "/" + day);
  }

  $scope.resetIndegineous = function () {
    if ($scope.model.ParticipantInfoStep3ViewModel.CanadianStatus != 1 && $scope.model.ParticipantInfoStep3ViewModel.CanadianStatus != null) {
      $scope.model.ParticipantInfoStep3ViewModel.PersonAboriginal = 2;
      $scope.model.ParticipantInfoStep3ViewModel.CanadaImmigrant = null;
      $scope.model.ParticipantInfoStep3ViewModel.YearToCanada = 0;
      $scope.model.ParticipantInfoStep3ViewModel.CanadaRefugee = null;
      $scope.model.ParticipantInfoStep3ViewModel.FromCountry = null;
      $scope.model.ParticipantInfoStep3ViewModel.VisibleMinority = null;
    }

    else if ($scope.model.ParticipantInfoStep3ViewModel.CanadianStatus == null || $scope.model.ParticipantInfoStep3ViewModel.CanadianStatus == 1) {
      $scope.model.ParticipantInfoStep3ViewModel.PersonAboriginal = null;
      $scope.model.ParticipantInfoStep3ViewModel.CanadaImmigrant = null;
      $scope.model.ParticipantInfoStep3ViewModel.YearToCanada = 0;
      $scope.model.ParticipantInfoStep3ViewModel.CanadaRefugee = null;
      $scope.model.ParticipantInfoStep3ViewModel.FromCountry = null;
      $scope.model.ParticipantInfoStep3ViewModel.VisibleMinority = null;
    }
  }

  $scope.resetHiddenControlsforPersonAboriginal = function () {
    if ($scope.model.ParticipantInfoStep3ViewModel.PersonAboriginal == 1) {
      $scope.model.ParticipantInfoStep3ViewModel.CanadaImmigrant = null;
      $scope.model.ParticipantInfoStep3ViewModel.YearToCanada = 0;
      $scope.model.ParticipantInfoStep3ViewModel.CanadaRefugee = null;
      $scope.model.ParticipantInfoStep3ViewModel.FromCountry = null;
      $scope.model.ParticipantInfoStep3ViewModel.VisibleMinority = null;
    }
    else if ($scope.model.ParticipantInfoStep3ViewModel.PersonAboriginal == 2 || $scope.model.ParticipantInfoStep3ViewModel.PersonAboriginal == 3) {
      $scope.model.ParticipantInfoStep3ViewModel.LiveOnReserve = null;
      $scope.model.ParticipantInfoStep3ViewModel.AboriginalBand = null;
      
    }
  }
  $scope.resetHiddenControlsforAboriginalBand = function () {
    if ($scope.model.ParticipantInfoStep3ViewModel.AboriginalBand != 1) {
      $scope.model.ParticipantInfoStep3ViewModel.LiveOnReserve = null;
    }
  }

  $scope.resetHiddenControlsforCanadaImmigrant = function () {
    if ($scope.model.ParticipantInfoStep3ViewModel.CanadaImmigrant != 1) {
      $scope.model.ParticipantInfoStep3ViewModel.YearToCanada = 0;
    }
  }
  $scope.resetHiddenControlsforCanadaRefugee = function () {
    if ($scope.model.ParticipantInfoStep3ViewModel.CanadaRefugee != 1) {
      $scope.model.ParticipantInfoStep3ViewModel.FromCountry = null;
    }
  }
  $scope.next = function () {
    var step = $scope.model.ParticipantInfoStep0ViewModel.Step;
    var endpoint = 'Step' + step;

    if (step > 1 && step < 6) {
      var model = $scope.model["ParticipantInfoStep" + step + "ViewModel"];
      switch (step) {
        case 2:
          var phone2 = model.Phone2AreaCode || model.Phone2Exchange || model.Phone2Number;

          model.DateOfBirth = $scope.getDate(angular.element('#birth-date-Year').val(), angular.element('#birth-date-Month').val(), angular.element('#birth-date-Day').val());
          model.SIN = model.SIN1 + model.SIN2 + model.SIN3;
          model.Phone1 = model.Phone1AreaCode + model.Phone1Exchange + model.Phone1Number;
          if (phone2)
            model.Phone2 = model.Phone2AreaCode + model.Phone2Exchange + model.Phone2Number;
          else
            model.Phone2 = null;

          model.AddressLine1 = document.getElementById("AddressLine1").value;
          model.City = document.getElementById("City").value;
          model.PostalCode = document.getElementById("PostalCode").value;
          model.RegionId = document.getElementById("RegionId").value.split(":").filter(r => r != "?").pop();

          break;
        case 4:
          if ($scope.model.ParticipantInfoStep0ViewModel.ReportedByApplicant) {
            endpoint = 'Form';
          }
          break;
        case 5:
          endpoint = 'Form';
          break;
      }

      return $scope.load({
        url: '/Part/Information/' + endpoint,
        method: 'POST',
        data: $scope.model,
        set: 'model'
      })
        .then(function (response) {
          if (response.data.RedirectUrl) window.location = response.data.RedirectUrl;

          $scope.goto(1);
          window.scrollTo(0, 0);
        })
        .catch(angular.noop);
    }
  }

  $scope.goto = function (increment) {
    return $timeout(function () {
      var step = $scope.model.ParticipantInfoStep0ViewModel.Step + increment;
      if (step == 5)
        $scope.model.ParticipantInfoStep5ViewModel.ParticipantConsentBody = $sce.trustAsHtml($scope.model.ParticipantInfoStep5ViewModel.ParticipantConsentBody);
      if (step > 1 && step < 6)
        $scope.model.ParticipantInfoStep0ViewModel.Step = step;
    });
  }

  $scope.cancel = function () {
    return $scope.confirmDialog('Decline', 'All information entered will be erased.\nAre you sure you want to proceed?')
      .then(function () {
        window.location = '/Part/Information/Declined';
      })
      .catch(angular.noop);
  }

  $scope.selectedEmploymentStatus = function () {
    var status = $scope.model.ParticipantInfoStep4ViewModel.EmploymentStatus;
    for (var item in $scope.model.ParticipantInfoStep4ViewModel.EmploymentStatuses) {
      if (item.Key == status)
        return item.Value;
    }
    return null;
  }

  $scope.calculatedYear = function (d, n) {
    if (d && d.getFullYear)
      return d.getFullYear() + n;
    else
      return null;
  };

  $scope.calculatedMonth = function (d, n) {
    if (d && d.getMonth)
      return d.getMonth() + 1 + n;
    else
      return null;
  };

  $scope.calculatedDay = function (d, n) {
    if (d && d.getDate)
      return d.getDate() + n;
    else
      return null;
  };

  /**
   * Open the modal file uploaded.
   * @function openAttachmentModal
   * @param {any} title - The title of the modal window.
   * @param {any} attachment - The attachment to update/add.
   * @returns {Promise} modal
   */
  function openAttachmentModal(title, attachment) {
    return ngDialog.openConfirm({
      template: '/content/dialogs/_TrainingProviderAttachment.html',
      data: {
        title: title,
        attachment: attachment
      },
      controller: ['$scope', function ($scope) {
        /**
         * Return the selected file in the promise.
         * @function save
         * @returns {Promise}
         **/
        $scope.save = function () {
          $scope.confirm($scope.ngDialogData.attachment);
        };

        /**
         * Manually call the file select.
         * @function chooseFile
         * @returns {void}
         **/
        $scope.chooseFile = function () {
          var $input = angular.element('#training-provider-upload');
          $input.trigger('click');
        }

        /**
         * Set the selected file as the active attachment.
         * @function fileChanged
         * @param {any} $files
         * @returns {void}
         */
        $scope.fileChanged = function ($files) {
          if ($files.length) {
            $scope.ngDialogData.attachment.File = $files[0];
            $scope.ngDialogData.attachment.FileName = $scope.ngDialogData.attachment.File.name;
          }
        }
      }]
    });
  }

  /**
   * Open modal file uploader popup and then add the new file to the model.
   * @function addAttachment
   * @param {string} prop - The name of the property for this attachment.
   * @returns {void}
   **/
  $scope.addAttachment = function () {
    openAttachmentModal('Consent Form', {
      Id: 0,
      FileName: '',
      Description: '',
      File: {}
    })
      .then(function (attachment) {
        $scope.model.ConsentForm = attachment;
        saveAttachment();
      })
      .catch(angular.noop);
  };

  /**
   * Open modal file uploader popup and allow user to updte the attachment and/or file.
   * @function changeAttachment
   * @param {string} prop - The name of the property for this attachment.
   * @returns {void}
   */
  $scope.changeAttachment = function () {
    openAttachmentModal('Consent Form', $scope.model.ConsentForm)
      .then(function (attachment) {
        $scope.model.ConsentForm = attachment;
        saveAttachment();
      })
      .catch(angular.noop);
  };

  /**
   * Save the Consent Form.
   * @function saveProvider
   * @returns {void}
   */
  function saveAttachment() {
    $scope.model.ParticipantInfoStep0ViewModel.HasConsentForm = false;
    $scope.ajax({
      url: '/Part/Information/Attachment',
      method: 'POST',
      data: function () {
        var files = [];
        if ($scope.model.ConsentForm && $scope.model.ConsentForm.File) {
          files.push($scope.model.ConsentForm.File);
          $scope.model.ConsentForm.Index = files.length - 1;
        }
        return {
          files: files,
          component: angular.toJson($scope.model)
        };
      },
      dataType: 'file'
    })
      .then(function (response) {
        return $timeout(function () {
          $scope.model.ParticipantInfoStep0ViewModel = response.data.ParticipantInfoStep0ViewModel;
        });
      })
      .catch(angular.noop);
  }
});
