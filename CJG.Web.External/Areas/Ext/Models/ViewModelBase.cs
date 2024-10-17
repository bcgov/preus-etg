using CJG.Web.External.Areas.Int.Models;

namespace CJG.Web.External.Areas.Ext.Models
{
    public abstract class ViewModelBase
    {
        public PartialUpdateViewModel<int> PartialUpdate { get; set; }

        protected ViewModelBase()
        {
            PartialUpdate = new PartialUpdateViewModel<int>();
        }
    }
}