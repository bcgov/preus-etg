using CJG.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class TrainingPeriodViewModel
    {
        #region Properties
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Caption { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime DefaultPublishDate { get; set; }

        [Required]
        public DateTime DefaultOpeningDate { get; set; }

        public int FiscalYearId { get; set; }

        [NotMapped]
        public virtual FiscalYearViewModel FiscalYear { get; set; }

        public byte[] RowVersion { get; set; }
        #endregion

        #region Constructors
        public TrainingPeriodViewModel()
        { }

        public TrainingPeriodViewModel(TrainingPeriod trainingPeriod)
        {
            if (trainingPeriod == null)
                throw new ArgumentNullException(nameof(trainingPeriod));

            this.Id = trainingPeriod.Id;
            this.Caption = trainingPeriod.Caption;
            this.StartDate = trainingPeriod.StartDate;
            this.EndDate = trainingPeriod.EndDate;
            this.DefaultPublishDate = trainingPeriod.DefaultPublishDate;
            this.DefaultOpeningDate = trainingPeriod.DefaultOpeningDate;
            this.FiscalYearId = trainingPeriod.FiscalYearId;
            this.FiscalYear = trainingPeriod.FiscalYear != null ? new FiscalYearViewModel(trainingPeriod.FiscalYear) : null;
            this.RowVersion = trainingPeriod.RowVersion;
        }
        #endregion

        #region Methods
        public static implicit operator TrainingPeriod(TrainingPeriodViewModel model)
        {
            if (model == null)
                return null;

            return new TrainingPeriod
            {
                Id = model.Id,
                Caption = model.Caption,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DefaultPublishDate = model.DefaultPublishDate,
                DefaultOpeningDate = model.DefaultOpeningDate,
                FiscalYearId = model.FiscalYearId,
                FiscalYear = model.FiscalYear,
                RowVersion = model.RowVersion
            };
        }
        #endregion
    }
}