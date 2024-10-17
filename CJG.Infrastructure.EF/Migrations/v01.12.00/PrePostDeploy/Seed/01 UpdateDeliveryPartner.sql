PRINT 'UPDATE [DeliveryPartners] - Make grant program specific'

Update [DeliveryPartners]
Set GrantProgramId = (Select TOP 1 [Id] from [GrantPrograms]), RowSequence = Id
Where GrantProgramId = 0

BEGIN
(SELECT t.[Caption], t.[IsActive], t.[RowSequence], t.[DateAdded], t.[DateUpdated], s.[Id]
INTO #DeliveryPartners
FROM [GrantPrograms] as s
LEFT JOIN [DeliveryPartners] as t
ON t.[GrantProgramId] <> s.[Id])

MERGE INTO [DeliveryPartners] WITH (HOLDLOCK) AS target
USING #DeliveryPartners AS source
    ON target.[GrantProgramId] = source.[Id]
WHEN MATCHED THEN 
    UPDATE SET target.[GrantProgramId] = source.[Id]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Caption]
           ,[IsActive]
           ,[RowSequence]
           ,[DateAdded]
           ,[DateUpdated]
           ,[GrantProgramId])
     VALUES
           (source.[Caption]
           ,source.[IsActive]
           ,source.[RowSequence]
           ,source.[DateAdded]
           ,source.[DateUpdated]
           ,source.[Id]);

DROP TABLE #DeliveryPartners
END;
