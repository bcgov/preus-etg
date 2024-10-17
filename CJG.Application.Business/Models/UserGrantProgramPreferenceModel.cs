namespace CJG.Application.Business.Models
{
	public class UserGrantProgramPreferenceModel
	{
		public int ProgramId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsNew { get; set; }
		public bool IsSelected { get; set; }
		public bool IsImplemented { get; set; }
	}
}
