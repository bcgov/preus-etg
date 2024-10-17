PRINT '[ClassificationOfInstructionalPrograms] Parent IDs';

DECLARE @RootId INT, @Level INT, @MaxCount INT, @Counter INT, @Code NVARCHAR(100), @ParentId INT;

SELECT @RootId = [Id] FROM [dbo].[ClassificationOfInstructionalPrograms] where [Level]=0;

SELECT @RootId ;--root level

UPDATE [dbo].[ClassificationOfInstructionalPrograms] 
SET [ParentId] = @RootId 
WHERE [Level]=1;

DECLARE @tblLevel TABLE(Id INT NOT NULL identity(1,1), Code NVARCHAR(100), [Level] INT, LevelId INT)
SET @Level = 1;

--BEGIN TRANSACTION

WHILE(@Level <= 3)
BEGIN 

INSERT INTO @tblLevel (Code, [Level], LevelId)
SELECT Code, [Level], Id FROM [dbo].[ClassificationOfInstructionalPrograms] where [Level] = @Level;

SELECT @MaxCount = MAX(Id) FROM @tblLevel;

--SELECT * FROM @tblLevel;

SET @Counter = 1;

	WHILE(@Counter <= @MaxCount)
	BEGIN
		SELECT @Code = Code, @ParentId = LevelId FROM @tblLevel where Id = @Counter;

		UPDATE [dbo].[ClassificationOfInstructionalPrograms]
		SET ParentId = @ParentId
		where [Level] = @Level+1 AND Code LIKE CONCAT(@Code,'%');

		SET @Counter = @Counter + 1;
	
	END
SET @Level = @Level + 1;

END

--COMMIT TRANSACTION
--ROLLBACK TRANSACTION



