using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Newtonsoft.Json;

namespace CJG.Web.External.Helpers
{
    public static class FormControlHelper
	{
		public static MvcHtmlString DatePickerDropDownList(this HtmlHelper helper, DatePickerFormControl control)
		{
			var html = "<div class=\"form__group form__group--date\">" +
							(control.Label == null ? "" : "<label class=\"form__label\">" + control.Label + (control.IsRequired ? " <abbr title=\"Required\">*</abbr>" : "") + "</label>") +
							(control.Description == null ? "" : ("<p>" + control.Description + "</p>")) +
							"<div class=\"form__control--flexible\""
							  + (control.EnableTextSwitch.Enabled ? " ng-if=\"" + control.EnableTextSwitch.EditCondition + "\"" : "") + ">" +
								"<div class=\"field__date ignore-parent datefield js-datefield--dob multi-field-validation\"" + (control.IsDisabled ? "data-disabled = 'true'" : "") + (control.CustomAttributes == null ? "" : " " + control.CustomAttributes) + ">" +
									"<div class=\"validation-group\" ng-class=\"{'has-error':" + GetErrorMessage(control.Date, true) + "}\">" +
										"<div class=\"selectmenu field__date--month\">" +
											"<select data-bind=\"" + control.Id + "-Month\" data-validate=\"false\"></select>" +
										"</div>" +
										"<div class=\"selectmenu field__date--day\">" +
											"<select data-bind=\"" + control.Id + "-Day\" data-validate=\"false\"></select>" +
										"</div>" +
										"<div class=\"selectmenu field__date--year\">" +
												"<select data-bind=\"" + control.Id + "-Year\" data-validate=\"false\"" +
												(control.StartYear == null ? "" : "data-start=\"" + control.StartYear + "\"") +
												(control.EndYear == null ? "" : "data-end=\"" + control.EndYear + "\"") + "></select>" +
										"</div>" +
									"</div>" +
									(control.Date == null ? "" :
									("<input type = \"hidden\" id=\"" + control.Id + "-Month\" value = \"{{" + (control.Month ?? "calculatedMonth(" + control.Date + ", 0)") + "}}\"/>" +
									"<input type = \"hidden\" id=\"" + control.Id + "-Day\" value =\"{{" + (control.Day ?? "calculatedDay(" + control.Date + ", 0)") + "}}\"/>" +
									"<input type = \"hidden\" id=\"" + control.Id + "-Year\" value =\"{{" + (control.Year ?? "calculatedYear(" + control.Date + ", 0)") + "}}\"/>")) +
								"</div>" +
								"<div class=\"block__form--message-wrapper\">" +
									helper.ValidationError(control.Date, true, control.ErrorMessageModelPrefix).ToHtmlString() +
								"</div>" +
							"</div>" +
							(control.EnableTextSwitch.Enabled ? "<div ng-if=\"" + control.EnableTextSwitch.TextCondition + "\" class=\"form__control\">{{ " + control.Date + " | date }}</div>" : "") +
						"</div>";

			return new MvcHtmlString(html);
		}

		public static MvcHtmlString DropDownList(this HtmlHelper helper, FormControl control)
		{
			TagBuilder div = new TagBuilder("div");

			div.Attributes.Add("class", "form__group" + (control.CssClass == null ? "" : " " + control.CssClass) + (control.Required ? " required" : ""));

			div.Attributes.Add("ng-class", "{\"has-error\":" + control.Model + "Error}");

			if (control.HtmlAttributes != null)
				div.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(control.HtmlAttributes));

