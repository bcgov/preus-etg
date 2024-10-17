PRINT '01. Updating [NationalOccupationalClassifications]'

update dbo.NationalOccupationalClassifications
set Description = CASE Code When 4 Then 'Occupations in education, law and social, community and government services'
                            When 41 Then 'Professional occupations in law and social, community and government services'
                            When 4168 Then 'Program officers unique to government'
                            When 0411 Then 'Government managers - health and social policy development and program administration'
                            When 0412 Then 'Government managers - economic analysis, policy development and program administration'
                            When 0413 Then 'Government managers - education policy development and program administration'
                            When 0012 Then 'Senior government managers and officials'
                  End

Where code in('4','41','4168','0411','0412','0413','0012')


PRINT 'Update Completed!!'