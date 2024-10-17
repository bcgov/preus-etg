using System.Web;

namespace CJG.Application.Services.Web
{
    public class InternalHttpSessionState : HttpSessionStateBase
    {
        private object _tempDataObject;

        public override object this[string name]
        {
            get
            {
                return _tempDataObject;
            }
            set
            {
                _tempDataObject = value;
            }
        }

        public InternalHttpSessionState()
        {

        }
    }
}
