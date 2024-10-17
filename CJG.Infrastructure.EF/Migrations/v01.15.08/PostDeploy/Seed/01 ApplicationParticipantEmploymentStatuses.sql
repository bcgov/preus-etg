PRINT 'INSERT [ApplicationParticipantEmploymentStatuses]'

-- Insert Data from ProgramDescriptions table
INSERT INTO [ApplicationParticipantEmploymentStatuses]
           ([GrantApplicationId]
           ,[Id])
SELECT [GrantApplicationId]
      ,[ParticipantEmploymentStatusId]
FROM [ProgramDescriptions]
WHERE [ParticipantEmploymentStatusId] is not null

PRINT 'FINISHED DATAFIX FOR [ApplicationParticipantEmploymentStatuses]'

CHECKPOINT