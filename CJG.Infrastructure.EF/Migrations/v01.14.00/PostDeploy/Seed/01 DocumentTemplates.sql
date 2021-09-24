PRINT 'Updating [DocumentTemplates]'

-- Fixing syntax error that occurs when parsing the Razor syntax. Hopefully for the last time.
UPDATE dbo.[DocumentTemplates]
SET [Body] = REPLACE([Body], '@(Model.GrantProgramCode)@("@gov.bc.ca")', '@Model.GrantProgramEmail')
WHERE [Body] LIKE '%@(Model.GrantProgramCode)@%'

UPDATE dbo.[DocumentTemplates]
SET	[Body] = REPLACE([Body], '@(Model.GrantProgramCode)@gov.bc.ca', '@Model.GrantProgramEmail')
WHERE [Body] LIKE '%@(Model.GrantProgramCode)@%'

UPDATE dbo.[DocumentTemplates]
SET	[Body] = REPLACE([Body], '@(Model.GrantProgramCode) + "@gov.bc.ca"', '@Model.GrantProgramEmail')
WHERE [Body] LIKE '%@(Model.GrantProgramCode) + "@gov.bc.ca"%'

UPDATE dbo.[DocumentTemplates]
SET	[Body] = REPLACE([Body], '@(Model.GrantProgramCode + "@gov.bc.ca")', '@Model.GrantProgramEmail')
WHERE [Body] LIKE '%@(Model.GrantProgramCode + "@gov.bc.ca")%'

CHECKPOINT
