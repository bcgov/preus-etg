using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.Notes
{
	public class NoteListViewModel: BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public IEnumerable<NoteViewModel> Notes { get; set; }

		public string PermittedAttachmentTypes { get; set; }
		public int MaxUploadSize { get; set; }
		#endregion

		#region Constructors
		public NoteListViewModel() { }

		public NoteListViewModel(GrantApplication grantApplication, IPrincipal user)
		{
			if (grantApplication == null) throw new ArgumentNullException(nameof(grantApplication));

			this.Id = grantApplication.Id;
			this.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			this.Notes = grantApplication.Notes.OrderByDescending(n => n.DateAdded).Select(n => new NoteViewModel(n, user, false)).ToArray();
			this.MaxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			this.PermittedAttachmentTypes = ConfigurationManager.AppSettings["PermittedAttachmentTypes"];
		}
		#endregion
	}
}