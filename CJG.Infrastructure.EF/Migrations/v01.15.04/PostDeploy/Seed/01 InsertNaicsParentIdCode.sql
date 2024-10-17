PRINT 'Alter NaIndustryClassificationSystems'
DECLARE @ParentId31 INT, @ParentId44 INT, @ParentId48 INT;

SELECT @ParentId31 = Id FROM [dbo].[NaIndustryClassificationSystems] where NAICSVersion = 2017 and Code like '31-%';
SELECT @ParentId44 = Id FROM [dbo].[NaIndustryClassificationSystems] where NAICSVersion = 2017 and Code like '44-%';
SELECT @ParentId48 = Id FROM [dbo].[NaIndustryClassificationSystems] where NAICSVersion = 2017 and Code like '48-%';

update NaIndustryClassificationSystems 
set Parentid = @ParentId31
where ParentId is null and NAICSVersion = 2017 and level = 2 AND Code LIKE '3%';

update NaIndustryClassificationSystems 
set Parentid = @ParentId44
where ParentId is null and NAICSVersion = 2017 and level = 2 AND (Code LIKE '44%' OR Code LIKE '45%');

update NaIndustryClassificationSystems 
set Parentid = @ParentId48 
where ParentId is null and NAICSVersion = 2017 and level = 2 AND (Code LIKE '48%' OR Code LIKE '49%');

PRINT 'done NaIndustryClassificationSystems'