using CJG.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class DeliveryMethodSelectionViewModel
    {
        #region Properties
        public int Id { get; set; }

        public string Caption { get; set; }

        public bool IsActive { get; set; }

        public int RowSequence { get; set; }

        [Required(ErrorMessage = "Please select at least one primary delivery method")]
        public bool IsSelected { get; set; }
        #endregion

        #region Constructors
        public DeliveryMethodSelectionViewModel()
        {

        }

        public DeliveryMethodSelectionViewModel(DeliveryMethod deliveryMethod, bool isSelected = false)
        {
            this.Id = deliveryMethod.Id;
            this.Caption = deliveryMethod.Caption;
            this.IsActive = deliveryMethod.IsActive;
            this.RowSequence = deliveryMethod.RowSequence;
            this.IsSelected = isSelected;
        }
        #endregion
    }
}