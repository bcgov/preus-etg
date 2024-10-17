PRINT 'Alter NaIndustryClassificationSystems'

-- [NotificationTypes]
ALTER TABLE [dbo].[NaIndustryClassificationSystems]
ADD [NAICSVersion] INT NOT NULL DEFAULT 2012;

CREATE NONCLUSTERED INDEX [IX_NAICSVersion]
ON [dbo].[NaIndustryClassificationSystems]([NAICSVersion] ASC);
