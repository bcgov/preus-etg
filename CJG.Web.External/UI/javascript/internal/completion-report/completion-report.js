app.controller('CompletionReportView', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'CompletionReportView',
    onRefresh: function () {
      return loadCompletionReport().catch(angular.noop);
    }
  };

  $scope.ESSs = null;
  $scope.serviceLines = [];
  $scope.serviceTable = { headerWidth: 0, contentWidth: 0 };

  angular.extend(this, $controller('Section', { $scope, $attrs }));

  /**
   * Make AJAX request for ESSs data
   * @function loadESSs
   * @returns {Promise}
   **/
  function loadESSs() {
    return $scope.load({
      url: '/Int/Application/Completion/Report/ESS/' + $attrs.grantApplicationId,
      set: 'ESSs',
      condition: !$scope.ESSs || !$scope.ESSs.length
    });
  }

  /**
   * Make AJAX request to load completion report data.
   * @function loadCompletionReport
   * @returns {Promise}
   **/
  function loadCompletionReport() {
    return $scope.load({
      url: '/Int/Application/Completion/Report/' + $attrs.grantApplicationId,
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
        
        return $timeout(function () {
          $scope.serviceTable.headerWidth = participantWidth;
          $scope.serviceTable.contentWidth = tableWidth - participantWidth - 5;
        });
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
   * get participant eligible cost breakdown.
   * @function getParticipantEligibleCostBreakdown
   * @param {object} answer - The participant answer.
   * @param {int} eligibleCostBreakdownId - The eligible cost breakdown id.
   * @returns {boolean}
   **/
  $scope.getParticipantEligibleCostBreakdown = function (answer, eligibleCostBreakdownId) {
    for (let i = 0; i < answer.EligibleCostBreakdownIds.length; i++) {
      if (answer.EligibleCostBreakdownIds[i] === eligibleCostBreakdownId)
        return true;
    }
    return false;
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

  init();
});
