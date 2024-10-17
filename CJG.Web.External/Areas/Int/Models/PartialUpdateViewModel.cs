namespace CJG.Web.External.Areas.Int.Models
{
    public class PartialUpdateViewModel<TEntityId>
    {
        public TEntityId EntityId { get; set; }
        public bool Allow { get; set; }
        public string ServiceUrlPath { get; set; } = "/Int/Application/UpdateField";
        
    }
}