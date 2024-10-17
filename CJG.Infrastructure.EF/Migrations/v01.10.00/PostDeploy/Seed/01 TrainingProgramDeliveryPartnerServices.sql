PRINT 'Copy [#GrantApplicationDeliveryPartnerServices] into [GrantApplicationDeliveryPartnerServices]'

INSERT INTO dbo.GrantApplicationDeliveryPartnerServices (
	GrantApplicationId
	, DeliveryPartnerServiceId
)
SELECT DISTINCT 
	GrantApplicationId
	, DeliveryPartnerServiceId 
FROM #GrantApplicationDeliveryPartnerServices

DROP TABLE #GrantApplicationDeliveryPartnerServices