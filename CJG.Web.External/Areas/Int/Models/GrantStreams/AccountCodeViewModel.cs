using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Int.Models.GrantStreams
{
	public class AccountCodeViewModel : BaseViewModel
	{
		#region Properties
		[Display(Name = "Client Numb")]
		public string GLClientNumber { get; set; }
		[Display(Name = "RESP")]
		public string GLRESP { get; set; }
		[Display(Name = "Service Line")]
		public string GLServiceLine { get; set; }
		[Display(Name = "STOB Normal")]
		public string GLSTOBNormal { get; set; }
		[Display(Name = "STOB Accrual")]
		public string GLSTOBAccrual { get; set; }
		[Display(Name = "Project Code")]
		public string GLProjectCode { get; set; }
		#endregion

		#region Constructors
		public AccountCodeViewModel() { }

		public AccountCodeViewModel(AccountCode accountCode)
		{
			if (accountCode == null) throw new ArgumentNullException(nameof(accountCode));

			Utilities.MapProperties(accountCode, this);
		}
		#endregion
	}
}