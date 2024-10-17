using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="TempData"/> class, provides a generic structure that can be used to store temporary data of specific types related to a parent entity.
    /// This allows for partial data to be saved and managed before the full dataset is saved to the appropriate tables.
    /// <typeparamref name="TempData"/> provides a way to mimic the real (full) data objects without enforcing all the constraints.
    /// </summary>
    /// <example>
    /// If a user wants to save their form values but they are currently not valid for the table you can save it as temporary data.
    /// </example>
    [Table(nameof(TempData))]
    public sealed class TempData : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key and way to identify the parent of this data.
        /// </summary>
        [Key, Column(Order = 1), MaxLength(50)]
        public string ParentType { get; set; }

        /// <summary>
        /// get/set - The primary key and way to identify the parent id.
        /// </summary>
        [Key, Column(Order = 2)]
        public int ParentId { get; set; }

        /// <summary>
        /// get/set - The primary key and a way to specify the type of data.
        /// </summary>
        [Key, Column(Order = 3), MaxLength(150)]
        public string DataType { get; set; }

        /// <summary>
        /// get/set - The data to be temporarily stored.
        /// </summary>
        [Required, Column(TypeName = "XML")]
        public string Data { get; set; }

        /// <summary>
        /// get/set - The data as XML.
        /// </summary>
        [NotMapped]
        public XElement Xml
        {
            get { return XElement.Parse(this.Data); }
            set { this.Data = value.ToString(); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="TempData"/> object.
        /// </summary>
        public TempData()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="TempData"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="parentId"></param>
        /// <param name="dataType"></param>
        /// <param name="data"></param>
        public TempData(Type parentType, int parentId, Type dataType, string data)
        {
            if (parentType == null)
                throw new ArgumentNullException(nameof(parentType));

            if (parentId <= 0)
                throw new ArgumentOutOfRangeException(nameof(parentId));

            if (dataType == null)
                throw new ArgumentNullException(nameof(dataType));

            this.ParentType = parentType.Name;
            this.ParentId = parentId;
            this.DataType = dataType.FullName;
            this.Data = data ?? throw new ArgumentNullException(nameof(data));
        }
        #endregion
    }
}
