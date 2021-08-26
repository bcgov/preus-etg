app.controller('CompletionReportView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'CompletionReportView',
    onRefresh: function () {
      return loadCompletionReport().catch(angular.noop);
    }
  };

  $scope.participants = [];
  $scope.ESSs = [];
  $scope.serviceLines = [];
  $scope.serviceTable = { headerWidth: 0, contentWidth: 0 };
  $scope.step = 0;

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for ESSs data
   * @function loadESSs
   * @returns {Promise}
   **/
  function loadESSs() {
    return $scope.load({
      url: '/Ext/Reporting/Completion/Report/ESS/' + $attrs.grantApplicationId,
      set: 'ESSs',
      condition: !$scope.ESSs || !$scope.ESSs.length
    });
  }

  /**
   * Make AJAX request and load NAICS data
   * @function loadNAICS
   * @param {object} participant - The participant.
   * @param {int} level - The NAICS level.
   * @param {int} [parentId] - The Id of the prior level.
   * @returns {Promise}
   **/
  function loadNAICS(participant, level, parentId) {
    if (!level) level = 1;
    return $scope.load({
      url: '/Ext/Reporting/Completion/Report/NAICS/' + level + '/' + (parentId ? parentId : ''),
      set: function (response) {
        participant['naics' + level] = response.data;
      },
      condition: level === 1 || parentId
    });
  }

  /**
   * Make AJAX request and load NOCs data
   * @function loadNOCs
   * @param {object} participant - The participant.
   * @param {int} level - The NOC level.
   * @param {int} [parentId] - The Id of the prior level.
   * @returns {Promise}
   **/
  function loadNOCs(participant, level, parentId) {
    if (!level) level = 1;
    return $scope.load({
      url: '/Ext/Reporting/Completion/Report/NOCs/' + level + '/' + (parentId ? parentId : ''),
      set: function (response) {
        participant['nocs' + level] = response.data;
      },
      condition: level === 1 || parentId
    });
  }

  /**
   * Make AJAX request to load completion report data.
   * @function loadCompletionReport
   * @returns {Promise}
   **/
  function loadCompletionReport() {
    return $scope.load({
      url: '/Ext/Reporting/Completion/Report/' + $attrs.grantApplicationId,
      set: 'model'
    });
  }

  /**
   * Fetch all the data for the form.
   * @function init
   * @returns {Promise}
   **/
  function init() {
    return Promise.all([
      loadESSs(),
      loadCompletionReport()
    ])
      .then(function () {
        $scope.setCompletionReportStep(1);
        for (let i = 0; i < $scope.ESSs.ServiceCategories.length; i++) {
          var serviceCategory = $scope.ESSs.ServiceCategories[i];
          $scope.serviceLines = $scope.serviceLines.concat(serviceCategory.ServiceLines);
        }

        var tableWidth = document.querySelectorAll(".completion-report--steps")[0].clientWidth;
        var participantWidth = 0;

        for (let i = 0; i < $scope.model.Participants.length; i++) {
          var participant = $scope.model.Participants[i];
          var width = calculateTextWidth(participant.FirstName + ' ' + participant.LastName, "0.9375rem Calibri, Candara, Segoe, Optima, sans serif");
          participantWidth = Math.max(participantWidth, width);
        }

        participantWidth = Math.max(participantWidth + 31, tableWidth * 0.1);
        participantWidth = Math.ceil(Math.min(participantWidth, tableWidth * 0.45));

        $scope.serviceTable.headerWidth = participantWidth;
        $scope.serviceTable.contentWidth = tableWidth - participantWidth - 5;
      })
      .catch(angular.noop);
  }

  /**
   * calculate the text width
   * @param {string} text - The text.
   * @param {string} font - The font.
   * @returns {number} text width
   */
  function calculateTextWidth(text, font) {
    this.element = document.createElement('canvas');
    this.context = this.element.getContext("2d");
    this.context.font = font;
    return Math.ceil(this.context.measureText(text).width);
  }

  /**
   * Set completion report step.
   * @function setCompletionReportStep
   * @param {int} number - The increase or decrease of step index.
   * @returns {void}
   **/
  $scope.setCompletionReportStep = function (number) {
    var from = $scope.step;
    var to = $scope.step + number;
    if (to > 0 && to <= $scope.model.CompletionReportGroupIds.length) {
      $scope.step = to;
      if (to > from) {
        var model = $scope.model.CompletionReportGroups[from - 1];
        switch (from) {
          case 1: {
            var question = model.Questions[0];
            var answer = question.Level1Answers[0];
            $scope.participants = [];
            for (let i = 0; i < $scope.model.Participants.length; i++)
              if (answer.BoolAnswer || !question.Level2Answers[i].BoolAnswer)
                $scope.participants.push($scope.model.Participants[i]);
            break;
          }
          case 2: {
            for (let i = 0; i < $scope.model.Participants.length; i++) {
              var participant = $scope.model.Participants[i];
              participant.completed = false;
            }
            break;
          }
        }
      }
      loadCompletionReportStep();
    }
  };

  /**
   * Make AJAX request to load next completion report step data.
   * @function gotoCompletionReportStep
   * @param {object} event - The event that fired.
   * @param {int} number - The increase or decrease of step index.
   * @param {boolean} saveOnly - If the request is save only.
   * @returns {void}
   **/
  $scope.gotoCompletionReportStep = function (event, number, saveOnly) {
    saveCompletionReportStep(event, saveOnly)
      .then(function () {
        $timeout(function () {
          $scope.setCompletionReportStep(number || 1);
        });
      })
      .catch(angular.noop);
  };

  /**
   * validate the completion report step data.
   * @function validateCompletionReportStep
   * @returns {void}
   **/
  $scope.validateCompletionReportStep = function () {
    var model = $scope.model.CompletionReportGroups[$scope.step - 1];
    if (!model)
      return false;
    var participants = $scope.getParticipants();
    switch (model.Id) {
      case 1:
      case 2: {
        if (!participants.length)
          return true;
        let question = model.Questions[0];
        let participantSelected = question.Level1Answers[0].BoolAnswer;
        if (!participantSelected || question.HasParticipantOutcomeReporting) {
          for (let i = 0; i < participants.length; i++) {
            let participant = participants[i];
            let answer = $scope.getParticipantAnswer(question.Level2Answers, participant.Id);
            if (answer.BoolAnswer) {
              participantSelected = true;
              if (!answer.IntAnswer || $scope.answerDisplayOther(question.Level2Options, answer.IntAnswer) && !answer.StringAnswer)
                return false;
            } else if (question.HasParticipantOutcomeReporting) {
              if (!answer.Noc4Id || !answer.Naics3Id || !answer.EmployerName)
                return false;
            }
          }
          return participantSelected;
        }
        break;
      }
      case 3: {
        let question = model.Questions[0];
        for (let i = 0; i < participants.length; i++) {
          let participant = participants[i];
          let answer = $scope.getParticipantAnswer(question.Level1Answers, participant.Id);
          if (!answer.IntAnswer)
            return false;
        }
        break;
      }
      case 4: {
        for (let i = 0; i < model.Questions.length; i++) {
          let question = model.Questions[i];
          let answer = question.Level1Answers[0];
          if (!answer.IntAnswer && !answer.StringAnswer)
            return false;
        }
        break;
      }
      case 5: {
        for (let i = 0; i < participants.length; i++) {
          let participant = participants[i];
          if (!participant.completed)
            return false;
        }
        break;
      }
    }
    return true;
  };

  /**
   * Make AJAX request to save completion report step data.
   * @function saveCompletionReportStep
   * @param {object} event - The event that fired.
   * @param {boolean} saveOnly - If the request is save only.
   * @returns {void}
   **/
  function saveCompletionReportStep(event, saveOnly) {
    var model = $scope.model.CompletionReportGroups[$scope.step - 1];
    model.SaveOnly = saveOnly;
    return $scope.ajax({
      url: '/Ext/Reporting/Completion/Report',
      method: 'POST',
      data: model
    });
  }

  /**
   * Make AJAX request to load completion report step data.
   * @function loadCompletionReportStep
   * @returns {void}
   **/
  function loadCompletionReportStep() {
    var completionReportGroupId = $scope.model.CompletionReportGroupIds[$scope.step - 1];

    if ($scope.model.CompletionReportGroups.length < $scope.step) {
      $scope.ajax({
        url: '/Ext/Reporting/Completion/Report/Group/' + $attrs.grantApplicationId + '/' + completionReportGroupId
      })
        .then(function (response) {
          $timeout(function () {
            var model = response.data;
            setCompletionReportStep(model);
            $scope.model.CompletionReportGroups.push(model);
          });
        })
        .catch(angular.noop);
    } else {
      var model = $scope.model.CompletionReportGroups[$scope.step - 1];
      setCompletionReportStep(model);
    }
  }

  /**
   * Initial setup for the question group.
   * @function setCompletionReportStep
   * @param {object} group - The question group.
   * @returns {void}
   **/
  function setCompletionReportStep(group) {
    $scope.title = group.Title;
    $scope.description = null;

    switch (group.Id) {
      case 1:
        $scope.description = "Please report completion for your participants below. Your completion report helps the Ministry measure program effectiveness and your satisfaction for program planning and improvement. Not reporting completion may impact your ability to use the Community Workforce Response Grant again.";
        break;
      case 2:
        for (let i = 0; i < $scope.participants.length; i++) {
          let participant = $scope.participants[i];
          let answer = $scope.getParticipantAnswer(group.Questions[0].Level2Answers, participant.Id);
          for (let x = 1; x <= 5; x++)
            loadNAICS(participant, x, x > 1 ? answer["Naics" + (x - 1) + "Id"] : 0);
          for (let x = 1; x <= 4; x++)
            loadNOCs(participant, x, x > 1 ? answer["Noc" + (x - 1) + "Id"] : 0);
        }
        break;
      case 3:
        group.allQuestion = group.Questions[0];
        break;
    }
  }

  /**
   * Make an AJAX request to fetch the next NAICS dropdown data for the selected parent NAICS.
   * @function changeNAICS
   * @param {object} participant - The participant.
   * @param {object} answers - The participant answers.
   * @param {int} level - The level to load.
   */
  $scope.changeNAICS = function (participant, answers, level) {
    let answer = $scope.getParticipantAnswer(answers, participant.Id);
    for (let x = level; x <= 5; x++) {
      answer["Naics" + x + "Id"] = null;
      participant['naics' + x] = [];
    }
    loadNAICS(participant, level, level > 1 ? answer["Naics" + (level - 1) + "Id"] : 0);
  };

  /**
   * Make an AJAX request to fetch the next NOC dropdown data for the selected parent NOC.
   * @function changeNOC
   * @param {object} participant - The participant.
   * @param {object} answers - The participant answers.
   * @param {int} level - The level to load.
   */
  $scope.changeNOC = function (participant, answers, level) {
    let answer = $scope.getParticipantAnswer(answers, participant.Id);
    for (let x = level; x <= 4; x++) {
      answer["Noc" + x + "Id"] = null;
      participant['nocs' + x] = [];
    }
    loadNOCs(participant, level, level > 1 ? answer["Noc" + (level - 1) + "Id"] : 0);
  };

  /**
   * Trigger the child level answers.
   * @function triggerChildLevelAnswers
   * @param {object} answers - The participant answers.
   * @param {boolean} trigger - Trigger the child level answers.
   */
  $scope.triggerChildLevelAnswers = function (answers, trigger) {
    if (trigger)
      for (let i = 0; i < answers.length; i++)
        answers[i].BoolAnswer = false;
  };

  /**
   * Trigger the parent level answer.
   * @function triggerParentLevelAnswer
   * @param {object} answer - The participant answer.
   * @param {boolean} trigger - Trigger the parent level answer.
   */
  $scope.triggerParentLevelAnswer = function (answer, trigger) {
    if (trigger)
      answer.BoolAnswer = false;
  };

  /**
   * Display the participant's answer.
   * @function displayAnswer
   * @param {object} options - The participant answer options.
   * @param {object} answer - The participant answer.
   * @returns {string}
   **/
  $scope.displayAnswer = function (options, answer) {
    for (let i = 0; i < options.length; i++) {
      let option = options[i];
      if (option.Id === answer)
        return option.Answer;
    }

    return null;
  };

  /**
   * Apply the NOC and NAICS to all employed participants.
   * @function applyToAllEmployed
   * @param {object} participant - The participant.
   * @param {object} answers - The participant answers.
   */
  $scope.applyToAllEmployed = function (participant, answers) {
    $scope.confirmDialog('Warning', 'Are you sure you want to copy this participant’s NOC, employer name and NAICS data to all employed participants?').then(function () {
      var answer = $scope.getParticipantAnswer(answers, participant.Id);
      for (let i = 0; i < $scope.participants.length; i++) {
        var _participant = $scope.participants[i];
        if (_participant.Id !== participant.Id) {
          var _answer = $scope.getParticipantAnswer(answers, _participant.Id);
          if (!_answer.BoolAnswer) {
            _answer.EmployerName = answer.EmployerName;
            for (let level = 1; level <= 5; level++) {
              _answer["Naics" + level + "Id"] = answer["Naics" + level + "Id"];
              loadNAICS(_participant, level, level > 1 ? _answer["Naics" + (level - 1) + "Id"] : 0);
              if (level < 5) {
                _answer["Noc" + level + "Id"] = answer["Noc" + level + "Id"];
                loadNOCs(_participant, level, level > 1 ? _answer["Noc" + (level - 1) + "Id"] : 0);
              }
            }
          }
        }
      }
    });
  };
  
  /**
   * Get the participants for the question group.
   * @function getParticipants
   * @param {string} pageKeyword - The search filter keyword.
   * @returns {object}
   **/
  $scope.getParticipants = function (pageKeyword) {
    var model = $scope.model.CompletionReportGroups[$scope.step - 1];
    var participants = $scope.participants;
    switch (model.Id) {
      case 1:
      case 5:
        participants = $scope.model.Participants;
        break;
    }
    if (!pageKeyword)
      return participants;
    var result = [];
    for (let i = 0; i < participants.length; i++) {
      var participant = participants[i];
      if (~participant.FirstName.indexOf(pageKeyword) || ~participant.LastName.indexOf(pageKeyword))
        result.push(participant);
    }
    return result;
  };

  /**
   * Get the participant answer for the participant.
   * @function getParticipantAnswer
   * @param {object} answers - The participant answers.
   * @param {int} participantId - Participant id.
   * @returns {object}
   **/
  $scope.getParticipantAnswer = function (answers, participantId) {
    for (let i = 0; i < answers.length; i++) {
      var answer = answers[i];
      if (answer.ParticipantFormId === participantId)
        return answer;
    }
    return null;
  };

  /**
   * Toggle all participant's boolean answers.
   * @function toggleAllAnswers
   * @param {object} answer - The participant parent answer.
   * @param {object} answers - The participant answers.
   * @returns {void}
   **/
  $scope.toggleAllAnswers = function (answer, answers) {
    var toggle = !$scope.allAnswersSelected(answers);
    answer.BoolAnswer = !toggle;
    for (let i = 0; i < answers.length; i++) {
      let item = answers[i];
      item.BoolAnswer = toggle;
    }
  };

  /**
   * Check if all participant's boolean answers are checked.
   * @function allAnswersSelected
   * @param {object} answers - The participant answers.
   * @returns {boolean}
   **/
  $scope.allAnswersSelected = function (answers) {
    for (let i = 0; i < answers.length; i++) {
      let item = answers[i];
      if (!item.BoolAnswer)
        return false;
    }
    return true;
  };

  /**
   * Check if the participant's answer needs to display other box.
   * @function answerDisplayOther
   * @param {object} options - The participant answer options.
   * @param {object} answer - The participant answer.
   * @returns {boolean}
   **/
  $scope.answerDisplayOther = function (options, answer) {
    for (let i = 0; i < options.length; i++) {
      var option = options[i];
      if (option.Id === answer)
        return option.DisplayOther;
    }

    return false;
  };

  /**
   * get participant eligible cost breakdown.
   * @function getParticipantEligibleCostBreakdown
   * @param {object} question - The question.
   * @param {int} participantId - The participant id.
   * @param {int} eligibleCostBreakdownId - The eligible cost breakdown id.
   * @returns {boolean}
   **/
  $scope.getParticipantEligibleCostBreakdown = function (question, participantId, eligibleCostBreakdownId) {
    for (let i = 0; i < question.Level1Answers.length; i++) {
      var answer = question.Level1Answers[i];
      if (answer.ParticipantFormId === participantId && answer.EligibleCostBreakdownId === eligibleCostBreakdownId)
        return true;
    }
    return false;
  };

  /**
   * update participant eligible cost breakdown.
   * @function updateParticipantEligibleCostBreakdown
   * @param {object} question - The question.
   * @param {int} participantId - The participant id.
   * @param {int} eligibleCostBreakdownId - The eligible cost breakdown id.
   * @returns {void}
   **/
  $scope.updateParticipantEligibleCostBreakdown = function (question, participantId, eligibleCostBreakdownId) {
    var model = null;
    for (let i = 0; i < question.Level1Answers.length; i++) {
      var answer = question.Level1Answers[i];
      if (answer.ParticipantFormId === participantId && answer.EligibleCostBreakdownId === eligibleCostBreakdownId) {
        model = answer;
        break;
      }
    }
    if (model)
      question.Level1Answers.splice(question.Level1Answers.indexOf(model), 1);
    else
      question.Level1Answers.push({
        GrantApplicationId: $attrs.grantApplicationId,
        ParticipantFormId: participantId,
        EligibleCostBreakdownId: eligibleCostBreakdownId
      });
  };

  /**
   * Parse the question text.
   * @function parseQuestionText
   * @param {string} text - The text.
   * @returns {void}
   **/
  $scope.parseQuestionText = function (text) {
    return Utils.replaceAll(Utils.replaceAll(text, '@Model.ProgramName', $scope.model.ProgramName), '\\n', '<br />');
  };

  /**
   * Apply the group answer to all questions in the group.
   * @function applyToAllAnswers
   * @param {object} group - The question group.
   * @returns {void}
   **/
  $scope.applyToAllAnswers = function (group) {
    $scope.confirmDialog('Warning', 'This will change the most important outcome for all participants in the table below.').then(function () {
      for (let i = 0; i < group.Questions.length; i++) {
        var question = group.Questions[i];
        for (let j = 0; j < $scope.participants.length; j++) {
          var participant = $scope.participants[j];
          for (let k = 0; k < question.Level1Answers.length; k++) {
            var answer = question.Level1Answers[k];
            if (answer.ParticipantFormId === participant.Id)
              question.Level1Answers[k].IntAnswer = i > 0 ? 0 : group.allAnswer;
          }
        }
      }
    });
  };

  /**
   * update and clear the participant's child level outcome reasons.
   * @function updateOutcomeReason
   * @param {object} group - The question group.
   * @param {int} participantId - The participant id.
   * @param {int} index - The question index.
   * @returns {void}
   **/
  $scope.updateOutcomeReason = function (group, participantId, index) {
    if ($scope.getParticipantAnswer(group.Questions[index].Level1Answers, participantId).IntAnswer === 0)
      for (let i = index + 1; i < group.Questions.length; i++)
        $scope.getParticipantAnswer(group.Questions[i].Level1Answers, participantId).IntAnswer = 0;
  };

  /**
   * Make AJAX request to save completion report data.
   * @function saveCompletionReport
   * @param {object} event - The event that fired.
   * @param {string} url - The overriding redirect url.
   * @returns {void}
   **/
  $scope.saveCompletionReport = function (event, url) {
    saveCompletionReportStep(event, url ? true : false)
      .then(function () {
        window.location = url || '/Ext/Reporting/Completion/Report/Details/' + $attrs.grantApplicationId;
      })
      .catch(angular.noop);
  };

  init();
});
