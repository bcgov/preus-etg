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
        public const string PostalCodeValidationRegEx = "^[abceghjklmnprstvxyABCEGHJKLMNPRSTVXY][0-9][abceghjklmnprstvwxyzABCEGHJKLMNPRSTVWXYZ]\\s?[0-9][abceghjklmnprstvwxyzABCEGHJKLMNPRSTVWXYZ][0-9]$";

		/// <summary>
		/// City Name Validation Regex
		/// </summary>
        public const string CityNameValidationRegEx = "^([a-zA-Z\u0080-\u024F]+(?:. |-| |'))*[a-zA-Z\u0080-\u024F]*$";

		/// <summary>
		/// Province Name Validation Regex
		/// </summary>
        public const string ProvinceNameValidationRegEx = "^[a-zA-Z -]*$";

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

		// == Completion report variables ==

		/// <summary>
		/// The completion report step with multiple questions.
		/// </summary>
		[Obsolete("Need to redesign this.")]
        public const int CompletionStepWithMultipleQuestions = 3;

		/// <summary>
		/// Completion report: Two different types of report. Values represent CompletionReports.Id
		/// </summary>
		public const int CompletionReportETG = 1;
		public const int CompletionReportCWRG = 2;

		/// <summary>
		/// Key used to determine which completion ID to use, when creating a grant application. This is the CWRG grant type
		/// which means we should use the CWRG completion report.
		/// </summary>
		public const int GrantProgramNameCWRGIdKey = 3;

		/// <summary>
		/// Completion report: Page numbers, which represent the GroupIds (CompletionReportGroups.Id) of the questions
		/// </summary>
		public const int CompletionReportETGPage1 = 1;
		public const int CompletionReportETGPage2 = 2;
		public const int CompletionReportETGPage3 = 3;
		public const int CompletionReportETGPage4 = 4;
		public const int CompletionReportETGPage5 = 5;
		public const int CompletionReportCWRGPage1 = 6;
		public const int CompletionReportCWRGPage2 = 7;
		public const int CompletionReportCWRGPage3 = 8;
		public const int CompletionReportCWRGPage4 = 9;

		public const int CompletionReportQuestionIntakePeriods = 32;
		[Obsolete("Need to redesign this.")]
        public const string ExcludeFromIntakeSum = "Reductions";

        /// <summary>
        /// the maximum number of documents which can be uploaded by the applicant for each claim.
        /// </summary>
        public const int MaximumNumberOfAttachmentsPerClaim = 50;

		/// <summary>
		/// Constants for the delivery type, based on the ID of the [DeliveryMethods] table.
		/// </summary>
		public const int Delivery_Classroom = 1;
		public const int Delivery_Workplace = 2;
		public const int Delivery_Online = 3;
	}
}