			if (control.Inline)
			{
				div.InnerHtml = "<div class=\"label-wrapper\""
							  + (control.EnableTextSwitch.Enabled && control.EnableTextSwitch.HideLabel ? " ng-if=\"" + control.EnableTextSwitch.EditCondition + "\"" : "")
							  + "><label class=\"form__label\">" + control.Label + "</label></div>"
							  + "<div class=\"control-wrapper\"><div class=\"selectmenu " + (control.Size ?? "input--medium") + "\""
							  + (control.EnableTextSwitch.Enabled ? " ng-if=\"" + control.EnableTextSwitch.EditCondition + "\"" : "")
							  + "> <select id=\""+ control.Id + "\" class=\"" + (control.Size ?? "input--medium") + " form-control\" ng-model=\"" + control.Model
							  + "\" ng-options=\"" + control.Options + "\"" + (control.OnChange == null ? "" : " ng-change=\"" + control.OnChange + "\"")
							  + "ng-class=\"{'has-error':" + control.ErrorMessageModel + "ErrorMessage}\""
							  + (control.CustomAttributes == null ? "" : " " + control.CustomAttributes)
							  + (control.DefaultOverride ? " ng-init=\"" + control.Model + "=null\"" : "")
							  + ">"
							  + (control.ShowDefaultPlaceHolder ? "<option value=\"\" selected>" + (string.IsNullOrEmpty(control.Placeholder) ? "< Select value >" : control.Placeholder) + "</option>" : "")
							  + "</select>"
							  + "</div>"
							  + (control.EnableTextSwitch.Enabled ? "<span ng-if=\"" + control.EnableTextSwitch.TextCondition + " && item." + control.ModelKey + " == " + control.Model
																  + "\" ng-repeat=\"item in " + control.EnableTextSwitch.Model + "\">{{item." + control.ModelValue + "}}</span>" : "")
							  + (control.ErrorMessageInside ? BuildErrorMessageBlock(control) : "")
							  + "</div>"
							  + (control.ErrorMessageInside ? "" : BuildErrorMessageBlock(control));
			}
			else
			{
				div.InnerHtml = "<div class=\"form__group\">" +
									"<label class=\"form__label\""
									+ (control.EnableTextSwitch.Enabled && control.EnableTextSwitch.HideLabel ? " ng-if=\"" + control.EnableTextSwitch.EditCondition + "\"" : "")
									+ ">" + control.Label + "</label>" +
									(control.Description == null ? "" : "<p>" + control.Description + "</p>") +
									"<div class=\"form__control\">"
									+ "<div class=\"selectmenu\"" + (control.EnableTextSwitch.Enabled ? " ng-if=\"" + control.EnableTextSwitch.EditCondition + "\"" : "") + ">" +
										 "<select " + " class=\"form-control\" ng-model=\"" + control.Model
											   + "\" ng-options=\"" + control.Options + "\"" + (control.OnChange == null ? "" : " ng-change=\"" + control.OnChange + "\"")
											   + (control.CustomAttributes == null ? "" : " " + control.CustomAttributes)
											   + (control.DefaultOverride ? "ng-init=\"" + control.Model + "=null\"" : "")
											   + "ng-class=\"{'has-error':" + control.ErrorMessageModel + "ErrorMessage}\"" + ">"
											   + (control.ShowDefaultPlaceHolder ? "<option value=\"\" selected>"
											   + (string.IsNullOrEmpty(control.Placeholder) ? "< Select value >" : control.Placeholder) + "</option>" : "") +
										 "</select>"
									+ "</div>"
									+ (control.EnableTextSwitch.Enabled ? "<span ng-if=\"" + control.EnableTextSwitch.TextCondition + " && item." + control.ModelKey + " == " + control.Model
																		+ "\" ng-repeat=\"item in " + control.EnableTextSwitch.Model + "\">{{item." + control.ModelValue + "}}</span>" : "") +
									"</div>"
								+ (control.ErrorMessageInside ? BuildErrorMessageBlock(control) : "") +
								"</div>"
								+ (control.ErrorMessageInside ? "" : BuildErrorMessageBlock(control));
			}

			return new MvcHtmlString(div.ToString());
		}

		public static MvcHtmlString DropDown(this HtmlHelper helper, FormControl control)
		{
			return new MvcHtmlString(BuildDropDown(control));
		}

		private static string BuildDropDown(FormControl control)
		{
			return "<div class=\"control-wrapper " + (control.Multiple ? "" : "selectmenu") + (control.CssClass == null ? "" : " " + control.CssClass) + "\"> <select "
				 + " class=\"" + (control.Size ?? "input--medium") + " form-control\" ng-model=\"" + control.Model + "\""
				 + (control.Multiple ? " multiple ng-multiple=\"true\" size=\"" + control.Lines + "\"" : "")
				 + " ng-options=\"" + control.Options + "\"" + (control.OnChange == null ? "" : " ng-change=\"" + control.OnChange + "\"")
				 + " ng-class=\"{'has-error':" + control.ErrorMessageModel + "ErrorMessage}\""
				 + (control.CustomAttributes == null ? "" : " " + control.CustomAttributes)
				 + ">"
				 + (control.ShowDefaultPlaceHolder ? "<option value=\"\" selected>" + (string.IsNullOrEmpty(control.Placeholder) ? "< Select value >" : control.Placeholder) + "</option>" : "")
				 + "</select></div>"
				 + BuildErrorMessageBlock(control);
		}

