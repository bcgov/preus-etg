PRINT 'Update [OrganizationTypes]'

UPDATE OrganizationTypes 
set Caption = 'Profit (Private)'
Where Id = 1;

Update OrganizationTypes
set Caption = 'Non-Profit'
Where Id = 2;

Update OrganizationTypes
set Caption = 'Government/Public'
Where Id = 3;

Update Organizations
Set OrganizationTypeId = 1
Where OrganizationTypeId > 3

Delete OrganizationTypes
Where Id > 3;