using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Web.External.Models.Shared
{
    public class PageViewModel
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 1;
        public int Page { get; set; }

        public int NumberofRecordsShowing { get; set; }
        public int NumberOfPageButtons { get; set; } = 6;

        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }

        public int StartPage
        {
            get
            {
                if (Page == PageCount && Page > NumberOfPageButtons)
                {
                    return PageCount - NumberOfPageButtons + 1;
                }
                else
                {
                    return Math.Max(Page - NumberOfPageButtons + 2, 1);
                }
            }
        }

        public int EndPage
        {
            get
            {
                return Math.Max(1, Math.Min(StartPage + NumberOfPageButtons - 1, PageCount));
            }
        }

        public int Start
        {
            get
            {
                return (Page - 1) * PageSize + 1;
            }
        }

        public int End
        {
            get
            {
                return Start + NumberofRecordsShowing - 1;
            }
        }
    }
}