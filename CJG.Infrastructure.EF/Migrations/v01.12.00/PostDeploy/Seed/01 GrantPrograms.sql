PRINT 'Update [GrantPrograms] - Set FIFO'

UPDATE dbo.[GrantPrograms]
SET UseFIFOReservation = 1
WHERE [Id] IN (1, 2)