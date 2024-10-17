app.controller('GrantProgramDefinition', function ($scope, $attrs, $controller, $timeout, Utils) {
  $scope.section = {
    name: 'GrantProgramDefinition',
    save: {
      url: function () {
        return $scope.model.Id === 0 ? '/Int/Admin/Grant/Program' : '/Int/Admin/Grant/Program/Definition';
      },
      method: function () { return $scope.model.Id === 0 ? 'POST' : 'PUT' },
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.section.isLoaded;
    },
    onSave: function (response) {
      $scope.emit('update', { model: $scope.model });
      var program = {
        Id: $scope.model.Id,
        Caption: $scope.model.Name,
        State: $scope.model.State,
        ProgramType: $scope.model.ProgramTypeId
      };
      $scope.grantPrograms.push(program);
      $scope.$parent.section.selectedProgram = program;
    },
    onRefresh: function () {
    },
    onEdit: function () {
    },
    onCancel: function () {
      if ($scope.model.Id === 0) {
        return $scope.loadProgram();
      }
    }
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));

  $scope.$watch('$parent.model', function (newValue, oldValue) {
    $scope.model = newValue;
  });

  /**
   * Make AJAX request for program types
   * @function loadProgramTypes
   * @returns {Promise}
   **/
  function loadProgramTypes() {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Types',
      set: 'programTypes',
      condition: !$scope.programTypes || !$scope.programTypes.length
    });
  }

  /**
   * Make AJAX request for program configurations
   * @function loadProgramConfigurations
   * @returns {Promise}
   **/
  function loadProgramConfigurations() {
    return $scope.load({
      url: '/Int/Admin/Grant/Program/Configurations',
      set: 'programConfigurations',
      condition: !$scope.programConfigurations || !$scope.programConfigurations.length
    })
      .then(function () {
        $scope.programConfigurations.unshift({ Key: 0, Value: 'Create New' });
      });
  }
  
  /**
   * Initialize the data for the form
   * @function init
   * @returns {void{
   **/
  $scope.init = function () {
    return Promise.all([
      loadProgramTypes(),
      loadProgramConfigurations()
    ])
      .then(function () {
        $scope.section.isLoaded = true;
      })
      .catch(angular.noop);
  }

  /**
   * Get the program type caption that is selected.
   * @function programTypesCaption
   * @returns {string}
   **/
  $scope.programTypeCaption = function () {
    return $scope.programTypes.find(function (item) {
      return item.Key === $scope.model.ProgramTypeId;
    }).Value;
  }

  /**
   * Get the program configuration caption that is selected.
   * @function programTypesCaption
   * @returns {string}
   **/
  $scope.programConfigurationCaption = function () {
    return $scope.programConfigurations.find(function (item) {
      return item.Key === $scope.model.ProgramConfigurationId;
    }).Value;
  }
});
