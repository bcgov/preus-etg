using CJG.Core.Entities;
using System;
using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models
{
    public class DebugLogViewModel
    {
        #region Properties
        public int Page { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 50;
        public int Total { get; set; }
        public IEnumerable<Log> Logs { get; set; }

        public string Level { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public DateTime? DateAdded { get; set; }
        #endregion

        #region Constructors
        public DebugLogViewModel()
        {

        }

        public DebugLogViewModel(int page, int itemsPerPage, IEnumerable<Log> logs, int total)
        {
            this.Page = page;
            this.ItemsPerPage = itemsPerPage;
            this.Logs = logs;
            this.Total = total;
        }
        #endregion
    }
}