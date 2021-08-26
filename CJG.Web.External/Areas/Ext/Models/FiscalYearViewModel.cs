using CJG.Core.Entities;
using DataAnnotationsExtensions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class FiscalYearViewModel
    {
        #region Properties
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Caption { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required, Min(1), DefaultValue(1)]
        public int NextAgreementNumber { get; set; } = 1;

        public byte[] RowVersion { get; set; }
        #endregion

        #region Constructors
        public FiscalYearViewModel()
        {

        }

        public FiscalYearViewModel(FiscalYear fiscalYear)
        {
            if (fiscalYear == null)
                throw new ArgumentNullException(nameof(fiscalYear));

            this.Id = fiscalYear.Id;
            this.Caption = fiscalYear.Caption;
            this.StartDate = fiscalYear.StartDate;
            this.EndDate = fiscalYear.EndDate;
            this.NextAgreementNumber = fiscalYear.NextAgreementNumber;
            this.Id = fiscalYear.Id;
            this.RowVersion = fiscalYear.RowVersion;
        }
        #endregion

        #region Methods
        public static implicit operator FiscalYear(FiscalYearViewModel model)
        {
            if (model == null)
                return null;

            var fiscalYear = new FiscalYear();
            fiscalYear.Id = model.Id;
            fiscalYear.Caption = model.Caption;
            fiscalYear.StartDate = model.StartDate;
            fiscalYear.EndDate = model.EndDate;
            fiscalYear.NextAgreementNumber = model.NextAgreementNumber;
            fiscalYear.Id = model.Id;
            fiscalYear.RowVersion = model.RowVersion;
            return fiscalYear;
        }
        #endregion
    }
}