using DataAnnotationsExtensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="RateFormat"/> class, provides the ORM a way to manage rate display formats.
    /// </summary>
    public class RateFormat : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key and rate value.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Min(0)]
        public double Rate { get; set; }

        /// <summary>
        /// get/set - The format of the rate as text.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "A format is required"), MaxLength(50)]
        public string Format { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="RateFormat"/> object.
        /// </summary>
        public RateFormat()
        { }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="RateFormat"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="format"></param>
        public RateFormat(double rate, string format)
        {
            if (rate < 0)
                throw new ArgumentException("Rate must be greater than or equal to 0.", nameof(rate));

            if (String.IsNullOrEmpty(format))
                throw new ArgumentNullException(nameof(format));

            this.Rate = rate;
            this.Format = format;
        }
        #endregion
    }
}
