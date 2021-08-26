PRINT 'Inserting [ApplicantDeclarations]'
SET IDENTITY_INSERT [dbo].[ApplicantDeclarations] ON 
INSERT [dbo].[ApplicantDeclarations]
 ([Id], [IsActive], [Body]) VALUES
 (1, 1, N'<li>I certify that I am authorized to submit this Application and to make this declaration on behalf of the applicant referred to in this Application (the "Applicant");</li>
 <li>I acknowledge that I have read and understand the CJG criteria applicable to this Application, including the sample CJG Agreement, consisting of the Approval Letter, Schedule A and Schedule B (and the Program Requirements referred to therein), as made available by the Province of British Columbia at the link below<anonymousCJGsampleagreementURL>;</li>
 <li>I acknowledge that, as the terms and conditions of the CJG Agreement are subject to change from time to time, should this Application be approved, the CJG Agreement that will be sent to the Applicant for signature may materially differ from the sample CJG Agreement that was posted at the time this Application was submitted and I acknowledge that I (or another individual authorized by the Applicant) will be responsible for reviewing, understanding and agreeing to the terms and conditions as they appear at the time the Applicant enters into a CJG Agreement with the Province;</li>
 <li>I certify that all of the information provided on this Application is true and correct to the best of my knowledge and belief;</li>
 <li>I acknowledge and agree that checking the box below has the same legal effect as making this declaration under a hand-written signature; and </li>
 <li>I do hereby make this declaration on my own behalf and on behalf of the Applicant as of the date that this Application is submitted.</li>')
SET IDENTITY_INSERT [dbo].[ApplicantDeclarations] OFF