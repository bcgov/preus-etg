PRINT 'INSERT [ServiceLines]'

SET IDENTITY_INSERT [dbo].[ServiceLines] ON

INSERT INTO dbo.[ServiceLines] (
	Id
	, Caption
	, Description
	, BreakdownCaption
	, ServiceCategoryId
	, EnableCost
	, IsActive
	, RowSequence
	, DateAdded
	, DateUpdated
) VALUES (
	1
	, 'Pre-employment Services and Supports'
	, 'For Example: Training plan – working with a participant to develop a specific plan including identifying training needs, key steps, timelines, resources and milestones to secure sustainable employment or move onto further training leading to employment.'
	, ''
	, 1
	, 0
	, 1
	, 1
	, GETUTCDATE()
	, GETUTCDATE()
), (
	2
	, 'Pre-employment counselling/ coaching'
	, 'Employment or career counselling to help the participant to achieve their employment or career goals. Includes:<ul>
<li>supporting participants with their training plan;</li>
<li>assisting in the exploration of specific opportunities in participant''s local community;</li>
<li>mentoring and coaching;</li>
<li>job search assistance;</li>
<li>mental health, drug and alcohol counselling.</li></ul>
Includes:<ul>
<li>Intake assessments to determine program eligibility and suitability, employment readiness, potential barriers to employment, and training needs;</li>
<li>Personality, vocational and essential skills assessments.</li></ul>'
	, ''
	, 1
	, 0
	, 1
	, 2
	, GETUTCDATE()
	, GETUTCDATE()
), (
	3
	, 'Indigenous cultural components'
	, 'For Example:<ul>
	<li>Unique Indigenous protocols;</li>
	<li>Elder Support.</li></ul>'
	, ''
	, 1
	, 0
	, 1
	, 3
	, GETUTCDATE()
	, GETUTCDATE()
), (
	4
	, 'Job Search Assistance'
	, 'Job readiness skills such as job search, resume writing, and interview skills., These services are usually provided by employment service providers to prepare participants for entering the workforce'
	, ''
	, 1
	, 0
	, 1
	, 4
	, GETUTCDATE()
	, GETUTCDATE()
), (
	5
	, 'Essential Skills Training'
	, 'Training to develop one or more of the nine essential skills for the workplace as defined by Employment and Skills Development Canada at: http://www.esdc.gc.ca/en/essential_skills/definitions.page'
	, 'Essential Skills Type'
	, 2
	, 1
	, 1
	, 1
	, GETUTCDATE()
	, GETUTCDATE()
),(
	6
	, 'Soft Skills Training'
	, 'Personal management skills such as personal goal setting, time management, problem solving, decision making, communication, teamwork, and ongoing learning.'
	, ''
	, 2
	, 1
	, 1
	, 2
	, GETUTCDATE()
	, GETUTCDATE()
),
(
	7
	, 'Short-Term Skills Training'
	, 'Short duration training certificates/courses (e.g. first aid certifications, food safety) needed to meet the specific job or industry requirements.  Includes technical skills defined as training to develop skills required to operate a particular machine or use a particular technology.'
	, 'Short-Term Skills Type'
	, 2
	, 1
	, 1
	, 3
	, GETUTCDATE()
	, GETUTCDATE()
), (
	8
	, 'Apprenticeship Skills Training'
	, 'ITA Certified Trades (Red Seal and Non-Red Seal) training.'
	, 'Apprenticeship training is for'
	, 2
	, 1
	, 1
	, 4
	, GETUTCDATE()
	, GETUTCDATE()
), (
	9
	, 'Occupational Skills Training'
	, 'Funded interventions leading to certification or diploma with aim of obtaining employment for individuals. This includes individuals training while employed.'
	, ''
	, 2
	, 1
	, 1
	, 5
	, GETUTCDATE()
	, GETUTCDATE()
), (
	10
	, 'On-the-Job Skills Training'
	, 'On-the-job training or work experience performing duties of a particular position. Volunteer experience acquired in the community that develops a participant''s ability to search for, obtain and maintain employment.  Both paid and volunteer experience.'
	, ''
	, 2
	, 1
	, 1
	, 6
	, GETUTCDATE()
	, GETUTCDATE()
), (
	11
	, 'Childcare'
	, 'For a participant''s child while the participant is attending training or other services.'
	, ''
	, 3
	, 0
	, 1
	, 1
	, GETUTCDATE()
	, GETUTCDATE()
), (
	12
	, 'Transportation'
	, 'To get to training, services, job interviews or employment. Includes bus passes and mileage.'
	, ''
	, 3
	, 0
	, 1
	, 2
	, GETUTCDATE()
	, GETUTCDATE()
), (
	13
	, 'Other supports'
	, 'For Example:<ul>
	<li>Disability supports - provided to persons with disabilities to assist them in participating in training and other services, and in obtaining and maintaining employment;</li>
	<li>Accommodations while attending training;</li>
	<li>Equipment or work gear.</li></ul>'
	, ''
	, 3
	, 0
	, 1
	, 3
	, GETUTCDATE()
	, GETUTCDATE()
), (
	14
	, 'Wage subsidies'
	, 'Financial supports and benefits provided to the employer to cover a part of a participant''s wage. It is intended to assist a participant to obtain and maintain a job while in training or in support of other services.'
	, ''
	, 4
	, 0
	, 1
	, 1
	, GETUTCDATE()
	, GETUTCDATE()
), (
	15
	, 'Direct supports to employers'
	, 'Includes supports to employers in hiring and supporting participants to maintain employment, including developing training plans, e.g. for youth.'
	, ''
	, 4
	, 0
	, 1
	, 2
	, GETUTCDATE()
	, GETUTCDATE()
), (
	16
	, 'Post job placement supports'
	, 'Includes follow-up support to assist participants in maintaining employment.  Includes job coach, post-employment counselling/coaching, mentoring on the job.'
	, ''
	, 4
	, 0
	, 1
	, 3
	, GETUTCDATE()
	, GETUTCDATE()
)


SET IDENTITY_INSERT [dbo].[ServiceLines] OFF