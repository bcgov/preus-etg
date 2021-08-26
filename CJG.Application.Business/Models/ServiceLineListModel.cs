using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJG.Application.Business.Models
{
    public class ServiceLineListModel : CollectionItemModel
    {
        public string BreakdownCaption { get; set; }

        public ServiceLineListModel() : base()
        {

        }

        public ServiceLineListModel(LookupTable<int> lookup, string description, bool selected, string breakdownCaption) : base(lookup, description, selected)
        {
            this.BreakdownCaption = breakdownCaption;
        }

    }
}
