PRINT 'Fixing [StreamObjects], [StreamCriterias]'

-- Workforce Training
UPDATE dbo.StreamObjectives
SET Description = '%3Cp%3EAre%20you%20applying%20under%20the%20correct%20stream%3F%20Failure%20to%20apply%20under%20the%20correct%20stream%20will%20result%20in%20a%20denied%20application%20and%20the%20need%20to%20reapply.%3C%2Fp%3E%3Cp%3EDo%20you%20require%20soft%20skills%20training%2C%20management%20skills%20training%2C%20or%20business%20improvement%20skills%20training%3F%20%20Please%20visit%20%3CA%20href%3D%22https%3A%2F%2Fwww.workbc.ca%2FEmployer-Resources%2FCanada-BC-Job-Grant%2FWorkforce-Training-Stream.aspx%22%20target%3D%22_blank%22%3EWorkforce%20Training%3C%2FA%3E%20for%20further%20eligibility%20requirements.%20Before%20you%20apply%20please%20make%20sure%20you%20have%20selected%20the%20correct%20stream.%3C%2Fp%3E'
WHERE Id = 21

UPDATE dbo.StreamCriterias
SET Description = '%3Cp%3EAre%20you%20applying%20under%20the%20correct%20stream%3F%20Failure%20to%20apply%20under%20the%20correct%20stream%20will%20result%20in%20a%20denied%20application%20and%20the%20need%20to%20reapply.%3C%2Fp%3E%3Cp%3EDo%20you%20require%20soft%20skills%20training%2C%20management%20skills%20training%2C%20or%20business%20improvement%20skills%20training%3F%20%20Please%20visit%20%3CA%20href%3D%22https%3A%2F%2Fwww.workbc.ca%2FEmployer-Resources%2FCanada-BC-Job-Grant%2FWorkforce-Training-Stream.aspx%22%20target%3D%22_blank%22%3EWorkforce%20Training%3C%2FA%3E%20for%20further%20eligibility%20requirements.%20Before%20you%20apply%20please%20make%20sure%20you%20have%20selected%20the%20correct%20stream.%3C%2Fp%3E'
WHERE Id = 13

-- Technical Skills Training
UPDATE dbo.StreamObjectives
SET Description = '%3Cp%3EAre%20you%20applying%20under%20the%20correct%20stream%3F%20Failure%20to%20apply%20under%20the%20correct%20stream%20will%20result%20in%20a%20denied%20application%20and%20the%20need%20to%20reapply.%3C%2Fp%3E%3Cp%3EAre%20you%20learning%20how%20to%20use%20software%2C%20a%20program%2C%20or%20piece%20of%20machinery%20or%20vehicle%3F%20%20Please%20visit%20%3CA%20href%3D%22https%3A%2F%2Fwww.workbc.ca%2FEmployer-Resources%2FCanada-BC-Job-Grant%2FTechnical-Skills-Training-Stream.aspx%22%20target%3D%22_blank%22%3ETechnical%20Skills%20Training%3C%2FA%3E%20for%20further%20eligibility%20requirements.%20Before%20you%20apply%20please%20make%20sure%20you%20have%20selected%20the%20correct%20stream.%3C%2Fp%3E'
WHERE Id = 20

UPDATE dbo.StreamCriterias
SET Description = '%3Cp%3EAre%20you%20applying%20under%20the%20correct%20stream%3F%20Failure%20to%20apply%20under%20the%20correct%20stream%20will%20result%20in%20a%20denied%20application%20and%20the%20need%20to%20reapply.%3C%2Fp%3E%3Cp%3EAre%20you%20learning%20how%20to%20use%20software%2C%20a%20program%2C%20or%20piece%20of%20machinery%20or%20vehicle%3F%20%20Please%20visit%20%3CA%20href%3D%22https%3A%2F%2Fwww.workbc.ca%2FEmployer-Resources%2FCanada-BC-Job-Grant%2FTechnical-Skills-Training-Stream.aspx%22%20target%3D%22_blank%22%3ETechnical%20Skills%20Training%3C%2FA%3E%20for%20further%20eligibility%20requirements.%20Before%20you%20apply%20please%20make%20sure%20you%20have%20selected%20the%20correct%20stream.%3C%2Fp%3E'
WHERE Id = 12

-- Foundational Training
UPDATE dbo.StreamObjectives
SET Description = '%3Cp%3EAre%20you%20applying%20under%20the%20correct%20stream%3F%20Failure%20to%20apply%20under%20the%20correct%20stream%20will%20result%20in%20a%20denied%20application%20and%20the%20need%20to%20reapply.%3C%2Fp%3E%3Cp%3EDo%20you%20need%20industry%20or%20sector%20certification%2C%20apprenticeship%20or%20trades%20training%2C%20or%20essential%20skills%20training%3F%20Please%20visit%20%3CA%20href%3D%22https%3A%2F%2Fwww.workbc.ca%2FEmployer-Resources%2FCanada-BC-Job-Grant%2FFoundational-Training-Stream.aspx%22%20target%3D%22_blank%22%3EFoundation%20Training%3C%2FA%3E%20for%20further%20eligibility%20requirements.%20%20Before%20you%20apply%20please%20make%20sure%20you%20have%20selected%20the%20correct%20stream.%3C%2Fp%3E'
WHERE Id = 19

UPDATE dbo.StreamCriterias
SET Description = '%3Cp%3EAre%20you%20applying%20under%20the%20correct%20stream%3F%20Failure%20to%20apply%20under%20the%20correct%20stream%20will%20result%20in%20a%20denied%20application%20and%20the%20need%20to%20reapply.%3C%2Fp%3E%3Cp%3EDo%20you%20need%20industry%20or%20sector%20certification%2C%20apprenticeship%20or%20trades%20training%2C%20or%20essential%20skills%20training%3F%20Please%20visit%20%3CA%20href%3D%22https%3A%2F%2Fwww.workbc.ca%2FEmployer-Resources%2FCanada-BC-Job-Grant%2FFoundational-Training-Stream.aspx%22%20target%3D%22_blank%22%3EFoundation%20Training%3C%2FA%3E%20for%20further%20eligibility%20requirements.%20%20Before%20you%20apply%20please%20make%20sure%20you%20have%20selected%20the%20correct%20stream.%3C%2Fp%3E'
WHERE Id = 14