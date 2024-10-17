using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace CJG.Web.External.Helpers
{
	/// <summary>
	/// <typeparamref name="HtmlHelperExtensions"/> static class, provides extension methods for <typeparamref name="HtmlHelper"/> objects.
	/// </summary>
	public static class HtmlHelperExtensions
	{
		/// <summary>
		/// Add the current <typeparamref name="ModelState"/> error messages to the <typeparamref name="ViewDataDictionary"/> so that the partial view has the information too.
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="viewData"></param>
		/// <param name="prefix"></param>
		private static void AddModelStateToViewDataDictionary(this HtmlHelper helper, ViewDataDictionary viewData, string prefix)
		{
			foreach (string key in helper.ViewData.ModelState.Keys)
			{
				if (key.StartsWith(prefix + "."))
				{
					foreach (ModelError err in helper.ViewData.ModelState[key].Errors)
					{
						if (!string.IsNullOrEmpty(err.ErrorMessage))
							viewData.ModelState.AddModelError(key, err.ErrorMessage);
						if (err.Exception != null)
							viewData.ModelState.AddModelError(key, err.Exception);
					}
					viewData.ModelState.SetModelValue(key, helper.ViewData.ModelState[key].Value);
				}
			}
		}

		/// <summary>
		/// Renders the partial view and includes any <typeparamref name="ModelState"/> information from the parent view.
		/// </summary>
		/// <param name="helper"><typeparamref name="HtmlHelper"/> object.</param>
		/// <param name="partialViewName">The name or path to the partial view.</param>
		/// <param name="model">The model that will be passed to the partial view.</param>
		/// <param name="prefix">The default <typeparamref name="TemplateInfo"/>.HtmlFieldPrefix value.</param>
		public static void RenderPartialWithPrefix(this HtmlHelper helper, string partialViewName, object model, string prefix)
		{
			var vdd = new ViewDataDictionary(helper.ViewData) { TemplateInfo = new System.Web.Mvc.TemplateInfo { HtmlFieldPrefix = prefix } };

			helper.RenderPartialWithPrefix(partialViewName, model, vdd, prefix);
		}

		/// <summary>
		/// Renders the partial view and includes any <typeparamref name="ModelState"/> information from the parent view.
		/// </summary>
		/// <param name="helper"><typeparamref name="HtmlHelper"/> object.</param>
		/// <param name="partialViewName">The name or path to the partial view.</param>
		/// <param name="model">The model that will be passed to the partial view.</param>
		/// <param name="viewData"><typeparamref name="ViewDataDictionary"/> object to pass to the partial view.</param>
		/// <param name="prefix">The default <typeparamref name="TemplateInfo"/>.HtmlFieldPrefix value.</param>
		public static void RenderPartialWithPrefix(this HtmlHelper helper, string partialViewName, object model, ViewDataDictionary viewData, string prefix)
		{
			if (viewData != null)
			{
				viewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = prefix };
			}

			var vdd = viewData ?? new ViewDataDictionary(helper.ViewData) { TemplateInfo = new System.Web.Mvc.TemplateInfo { HtmlFieldPrefix = prefix } };

			helper.RenderPartial(partialViewName, model, vdd);
		}

		/// <summary>
		/// Renders the partial view and includes any <typeparamref name="ModelState"/> information from the parent view.
		/// </summary>
		/// <param name="helper"><typeparamref name="HtmlHelper"/> object.</param>
		/// <param name="partialViewName">The name or path to the partial view.</param>
		/// <param name="model">The model that will be passed to the partial view.</param>
		/// <param name="prefix">The default <typeparamref name="TemplateInfo"/>.HtmlFieldPrefix value.</param>
		public static MvcHtmlString PartialWithPrefix(this HtmlHelper helper, string partialViewName, object model, string prefix)
		{
			var vdd = new ViewDataDictionary(helper.ViewData) { TemplateInfo = new System.Web.Mvc.TemplateInfo { HtmlFieldPrefix = prefix } };

			return helper.PartialWithPrefix(partialViewName, model, vdd, prefix);
		}

		/// <summary>
		/// Renders the partial view and includes any <typeparamref name="ModelState"/> information from the parent view.
		/// </summary>
		/// <param name="helper"><typeparamref name="HtmlHelper"/> object.</param>
		/// <param name="partialViewName">The name or path to the partial view.</param>
		/// <param name="model">The model that will be passed to the partial view.</param>
		/// <param name="viewData"><typeparamref name="ViewDataDictionary"/> object to pass to the partial view.</param>
		/// <param name="prefix">The default <typeparamref name="TemplateInfo"/>.HtmlFieldPrefix value.</param>
		public static MvcHtmlString PartialWithPrefix(this HtmlHelper helper, string partialViewName, object model, ViewDataDictionary viewData, string prefix)
		{
			if (viewData != null)
			{
				viewData.TemplateInfo = new TemplateInfo { HtmlFieldPrefix = prefix };
			}

			var vdd = viewData ?? new ViewDataDictionary(helper.ViewData) { TemplateInfo = new System.Web.Mvc.TemplateInfo { HtmlFieldPrefix = prefix } };

			return helper.Partial(partialViewName, model, vdd);
		}

		public static MvcHtmlString SpanFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
		{
			var valueGetter = expression.Compile();
			var value = valueGetter(helper.ViewData.Model);

			var span = new TagBuilder("span");
			span.MergeAttributes(new RouteValueDictionary(htmlAttributes));
			if (value != null)
			{
				if (typeof(TProperty) == typeof(decimal))
				{
					span.SetInnerText(Convert.ToDecimal(value.ToString()).ToDollarCurrencyString());
				}
				else
				{
					span.SetInnerText(value.ToString());
				}
			}

			return MvcHtmlString.Create(span.ToString());
		}

		public static IHtmlString TextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> ex, object htmlAttributes, bool disabled)
		{
			var attributes = SetDisabledAttribute(htmlAttributes, disabled);
			return htmlHelper.TextAreaFor(ex, attributes);
		}

		public static IHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> ex, object htmlAttributes, bool disabled)
		{
			var attributes = SetDisabledAttribute(htmlAttributes, disabled);
			return htmlHelper.TextBoxFor(ex, attributes);
		}

		// public static IHtmlString Button(this HtmlHelper htmlHelper)

		private static RouteValueDictionary SetDisabledAttribute(object htmlAttributes, bool disabled)
		{
			var attributes = new RouteValueDictionary(htmlAttributes);
			if (disabled)
			{
				attributes["disabled"] = "disabled";
			}

			return attributes;
		}

		public static bool IsDebugBuild(this HtmlHelper htmlHelper)
		{
#if DEBUG
			return true;
#elif TEST
			return true;
#else
			return false;
#endif
		}
	}
}
