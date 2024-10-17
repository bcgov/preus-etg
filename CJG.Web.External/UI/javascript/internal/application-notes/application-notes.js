app.filter('unsafe', function ($sce) { return $sce.trustAsHtml; });

app.controller('ApplicationNotes', function ($scope, $attrs, $controller, $timeout, Utils, ngDialog) {
  $scope.section = {
    name: 'ApplicationNotes',
    displayName: 'Notes',
    save: {
      url: '/Int/Application/Note',
      method: function () {
        return $scope.model.Id == 0 ? 'POST' : 'PUT';
      },
      data: function () {
        return $scope.model;
      },
      backup: true
    },
    loaded: function () {
      return $scope.model && $scope.model.RowVersion 
        && (($scope.grantFile && $scope.model.RowVersion === $scope.grantFile.RowVersion)
        || ($scope.claim && $scope.model.RowVersion === $scope.claim.RowVersion)); 
    },
    onSave: function () {
      $scope.emit('update', { grantFile: { RowVersion: $scope.model.RowVersion } });
    },
    onRefresh: function () {
      return loadNotes().catch(angular.noop);
    },
    showAdd: true,
    grantApplicationId: $scope.parent ? $scope.parent.grantApplicationId : 0
  };

  angular.extend(this, $controller('Section', { $scope: $scope, $attrs: $attrs }));
  
  /**
   * Make AJAX request to get note types.
   * @function loadNoteTypes
   * @returns {Promise}
   **/
  function loadNoteTypes() {
    return $scope.load({
      url: '/Int/Application/Note/Types',
      set: 'noteTypes',
      condition: !$scope.noteTypes || !$scope.noteTypes.length,
      localCache: true
    });
  }

  /**
   * Make AJAX request to get note users.
   * @function loadNoteUsers
   * @returns {Promise}
   **/
  function loadNoteUsers() {
    if ($scope.section.grantApplicationId == null) {
      return;
    }
    return $scope.load({
      url: '/Int/Application/Note/Users/' + $scope.section.grantApplicationId,
      set: 'noteUsers'
    });
  }

  /**
   * Filter note according the dropdown
   * @function noteFilterChange
   * @returns {void}
   **/
  $scope.noteFilterChange = function() {
    $scope.model.filterNotes = $scope.model.Notes.filter(note => addTofilterNote(note));
  }

  /**
   * Make AJAX request to get application notes data.
   * @function loadNotes
   * @returns {Promise}
   **/
  function loadNotes() {
    if ($scope.section.grantApplicationId == null) {
      return;
    }

    return $scope.load({
      url: '/Int/Application/Notes/' + $scope.section.grantApplicationId,
      set: 'model'
    })
      .then(function () {
        loadNoteUsers();
        return $timeout(function () {
          $scope.model.Notes.forEach(function (note) {
            note.Content = $scope.convertJson(note);
          });
          $scope.noteFilterChange();
        });
      });
  }

  /**
   * Initialize form with data.
   * @function init
   * @returns {Promise}
   **/
  $scope.init = function () {
    return Promise.all([
      loadNoteTypes(),
      loadNotes()
    ]).catch(angular.noop);
  }

  /**
   * Download the attachment.
   * @function downloadAttachment
   * @param {any} $event
   * @returns {void}
   */
  $scope.downloadAttachment = function ($event, noteId, attachmentId) {
    $event.stopPropagation();
    window.open('/Int/Application/Note/' + noteId + '/Download/' + attachmentId);
  }

  /**
   * Open dialog to add a new note.
   * @function addNote
   * @returns {Promise}
   **/
  $scope.addNote = function () {
    return showDialog({
      Id: 0,
      GrantApplicationId: $scope.model.Id,
      DateAdded: new Date(),
      AllowEdit: true,
      IsCreator: true
    })
    .then(function (data) {
      return $timeout(function(){ 
        $scope.model.Notes.unshift(data); // Add.
        if (addTofilterNote(data)) {
            $scope.model.filterNotes.unshift(data);
        }
        if(!$scope.noteUsers.find(user => user.CreatorId === data.CreatorId)) {
          $scope.noteUsers.unshift({CreatorId: data.CreatorId, CreatorName: data.CreatorName});
        }
      });
    })
    .catch(angular.noop);
  }

  /**
   * Check whether filter the note.
   * @function addTofilterNote
   * @param {any} note
   * @returns {boolean}
   **/
  function addTofilterNote(note) {
    return !$scope.note 
    || ((!$scope.note.NoteTypeId || $scope.note.NoteTypeId === note.NoteTypeId) 
    && ($scope.note.CreatorId == undefined || $scope.note.CreatorId === note.CreatorId));
  }

  /**
   * Show the dialog to view/edit/add a note.
   * @function showDialog
   * @returns {Promise}
   **/
  function showDialog(note) {
    return ngDialog.openConfirm({
      template: '/Int/Application/Note/View/' + note.Id,
      data: {
        title: 'Application Note',
        note: note,
        noteTypes: $scope.noteTypes,
        maxUploadSize: $scope.model.MaxUploadSize,
        permittedAttachmentTypes: $scope.model.PermittedAttachmentTypes,
        convertJson: $scope.convertJson
      },
      controller: 'ApplicationNote'
    });
  }

  /**
   * Show dialog to view and edit a note.
   * @function openNote
   * @param {any} $event
   * @param {any} note
   * @returns {Promise}
   */
  $scope.openNote = function($event, note) {
    return showDialog(note)
      .then(function(data) {
        if (data.Id) {
          return $scope.sync(data, note);
        } else {
          return $timeout(function() {
            var index = $scope.model.Notes.indexOf(note);
            $scope.model.Notes.splice(index, 1);
            index = $scope.model.filterNotes.indexOf(note);
            $scope.model.filterNotes.splice(index, 1);
          });
        }
      })
      .catch(angular.noop);
    $event.stopPropagation();
  }

  $scope.toggleNote = function ($event, note, toggleState) {
      note.ShowNote = toggleState;
  }

  /**
   * Converts the JSON content into HTML.
   * Checks if it is JSON, if it isn't, it will return the original note.
   * Any failures will return the original note.
   * @function convertJson
   * @param {any} note - The text to convert.
   * @return {string}
   */
  $scope.convertJson = function(note) {
    if (note.Content.startsWith('{') || note.Content.startsWith('[')) {
      try {
        var result = '';
        var changeContentArray = JSON.parse(note.Content);
        changeContentArray.forEach(function (content) {
          result += '<div class="change-tracking"><label>' + content.name + ' (' + content.state + '):</label>';

          if (content.state === 'modified') {
            result += '<ul class="list-style-none">';
            content.changes.forEach(function (element) {
              switch (element.state) {
                case ('Added'):
                  result += '<li>&nbsp;' + element.name + ": added '" + element.newValue + "'</li>";
                  break;
                case ('Deleted'):
                  result += '<li>&nbsp;' + element.name + ": removed '" + element.oldValue + "'</li>";
                  break;
                case ('Modified'):
                default:
                  result += '<li>&nbsp;' + element.name + ": changed from '" + element.oldValue + "' to '" + element.newValue + "'</li>";
                  break;
              }
            });
            result += '</ul>';
          }

          if (content.state === 'added') {
            result += '<ul class="list-style-none">';
            content.changes.forEach(function (element) {
              if (element.name === 'Attachment') {
                switch (element.state) {
                  case ('Added'):
                    result += '<li>&nbsp;' + element.name + ": added '" + element.newValue + "'</li>";
                    break;
                  case ('Deleted'):
                    result += '<li>&nbsp;' + element.name + ": removed '" + element.oldValue + "'</li>";
                    break;
                  case ('Modified'):
                default:
                  result += '<li>&nbsp;' + element.name + ": changed from '" + element.oldValue + "' to '" + element.newValue + "'</li>";
                  break;
                }
              }
            });
            result += '</ul>';
          }
          result += '</div>';
        });
        return result;
      } catch(error) {
        return note.Content;
      }
    } else {
      return note.Content;
    }
  }
});

app.filter('filterExistNoteTypes', function () {
  return function (item, noteTypes, notes) {
   if (noteTypes == undefined || notes == undefined) {
     return noteTypes;
   }
   return noteTypes.filter(function(noteType) {
    return notes.filter(note => note.NoteTypeId === noteType.Id).length > 0
    });
  }
});
