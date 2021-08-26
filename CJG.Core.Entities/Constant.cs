using System;

namespace CJG.Core.Entities
{
    /// <summary>
    /// Shared constant values.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Postal code regex.
        /// </summary>
        public const string PostalCodeValidationRegEx =
          "^[abceghjklmnprstvxyABCEGHJKLMNPRSTVXY][0-9][abceghjklmnprstvwxyzABCEGHJKLMNPRSTVWXYZ]\\s?[0-9][abceghjklmnprstvwxyzABCEGHJKLMNPRSTVWXYZ][0-9]$";

        /// <summary>
        /// International zip code regex.
        /// </summary>
        public const string InternationalPostalCodeValidationRegEx = "^[a-zA-Z0-9 \\-\\,]*$";

        /// <summary>
        /// Canada's country code ID.
        /// </summary>
        public const string CanadaCountryId = "CA";

        /// <summary>
        /// United States country code ID.
        /// </summary>
        [Obsolete("Need to redesign this.")]
        public const string USCountryID = "US";

        /// <summary>
        /// Bosnia's country code ID.
        /// </summary>
        [Obsolete("Need to redesign this.")]
        public const string BosniaCountryID = "BA";

        /// <summary>
        /// China's country code Id.
        /// </summary>
        [Obsolete("Need to redesign this.")]
        public const string ChinaCountryID = "CN";

        /// <summary>
        /// Poland's country code ID.
        /// </summary>
        [Obsolete("Need to redesign this.")]
        public const string PolandCountryID = "PL";
		
        /// <summary>
        /// The completion report step with multiple questions.
        /// </summary>
        [Obsolete("Need to redesign this.")]
        public const int CompletionStepWithMultipleQuestions = 3;

        [Obsolete("Need to redesign this.")]
        public const string ExcludeFromIntakeSum = "Reductions";

        /// <summary>
        /// the maximum number of documents which can be uploaded by the applicant for each claim.
        /// </summary>
        public const int MaximumNumberOfAttachmentsPerClaim = 50;

    }
}
