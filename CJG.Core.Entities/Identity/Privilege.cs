using System;

namespace CJG.Infrastructure.Identity
{
    /// <summary>
    /// User Privileges for Composing User Roles
    /// This enumeration should always be synced with content of ApplicationClaim table
    /// </summary>
    [Flags]
    public enum Privilege
    {
        /// <summary>
        /// Can view applications and claims
        /// </summary>
        IA1,
        /// <summary>
        /// Can view participant information
        /// </summary>
        IA2,
        /// <summary>
        /// Can view participant Social Insurance Number (required in all cases to access a SIN regardless of other privileges assigned)
        /// </summary>
        IA3,
        /// <summary>
        /// Can view grant streams and openings
        /// </summary>
        IA4,
        /// <summary>
        /// Can generate SDSI participant report
        /// </summary>
        IA5,
        /// <summary>
        /// Can manage grant streams and openings
        /// </summary>
        GM1,
        /// <summary>
        /// Can select (self-assign) applications or claims for assessment
        /// </summary>
        AM1,
        /// <summary>
        /// Can assess assigned applications or claims and recommend to Director and can view participant information only for an assigned file (see IA2)
        /// </summary>
        AM2,
        /// <summary>
        /// Within workflow rules, can issue, approve, edit, withdraw or cancel grant agreements.  Can assign applications to any assessor for assessment
        /// </summary>
        AM3,
        /// <summary>
        /// Can override workflow rules for AM3 privileges
        /// </summary>
        AM4,
        /// <summary>
        /// Can approve claim assessments that lead to payment requests
        /// </summary>
        AM5,
        /// <summary>
        /// Can print or reprint payment requests
        /// </summary>
        PM1,
        /// <summary>
        /// Can reconcile payment requests
        /// </summary>
        PM2,
        /// <summary>
        /// Can add a new user, deactivate a user or change a user’s role
        /// </summary>
        UM1,
        /// <summary>
        /// Can create or edit user roles
        /// </summary>
        UM2,
        /// <summary>
        /// Can post system notices to external users
        /// </summary>
        UM3,
        /// <summary>
        /// May add to training provider list and edit notes
        /// </summary>
        TP1,
        /// <summary>
        /// May change training provider eligibility and maintain training provider list entries
        /// </summary>
        TP2,
        /// <summary>
        /// Can edit notification templates
        /// </summary>
        IM1,
        /// <summary>
        /// Can edit training period calendar
        /// </summary>
        IM2,
        /// <summary>
        /// SM – is a system manager for the system
        /// </summary>
        SM
    }
}