-- Participant Consent
PRINT 'UPDATE [dbo].[DocumentTemplates] - ParticipantConsentTemplate'

UPDATE [dbo].[DocumentTemplates] 
SET body = N'
@model CJG.Core.Entities.GrantApplication
@{
    var program = Model.GrantOpening.GrantStream.GrantProgram;
}
<h2 class="subheader">Personal Information</h2>
<p>
    The Government of Canada ("Canada") provides funding for the @program.Name ("@program.ProgramCode") under the Canada-British Columbia Workforce Development Agreement (WDA), administered by the British Columbia Ministry of Advanced Education, Skills and Training ("AEST"), and under the Labour Market Development Agreement ("LMDA"), administered by the British Columbia Ministry of Social Development and Poverty Reduction ("SDPR").
</p>
<p>
    All personal information related to your participation in a @program.ProgramCode training opportunity ("Personal Information") is collected pursuant to sections 26(c) and 26(e) of the Freedom of Information and Protection of Privacy Act ("FOIPPA").  This information will be used for administrative, evaluation, research, accountability, and reporting purposes, including to determine your eligibility for participation in the @program.ProgramCode program and to meet federal reporting requirements under the WDA and LMDA.
</p>
<p>
    Under the authority of Section 33.1(d) and 33.2(d) of FOIPPA, the personal information collected may be disclosed to the Department of Employment and Skills Development Canada ("ESDC") or the Canada Employment Insurance Commission ("CEIC") for the purposes of administering the Employment Insurance Act.  The CEIC may also use your Personal Information for policy analysis, research or evaluation purposes.
</p>
<p>
    Under the authority of Section 3.6 of the LMDA, if you are currently in receipt of Employment Insurance (EI) benefits, a referral under Section 25 of the EI Act may be placed on your EI claim, to allow you to continue to receive EI benefits, up to the end of your EI benefit period, while you participate in the @Model.GrantOpening.GrantStream.GrantProgram.Name program.
</p>
<p>
    All @program.ProgramCode training participants are required to complete two satisfaction surveys at approximately 3 months and 12 months following completion of their training ("Surveys").  The Surveys will ask basic questions about the outcomes of training and whether the training met your employment needs.  Your contact information will be shared with British Columbia''s statistical agency, BC Stats, in order for them to contact you to conduct these Surveys.
</p>
<p>
    You may also be asked if you wish to, or you may volunteer to, provide a testimonial regarding your @program.ProgramCode training experience ("Testimonial").  Testimonials, and any Personal Information that you choose to include in a Testimonial, may be used and disclosed to the public to promote the @program.ProgramCode program.
</p>
<h2>Consent and Certification</h2>
<p>
    Effective as of the date set out below, and in consideration of the opportunity for me to participate in @program.ProgramCode training, I:
</p>
<ul>
    <li>consent to the collection use, and disclosure of my Personal Information for purposes set out above; </li>
    <li>consent to my Personal Information being used to contact me to conduct the Surveys and to request a Testimonial;</li>
    <li>certify that all of the information that I have provided in this form is accurate and complete; and</li>
    <li>certify that I understand that I am expected to complete my training and I must complete the Surveys in order for my employer to meet all of its @program.Name Agreement obligations;</li>
</ul>
<p>
    By confirming my consent, I acknowledge and agree that this typed signature has the same legal effect as a written signature.
</p>
<hr>
<p>
    If you have any questions about the collection, use or disclosure of your Personal Information, please contact the Program Manager, B.C. Employer Training Grant at 1-877-952-6914, or by mail at: Program Manager, @Model.GrantOpening.GrantStream.GrantProgram.Name, 2nd Floor, 1106 Cook Street, Victoria, BC, V8V 3Z9 or by submitting an email to <a href="mailto:@(program.ProgramCode)@("@")gov.bc.ca">@(program.ProgramCode)@("@")gov.bc.ca</a>
</p>'
WHERE Id = 5

-- Agreement Schedule A
PRINT 'UPDATE [dbo].[DocumentTemplates] - ApplicantScheduleATemplate'
UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
@model CJG.Core.Entities.GrantApplication
@using CJG.Core.Entities
@using System.Globalization
@{
    var showAgreedCosts = Model.TrainingPrograms.FirstOrDefault().GrantApplication.ApplicationStateInternal.ShowAgreedCosts();
}

@functions {
    public string ToCurrency(decimal number, int precision = 2, int currencyNegativePattern = 0)
    {
        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        nfi.CurrencyDecimalDigits = precision;
        nfi.CurrencyNegativePattern = currencyNegativePattern;
        return number.ToString("C", nfi);
    }
}

