require('./document-template-preview');

app.controller('GrantProgramDocumentTemplates', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'GrantProgramDocumentTemplates',
    save: {
      url: '/Int/Admin/Grant/Program/Document/Templates',
      method: 'PUT',
      data: function () {
        $scope.model[$scope.section.template] = $scope.section.body;
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model.Id === $scope.$parent.model.Id && $scope.model.RowVersion && $scope.model.RowVersion === $scope.$parent.model.RowVersion;
    },
    onSave: function () {
      $scope.emit('update', { model: { RowVersion: $scope.model.RowVersion } });
      return $scope.changeTemplate();
    },
    onRefresh: function () {
      return loadTemplates().catch(angular.noop);
    },
    templates: []
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  /**
   * Make AJAX request for grant program templates.
   * @function loadTemplates
   * @returns {Promise}
   **/
  function loadTemplates() {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Templates/' + $scope.$parent.model.Id,
      set: 'model'
    })
      .then(function () {
        return $timeout(function () {
          $scope.section.templates = [
            { Key: 'ApplicantDeclarationTemplate', Caption: 'Applicant Declaration', Template: "GenerateApplicantDeclarationBody" },
            { Key: 'ApplicantCoverLetterTemplate', Caption: 'Applicant Coverletter', Template: "GenerateApplicantCoverLetterBody" },
            { Key: 'ApplicantScheduleATemplate', Caption: 'Applicant Schedule A', Template: "GenerateAgreementScheduleABody" },
            { Key: 'ApplicantScheduleBTemplate', Caption: 'Applicant Schedule B', Template: "GenerateAgreementScheduleBBody" },
            { Key: 'ParticipantConsentTemplate', Caption: 'Participant Consent', Template: "GenerateParticipantConsentBody" }
          ];
          $scope.section.template = 'ApplicantDeclarationTemplate';
          $scope.section.body = $scope.model['ApplicantDeclarationTemplate'];
        });
      });
  }

  /**
   * Initialize the data for the form.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadTemplates()
    ])
      .then(function () {
        $scope.section.isLoaded = true;
      })
      .catch(angular.noop);
  };

  $scope.changeTemplate = function (previousTemplate) {
    return $timeout(function () {
      if (previousTemplate) $scope.model[previousTemplate] = $scope.section.body;
      $scope.section.body = $scope.model[$scope.section.template];
    });
  };

  /**
   * Open a new tab and display the document.
   * @function preview
   * @returns {Promise}
   **/
  $scope.preview = function () {
    return $scope.ajax({
      url: '/Int/Admin/Grant/Program/Document/Template/Preview/View',
      method: 'POST',
      data: {
        grantProgramId: $scope.$parent.model.Id,
        Template: $scope.section.templates.find(o => { return o.Key === $scope.section.template; }).Template,
        Body: $scope.section.body
      }
    })
      .then(function (response) {
        var w = window.open('about:blank');
        w.document.open();
        w.document.write(response.data);
        w.document.close();
      })
      .catch(angular.noop);
  };
});
