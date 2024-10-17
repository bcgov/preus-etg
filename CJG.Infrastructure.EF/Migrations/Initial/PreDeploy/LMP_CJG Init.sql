PRINT N'Initializing Database LMP_CJG'
--Configure User/login
PRINT N'Configure User & Login'
DECLARE @user NVARCHAR(10) = 'LMP_CJG'
IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = @user )
BEGIN
	EXECUTE ('CREATE LOGIN @LMP_CJG WITH DEFAULT_DATABASE = LMP_CJG')
END
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = @user )
BEGIN
	EXECUTE ('CREATE USER LMP_CJG FOR LOGIN LMP_CJG WITH DEFAULT_SCHEMA = dbo')
END
EXECUTE ('GRANT CONNECT TO LMP_CJG AS [dbo]')
EXECUTE sp_addrolemember @rolename = N'db_datareader', @membername = @user;
EXECUTE sp_addrolemember @rolename = N'db_datawriter', @membername = @user;
EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = @user;
