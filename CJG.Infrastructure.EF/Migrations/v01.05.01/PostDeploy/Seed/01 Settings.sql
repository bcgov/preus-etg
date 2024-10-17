PRINT 'Updating [Settings]'

IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[Settings] WHERE [Key] = N'EnableClaimsOn'))
BEGIN
	INSERT [dbo].[Settings]
	([Key], [Value], [ValueType]) VALUES 
	(N'EnableClaimsOn', N'2017/09/05 12:00:00AM', N'System.DateTime')
END
ELSE
BEGIN
	UPDATE [dbo].[Settings]
	SET [Value] = N'2017/09/05 12:00:00AM'
	WHERE [Key] = N'EnableClaimsOn'
END