PRINT 'UPDATE ParticipantForms.IsExcludedFromClaim'
;
WITH CurrentClaim
AS (SELECT
  t.GrantApplicationId,
  t.ClaimId,
  t.ClaimVersion
FROM (SELECT
  ga.id AS GrantApplicationId,
  c.Id AS ClaimId,
  c.claimversion,
  ROW_NUMBER() OVER (PARTITION BY ga.id ORDER BY c.Id DESC, c.ClaimVersion DESC) AS rn
FROM GrantApplications AS ga
JOIN Claims AS c
  ON c.GrantApplicationId = ga.Id) AS t
WHERE t.rn = 1)

UPDATE p
SET p.IsExcludedFromClaim = 1
FROM ParticipantForms AS p
JOIN GrantApplications AS ga
  ON ga.Id = p.GrantApplicationId
JOIN CurrentClaim AS c
  ON c.GrantApplicationId = ga.Id
JOIN ClaimEligibleCosts AS cec
  ON cec.ClaimId = c.ClaimId
  AND cec.ClaimVersion = c.ClaimVersion
LEFT JOIN ParticipantCosts AS pc
  ON pc.ClaimEligibleCostId = cec.Id
  AND pc.ParticipantFormId = p.Id
WHERE pc.Id IS NULL