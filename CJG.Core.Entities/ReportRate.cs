using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="ReportRate"/> class, provides ORM a way to manage Report rates.
    /// </summary>
    public class ReportRate : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key and foreign key to the fiscal year.
        /// </summary>
        [Key, Column(Order = 1)]
        public int FiscalYearId { get; set; }

        /// <summary>
        /// get/set - The parent fiscal year.
        /// </summary>
        [ForeignKey(nameof(FiscalYearId))]
        public virtual FiscalYear FiscalYear { get; set; }

        /// <summary>
        /// get/set - The primary key and foreign key to the grant program.
        /// </summary>
        [Key, Column(Order = 2)]
        public int? GrantProgramId { get; set; }

        /// <summary>
        /// get/set - The parent grant program.
        /// </summary>
        [ForeignKey(nameof(GrantProgramId))]
        public virtual GrantProgram GrantProgram { get; set; }

        /// <summary>
        /// get/set - The primary key and foreign key to the grant stream.
        /// </summary>
        [Key, Column(Order = 3)]
        public int GrantStreamId { get; set; }

        /// <summary>
        /// get/set - The Grant Stream that these report rates are linked to.
        /// </summary>
        public GrantStream GrantStream { get; set; }

        /// <summary>
        /// get/set - The agreement cancellation rate.
        /// </summary>
        [Min(0, ErrorMessage = "The agreement cancellation rate cannot be less than 0.")]
        public double AgreementCancellationRate { get; set; }

        /// <summary>
        /// get/set - The agreement slippage rate.
        /// </summary>
        [Min(0, ErrorMessage = "The agreement slippage rate cannot be less than 0.")]
        public double AgreementSlippageRate { get; set; }

        /// <summary>
        /// get/set - The claim slippage rate.
        /// </summary>
        [Min(0, ErrorMessage = "The claim slippage rate cannot be less than 0.")]
        public double ClaimSlippageRate { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="ReportRate"/> object.
        /// </summary>
        public ReportRate()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantStream"/> object.
        /// </summary>
        /// <param name="fiscal"></param>
        /// <param name="program"></param>
        /// <param name="stream"></param>
        public ReportRate(FiscalYear fiscal, GrantProgram program, GrantStream stream)
        {
            this.FiscalYear = fiscal ?? throw new ArgumentNullException(nameof(fiscal));
            this.FiscalYearId = fiscal.Id;
            this.GrantProgram = program ?? throw new ArgumentNullException(nameof(program));
            this.GrantProgramId = program.Id;
            this.GrantStream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.GrantStreamId = stream.Id;
        }
        #endregion

        #region Methods
        #endregion
    }
}
