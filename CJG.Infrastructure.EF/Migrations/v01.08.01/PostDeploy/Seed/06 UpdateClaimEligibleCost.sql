PRINT 'UPDATE Claim Eligible Cost'
select 
distinct cec.Id
	, ga.ReimbursementRate
	, ec.AgreedMaxCost
	, ec.AgreedMaxParticipants
into #ClaimEligibleCost
from dbo.ParticipantCosts pc 
inner join dbo.ClaimEligibleCosts cec on pc.ClaimEligibleCostId = cec.Id 
inner join dbo.Claims c on cec.ClaimId = c.Id and cec.ClaimVersion = c.ClaimVersion 
inner join dbo.TrainingPrograms tp on c.TrainingProgramId = tp.Id 
inner join dbo.EligibleCosts ec on cec.EligibleCostId = ec.Id 
inner join dbo.GrantApplications ga on tp.GrantApplicationId = ga.Id
where (ec.AgreedMaxCost != cec.AssessedCost 
	or ec.AgreedMaxParticipants != cec.AssessedParticipants)
	and cec.AssessedMaxParticipantCost <> 0  
	and c.ClaimState in (0, 1)
	and ga.ApplicationStateInternal != 30
	and c.ClaimVersion <= 1;

update cec
set cec.AssessedCost = t.AgreedMaxCost,
    cec.AssessedParticipants = t.AgreedMaxParticipants,
	cec.AssessedMaxParticipantCost = CASE WHEN t.AgreedMaxParticipants = 0 THEN 0 
									 ELSE 
										  ROUND(t.AgreedMaxCost / t.AgreedMaxParticipants, 2, 1)
									 END,
	cec.AssessedMaxParticipantReimbursementCost = ROUND((CASE WHEN t.AgreedMaxParticipants = 0 THEN 0 
													     ELSE 
															  ROUND(t.AgreedMaxCost / t.AgreedMaxParticipants, 2, 1)
														 END) * t.ReimbursementRate, 2, 1),
	cec.AssessedParticipantEmployerContribution = ROUND((CASE WHEN t.AgreedMaxParticipants = 0 THEN 0 
														 ELSE 
															  ROUND(t.AgreedMaxCost / t.AgreedMaxParticipants, 2, 1)
														 END) - 
														(ROUND((CASE WHEN t.AgreedMaxParticipants = 0 THEN 0 
															    ELSE 
																	ROUND(t.AgreedMaxCost / t.AgreedMaxParticipants, 2, 1)
															    END) * t.ReimbursementRate, 2, 1)), 2)
from ClaimEligibleCosts cec inner join 
	 #ClaimEligibleCost t on cec.Id = t.Id;

drop table #ClaimEligibleCost;