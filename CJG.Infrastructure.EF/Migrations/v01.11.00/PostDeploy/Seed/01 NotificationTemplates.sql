PRINT 'Update [NotificationTemplates]'

UPDATE nte
SET nte.EmailBody = '<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p>Your training provider change request for Grant Agreement #@Model.FileNumber, "@Model.TrainingProgramTitle", has been denied for the following reason:</p>@Raw(Model.CRReason)<p>You may Login at: <a href="@Model.BaseURL">@Model.BaseURL</a> to submit another request.</p><p>@Model.ProgramName Team</p></body></html>'
FROM dbo.[NotificationTemplates] nte
INNER JOIN dbo.[NotificationTypes] nt ON nte.Id = nt.NotificationTemplateId
WHERE nt.[Id] = 15 -- Change Request Denied.


UPDATE nte
SET nte.EmailBody = '<!DOCTYPE html><html><head/><body><p>Dear @Model.RecipientFirstName,</p><p >Your training provider change request for Grant Agreement #@Model.FileNumber, "@Model.TrainingProgramTitle", has been accepted.</p>@Raw(Model.CRReason)<p>Please Login at: <a href="@Model.BaseURL">@Model.BaseURL</a> and review the change to your grant agreement.</p></body><p>@Model.ProgramName Team</p></html>'
FROM dbo.[NotificationTemplates] nte
INNER JOIN dbo.[NotificationTypes] nt ON nte.Id = nt.NotificationTemplateId
WHERE nt.[Id] = 14 -- Change Request Approved.
