using CJG.Core.Entities;
using CJG.Core.Interfaces;
using Microsoft.CSharp.RuntimeBinder;
using RazorEngine.Templating;
using System;

namespace CJG.Application.Services
{
	public class TemplateException : Exception
	{
		#region Properties
		/// <summary>
		/// get/set - The display name.
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// get/set - The property name of the template.
		/// </summary>
		public string PropertyName { get; set; }
		#endregion

		#region Constructors
		

		public TemplateException(string propertyName, string displayName, TemplateCompilationException innerException) : base("The document template failed to compile.", innerException)
		{
			this.PropertyName = propertyName;
			this.DisplayName = displayName;
		}

		public TemplateException(string propertyName, string displayName, TemplateParsingException innerException) : base("The document template failed to parse.", innerException)
		{
			this.PropertyName = propertyName;
			this.DisplayName = displayName;
		}

		public TemplateException(string propertyName, string displayName, RuntimeBinderException innerException) : base("The document template failed to compile.", innerException)
		{
			this.PropertyName = propertyName;
			this.DisplayName = displayName;
		}


		public TemplateException(DocumentTypes documentType, TemplateCompilationException innerException) : this(documentType.ToString(), documentType.GetDescription(), innerException)
		{
		}

		public TemplateException(DocumentTypes documentType, TemplateParsingException innerException) : this(documentType.ToString(), documentType.GetDescription(), innerException)
		{
		}
		#endregion

		#region Methods
		public string GetErrorMessages()
		{
			if (this.InnerException is TemplateCompilationException)
				return this.Message + "<br/>" + ((TemplateCompilationException)this.InnerException).GetErrorMessages();
			return this.GetAllMessages();
		}
		#endregion
	}
}
