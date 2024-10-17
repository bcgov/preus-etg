using System.ComponentModel.DataAnnotations;

namespace CJG.Core.Entities.Attributes
{
    public class NameValidationAttribute : RegularExpressionAttribute
    {
        public NameValidationAttribute(string prefix = null) : base("^[a-zA-Z- '\"]*$")
        {
            ErrorMessage = prefix + "Names must not contain numbers or special characters";
        }
    }
}
