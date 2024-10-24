PRINT 'Inserting [TrainingProviderInventory]'

CREATE TABLE #TrainingProviderInventory (
	[IsActive] BIT,
	[IsEligible] BIT,
	[Name] NVARCHAR(250),
	[Notes] NVARCHAR(MAX)
)

INSERT INTO #TrainingProviderInventory
 ([IsActive], [IsEligible], [Name], [Notes]) VALUES
 (1, 1, N'0763658 BC Ltd.', N'ID-03555 Designated
Canadian College')
,(1, 1, N'0833888 BC Ltd.', N'ID-03501 Designated
Academy of Learning College - Langley')
,(1, 1, N'0833917 BC Ltd.', N'ID-03500 Designated
Academy of Learning College - Abbotsford')
,(1, 1, N'0837335 BC Ltd.', N'ID-00456 Designated
Imperial Hotel Management College')
,(1, 1, N'0921157 BC Ltd.', N'ID-03834 Designated
Canadian Vocational Training Centre')
,(1, 1, N'1006042 British Columbia Ltd.', N'ID-03624 Designated
Aveda Institute Vancouver')
,(1, 1, N'18320 Holdings Inc.', N'ID-00745 Designated
Automotive Training Centre')
,(1, 1, N'3Ho Foundation Society', N'ID-02484 Registered
Yoga West of Vancouver')
,(1, 1, N'546970 BC Ltd.', N'ID-03733 Designated
Legends Academy')
,(1, 1, N'641962 BC Ltd.', N'ID-03036 Designated
Greystone College of Business and Technology')
,(1, 1, N'7650329 Canada Inc.', N'ID-03661 Designated
The Institute of Holistic Nutrition')
,(1, 1, N'7990642 Canada Inc.', N'ID-03837 Designated
Integra College')
,(1, 1, N'8994269 Canada Ltd.', N'ID-01365 Designated
Academy of Classical Oriental Sciences')
,(1, 1, N'A & L World Of Beauty Ltd.', N'ID-00667 Designated
Gente Bella Beauty Academy')
,(1, 1, N'AAA Aviation Ltd.', N'ID-03921 Designated
AAA Aviation Flight Academy')
,(1, 1, N'Academy Of Excellence Hair Design & Aesthetics Ltd.', N'ID-00657 Designated')
,(1, 1, N'Accomplishment Coaching Canada, ULC', N'ID-04176 Registered
Accomplishment Coaching Canada')
,(1, 1, N'Adlerian Psychology Association Of BC', N'ID-03558 Registered
The Alfred Adler Institute')
,(1, 1, N'Aet Paramedic Academy Inc.', N'ID-03515 Registered
Aet Paramedic Academy')
,(1, 1, N'Ajna Yoga Inc.', N'ID-03723 Designated
Ajna Yoga Teacher Training')
,(1, 1, N'Alive Publishing Group Inc.', N'ID-02001 Registered
Alive Academy of Natural Health')
,(1, 1, N'All Body Laser Corp.', N'ID-03402 Designated
All Body Laser Corp. Training Institute')
,(1, 1, N'Allied Healthcare Training Inc.', N'ID-00209 Designated
West Coast College of Health Care')
,(1, 1, N'Alter Ego Studio And Cosmetology Institute', N'ID-03760 Designated')
,(1, 1, N'Amethyst College Incorporated', N'ID-03658 Registered
Amethyst College')
,(1, 1, N'Arbutus College of Communication Arts, Business & Technology', N'ID-02894 Designated')
,(1, 1, N'Artist Development Institute of Canada Inc.', N'ID-03862 Registered
Artist Development Institute of Canada')
,(1, 1, N'Arv Canada College', N'ID-03818 Registered')
,(1, 1, N'Ashton College Ltd.', N'ID-02275 Designated
Ashton College')
,(1, 1, N'BC College of Optics Inc.', N'ID-00276 Designated
BC College of Optics')
,(1, 1, N'BC NLP Neuro-Linguistic Programming Institute Inc.', N'ID-00550 Registered
Erickson Coaching International')
,(1, 1, N'BC Resident Care Aide Inc.', N'ID-03364 Designated
Pacific Coast Community College')
,(1, 1, N'Beauty Gateway Aesthetics & Aromatherapy School Inc.', N'ID-03355 Registered')
,(1, 1, N'Blue Bird Flight Academy Inc.', N'ID-03810 Designated')
,(1, 1, N'Body Glamour Institute of Beauty by Anita', N'ID-03328 Designated')
,(1, 1, N'Body Intelligence', N'ID-03524 Registered')
,(1, 1, N'Boucher Institute of Naturopathic Medicine Society', N'ID-02647 Designated
Boucher Institute of Naturopathic Medicine')
,(1, 1, N'Brainstation BC LLP', N'ID-04124 Registered
BrainStation')
,(1, 1, N'Brighton College Ltd.', N'ID-02632 Designated
Brighton College')
,(1, 1, N'British Columbia College of Equine Therapy', N'ID-01446 Registered')
,(1, 1, N'British Columbia Helicopters Ltd.', N'ID-01763 Designated
BC Helicopters Ltd.')
,(1, 1, N'C Morin Aviation (BC) Inc.', N'ID-03314 Registered
Glacier Air')
,(1, 1, N'Cambria College Ltd.', N'ID-02852 Designated
Cambria College')
,(1, 1, N'Cambridge College Of Technology Inc.', N'ID-01682 Designated
Cambridge College')
,(1, 1, N'Camcor Diving Inc.', N'ID-01816 Registered
BC College of Diving')
,(1, 1, N'Campbell River And District Association for Community Living', N'ID-03927 Registered
Community Living College')
,(1, 1, N'Canada International Career College Inc.', N'ID-03730 Registered')
,(1, 1, N'Canadian Acupressure College Inc.', N'ID-00973 Registered
Canadian Acupressure College')
,(1, 1, N'Canadian Arts & Sciences Institute Inc', N'ID-04092 Registered
Canadian Arts & Sciences Institute')
,(1, 1, N'Canadian College of Shiatsu Therapy Inc.', N'ID-02343 Designated
Canadian College of Shiatsu Therapy')
,(1, 1, N'Canadian Electrolysis College Ltd.', N'ID-03773 Registered')
,(1, 1, N'Canadian Film and Television Institute Ltd.', N'ID-03792 Registered
Canadian Film and Television Institute')
,(1, 1, N'Canadian Flight Centre Inc.', N'ID-00728 Designated
Canadian Flight Centre')
,(1, 1, N'Canadian Heritage Arts Society', N'ID-02149 Designated
Canadian College of Performing Arts')
,(1, 1, N'Canadian International Institute of Art Therapy', N'ID-04155 Registered
CiiAT')
,(1, 1, N'Canadian School of Natural Nutrition Vancouver Island', N'ID-03734 Designated')
,(1, 1, N'Canadian Society Of Immigration Consultants', N'ID-03765 Designated
Csic E-Academy')
,(1, 1, N'Canadian Sports Business Academy Inc.', N'ID-01524 Registered
Canadian Sports Business Academy')
,(1, 1, N'Canadian Tourism Business School Ltd.', N'ID-00304 Designated
Canadian Tourism College')
,(1, 1, N'Cancom Consulting Corporation', N'ID-00201 Designated
Academy of Learning College - Vancouver - Broadway')
,(1, 1, N'Can-Quest ESL & TOEFL Academy Inc.', N'ID-03952 Designated
Can-Quest ESL Academy')
,(1, 1, N'Canscribe Career Centre Inc.', N'ID-03167 Designated
Canscribe Career College')
,(1, 1, N'Capital College Ltd.', N'ID-03126 Designated
Capital College')
,(1, 1, N'Career City College Inc.', N'ID-04071 Registered
Career City College')
,(1, 1, N'Cat Centre for Arts & Technology Canada Inc.', N'ID-02951 Designated
Centre for Arts and Technology - Kelowna')
,(1, 1, N'Catholic Pacific College Society', N'ID-03916 Designated
Catholic Pacific College')
,(1, 1, N'Central College Inc.', N'ID-02684 Designated
Central College')
,(1, 1, N'CEO (Etudes en Osteopathie) Inc. CEO (Osteopathic Studies) Inc.', N'ID-03908 Registered
Canadian School of Osteopathy Manual Practice - Vancouver')
,(1, 1, N'CG Masters Academy Inc.', N'ID-03774 Designated
CG Masters School of 3D Animation & VFX')
,(1, 1, N'CHCA Canadian Health Care Academy Inc.', N'ID-01332 Designated
Canadian Health Care Academy')
,(1, 1, N'Chinook Helicopters (1982) Ltd.', N'ID-02105 Designated')
,(1, 1, N'City Ballet Academy Of Vancouver Ltd.', N'ID-00049 Designated
Goh Ballet Academy')
,(1, 1, N'Classic Aviation Ltd.', N'ID-03490 Registered
Classic Aviation')
,(1, 1, N'Clearmind International Institute Inc.', N'ID-03151 Designated')
,(1, 1, N'Cloud Nine College Ltd.', N'ID-03984 Designated
Cloud Nine College')
,(1, 1, N'Coast Helicopter College Ltd.', N'ID-03265 Registered')
,(1, 1, N'Coastal International Language Institute Inc.', N'ID-03968 Designated
inlingua Victoria')
,(1, 1, N'Coastal Pacific Aviation Ltd.', N'ID-00714 Designated')
,(1, 1, N'CodeCore Technologies Inc.', N'ID-03906 Designated
CodeCore')
,(1, 1, N'Colcanada Trading Corp.', N'ID-03935 Designated
inlingua Vancouver')
,(1, 1, N'College Educacentre College', N'ID-02018 Designated')
,(1, 1, N'Columbia Bible College', N'ID-00019 Designated')
,(1, 1, N'Compuran Computer Services Ltd.', N'ID-02572 Designated
Institute of Technology Development of Canada (ITD)')
,(1, 1, N'Construction & Specialized Workers Training Society', N'ID-00761 Designated')
,(1, 1, N'Core Belief Engineering Ltd.', N'ID-01818 Registered
College of Core Belief Engineering')
,(1, 1, N'Cornerstone International Education Inc.', N'ID-00570 Designated
Cornerstone International Community College of Canada')
,(1, 1, N'Cottle & Earle Agencies Ltd.', N'ID-03420 Registered
International Academy of Canine Trainers')
,(1, 1, N'Create Career College Inc.', N'ID-03664 Designated
Create Career College')
,(1, 1, N'CSLI - Canadian As a Second Language Institute Inc.', N'ID-03985 Designated
Canadian Second Language Institute')
,(1, 1, N'Del Rio Academy of Hair and Esthetics Ltd.', N'ID-03509 Designated')
,(1, 1, N'Dharma Sara Satsang Society', N'ID-03086 Registered
Salt Spring Centre of Yoga')
,(1, 1, N'Discovery Community College Ltd.', N'ID-00193 Designated
Discovery Community College')
,(1, 1, N'Dominion Herbal College Inc.', N'ID-00283 Designated
Dominion Herbal College')
,(1, 1, N'Dorset College Inc.', N'ID-02677 Designated
Dorset College')
,(1, 1, N'Drake Medox Training And Development (British Columbia) Inc.', N'ID-03117 Designated
Drake Medox College')
,(1, 1, N'Eaton Cognitive Improvement Centre Ltd.', N'ID-04033 Designated
Eaton Cognitive Improvement Centre')
,(1, 1, N'EC Vancouver Language Center Ltd.', N'ID-03980 Designated
EC Vancouver')
,(1, 1, N'EF International Language Schools (Canada) Ltd.', N'ID-03958 Designated
EF International Language Centers')
,(1, 1, N'Electrical Joint Training Committee Society', N'ID-02629 Designated
Electrical Joint Training Committee')
,(1, 1, N'ELS Education Services Canada Inc.', N'ID-04002 Designated
ELS Language Centers')
,(1, 1, N'Elsha Wellness Group Inc.', N'ID-03814 Registered
Canadian School of Natural Nutrition - Kelowna')
,(1, 1, N'ESLI, Inc.', N'ID-03978 Designated
ESLI')
,(1, 1, N'Eton College Canada Inc.', N'ID-03111 Designated
Eton College')
,(1, 1, N'First Community College Inc.', N'ID-03789 Registered
First College')
,(1, 1, N'Flower Power Enterprises Inc.', N'ID-00860 Designated
Aveda Institute Victoria')
,(1, 1, N'Focus College of Professionals Ltd.', N'ID-00340 Designated')
,(1, 1, N'Franklin English Language College Inc.', N'ID-03986 Designated
Franklin English Language College')
,(1, 1, N'Fraser Education Inc.', N'ID-03662 Designated
Western Maritime Institute')
,(1, 1, N'Future Hair Training Centre Ltd.', N'ID-02172 Designated
Future Hair Training Centre')
,(1, 1, N'Gateway College Inc.', N'ID-00267 Designated
Gateway College')
,(1, 1, N'GBC Education Ltd.', N'ID-03469 Designated
Gastown Business College')
,(1, 1, N'GEOS Language Corporation / Corporation De Langues GEOS', N'ID-03954 Designated
GEOS Languages Plus')
,(1, 1, N'Gibson''s Barber Shop and School, Ltd.', N'ID-04064 Registered
Gibson''s Barber Shop and School')
,(1, 1, N'Global Model And Talent Inc.', N'ID-00647 Designated
New Image College of Fine Arts')
,(1, 1, N'Global Village English Centre Victoria Ltd.', N'ID-03966 Designated
Global Village Victoria')
,(1, 1, N'Global Village Vancouver Teacher Training & Services Inc.', N'ID-03344 Registered
Global Village Career Training Centre')
,(1, 1, N'Good Dog Wellness Inc.', N'ID-03738 Registered
Good Dog Academy')
,(1, 1, N'Grandesco College Ltd.', N'ID-03003 Designated
Grandesco College')
,(1, 1, N'Hair Art 2014 Holdings Ltd.', N'ID-00878 Designated
Hair Art Academy')
,(1, 1, N'Hamasaki Enterprises Ltd.', N'ID-03101 Designated
Advantage English School E/J')
,(1, 1, N'Hanson International Education And Employment Services Ltd.', N'ID-03800 Designated
Hanson International Academy')
,(1, 1, N'Harbourside Music Productions Inc.', N'ID-03381 Designated
Harbourside Institute of Technology')
,(1, 1, N'Haven Foundation', N'ID-00544 Registered
The Haven')
,(1, 1, N'HeartSafe EMS Paramedic Training Ltd.', N'ID-03876 Registered
HeartSafe EMS')
,(1, 1, N'Heli - College Canada Training Inc.', N'ID-00809 Designated')
,(1, 1, N'Heritage Community College Of Business Inc.', N'ID-03641 Registered
Heritage Community College')
,(1, 1, N'Hilltop Training Systems Corp.', N'ID-00938 Designated
Hilltop Academy')
,(1, 1, N'HJW Equine Studies', N'ID-03433 Registered')
,(1, 1, N'Horiticulture Centre Of The Pacific', N'ID-00529 Designated
Pacific Horticulture College')
,(1, 1, N'IH Career College Inc.', N'ID-03387 Designated
IH Career College')
,(1, 1, N'ILAC International College (BC) Ltd.', N'ID-03843 Designated
ILAC International College')
,(1, 1, N'Immigrant Services Society of British Columbia', N'ID-00473 Designated
ISS Language and Career College of BC')
,(1, 1, N'In Focus Film School Society', N'ID-03763 Designated
In Focus Film School')
,(1, 1, N'Infofit Industries', N'ID-03721 Registered
Infofit Educators')
,(1, 1, N'Insignia College of Health and Business Ltd.', N'ID-03468 Designated
Insignia College of Health and Business')
,(1, 1, N'Interior Academy of Hair Design Ltd. Kamloops', N'ID-01007 Designated
Interior Academy')
,(1, 1, N'Interior Heavy Equipment Operator School Ltd.', N'ID-03395 Designated')
,(1, 1, N'International College of Medical Intuition, Inc.', N'ID-03848 Registered')
,(1, 1, N'International College of Traditional Chinese Medicine of Vancouver', N'ID-00285 Designated')
,(1, 1, N'International Flight Centre Inc.', N'ID-03432 Designated')
,(1, 1, N'International Language Academy of Canada LTD', N'ID-03993 Designated
International Language Academy of Canada (ILAC)')
,(1, 1, N'Iriol Software Ltd.', N'ID-03556 Registered
Hitek Computer School')
,(1, 1, N'Island Coastal Aviation Inc.', N'ID-03883 Designated')
,(1, 1, N'Istuary Innovation Institute Ltd.', N'ID-00629 Designated
Istuary Innovation Institute')
,(1, 1, N'iTTTi Vancouver Enterprises Incorporated', N'ID-03955 Designated
iTTTi Vancouver')
,(1, 1, N'Iuoe Local 115 Training Association', N'ID-00776 Designated')
,(1, 1, N'Joami Arts Development Inc.', N'ID-03602 Designated
Victoria College of Art')
,(1, 1, N'John Casablancas Institute Inc.', N'ID-00649 Designated
JCI Institute')
,(1, 1, N'Joy Beauty Salon Ltd.', N'ID-02204 Registered
Joy Beauty School')
,(1, 1, N'JWD Adventures Ltd.', N'ID-03969 Designated
Camber College')
,(1, 1, N'Kangjia International Holdings Inc.', N'ID-03087 Designated
Vancouver Beijing College of Chinese Medicine')
,(1, 1, N'Kcpc, Kelowna College of Professional Counselling, Inc.', N'ID-03285 Designated
Kelowna College of Professional Counselling')
,(1, 1, N'Kelowna English Centre Ltd.', N'ID-04081 Designated
VanWest College (Kelowna)')
,(1, 1, N'KGIC Business College (2010) Corp.', N'ID-03651 Designated
King George International Business College (KGIBC) - Canada TESOL Centre (CTC)')
,(1, 1, N'KGIC Language College (2010) Corp.', N'ID-03973 Designated
King George International College (KGIC)')
,(1, 1, N'Kheiron College Inc.', N'ID-03809 Registered
Kheiron College of Equine Therapy')
,(1, 1, N'Kitimaat Valley Education Society', N'ID-03114 Registered
Kitimat Valley Institute')
,(1, 1, N'Knights College Inc.', N'ID-04153 Registered
Knights College')
,(1, 1, N'Korol Marine Services Ltd.', N'ID-03455 Designated
Divesafe International')
,(1, 1, N'Kosmetae - International Skin Care Training Institute Inc.', N'ID-00638 Designated
Kosmetae Academy')
,(1, 1, N'Kutenai Art Therapy Institute Association', N'ID-01136 Designated
Kutenai Art Therapy Institute')
,(1, 1, N'Langley Flying School Inc.', N'ID-01540 Designated')
,(1, 1, N'Language Studies International (Canada) Inc.', N'ID-03972 Designated
Language Studies International')
,(1, 1, N'LaSalle College International Vancouver Inc.', N'ID-02160 Designated
LaSalle College Vancouver')
,(1, 1, N'Learning Matters Ltd.', N'ID-03060 Registered
Victoria Feldenkrais Teacher Training Program')
,(1, 1, N'Learntech Solutions Ltd.', N'ID-03473 Designated
Academy of Learning College - Surrey')
,(1, 1, N'Lighthouse Labs Inc.', N'ID-03874 Designated
Lighthouse Labs')
,(1, 1, N'Limpark Investments Ltd.', N'ID-03933 Designated
Juillet College')
,(1, 1, N'Living Language Institute Foundation', N'ID-03687 Designated')
,(1, 1, N'Living Systems: Family Systems Counselling, Education, Train', N'ID-00542 Registered')
,(1, 1, N'London School of Hairdressing Ltd.', N'ID-00617 Designated
London School')
,(1, 1, N'Lost Boys Studios Inc.', N'ID-03430 Designated
Lost Boys Studios - School of Visual Effects')
,(1, 1, N'Loxx Academy of Hair Design Inc.', N'ID-01784 Designated')
,(1, 1, N'M Grenkow Inc.', N'ID-03791 Registered
Animal Haven Grooming')
,(1, 1, N'M.C. College Group Inc.', N'ID-01776 Designated
M.C. College')
,(1, 1, N'Madlab School of Fitness Inc.', N'ID-03588 Designated
Madlab School of Fitness')
,(1, 1, N'Magna Learning Solutions Ltd.', N'ID-03807 Registered
Concordia College')
,(1, 1, N'Mark Anthony Academy of Cosmetology Ltd.', N'ID-03732 Designated
Mark Anthony Academy of Cosmetology')
,(1, 1, N'Medical Reception College Ltd.', N'ID-03754 Designated')
,(1, 1, N'Metropolitan Community College Ltd.', N'ID-02566 Designated
Metropolitan Community College')
,(1, 1, N'Mina & Em Enterprises Inc.', N'ID-03388 Designated
Em Beautician School of Canada')
,(1, 1, N'Mirage Spa', N'ID-03563 Registered
Mirage Spa Education Canada')
,(1, 1, N'Montair Aviation Inc.', N'ID-03999 Designated
Montair')
,(1, 1, N'Montessori Training Centre Society Of British Columbia', N'ID-00556 Designated
Montessori Training Centre of British Columbia')
,(1, 1, N'Mountain Transport Institute Ltd.', N'ID-02506 Designated
Mountain Transport Institute Ltd. - Castlegar')
,(1, 1, N'Ms. Lorea''s College of Esthetics & Nail Technology Inc.', N'ID-01163 Designated')
,(1, 1, N'MTI Community College Ltd.', N'ID-00740 Suspended (Designated)
MTI Community College')
,(1, 1, N'NEC Native Education College', N'ID-00597 Designated
Native Education College')
,(1, 1, N'Neety''s Shahnaz Herbal Day Spa & School of Cosmetology Inc.', N'ID-03630 Registered')
,(1, 1, N'Neha Hair Salon Ltd.', N'ID-03574 Registered
Neha Hair Dressing School')
,(1, 1, N'Net-Pacific Coordinations, Inc.', N'ID-03987 Designated
International House Vancouver')
,(1, 1, N'New Link College Inc.', N'ID-03856 Designated
New Link College')
,(1, 1, N'Nimbus School of Recording Arts Ltd.', N'ID-03571 Designated
Nimbus School of Recording & Media')
,(1, 1, N'North East Native Advancing Society', N'ID-03464 Registered
Aboriginal Centre for Leadership and Innovation')
,(1, 1, N'Northwest Culinary Academy of Vancouver Inc.', N'ID-03232 Designated
Northwest Culinary Academy of Vancouver')
,(1, 1, N'Nu-Way Hairdressing School Ltd.', N'ID-01829 Registered
Nu-Way Hairdressing & Esthetics School (International)')
,(1, 1, N'O''Brien Training Ltd.', N'ID-03467 Registered
O''Brien Training')
,(1, 1, N'Ocean Quest Watersports Ltd.', N'ID-02392 Registered
Ocean Quest Water Sports')
,(1, 1, N'Okanagan Cosmetology Institute Ltd.', N'ID-03576 Designated')
,(1, 1, N'Okanagan Indian Educational Resource Society', N'ID-00595 Registered
En''Owkin Centre')
,(1, 1, N'Okanagan Mountain Helicopters FTU Ltd.', N'ID-03166 Designated')
,(1, 1, N'Okanagan Valley College of Massage Therapy Ltd.', N'ID-00989 Designated')
,(1, 1, N'Omni Centre Of Excellence Ltd.', N'ID-02461 Designated
OMNI College')
,(1, 1, N'Orca Training Group Ltd.', N'ID-01529 Designated
Orca Institute')
,(1, 1, N'Oshio College of Acupuncture and Herbology', N'ID-02360 Designated')
,(1, 1, N'Otter Training School Ltd.', N'ID-00677 Registered
Operators Training School')
,(1, 1, N'Oxygen Yoga & Fitness Inc', N'ID-03937 Registered
O2 Yoga Teacher Training')
,(1, 1, N'Pacific DanceArts Inc.', N'ID-03974 Designated
Pacific DanceArts')
,(1, 1, N'Pacific Design Academy Inc.', N'ID-00702 Designated
Pacific Design Academy')
,(1, 1, N'Pacific Flying Club', N'ID-00731 Designated')
,(1, 1, N'Pacific Institute of Culinary Arts Inc.', N'ID-01564 Designated
Pacific Institute of Culinary Arts')
,(1, 1, N'Pacific Language Institute Inc.', N'ID-03936 Designated
Kaplan International Vancouver')
,(1, 1, N'Pacific Professional Flight Centre Ltd.', N'ID-00735 Designated
Professional Flight Centre')
,(1, 1, N'Pacific Rim Aviation Academy Inc.', N'ID-03266 Designated')
,(1, 1, N'Pacific Rim College Inc.', N'ID-03445 Designated
Pacific Rim College')
,(1, 1, N'Pacific Rim Early Childhood Institute Inc.', N'ID-01860 Designated')
,(1, 1, N'Pacifique Riche Enterprises Ltd.', N'ID-00659 Designated
Blanche Macdonald Centre')
,(1, 1, N'Parone Stevenson Group Inc.', N'ID-00221 Designated
Excel Career College')
,(1, 1, N'Penticton School of Hair Ltd.', N'ID-02896 Designated
Penticton School of Hair')
,(1, 1, N'Pera College Ltd.', N'ID-03684 Designated
Pera College')
,(1, 1, N'PGIC Career College Inc.', N'ID-03488 Designated
PGIC Career College')
,(1, 1, N'PGIC Vancouver Studies Inc.', N'ID-03971 Designated
PGIC Vancouver')
,(1, 1, N'Pics Asia Pacific College Education (Pace) Canada Society', N'ID-01737 Registered
Pace Canada College')
,(1, 1, N'Piping Industry Apprenticeship Board', N'ID-02465 Designated
UA Piping Industry College of BC')
,(1, 1, N'Poludo Institute of Technology & Media Inc.', N'ID-04119 Registered')
,(1, 1, N'Prince George Native Friendship Centre Society', N'ID-00577 Registered
Prince George Native Friendship Centre')
,(1, 1, N'Prince George Nechako Aboriginal Employment & Training Association', N'ID-03600 Registered
PGNAETA Aboriginal Gateway Training Centre')
,(1, 1, N'Principal Air Ltd.', N'ID-03154 Designated
Principal Air')
,(1, 1, N'Professional Diving Technologies Ltd.', N'ID-00705 Designated
Diving Dynamics')
,(1, 1, N'Prospect College Inc.', N'ID-03454 Registered
Prospect College of Business and Language')
,(1, 1, N'Quantum Learning Academy Ltd.', N'ID-03892 Designated
Quantum Learning Academy')
,(1, 1, N'Rayway Operator Training School Ltd.', N'ID-03915 Registered')
,(1, 1, N'Red Academy Inc.', N'ID-04053 Registered
Red Academy')
,(1, 1, N'Rhodes Wellness College Ltd.', N'ID-01422 Designated')
,(1, 1, N'Richard Mar Inc.', N'ID-03497 Designated
Richard Mar Advanced School of Hairstyling & Esthetics')
,(1, 1, N'Robson Education Group Inc.', N'ID-03953 Designated
Global College')
,(1, 1, N'Roggendorf School of Hairdressing Inc.', N'ID-00622 Designated
Roggendorf School of Hairdressing & Nails')
,(1, 1, N'Rosen Method Institute Canada Inc.', N'ID-03593 Registered')
,(1, 1, N'Rsh Cosmetology Inc.', N'ID-00623 Designated
RSH International College of Cosmetology')
,(1, 1, N'Saint Elizabeth Health Care', N'ID-03742 Designated
Saint Elizabeth Health Career College')
,(1, 1, N'School District 42 (Maple Ridge-Pitt Meadows)', N'ID-00222 Designated
Ridge Meadows College')
,(1, 1, N'SchoolCreative Studios Inc.', N'ID-03472 Designated
SchoolCreative')
,(1, 1, N'Sci Stenberg College International Inc.', N'ID-03147 Designated
Stenberg College')
,(1, 1, N'Sea Land Air Management Ltd.', N'ID-03535 Designated
Sea Land Air Flight Centre')
,(1, 1, N'Seabird Island Education Society, Lalme'' Iwesawtexw', N'ID-03550 Designated
Seabird College')
,(1, 1, N'Selc Career College Canada Ltd.', N'ID-03618 Designated
SELC Career College Vancouver')
,(1, 1, N'SELC English Language Centre Canada LTD', N'ID-03959 Designated
SELC English Language Centre Canada')
,(1, 1, N'Semperviva Natural Health And Body Care Products Limited', N'ID-03479 Registered
Semperviva International Yoga College')
,(1, 1, N'Senniyo Aesthetics International School of Canada Inc.', N'ID-01258 Designated')
,(1, 1, N'SGIC Language School Inc.', N'ID-03961 Designated
St. George International College')
,(1, 1, N'Shinobi School of CG Inc.', N'ID-03790 Designated
Shinobi School of CG')
,(1, 1, N'Skyquest Aviation Ltd.', N'ID-03289 Designated')
,(1, 1, N'Sol Schools Vancouver Inc.', N'ID-03982 Designated
OHC Vancouver')
,(1, 1, N'Sonu Hairdressing & Esthetics School Ltd.', N'ID-03627 Registered')
,(1, 1, N'South Granville Business College Ltd.', N'ID-00181 Designated
Granville College')
,(1, 1, N'Southern Interior Construction Association', N'ID-01973 Designated
SICA Construction Training Centre')
,(1, 1, N'Southern Interior Flight Centre 1993 Ltd.', N'ID-00724 Registered
Southern Interior Flight Centre')
,(1, 1, N'Sprott-Shaw Degree College Corp.', N'ID-00182 Designated
Sprott Shaw College')
,(1, 1, N'St Giles International Languages Centers (Canada) Ltd.', N'ID-03979 Designated
St Giles Vancouver')
,(1, 1, N'Stage One Academy Inc', N'ID-03796 Registered
Stage One Academy')
,(1, 1, N'Statik The Salon Inc.', N'ID-03608 Designated
Vancouver Hairdressing Academy')
,(1, 1, N'Stewart College of Languages, Inc', N'ID-03967 Designated
Stewart College')
,(1, 1, N'Strathcona Park Lodge Ltd.', N'ID-02902 Designated
Canadian Outdoor Leadership Training - COLT')
,(1, 1, N'Studio Chi', N'ID-03240 Registered')
,(1, 1, N'Study English in Canada Inc. (Vancouver)', N'ID-03970 Designated
Study English in Canada')
,(1, 1, N'Suki''s Advanced Hairdressing School (International) Ltd.', N'ID-00624 Designated
Suki''s Advanced Hair Academy')
,(1, 1, N'TAMIK Training and Supplies Ltd', N'ID-03649 Registered
Vancouver Island Institute of Medical Technology')
,(1, 1, N'Tamwood Careers Ltd.', N'ID-03898 Designated
Tamwood Careers')
,(1, 1, N'Tamwood International College Ltd.', N'ID-03960 Designated
Tamwood Language Centres')
,(1, 1, N'Taylor Pro Training Ltd.', N'ID-02974 Designated')
,(1, 1, N'The Art Institute of Vancouver Inc.', N'ID-00045 Designated
The Art Institute of Vancouver')
,(1, 1, N'The Canadian College of English Language Ltd.', N'ID-04079 Designated
The Canadian College of English Language')
,(1, 1, N'The ILSC Education Group Inc.', N'ID-03975 Designated
ILSC - Vancouver')
,(1, 1, N'The Vancouver School of the Alexander Technique', N'ID-03136 Registered')
,(1, 1, N'Think Tank Training Centre Ltd.', N'ID-03397 Designated
Think Tank Training Centre')
,(1, 1, N'Tillicum Lelum Aboriginal Society', N'ID-00578 Registered
Tillicum Lelum Aboriginal Friendship Centre')
,(1, 1, N'Top Pro Hair School Ltd.', N'ID-03842 Registered
Rainbow Art')
,(1, 1, N'Tru Spa Institute of Aesthetics Ltd.', N'ID-03560 Designated')
,(1, 1, N'Tylair Aviation Ltd.', N'ID-03858 Interim Designated')
,(1, 1, N'United Pacific College Ltd.', N'ID-02103 Registered
United Pacific College')
,(1, 1, N'Universal Learning Institute Ltd.', N'ID-01606 Designated
Universal Learning Institute')
,(1, 1, N'Upgrade Learning Academy Inc.', N'ID-00165 Designated
Academy of Learning College - Richmond')
,(1, 1, N'Upper Career College of Business & Technology (Vancouver) Inc.', N'ID-03804 Designated
Upper Career College of Business & Technology')
,(1, 1, N'Vancouver Academy of Dramatic Arts Ltd.', N'ID-02798 Designated
Vancouver Academy of Dramatic Arts')
,(1, 1, N'Vancouver Academy of Music', N'ID-00042 Designated')
,(1, 1, N'Vancouver Animation Inc.', N'ID-03642 Designated
Vancouver Animation School')
,(1, 1, N'Vancouver Art Therapy Institute Association', N'ID-00545 Designated
Vancouver Art Therapy Institute')
,(1, 1, N'Vancouver Career College (Burnaby) Inc.', N'Designated:
ID-03162 CDI College of Business, Technology & Health Care
ID-03057 PCU College of Holistic Medicine
ID-00208 Vancouver Career College
ID-03581 VCAD')
,(1, 1, N'Vancouver College of Counsellor Training Ltd.', N'ID-03294 Designated
Vancouver College of Counsellor Training')
,(1, 1, N'Vancouver College of Dental Hygiene Inc.', N'ID-03368 Designated')
,(1, 1, N'Vancouver College of Manual Therapy Ltd.', N'ID-02099 Designated
Vancouver College of Massage Therapy (VCMT)')
,(1, 1, N'Vancouver Film School Limited', N'ID-00027 Designated
Vancouver Film School')
,(1, 1, N'Vancouver Institute of Media Arts Ltd.', N'ID-01349 Designated
Vancouver Institute of Media Arts (VanArts)')
,(1, 1, N'Vancouver International College of English Inc.', N'ID-03977 Designated
Vancouver International College')
,(1, 1, N'Vancouver Island School of Art Society', N'ID-03403 Designated
Vancouver Island School of Art')
,(1, 1, N'Vancouver Lions Gate College Inc.', N'ID-03951 Registered')
,(1, 1, N'Vancouver Maple Leaf Language College Inc.', N'ID-03981 Designated
Eurocentres Canada')
,(1, 1, N'Vancouver Metal Art School', N'ID-02827 Registered')
,(1, 1, N'Vancouver Premier College of Hotel Management Ltd.', N'ID-02048 Designated')
,(1, 1, N'Vancouver School of Bodywork and Massage Ltd.', N'ID-03187 Designated
Vancouver School of Bodywork and Massage')
,(1, 1, N'Vanint Education Inc.', N'ID-00202 Designated
Academy of Learning College - Victoria')
,(1, 1, N'Vanperpetual English Language College Ltd.', N'ID-04004 Registered
Vanperpetual English Language College')
,(1, 1, N'VanWest College Ltd.', N'ID-03938 Designated
VanWest College (Vancouver)')
,(1, 1, N'VGC Education Inc.', N'ID-03861 Designated
VGC International College')
,(1, 1, N'Victor College Inc.', N'ID-03930 Designated
Victor College')
,(1, 1, N'Victoria Academy of Ballet Inc.', N'ID-03762 Designated
Victoria Academy of Ballet')
,(1, 1, N'Victoria Academy of Dramatic Arts', N'ID-03784 Designated')
,(1, 1, N'Victoria Conservatory of Music', N'ID-00912 Designated')
,(1, 1, N'Victoria Flying Club', N'ID-00718 Designated')
,(1, 1, N'Victoria International Academy Of Teacher Training Ltd.', N'ID-03380 Designated
VIA Training Centre')
,(1, 1, N'Victoria School Of Business And Technology Inc.', N'ID-03010 Designated
Q College')
,(1, 1, N'Visions Learning Centre Salon & Spa', N'ID-03465 Designated
Visions Learning Centre: Salon & Spa')
,(1, 1, N'Vogue College Ltd.', N'ID-02207 Designated
Vogue College')
,(1, 1, N'Wales Young Education Inc.', N'ID-03857 Registered
Wales Young Institute')
,(1, 1, N'West Bay College Inc.', N'ID-03533 Registered
West Bay College')
,(1, 1, N'West Coast College of Massage Therapy Inc.', N'ID-00831 Designated
West Coast College of Massage Therapy - New Westminster')
,(1, 1, N'West Coast Institute for Studies in Anthroposophy', N'ID-02287 Registered')
,(1, 1, N'West Coast Skills For Life Inc.', N'ID-03926 Designated
Academy of Learning College - Williams Lake')
,(1, 1, N'Westcoast Adventure College Ltd.', N'ID-03115 Designated
Westcoast Adventure College')
,(1, 1, N'Westcoast English Language Centre Ltd', N'ID-03983 Designated
Global Village Vancouver')
,(1, 1, N'Western Community College Inc.', N'ID-03758 Designated
Western Community College')
,(1, 1, N'Western Dog Grooming School Ltd.', N'ID-03617 Registered
Western Dog Grooming School')
,(1, 1, N'Western Holistic Health Inc.', N'ID-01892 Designated
Canadian School of Natural Nutrition')
,(1, 1, N'Westminster College, Inc.', N'ID-03498 Registered
Westminster College')
,(1, 1, N'Whistler Mountain Adventure School Ltd.', N'ID-03879 Designated
Whistler Adventure School')
,(1, 1, N'William Weselowski & Associates Ltd.', N'ID-02706 Registered
Innerstart Training and Education')
,(1, 1, N'Windsong College of Healing Arts', N'ID-02056 Designated')
,(1, 1, N'Winston College Management Ltd.', N'ID-03066 Designated
Winston College')
,(1, 1, N'WTS Waterworks Technology School Inc.', N'ID-04055 Registered
Waterworks Technology School')
,(1, 1, N'Yes Capital Corp.', N'ID-03539 Designated
Pacific Aviation Academy of British Columbia')
,(1, 1, N'Your Authentic Self Hypnotherapy Inc.', N'ID-03912 Registered
Horizon Centre School of Hypnotherapy')

-- Only insert them if the name doesn't already exist.
INSERT INTO [dbo].[TrainingProviderInventory]
 ([IsActive], [IsEligible], [Name], [Notes])
SELECT [IsActive], [IsEligible], [Name], [Notes]
FROM #TrainingProviderInventory tpi
WHERE NOT EXISTS (SELECT [Name] FROM [dbo].[TrainingProviderInventory] WHERE [Name] = tpi.[Name])

DROP TABLE #TrainingProviderInventory

PRINT 'Deleting [TrainingProviderInventory]';

-- Delete all duplicate Training Provider Invetory that is not referenced by a Training Provider.
WITH cte AS (
	SELECT tpi.[Id], tpi.[Name], ROW_NUMBER() OVER(PARTITION BY tpi.[Name] ORDER BY tpi.[Id]) AS [rn]
	FROM [dbo].[TrainingProviderInventory] tpi
)
DELETE tpi FROM dbo.[TrainingProviderInventory] tpi
WHERE tpi.[Id] IN (SELECT [Id] FROM cte WHERE [rn] > 1)
	AND NOT EXISTS (SELECT [TrainingProviderInventoryId] FROM dbo.[TrainingProviders] WHERE [TrainingProviderInventoryId] = tpi.[Id])