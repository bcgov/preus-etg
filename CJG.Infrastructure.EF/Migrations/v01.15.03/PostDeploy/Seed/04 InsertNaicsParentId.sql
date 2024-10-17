DECLARE @RootId INT, @Level INT, @MaxCount INT, @Counter INT, @Code NVARCHAR(100), @ParentId INT;

SELECT @RootId = [Id] FROM [dbo].[NaIndustryClassificationSystems] where NAICSVersion = 2017 and [Level]=0;

SELECT @RootId ;--root level

UPDATE [dbo].[NaIndustryClassificationSystems] 
SET [ParentId] = @RootId 
WHERE NAICSVersion = 2017 and [Level]=1;

DECLARE @tblLevel TABLE(Id INT NOT NULL identity(1,1), Code NVARCHAR(100), [Level] INT, LevelId INT)
SET @Level = 1;

--BEGIN TRANSACTION

WHILE(@Level <= 5)
BEGIN 

INSERT INTO @tblLevel (Code, [Level], LevelId)
SELECT Code, [Level], Id FROM [dbo].[NaIndustryClassificationSystems] where NAICSVersion = 2017 AND [Level] = @Level;

SELECT @MaxCount = MAX(Id) FROM @tblLevel;

--SELECT * FROM @tblLevel;

SET @Counter = 1;

	WHILE(@Counter <= @MaxCount)
	BEGIN
		SELECT @Code = Code, @ParentId = LevelId FROM @tblLevel where Id = @Counter;

		UPDATE [dbo].[NaIndustryClassificationSystems]
		SET ParentId = @ParentId
		where NAICSVersion = 2017 
		AND [Level] = @Level+1 AND Code LIKE CONCAT(@Code,'%');

		SET @Counter = @Counter + 1;

	END
SET @Level = @Level + 1;

END

--COMMIT TRANSACTION
--ROLLBACK TRANSACTION



