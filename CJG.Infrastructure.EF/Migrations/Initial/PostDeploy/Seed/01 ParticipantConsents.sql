PRINT 'Inserting [ParticipantConsents]'
SET IDENTITY_INSERT [dbo].[ParticipantConsents] ON 
INSERT [dbo].[ParticipantConsents]
 ([Id], [IsActive], [Description]) VALUES
 (1, 1, 
N'<h2 class="subheader">Consent for Collection and Use of Personal Information</h2>

<p>All personal information collected here and by future surveys and testimonials and other information
related to your participation in a Canada-BC Job Grant training opportunity ("Personal Information")
is collected pursuant to sections 26(c) and 26(e) of the Freedom of Information and Protection of Privacy Act.
</p>

<p>
This information will be used for administrative and evaluation purposes, including to determine your eligibility for participation in the Canada-BC Job Grant program,
and, in the case of any Testimonial, may be used and disclosed to publicly promote the Canada-BC Job Grant program ("Purposes").
The Government of Canada ("Canada") provides funding for the Canada-BC Job Grant under the Canada-British Columbia Job Fund Agreement.
Personal Information will be used to create reports about Canada-BC Job Grant training in British Columbia that will be provided to Canada in aggregate form only.
No personally identifiable information will be disclosed to Canada for this purpose.
Personal information will be shared with BC Stats, BC''s central statistics agency, for the purpose of conducting outcome surveys with Canada-BC Job Grant participants.
All information provided to BC Stats is protected under the BC Statistics Act and is kept confidential.
If you have any questions about the use of this information, please contact the Program Manager, Canada-BC Job Grant at 1-877-952-6914, or by mail to:
</p>
<p class="well">
    Program Manager, Canada-BC Job Grant<br />
    2nd Floor, 1106 Cook Street<br />
    Victoria, BC, V8V 3Z9<br />
</p>
<p>
    Effective as of the date set out below, and in consideration of the opportunity for me to participate in Canada-BC Job Grant training, I certify that:
</p>
<ul>
    <li>all of the information that I will provide is accurate and complete;</li>
    <li>
        I understand that I am expected to complete my training and I must complete surveys in order for the employer to meet all of their obligations under their Canada-BC Job Grant Agreement; and,
    </li>
</ul>
<p>I consent to:</p>
<ul>
    <li>
        the collection and use of my Personal Information by the Province of British Columbia for purposes of accountability, quality assurance, research, and evaluation of the Canada-BC Job Grant; and,
    </li>
    <li>my Personal Information being used to contact me to conduct the Surveys and to request a Testimonial.</li>
</ul>')
SET IDENTITY_INSERT [dbo].[ParticipantConsents] OFF
