namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="TrainingProviderAttachmentType"/> enum, provides all the possible training provider attachment types.
    /// </summary>
    public enum TrainingProviderAttachmentTypes
    {
        /// <summary>
        /// Unset, empty or unused value
        /// </summary>
        Void = 0,
        /// <summary>
        /// Training Providers Proof of Qualifications
        /// </summary>
        ProofOfQualifications = 1,
        /// <summary>
        /// Course Outline
        /// </summary>
        CourseOutline = 2,
        /// <summary>
        /// Business Case when the training provider is outside BC
        /// </summary>
        BusinessCase = 3
    }
}