		public static string BuildLabel(FormControl control)
		{
			return "<div class=\"label-wrapper\"><label class=\"form__label\">" + control.Label + " </label></div>"
							 + BuildErrorMessageBlock(control);

		}

		public static MvcHtmlString TextBoxBlock(this HtmlHelper helper, FormControl control)
		{
			TagBuilder div = new TagBuilder("div");

			div.Attributes.Add("class", "form__group" + (control.CssClass == null ? "" : " " + control.CssClass) + (control.Required ? " required" : ""));
			div.Attributes.Add("ng-class", "{\"has-error\":" + control.Model + "Error}");

			if (control.HtmlAttributes != null)
				div.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(control.HtmlAttributes));

			div.InnerHtml = BuildTextBox(control);

			return new MvcHtmlString(div.ToString());
		}

		private static string BuildTextBox(FormControl control)
		{
			return (control.Label == null ? "" : "<div class=\"label-wrapper\""
				 + (control.EnableTextSwitch.Enabled && control.EnableTextSwitch.HideLabel ? " ng-if=\"" + control.EnableTextSwitch.EditCondition + "\"" : "") + "" +
				 "><label class=\"form__label\">" + control.Label + "</label></div>")
				 + (control.Description == null ? "" : "<p>" + control.Description + "</p>")
				 + "<div class=\"control-wrapper form-control\"> <input id=\"" + control.Id + "\" type=\"text\"" + " onchange = \"" + control.OnChange + "\" class=\"" + (control.Size ?? "input--medium") + "\" ng-model=\"" + control.Model + "\""
				 + "ng-class=\"{'has-error':" + control.ErrorMessageModel + "ErrorMessage}\"" + (control.Options == null ? "" : " " + control.Options)
				 + (control.Placeholder == null ? "" : " placeholder=\"" + control.Placeholder + "\"")
				 + (control.CustomAttributes == null ? "" : " " + control.CustomAttributes)
				 + (control.EnableTextSwitch.Enabled ? " ng-if=\"" + control.EnableTextSwitch.EditCondition + "\"" : "")
				 + " />"
				 + (control.EnableTextSwitch.Enabled ? "<span ng-if=\"" + control.EnableTextSwitch.TextCondition + "\">{{" + control.Model + "}}</span>" : "")
				 + (control.ErrorMessageInside ? BuildErrorMessageBlock(control) : "")
				 + "</div>"
				 + (control.ErrorMessageInside ? "" : BuildErrorMessageBlock(control));
		}

		public static MvcHtmlString TextBox(this HtmlHelper helper, string model, string errorMessageModel, string customClass = null, string customAttributes = null, string onChange = null, string disabled = null)
		{
			var html = "<input type =\"text\"" + " class=\"input--small" + (customClass == null ? "" : " " + customClass) + "\" ng-model=\"" + model + "\""
					 + " ng-class=\"{'has-error':" + errorMessageModel + "ErrorMessage}\""
					 + (customAttributes == null ? "" : " " + customAttributes)
					 + (string.IsNullOrEmpty(onChange) ? "" : " ng-change=\"" + onChange + "\"") + ""
					 + (string.IsNullOrEmpty(disabled) ? "" : " ng-disabled=\"" + disabled + "\"") + ""
					 + " />";
			return new MvcHtmlString(html);
		}

		public static MvcHtmlString TextBox(this HtmlHelper helper, FormControl control)
		{
			var html = "<input type =\"text\"" + " class=\"" + (control.Size ?? "input--medium")
					 + (control.CssClass == null ? "" : " " + control.CssClass) + "\" ng-model=\""
					 + control.Model + "\"" + (control.Id == null ? "" : " id=\"" + control.Id + "\"")
					 + " ng-class=\"{'has-error':" + control.ErrorMessageModel + "ErrorMessage}\""
					 + (control.Options == null ? "" : " " + control.Options)
					 + (control.Placeholder == null ? "" : " placeholder=\"" + control.Placeholder + "\"")
					 + (string.IsNullOrEmpty(control.OnChange) ? "" : " ng-change=\"" + control.OnChange + "\"")
					 + (control.EnableTextSwitch.Enabled ? " ng-if=\"" + control.EnableTextSwitch.EditCondition + "\"" : "")
					 + " />"
					 + (control.EnableTextSwitch.Enabled ? "<span ng-if=\"" + control.EnableTextSwitch.TextCondition + "\">{{" + control.Model + "}}</span>" : "");
			return new MvcHtmlString(html);
		}

		public static MvcHtmlString CheckBox(this HtmlHelper helper, FormControl control)
		{
			var html = "<input type =\"checkbox\" id=\"" + control.ModelName + "\""
					 + (control.Value == null ? "" : " ng-value=\"" + control.Value + "\"")
					 + " class=\"" + (control.CssClass == null ? "" : " " + control.CssClass) + "\" ng-model=\"" + control.Model + "\""
					 + " ng-class=\"{'has-error':" + control.ErrorMessageModel + "ErrorMessage}\""
					 + (control.Options == null ? "" : " " + control.Options)
					 + (control.CustomAttributes == null ? "" : " " + control.CustomAttributes)
					 + " /><label class=\"form__label\" for=\"" + control.ModelName + "\">" + control.Label + "</label>";
			return new MvcHtmlString(html);
		}

		public static MvcHtmlString CheckBoxWithCustomId(this HtmlHelper helper, FormControl control)
		{
			var html = "<input type =\"checkbox\" id=\"" + control.Id + "\""
					 + (control.Value == null ? "" : " ng-value=\"" + control.Value + "\"")
					 + " class=\"" + (control.CssClass == null ? "" : " " + control.CssClass) + "\" ng-model=\"" + control.Model + "\""
					 + " ng-class=\"{'has-error':" + control.ErrorMessageModel + "ErrorMessage}\""
					 + (control.Options == null ? "" : " " + control.Options)
					 + (control.CustomAttributes == null ? "" : " " + control.CustomAttributes)
					 + " /><label class=\"form__label\" for=\"" + control.Id + "\">" + control.Label + "</label>";
			return new MvcHtmlString(html);
		}

		public static MvcHtmlString RadioButton(this HtmlHelper helper, FormControl control)
		{
			var html = "<input type =\"radio\" id=\"" + control.Id + "\""
					 + (control.Value == null ? "" : " ng-value=\"" + control.Value + "\"")
					 + " class=\"" + (control.CssClass == null ? "" : " " + control.CssClass)
					 + "\" ng-model=\"" + control.Model + "\" name=\"" + control.Model +"\""
					 + (control.OnChange == null ? "" : " ng-change=\"" + control.OnChange) + "\""
					 + (control.EnableMultiSelectListModel.Enabled ? control.EnableMultiSelectListModel.Model : "") 
					 + " ng-class=\"{'has-error':" + control.ErrorMessageModel + "ErrorMessage}\""
					 + (control.Options == null ? "" : " " + control.Options)
					 + (control.CustomAttributes == null ? "" : " " + control.CustomAttributes)
					 + " /><label for=\"" + control.Id + "\">" + control.Label + "</label>";
			return new MvcHtmlString(html);
		}

		public static MvcHtmlString ValidationError(this HtmlHelper helper, string model, bool inner = false, string prefix = "model.")
		{
			var html = BuildErrorMessageBlock(new FormControl() { Model = model, IsInnerModel = inner, ErrorMessageModelPrefix = prefix });
			return new MvcHtmlString(html);
		}

		private static string BuildErrorMessageBlock(FormControl control, bool multiselect = false)
		{
			var errorMessage = GetErrorMessage(multiselect ? control.MultiSelectListModelName : control.Model, control.IsInnerModel, control.ErrorMessageModelPrefix);
			var result = $"<span ng-if=\"{errorMessage}\" class=\"field-validation-error\">{"{{" + errorMessage + "}}"}</span>";
			return result;
		}

		private static string GetErrorMessage(string model, bool inner, string prefix = "model.")
		{
			return (inner ? model.StartsWith(prefix) ? prefix + model.Substring(prefix.Length).Replace(".", "_") : model.Replace(".", "_") : model) + "ErrorMessage";
		}

		public static string JsonEncode(this HtmlHelper helper, object model)
		{
			return JsonConvert.SerializeObject(model, new JsonSerializerSettings()
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});
		}

		public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, Type controller, object routeValues = null, object htmlAttributes = null)
		{
			return htmlHelper.ActionLink(linkText, actionName, controller.Name.Replace("Controller", ""), routeValues, htmlAttributes);
		}

		public static string ActionUrl(this UrlHelper urlHelper, string actionName, Type controller, object routeValues = null)
		{
			return urlHelper.Action(actionName, controller.Name.Replace("Controller", ""), routeValues);
		}

		public static MvcHtmlString IncludeCanadaPostAutoComplete(this HtmlHelper helper)
		{
			var configuration = new CanadaPostConfiguration(new HttpContextWrapper(HttpContext.Current));
			var jsPath = configuration.GetJsPath();
			var cssPath = configuration.GetCssPath();

			return MvcHtmlString.Create($"<script type='text/javascript' src='{jsPath}'>{Environment.NewLine}</script><link rel='stylesheet' type='text/css' href='{cssPath}'/>");
		}
	}

	public class DatePickerFormControl
	{
		public string Id { get; set; }
		public string Label { get; set; }
		public string StartYear { get; set; }
		public string EndYear { get; set; }
		public string Description { get; set; }
		public string Date { get; set; }
		public string Year { get; set; }
		public string Month { get; set; }
		public string Day { get; set; }
		public bool IsRequired { get; set; }
		public bool IsDisabled { get; set; }
		public string ErrorMessageModelPrefix { get; set; } = "model.";
		public FormControl.TextSwitch EnableTextSwitch { get; set; }
		public string CustomAttributes { get; set; }
	}

	public class FormControl
	{
		public struct TextSwitch
		{
			public bool Enabled { get; set; }
			public string EditCondition { get; set; }
			public string TextCondition { get; set; }
			public string Model { get; set; }
			public bool HideLabel { get; set; }

			public TextSwitch(string condition, string model = null)
			{
				Enabled = true;
				EditCondition = condition;
				TextCondition = "!" + condition;
				Model = model;
				HideLabel = false;
			}
		}

		public struct MultiSelectListModel
		{
			public bool Enabled { get; set; }
			public string Model { get; set; }
			public string Value { get; set; }

			public MultiSelectListModel(string model, string value = null)
			{
				Enabled = true;
				Model = model;
				Value = value;
			}
		}

		public string Id { get; set; }
		public string Label { get; set; }
		public string Description { get; set; }
		public string Value { get; set; }
		public bool HideLabel { get; set; }

		public string Model { get; set; }
		public string ModelKey { get; set; } = "Key";
		public string ModelValue { get; set; } = "Value";
		public string ModelName
		{
			get
			{
				return Model?.Replace(".", "_");
			}
		}

		public string MultiSelectListModelName
		{
			get
			{
				return EnableMultiSelectListModel.Enabled ? EnableMultiSelectListModel.Model : Model;
			}
		}

		public bool ErrorMessageInside { get; set; }
		public string ErrorMessageModelPrefix { get; set; } = "model.";
		private string _ErrorMessageModel = null;

		public string ErrorMessageModel
		{
			get
			{
				return _ErrorMessageModel ?? (IsInnerModel ? Model.StartsWith(ErrorMessageModelPrefix) ?
											  ErrorMessageModelPrefix + Model.Substring(ErrorMessageModelPrefix.Length).Replace(".", "_") : ModelName : Model);
			}
			set
			{
				_ErrorMessageModel = value;
			}
		}

		public bool IsInnerModel { get; set; }
		public string Options { get; set; }
		public string OnChange { get; set; }
		public bool Required { get; set; }
		public string CssClass { get; set; }
		public object HtmlAttributes { get; set; }
		public bool Inline { get; set; } = true;
		public string Format { get; set; }
		public string Size { get; set; }

		public string CustomAttributes { get; set; }
		public bool ShowDefaultPlaceHolder { get; set; } = true;
		public string Placeholder { get; set; }
		public bool DefaultOverride { get; set; } = false;

		public TextSwitch EnableTextSwitch { get; set; }
		public MultiSelectListModel EnableMultiSelectListModel { get; set; }
		public string MultiSelectListModelValue
		{
			get
			{
				return EnableMultiSelectListModel.Value ?? ModelKey;
			}
		}

		public bool Multiple { get; set; }
		public int Lines { get; set; } = 15;
	}
}
