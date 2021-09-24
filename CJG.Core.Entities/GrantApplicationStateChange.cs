using CJG.Core.Entities.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="GrantApplicationStateChange"/> class, provides an EF entity to manage GrantApplication state changes.
    /// </summary>
    public class GrantApplicationStateChange : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key to identify this state change.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// get/set - The foreign key to the associated grant application.
        /// </summary>
        [ForeignKey(nameof(GrantApplication))]
        public int GrantApplicationId { get; set; }

        /// <summary>
        /// get/set - The associated grant application.
        /// </summary>
        public virtual GrantApplication GrantApplication { get; set; }

        /// <summary>
        /// get/set - The original state the grant application is changing from.
        /// </summary>
        public ApplicationStateInternal FromState { get; set; }

        /// <summary>
        /// get/set - The state the grant application is changing to.
        /// </summary>
        public ApplicationStateInternal ToState { get; set; }

        /// <summary>
        /// get/set - The date the change occured.
        /// </summary>
        [DateTimeKind(DateTimeKind.Utc)]
        public DateTime ChangedDate { get; set; }

        /// <summary>
        /// get/set - The reason the state change occured.
        /// </summary>
        [MaxLength(2000)]
        public string Reason { get; set; }

        /// <summary>
        /// get/set - The foreign key for the application administrator who performed the change.
        /// </summary>
        public int? ApplicationAdministratorId { get; set; }

        /// <summary>
        /// get/set - The application administrator who performed the change.
        /// </summary>
        [ForeignKey(nameof(ApplicationAdministratorId))]
        public User ApplicationAdministrator { get; set; }

        /// <summary>
        /// get/set - The foreign key for the assessor who performed the change.
        /// </summary>
        public int? AssessorId { get; set; }

        /// <summary>
        /// get/set - The assessor who performed the change.
        /// </summary>
        [ForeignKey(nameof(AssessorId))]
        public virtual InternalUser Assessor { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantApplicationStateChange"/> object.
        /// </summary>
        public GrantApplicationStateChange()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantApplicationStateChange"/> object.
        /// </summary>
        /// <param name="grantApplication"></param>
        /// <param name="fromState"></param>
        /// <param name="toState"></param>
        /// <param name="user"></param>
        /// <param name="reason"></param>
        public GrantApplicationStateChange(GrantApplication grantApplication, ApplicationStateInternal fromState, ApplicationStateInternal toState, InternalUser user, string reason)
        {
            this.GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
            this.GrantApplicationId = grantApplication.Id;
            this.FromState = fromState;
            this.ToState = toState;
            this.Assessor = user ?? throw new ArgumentNullException(nameof(user));
            this.AssessorId = user.Id;
            this.Reason = reason;
            this.ChangedDate = AppDateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantApplicationStateChange"/> object.
        /// </summary>
        /// <param name="grantApplication"></param>
        /// <param name="fromState"></param>
        /// <param name="toState"></param>
        /// <param name="user"></param>
        /// <param name="reason"></param>
        public GrantApplicationStateChange(GrantApplication grantApplication, ApplicationStateInternal fromState, InternalUser user, string reason) : this(grantApplication, fromState, grantApplication.ApplicationStateInternal, user, reason)
        {
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantApplicationStateChange"/> object.
        /// </summary>
        /// <param name="grantApplication"></param>
        /// <param name="fromState"></param>
        /// <param name="toState"></param>
        /// <param name="user"></param>
        /// <param name="reason"></param>
        public GrantApplicationStateChange(GrantApplication grantApplication, ApplicationStateInternal fromState, ApplicationStateInternal toState, User user, string reason)
        {
            this.GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
            this.GrantApplicationId = grantApplication.Id;
            this.FromState = fromState;
            this.ToState = toState;
            this.ApplicationAdministrator = user ?? throw new ArgumentNullException(nameof(user));
            this.ApplicationAdministratorId = user.Id;
            this.Reason = reason;
            this.ChangedDate = AppDateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantApplicationStateChange"/> object.
        /// </summary>
        /// <param name="grantApplication"></param>
        /// <param name="fromState"></param>
        /// <param name="toState"></param>
        /// <param name="user"></param>
        /// <param name="reason"></param>
        public GrantApplicationStateChange(GrantApplication grantApplication, ApplicationStateInternal fromState, User user, string reason) : this(grantApplication, fromState, grantApplication.ApplicationStateInternal, user, reason)
        {
        }
        #endregion
    }
}
