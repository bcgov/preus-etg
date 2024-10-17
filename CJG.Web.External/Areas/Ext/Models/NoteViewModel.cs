using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class NoteViewModel
    {
        public InternalUser CurrentUser { get; set; }
        public IEnumerable<NoteType> NoteTypes { get; set; }
        public int GrantApplicationId { get; set; }
        public string AllowFileAttachmentExtensions { get; set; }
    }
}