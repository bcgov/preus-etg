window.moment = require('moment-timezone');
require('../shared/clipboard');

// Angular shared features
require('../shared/angular-root');
require('../shared/angular-utils');
require('../shared/angular-base');
require('../shared/angular-section');
require('../shared/angular-parent-section');

// Utilities
require('./header');
require('./sidebar');

// Home
require('./home')

// User Profile
require('./user-profile');

// Organization Profile
require('./organization-profile');

// Application Creation
require('./application');
require('./attachments');
require('./training-provider');
require('./training-program');
require('./program-description');
require('./skills-training');
require('./service');
require('./service-provider');
require('./training-costs');

// Application Review
require('./application-review');

// Application View
require('./application-view');

// Agreement View
require('./agreement');

// Change Requests
require('./change-request');

// Reporting
require('./reporting')

// Participant Reporting
require('./participant-reporting');

// Claim Reporting
require('./claim');

// Completion Reporting
require('./completion-report');
