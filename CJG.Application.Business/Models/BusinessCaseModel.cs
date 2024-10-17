using CJG.Core.Entities;
using System;

namespace CJG.Application.Business.Models
{
	public class BusinessCaseModel
	{
		#region Properties
		public int Id { get; set; }
		public string FileName { get; set; }
		public string Description { get; set; }
		public string RowVersion { get; set; }
		#endregion

		#region Constructors
		public BusinessCaseModel()
		{

		}
		#endregion
	}
}
