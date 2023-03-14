namespace CJG.Web.External.Areas.Ext.Models.ParticipantReporting
{
	public class ParticipantWarningModel
	{
		private const decimal NearingLimitPercentage = 0.10M;

		public string ParticipantName { get; set; }
		public decimal FiscalYearLimit { get; set; }
		public decimal CurrentClaims { get; set; }
		public bool ParticipantIsNearingLimit => IsNearingLimit();
		public bool ParticipantIsOverLimit => IsOverLimit();

		public bool IsNearingLimit()
		{
			var nearingAmount = FiscalYearLimit - (FiscalYearLimit * NearingLimitPercentage);

			if (CurrentClaims < nearingAmount)
				return false;

			if (IsOverLimit())
				return false;

			if (CurrentClaims < nearingAmount)
				return false;

			return true;
		}

		public bool IsOverLimit()
		{
			return CurrentClaims >= FiscalYearLimit;
		}
	}
}