<div class="form--readonly">
    <div class="form__group two-col">
        <label class="form__label" for="FileNumber">File Number:</label>
        <div class="form__control">
            @Model.FileNumber
        </div>
    </div>
    <div class="form__group two-col">
        <label class="form__label" for="Organization_LegalName">Applicant Name:</label>
        <div class="form__control">
            @Model.Organization.LegalName
        </div>
    </div>
    <p><h3>Agreement Term</h3></p>
    <div class="form__group two-col">
        <label class="form__label" for="GrantAgreement_StartDate">Start Date:</label>
        <div class="form__control">
            @(Model.GrantAgreement.StartDate.ToLocalTime().ToString("MMMM dd, yyyy"))
        </div>
    </div>
    <div class="form__group two-col">
        <label class="form__label" for="GrantAgreement_EndDate">End Date:</label>
        <div class="form__control">
            @(Model.GrantAgreement.EndDate.ToLocalTime().ToString("MMMM dd, yyyy"))
        </div>
    </div>
    <p><h3>Delivery Plan</h3></p>
    <div class="form__group two-col">
        <label class="form__label" for="DefaultTrainingProgram_CourseTitle">Training Program:</label>
        <div class="form__control">
            @Model.TrainingPrograms.FirstOrDefault().CourseTitle
        </div>
    </div>
    <div class="form__group__wrapper">
        <div class="form__group two-col">
            <label class="form__label" for="DefaultTrainingProgram_TrainingProvider_Name">Training Provider:</label>
            <div class="form__control">
                @Model.TrainingPrograms.FirstOrDefault().TrainingProvider.Name
            </div>
        </div>
        ::RequestChangeTrainingProvider::
    </div>
    <div class="form__group__wrapper ">
        <div class="form__group two-col">
            <label class="form__label" for="DefaultTrainingProgram_StartDate">Training Start Date:</label>
            <div class="form__control">
                @(Model.StartDate.ToLocalTime().ToString("MMMM dd, yyyy"))
            </div>
        </div>
        ::RequestChangeTrainingDates::
    </div>
    <div class="form__group two-col">
        <label class="form__label" for="DefaultTrainingProgram_EndDate">Training End Date:</label>
        <div class="form__control">
            @(Model.EndDate.ToLocalTime().ToString("MMMM dd, yyyy"))
        </div>
    </div>
    <div class="form__group two-col">
        <label class="form__label" for="GrantAgreement_ParticipantReportingDueDate">Participant Reporting Due Date:</label>
        <div class="form__control">
            @(Model.GrantAgreement.ParticipantReportingDueDate.ToLocalTime().ToString("MMMM dd, yyyy"))
        </div>
    </div>
    <div class="form__group two-col">
        <div class="label-desc-group">
            <label class="form__label" for="GrantAgreement_ReimbursementClaimDueDate">
                Reimbursement Claim Due Date:
            </label>
            <br />
            <span class="text--small text--normal">To remain eligible for a grant, a Reimbursement Claim must be submitted on or before this date.</span>
        </div>
        <div class="form__control">
            @(Model.GrantAgreement.ReimbursementClaimDueDate.ToLocalTime().ToString("MMMM dd, yyyy"))
        </div>
    </div>
    <div class="form__group two-col">
        <label class="form__label" for="GrantAgreement_ReimbursementClaimDueDate">
            Number of Participants in Training Program:
        </label>
        <div class="form__control">
            @if (Model.TrainingCost.AgreedParticipants > 0)
            {
                @Model.TrainingCost.AgreedParticipants
            }
            else
            {
                @Model.TrainingCost.EstimatedParticipants
            }
        </div>
    </div>
    @* hide the edit/delete buttons when rendering to the application administrator *@
    <h3>Eligible Training Costs</h3>
    <table id="tblExpenses" class="table table--expenses js-table-expense-list">
        <thead>
            <tr>
                <th>Expense type</th>
                <th class="num-col">Number of participants</th>
                <th class="num-col">Cost per participant</th>
                <th class="num-col">Total training cost</th>
                <th class="num-col">Employer contribution</th>
                <th class="num-col">Requested Government Contribution</th>
            </tr>
        </thead>

        <tbody>
            @{
                var OrderedList = Model.TrainingCost.EligibleCosts.Where(ec => ec.AgreedMaxCost > 0).OrderBy(x => x.TrainingCost.GrantApplication.TrainingPrograms.FirstOrDefault().Id);
            }

            @foreach (var expenseItem in OrderedList)
            {
                if (showAgreedCosts)
                {
                    <tr data-expense-id="@expenseItem.EligibleExpenseType.Id"
                        data-expense-item-id="@expenseItem.Id"
                        data-total="@expenseItem.AgreedMaxCost.ToString()"
                        data-participants="@expenseItem.AgreedMaxParticipants.ToString()">
                        <td>@(expenseItem.EligibleExpenseType.Caption)</td>
                        <td class="num-col">@(expenseItem.AgreedMaxParticipants.ToString())</td>
                        <td class="num-col">@(ToCurrency(expenseItem.AgreedMaxParticipantCost))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.AgreedMaxCost))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.AgreedEmployerContribution))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.AgreedMaxReimbursement))</td>
                    </tr>
                }
                else
                {
                    <tr data-expense-id="@expenseItem.EligibleExpenseType.Id"
                        data-expense-item-id="@expenseItem.Id"
                        data-total="@expenseItem.EstimatedCost.ToString()"
                        data-participants="@expenseItem.EstimatedParticipants.ToString()">
                        <td>@(expenseItem.EligibleExpenseType.Caption)</td>
                        <td class="num-col">@(expenseItem.EstimatedParticipants.ToString())</td>
                        <td class="num-col">@(ToCurrency(expenseItem.EstimatedParticipantCost))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.EstimatedCost))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.EstimatedEmployerContribution))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.EstimatedReimbursement))</td>
                    </tr>
                }
            }
            <tr>
                <td><b>Totals</b></td>
                <td></td>
                <td></td>
                <td class="num-col">
                    <b>@(ToCurrency(Model.TrainingCost.TotalAgreedMaxCost))</b>
                </td>
                <td class="num-col">
                    <b>@(ToCurrency(Model.CalculateAgreedEmployerContribution()))</b>
                </td>
                <td class="num-col">
                    <b>@(ToCurrency(Model.CalculateAgreedMaxReimbursement()))</b>
                </td>
            </tr>
        </tbody>
    </table>
