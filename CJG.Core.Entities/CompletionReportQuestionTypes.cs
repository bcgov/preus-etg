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
		NOCAndNAICSList = 5
	}
}
