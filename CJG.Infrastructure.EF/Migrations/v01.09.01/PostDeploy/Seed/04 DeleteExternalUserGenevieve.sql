PRINT 'Deleting User - Genevieve'

-- Delete External Users
select * into #userIds from 
(select [Id]
from [dbo].[Users]
where (BCeIDGuid = '428C86A0-1B20-4425-A971-D5D829831450')) as t

delete [dbo].[UserGrantProgramPreferences]
where UserId in (select * from #userIds)

delete [dbo].[UserPreferences]
where UserId in (select * from #userIds)

delete [dbo].[Users]
where Id in (select * from #userIds)

drop table #userIds
