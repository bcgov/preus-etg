namespace CJG.Web.External.Areas.Int.Models
{
	public class ParticipantWarningModel
	{
		public int MappedParticipantFormId { get; set; }
		public decimal CurrentClaims { get; set; }
		public decimal FiscalYearLimit { get; set; }
		public decimal CostsOnThisClaim { get; set; }

		public bool HasWarning()
		{
			return CostsOnThisClaim + CurrentClaims > FiscalYearLimit;
		}
	}
}