using CJG.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace CJG.Web.External.Areas.Ext.Models.TrainingPrograms
{
	public class CourseLinkValidation
	{
		public static ValidationResult ValidateCourseLink(string courseLink, ValidationContext context)
		{
			TrainingProgramViewModel model = context.ObjectInstance as TrainingProgramViewModel;
			if (model == null) throw new ArgumentNullException();
			ValidationResult result = ValidationResult.Success;

			if (!(string.IsNullOrEmpty(courseLink) || IsValidLink(courseLink)))
			{
				result = new ValidationResult("You must enter a valid URL format.");
			}
			else
			{
				var reg = new Regex(@"^((?:https?:\/\/)?[^./]+(?:\.[^./]+)+(?:\/.*)?)$");
				if (!(string.IsNullOrEmpty(courseLink)) && !reg.IsMatch(courseLink))
				{

					result = new ValidationResult("You must enter a valid URL format.");
				}
			}
			return result;
		}

		private static bool IsValidLink(string courseLink)
		{
			Uri uriResult;
			bool result = Uri.TryCreate("http://" + courseLink, UriKind.Absolute, out uriResult)
				&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

			return result;
		}
	}
}