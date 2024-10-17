PRINT 'Update Training Provider Types'

-- Only ‘BC Public Post-Secondary Institution’ and ‘Training Organization operated by a BC School District’ do not require course outline and proof of qualifications.
Update [TrainingProviderTypes]
Set [PrivateSectorValidationType] = 2
Where [Id] > 2

-- The ‘Private Training Provider - Not Certified by Private Training Institutions Branch’ always require course outline and proof of qualifications.
Update [TrainingProviderTypes]
Set [PrivateSectorValidationType] = 1
Where [Id] = 8
