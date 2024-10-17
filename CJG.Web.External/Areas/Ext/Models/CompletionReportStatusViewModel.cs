using CJG.Core.Entities;
using System.ComponentModel;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class CompletionReportStatusViewModel
    {
        public string LabelClass { get; set; }

        public string StatusText { get; set; }

        public CompletionReportStatus CompletionReportStatus
        {
            set
            {
                switch(value)
                {
                    case CompletionReportStatus.NotStarted:
                    case CompletionReportStatus.Incomplete:
                    case CompletionReportStatus.Complete:
                        this.LabelClass = value.ToString().ToLower();
                        break;
                    case CompletionReportStatus.Submitted:
                        this.LabelClass = "complete";
                        break;
                    default:
                        break;
                }

                this.StatusText = value.GetDescription().ToUpper();
            }
        }
    }

    public enum CompletionReportStatus
    {
        [Description("Not Started")]
        NotStarted = 0,
        [Description("Incomplete")]
        Incomplete = 1,
        [Description("Complete")]
        Complete = 2,
        [Description("Submitted")]
        Submitted = 3
    }
}