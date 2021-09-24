PRINT 'Updating [AccountCodes]'

SET IDENTITY_INSERT [dbo].[AccountCodes] ON

INSERT INTO dbo.[AccountCodes] (
	Id
    , GLClientNumber
    , GLRESP
    , GLServiceLine
    , GLSTOBNormal
    , GLSTOBAccrual
    , GLProjectCode
) VALUES (
	1
	, '019'
	, '11352'
	, '20921'
	, '8001'
	, '3075'
	, '1111CJG'
), (
	2
	, '019'
	, '11352'
	, '20921'
	, '8001'
	, '3075'
	, '1111CJG'
)

SET IDENTITY_INSERT [dbo].[AccountCodes] OFF