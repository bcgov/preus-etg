PRINT 'Start updating [InDemandOccupations]'

PRINT ' - Deactivating [InDemandOccupations]'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Automotive Painter'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Automotive Refinishing Prep Technician'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Dairy Production Technician 1'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Dairy Production Technician 2'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Domestic / Commercial Gasfitter (Class B)'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Electric Motor Systems Technician'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Horticultural Technician Foundation'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Horticulturist, Landscape'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Horticulturist, Production'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Inboard / Outboard Mechanic'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Marine Foundation'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Mobile Crane Operator Lattice Boom Friction Crane'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Mobile Crane Operator Lattice Boom Hydraulic Crane'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Mobile Crane Operator Unlimited Tonnage'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Motor Vehicle Body Repairer (Metal & Paint)'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Motorcycle and Power Equipment Technician'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Planermill Maintenance Technician 1'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Planermill Maintenance Technician 2'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Rig Technician'
UPDATE InDemandOccupations SET IsActive = 0, DateUpdated = GETUTCDATE() WHERE Caption = 'Road Builder and Heavy Construction Foundation'

PRINT ' - Renaming [InDemandOccupations]'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Asphalt Paving/Laydown Technician' WHERE Caption = 'Asphalt Paving / Laydown Technician'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Bricklayer' WHERE Caption = 'Bricklayer (Mason)'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Professional Cook 3' WHERE Caption = 'Cook'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Floorcovering Installer' WHERE Caption = 'Floor Covering Installer'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Gasfitter - Class A' WHERE Caption = 'Gasfitter (Class A)'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Gasfitter - Class B' WHERE Caption = 'Gasfitter (Class B)'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Geotechnical/Environmental Driller' WHERE Caption = 'Geotechnical / Environmental Driller'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Ironworker (Generalist)' WHERE Caption = 'Ironworker Generalist'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Ironworker (Reinforcing)' WHERE Caption = 'Ironworker - Reinforcing'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Parts Technician' WHERE Caption = 'Parts and Warehousing Person 1'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Parts Technician 1' WHERE Caption = 'Partsperson 2'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Parts Technician 2' WHERE Caption = 'Partsperson 3'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Powerline Technician' WHERE Caption = 'Power Line Technician'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Shipyard Labourer (Occupational Certificate)' WHERE Caption = 'Shipyard Labourer'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Sprinkler Fitter' WHERE Caption = 'Sprinkler System Installer'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Steamfitter/Pipefitter' WHERE Caption = 'Steamfitter / Pipefitter'
UPDATE InDemandOccupations SET DateUpdated = GETUTCDATE(), Caption = 'Welder Endorsement Multi-Process Alloy Welding (MPAW)' WHERE Caption = 'Welder Endorsement: Multi-Process Alloy Welding (MPAW)'

PRINT ' - Adding [InDemandOccupations]'
SET IDENTITY_INSERT [dbo].[InDemandOccupations] ON 

INSERT INTO [dbo].[InDemandOccupations] ([Id], [IsActive], [RowSequence], [Caption], [DateAdded]) 
VALUES
	(110, 1, 0, N'Auto Body and Collision Technician', GETUTCDATE()),
	(111, 1, 0, N'Automotive Refinishing Technician', GETUTCDATE()),
	(112, 1, 0, N'Boilermaker Endorsement: Marine Fitter', GETUTCDATE()),
	(113, 1, 0, N'Landscape Horticulturist', GETUTCDATE()),
	(114, 1, 0, N'Metal Fabricator (Fitter) Endorsement: Marine Fitter', GETUTCDATE()),
	(115, 1, 0, N'Motorcycle Technician', GETUTCDATE()),
	(116, 1, 0, N'Winder Electrician', GETUTCDATE())

SET IDENTITY_INSERT [dbo].[InDemandOccupations] OFF

PRINT 'Done updating [InDemandOccupations]'
