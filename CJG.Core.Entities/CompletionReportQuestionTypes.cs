namespace CJG.Core.Entities
{
    /// <summary>
    /// CompletionReportQuestionTypes enum, provides a valid list of question types.
    /// </summary>
    public enum CompletionReportQuestionTypes
    {
        /// <summary>
        /// Default - A default question type.
        /// </summary>
        Default = 1,
        /// <summary>
        /// MultipleChoice - Choose one from a list.
        /// </summary>
        MultipleChoice = 2,
        /// <summary>
        /// Freeform - Enter text to answer question.
        /// </summary>
        Freeform = 3,
		/// <summary>
		/// DynamicCheckbox - Dynamic checkbox selection.
		/// </summary>
		DynamicCheckbox = 4,
		/// <summary>
		/// NOCAndNAICSList - NOC and NAICS selection.
		/// </summary>
		NOCAndNAICSList = 5,
		/// <summary>
		/// CommunityList - Community selection.
		/// </summary>
		CommunityList = 6,
		/// <summary>
		/// NAICSList - NAICS selection (single control).
		/// </summary>
		NAICSList = 7,
		/// <summary>
		/// NOCList - NOC selection (single control).
		/// </summary>
		NOCList = 8,
		/// <summary>
		/// MultipleCheckbox - Multiple checkboxes may be selected
		/// </summary>
		MultipleCheckbox = 9

	}
}
