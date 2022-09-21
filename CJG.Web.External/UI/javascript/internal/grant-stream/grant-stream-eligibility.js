app.filter('unsafe', function ($sce) { return $sce.trustAsHtml; });

app.controller('GrantStreamEligibility', function ($scope, $attrs, $controller) {
  $scope.section = {
    name: 'GrantStreamEligibility',
    save: {
      url: function () {
        return '/Int/Admin/Grant/Streams/UpdateGrantStreamQuestions';
      },
      method: 'PUT',
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    onSave: function () {
      $scope.emit('refresh');
    },
    loaded: function () {
      return $scope.section.isLoaded;
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.tinymceOptions = {
    plugins: 'link image code autoresize preview fullscreen lists advlist anchor',
    toolbar: 'undo redo | bold italic | formatselect | alignleft aligncenter alignright | outdent indent | numlist bullist | anchor | preview | fullscreen | code',
    setup: function (ed) {
      ed.on('init', function (ed) {
        $('div.tox-tinymce-aux').css('z-index', '99999');
      });
    }
  };

  $scope.$watch('$parent.model', function (newValue, oldValue) {
    $scope.model = newValue;
  });

  /**
 * Initialize the data for the form
 * @function init
 * @returns {void{
 **/
  $scope.init = function () {
    if(typeof $scope.model.EligibilityQuestionsShowDisabled === 'undefined')
      $scope.model.EligibilityQuestionsShowDisabled = true;
    return Promise.resolve();
  }
  /**
   * Open a new tab and display the message.
   * @function preview
   * @returns {Promise}
   **/
  $scope.preview = function () {
    return $scope.ajax({
      url: '/Int/Admin/Grant/Stream/Preview',
      method: 'POST',
      data: {
        title: $scope.model.Name,
        message: $scope.model.EligibilityRequirements
      }
    })
      .then(function (response) {
        var w = window.open('about:blank');
        w.document.open();
        w.document.write(response.data);
        w.document.close();
      })
      .catch(angular.noop);
  }

  /**
   * Create a stream question.
   * @function createStreamQuestion
   * @returns 
   */
  $scope.createStreamQuestion = function () {
    $scope.model.StreamQuestions.unshift({
      Id: 0,
      GrantStreamId: $scope.model.Id,
      EligibilityRequirements: "",
      EligibilityQuestion: "",
      IsActive: true,
      EligibilityPositiveAnswerRequired: true,
      EligibilityRationaleAnswerAllowed: false,
      EligibilityRationaleAnswerLabel: '',
      RowSequence: 0
    });
    $scope.renumberStreamQuestions(1);
  }

  /**
   * Renumber all the questions.
   * Take current list and number by [increment]
   * @function renumberStreamQuestions
   * @returns 
   */
  $scope.renumberStreamQuestions = function (increment) {
    $scope.model.StreamQuestions.sort(function (a, b) { return a.RowSequence - b.RowSequence });
    for (var aIdx = 0; aIdx < $scope.model.StreamQuestions.length; aIdx++) {
      $scope.model.StreamQuestions[aIdx].RowSequence = (aIdx + 1) * increment;
    }
  }

  /**
 * Move a question up or down in the list
 * @function moveStreamQuestion
 * @returns 
 */
  $scope.moveStreamQuestion = function (question, up) {
    // Renumber by 100's. To move up or down, simply add or subtract 101, then renumber.
    $scope.renumberStreamQuestions(100);
    if (up === 1)
      question.RowSequence -= 101;
    else
      question.RowSequence += 101;
    $scope.renumberStreamQuestions(1);
  }
  /**
 * Filter for grid- show Active questions
 * @function filterQuestions
 * @returns true if question should display
 */
  $scope.filterQuestions = function (question) {
    if (!question)
      return false;

    if ($scope.model.EligibilityQuestionsShowDisabled || question.IsActive)
      return true;

    return false;
  }
});
