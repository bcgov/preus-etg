window.moment = require('moment-timezone');
require('../shared/clipboard');

// Angular shared features
require('../shared/angular-root');
require('../shared/angular-utils');
require('../shared/angular-base');
require('../shared/angular-section');
require('../shared/angular-parent-section');

// Application Details
require('./application-notes');
require('./application-details');

// Grant Agreement
require('./grant-agreement');

// Claim Assessment
require('./claim-assessment');

// Grant Program Management
require('./grant-program');

// Grant Stream Management
require('./grant-stream');

// Grant Opening Management
require('./grant-opening');

// Notification Management
require('./notification');

// Program Notification Management
require('./program-notification');

// Director Dashboard
require('./batch-approval');

// Intake Queue
require('./intake-queue');

// Work Queue
require('./work-queue');

// User Management
require('./user');

// Community Management
require('./community');

// Payment Requests
require('./payment-request');

// Payment Reconcilation
require('./reconciliation');

// Claim Management Dashboard
require('./claim-dashboard');

// Completion Report
require('./completion-report');

// Service Descriptions
require('./service-description');

// Organization Profile
require('./organization');

// Participant Info
require('./participant');

// Training Provider Inventory
require('./training-provider-inventory');

// Debug
require('./debug');
