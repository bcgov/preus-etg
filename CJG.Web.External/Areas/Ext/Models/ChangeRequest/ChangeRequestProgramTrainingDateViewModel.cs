using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Ext.Models.ChangeRequest
{
	public class ChangeRequestProgramTrainingDateViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		#endregion

		#region Constructors
		public ChangeRequestProgramTrainingDateViewModel()
		{

		}

		public ChangeRequestProgramTrainingDateViewModel(TrainingProgram trainingProgram)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));
			this.Id = trainingProgram.Id;
			this.StartDate = trainingProgram.StartDate.ToLocalTime();
			this.EndDate = trainingProgram.EndDate.ToLocalTime();
			this.RowVersion = Convert.ToBase64String(trainingProgram.RowVersion);
		}
		#endregion
	}
}