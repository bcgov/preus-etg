PRINT 'Updating [NotificationTemplates] - Fix Payment Request'

UPDATE dbo.[NotificationTemplates]
SET emailbody = '<!DOCTYPE html><html><head><body>
@if (Model.isPayment) {
	<p>Dear @Model.RecipientFirstName,</p>
	<p>Your reimbursement claim for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle," has been processed and a reimbursement in the amount of @Model.ReimbursementPayment will be sent to you by electronic funds transfer or by a cheque mailed to the business mailing address you submitted with your application.</p>
	<p>If you have any questions, please contact us at <a href="mailto:@(Model.ProgramCode)@("@gov.bc.ca")">@(Model.ProgramCode)@("@gov.bc.ca")</a></p><p>@Model.ProgramName Team</p>
} else {
	<p>Dear @Model.RecipientFirstName,<br/><br/></p>
	<p>Your reimbursement claim for Grant Agreement #@Model.FileNumber, "@Model.ProgramTitle," has been reviewed, and an overpayment in the amount of @Model.ReimbursementPayment has been made.  
	Please mail a cheque made payable to the "Minister of Finance", and send to the following address within 14 days:</p>
	<p>Ministry of Advanced Education, Skills & Training</p>
	<p>Attention: Finance Unit</p>
	<p>PO Box 9189 Stn Prov Govt</p>
	<p>Victoria BC  V8W 9E6<br/><br/></p>
	<p>If you have any questions, please contact us at <a href="mailto:@(Model.ProgramCode)@("@gov.bc.ca")">@(Model.ProgramCode)@("@gov.bc.ca")</a><br/><br/></p>
	<p>Community Workforce Grant</p>
}
</body></html>',
AlertCaption = '@Model.ProgramName File #@Model.FileNumber Reimbursement Claim Notification',
EmailSubject = '@Model.ProgramName File #@Model.FileNumber Reimbursement Claim Notification'
WHERE Id = 38 OR Id = 18
