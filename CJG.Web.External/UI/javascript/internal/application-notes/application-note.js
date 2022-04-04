app.controller('ApplicationNote', function ($scope, $controller, $timeout) {
    Object.assign({
        name: 'ApplicationNote',
        displayName: 'Application Note'
    }, $scope.section)

    angular.extend(this, $controller('Base', { $scope: $scope }));

    /**
     * Make AJAX request for the specified note.
     * @function loadNote
     * @param {any} id - The note id.
     * @returns {Promise}
     */
    function loadNote(id) {
        return $scope.load({
            url: '/Int/Application/Note/' + id,
            set: 'ngDialogData.note',
            overwrite: true
        }).then(function () {
            return $timeout(function () {
                $scope.ngDialogData.note.Content = $scope.ngDialogData.convertJson($scope.ngDialogData.note);
            });
        });
    }

    /**
     * Map attachment to $scope.ngDialogData.note
     * @function mapAttachmentToNote
     * @param {object} attachment - The attachment.
     * @returns {void}
     **/
    function mapAttachmentToNote(attachment) {
        $scope.ngDialogData.note.Attachment = attachment;
        $scope.ngDialogData.note.AttachmentDescription = attachment.Description;
        $scope.ngDialogData.note.AttachmentFileName = attachment.FileName;
        $scope.ngDialogData.note.AttachmentId = attachment.Id;
        $scope.ngDialogData.note.File = attachment.File;
    }

    /**
     * Initialize the form data.
     * @function init
     * @returns {Promise}
     **/
    function init() {
      if ($scope.ngDialogData.note.Id) {
        return loadNote($scope.ngDialogData.note.Id)
          .catch(angular.noop);
      }
    }

    /**
     * Make AJAX request to delete the specified note.
     * @function deleteNote
     * @returns {Promise}
     **/
    $scope.deleteNote = function() {
      return $scope.confirmDialog('Delete Note', 'Do you want to delete this note?')
        .then(function() {
          return $scope.ajax({
            url: '/Int/Application/Note/Delete',
            method: 'PUT',
            data: function() {
              return $scope.ngDialogData.note;
            }
          }).then(function() {
            $scope.ngDialogData.note.Id = 0;
            $scope.confirm($scope.ngDialogData.note);
          })
        }).catch(angular.noop);
    }

    /**
     * Make AJAX request to save the specified note.
     * @function saveNote
     * @returns {Promise}
     **/
    $scope.saveNote = function () {
        return $scope.ajax({
            url: '/Int/Application/Note',
            method: $scope.ngDialogData.note.Id === 0 ? 'POST' : 'PUT',
            dataType: 'file',
            data: function () {
                var note = $scope.ngDialogData.note;
                var file = note.Attachment ? note.Attachment.File : null;
                note.File = file;
                note.AttachmentId = note.Attachment ? note.Attachment.Id : note.AttachmentId;
                note.DateAdded = $scope.toPST(note.DateAdded, 'YYYY-MM-DD h:mm:ss a');
                return note;
            }
        }).then(function (response) {
            $scope.ngDialogData.note = null;
            return $scope.confirm(response.data);
        }).catch(angular.noop);
    }

    /**
     * Open attachment dialog to choose a new attachment.
     * @function changeAttachment
     * @returns {Promise}
     **/
    $scope.changeAttachment = function () {
        return $scope.attachmentDialog('Change Attachment', $scope.ngDialogData.note.Attachment)
          .then(function (attachment) {
              mapAttachmentToNote(attachment);
          })
          .catch(angular.noop);
    }

    /**
     * Open attachment dialog to add a new attachment.
     * @function addAttachment
     * @returns {Promise}
     **/
    $scope.addAttachment = function () {
        return $scope.attachmentDialog('Add Attachment')
          .then(function (attachment) {
              mapAttachmentToNote(attachment);
          })
          .catch(angular.noop);
    }

    /**
     * Open confirmation dialog before removing file from note.
     * @function deleteAttachment
     * @returns {Promise}
     **/
    $scope.deleteAttachment = function () {
        return $scope.confirmDialog('Delete Attachment', 'Do you want to delete this attachment?')
          .then(function () {
              $scope.ngDialogData.note.AttachmentId = null;
              $scope.ngDialogData.note.Attachment = null;
              $scope.ngDialogData.note.File = null;
              $scope.ngDialogData.note.AttachmentDescription = null;
              $scope.ngDialogData.note.AttachmentFileName = null;
          })
          .catch(angular.noop);
    }

    $scope.tinymceOptions = {
      plugins: 'link code autoresize preview fullscreen lists advlist anchor',
      toolbar: 'undo redo | bold italic | formatselect | alignleft aligncenter alignright | outdent indent | numlist bullist | anchor | preview | fullscreen | code ',
      forced_root_blocks: true,
      setup: function (ed) {
        ed.on('init', function (ed) {
          $('div.tox-tinymce-aux').css('z-index', '999999');
        });
      }
  };

    $(document).on('focusin', function (e) {
      if ($(e.target).closest(".mce-window").length)
        e.stopImmediatePropagation();
    });

    init();
});
