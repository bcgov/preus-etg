using System.Collections.Generic;

namespace CJG.Web.External.Areas.Int.Models
{
    public class LoginViewModel
    {
        public List<KeyValuePair<string, string>> UserList { get; set; }

        public string SelectedUser { get; set; }
    }
}