PRINT '04. Update [GrantApplications] - BusinessCaseDocument - START'

UPDATE [dbo].GrantApplications
SET  [dbo].GrantApplications.BusinessCaseDocumentId = mtp.BusinessCaseDocumentId
FROM  dbo.GrantApplications mga, TrainingPrograms mp, TrainingProviders mtp, TrainingProgramTrainingProviders mpp
WHERE
    mga.Id = mp.GrantApplicationId
	AND mpp.TrainingProgramId = mp.Id
	AND mpp.TrainingProviderId = mtp.Id
	AND mga.Id IN (
		select top 200 ga.Id
		from GrantApplications ga, dbo.GrantOpenings o, dbo.TrainingPeriods pd, dbo.FiscalYears fy, TrainingPrograms p,TrainingProviders tp, TrainingProgramTrainingProviders pp, dbo.GrantStreams s
		where
		ga.GrantOpeningId = o.Id
		and o.TrainingPeriodId = pd.Id
		and fy.Id = pd.FiscalYearId
		and ga.Id = p.GrantApplicationId
		and pp.TrainingProgramId = p.Id
		and pp.TrainingProviderId = tp.Id
		and o.GrantStreamId = s.Id
		and tp.TrainingOutsideBC = 1
		and (fy.Caption like 'FY2020%' OR fy.Caption like 'FY2021%')
		and s.GrantProgramId = 2
		and ga.ApplicationStateInternal not in (30,31,32)
		group by  ga.Id
	)
GO

UPDATE [dbo].GrantApplications
SET  [dbo].GrantApplications.BusinessCaseDocumentId = mtp.BusinessCaseDocumentId
FROM  [dbo].GrantApplications mga, [dbo].TrainingProviders mtp
WHERE
    mga.Id = mtp.GrantApplicationId
	AND mga.Id IN (
			select top (200) tp.[GrantApplicationId]
			from [dbo].[TrainingProviders] tp, [dbo].GrantApplications ga, [dbo].GrantApplicationInternalStates ais
			where tp.GrantApplicationId = ga.Id
			and ais.Id = ga.ApplicationStateInternal
			and TrainingOutsideBC = 1
			and GrantApplicationId is not null
			and ga.ApplicationStateInternal not in (30,31,32)
			and tp.DateAdded > '2019-01-01'
			)
GO

PRINT 'Update [GrantApplications] BusinessCaseDocument - END'