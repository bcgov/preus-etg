PRINT 'Deleting [Users]'

-- Delete External Users
select * into #userIds from 
(select [Id]
from [dbo].[Users]
where (FirstName = 'Bonny' and LastName = 'Hasting')
or (FirstName = 'Shane' and LastName = 'Mantle')
or (FirstName = 'Kate' and LastName = 'Moyer')
or (FirstName = 'Per' and LastName = 'Wallenius')) as t

delete [dbo].[UserGrantProgramPreferences]
where UserId in (select * from #userIds)

delete [dbo].[UserPreferences]
where UserId in (select * from #userIds)

delete [dbo].[Users]
where Id in (select * from #userIds)

drop table #userIds
