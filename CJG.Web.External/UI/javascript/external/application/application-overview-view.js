var utils = require('../../shared/utils');
app.controller('ApplicationOverviewView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {

  $scope.section = {
    grantApplicationId: $attrs.ngGrantApplicationId,
  }

  angular.extend(this, $controller('Base', { $scope: $scope, $attrs: $attrs }));

  var types = {
    SELECTION: "Selection",
    DESCRIPTION: "Description",
    ASSOCIATEDPROVIDER: "AssociatedProvider",
    PROVIDER: "Provider",
    PROGRAM: "Program",
    COST: "Cost",
    ATTACHMENT: "Attachment",
    PIF: "PIF",
    BUSINESSCASE: "BusinessCase"
  }
  var classTypes = {
    NOTSTART: "label--notstarted",
    STARTED: "label--started",
    INCOMPLETE: "label--incomplete",
    INPROGRESS: "label--incomplete",
    COMPLETE: "label--complete"
  }
  var textTypes = {
    NOTSTART: "Not Started",
    STARTED: "Started",
    INCOMPLETE: "Incomplete",
    INPROGRESS: "In Progress",
    COMPLETE: "Complete",
    OPTIONAL: "Optional"
  }

  function loadApplication() {
    return $scope.load({
      url: '/Ext/Application/Overview/' + $scope.section.grantApplicationId,
      set: 'model'
    });
  }

 // function loadAlternateUsers() {
 //   return $scope.load({
 //    url: '/Ext/Application/AlternateUsers/' + $scope.section.grantApplicationId,
 //    set: 'alternateUsers'
 //  });
 //}

  function init() {
    return Promise.all([
      loadApplication()
      //loadAlternateUsers()
    ]).catch(angular.noop);
  }

  $scope.Populate = function (type, populateTypes) {
    if (!$scope.model)
      return;

    if (type === types.SELECTION) {
      if ($scope.model.ApplicationStateExternal === utils.ApplicationStateExternal.ApplicationWithdrawn ||
        ($scope.model.ApplicationStateExternal === utils.ApplicationStateExternal.Incomplete &&
          ($scope.model.GrantOpeningState === utils.GrantOpeningStates.Closed || !$scope.model.EligibilityConfirmed || !$scope.model.HasValidDate))) {
        return populateTypes.INCOMPLETE;
      }
    } else if (type === types.DESCRIPTION) {
      if (!$scope.model.ProgramDescription || !$scope.model.ProgramDescription.ApplicantType) {
        return populateTypes.NOTSTART;
      } else if ($scope.model.ProgramDescription.DescriptionState === utils.DescriptionStates.Incomplete) {
        return populateTypes.INCOMPLETE;
      }
    } else if (type === types.ASSOCIATEDPROVIDER || type === types.PROVIDER) {
      if (!$scope.model.TrainingProvider) {
        return populateTypes.NOTSTART;
      } else if (($scope.model.TrainingProvider.TrainingProviderType.PrivateSectorValidationType === 1
        || ($scope.model.TrainingProvider.TrainingProviderType.PrivateSectorValidationType === 2
          && $scope.model.CheckPrivateSectorsOn && $scope.model.CheckPrivateSectorsOn <= $scope.model.DateSubmitted)
      ) && ($scope.IsInvalidTrainingProvider($scope.model.TrainingProvider))) {
        return populateTypes.INCOMPLETE;
      } else if ($scope.model.TrainingProvider.TrainingProviderState === utils.TrainingProviderStates.Incomplete) {
        return populateTypes.INCOMPLETE;
      }
    } else if (type === types.PROGRAM) {
      if (!$scope.model.TrainingProgram) {
        return populateTypes.NOTSTART;
      } else if ($scope.model.TrainingProgram.TrainingProgramState === utils.TrainingProgramStates.Incomplete) {
        return populateTypes.INCOMPLETE;
      }
    } else if (type === types.COST) {
      if (!$scope.model.TrainingCost || $scope.model.TrainingCost.TotalEstimatedCost === 0) {
        return populateTypes.NOTSTART;
      } else if ($scope.model.TrainingCost.TrainingCostState === utils.TrainingCostStates.Incomplete) {
        return populateTypes.INCOMPLETE;
      }
    } else if (type === types.ATTACHMENT) {
      if ($scope.model.AttachmentsState == 1) {
        return populateTypes.INCOMPLETE;
      } else if (!$scope.model.AttachmentsRequired && $scope.model.AttachmentsState === 0) {
        return populateTypes.OPTIONAL;
      } else if ($scope.model.AttachmentsState === 0) {
        return populateTypes.NOTSTART;
      }
    } else if (type === types.PIF) {
      // Grey No participants yet, Yellow some participants, Green all participants have signed up
      if (!$scope.model.Participants || $scope.model.Participants.length === 0) {
        if ($scope.model.ProgramType === 2) {  // CWRG
          return populateTypes.OPTIONAL;
        } else {
          return populateTypes.NOTSTART;
        }
      }
      else if ($scope.model.Participants.length < $scope.model.MaxParticipantsAllowed) {
        return populateTypes.INPROGRESS;
      }
      else {
        const totalNoOutcome = $scope.model.Participants.filter(p => p.ExpectedOutcome === 0).length;
        if (totalNoOutcome > 0)
          return populateTypes.INPROGRESS;

        return populateTypes.COMPLETE;
      }
    } else if (type === types.BUSINESSCASE) {
      if ($scope.model.BusinessCaseState == 1) {
        return populateTypes.INCOMPLETE;
      } else if (!$scope.model.BusinessCaseRequired && $scope.model.BusinessCaseState === 0) {
        return populateTypes.OPTIONAL;
      } else if ($scope.model.BusinessCaseState === 0) {
        return populateTypes.NOTSTART;
      }
    }
    return populateTypes.COMPLETE;
  }

  $scope.PopulateClass = function (type) {
    return $scope.Populate(type, classTypes);
  }

  $scope.PopulateText = function (type) {
    return $scope.Populate(type, textTypes);
  }

  $scope.PopulateSection = function (process, populateTypes) {
    if (process.ServiceCategoryTypeId === utils.ServiceTypes.SkillsTraining) {

      if (!process.TrainingPrograms || process.TrainingPrograms.length === 0) {
        return populateTypes.NOTSTART;
      }

      var matching = $.grep(process.TrainingPrograms, function (n, i) {
        return n.TrainingProgramState < 1 || $scope.IsInvalidTrainingProvider(n.AssociatedProvider);
      });

      if (matching.length > 0) {
        return populateTypes.INCOMPLETE;
      }
      return populateTypes.COMPLETE;

    } else if (process.ServiceCategoryTypeId === utils.ServiceTypes.EmploymentServicesAndSupports) {

      if (!process.AssociatedEligibleCost && !process.TrainingProvider && process.MinProvider == 0) {
        return populateTypes.OPTIONAL;
      }

      if (!process.AssociatedEligibleCost) {
        return populateTypes.NOTSTART;
      }

      if (process.MinProvider > process.TrainingProviders.length) {
        return populateTypes.INCOMPLETE;

      } else if (process.MaxProvider > 0) {
        if ((!process.TrainingProviders || process.TrainingProviders.length === 0) && process.AssociatedEligibleCost.EstimatedCost > 0) {
          return populateTypes.INCOMPLETE;
        }
        var matching = $.grep(process.TrainingProviders, function (n, i) {
          return n.TrainingProviderState < 1;
        });
        if (matching.length > 0
          || (process.AssociatedEligibleCost.EstimatedCost <= 0 && (!process.AssociatedEligibleCost.ServiceLines || process.AssociatedEligibleCost.ServiceLines.length === 0))
          || (process.AssociatedEligibleCost.EstimatedCost <= 0 && (process.AssociatedEligibleCost.ServiceLines || process.AssociatedEligibleCost.ServiceLines.length > 0))
          || (process.AssociatedEligibleCost.EstimatedCost > 0 && (!process.AssociatedEligibleCost.ServiceLines || process.AssociatedEligibleCost.ServiceLines.length === 0))) {
          if (process.MinProvider == 0 && process.TrainingProviders.length === 0 && process.AssociatedEligibleCost.EstimatedCost == 0) {
            return populateTypes.OPTIONAL;
          }
          return populateTypes.INCOMPLETE;
        }
      } else if (process.AssociatedEligibleCost.EstimatedCost === 0 && process.AssociatedEligibleCost.ServiceLines && process.AssociatedEligibleCost.ServiceLines.length === 0) {
        if (process.MinProvider == 0)
          return populateTypes.OPTIONAL;
        return populateTypes.NOTSTART;
      } else if ($scope.model.ApplicationStateExternal === utils.ApplicationStateExternal.ApplicationWithdrawn
        || (process.AssociatedEligibleCost.EstimatedCost <= 0 && process.AssociatedEligibleCost.ServiceLines && process.AssociatedEligibleCost.ServiceLines.length > 0)
        || (process.AssociatedEligibleCost.EstimatedCost > 0 && process.AssociatedEligibleCost.ServiceLines && process.AssociatedEligibleCost.ServiceLines.length === 0)) {
        return populateTypes.INCOMPLETE;
      }
      return populateTypes.COMPLETE;
    }
  }

  $scope.PopulateSectionClass = function (process) {
    return $scope.PopulateSection(process, classTypes);
  }

  $scope.PopulateSectionText = function (process) {
    return $scope.PopulateSection(process, textTypes);
  }

  $scope.PopulateSubSection = function (target, type, populateTypes) {
    let isProgram = type === types.PROGRAM;
    let isProvider = type === types.PROVIDER;

    if ((isProgram && target.TrainingProgramState === utils.TrainingProgramStates.Incomplete)
      || (isProvider && target.TrainingProviderState === utils.TrainingProviderStates.Incomplete)) {
      return populateTypes.INCOMPLETE;

    } else if ((isProgram && $scope.IsInvalidTrainingProvider(target.AssociatedProvider)) || (isProvider && $scope.IsInvalidTrainingProvider(target))) {
      return populateTypes.INCOMPLETE;

    } else if (isProgram || (isProvider && target.TrainingProviderState === utils.TrainingProviderStates.Complete)) {
      return populateTypes.COMPLETE;
    }

    return populateTypes.NOTSTART;
  }

  $scope.IsInvalidTrainingProvider = function(trainingProvider) {
    let requireCourseOutline = trainingProvider.TrainingProviderType.CourseOutline;
    let requireProofOfQualificationsDocument = trainingProvider.TrainingProviderType.ProofOfQualificationsDocument;
    let missingCourseOutline = requireCourseOutline && trainingProvider.CourseOutlineDocument === null;
    let missingProofOfQualifications = requireProofOfQualificationsDocument && trainingProvider.ProofOfQualificationsDocument === null;

    return trainingProvider.TrainingProviderType.PrivateSectorValidationType && (missingCourseOutline || missingProofOfQualifications);
  }

  $scope.PopulateSubSectionClass = function (target, type) {
    return $scope.PopulateSubSection(target, type, classTypes);
  }

  $scope.PopulateSubSectionText = function (target, type) {
    return $scope.PopulateSubSection(target, type, textTypes);
  }

  $scope.initEventBinding = function () {
    $('.panel-bar > .k-item').on('click', function (e) {
      if (e.currentTarget.dataset.state == 'disabled')
        e.stopPropagation();
    });

    $('.panel-bar__status a, .panel-bar__status input').on('click', function (e) {
      e.stopPropagation();
    });
  }

  /**
   * Delete the specified training provider or training program
   * // TODO: This needs to be rewritten to pass a model and not the rowversion as a query string parameter.
   * @function delete
   * @param {any} obj - The model of the entity to delete.
   * @param {any} type - The url path name.
   * @returns {Promise}
   */
  $scope.delete = function ($event, obj, type) {
    $event.preventDefault();
    $event.stopPropagation();
    return $scope.confirmDialog('Delete Confirmation', "Are you sure you want to delete the training " + type.toLocaleLowerCase() + "?")
      .then(function () {
        return $scope.ajax({
          url: '/Ext/Training/' + type + '/Delete?id=' + obj.Id + '&rowVersion=' + encodeURIComponent(obj.RowVersion),
          method: 'PUT',
          data: {}
        })
          .then(function (response) {
            return loadApplication();
          })
      })
      .catch(angular.noop);
  }

  /**
   * Delete the specified training provider or training program
   * // TODO: This needs to be rewritten to pass a model and not the rowversion as a query string parameter.
   * @function delete
   * @param {any} obj - The model of the entity to delete.
   * @param {any} type - The url path name.
   * @returns {Promise}
   */
  $scope.edit = function ($event) {
    $event.preventDefault();
    $event.stopPropagation();
    window.location = $event.currentTarget.href;
  }

  $scope.toggleSection = function ($event) {
    if ($event.currentTarget.parentElement.dataset.state == "disabled") {
      return;
    }
    var section = $event.currentTarget.nextElementSibling;
    var icon = $event.currentTarget.lastElementChild;
    if (section.classList.contains('ng-hide')) {
      section.classList.remove('ng-hide');
      section.classList.add('ng-show');
      icon.classList.remove("k-i-arrow-s");
      icon.classList.remove("k-panelbar-expand");
      icon.classList.add("k-i-arrow-n");
      icon.classList.add("k-panelbar-collapse");
    } else {
      section.classList.remove('ng-show');
      section.classList.add('ng-hide');
      icon.classList.remove("k-i-arrow-n");
      icon.classList.remove("k-panelbar-collapse");
      icon.classList.add("k-i-arrow-s");
      icon.classList.add("k-panelbar-expand");
    }
  }

  $scope.toggleHelper = function ($element, show, innerHTML, display) {
    $element.currentTarget.dataset.toggle = show;
    $element.currentTarget.firstElementChild.innerHTML = innerHTML;
    $element.currentTarget.nextElementSibling.style.display = display;
  }

  $scope.toggle = function ($element) {
    if ($element.currentTarget.dataset.toggle == 'hide') {
      $scope.toggleHelper($element, 'show', '&#9660;', "block");
    } else {
      $scope.toggleHelper($element, 'hide', '&#9654;', "none");
    }
  }

  $scope.AllParticipantsHaveOutcomes = function() {
    const currentParticipants = $scope.model.Participants.length;
    const participantsWithOutcome = $scope.model.Participants.filter(p => p.ExpectedOutcome !== 0).length;

    if (currentParticipants === participantsWithOutcome)
      return true;

    return false;
  }

  //$scope.changeApplicationContact = function () {
  //  const selectedNewUserId = $scope.model.SelectedNewUser;
  //  const selectedUser = $scope.alternateUsers.filter(a => a.Key === selectedNewUserId).pop().Value;

  //  if (selectedNewUserId === 0)
  //    return angular.noop;

  //  return $scope.confirmDialog('Change Application Contact', '<p>Please confirm that you\'d like to assign this application to <strong>' + selectedUser + '</strong>.</p>')
  //    .then(function () {
  //      return $scope.ajax({
  //        url: '/Ext/Application/AlternateUsers/ChangeUser/',
  //        method: 'PUT',
  //        data: {
  //          Id: $scope.section.grantApplicationId,
  //          RowVersion: $scope.model.RowVersion,
  //          ApplicantContactId: selectedNewUserId
  //        }
  //      });
  //    })
  //    .then(function (response) {
  //      if (response.data.RedirectURL)
  //        window.location = response.data.RedirectURL;
  //    })
  //    .catch(angular.noop);
  //}

  init();
});
