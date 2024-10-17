PRINT 'Copy [TrainingProgramDeliveryPartnerServices] into [#GrantApplicationDeliveryPartnerServices]'

CREATE TABLE #GrantApplicationDeliveryPartnerServices (
	GrantApplicationId INT,
	DeliveryPartnerServiceId INT
)

INSERT INTO #GrantApplicationDeliveryPartnerServices (
	GrantApplicationId
	, DeliveryPartnerServiceId
)
SELECT DISTINCT 
	tp.GrantApplicationId
	, t.DeliveryPartnerServiceId 
FROM dbo.TrainingProgramDeliveryPartnerServices t 
	INNER JOIN dbo.TrainingPrograms tp ON t.TrainingProgramId = tp.Id