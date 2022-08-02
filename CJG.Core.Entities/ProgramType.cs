using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// Provides a way to control program application development behaviour.
    /// </summary>
    public enum ProgramTypes
    {
        /// <summary>
        /// EmployerGrant (ETG) - This program type is the original and only allows a single training program and training provider.
        /// </summary>
        EmployerGrant = 1,
        /// <summary>
        /// WDAService (CWRG) - This program type allows for a configurable application process through service categories and service lines.  This allows for multiple training providers and training programs.
        /// </summary>
        WDAService = 2
    }

    /// <summary>
    /// ProgramType provides a way to control program application development behaviour.
    /// </summary>
    public class ProgramType : LookupTable<int>
    {
	    /// <summary>
        /// get/set - The primary key does not use IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new ProgramTypes Id { get; set; }

        /// <summary>
        /// get/set - A description of this program type.
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// get - The grant programs that use this program type.
        /// </summary>
        public virtual ICollection<GrantProgram> GrantPrograms { get; set; } = new List<GrantProgram>();

        /// <summary>
        /// Creates a new instance of a ProgramType object.
        /// </summary>
        public ProgramType()
        {

        }

        /// <summary>
        /// Creates a new instance of a ProgramType object and initializes it.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isActive"></param>
        public ProgramType(ProgramTypes type, bool isActive = true) : base(type.GetDescription())
        {
            IsActive = isActive;
        }
    }
}
