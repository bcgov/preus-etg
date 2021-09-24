using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Helpers.XEditable;

namespace CJG.Web.External.Helpers
{
    /// <summary>
    /// Set of HTML helpers that renders different types of x-editable controls from 
    /// https://vitalets.github.io/x-editable/ JS library
    /// </summary>
    public static class HtmlXEditableExtensions
    {
        public static readonly KeyValuePair<int, string>  DefaultNoneItem = 
            new KeyValuePair<int, string>(PartialEntityUpdateConstants.NoneKey, DropDownListHelper.SelectValueText);

        /// <summary>
        /// HTML helper for list with single selected item without "none" item. 
        /// The list is rendered as "radio" button group 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="primaryKey"></param>
        /// <param name="serverUpdateUrl"></param>
        /// <param name="fieldKey"></param>
        /// <param name="list"></param>
        /// <param name="selectedItemKey"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString XEditRadio(
            this HtmlHelper htmlHelper,
            int primaryKey,
            string serverUpdateUrl,
            string fieldKey,
            IList<KeyValuePair<int, string>> list,
            int selectedItemKey,
            object htmlAttributes)
        {
            return CreateHtml(new XEditableRadioRender(primaryKey, serverUpdateUrl, fieldKey, list, selectedItemKey,
                        HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
        }

        /// <summary>
        /// HTML helper for drop-down list with single selected value and 
        /// optional "none" item where key is number
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="primaryKey"></param>
        /// <param name="serverUpdateUrl"></param>
        /// <param name="fieldKey"></param>
        /// <param name="list"></param>
        /// <param name="selectedItemKey"></param>
        /// <param name="noneItem"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="showBlankInReadOnly"></param>
        /// <returns></returns>
        public static MvcHtmlString XEditSelect(
            this HtmlHelper htmlHelper,
            int primaryKey,
            string serverUpdateUrl,
            string fieldKey,
            IList<KeyValuePair<int, string>> list,
            int selectedItemKey,
            KeyValuePair<int, string>? noneItem,
            object htmlAttributes, bool showBlankInReadOnly = false)
        {
            return CreateHtml(new XEditableSelectRender<int>(primaryKey, serverUpdateUrl, fieldKey, list, selectedItemKey, 
                noneItem, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), showBlankInReadOnly));
        }
        
        /// <summary>
        /// HTML helper for drop-down list where items loaded on ajax call
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="primaryKey"></param>
        /// <param name="serverUpdateUrl"></param>
        /// <param name="fieldKey"></param>
        /// <param name="dataSourceUrl"></param>
        /// <param name="selectedItemKey"></param>
        /// <param name="selectedItemText"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString XEditSelect(
            this HtmlHelper htmlHelper,
            int primaryKey,
            string serverUpdateUrl,
            string fieldKey,
            string dataSourceUrl,
            string selectedItemKey,
            string selectedItemText,
            object htmlAttributes)
        {
            return CreateHtml(new XEditableSelectSourceRender(primaryKey, serverUpdateUrl, fieldKey, selectedItemKey, 
                selectedItemText, dataSourceUrl, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
        }

        /// <summary>
        /// HTML helper for checklist control
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="primaryKey"></param>
        /// <param name="serverUpdateUrl"></param>
        /// <param name="fieldKey"></param>
        /// <param name="list"></param>
        /// <param name="selectedItemKeys"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString XEditChecklist(
            this HtmlHelper htmlHelper,
            int primaryKey,
            string serverUpdateUrl,
            string fieldKey,
            IList<KeyValuePair<int, string>> list,
            int[] selectedItemKeys,
            object htmlAttributes)
        {
            return CreateHtml(new XEditableChecklistRender(primaryKey, serverUpdateUrl, fieldKey, list, selectedItemKeys,
                        HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));

        }

        /// <summary>
        /// HTML helper for single text value control
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="primaryKey"></param>
        /// <param name="serverUpdateUrl"></param>
        /// <param name="fieldKey"></param>
        /// <param name="value"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString XEditText(
            this HtmlHelper htmlHelper,
            int primaryKey,
            string serverUpdateUrl,
            string fieldKey,
            string value,
            object htmlAttributes)
        {
            return CreateHtml(new XEditableTextRender(primaryKey, serverUpdateUrl, fieldKey, value,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
        }

        /// <summary>
        /// HTML helper for single text value control that represents phone number
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="primaryKey"></param>
        /// <param name="serverUpdateUrl"></param>
        /// <param name="fieldKey"></param>
        /// <param name="value"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString XEditPhone(
            this HtmlHelper htmlHelper,
            int primaryKey,
            string serverUpdateUrl,
            string fieldKey,
            string value,
            object htmlAttributes)
        {
            return CreateHtml(new XEditablePhoneRender(primaryKey, serverUpdateUrl, fieldKey, value,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
        }

        /// <summary>
        /// HTML helper for date
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="primaryKey"></param>
        /// <param name="serverUpdateUrl"></param>
        /// <param name="fieldKey"></param>
        /// <param name="value"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString XEditDate(
            this HtmlHelper htmlHelper,
            int primaryKey,
            string serverUpdateUrl,
            string fieldKey,
            DateTime? value,
            object htmlAttributes)
        {
            return  CreateHtml(new XEditableDateRender(primaryKey, serverUpdateUrl, fieldKey, value,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));
        }

        /// <summary>
        /// HTML helper for postal address
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="primaryKey"></param>
        /// <param name="serverUpdateUrl"></param>
        /// <param name="fieldKey"></param>
        /// <param name="addressModel"></param>
        /// <param name="htmlAttributes"></param>
        /// <param name="provinces"></param>
        /// <param name="countries"></param>
        /// <returns></returns>
        public static MvcHtmlString XEditAddressExtension(
            this HtmlHelper htmlHelper,
            int primaryKey,
            string serverUpdateUrl,
            string fieldKey,
            AddressViewModel addressModel,
            IList<KeyValuePair<string, string>> provinces,
            IList<KeyValuePair<string, string>> countries,
            object htmlAttributes)
        {
            return CreateHtml(new XEditableAddressExtensionRender(primaryKey, serverUpdateUrl, fieldKey, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), addressModel, provinces, countries, "address" ));
        }

        public static MvcHtmlString XEditNaics(
            this HtmlHelper htmlHelper,
            int primaryKey,
            string serverUpdateUrl,
            string fieldKey,
            IList<KeyValuePair<int?, string>> naicsList,
            object htmlAttributes = null)
        {
            return CreateHtml(new XEditableNaicsRender(primaryKey, serverUpdateUrl, fieldKey, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), naicsList));
        }

        private static MvcHtmlString CreateHtml(ICustomControlProvider controlProvider)
        {
            return controlProvider.CreateHtml();
        }

        public static IList<KeyValuePair<int, string>> YesNoList { get; set; } = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(Convert.ToInt32(true), "Yes"),
                new KeyValuePair<int, string>(Convert.ToInt32(false), "No")
            };
    }
}