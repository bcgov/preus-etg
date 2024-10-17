PRINT N'Initializing Database CJG'
--Configure User/login
PRINT N'Configure User & Login'
DECLARE @user NVARCHAR(10) = 'CJG'
IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = @user )
BEGIN
	EXECUTE ('CREATE LOGIN @CJG WITH DEFAULT_DATABASE = CJG')
END
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = @user )
BEGIN
	EXECUTE ('CREATE USER CJG FOR LOGIN CJG WITH DEFAULT_SCHEMA = dbo')
END
EXECUTE ('GRANT CONNECT TO CJG AS [dbo]')
EXECUTE sp_addrolemember @rolename = N'db_datareader', @membername = @user;
EXECUTE sp_addrolemember @rolename = N'db_datawriter', @membername = @user;
EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = @user;
