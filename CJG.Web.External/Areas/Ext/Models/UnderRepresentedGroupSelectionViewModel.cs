using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class UnderRepresentedGroupSelectionViewModel
    {
        #region Properties
        public int Id { get; set; }

        public string Caption { get; set; }

        public bool IsActive { get; set; }

        public int RowSequence { get; set; }

        public bool IsSelected { get; set; }
        #endregion

        #region Constructors
        public UnderRepresentedGroupSelectionViewModel()
        {

        }

        public UnderRepresentedGroupSelectionViewModel(UnderRepresentedGroup underRepresentGroup, bool isSelected = false)
        {
            this.Id = underRepresentGroup.Id;
            this.Caption = underRepresentGroup.Caption;
            this.IsActive = underRepresentGroup.IsActive;
            this.RowSequence = underRepresentGroup.RowSequence;
            this.IsSelected = isSelected;
        }
        #endregion
    }
}