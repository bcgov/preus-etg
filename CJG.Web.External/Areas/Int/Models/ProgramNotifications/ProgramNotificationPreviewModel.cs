using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.ProgramNotifications
{
	public class ProgramNotificationPreviewModel
	{
		#region Properties
		public string Name { get; set; }
		public string Description { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		#endregion

		#region Constructors
		public ProgramNotificationPreviewModel() { }
		#endregion
	}
}
