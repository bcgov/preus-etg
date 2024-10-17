using System.Security.Principal;
using System.Web;

namespace CJG.Application.Services.Web
{
    /// <summary>
    /// <typeparamref name="InternalHttpContext"/> class, provides a way to create a custom internal HttpContext object to be used in dependency injection.
    /// </summary>
    public class InternalHttpContext : HttpContextBase
    {
        #region Variables
        private readonly HttpSessionStateBase _session = new InternalHttpSessionState();
        #endregion

        #region Properties
        public override HttpRequestBase Request { get; }

        public override HttpResponseBase Response { get; }

        public override HttpServerUtilityBase Server { get; }

        public override HttpSessionStateBase Session
        {
            get { return _session; }
        }

        public override IPrincipal User { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="InternalHttpContext"/> class.
        /// </summary>
        /// <param name="httpContext"></param>
        public InternalHttpContext(HttpContextBase httpContext)
        {
            this.Request = httpContext.Request;
            this.Response = httpContext.Response;
            this.Server = httpContext.Server;
            this.User = httpContext.User;
        }
        #endregion
    }
}
