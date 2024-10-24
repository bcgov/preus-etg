PRINT 'INSERT [DocumentTemplates] - CWRG - Cover Letter'
SET IDENTITY_INSERT [dbo].[DocumentTemplates] ON 

GO
INSERT [dbo].[DocumentTemplates] ([Id], [DocumentType], [Title], [Body], [IsActive], [DateAdded], [DateUpdated], [Description]) VALUES (12, 1, N'Default CWRG Applicant Cover Letter Template', N'@model CJG.Core.Entities.GrantApplication
<div style="float: left;">@CJG.Core.Entities.AppDateTime.UtcNow.ToString("MMMM dd, yyyy")</div>
<div style="float: right;">Application# @Model.FileNumber</div>
<br clear="all" />
<br />@string.Format("{0} {1}", Model.ApplicantFirstName, Model.ApplicantLastName)
<br />@Model.Organization.LegalName
<br />@Model.ApplicantPhysicalAddress
<br />@Model.ApplicantPhoneNumber
<br />
<br />
<p>Attention:</p>
<p>
    Based upon your application for funding under the @Model.GrantOpening.GrantStream.GrantProgram.Name
    <strong>("@Model.GrantOpening.GrantStream.GrantProgram.ProgramCode")</strong>,
    we are pleased to inform you that we have approved the Training Provider, Training and Eligible Training
    Costs set out in Schedule A &ndash; Grant Services. Please refer to Schedule B for additional defined terms used in this
    <strong>"Approval Letter"</strong> (Schedule B governs if there is an inconsistency between provisions).


    we are pleased to inform you that we have approved your application and the Program Services and Eligible Program Delivery Costs set out in Schedule A &ndash;
    Grant Services (<b>"Schedule A"</b>). Please refer to Schedule B – Definitions and General Terms (<b>"Schedule B"</b>)
    for additional defined terms used in this <b>"Approval Letter"</b> (Schedule B governs in the event of any inconsistency).
</p>

<p>
    After you have acknowledged acceptance of them,
    the following provisions of this Approval Letter, along with Schedule A and Schedule B,
    will form a Shared Cost Arrangement agreement (<b>"Agreement"</b>) between you and the Province.
    When the Agreement is in place, your primary obligations and the funding Claim process will be as follows:
</p>
<ol class="list--numbered">
    <li>
        <p>
            You must arrange and pay for all Program Services delivered by Third Party Service Providers.  , Other than applicable Participant Financial Supports, you are not entitled to provide any of the Program Services directly to Participants.
        </p>
    </li>
    <li>
        <p>
            Subject to the terms of this Agreement, we will reimburse and/or pre-pay you for up to 100 percent of the Eligible Program Delivery Costs up to the amount(s) shown in Schedule A (“Reimbursement”).
        </p>
    </li>
    <li>
        <p>
            Unless otherwise directed by us, you must use the Skills Training Grant System to electronically submit Claims and any Reports or other supporting documentation reasonably required by us. No Reimbursement will be made until we have received a complete Claim.
        </p>
    </li>
    <li>
        <p>
            Reimbursement payments will normally be made within 30 days following our receipt of a Claim.  Any costs or expenses that are not reimbursed by us will remain your responsibility.
        </p>
    </li>
    <li>
        <p>
            Claims made for Eligible Program Delivery Costs are subject to audit and verification by us at any time. You must keep original Receipts, Invoices and any proof of expenditure records, along with records documenting the provision of Program Services, and make them available for review by us for a minimum period of seven years following the end of the Term.  If any eligible expenses were pre-paid through a Reimbursement, copies of Receipts for all such expenses must be submitted to the Province within 30 days following the date that each such expense was incurred.

        </p>
    </li>
    <li>
        <p>
            You are responsible for any costs or expenses, including Eligible Program Delivery Costs, that are:
            <ol class="list--lettered">
                <li>
                    not identified in Schedule A;

                </li>
                <li>
                    over the maximum amount to be reimbursed by us under this Agreement
                </li>
                <li>
                    at any time deemed by us to be ineligible, due to Participant or Third Party Service Provider ineligibility; or
                </li>
                <li>
                    not supported by Receipts or other records in accordance with section 5 above.
                </li>
            </ol>

        </p>
    </li>
    <li>
        <p>
            Participants must:
            <ol class="list--lettered">
                <li>
                    be at least 15 years old at the time of entry into the Program;
                </li>
                <li>
                    be Canadian citizens, permanent residents or protected persons entitled to work in Canada; and
                </li>
                <li>
                    meet any other eligibility requirements applicable to the particular @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program funding stream under which the Program is being funded, as identified in the   @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements.
                </li>
            </ol>

        </p>
    </li>


    <li>
        <p>
            You are responsible for ensuring that Participants meet the above eligibility requirements before they enter the Program and that they submit Participant Information Forms using the Skills Training Grant System. If any Participants are found to be ineligible, the amount of the Reimbursement will be prorated accordingly and, in the event of pre-payment, the amount pre-paid for any ineligible Participants will become an Overpayment.
        </p>
    </li>
    <li>
        <p>
            All or any part of any Reimbursement made to you, including administrative fees (?) may later be deemed by us to be an Overpayment for any of the following reasons:
            <ol class="list--lettered">
                <li>
                    the Program was not delivered in whole or in part in accordance with the Schedule A;
                </li>
                <li>
                    the Program is not completed by the end of the Term;
                </li>
                <li>
                    a Participant did not complete the program as outlined in the Schedule A;
                </li>
                <li>
                    a Participant did not achieve employment upon completion of the program;
                </li>
                <li>
                    your Claim included any items that were not Eligible Program Delivery Costs;
                </li>
                <li>
                    you fail to provide or ensure the provision of any Receipts or Report(s) that we require; or
                </li>
                <li>
                    you receive funding or any refund from any other person or entity, including another government or governmental body, that reimburses you for any of the Eligible Program Delivery Costs.
                </li>
            </ol>
        </p>
    </li>
    <li>
        <p>
            At the end of the Term, any outstanding Overpayments will become a debt owing to the Province and must be repaid by you within 14 days.
        </p>
    </li>
    <li>
        <p>
            Within 30 days of the end of the entire Program, you must submit a Participant Completion Report.
        </p>
    </li>
    <li>
        <p>
            For greater certainty, you will have the autonomy, flexibility and discretion to deliver the “In-Scope Services” set out in Schedule A in a manner that aligns with each Participant’s needs and, with respect to the funds identified in Schedule A for Employment Assistance Services and Participant Financial Supports, to allocate and expend those funds across these two Program Services areas in a manner that you believe will best support each Participant’s needs.
        </p>
    </li>
    <li>
        <p>
            You must comply with all parts of this Agreement (including Schedule B), the   @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program Requirements and all applicable laws.
        </p>
    </li>
    <li>
        <p>
            We reserve the right to contact Participants, Third Party Service Providers or any other person in order to substantiate Reimbursement requests, Program Services activities, records or other matters pertaining to your obligations under or your participation in the   @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program.
        </p>
    </li>

</ol>
<p>
    If the Terms of this Agreement are acceptable to you and you wish to proceed with the Program, you must first review and confirm that you have read and understand each part of the Agreement, including Schedule A and Schedule B.  You must then agree to be bound by the Agreement by electronically accepting the Agreement using the “Accept Agreement” button on the Review and Accept Grant Agreement page.  If we do not receive your acceptance of the Agreement by the date set out in Schedule A, this approval will expire.
</p>
<p>
    If, after entering into the Agreement, you want to make a change to the Program (other than as set out in the following paragraph) including making a change to a Third Party Service Provider, you must use the Skills Training Grant System to submit a change request. The change request must be approved by us before the Program Services begin. Schedule A describes how to submit a change request.
</p>
<p>
    Start and end dates for Skills Training Program may be modified in the Skills Training Grant System at any time without our prior approval as long as:
</p>
<ol class="list--numbered">
    <li>
        <p>
            	each of the components of the Program Services that has a start date identified in the “Delivery Plan” in Schedule A must begin within the Delivery Period, but not before the Term start date;
            	

        </p>
    </li>
    <li>
        <p>
            the applicable Skills Training Program or Employment Assistance Services end dates fall within the Term; and
        </p>
    </li>
    <li>
        <p>
            no other material changes are made to the Program, the Program Services or any Third Party Service Provider.
        </p>
    </li>
</ol>
<p>
  
    Thank you for your participation in the   @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program. The Government of British Columbia introduced the   @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program to assist communities, sectors and industries to respond to emerging, urgent labour market needs, providing skills and supports to unemployed and precariously employed British Columbians and connecting them to good-paying jobs in their communities.
</p>
<p>
    We are interested in your feedback and would appreciate learning from your experience with the  @Model.GrantOpening.GrantStream.GrantProgram.ProgramCode Program. Please contact us with any questions or concerns using the Contact Us link below.
</p>
<p>Sincerely,</p> Director
<br /> @Model.GrantOpening.GrantStream.GrantProgram.Name Program
<br />
<br />
<p>
    I am authorized to act and to enter into this Agreement on behalf of the Applicant. On the Applicant’s behalf, I do hereby accept and agree to all of the terms and conditions of this Agreement, including this Approval Letter and associated Schedule A and Schedule B.
</p>
', 1, CAST(N'2018-09-19T16:49:07.8837450' AS DateTime2), CAST(N'2018-09-19T16:49:07.8837450' AS DateTime2), NULL)
GO
SET IDENTITY_INSERT [dbo].[DocumentTemplates] OFF
GO
