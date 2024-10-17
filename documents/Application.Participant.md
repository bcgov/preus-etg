# Participant Application
The Participant Information Collection Form (PIF) is used by participants to enter their information so that they can participant in training.

The PIF contains personal private information (i.e. SIN, Address).  As such it requires specific care for storing this information and reporting on it.

The PIF application site is an anonymous access application.  It requires a specific invitation by the Applicant of an approved Grant Application.
The invitiation contains a temporary private key (GUID) and URL that will allow for a time-period access to the PIF.

If the participant does not Submit their information, it will not be saved.

If the participant Submits their information, it will be saved with the related Grant Application.

PIF Security is covered by the following;
- Participant requires temporary private key (GUID) and URL
- Participant must pass ReCaptcha authentication
- Participant information is only saved if they Submit
- Participant cannot view any information after Submitting
- The form will timeout and lose their information after a configured time-period
- All data stored after Submit is only accessible to authorized resources