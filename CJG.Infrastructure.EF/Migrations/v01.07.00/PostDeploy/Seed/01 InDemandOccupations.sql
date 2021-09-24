PRINT 'Updating [InDemandOccupations]'

-- Original Values before update
-- ([Id], [IsActive], [RowSequence], [Caption]) VALUES
-- (01, 1, 0, N'Carpenters')
-- (02, 1, 0, N'Gasfitters')
-- (03, 1, 0, N'Electricians')
-- (04, 1, 0, N'Heavy Duty Equipment Mechanic')
-- (05, 1, 0, N'Heavy Equipment Operators')
-- (06, 1, 0, N'Machinist')
-- (07, 1, 0, N'Millwrights')
-- (08, 1, 0, N'Plumbers')
-- (09, 1, 0, N'Sheet Metal Workers')
-- (10, 1, 0, N'Steamfitters, Pipefitters, and Sprinkler System Installers')
-- (11, 1, 0, N'Industrial Electricians')
-- (12, 1, 0, N'Crane Operators')
-- (13, 1, 0, N'Concrete Finishers')
-- (14, 1, 0, N'Cooks')
-- (15, 1, 0, N'Bakers')
-- (16, 1, 0, N'Welders')

 UPDATE [dbo].[InDemandOccupations] 
 SET [IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Carpenter'
 WHERE [Id] = 01

 UPDATE [dbo].[InDemandOccupations]
  SET [IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Gasfitter (Class A)'
 WHERE [Id] = 02

 UPDATE [dbo].[InDemandOccupations] 
 SET
	[IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Electrician, Construction'
 WHERE [Id] = 03

 UPDATE [dbo].[InDemandOccupations] 
 SET
	[IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Heavy Duty Equipment Technician'
 WHERE [Id] = 04

 UPDATE [dbo].[InDemandOccupations] 
 SET [IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Heavy Equipment Operator'
 WHERE [Id] = 05

 UPDATE [dbo].[InDemandOccupations] 
 SET [IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Machinist'		
 WHERE [Id] = 06

 UPDATE [dbo].[InDemandOccupations] 
 SET [IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Industrial Mechanic (Millwright)'
 WHERE [Id] = 07

 UPDATE [dbo].[InDemandOccupations] 
 SET [IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Plumber'	
 WHERE [Id] = 08

 UPDATE [dbo].[InDemandOccupations] 
 SET
	[IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Sheet Metal Worker'
 WHERE [Id] = 09

 UPDATE [dbo].[InDemandOccupations] 
 SET
	[IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Steamfitter / Pipefitter'
 WHERE [Id] = 10

 UPDATE [dbo].[InDemandOccupations] 
 SET [IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Electrician, Industrial'
 WHERE [Id] = 11

 UPDATE [dbo].[InDemandOccupations] 
 SET [IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Mobile Crane Operator'
 WHERE [Id] = 12

 UPDATE [dbo].[InDemandOccupations] 
 SET [IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Concrete Finisher'
 WHERE [Id] = 13

 UPDATE [dbo].[InDemandOccupations] 
 SET [IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Cook'
 WHERE [Id] = 14

 UPDATE [dbo].[InDemandOccupations] 
 SET	
	[IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Baker'
 WHERE [Id] = 15

 UPDATE [dbo].[InDemandOccupations] 
 SET [IsActive] = 1, 
	[RowSequence] = 0, 
	[Caption] = N'Welder'
 WHERE [Id] = 16

SET IDENTITY_INSERT [dbo].[InDemandOccupations] ON 

INSERT INTO [dbo].[InDemandOccupations] (
	[Id], [IsActive], [RowSequence], [Caption]
) VALUES ( 
	17, 1, 0, N'Agricultural Equipment Technician')
	, ( 18, 1, 0, N'Aircraft Maintenance Technician')
	, ( 19, 1, 0, N'Aircraft Structural Technician')
	, ( 20, 1, 0, N'Appliance Service Technician')
	, ( 21, 1, 0, N'Arborist Technician')
	, ( 22, 1, 0, N'Architectural Sheet Metal Worker')
	, ( 23, 1, 0, N'Asphalt Paving / Laydown Technician')
	, ( 24, 1, 0, N'Automotive Glass Technician')
	, ( 25, 1, 0, N'Automotive Painter')
	, ( 26, 1, 0, N'Automotive Refinishing Prep Technician')
	, ( 27, 1, 0, N'Automotive Service Technician')
	, ( 28, 1, 0, N'Boilermaker')
	, ( 29, 1, 0, N'Boom Truck Operator - Folding Boom Unlimited Tonnage')
	, ( 30, 1, 0, N'Boom Truck Operator - Stiff Boom Unlimited Tonnage')
	, ( 31, 1, 0, N'Bricklayer (Mason)')
	, ( 32, 1, 0, N'Broadband Network Technician')
	, ( 33, 1, 0, N'Cabinet Maker')
	, ( 34, 1, 0, N'Climbing Arborist')
	, ( 35, 1, 0, N'Construction Craft Worker (Labourer)')
	, ( 36, 1, 0, N'Dairy Production Technician 1')
	, ( 37, 1, 0, N'Dairy Production Technician 2')
	, ( 38, 1, 0, N'Diesel Engine Mechanic')
	, ( 39, 1, 0, N'Domestic / Commercial Gasfitter (Class B)')
	, ( 40, 1, 0, N'Drywall Finisher')
	, ( 41, 1, 0, N'Electric Motor Systems Technician')
	, ( 42, 1, 0, N'Electrician Endorsement: Marine')
	, ( 43, 1, 0, N'Embalmer')
	, ( 44, 1, 0, N'Embalmer and Funeral Director')
	, ( 45, 1, 0, N'Field Arborist')
	, ( 46, 1, 0, N'Floor Covering Installer')
	, ( 47, 1, 0, N'Funeral Director')
	, ( 48, 1, 0, N'Geoexchange Driller')
	, ( 49, 1, 0, N'Geotechnical / Environmental Driller')
	, ( 50, 1, 0, N'Glazier')
	, ( 51, 1, 0, N'Hairstylist')
	, ( 52, 1, 0, N'Horticultural Technician Foundation')
	, ( 53, 1, 0, N'Horticulturist, Landscape')
	, ( 54, 1, 0, N'Horticulturist, Production')
	, ( 55, 1, 0, N'Inboard / Outboard Mechanic')
	, ( 56, 1, 0, N'Instrumentation and Control Technician')
	, ( 57, 1, 0, N'Insulator (Heat and Frost)')
	, ( 58, 1, 0, N'Ironworker - Reinforcing')
	, ( 59, 1, 0, N'Ironworker Generalist')
	, ( 60, 1, 0, N'Lather (Interior Systems Mechanic)')
	, ( 61, 1, 0, N'Locksmith')
	, ( 62, 1, 0, N'Marine Foundation')
	, ( 63, 1, 0, N'Marine Mechanical Technician')
	, ( 64, 1, 0, N'Marine Service Technician')
	, ( 65, 1, 0, N'Meatcutter')
	, ( 66, 1, 0, N'Metal Fabricator (Fitter)')
	, ( 67, 1, 0, N'Mobile Crane Operator Hydraulic 80 Tonnes and Under')
	, ( 68, 1, 0, N'Mobile Crane Operator Unlimited Tonnage')
	, ( 69, 1, 0, N'Mobile Crane Operator Lattice Boom Friction Crane')
	, ( 70, 1, 0, N'Mobile Crane Operator Lattice Boom Hydraulic Crane')
	, ( 71, 1, 0, N'Motor Vehicle Body Repairer (Metal & Paint)')
	, ( 72, 1, 0, N'Motorcycle and Power Equipment Technician')
	, ( 73, 1, 0, N'Oil Heat System Technician')
	, ( 74, 1, 0, N'Painter and Decorator')
	, ( 75, 1, 0, N'Parts and Warehousing Person 1')
	, ( 76, 1, 0, N'Partsperson 2')
	, ( 77, 1, 0, N'Partsperson 3')
	, ( 78, 1, 0, N'Petroleum Equipment Installer')
	, ( 79, 1, 0, N'Petroleum Equipment Service Technician')
	, ( 80, 1, 0, N'Piledriver and Bridgeworker')
	, ( 81, 1, 0, N'Planermill Maintenance Technician 1')
	, ( 82, 1, 0, N'Planermill Maintenance Technician 2')
	, ( 83, 1, 0, N'Power Line Technician')
	, ( 84, 1, 0, N'Professional Cook 1')
	, ( 85, 1, 0, N'Professional Cook 2')
	, ( 86, 1, 0, N'Railway Car Technician')
	, ( 87, 1, 0, N'Recreation Vehicle Service Technician')
	, ( 88, 1, 0, N'Refrigeration and Air Conditioning Mechanic')
	, ( 89, 1, 0, N'Residential Building Maintenance Worker')
	, ( 90, 1, 0, N'Residential Steep Roofer')
	, ( 91, 1, 0, N'Rig Technician')
	, ( 92, 1, 0, N'Road Builder and Heavy Construction Foundation')
	, ( 93, 1, 0, N'Roofer')
	, ( 94, 1, 0, N'Saw Filer')
	, ( 95, 1, 0, N'Saw Filer Endorsement: Benchperson')
	, ( 96, 1, 0, N'Security Systems Technician')
	, ( 97, 1, 0, N'Shipyard Labourer')
	, ( 98, 1, 0, N'Sprinkler System Installer')
	, ( 99, 1, 0, N'Tidal Angling Guide')
	, ( 100, 1, 0, N'Tilesetter')
	, ( 101, 1, 0, N'Tool and Die Maker')
	, ( 102, 1, 0, N'Tower Crane Operator')
	, ( 103, 1, 0, N'Transport Trailer Technician')
	, ( 104, 1, 0, N'Truck and Transport Mechanic')
	, ( 105, 1, 0, N'Utility Arborist')
	, ( 106, 1, 0, N'Water Well Driller')
	, ( 107, 1, 0, N'Welder Endorsement: Multi-Process Alloy Welding (MPAW)')
	, ( 108, 1, 0, N'Wet Pump Installer') 
	, ( 109, 1, 0, N'Gasfitter (Class B)')

SET IDENTITY_INSERT [dbo].[InDemandOccupations] OFF