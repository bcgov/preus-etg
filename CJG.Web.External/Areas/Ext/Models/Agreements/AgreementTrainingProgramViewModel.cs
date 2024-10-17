using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Ext.Models.Agreements
{
	public class AgreementTrainingProgramViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }

		public int GrantApplicationId { get; set; }

		public string CourseTitle { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public AgreementTrainingProviderViewModel TrainingProvider { get; set; }

		public AgreementTrainingProviderViewModel RequestedTrainingProvider { get; set; }
		#endregion

		#region Constructors
		public AgreementTrainingProgramViewModel() { }

		public AgreementTrainingProgramViewModel(TrainingProgram trainingProgram)
		{
			if (trainingProgram == null) throw new ArgumentNullException(nameof(trainingProgram));

			this.Id = trainingProgram.Id;
			this.RowVersion = Convert.ToBase64String(trainingProgram.RowVersion);
			this.GrantApplicationId = trainingProgram.GrantApplicationId;
			this.CourseTitle = trainingProgram.CourseTitle;
			this.StartDate = trainingProgram.StartDate;
			this.EndDate = trainingProgram.EndDate;
			this.TrainingProvider = new AgreementTrainingProviderViewModel(trainingProgram);
			if (trainingProgram.RequestedTrainingProvider != null)
				this.RequestedTrainingProvider = new AgreementTrainingProviderViewModel(trainingProgram.RequestedTrainingProvider);
		}
		#endregion
	}
}