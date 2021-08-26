using CJG.Core.Entities;
using DataAnnotationsExtensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class EligibleCostViewModel
    {
        #region Properties
        public int Id { get; set; }

        public int GrantApplicationId { get; set; }

        public int EligibleExpenseTypeId { get; set; }

        [NotMapped]
        public virtual EligibleExpenseType EligibleExpenseType { get; set; }

        /// <summary>
        /// get/set - The estimated total cost.
        /// </summary>
        [Required, Min(0, ErrorMessage = "The estimated cost must be greater than or equal to 0.")]
        public decimal EstimatedCost { get; set; }

        /// <summary>
        /// get/set - The estimated number of participants.
        /// </summary>
        [Required, Min(1, ErrorMessage = "The estimated participants must be greater than or equal to 1.")]
        public int EstimatedParticipants { get; set; }

        /// <summary>
        /// get/set - This is a calculated amount (EstimatedCost / EstimatedParticipants).
        /// </summary>
        [Required, Min(0, ErrorMessage = "The estimated participant cost must be greater than or equal to 0.")]
        public decimal EstimatedParticipantCost { get; set; }

        /// <summary>
        /// get/set - This is a calculated amount (EstimatedParticipantCost * Stream.ReimbursementRate).
        /// </summary>
        [Required, Min(0, ErrorMessage = "The estimated reimbursement must be greater than or equal to 0.")]
        public decimal EstimatedReimbursement { get; set; }

        /// <summary>
        /// get/set - This is a calculated amount (EstimatedCost - EstimatedReimbursement).
        /// </summary>
        [Required, Min(0, ErrorMessage = "The estimated employer contribution must be greater than or equal to 0.")]
        public decimal EstimatedEmployerContribution { get; set; }

        #region Assessment
        /// <summary>
        /// get/set - The agreed maximum cost.
        /// </summary>
        [Required, Min(0, ErrorMessage = "The agreed max cost must be greater than or equal to 0.")]
        public decimal AgreedMaxCost { get; set; }

        /// <summary>
        /// get/set - The agreed maximum number of participants.
        /// </summary>
        [Required, Min(0, ErrorMessage = "The agreed max participants must be greater than or equal to 0.")]
        public int AgreedMaxParticipants { get; set; }

        /// <summary>
        /// get/set - This is a calculated amount (AgreedMaxCost / AgreedMaxParticipants).
        /// </summary>
        [Required, Min(0, ErrorMessage = "The agreed max participant cost must be greater than or equal to 0.")]
        public decimal AgreedMaxParticipantCost { get; set; }

        /// <summary>
        /// get/set - This is a calculated amount (AgreedMaxParticipantCost * Stream.ReimbursementRate).
        /// </summary>
        [Required, Min(0, ErrorMessage = "The agreed max reimbursement must be greater than or equal to 0.")]
        public decimal AgreedMaxReimbursement { get; set; }

        /// <summary>
        /// get/set - This is a calculated amount (AgreedMaxCost - AgreedMaxReimbursement).
        /// </summary>
        [Required, Min(0, ErrorMessage = "The agreed employer contribution must be greater than or equal to 0.")]
        public decimal AgreedEmployerContribution { get; set; }
        #endregion

        public byte[] RowVersion { get; set; }
        #endregion

        #region Constructors
        public EligibleCostViewModel()
        { }

        public EligibleCostViewModel(EligibleCost eligibleCost)
        {
            if (eligibleCost == null)
                throw new ArgumentNullException(nameof(eligibleCost));

            this.Id = eligibleCost.Id;
            this.GrantApplicationId = eligibleCost.GrantApplicationId;
            this.EligibleExpenseTypeId = eligibleCost.EligibleExpenseTypeId;
            this.EligibleExpenseType = eligibleCost.EligibleExpenseType;
            this.EstimatedCost = eligibleCost.EstimatedCost;
            this.EstimatedParticipants = eligibleCost.EstimatedParticipants;
            this.EstimatedParticipantCost = eligibleCost.EstimatedParticipantCost;
            this.EstimatedReimbursement = eligibleCost.EstimatedReimbursement;
            this.EstimatedEmployerContribution = eligibleCost.EstimatedEmployerContribution;
            this.AgreedMaxCost = eligibleCost.AgreedMaxCost;
            this.AgreedMaxParticipants = eligibleCost.AgreedMaxParticipants;
            this.AgreedMaxParticipantCost = eligibleCost.AgreedMaxParticipantCost;
            this.AgreedMaxReimbursement = eligibleCost.AgreedMaxReimbursement;
            this.AgreedEmployerContribution = eligibleCost.AgreedEmployerContribution;
            this.RowVersion = eligibleCost.RowVersion;
        }
        #endregion
    }
}