</div>
'
WHERE ID = 2;

-- Agreement Schedule A
PRINT 'UPDATE [dbo].[DocumentTemplates] - ApplicantScheduleATemplate'
UPDATE [dbo].[DocumentTemplates] 
SET [Body] = 
N'
@model CJG.Core.Entities.GrantApplication
@using CJG.Core.Entities
@using System.Globalization
@{
    var showAgreedCosts = Model.TrainingPrograms.FirstOrDefault().GrantApplication.ApplicationStateInternal.ShowAgreedCosts();
}

@functions {
    public string ToCurrency(decimal number, int precision = 2, int currencyNegativePattern = 0)
    {
        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        nfi.CurrencyDecimalDigits = precision;
        nfi.CurrencyNegativePattern = currencyNegativePattern;
        return number.ToString("C", nfi);
    }
}

<div class="form--readonly">
    <div class="form__group two-col">
        <label class="form__label" for="FileNumber">File Number:</label>
        <div class="form__control">
            @Model.FileNumber
        </div>
    </div>
    <div class="form__group two-col">
        <label class="form__label" for="Organization_LegalName">Applicant Name:</label>
        <div class="form__control">
            @Model.Organization.LegalName
        </div>
    </div>
    <p><h3>Agreement Term</h3></p>
    <div class="form__group two-col">
        <label class="form__label" for="GrantAgreement_StartDate">Start Date:</label>
        <div class="form__control">
            @(Model.GrantAgreement.StartDate.ToLocalTime().ToString("MMMM dd, yyyy"))
        </div>
    </div>
    <div class="form__group two-col">
        <label class="form__label" for="GrantAgreement_EndDate">End Date:</label>
        <div class="form__control">
            @(Model.GrantAgreement.EndDate.ToLocalTime().ToString("MMMM dd, yyyy"))
        </div>
    </div>
    <p><h3>Delivery Plan</h3></p>
    <div class="form__group two-col">
        <label class="form__label" for="DefaultTrainingProgram_CourseTitle">Training Program:</label>
        <div class="form__control">
            @Model.TrainingPrograms.FirstOrDefault().CourseTitle
        </div>
    </div>
    <div class="form__group__wrapper">
        <div class="form__group two-col">
            <label class="form__label" for="DefaultTrainingProgram_TrainingProvider_Name">Training Provider:</label>
            <div class="form__control">
                @Model.TrainingPrograms.FirstOrDefault().TrainingProvider.Name
            </div>
        </div>
        ::RequestChangeTrainingProvider::
    </div>
    <div class="form__group__wrapper ">
        <div class="form__group two-col">
            <label class="form__label" for="DefaultTrainingProgram_StartDate">Training Start Date:</label>
            <div class="form__control">
                @(Model.StartDate.ToLocalTime().ToString("MMMM dd, yyyy"))
            </div>
        </div>
        ::RequestChangeTrainingDates::
    </div>
    <div class="form__group two-col">
        <label class="form__label" for="DefaultTrainingProgram_EndDate">Training End Date:</label>
        <div class="form__control">
            @(Model.EndDate.ToLocalTime().ToString("MMMM dd, yyyy"))
        </div>
    </div>
    <div class="form__group two-col">
        <label class="form__label" for="GrantAgreement_ParticipantReportingDueDate">Participant Reporting Due Date:</label>
        <div class="form__control">
            @(Model.GrantAgreement.ParticipantReportingDueDate.ToLocalTime().ToString("MMMM dd, yyyy"))
        </div>
    </div>
    <div class="form__group two-col">
        <div class="label-desc-group">
            <label class="form__label" for="GrantAgreement_ReimbursementClaimDueDate">
                Reimbursement Claim Due Date:
            </label>
            <br />
            <span class="text--small text--normal">To remain eligible for a grant, a Reimbursement Claim must be submitted on or before this date.</span>
        </div>
        <div class="form__control">
            @(Model.GrantAgreement.ReimbursementClaimDueDate.ToLocalTime().ToString("MMMM dd, yyyy"))
        </div>
    </div>
    <div class="form__group two-col">
        <label class="form__label" for="GrantAgreement_ReimbursementClaimDueDate">
            Number of Participants in Training Program:
        </label>
        <div class="form__control">
            @if (Model.TrainingCost.AgreedParticipants > 0)
            {
                @Model.TrainingCost.AgreedParticipants
            }
            else
            {
                @Model.TrainingCost.EstimatedParticipants
            }
        </div>
    </div>
    @* hide the edit/delete buttons when rendering to the application administrator *@
    <h3>Eligible Training Costs</h3>
    <table id="tblExpenses" class="table table--expenses js-table-expense-list">
        <thead>
            <tr>
                <th>Expense type</th>
                <th class="num-col">Number of participants</th>
                <th class="num-col">Cost per participant</th>
                <th class="num-col">Total training cost</th>
                <th class="num-col">Employer contribution</th>
                <th class="num-col">Requested Government Contribution</th>
            </tr>
        </thead>

        <tbody>
            @{
                var OrderedList = Model.TrainingCost.EligibleCosts.Where(ec => ec.AgreedMaxCost > 0).OrderBy(x => x.TrainingCost.GrantApplication.TrainingPrograms.FirstOrDefault().Id);
            }

            @foreach (var expenseItem in OrderedList)
            {
                if (showAgreedCosts)
                {
                    <tr data-expense-id="@expenseItem.EligibleExpenseType.Id"
                        data-expense-item-id="@expenseItem.Id"
                        data-total="@expenseItem.AgreedMaxCost.ToString()"
                        data-participants="@expenseItem.AgreedMaxParticipants.ToString()">
                        <td>@(expenseItem.EligibleExpenseType.Caption)</td>
                        <td class="num-col">@(expenseItem.AgreedMaxParticipants.ToString())</td>
                        <td class="num-col">@(ToCurrency(expenseItem.AgreedMaxParticipantCost))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.AgreedMaxCost))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.AgreedEmployerContribution))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.AgreedMaxReimbursement))</td>
                    </tr>
                }
                else
                {
                    <tr data-expense-id="@expenseItem.EligibleExpenseType.Id"
                        data-expense-item-id="@expenseItem.Id"
                        data-total="@expenseItem.EstimatedCost.ToString()"
                        data-participants="@expenseItem.EstimatedParticipants.ToString()">
                        <td>@(expenseItem.EligibleExpenseType.Caption)</td>
                        <td class="num-col">@(expenseItem.EstimatedParticipants.ToString())</td>
                        <td class="num-col">@(ToCurrency(expenseItem.EstimatedParticipantCost))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.EstimatedCost))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.EstimatedEmployerContribution))</td>
                        <td class="num-col">@(ToCurrency(expenseItem.EstimatedReimbursement))</td>
                    </tr>
                }
            }
            <tr>
                <td><b>Totals</b></td>
                <td></td>
                <td></td>
                <td class="num-col">
                    <b>@(ToCurrency(Model.TrainingCost.TotalAgreedMaxCost))</b>
                </td>
                <td class="num-col">
                    <b>@(ToCurrency(Model.CalculateAgreedEmployerContribution()))</b>
                </td>
                <td class="num-col">
                    <b>@(ToCurrency(Model.CalculateAgreedMaxReimbursement()))</b>
                </td>
            </tr>
        </tbody>
    </table>
</div>
'
WHERE ID = 7;
